namespace SpellBrigadeLamaPen.Game;

using UnityEngine.InputSystem;
using UObject = UnityEngine.Object;

/// <summary>
/// Test hotkeys (config <c>Debug.TestHotkeys</c>), read every frame:
/// Ctrl+1 starts a herding objective through the game's own <c>ObjectiveDirector.StartObjective</c>;
/// Ctrl+2 charms the running objective's lamas that are still outside the pen (<see cref="LamaWatcher.CharmForTest"/>).
/// Host only, and only inside a run. Deliberately independent of <c>General.Enabled</c>: with the mod off, Ctrl+2
/// shows how the game itself treats charmed lamas in the pen.
/// </summary>
internal static class TestHotkeys
{
    private static bool _keyboardRead;

    public static void Update(float now)
    {
        if (!ModConfig.TestHotkeys.Value) return;
        var first = !_keyboardRead;
        _keyboardRead = true; // before the read, so a throwing read can't write this breadcrumb every frame
        if (first) ModLog.Step("first Keyboard.current read");
        var keyboard = Keyboard.current;
        if (first) ModLog.Step("first Keyboard.current read done");
        if (keyboard == null) return;
        if (!keyboard.leftCtrlKey.isPressed && !keyboard.rightCtrlKey.isPressed) return;

        if (keyboard.digit1Key.wasPressedThisFrame) StartHerding();
        else if (keyboard.digit2Key.wasPressedThisFrame) LamaWatcher.CharmForTest(now);
    }

    private static void StartHerding()
    {
        ModLog.StepOnce("director", "first Ctrl+1: reading ObjectiveDirector");
        var director = ObjectiveDirector.Instance;
        if (director == null)
        {
            ModLog.Info("Ctrl+1: not in a run (no ObjectiveDirector).");
            return;
        }

        var isServer = director.IsServer;
        var active = director.ActiveObjective;
        ModLog.StepOnce("director-done", "first Ctrl+1: reading ObjectiveDirector done");
        if (!isServer)
        {
            ModLog.Info("Ctrl+1: only the host can start objectives.");
            return;
        }

        if (active != null)
        {
            ModLog.Info("Ctrl+1: an objective is already running; finish it first.");
            return;
        }

        if (HerdingStillOnTheField())
        {
            ModLog.Info("Ctrl+1: the previous herding objective is still ending; try again in a few seconds.");
            return;
        }

        ModLog.Step("Ctrl+1: ObjectiveDirector.StartObjective(Herding)");
        var objective = director.StartObjective(Objective.Herding);
        ModLog.Step("Ctrl+1: StartObjective done");
        ModLog.Info(objective == null
            ? "Ctrl+1: the game started no objective (StartObjective returned nothing)."
            : "Ctrl+1: started a herding objective.");
    }

    /// <summary>Any herding objective still active, including one playing its end sequence.</summary>
    private static bool HerdingStillOnTheField()
    {
        var all = UObject.FindObjectsOfType<HerdingObjectiveManager>();
        if (all == null) return false;
        for (int i = 0; i < all.Length; i++)
            if (all[i] != null && all[i].isActiveAndEnabled) return true;
        return false;
    }
}
