# Respawn Token Customizer

EXILED plugin for SCP: Secret Laboratory that lets server owners configure main-wave respawn tokens for NTF and Chaos.

## What It Controls

- Starting main-wave tokens per faction.
- Earnable main-wave token cap per faction.
- Influence milestone thresholds that grant earned tokens.

The game normally uses a shared earned-token pool. This plugin patches the vanilla earned-token handler so NTF and Chaos can have independent earnable limits.

## Build

```bash
dotnet build RespawnTokenCustomizer.csproj -c Release
```

Output:

```text
bin/Release/net48/RespawnTokenCustomizer.dll
```

Copy the DLL into the server's EXILED plugins folder.

## Config

Generated config prefix:

```text
respawn_token_customizer
```

EXILED normally writes this section into the server config file automatically. If it does not appear, copy the contents of `default_config.yml` into the config manually and check the server console for plugin-load warnings.

Example:

```yaml
respawn_token_customizer:
  is_enabled: true
  debug: false
  override_earned_token_pool: true
  reset_current_tokens_on_reload: false
  extra_milestone_step: 50
  nine_tailed_fox:
    starting_tokens: 2
    earnable_tokens: 4
    milestone_thresholds:
    - 40
    - 80
    - 150
    - 200
  chaos_insurgency:
    starting_tokens: 1
    earnable_tokens: 2
    milestone_thresholds:
    - 40
    - 100
```

If `earnable_tokens` is higher than the threshold list, extra thresholds are generated using `extra_milestone_step`.

## Commands

- `respawntokensreload` reloads this plugin's config.
- `respawntokensstatus` prints current wave token state.

Permissions:

- `respawntokencustomizer.reload`
- `respawntokencustomizer.inspect`
