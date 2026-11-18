namespace SpellBrigadeLamaPen;

using BepInEx.Configuration;

/// <summary>BepInEx/config/com.stefanws0.spellbrigadelamapen.cfg</summary>
internal static class ModConfig
{
    public static ConfigEntry<bool> Enabled = null!;
    public static ConfigEntry<bool> LogLamas = null!;
    public static ConfigEntry<bool> TestHotkeys = null!;

    public static void Init(ConfigFile cfg)
    {
        Enabled = cfg.Bind("General", "Enabled", true,
            "Charmed lamas lose their charm once they are inside the herding pen, so they stop following you back out. " +
            "Only works for the host. Can be switched off while playing.");
        LogLamas = cfg.Bind("Debug", "LogLamas", false,
            "Write a line to BepInEx/SpellBrigadeLamaPen.log whenever a lama changes what it is doing. Handy for bug " +
            "reports. Decharms are always logged.");
        TestHotkeys = cfg.Bind("Debug", "TestHotkeys", false,
            "For testing, as host inside a run: Ctrl+1 starts a herding objective (when no objective is running), " +
            "Ctrl+2 charms that objective's lamas that are still outside the pen.");
    }
}
