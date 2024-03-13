using static StaminaExtended.StaminaExtended;
using HarmonyLib;
using System;
using static ItemDrop;
using static ItemDrop.ItemData;

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

        private static bool IsFoodItemForExtraStaminaRegeneration(ItemData item)
        {
            return extraStaminaRegeneration.Value && Player.m_localPlayer != null && item.m_shared.m_itemType == ItemType.Consumable && item.m_shared.m_foodStamina > 0f;
        }

        private static float GetAdditionalBaseStamina(Player player)
        {
            if (!baseStamina.Value)
                return 0f;

            return player.GetSkillFactor(Skills.SkillType.Run) * runBaseStaminaIncrease.Value +
                   player.GetSkillFactor(Skills.SkillType.Swim) * swimBaseStaminaIncrease.Value +
                   player.GetSkillFactor(Skills.SkillType.Fishing) * fishingBaseStaminaIncrease.Value +
                   player.GetSkillFactor(Skills.SkillType.Sneak) * sneakBaseStaminaIncrease.Value +
                   player.GetSkillFactor(Skills.SkillType.Jump) * jumpBaseStaminaIncrease.Value;
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

        [HarmonyPatch(typeof(ItemData), nameof(ItemData.GetTooltip), new Type[] { typeof(ItemData), typeof(int), typeof(bool), typeof(float) })]
        public static class ItemDrop_ItemData_GetTooltip_StaminaRegenTooltipForFoodRegen
        {
            public static ItemData addStaminaRegenTooltip;

            [HarmonyPriority(Priority.First)]
            private static void Prefix(ItemData item)
            {
                if (!modEnabled.Value)
                    return;

                if (!IsFoodItemForExtraStaminaRegeneration(item))
                    return;

                addStaminaRegenTooltip = item;
            }

            [HarmonyPriority(Priority.First)]
            private static void Postfix()
            {
                addStaminaRegenTooltip = null;
            }
        }

        [HarmonyPatch(typeof(ItemData), nameof(ItemData.GetStatusEffectTooltip))]
        public static class ItemDrop_ItemData_GetStatusEffectTooltip_StaminaRegenTooltipForFoodRegen
        {
            [HarmonyPriority(Priority.First)]
            private static void Prefix(ItemData __instance)
            {
                if (!modEnabled.Value)
                    return;

                if (__instance != ItemDrop_ItemData_GetTooltip_StaminaRegenTooltipForFoodRegen.addStaminaRegenTooltip)
                    return;

                m_stringBuilder.AppendFormat("\n$se_staminaregen: <color=#ffff80ff>{0:P1}</color> ($item_current:<color=yellow>{1:P1}</color>)",
                                                GetStaminaRegenerationValueFromStaminaPoints(__instance.m_shared.m_foodStamina),
                                                GetMultiplier(Player.m_localPlayer));
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
