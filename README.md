# Lama Pen

A mod for The Spell Brigade: in the herding objective, a charmed lama loses its charm once it's inside the pen, so it
stops following you back out. It walks a bit further in and stays there.

Only the host needs it.

Install it from Thunderstore with r2modman or Gale:
[StrayingFox/Lama_Pen](https://thunderstore.io/c/the-spell-brigade/p/StrayingFox/Lama_Pen/).

## Building

Copy `GamePaths.props.user.example` to `GamePaths.props.user` and point it at your BepInEx folder. Then, with the game
closed:

```bash
dotnet build src/SpellBrigadeLamaPen -c Release -p:DeployToGame=true
```

Tests: `dotnet test tests/SpellBrigadeLamaPen.Tests`. Thunderstore zip: `dotnet build src/SpellBrigadeLamaPen -c Release -t:Package`.

## My other mods

- [Dash](https://github.com/stefanws0/Spell-brigade-dash)
- [Minimap](https://github.com/stefanws0/Spell-Bridge-Minimap)
- [Mouse Ping](https://github.com/stefanws0/Spell-brigade-mouse-ping)
