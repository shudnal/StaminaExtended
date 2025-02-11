using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using UnityEngine;
using static StaminaExtended.StaminaExtended;

namespace StaminaExtended
{
    internal static class StaminaMultipliers
    {
        private static bool IsOutOfCombat() => Player.m_localPlayer != null && !Player.m_localPlayer.IsSensed() && !Player.m_localPlayer.IsTargeted() && Player.m_localPlayer.CanSwitchPVP();

        private static float GetSneakStaminaDrainMultiplier() => Math.Max(IsOutOfCombat() ? sneakingStaminaUsageOutOfCombat.Value : sneakingStaminaDrainMultiplier.Value, 0.01f);

        private static float GetRunStaminaDrainMultiplier() => Math.Max(IsOutOfCombat() ? runStaminaDrainOutOfCombat.Value : runStaminaDrain.Value, 0.01f);

        private static float GetDodgeStaminaDrainMultiplier() => Math.Max(IsOutOfCombat() ? dodgeStaminaUsageOutOfCombat.Value : dodgeStaminaUsage.Value, 0.01f);

        private static float GetJumpStaminaDrainMultiplier() => Math.Max(IsOutOfCombat() ? jumpStaminaUsageOutOfCombat.Value : jumpStaminaUsage.Value, 0.01f);

        [HarmonyPatch(typeof(Player), nameof(Player.UpdateStats), typeof(float))]
        public static class Player_UpdateStats_StaminaRegenMultiplier
        {
            private static float _m_encumberedStaminaDrain;
            private static float _m_staminaRegenTimeMultiplier;
            private static float _m_staminaRegen;
            private static float _blockStaminaRegen = 0.8f;

            [HarmonyPriority(Priority.VeryLow)]
            public static void Prefix(Player __instance, float dt)
            {
                if (!modEnabled.Value)
                    return;

                if (__instance.InIntro() || __instance.IsTeleporting())
                    return;

                _blockStaminaRegen = blockStaminaRegen.Value;

                _m_staminaRegenTimeMultiplier = __instance.m_staminaRegenTimeMultiplier;
                _m_staminaRegen = __instance.m_staminaRegen;

                if (extraStaminaRegeneration.Value && extraStaminaRegenerationPercent.Value > 0f && extraStaminaRegenerationPoints.Value > 0)
                    __instance.m_staminaRegen *= 1f + ExtraStamina.GetMultiplier(__instance);

                if (sneakingStamina.Value && __instance.IsSneaking())
                    __instance.m_staminaRegen *= 1f + sneakingStaminaRegenerationMultiplier.Value * ExtraStamina.GetSkillFactor(__instance, Skills.SkillType.Sneak);

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

            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
            {
                var codes = new List<CodeInstruction>(instructions);

                MethodInfo isBlockingMethod = AccessTools.Method(typeof(Player), nameof(Player.IsBlocking));
                FieldInfo customMultiplierField = AccessTools.Field(typeof(Player_UpdateStats_StaminaRegenMultiplier), "_blockStaminaRegen");

                for (int i = 0; i < codes.Count - 1; i++)
                {
                    if (codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 0.8f)
                    {
                        codes[i] = new CodeInstruction(OpCodes.Ldsfld, customMultiplierField);
                        break;
                    }
                }

                return codes.AsEnumerable();
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

                if (__instance.m_staminaRegen != _m_staminaRegen && _m_staminaRegen != 0f)
                    __instance.m_staminaRegen = _m_staminaRegen;
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
                        float skillFactor = ExtraStamina.GetSkillFactor(__instance, Skills.SkillType.Swim);
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

                __instance.m_sneakStaminaDrain *= sneakingStaminaNoEnemies.Value && !IsEnemyInRange(__instance) ? 0f : GetSneakStaminaDrainMultiplier();

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
            private static float GetBlockingMultiplier(Humanoid human)
            {
                if (human.GetCurrentBlocker() is ItemDrop.ItemData currentBlocker && currentBlocker.m_shared.m_timedBlockBonus > 1f && human.m_blockTimer != -1f && human.m_blockTimer < 0.25f)
                    return perfectParryStaminaDrain.Value;

                return blockStaminaDrain.Value;
            }

            [HarmonyPriority(Priority.VeryLow)]
            public static void Prefix(Humanoid __instance, ref float __state)
            {
                if (!modEnabled.Value || __instance != Player.m_localPlayer)
                    return;

                __state = __instance.m_blockStaminaDrain;
                __instance.m_blockStaminaDrain *= GetBlockingMultiplier(__instance);

                if (blockStaminaSkill.Value)
                    __instance.m_blockStaminaDrain *= 1f - 0.33f * ExtraStamina.GetSkillFactor(__instance as Player, Skills.SkillType.Blocking);
            }

            [HarmonyPriority(Priority.VeryHigh)]
            private static void Postfix(Humanoid __instance, float __state)
            {
                if (!modEnabled.Value || __instance != Player.m_localPlayer)
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

                __instance.m_runStaminaDrain *= GetRunStaminaDrainMultiplier();
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
                __instance.m_dodgeStaminaUsage *= GetDodgeStaminaDrainMultiplier();

                if (dodgeStaminaSkill.Value)
                    __instance.m_dodgeStaminaUsage *= (1f - 0.33f * ExtraStamina.GetSkillFactor(__instance, Skills.SkillType.Jump));

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
                __instance.m_jumpStaminaUsage *= GetJumpStaminaDrainMultiplier();

                if (jumpStaminaSkill.Value)
                    __instance.m_jumpStaminaUsage *= 1f - 0.33f * ExtraStamina.GetSkillFactor(__instance, Skills.SkillType.Jump);
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
