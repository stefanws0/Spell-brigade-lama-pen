namespace SpellBrigadeLamaPen;

using BepInEx.Configuration;

/// <summary>BepInEx/config/com.stefanws0.spellbrigadelamapen.cfg</summary>
internal static class ModConfig
{
    public static ConfigEntry<bool> Enabled = null!;
    public static ConfigEntry<bool> LogLamas = null!;

    public static void Init(ConfigFile cfg)
    {
        Enabled = cfg.Bind("General", "Enabled", true,
            "Charmed Mana Beasts (lamas) lose their charm once they are inside the herding pen, so they stop following " +
            "you back out. Only has an effect on the host's PC (the lamas' AI runs there). Read every check.");
        LogLamas = cfg.Bind("Debug", "LogLamas", true,
            "Write a line to BepInEx/SpellBrigadeLamaPen.log when a lama first shows up and whenever its state, pen or " +
            "charm changes. Decharms are always logged.");
    }
}
