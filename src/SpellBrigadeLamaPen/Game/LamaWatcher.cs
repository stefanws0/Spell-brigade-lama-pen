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

    /// <summary>The running herding objective, or null. Looks for a new one at most every 2 s.</summary>
    private static HerdingObjectiveManager? CurrentObjective(float now)
    {
        if (Running(_herd)) return _herd;
        if (_herd is not null) ObjectiveEnded(); // `is not null`: our reference, not Unity's "destroyed" check

        if (now < _nextLookup) return null;
        _nextLookup = now + LookupSeconds;
        ModLog.StepOnce("lookup", "first FindObjectOfType<HerdingObjectiveManager>");
        var found = UObject.FindObjectOfType<HerdingObjectiveManager>();
        if (!Running(found)) return null;

        _herd = found;
        ModLog.Info($"Herding objective found: {found!.currentHerdingTargets?.Count ?? 0} lama(s).");
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
    }

    private static void Visit(HerdingTarget target, float now)
    {
        LamaInfo lama;
        try
        {
            ModLog.StepOnce("read", "first read of a HerdingTarget");
            lama = LamaSnapshot.Read(target);
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

        if (!DecharmRule.ShouldDecharm(lama, Decharmed.Contains(lama.Id))) return;
        Decharmed.Add(lama.Id); // before the calls, so a failing decharm is not retried every tick
        try
        {
            Decharmer.Decharm(target, lama);
            ModLog.Info($"lama {lama.Id}: decharmed in the pen (was {lama.State}).");
        }
        catch (Exception ex)
        {
            ModLog.ErrorThrottled("decharm", ex, now);
        }
    }
}
