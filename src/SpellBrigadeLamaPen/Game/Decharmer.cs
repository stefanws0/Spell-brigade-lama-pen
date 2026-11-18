namespace SpellBrigadeLamaPen.Game;

using System;
using SpellBrigadeLamaPen.Core;

/// <summary>
/// Takes one lama's charm off with the game's own methods (called, never patched). Only call it on the host
/// (<see cref="DecharmRule"/> checks <c>IsOwner</c>): the lama's AI runs there.
/// </summary>
internal static class Decharmer
{
    /// <summary>True if every step ran without an exception (failures are logged as WARN lines with the lama id).</summary>
    public static bool Decharm(HerdingTarget target, LamaInfo before)
    {
        var ok = Run(before.Id, "SetWillFollowHerders(false)", () => target.SetWillFollowHerders(false));

        // A lama that is following someone right now keeps following until its state changes.
        if (before.State == StateNames.Follow)
            ok &= Run(before.Id, "TransitionToIdleState()", () => target.TransitionToIdleState());

        // The hearts / "loving" animation.
        ok &= Run(before.Id, "GetAnimationPlayer().SetIsLoving(false)", () =>
        {
            var animation = target.GetAnimationPlayer();
            if (animation != null) animation.SetIsLoving(false);
        });

        ModLog.StepOnce("decharm-done", "first decharm done");
        return ok;
    }

    private static bool Run(int lamaId, string step, Action call)
    {
        ModLog.StepOnce("decharm " + step, "first decharm: " + step);
        try
        {
            call();
            return true;
        }
        catch (Exception ex)
        {
            ModLog.Warn($"lama {lamaId}: {step} failed: {ex.GetType().Name}: {ex.Message}");
            return false;
        }
    }
}
