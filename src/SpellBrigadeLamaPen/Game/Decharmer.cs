namespace SpellBrigadeLamaPen.Game;

using SpellBrigadeLamaPen.Core;

/// <summary>
/// Takes one lama's charm off with the game's own methods (called, never patched). Only call it on the host
/// (<see cref="DecharmRule"/> checks <c>IsOwner</c>): the lama's AI runs there.
/// </summary>
internal static class Decharmer
{
    public static void Decharm(HerdingTarget target, LamaInfo before)
    {
        ModLog.StepOnce("decharm-flag", "first decharm: SetWillFollowHerders(false)");
        target.SetWillFollowHerders(false);

        // A lama that is following someone right now keeps following until its state changes.
        if (before.State == StateNames.Follow)
        {
            ModLog.StepOnce("decharm-idle", "first decharm: TransitionToIdleState()");
            target.TransitionToIdleState();
        }

        // The hearts / "loving" animation.
        ModLog.StepOnce("decharm-loving", "first decharm: GetAnimationPlayer().SetIsLoving(false)");
        var animation = target.GetAnimationPlayer();
        if (animation != null) animation.SetIsLoving(false);
    }
}
