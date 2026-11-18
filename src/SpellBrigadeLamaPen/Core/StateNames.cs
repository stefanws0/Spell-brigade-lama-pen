namespace SpellBrigadeLamaPen.Core;

using System;

/// <summary>
/// Short names for the lama's state-machine classes: the game's <c>HerdingTargetFollowState</c> becomes "Follow".
/// Fed with the IL2CPP type name of <c>HerdingTarget.currentState</c>.
/// </summary>
internal static class StateNames
{
    private const string Prefix = "HerdingTarget";
    private const string Suffix = "State";

    /// <summary>The short name of the game's follow state (what a charmed lama does while following a wizard).</summary>
    public const string Follow = "Follow";

    public static string Short(string? typeName)
    {
        if (string.IsNullOrEmpty(typeName)) return "none";
        var name = typeName!;
        var middle = name.Length - Prefix.Length - Suffix.Length;
        if (middle > 0 && name.StartsWith(Prefix, StringComparison.Ordinal) && name.EndsWith(Suffix, StringComparison.Ordinal))
            return name.Substring(Prefix.Length, middle);
        return name;
    }
}
