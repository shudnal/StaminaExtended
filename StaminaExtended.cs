using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using ServerSync;

namespace StaminaExtended
{
    [BepInPlugin(pluginID, pluginName, pluginVersion)]
    public class StaminaExtended : BaseUnityPlugin
    {
        public const string pluginID = "shudnal.StaminaExtended";
        public const string pluginName = "Stamina Extended";
        public const string pluginVersion = "1.0.7";

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
        public static ConfigEntry<float> handsBaseStaminaIncrease;

        public static ConfigEntry<float> pullStaminaUse;
        public static ConfigEntry<float> hookedStaminaPerSec;
        public static ConfigEntry<float> harpoonedStaminaDrain;
        public static ConfigEntry<float> toolStaminaDrain;
        public static ConfigEntry<float> blockStaminaDrain;
        public static ConfigEntry<float> runStaminaDrain;
        public static ConfigEntry<float> dodgeStaminaUsage;
        public static ConfigEntry<float> jumpStaminaUsage;

        public static ConfigEntry<float> sneakingStaminaUsageOutOfCombat;
        public static ConfigEntry<float> runStaminaDrainOutOfCombat;
        public static ConfigEntry<float> dodgeStaminaUsageOutOfCombat;
        public static ConfigEntry<float> jumpStaminaUsageOutOfCombat;

        public static ConfigEntry<bool> hideStaminaValue;
        public static ConfigEntry<bool> blockStaminaSkill;
        public static ConfigEntry<bool> dodgeStaminaSkill;
        public static ConfigEntry<bool> jumpStaminaSkill;
        public static ConfigEntry<float> blockStaminaRegen;
        public static ConfigEntry<float> perfectParryStaminaDrain;

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

        private void Awake()
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

        private void OnDestroy()
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
            handsBaseStaminaIncrease = config("7 - Base stamina", "Unarmed", 0f, "Base stamina will be increased by set value when Unarmed skill is level 100");

            pullStaminaUse = config("8 - Various multipliers", "Fishing pull stamina", 1f, "Stamina required to reel");
            hookedStaminaPerSec = config("8 - Various multipliers", "Fishing hooked stamina", 1f, "Stamina required to keep a fish on the line");
            harpoonedStaminaDrain = config("8 - Various multipliers", "Harpooned pull stamina", 1f, "Stamina required to pull harpooned target");
            toolStaminaDrain = config("8 - Various multipliers", "Tools stamina drain", 1f, "Stamina required to use tools such as hammer, hoe, cultivator");
            blockStaminaDrain = config("8 - Various multipliers", "Block stamina drain", 1f, "Stamina required to block");
            runStaminaDrain = config("8 - Various multipliers", "Run stamina drain", 1f, "Stamina required to run");
            dodgeStaminaUsage = config("8 - Various multipliers", "Dodge stamina usage", 1f, "Stamina required to dodge");
            jumpStaminaUsage = config("8 - Various multipliers", "Jump stamina usage", 1f, "Stamina required to jump");

            runStaminaDrainOutOfCombat = config("8 - Various multipliers - Out of combat", "Run stamina drain", 1f, "Stamina required to run");
            dodgeStaminaUsageOutOfCombat = config("8 - Various multipliers - Out of combat", "Dodge stamina usage", 1f, "Stamina required to dodge");
            jumpStaminaUsageOutOfCombat = config("8 - Various multipliers - Out of combat", "Jump stamina usage", 1f, "Stamina required to jump");
            sneakingStaminaUsageOutOfCombat = config("8 - Various multipliers - Out of combat", "Sneak stamina usage", 1f, "Stamina required to sneak");

            hideStaminaValue = config("9 - Misc", "Hide stamina text", false, "Hide stamina text value on stamina bar");
            blockStaminaSkill = config("9 - Misc", "Block stamina usage depends on skill", true, "Amount of stamina needed to block is reduced by 33% when Block skill is 100");
            dodgeStaminaSkill = config("9 - Misc", "Dodge stamina usage depends on Jump skill", true, "Amount of stamina needed to dodge is reduced by 33% when Dodge skill is 100");
            jumpStaminaSkill = config("9 - Misc", "Jump stamina usage depends on Jump skill", true, "Amount of stamina needed to jump is reduced by 33% when Jump skill is 100");
            blockStaminaRegen = config("9 - Misc", "Stamina regen multiplier while blocking", 0.8f, "Stamina regeneration rate while holding block");
            perfectParryStaminaDrain = config("9 - Misc", "Stamina usage multiplier on perfect parry", 0.5f, "Amount of stamina needed to block if it is perfect parry.");

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
    }
}