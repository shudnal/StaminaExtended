# 1.0.11
* Avoided dodge modifier calculations when no dodge can start and skipped combat-state queries when both configured multipliers match.
* Reduced repeated regeneration getter calls and unused maximum-stamina queries.
* Restored temporary dodge and food-derived base-stamina values even after exceptions or live configuration changes.
* Fixed extra regeneration tooltip text not being inserted before the following line.

# 1.0.10
* Updated for the Valheim 1.0.7 release.
* Completed the migration to the standalone ConditionalConfigSync dependency.
* Updated required dependencies to BepInExPack Valheim 5.4.2350 and ConditionalConfigSync 1.0.5.
* Apply snow surface settings to deep and very deep snow.
* Add configurable ice surface modifiers and recognize ice building materials.
* Clear stale surface modifiers when moving onto an unrecognized surface.
* Fixed per-call restoration of temporary regeneration, swimming and sneaking parameters, including exceptional exits.
* Fixed sneaking drain restoration depending on the unrelated swimming setting.
* Restored vanilla blocking regeneration when the mod is disabled.

# 1.0.9
* fixed fishing pull and hooking stamina usage multipliers
* added new fish escape stamina usage (stamina required to fight fish trying to escape)

# 1.0.8
* patch 0.220.3
* ServerSync updated

# 1.0.7
* out of combat configurable stamina usage for sneaking, running, jumping and dodging
* configurable stamina drain on perfect parry

# 1.0.6
* fix for extra stamina regeneration not applied in certain scenarios
* little text tweaks
* configurable blocking stamina regeneration rate

# 1.0.5
* unarmed base stamina scale

# 1.0.4
* feast stamina regeneration tooltip

# 1.0.3
* bog witch patch

# 1.0.2
* Ash and Lava ground modifiers

# 1.0.1
* Ashlands

# 1.0.0
 * Initial Release