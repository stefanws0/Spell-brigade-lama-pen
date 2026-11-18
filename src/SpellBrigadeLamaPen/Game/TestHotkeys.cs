namespace SpellBrigadeLamaPen.Game;

using UnityEngine.InputSystem;

/// <summary>
/// Test hotkeys (config <c>Debug.TestHotkeys</c>), read every frame:
/// Ctrl+1 starts a herding objective through the game's own <c>ObjectiveDirector.StartObjective</c>;
/// Ctrl+2 charms the running objective's lamas that are still outside the pen (<see cref="LamaWatcher.CharmForTest"/>).
/// Host only, and only inside a run.
/// </summary>
internal static class TestHotkeys
{
    private static bool _keyboardRead;

    public static void Update(float now)
    {
        if (!ModConfig.TestHotkeys.Value) return;
        if (!_keyboardRead) ModLog.Step("first Keyboard.current read");
        var keyboard = Keyboard.current;
        if (!_keyboardRead)
        {
            _keyboardRead = true;
            ModLog.Step("first Keyboard.current read done");
        }

        if (keyboard == null) return;
        if (!keyboard.leftCtrlKey.isPressed && !keyboard.rightCtrlKey.isPressed) return;

        if (keyboard.digit1Key.wasPressedThisFrame) StartHerding();
        else if (keyboard.digit2Key.wasPressedThisFrame) LamaWatcher.CharmForTest(now);
    }

    private static void StartHerding()
    {
        var director = ObjectiveDirector.Instance;
        if (director == null)
        {
            ModLog.Info("Ctrl+1: not in a run (no ObjectiveDirector).");
            return;
        }

        if (!director.IsServer)
        {
            ModLog.Info("Ctrl+1: only the host can start objectives.");
            return;
        }

        if (director.ActiveObjective != null)
        {
            ModLog.Info("Ctrl+1: an objective is already running; finish it first.");
            return;
        }

        ModLog.Step("Ctrl+1: ObjectiveDirector.StartObjective(Herding)");
        var objective = director.StartObjective(Objective.Herding);
        ModLog.Step("Ctrl+1: StartObjective done");
        ModLog.Info(objective == null
            ? "Ctrl+1: the game started no objective (StartObjective returned nothing)."
            : "Ctrl+1: started a herding objective.");
    }
}
