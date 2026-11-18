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
            "Charmed Mana Beasts (lamas) lose their charm once they are inside the herding pen, so they stop following " +
            "you back out. Only has an effect on the host's PC (the lamas' AI runs there). Checked every 0.25 s, so it can be switched " +
            "off while playing.");
        LogLamas = cfg.Bind("Debug", "LogLamas", true,
            "Write a line to BepInEx/SpellBrigadeLamaPen.log when a lama first shows up and whenever its state, pen or " +
            "charm changes. Decharms are always logged.");
        TestHotkeys = cfg.Bind("Debug", "TestHotkeys", true,
            "For testing, host only, inside a run: Ctrl+1 starts a herding objective (when no objective is running); " +
            "Ctrl+2 charms that objective's lamas that are still outside the pen. Turn off for normal play.");
    }
}
