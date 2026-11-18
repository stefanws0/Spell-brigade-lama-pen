namespace SpellBrigadeLamaPen;

using BepInEx;
using BepInEx.Unity.IL2CPP;

/// <summary>BepInEx entry point: config and the per-frame runner. No Harmony patches in this mod.</summary>
[BepInPlugin(Guid, Name, Version)]
public class Plugin : BasePlugin
{
    public const string Guid = "com.stefanws0.spellbrigadelamapen";
    public const string Name = "SpellBrigadeLamaPen";
    public const string Version = "0.1.0";

    public override void Load()
    {
        ModLog.Init(Log);
        ModLog.Step("Load: config");
        ModConfig.Init(Config);
        ModLog.Step("Load: done");
        ModLog.Info($"{Name} {Version} loaded.");
    }
}
