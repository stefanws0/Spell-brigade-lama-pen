namespace SpellBrigadeLamaPen;

using BepInEx;
using BepInEx.Unity.IL2CPP;
using SpellBrigadeLamaPen.Game;

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
        ModLog.Step("Load: registering LamaPenRunner");
        AddComponent<LamaPenRunner>();
        ModLog.Step("Load: done");
        var off = ModConfig.Enabled.Value ? "" : " (General.Enabled is off: lamas are left alone)";
        ModLog.Info($"{Name} {Version} loaded{off}.");
    }
}
