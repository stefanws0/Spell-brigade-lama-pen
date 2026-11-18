namespace SpellBrigadeLamaPen.Core;

/// <summary>
/// Test hotkey Ctrl+2: which lamas get charmed so the decharm can be tested without waiting for the game's charmed
/// variant. Lamas you can still lead in: on the host, outside the pen, not charmed yet, not despawning.
/// </summary>
internal static class TestCharmRule
{
    public static bool ShouldCharm(LamaInfo lama) => lama.IsOwner && !lama.InPen && !lama.Charmed && !lama.Despawning;
}
