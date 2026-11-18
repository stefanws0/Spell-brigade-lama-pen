namespace SpellBrigadeLamaPen.Game;

using System;
using UnityEngine;

/// <summary>
/// The mod's only injected MonoBehaviour (IL2CPP needs the IntPtr constructor). Lives on BepInEx's persistent
/// manager object, so it survives scene changes. Field-free on purpose: all state is in <see cref="LamaWatcher"/>.
/// </summary>
public class LamaPenRunner : MonoBehaviour
{
    public LamaPenRunner(IntPtr ptr) : base(ptr)
    {
    }

    public void Update()
    {
        var now = Time.unscaledTime;
        try
        {
            LamaWatcher.Update(now);
        }
        catch (Exception ex)
        {
            ModLog.ErrorThrottled("runner", ex, now);
        }

        try
        {
            TestHotkeys.Update(now);
        }
        catch (Exception ex)
        {
            ModLog.ErrorThrottled("hotkeys", ex, now);
        }
    }

    /// <summary>Marks a normal quit, so the next start can tell a crash from a clean exit.</summary>
    public void OnApplicationQuit() => ModLog.Step(ModLog.CleanExit);
}
