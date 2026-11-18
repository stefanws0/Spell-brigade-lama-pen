namespace SpellBrigadeLamaPen.Game;

using System;
using System.Collections.Generic;
using SpellBrigadeLamaPen.Core;
using UObject = UnityEngine.Object;

/// <summary>
/// Every 0.25 s: find the running herding objective, read each lama, log what changed, and decharm charmed lamas that
/// are inside the pen. Patch-free: it only reads the game's fields and calls the game's own methods. All state is
/// static (the injected MonoBehaviour stays field-free) and is forgotten when the objective ends.
/// </summary>
internal static class LamaWatcher
{
    private const float TickSeconds = 0.25f;
    private const float LookupSeconds = 2f;

    private static readonly HashSet<int> Decharmed = new();
    private static readonly LamaChanges Changes = new();
    private static readonly WalkWatchdog Walks = new();
    private static HerdingObjectiveManager? _herd;
    private static float _nextTick;
    private static float _nextLookup;

    /// <summary>Called every frame by <see cref="LamaPenRunner"/>.</summary>
    public static void Update(float now)
    {
        if (now < _nextTick) return;
        _nextTick = now + TickSeconds;
        if (!ModConfig.Enabled.Value) return;

        var herd = CurrentObjective(now);
        if (herd == null) return;
        var list = herd.currentHerdingTargets;
        if (list == null) return;
        for (int i = 0; i < list.Count; i++)
        {
            var target = list[i];
            if (target == null || !target.isActiveAndEnabled) continue;
            Visit(target, now);
        }
    }

    /// <summary>
    /// Test hotkey Ctrl+2: charm every lama of the running objective that is still outside the pen
    /// (<see cref="TestCharmRule"/>), so the decharm can be tested without waiting for the game's charmed variant.
    /// </summary>
    public static void CharmForTest(float now)
    {
        _nextLookup = 0f; // look now, even if the objective was started a moment ago
        var herd = CurrentObjective(now);
        if (herd == null)
        {
            ModLog.Info("Ctrl+2: no running herding objective.");
            return;
        }

        var list = herd.currentHerdingTargets;
        if (list == null)
        {
            ModLog.Info("Ctrl+2: the herding objective has no lama list yet; press again in a moment.");
            return;
        }

        int charmed = 0;
        for (int i = 0; i < list.Count; i++)
        {
            var target = list[i];
            if (target == null || !target.isActiveAndEnabled) continue;
            try
            {
                ModLog.StepOnce("read", "first read of a HerdingTarget");
                var lama = LamaSnapshot.Read(target);
                ModLog.StepOnce("read-done", "first read of a HerdingTarget done");
                if (!TestCharmRule.ShouldCharm(lama)) continue;
                ModLog.StepOnce("test-charm", "first test charm: SetWillFollowHerders(true) + SetIsLoving(true)");
                target.SetWillFollowHerders(true);
                var animation = target.GetAnimationPlayer();
                if (animation != null) animation.SetIsLoving(true); // hearts, so you can see which lamas were charmed
                ModLog.StepOnce("test-charm-ok", "first test charm ok");
                Decharmed.Remove(lama.Id); // charmed again, so it can be decharmed again
                charmed++;
            }
            catch (Exception ex)
            {
                ModLog.Warn($"Ctrl+2: charming a lama failed: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
            }
        }

        ModLog.Info($"Ctrl+2: charmed {charmed} of {list.Count} lama(s); the others are in the pen, already charmed or " +
                    "not ours. No lamas yet? They spawn a moment after the objective starts: press again.");
    }

    /// <summary>The running herding objective, or null. Looks for a new one at most every 2 s.</summary>
    private static HerdingObjectiveManager? CurrentObjective(float now)
    {
        if (Running(_herd)) return _herd;
        if (_herd is not null) ObjectiveEnded(); // `is not null`: our reference, not Unity's "destroyed" check

        if (now < _nextLookup) return null;
        _nextLookup = now + LookupSeconds;
        ModLog.StepOnce("lookup", "first FindObjectsOfType<HerdingObjectiveManager>");
        var all = UObject.FindObjectsOfType<HerdingObjectiveManager>();
        ModLog.StepOnce("lookup-done", "first FindObjectsOfType<HerdingObjectiveManager> done");
        HerdingObjectiveManager? found = null;
        if (all != null)
        {
            for (int i = 0; i < all.Length; i++)
            {
                if (!Running(all[i])) continue;
                found = all[i];
                break;
            }
        }

        if (found == null) return null;
        _herd = found;
        ModLog.Info("Herding objective found.");
        return _herd;
    }

    private static bool Running(HerdingObjectiveManager? herd) =>
        herd != null && herd.isActiveAndEnabled && !herd.isDespawning;

    private static void ObjectiveEnded()
    {
        ModLog.Info($"Herding objective ended ({Decharmed.Count} lama(s) decharmed).");
        _herd = null;
        Decharmed.Clear();
        Changes.Clear();
        Walks.Clear();
    }

    private static void Visit(HerdingTarget target, float now)
    {
        LamaInfo lama;
        try
        {
            ModLog.StepOnce("read", "first read of a HerdingTarget");
            lama = LamaSnapshot.Read(target);
            ModLog.StepOnce("read-done", "first read of a HerdingTarget done");
        }
        catch (Exception ex)
        {
            ModLog.ErrorThrottled("read-lama", ex, now);
            return;
        }

        if (ModConfig.LogLamas.Value)
        {
            var change = Changes.Describe(lama);
            if (change != null) ModLog.Info(change);
        }

        if (!lama.IsOwner)
            ModLog.InfoOnce("not-host", "Not the host: the lamas' AI runs on the host's PC, so this mod leaves them alone here.");

        if (Walks.ShouldStop(lama.Id, lama.State, now)) StopStuckWalk(target, lama.Id);

        if (!DecharmRule.ShouldDecharm(lama, Decharmed.Contains(lama.Id))) return;
        Decharmed.Add(lama.Id); // before the calls, so a failing decharm is not retried every tick
        try
        {
            var ok = Decharmer.Decharm(target, lama);
            Walks.Started(lama.Id, now); // forgotten again as soon as the lama is seen in any other state
            ModLog.Info($"lama {lama.Id}: decharmed in the pen (was {lama.State}){(ok ? "" : ", with errors (see the WARN lines above)")}.");
        }
        catch (Exception ex)
        {
            ModLog.ErrorThrottled("decharm", ex, now);
        }
    }

    /// <summary>The <see cref="WalkWatchdog"/> fired: the walk-in spot was probably unreachable. Let the lama idle.</summary>
    private static void StopStuckWalk(HerdingTarget target, int lamaId)
    {
        try
        {
            ModLog.StepOnce("stuck-walk", "first stuck walk: TransitionToIdleState()");
            target.TransitionToIdleState();
            ModLog.StepOnce("stuck-walk-ok", "first stuck walk: TransitionToIdleState() ok");
            ModLog.Info($"lama {lamaId}: still walking {WalkWatchdog.MaxWalkSeconds:0} s after its decharm, so it was " +
                        "stopped (its spot was probably unreachable).");
        }
        catch (Exception ex)
        {
            ModLog.Warn($"lama {lamaId}: stopping a stuck walk failed: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
        }
    }
}
