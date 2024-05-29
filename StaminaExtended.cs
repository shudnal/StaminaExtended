using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using ServerSync;
using System;
using UnityEngine;

namespace StaminaExtended
{
    [BepInPlugin(pluginID, pluginName, pluginVersion)]
    public class StaminaExtended : BaseUnityPlugin
    {
        const string pluginID = "shudnal.StaminaExtended";
        const string pluginName = "Stamina Extended";
        const string pluginVersion = "1.0.2";

        private readonly Harmony harmony = new Harmony(pluginID);

        internal static readonly ConfigSync configSync = new ConfigSync(pluginID) { DisplayName = pluginName, CurrentVersion = pluginVersion, MinimumRequiredVersion = pluginVersion };

        public static ConfigEntry<bool> modEnabled;
        public static ConfigEntry<bool> configLocked;

        public static ConfigEntry<bool> loggingEnabled;

        public static ConfigEntry<bool> linearRegeneration;
        public static ConfigEntry<float> linearRegenerationMultiplier;
        public static ConfigEntry<float> linearRegenerationThreshold;

        public static ConfigEntry<bool> extraStaminaRegeneration;
        public static ConfigEntry<float> extraStaminaRegenerationPercent;
        public static ConfigEntry<int> extraStaminaRegenerationPoints;
        public static ConfigEntry<bool> extraStaminaRegenerationOnlyFood;

        public static ConfigEntry<bool> encumberedStamina;
        public static ConfigEntry<float> encumberedStaminaDrainMultiplier;
        public static ConfigEntry<bool> encumberedStaminaRegeneration;
        public static ConfigEntry<float> encumberedStaminaRegenerationMultiplier;

        public static ConfigEntry<bool> swimmingStamina;
        public static ConfigEntry<float> swimmingStaminaDrainMultiplier;
        public static ConfigEntry<bool> swimmingStaminaRegeneration;
        public static ConfigEntry<float> swimmingStaminaRegenerationMultiplier;
        public static ConfigEntry<float> swimmingStaminaRegenerationDelay;
        public static ConfigEntry<bool> swimmingRun;
        public static ConfigEntry<float> swimmingRunMaximumSpeed;

        public static ConfigEntry<bool> sneakingStamina;
        public static ConfigEntry<float> sneakingStaminaDrainMultiplier;
        public static ConfigEntry<float> sneakingStaminaRegenerationMultiplier;
        public static ConfigEntry<bool> sneakingStaminaNoEnemies;
        public static ConfigEntry<float> sneakingStaminaNoEnemiesRange;

        public static ConfigEntry<bool> baseStamina;
        public static ConfigEntry<float> runBaseStaminaIncrease;
        public static ConfigEntry<float> swimBaseStaminaIncrease;
        public static ConfigEntry<float> fishingBaseStaminaIncrease;
        public static ConfigEntry<float> sneakBaseStaminaIncrease;
        public static ConfigEntry<float> jumpBaseStaminaIncrease;

        public static ConfigEntry<float> pullStaminaUse;
        public static ConfigEntry<float> hookedStaminaPerSec;
        public static ConfigEntry<float> harpoonedStaminaDrain;
        public static ConfigEntry<float> toolStaminaDrain;
        public static ConfigEntry<float> blockStaminaDrain;
        public static ConfigEntry<float> runStaminaDrain;
        public static ConfigEntry<float> dodgeStaminaUsage;
        public static ConfigEntry<float> jumpStaminaUsage;

        public static ConfigEntry<bool> hideStaminaValue;
        public static ConfigEntry<bool> blockStaminaSkill;
        public static ConfigEntry<bool> dodgeStaminaSkill;
        public static ConfigEntry<bool> jumpStaminaSkill;

        public static ConfigEntry<bool> groundsEnabled;
        public static ConfigEntry<bool> groundsShowStatusEffect;
        public static ConfigEntry<string> groundsNameDefault;

        public static ConfigEntry<float> groundsSpeedSnow;
        public static ConfigEntry<float> groundsJumpSnow;
        public static ConfigEntry<float> groundsStaminaDrainSnow;
        public static ConfigEntry<float> groundsStaminaRegenSnow;
        public static ConfigEntry<string> groundsNameSnow;

        public static ConfigEntry<float> groundsSpeedMud;
        public static ConfigEntry<float> groundsJumpMud;
        public static ConfigEntry<float> groundsStaminaDrainMud;
        public static ConfigEntry<float> groundsStaminaRegenMud;
        public static ConfigEntry<string> groundsNameMud;

        public static ConfigEntry<float> groundsSpeedGrass;
        public static ConfigEntry<float> groundsJumpGrass;
        public static ConfigEntry<float> groundsStaminaDrainGrass;
        public static ConfigEntry<float> groundsStaminaRegenGrass;
        public static ConfigEntry<string> groundsNameGrass;

        public static ConfigEntry<float> groundsSpeedWood;
        public static ConfigEntry<float> groundsJumpWood;
        public static ConfigEntry<float> groundsStaminaDrainWood;
        public static ConfigEntry<float> groundsStaminaRegenWood;
        public static ConfigEntry<string> groundsNameWood;

        public static ConfigEntry<float> groundsSpeedHardWood;
        public static ConfigEntry<float> groundsJumpHardWood;
        public static ConfigEntry<float> groundsStaminaDrainHardWood;
        public static ConfigEntry<float> groundsStaminaRegenHardWood;
        public static ConfigEntry<string> groundsNameHardWood;

        public static ConfigEntry<float> groundsSpeedStone;
        public static ConfigEntry<float> groundsJumpStone;
        public static ConfigEntry<float> groundsStaminaDrainStone;
        public static ConfigEntry<float> groundsStaminaRegenStone;
        public static ConfigEntry<string> groundsNameStone;

        public static ConfigEntry<float> groundsSpeedMarble;
        public static ConfigEntry<float> groundsJumpMarble;
        public static ConfigEntry<float> groundsStaminaDrainMarble;
        public static ConfigEntry<float> groundsStaminaRegenMarble;
        public static ConfigEntry<string> groundsNameMarble;

        public static ConfigEntry<float> groundsSpeedMetal;
        public static ConfigEntry<float> groundsJumpMetal;
        public static ConfigEntry<float> groundsStaminaDrainMetal;
        public static ConfigEntry<float> groundsStaminaRegenMetal;
        public static ConfigEntry<string> groundsNameMetal;

        public static ConfigEntry<float> groundsSpeedCultivated;
        public static ConfigEntry<float> groundsJumpCultivated;
        public static ConfigEntry<float> groundsStaminaDrainCultivated;
        public static ConfigEntry<float> groundsStaminaRegenCultivated;
        public static ConfigEntry<string> groundsNameCultivated;

        public static ConfigEntry<float> groundsSpeedCleared;
        public static ConfigEntry<float> groundsJumpCleared;
        public static ConfigEntry<float> groundsStaminaDrainCleared;
        public static ConfigEntry<float> groundsStaminaRegenCleared;
        public static ConfigEntry<string> groundsNameCleared;

        public static ConfigEntry<float> groundsSpeedPaved;
        public static ConfigEntry<float> groundsJumpPaved;
        public static ConfigEntry<float> groundsStaminaDrainPaved;
        public static ConfigEntry<float> groundsStaminaRegenPaved;
        public static ConfigEntry<string> groundsNamePaved;

        public static ConfigEntry<float> groundsSpeedAsh;
        public static ConfigEntry<float> groundsJumpAsh;
        public static ConfigEntry<float> groundsStaminaDrainAsh;
        public static ConfigEntry<float> groundsStaminaRegenAsh;
        public static ConfigEntry<string> groundsNameAsh;

        public static ConfigEntry<float> groundsSpeedLava;
        public static ConfigEntry<float> groundsJumpLava;
        public static ConfigEntry<float> groundsStaminaDrainLava;
        public static ConfigEntry<float> groundsStaminaRegenLava;
        public static ConfigEntry<string> groundsNameLava;

        public static StaminaExtended instance;

        public const string statusEffectSurfaceName = "Surface";
        public static int statusEffectSurfaceHash = statusEffectSurfaceName.GetStableHashCode();

        void Awake()
        {
            harmony.PatchAll();
            instance = this;

            ConfigInit();
            _ = configSync.AddLockingConfigEntry(configLocked);

            Game.isModded = true;
        }

        private void FixedUpdate()
        {
            Player player = Player.m_localPlayer;
            if (player == null)
                return;

            if (groundsEnabled.Value && !player.GetSEMan().HaveStatusEffect(statusEffectSurfaceHash))
                player.GetSEMan().AddStatusEffect(statusEffectSurfaceHash);
            else if (!groundsEnabled.Value && player.GetSEMan().HaveStatusEffect(statusEffectSurfaceHash))
                player.GetSEMan().RemoveStatusEffect(statusEffectSurfaceHash, quiet: true);

        }


        void OnDestroy()
        {
            harmony?.UnpatchSelf();
            instance = null;
        }

        private void ConfigInit()
        {
            config("1 - General", "NexusID", 2719, "Nexus mod ID for updates", false);

            modEnabled = config("1 - General", "Enabled", true, "Mod alters stamina behavior");
            configLocked = config("1 - General", "Lock Configuration", defaultValue: true, "Configuration is locked and can be changed by server admins only.");
            loggingEnabled = config("1 - General", "Logging enabled", false, "Enable logging. [Not Synced with Server]", false);

            linearRegeneration = config("2 - Linear regeneration change", "Enabled", true, "Enable linear change of stamina regeneration rate. Overall time to regenerate stamina to 100% is still almost the same.");
            linearRegenerationMultiplier = config("2 - Linear regeneration change", "Multiplier", 3f, "Multiplier of regeneration rate when stamina is 0." +
                                                                                                "\nIf value is above 1. Stamina will regenerate faster at lower values and proportionally slower at higher values." +
                                                                                                "\nIf value is below 1. Stamina will regenerate slower at lower values and proportionally higher at higher values.");
            linearRegenerationThreshold = config("2 - Linear regeneration change", "Regeneration threshold", 0.5f, "Inflection point of stamina regeneration rate. Stamina regeneration rate is normal only in that point." +
                                                                                                                    "\nIn that point regeneration rate changes its sign." +
                                                                                                                    "\nIf set value is outside of 0-1 range stamina will regenerate normally");

            extraStaminaRegeneration = config("3 - Extra stamina regeneration", "Enabled", true, "Enable increased stamina regeneration by X% per every Y point of additional stamina");
            extraStaminaRegenerationPercent = config("3 - Extra stamina regeneration", "Percent", 1f, "Stamina regeneration increased by X%");
            extraStaminaRegenerationPoints = config("3 - Extra stamina regeneration", "Points", 10, "Stamina regeneration increased per every Y points of additional stamina");
            extraStaminaRegenerationOnlyFood = config("3 - Extra stamina regeneration", "Stamina from food only", true, "Only stamina gain from food is counted for increased regeneration");

            encumberedStamina = config("4 - Encumbered stamina", "Enabled", true, "Enable encumbered stamina control");
            encumberedStaminaDrainMultiplier = config("4 - Encumbered stamina", "Stamina drain multiplier", 2f, "Encumbered stamina drain multiplier. Set 0 to disable stamina usage while encumbered");
            encumberedStaminaRegeneration = config("4 - Encumbered stamina", "Stamina regeneration while encumbered", true, "Stamina will regenerate while encumbered");
            encumberedStaminaRegenerationMultiplier = config("4 - Encumbered stamina", "Stamina regeneration multiplier", 0.2f, "Stamina regeneration multiplier of normal stamina regeneration rate");

            swimmingStamina = config("5 - Swimming stamina", "Enabled", true, "Enable swimming stamina control");
            swimmingStaminaDrainMultiplier = config("5 - Swimming stamina", "Stamina drain multiplier", 1f, "Swimming stamina drain multiplier. Set 0 to disable stamina usage while swimming");
            swimmingStaminaRegeneration = config("5 - Swimming stamina", "Stamina regeneration while swimming", true, "Stamina will regenerate while encumbered");
            swimmingStaminaRegenerationMultiplier = config("5 - Swimming stamina", "Stamina regeneration multiplier", 0.2f, "Stamina regeneration multiplier of normal stamina regeneration rate");
            swimmingStaminaRegenerationDelay = config("5 - Swimming stamina", "Stamina regeneration delay", 2f, "Additional delay in seconds before stamina begins to regenerate while not moving");
            swimmingRun = config("5 - Swimming stamina", "Swimming acceleration", true, "Press Run hotkey to swim faster depleting proportionally more stamina. Swimming speed will gradually increase until maximum speed is reached.");
            swimmingRunMaximumSpeed = config("5 - Swimming stamina", "Swimming acceleration maximum speed", 2f, "Maximum speed of swimming acceleration in proportion to normal swim speed.");

            sneakingStamina = config("6 - Sneaking stamina", "Enabled", true, "Enable sneaking stamina regeneration control");
            sneakingStaminaDrainMultiplier = config("6 - Sneaking stamina", "Stamina drain multiplier", 1f, "Sneaking stamina drain multiplier");
            sneakingStaminaRegenerationMultiplier = config("6 - Sneaking stamina", "Stamina regeneration while sneaking", 0.5f, "Set additional stamina regen rate. Stamina will regenerate faster while sneaking depending on the Sneak skill" +
                                                                                                                              "\n0.5 with 40 sneak skill means stamina will regenerate 20% faster while sneaking and not moving");
            sneakingStaminaNoEnemies = config("6 - Sneaking stamina", "Stamina is not spent if no enemy in range", true, "Stamina regeneration multiplier of normal stamina regeneration rate");
            sneakingStaminaNoEnemiesRange = config("6 - Sneaking stamina", "Stamina is not spent range", 20f, "Distance to nearby enemy to activate stamina usage while sneaking");

            baseStamina = config("7 - Base stamina", "Enabled", true, "Base stamina will be increased proportionally to skills where set value reached at skill level 100.");
            runBaseStaminaIncrease = config("7 - Base stamina", "Run", 20f, "Base stamina will be increased by set value when Run skill is level 100");
            swimBaseStaminaIncrease = config("7 - Base stamina", "Swim", 20f, "Base stamina will be increased by set value when Swim skill is level 100");
            fishingBaseStaminaIncrease = config("7 - Base stamina", "Fishing", 20f, "Base stamina will be increased by set value when Fishing skill is level 100");
            sneakBaseStaminaIncrease = config("7 - Base stamina", "Sneak", 20f, "Base stamina will be increased by set value when Sneak skill is level 100");
            jumpBaseStaminaIncrease = config("7 - Base stamina", "Jump", 20f, "Base stamina will be increased by set value when Jump skill is level 100");

            pullStaminaUse = config("8 - Various multipliers", "Fishing pull stamina", 1f, "Stamina required to reel");
            hookedStaminaPerSec = config("8 - Various multipliers", "Fishing hooked stamina", 1f, "Stamina required to keep a fish on the line");
            harpoonedStaminaDrain = config("8 - Various multipliers", "Harpooned pull stamina", 1f, "Stamina required to pull harpooned target");
            toolStaminaDrain = config("8 - Various multipliers", "Tools stamina drain", 1f, "Stamina required to use tools such as hammer, hoe, cultivator");
            blockStaminaDrain = config("8 - Various multipliers", "Block stamina drain", 1f, "Stamina required to block");
            runStaminaDrain = config("8 - Various multipliers", "Run stamina drain", 1f, "Stamina required to run");
            dodgeStaminaUsage = config("8 - Various multipliers", "Dodge stamina usage", 1f, "Stamina required to dodge");
            jumpStaminaUsage = config("8 - Various multipliers", "Jump stamina usage", 1f, "Stamina required to jump");

            hideStaminaValue = config("9 - Misc", "Hide stamina text", false, "Hide stamina text value on stamina bar");
            blockStaminaSkill = config("9 - Misc", "Block stamina usage depends on skill", true, "Amount of stamina needed to block is reduced by 33% when Block skill is 100");
            dodgeStaminaSkill = config("9 - Misc", "Dodge stamina usage depends on Jump skill", true, "Amount of stamina needed to block is reduced by 33% when Jump skill is 100");
            jumpStaminaSkill = config("9 - Misc", "Jump stamina usage depends on Jump skill", true, "Amount of stamina needed to block is reduced by 33% when Jump skill is 100");

            groundsEnabled = config("Grounds", "Enabled", true, "Enable change of movement speed, run, jump and dodge stamina consumption on different surfaces");
            groundsShowStatusEffect = config("Grounds", "Show status effect", false, "Show status effect of current surface. Status effect will still be shown in Raven menu if disabled.");
            groundsNameDefault = config("Grounds", "Status name by default", "Default", "Localized default name for status effect");

            groundsEnabled.SettingChanged += (sender, args) => SE_Surface.UpdateSurfaceStatusEffectStats();
            groundsShowStatusEffect.SettingChanged += (sender, args) => SE_Surface.UpdateSurfaceStatusEffectStats();

            groundsSpeedSnow = config("Grounds - Snow", "Speed multiplier", 1f, "Movement speed multiplier");
            groundsJumpSnow = config("Grounds - Snow", "Jump multiplier", 1f, "Jump height multiplier");
            groundsStaminaDrainSnow = config("Grounds - Snow", "Stamina drain multiplier", 1f, "Stamina drain multiplier");
            groundsStaminaRegenSnow = config("Grounds - Snow", "Stamina regen multiplier", 1f, "Stamina regen multiplier");
            groundsNameSnow = config("Grounds - Snow", "Status effect name", "Snow", "Localized name for status effect");

            groundsSpeedMud = config("Grounds - Mud", "Speed multiplier", 1f, "Movement speed multiplier (swamps ground)");
            groundsJumpMud = config("Grounds - Mud", "Jump multiplier", 1f, "Jump height multiplier (swamps ground)");
            groundsStaminaDrainMud = config("Grounds - Mud", "Stamina drain multiplier", 1f, "Stamina drain multiplier (swamps ground)");
            groundsStaminaRegenMud = config("Grounds - Mud", "Stamina regen multiplier", 1f, "Stamina regen multiplier (swamps ground)");
            groundsNameMud = config("Grounds - Mud", "Status effect name", "Mud", "Localized name for status effect (swamps ground)");

            groundsSpeedGrass = config("Grounds - Grass", "Speed multiplier", 1f, "Movement speed multiplier (meadows and black forest)");
            groundsJumpGrass = config("Grounds - Grass", "Jump multiplier", 1f, "Jump height multiplier");
            groundsStaminaDrainGrass = config("Grounds - Grass", "Stamina drain multiplier", 1f, "Stamina drain multiplier (meadows and black forest)");
            groundsStaminaRegenGrass = config("Grounds - Grass", "Stamina regen multiplier", 1f, "Stamina regen multiplier");
            groundsNameGrass = config("Grounds - Grass", "Status effect name", "Grass", "Localized name for status effect (meadows and black forest)");

            groundsSpeedWood = config("Grounds - Wood", "Speed multiplier", 1.15f, "Movement speed multiplier");
            groundsJumpWood = config("Grounds - Wood", "Jump multiplier", 1.05f, "Jump height multiplier");
            groundsStaminaDrainWood = config("Grounds - Wood", "Stamina drain multiplier", 0.85f, "Stamina drain multiplier");
            groundsStaminaRegenWood = config("Grounds - Wood", "Stamina regen multiplier", 1f, "Stamina regen multiplier");
            groundsNameWood = config("Grounds - Wood", "Status effect name", "Wood", "Localized name for status effect");

            groundsSpeedHardWood = config("Grounds - HardWood", "Speed multiplier", 1.2f, "Movement speed multiplier");
            groundsJumpHardWood = config("Grounds - HardWood", "Jump multiplier", 1.05f, "Jump height multiplier");
            groundsStaminaDrainHardWood = config("Grounds - HardWood", "Stamina drain multiplier", 0.85f, "Stamina drain multiplier");
            groundsStaminaRegenHardWood = config("Grounds - HardWood", "Stamina regen multiplier", 1f, "Stamina regen multiplier");
            groundsNameHardWood = config("Grounds - HardWood", "Status effect name", "Hardwood", "Localized name for status effect");

            groundsSpeedStone = config("Grounds - Stone", "Speed multiplier", 1.25f, "Movement speed multiplier");
            groundsJumpStone = config("Grounds - Stone", "Jump multiplier", 1.1f, "Jump height multiplier");
            groundsStaminaDrainStone = config("Grounds - Stone", "Stamina drain multiplier", 0.8f, "Stamina drain multiplier");
            groundsStaminaRegenStone = config("Grounds - Stone", "Stamina regen multiplier", 1f, "Stamina regen multiplier");
            groundsNameStone = config("Grounds - Stone", "Status effect name", "Stone", "Localized name for status effect");

            groundsSpeedMarble = config("Grounds - Marble", "Speed multiplier", 1.3f, "Movement speed multiplier (dverger marble, ashstone and ancient)");
            groundsJumpMarble = config("Grounds - Marble", "Jump multiplier", 1.1f, "Jump height multiplier (dverger marble, ashstone and ancient)");
            groundsStaminaDrainMarble = config("Grounds - Marble", "Stamina drain multiplier", 0.75f, "Stamina drain multiplier (dverger marble, ashstone and ancient)");
            groundsStaminaRegenMarble = config("Grounds - Marble", "Stamina regen multiplier", 1f, "Stamina regen multiplier (dverger marble, ashstone and ancient)");
            groundsNameMarble = config("Grounds - Marble", "Status effect name", "Marble", "Localized name for status effect (dverger marble, ashstone and ancient)");

            groundsSpeedMetal = config("Grounds - Metal", "Speed multiplier", 1.25f, "Movement speed multiplier");
            groundsJumpMetal = config("Grounds - Metal", "Jump multiplier", 1.05f, "Jump height multiplier");
            groundsStaminaDrainMetal = config("Grounds - Metal", "Stamina drain multiplier", 0.8f, "Stamina drain multiplier");
            groundsStaminaRegenMetal = config("Grounds - Metal", "Stamina regen multiplier", 1f, "Stamina regen multiplier");
            groundsNameMetal = config("Grounds - Metal", "Status effect name", "Metal", "Localized name for status effect");

            groundsSpeedCultivated = config("Grounds - Cultivated", "Speed multiplier", 0.9f, "Movement speed multiplier");
            groundsJumpCultivated = config("Grounds - Cultivated", "Jump multiplier", 0.9f, "Jump height multiplier");
            groundsStaminaDrainCultivated = config("Grounds - Cultivated", "Stamina drain multiplier", 1.1f, "Stamina drain multiplier");
            groundsStaminaRegenCultivated = config("Grounds - Cultivated", "Stamina regen multiplier", 1.1f, "Stamina regen multiplier");
            groundsNameCultivated = config("Grounds - Cultivated", "Status effect name", "Cultivated", "Localized name for status effect");

            groundsSpeedCleared = config("Grounds - Cleared", "Speed multiplier", 1.1f, "Movement speed multiplier");
            groundsJumpCleared = config("Grounds - Cleared", "Jump multiplier", 1f, "Jump height multiplier");
            groundsStaminaDrainCleared = config("Grounds - Cleared", "Stamina drain multiplier", 0.9f, "Stamina drain multiplier");
            groundsStaminaRegenCleared = config("Grounds - Cleared", "Stamina regen multiplier", 1f, "Stamina regen multiplier");
            groundsNameCleared = config("Grounds - Cleared", "Status effect name", "Cleared", "Localized name for status effect");

            groundsSpeedPaved = config("Grounds - Paved", "Speed multiplier", 1.2f, "Movement speed multiplier");
            groundsJumpPaved = config("Grounds - Paved", "Jump multiplier", 1.05f, "Jump height multiplier");
            groundsStaminaDrainPaved = config("Grounds - Paved", "Stamina drain multiplier", 0.85f, "Stamina drain multiplier");
            groundsStaminaRegenPaved = config("Grounds - Paved", "Stamina regen multiplier", 1f, "Stamina regen multiplier");
            groundsNamePaved = config("Grounds - Paved", "Status effect name", "Paved", "Localized name for status effect");

            groundsSpeedAsh = config("Grounds - Ash", "Speed multiplier", 1f, "Movement speed multiplier (Ashlands ground)");
            groundsJumpAsh = config("Grounds - Ash", "Jump multiplier", 1f, "Jump height multiplier (Ashlands ground)");
            groundsStaminaDrainAsh = config("Grounds - Ash", "Stamina drain multiplier", 1f, "Stamina drain multiplier (Ashlands ground)");
            groundsStaminaRegenAsh = config("Grounds - Ash", "Stamina regen multiplier", 1f, "Stamina regen multiplier (Ashlands ground)");
            groundsNameAsh = config("Grounds - Ash", "Status effect name", "Ash", "Localized name for status effect (Ashlands ground)");

            groundsSpeedLava = config("Grounds - Lava", "Speed multiplier", 1f, "Movement speed multiplier (lava in Ashlands)");
            groundsJumpLava = config("Grounds - Lava", "Jump multiplier", 1f, "Jump height multiplier (lava in Ashlands)");
            groundsStaminaDrainLava = config("Grounds - Lava", "Stamina drain multiplier", 1f, "Stamina drain multiplier (lava in Ashlands)");
            groundsStaminaRegenLava = config("Grounds - Lava", "Stamina regen multiplier", 1f, "Stamina regen multiplier (lava in Ashlands)");
            groundsNameLava = config("Grounds - Lava", "Status effect name", "Lava", "Localized name for status effect (lava in Ashlands)");
        }

        ConfigEntry<T> config<T>(string group, string name, T defaultValue, ConfigDescription description, bool synchronizedSetting = true)
        {
            ConfigEntry<T> configEntry = Config.Bind(group, name, defaultValue, description);

            SyncedConfigEntry<T> syncedConfigEntry = configSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }

        ConfigEntry<T> config<T>(string group, string name, T defaultValue, string description, bool synchronizedSetting = true) => config(group, name, defaultValue, new ConfigDescription(description), synchronizedSetting);

        public static void LogInfo(object message)
        {
            if (loggingEnabled.Value)
                instance.Logger.LogInfo(message);
        }

        public static void LogWarning(object data)
        {
            if (loggingEnabled.Value)
                instance.Logger.LogWarning(data);
        }

        public string GetStringConfig(string fieldName)
        {
            return (GetType().GetField(fieldName).GetValue(this) as ConfigEntry<string>).Value;
        }

        public float GetFloatConfig(string fieldName)
        {
            return (GetType().GetField(fieldName).GetValue(this) as ConfigEntry<float>).Value;
        }

        [HarmonyPatch(typeof(Player), nameof(Player.UpdateStats), new Type[] { typeof(float) })]
        public static class Player_UpdateStats_StaminaRegenMultiplier
        {
            private static float _m_encumberedStaminaDrain;
            private static float _m_staminaRegenTimeMultiplier;

            [HarmonyPriority(Priority.VeryLow)]
            public static void Prefix(Player __instance, float dt)
            {
                if (!modEnabled.Value)
                    return;

                if (__instance.InIntro() || __instance.IsTeleporting())
                    return;

                _m_staminaRegenTimeMultiplier = __instance.m_staminaRegenTimeMultiplier;

                if (extraStaminaRegeneration.Value && extraStaminaRegenerationPercent.Value > 0f && extraStaminaRegenerationPoints.Value > 0)
                    __instance.m_staminaRegenTimeMultiplier += ExtraStamina.GetMultiplier(__instance);

                if (sneakingStamina.Value && __instance.IsSneaking())
                    __instance.m_staminaRegenTimeMultiplier += sneakingStaminaRegenerationMultiplier.Value * __instance.m_skills.GetSkillFactor(Skills.SkillType.Sneak);

                if (linearRegeneration.Value && 0f < linearRegenerationThreshold.Value && linearRegenerationThreshold.Value < 1f && linearRegenerationMultiplier.Value > 0f && __instance.GetMaxStamina() != 0f)
                {
                    if (__instance.GetStaminaPercentage() < linearRegenerationThreshold.Value)
                    {
                        float t = Mathf.Clamp01(__instance.GetStamina() / (__instance.GetMaxStamina() * linearRegenerationThreshold.Value));
                        __instance.m_staminaRegenTimeMultiplier = Mathf.Lerp(linearRegenerationMultiplier.Value, __instance.m_staminaRegenTimeMultiplier, t);
                    }
                    else if (__instance.GetStaminaPercentage() > linearRegenerationThreshold.Value)
                    {
                        float t = Mathf.Clamp01((__instance.GetMaxStamina() - __instance.GetStamina()) / (__instance.GetMaxStamina() * (1f - linearRegenerationThreshold.Value)));
                        __instance.m_staminaRegenTimeMultiplier = Mathf.Lerp(1 / linearRegenerationMultiplier.Value, __instance.m_staminaRegenTimeMultiplier, t);
                    }
                }

                if (encumberedStamina.Value && __instance.IsEncumbered())
                {
                    _m_encumberedStaminaDrain = __instance.m_encumberedStaminaDrain;
                    __instance.m_encumberedStaminaDrain *= encumberedStaminaDrainMultiplier.Value;

                    if (encumberedStaminaRegeneration.Value && __instance.m_moveDir.magnitude <= 0.1f)
                        if (__instance.GetStamina() < __instance.GetMaxStamina() && __instance.m_staminaRegenTimer <= 0f)
                            __instance.m_stamina = Mathf.Min(__instance.GetMaxStamina(), __instance.m_stamina + encumberedStaminaRegenerationMultiplier.Value * __instance.m_staminaRegen * dt * Game.m_staminaRegenRate);
                }
            }

            [HarmonyPriority(Priority.VeryHigh)]
            public static void Postfix(Player __instance)
            {
                if (!modEnabled.Value)
                    return;

                if (__instance.m_staminaRegenTimeMultiplier != _m_staminaRegenTimeMultiplier && _m_staminaRegenTimeMultiplier != 0f)
                    __instance.m_staminaRegenTimeMultiplier = _m_staminaRegenTimeMultiplier;

                if (__instance.m_encumberedStaminaDrain != _m_encumberedStaminaDrain && _m_encumberedStaminaDrain != 0f)
                    __instance.m_encumberedStaminaDrain = _m_encumberedStaminaDrain;
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.OnSwimming))]
        public static class Player_OnSwimming_SwimmingStamina
        {
            public static void Prefix(Player __instance, Vector3 targetVel, float dt)
            {
                if (!modEnabled.Value)
                    return;

                if (swimmingStaminaRegeneration.Value && targetVel.magnitude <= 0.1f)
                {
                    __instance.m_staminaRegenTimer -= dt;
                    if (__instance.GetStamina() < __instance.GetMaxStamina() && __instance.m_staminaRegenTimer <= -swimmingStaminaRegenerationDelay.Value)
                    {
                        float skillFactor = __instance.m_skills.GetSkillFactor(Skills.SkillType.Swim);
                        __instance.m_stamina = Mathf.Min(__instance.GetMaxStamina(), __instance.m_stamina + (1f + skillFactor) * swimmingStaminaRegenerationMultiplier.Value * __instance.m_staminaRegen * dt * Game.m_staminaRegenRate);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Character), nameof(Character.UpdateSwimming))]
        public static class Character_UpdateSwimming_SwimmingStamina
        {
            private static float _m_swimStaminaDrainMinSkill;
            private static float _m_swimStaminaDrainMaxSkill;
            private static float _m_swimSpeed;

            internal static float shiftSwimDownTime;
            internal static bool shiftSwimStaminaDepleted;

            [HarmonyPriority(Priority.VeryLow)]
            public static void Prefix(Character __instance, float dt)
            {
                if (!modEnabled.Value)
                    return;

                if (!swimmingStamina.Value)
                    return;

                if (__instance != Player.m_localPlayer || __instance.IsOnGround())
                    return;

                _m_swimStaminaDrainMinSkill = Player.m_localPlayer.m_swimStaminaDrainMinSkill;
                _m_swimStaminaDrainMaxSkill = Player.m_localPlayer.m_swimStaminaDrainMaxSkill;
                _m_swimSpeed = __instance.m_swimSpeed;

                Player.m_localPlayer.m_swimStaminaDrainMinSkill *= swimmingStaminaDrainMultiplier.Value;
                Player.m_localPlayer.m_swimStaminaDrainMaxSkill *= swimmingStaminaDrainMultiplier.Value;

                if (!swimmingRun.Value)
                    return;

                shiftSwimStaminaDepleted = shiftSwimStaminaDepleted || !Player.m_localPlayer.HaveStamina();

                bool run = (ZInput.GetButton("Run") || ZInput.GetButton("JoyRun"));
                if (run)
                {
                    shiftSwimDownTime += dt;
                    if (!shiftSwimStaminaDepleted)
                    {
                        float factor = Math.Abs(Mathf.Min(swimmingRunMaximumSpeed.Value - 1f, shiftSwimDownTime / 10));
                        __instance.m_swimSpeed *= 1f + factor;

                        Player.m_localPlayer.m_swimStaminaDrainMinSkill *= 1f + factor;
                        Player.m_localPlayer.m_swimStaminaDrainMaxSkill *= 1f + factor;
                    }
                }
                else
                {
                    shiftSwimDownTime = 0f;
                    shiftSwimStaminaDepleted = false;
                }
            }

            [HarmonyPriority(Priority.VeryHigh)]
            public static void Postfix(Character __instance)
            {
                if (!modEnabled.Value)
                    return;

                if (!swimmingStamina.Value)
                    return;

                if (__instance != Player.m_localPlayer || __instance.IsOnGround())
                    return;

                if (Player.m_localPlayer.m_swimSpeed != _m_swimSpeed && _m_swimSpeed != 0f)
                    Player.m_localPlayer.m_swimSpeed = _m_swimSpeed;

                if (Player.m_localPlayer.m_swimStaminaDrainMinSkill != _m_swimStaminaDrainMinSkill && _m_swimStaminaDrainMinSkill != 0f)
                    Player.m_localPlayer.m_swimStaminaDrainMinSkill = _m_swimStaminaDrainMinSkill;

                if (Player.m_localPlayer.m_swimStaminaDrainMaxSkill != _m_swimStaminaDrainMaxSkill && _m_swimStaminaDrainMaxSkill != 0f)
                    Player.m_localPlayer.m_swimStaminaDrainMaxSkill = _m_swimStaminaDrainMaxSkill;
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.OnSneaking))]
        public static class Player_OnSneaking_SneakingStamina
        {
            private static float _m_sneakStaminaDrain;

            public static bool IsEnemyInRange(Character me)
            {
                foreach (BaseAI instance in BaseAI.BaseAIInstances)
                {
                    if (!BaseAI.IsEnemy(me, instance.m_character))
                        continue;

                    if (Vector3.Distance(me.transform.position, instance.transform.position) < Math.Max(instance.m_viewRange, (instance.IsAlerted() ? 1.5f : 1) * sneakingStaminaNoEnemiesRange.Value))
                        return true;
                }

                return false;
            }

            [HarmonyPriority(Priority.VeryLow)]
            public static void Prefix(Player __instance)
            {
                if (!modEnabled.Value)
                    return;

                if (!sneakingStamina.Value)
                    return;

                if (__instance != Player.m_localPlayer)
                    return;

                _m_sneakStaminaDrain = __instance.m_sneakStaminaDrain;

                __instance.m_sneakStaminaDrain *= sneakingStaminaNoEnemies.Value && !IsEnemyInRange(__instance) ? 0f : sneakingStaminaDrainMultiplier.Value;

                if (__instance.m_sneakStaminaDrain == 0)
                    __instance.m_staminaRegenTimer = __instance.m_staminaRegenDelay;
            }

            [HarmonyPriority(Priority.VeryHigh)]
            public static void Postfix(Player __instance)
            {
                if (!modEnabled.Value)
                    return;

                if (!swimmingStamina.Value)
                    return;

                if (__instance != Player.m_localPlayer)
                    return;

                if (__instance.m_sneakStaminaDrain != _m_sneakStaminaDrain && _m_sneakStaminaDrain != 0f)
                    __instance.m_sneakStaminaDrain = _m_sneakStaminaDrain;
            }
        }

        [HarmonyPatch(typeof(Hud), nameof(Hud.UpdateStamina))]
        public class Hud_UpdateStamina_HideStaminaValue
        {
            public static void Prefix(Hud __instance)
            {
                if (__instance.m_staminaText == null)
                    return;

                __instance.m_staminaText.gameObject.SetActive(modEnabled.Value && !hideStaminaValue.Value);
            }
        }

        [HarmonyPatch(typeof(FishingFloat), nameof(FishingFloat.Awake))]
        public static class FishingFloat_Awake_FishingStaminaDrainMultipliers
        {
            private static void Postfix(FishingFloat __instance)
            {
                if (!modEnabled.Value)
                    return;

                __instance.m_pullStaminaUse *= pullStaminaUse.Value;
                __instance.m_hookedStaminaPerSec *= hookedStaminaPerSec.Value;

            }
        }

        [HarmonyPatch(typeof(SE_Harpooned), nameof(SE_Harpooned.Setup))]
        public static class SE_Harpooned_Setup_HarpoonedStaminaDrainMultiplier
        {
            private static void Postfix(SE_Harpooned __instance)
            {
                if (!modEnabled.Value)
                    return;

                __instance.m_staminaDrain *= harpoonedStaminaDrain.Value;

            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.UpdatePlacement))]
        public static class Player_UpdatePlacement_ToolsStaminaDrainMultiplier
        {
            [HarmonyPriority(Priority.VeryLow)]
            public static void Prefix(Player __instance, bool takeInput, ref float __state)
            {
                if (!modEnabled.Value)
                    return;

                ItemDrop.ItemData rightItem = __instance.GetRightItem();
                if (__instance.InPlaceMode() && !__instance.IsDead() && takeInput && rightItem != null)
                {
                    __state = rightItem.m_shared.m_attack.m_attackStamina;
                    rightItem.m_shared.m_attack.m_attackStamina *= toolStaminaDrain.Value;
                }
            }

            [HarmonyPriority(Priority.VeryHigh)]
            private static void Postfix(Player __instance, float __state)
            {
                if (!modEnabled.Value)
                    return;

                if (__state != 0f && __instance.GetRightItem() != null)
                    __instance.GetRightItem().m_shared.m_attack.m_attackStamina = __state;
            }
        }

        [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.BlockAttack))]
        public static class Humanoid_BlockAttack_BlockStaminaDrainMultiplier
        {
            [HarmonyPriority(Priority.VeryLow)]
            public static void Prefix(Humanoid __instance, ref float __state)
            {
                if (!modEnabled.Value)
                    return;

                __state = __instance.m_blockStaminaDrain;
                __instance.m_blockStaminaDrain *= blockStaminaDrain.Value;

                if (blockStaminaSkill.Value)
                    __instance.m_blockStaminaDrain *= (1f - 0.33f * __instance.GetSkillFactor(Skills.SkillType.Blocking));
            }

            [HarmonyPriority(Priority.VeryHigh)]
            private static void Postfix(Humanoid __instance, float __state)
            {
                if (!modEnabled.Value)
                    return;

                if (__state != 0f)
                    __instance.m_blockStaminaDrain = __state;
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.CheckRun))]
        public static class Player_CheckRun_RunStaminaDrainMultiplier
        {
            [HarmonyPriority(Priority.VeryLow)]
            public static void Prefix(Player __instance, ref float __state)
            {
                if (!modEnabled.Value)
                    return;

                __state = __instance.m_runStaminaDrain;
                __instance.m_runStaminaDrain *= runStaminaDrain.Value;
            }

            [HarmonyPriority(Priority.VeryHigh)]
            private static void Postfix(Player __instance, float __state)
            {
                if (!modEnabled.Value)
                    return;

                if (__state != 0f)
                    __instance.m_runStaminaDrain = __state;
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.UpdateDodge))]
        public static class Player_UpdateDodge_DodgeStaminaDrainMultiplier
        {
            [HarmonyPriority(Priority.VeryLow)]
            public static void Prefix(Player __instance, ref float __state)
            {
                if (!modEnabled.Value)
                    return;

                __state = __instance.m_dodgeStaminaUsage;
                __instance.m_dodgeStaminaUsage *= dodgeStaminaUsage.Value;

                if (dodgeStaminaSkill.Value)
                    __instance.m_dodgeStaminaUsage *= (1f - 0.33f * __instance.GetSkillFactor(Skills.SkillType.Jump));

                if (groundsEnabled.Value)
                    __instance.m_dodgeStaminaUsage *= SE_Surface.GetSurfaceMaterialStaminaDrain(SurfaceStaminaSpeed.currentSurface);
            }

            [HarmonyPriority(Priority.VeryHigh)]
            private static void Postfix(Player __instance, float __state)
            {
                if (!modEnabled.Value)
                    return;

                if (__state != 0f)
                    __instance.m_dodgeStaminaUsage = __state;
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.OnJump))]
        public static class Player_OnJump_JumpStaminaDrainMultiplier
        {
            [HarmonyPriority(Priority.VeryLow)]
            public static void Prefix(Player __instance, ref float __state)
            {
                if (!modEnabled.Value)
                    return;

                __state = __instance.m_jumpStaminaUsage;
                __instance.m_jumpStaminaUsage *= jumpStaminaUsage.Value;

                if (jumpStaminaSkill.Value)
                    __instance.m_jumpStaminaUsage *= (1f - 0.33f * __instance.GetSkillFactor(Skills.SkillType.Jump));
            }

            [HarmonyPriority(Priority.VeryHigh)]
            private static void Postfix(Player __instance, float __state)
            {
                if (!modEnabled.Value)
                    return;

                if (__state != 0f)
                    __instance.m_jumpStaminaUsage = __state;
            }
        }

    }
}