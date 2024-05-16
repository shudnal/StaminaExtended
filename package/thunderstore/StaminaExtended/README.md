# Stamina Extended

Control various stamina aspects. Linear regeneration change, more regen from extra stamina. Encumbered, swim, sneaking tweaks. Surface dependent stamina usage and movement speed.

## Linear regeneration rate change

Makes stamina recovery rate dependent on the current value. Overall time to regenerate stamina to 100% is still almost the same.

If Multiplier value is above 1. Stamina will regenerate faster at lower values and proportionally slower at higher values. 

If Multiplier value is below 1. Stamina will regenerate slower at lower values and proportionally higher at higher values.

You can set regeneration threshold aka inflection point of stamina regeneration rate. In that point regeneration rate changes its sign.

For example using default values (multiplier 3.0, threshold 0.5):
* stamina 0% - regeneration rate is 300% of normal
* stamina 25% - regeneration rate is 150% of normal
* stamina 50% - regeneration rate is 100% of normal
* stamina 75% - regeneration rate is 66% of normal
* stamina 100% - regeneration rate is 33% of normal

## Extra stamina regeneration

Tired of late game big stamina pool regenerating rate is as low as on day 1?

Make extra stamina points increase overall regeneration rate.

Default config is 1% of regeneration rate per every 10 points of extra stamina (gained from any source like food, enchants, jewels or any other source of stamina increase outside of base stamina).

You can limit the calculation of extra stamina so it takes into account stamina from food only.

Default config of 1% regeneration rate per 10 extra points means you will have 25% stamina regeneration rate if you have 250 stamina from best stamina food combination.
That way you can still use Medium stamina mead instead of Lingering stamina mead.

Regeneration rate will be shown in the food tooltip and in the Raven menu in Status effects section.

## Encumbered stamina

Control how much stamina is used while encumbered.

Make stamina still regenerating but on lower rate while encumbered.

## Swimming stamina

Control how much stamina is used while swimming.

Make stamina still regenerating but on lower rate and with a delay while swimming.

Press "Run" button to swim faster depleting proportionally more stamina. 
Swimming speed will gradually increase until maximum speed is reached.

## Sneaking stamina

Control how much stamina is used while sneaking.

Make Sneaking skill increase stamina regeneration rate while not moving.

Make sneaking not to use stamina when no enemy is in a range.

## Base stamina increase by skills

Base stamina will be increased proportionally by skills levels where max value reached at skill level 100.

Skills that affect base stamina:
* Run
* Jump
* Sneak
* Swim
* Fishing

Value is configurable skill-wise.

Default config means you will have 10 more base stamina for any skill with level 50.
So maximum increase will be 100 stamina when all skills are 100.

## Various multipliers

Set stamina drain by various actions:
* Fishing pull stamina
* Fishing hooked stamina (to keep fish on the line)
* Harpooned target pull
* Tools usage (hammer, cultivator, hoe)
* Block
* Run
* Dodge
* Jump

## Misc settings

Hide stamina numeric value shown on the bar.

Make stamina needed to Block, Dodge and Jump to be reduced by appropriate skill. 33% at max level to meet behavior of other skills.

## Grounds

Enable detection of current surface your character's feet are touching.

Set multipliers based on surface:
* movement speed
* jump height
* stamina consumption
* stamina regeneration rate

Detected surfaces:
* Snow
* Mud (swamp)
* Grass (in black forest, plains and meadows)
* Wood (basic wooden structures)
* Hard wood (core and dark wood)
* Stone
* Marble
* Metal
* Cultivated ground
* Cleared ground
* Paved ground

Detection point is a point where you foot touching the ground. Detection happens only when foot touches the ground. 

In short when you hear footstep your current surface is calculated.

Status effect name change could be spammy and in general useless so it's disabled by default.

## Installation (manual)
extract StaminaExtended.dll to your BepInEx\Plugins\ folder

## Configurating
The best way to handle configs is [Configuration Manager](https://thunderstore.io/c/valheim/p/shudnal/ConfigurationManager/).

Or [Official BepInEx Configuration Manager](https://valheim.thunderstore.io/package/Azumatt/Official_BepInEx_ConfigurationManager/).

## Mirrors
[Nexus](https://www.nexusmods.com/valheim/mods/2719)