namespace SpellBrigadeLamaPen.Core;

/// <summary>
/// Which lamas lose their charm: charmed ones inside the pen, on the PC that runs their AI (the host), not while the
/// objective is despawning them, and each one only once. What it is doing right now doesn't matter.
/// </summary>
internal static class DecharmRule
{
    public static bool ShouldDecharm(LamaInfo lama, bool alreadyDecharmed) =>
        lama.IsOwner && lama.InPen && lama.Charmed && !lama.Despawning && !alreadyDecharmed;
}
