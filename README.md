# Respawn Token Customizer

EXILED plugin for SCP: Secret Laboratory that lets server owners configure main-wave respawn tokens for NTF and Chaos.

## What It Controls

- Starting main-wave tokens per faction.
- Shared or per-faction earned-token pools.
- Influence milestone thresholds that grant earned tokens.

The game normally uses a shared earned-token pool. This plugin can keep that behavior with a configurable shared pool size, or switch to separate earned-token pools for NTF and Chaos.

## Build

```bash
dotnet build RespawnTokenCustomizer.sln -c Release
```

Output:

```text
bin/Release/net48/RespawnTokenCustomizer.dll
```

Copy the DLL into the server's EXILED plugins folder.

Built against EXILED 9.14.0. That EXILED release is currently marked as a pre-release by ExMod-Team.

## Config

Generated config prefix:

```text
respawn_token_customizer
```

Shared pool example:

```yaml
respawn_token_customizer:
  is_enabled: true
  debug: false
  earned_token_pool_mode: Shared
  shared_earnable_tokens: 3
  reset_current_tokens_on_reload: false
  extra_milestone_step: 50
  nine_tailed_fox:
    starting_tokens: 2
    milestone_thresholds:
    - 40
    - 80
    - 150
    - 200
  chaos_insurgency:
    starting_tokens: 1
    milestone_thresholds:
    - 40
    - 100
```

`earned_token_pool_mode: Shared` keeps vanilla-style competition for one pool. `shared_earnable_tokens` controls how many earned tokens are available total.

Per-faction pool example:

```yaml
earned_token_pool_mode: PerFaction
nine_tailed_fox:
  earnable_tokens: 4
chaos_insurgency:
  earnable_tokens: 2
```

`earned_token_pool_mode: PerFaction` gives each faction its own earned-token pool. In that mode, each faction's `earnable_tokens` value controls its own cap.

If more milestone thresholds are needed than listed, extra thresholds are generated using `extra_milestone_step`.

## Commands

- `respawntokensreload` reloads this plugin's config.
- `respawntokensstatus` prints current wave token state.

Permissions:

- `respawntokencustomizer.reload`
- `respawntokencustomizer.inspect`
