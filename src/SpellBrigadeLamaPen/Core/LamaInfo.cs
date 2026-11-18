namespace SpellBrigadeLamaPen.Core;

/// <summary>What the mod knows about one lama (the game's <c>HerdingTarget</c>) at one moment. Plain data, no game types.</summary>
internal sealed class LamaInfo
{
    /// <summary>Unity instance id: unique per lama for the whole game session.</summary>
    public int Id;

    /// <summary>This PC runs the lama's AI (the host). Only then can the mod change it.</summary>
    public bool IsOwner;

    /// <summary>The game counts it as inside the pen (<c>HerdingTarget.IsInZone</c>).</summary>
    public bool InPen;

    /// <summary>It follows wizards (<c>HerdingTarget.WillFollowHerders</c>): the charm.</summary>
    public bool Charmed;

    /// <summary>The objective is ending and the lama is on its way out (<c>HerdingTarget.isDespawning</c>).</summary>
    public bool Despawning;

    /// <summary>Short state name, see <see cref="StateNames"/>.</summary>
    public string State = "";

    public string Summary() =>
        $"{State}, {(InPen ? "in pen" : "outside")}, {(Charmed ? "charmed" : "not charmed")}" +
        (IsOwner ? "" : ", not host") +
        (Despawning ? ", despawning" : "");
}
