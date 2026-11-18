namespace SpellBrigadeLamaPen.Game;

using SpellBrigadeLamaPen.Core;

/// <summary>Reads one of the game's lamas (<c>HerdingTarget</c>) into a plain <see cref="LamaInfo"/>. Read-only.</summary>
internal static class LamaSnapshot
{
    public static LamaInfo Read(HerdingTarget target)
    {
        var state = target.currentState;
        return new LamaInfo
        {
            Id = target.GetInstanceID(),
            IsOwner = target.IsOwner,
            InPen = target.IsInZone,
            Charmed = target.WillFollowHerders,
            Despawning = target.isDespawning,
            State = StateNames.Short(state == null ? null : state.GetIl2CppType().Name),
        };
    }
}
