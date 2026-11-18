namespace SpellBrigadeLamaPen.Game;

using System;
using SpellBrigadeLamaPen.Core;
using Vector3 = UnityEngine.Vector3;

/// <summary>
/// Takes one lama's charm off with the game's own methods (called, never patched), then walks it further into the pen.
/// Only call it on the host (<see cref="DecharmRule"/> checks <c>IsOwner</c>): the lama's AI runs there.
/// </summary>
internal static class Decharmer
{
    private static readonly Random Rng = new();

    /// <summary>True if every step ran without an exception (failures are logged as WARN lines with the lama id).</summary>
    public static bool Decharm(HerdingTarget target, LamaInfo before)
    {
        var ok = Run(before.Id, "SetWillFollowHerders(false)", () => target.SetWillFollowHerders(false));

        // Ends any following too: the walking state replaces the follow state.
        var walking = Run(before.Id, "walk into the pen", () => WalkIntoPen(target, before.Id));
        ok &= walking;

        // Fallback if the walk failed while the lama is still following (it would keep following until its state
        // changes). If it did get into the walking state, leave it there: the game's own in-pen target takes it inward.
        if (!walking)
        {
            ok &= Run(before.Id, "TransitionToIdleState() fallback", () =>
            {
                if (LamaSnapshot.Read(target).State == StateNames.Follow) target.TransitionToIdleState();
            });
        }

        // The hearts / "loving" animation.
        ok &= Run(before.Id, "GetAnimationPlayer().SetIsLoving(false)", () =>
        {
            var animation = target.GetAnimationPlayer();
            if (animation != null) animation.SetIsLoving(false);
        });

        return ok;
    }

    /// <summary>
    /// Walks the lama to a <see cref="PenSpot"/> through the game's own walking state (what the game itself does for a
    /// normal lama entering the pen, but with our spot). On the host the transition is immediate; the state's
    /// <c>Enter</c> picks its own target first, then we override it: the state goes idle by itself within 1 unit of its
    /// <c>targetPosition</c>, so that is set to our spot too. <see cref="WalkWatchdog"/> catches an unreachable spot.
    /// </summary>
    private static void WalkIntoPen(HerdingTarget target, int lamaId)
    {
        var center = target.GetZonePosition();
        var position = target.transform.position;
        var radius = HerdingObjectiveManager.ZoneRadius;
        var spot = PenSpot.Pick(new FlatPoint(position.x, position.z), new FlatPoint(center.x, center.z), radius,
            (float)Rng.NextDouble(), (float)Rng.NextDouble());
        var destination = new Vector3(spot.X, position.y, spot.Z);

        ModLog.StepOnce("walk-transition", "first walk: TransitionToWalkingState()");
        target.TransitionToWalkingState();
        ModLog.StepOnce("walk-transition-ok", "first walk: TransitionToWalkingState() ok");
        var state = target.currentState;
        var walkingState = state == null ? null : state.TryCast<HerdingTargetWalkingState>();
        if (walkingState == null)
            throw new InvalidOperationException("the lama is not in the walking state after TransitionToWalkingState()");

        ModLog.StepOnce("walk-target", "first walk: walking state targetPosition");
        walkingState.targetPosition = destination;
        ModLog.StepOnce("walk-target-ok", "first walk: walking state targetPosition ok");
        ModLog.StepOnce("walk-towards", "first walk: WalkTowards()");
        target.WalkTowards(destination);
        ModLog.StepOnce("walk-towards-ok", "first walk: WalkTowards() ok");

        var fromCenter = MathF.Sqrt((spot.X - center.x) * (spot.X - center.x) + (spot.Z - center.z) * (spot.Z - center.z));
        ModLog.Info($"lama {lamaId}: walking to a spot {fromCenter:0.0} from the pen center (pen radius {radius:0.0}).");
    }

    private static bool Run(int lamaId, string step, Action call)
    {
        ModLog.StepOnce("decharm " + step, "first decharm: " + step);
        try
        {
            call();
            ModLog.StepOnce("decharm-ok " + step, "first decharm: " + step + " ok");
            return true;
        }
        catch (Exception ex)
        {
            ModLog.Warn($"lama {lamaId}: {step} failed: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
            return false;
        }
    }
}
