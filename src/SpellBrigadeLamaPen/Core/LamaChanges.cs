namespace SpellBrigadeLamaPen.Core;

using System.Collections.Generic;

/// <summary>
/// One log line per lama when it first shows up and whenever its <see cref="LamaInfo.Summary"/> changes, so the log
/// shows what each lama did without a line every tick. <see cref="Clear"/> when the objective ends.
/// </summary>
internal sealed class LamaChanges
{
    private readonly Dictionary<int, string> _last = new();

    /// <summary>The log line for this sighting, or null if nothing changed since the last one.</summary>
    public string? Describe(LamaInfo lama)
    {
        var now = lama.Summary();
        if (!_last.TryGetValue(lama.Id, out var before))
        {
            _last[lama.Id] = now;
            return $"lama {lama.Id}: new, {now}";
        }

        if (before == now) return null;
        _last[lama.Id] = now;
        return $"lama {lama.Id}: {before} -> {now}";
    }

    public void Clear() => _last.Clear();
}
