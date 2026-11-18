# Spell Brigade Lama Pen

A BepInEx 6 (IL2CPP) mod for The Spell Brigade.

In "Herd the Mana Beasts", charmed lamas follow you, so they follow you straight back out of the pen again. With this
mod, a charmed lama that is inside the pen loses its charm and stays there like a normal lama.

- Automatic: no key to press.
- Only charmed lamas are changed. Normal lamas behave as the game made them.
- **Co-op: the host needs the mod.** The lamas' AI runs on the host's PC. On a joining player's PC the mod only
  watches (and logs); it changes nothing.
- No game methods are patched.

## Config

`BepInEx/config/com.stefanws0.spellbrigadelamapen.cfg`

- `General.Enabled` (true): turn the mod off without uninstalling it.
- `Debug.LogLamas` (true): log every lama's state changes to `BepInEx/SpellBrigadeLamaPen.log`.

## Build

Copy `GamePaths.props.user.example` to `GamePaths.props.user` and point it at your BepInEx folder, then:

    dotnet test tests/SpellBrigadeLamaPen.Tests
    dotnet build src/SpellBrigadeLamaPen -c Release -p:DeployToGame=true

Deploy only while the game is closed.
