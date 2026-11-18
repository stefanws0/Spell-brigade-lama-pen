namespace SpellBrigadeLamaPen.Core;

using System.Collections.Generic;

/// <summary>
/// Safety net for the walk into the pen: the game's walking state only goes idle within 1 unit of its target and has no
/// timeout, so a spot the lama can't reach (a prop in the pen, a height step) would leave it walking in place forever.
/// A lama still walking <see cref="MaxWalkSeconds"/> after its decharm is stopped once; a lama that left the walking
/// state (arrived) is forgotten, so its later wandering is the game's business. A despawning lama, or one this PC
/// doesn't control, is never stopped and is forgotten too. <see cref="Clear"/> per objective.
/// </summary>
internal sealed class WalkWatchdog
{
    public const float MaxWalkSeconds = 10f;

    private readonly Dictionary<int, float> _since = new();

    public void Started(int lamaId, float now) => _since[lamaId] = now;

    /// <summary>True once, when this lama has been walking longer than <see cref="MaxWalkSeconds"/> since it was sent.</summary>
    public bool ShouldStop(LamaInfo lama, float now)
    {
        if (!_since.TryGetValue(lama.Id, out var since)) return false;
        if (lama.State != StateNames.Walking || lama.Despawning || !lama.IsOwner)
        {
            _since.Remove(lama.Id);
            return false;
        }

        if (now - since < MaxWalkSeconds) return false;
        _since.Remove(lama.Id);
        return true;
    }

    public void Clear() => _since.Clear();
}
