using static StaminaExtended.StaminaExtended;
using HarmonyLib;
using System;
using static ItemDrop;

namespace StaminaExtended
{
    internal static class ExtraStamina
    {
        public static float GetMultiplier(Player player)
        {
            float maxStam = player.GetMaxStamina();
            if (extraStaminaRegenerationOnlyFood.Value)
                player.GetTotalFoodValue(out _, out maxStam, out _);

            return GetStaminaRegenerationValueFromStaminaPoints(maxStam - player.m_baseStamina - GetAdditionalBaseStamina(player));
        }

        private static float GetStaminaRegenerationValueFromStaminaPoints(float points)
        {
            return (extraStaminaRegenerationPercent.Value / 100f) * (points) / extraStaminaRegenerationPoints.Value;
        }

        private static bool IsFoodItemForExtraStaminaRegeneration(ItemData item, out float foodStamina)
        {
            foodStamina = 0f;

            if (!extraStaminaRegeneration.Value || Player.m_localPlayer == null)
                return false;

            if (item.m_shared.m_itemType == ItemData.ItemType.Consumable)
                foodStamina = item.m_shared.m_foodStamina;

            return foodStamina > 0 || item.m_shared.m_appendToolTip != null && IsFoodItemForExtraStaminaRegeneration(item.m_shared.m_appendToolTip.m_itemData, out foodStamina);
        }

        private static float GetAdditionalBaseStamina(Player player)
        {
            if (!baseStamina.Value)
                return 0f;

            return player.GetSkillFactor(Skills.SkillType.Run) * runBaseStaminaIncrease.Value +
                   player.GetSkillFactor(Skills.SkillType.Swim) * swimBaseStaminaIncrease.Value +
                   player.GetSkillFactor(Skills.SkillType.Fishing) * fishingBaseStaminaIncrease.Value +
                   player.GetSkillFactor(Skills.SkillType.Sneak) * sneakBaseStaminaIncrease.Value +
                   player.GetSkillFactor(Skills.SkillType.Jump) * jumpBaseStaminaIncrease.Value +
                   player.GetSkillFactor(Skills.SkillType.Unarmed) * handsBaseStaminaIncrease.Value;
        }

        [HarmonyPatch(typeof(Player), nameof(Player.GetTotalFoodValue))]
        public static class Player_GetTotalFoodValue_BaseStaminaIncrease
        {
            [HarmonyPriority(Priority.VeryLow)]
            public static void Prefix(Player __instance, ref float __state)
            {
                if (!modEnabled.Value)
                    return;

                if (!baseStamina.Value)
                    return;

                if (__instance != Player.m_localPlayer)
                    return;

                __state = __instance.m_baseStamina;

                __instance.m_baseStamina += GetAdditionalBaseStamina(__instance);
            }

            [HarmonyPriority(Priority.VeryHigh)]
            public static void Postfix(Player __instance, float __state)
            {
                if (!modEnabled.Value)
                    return;

                if (__state != 0)
                    __instance.m_baseStamina = __state;
            }
        }

        [HarmonyPatch(typeof(ItemData), nameof(ItemData.GetTooltip), typeof(ItemData), typeof(int), typeof(bool), typeof(float), typeof(int))]
        public static class ItemDrop_ItemData_GetTooltip_StaminaRegenTooltipForFoodRegen
        {
            private static string[] tooltipTokens = new string[] { "$item_food_regen", "$item_food_duration", "$item_food_eitr", "$item_food_stamina" };

            [HarmonyPriority(Priority.First)]
            [HarmonyBefore("shudnal.MyLittleUI")]
            private static void Postfix(ItemData item, ref string __result)
            {
                if (!modEnabled.Value)
                    return;

                if (!IsFoodItemForExtraStaminaRegeneration(item, out float foodStamina))
                    return;

                int index = -1;
                foreach (string tailString in tooltipTokens)
                {
                    index = __result.IndexOf(tailString, StringComparison.InvariantCulture);
                    if (index != -1)
                        break;
                }

                if (index == -1)
                    return;

                string tooltip = String.Format("\n$se_staminaregen: <color=#ffff80ff>{0:P1}</color> ($item_current:<color=yellow>{1:P1}</color>)",
                                                GetStaminaRegenerationValueFromStaminaPoints(foodStamina),
                                                GetMultiplier(Player.m_localPlayer));

                int i = __result.IndexOf("\n", index, StringComparison.InvariantCulture);
                if (i != -1)
                    __result.Insert(i, tooltip);
                else
                    __result += tooltip;
            }
        }

        [HarmonyPatch(typeof(TextsDialog), nameof(TextsDialog.AddActiveEffects))]
        public static class TextsDialog_AddActiveEffects_SeasonTooltipWhenBuffDisabled
        {
            private static void Postfix(TextsDialog __instance)
            {
                if (!modEnabled.Value)
                    return;

                if (Player.m_localPlayer == null)
                    return;

                float multiplier = GetMultiplier(Player.m_localPlayer);
                if (multiplier < 0.01f)
                    return;

                __instance.m_texts[0].m_text += Localization.instance.Localize($"\n$se_staminaregen ({(extraStaminaRegenerationOnlyFood.Value ? "$item_food" : "$hud_misc")}): <color=orange>{multiplier:P1}</color>");
            }
        }

    }
}
