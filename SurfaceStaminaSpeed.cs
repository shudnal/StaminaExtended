using static StaminaExtended.StaminaExtended;
using HarmonyLib;
using static FootStep;
using UnityEngine;
using static WearNTear;
using System.Collections.Generic;
using static StaminaExtended.SurfaceStaminaSpeed;
using System.Linq;

namespace StaminaExtended
{
    public class SE_Surface : SE_Stats
    {
        public static Sprite statusEffectIcon = null;

        private SurfaceMaterial surfaceMaterial = SurfaceMaterial.None;

        public override void UpdateStatusEffect(float dt)
        {
            if (surfaceMaterial != currentSurface)
                Setup(m_character);
            else
                base.UpdateStatusEffect(dt);
        }

        public override void Setup(Character character)
        {
            surfaceMaterial = currentSurface;

            UpdateSurfaceStatusEffect();

            base.Setup(character);
        }

        public void UpdateSurfaceStatusEffect()
        {
            m_name = GetSurfaceMaterialName(surfaceMaterial);
            m_icon = groundsShowStatusEffect.Value ? statusEffectIcon : null;

            m_runStaminaDrainModifier = GetSurfaceMaterialStaminaDrain(surfaceMaterial) - 1f;
            m_jumpStaminaUseModifier = GetSurfaceMaterialStaminaDrain(surfaceMaterial) - 1f;
            m_staminaRegenMultiplier = GetSurfaceMaterialStaminaRegen(surfaceMaterial);
            m_speedModifier = GetSurfaceMaterialSpeed(surfaceMaterial) - 1f;
            m_jumpModifier = new Vector3(0f, GetSurfaceMaterialJump(surfaceMaterial) - 1f, 0f);
        }

        public static void UpdateSurfaceStatusEffectStats()
        {
            if (ObjectDB.instance == null)
                return;

            if (Player.m_localPlayer == null)
                return;

            (Player.m_localPlayer.GetSEMan().GetStatusEffect(statusEffectSurfaceHash) as SE_Surface)?.Setup(Player.m_localPlayer);
        }

        public static float GetSurfaceMaterialSpeed(SurfaceMaterial surfaceMaterial)
        {
            if (surfaceMaterial == SurfaceMaterial.None)
                return 1f;

            return instance.GetFloatConfig($"groundsSpeed{surfaceMaterial}");
        }

        public static float GetSurfaceMaterialJump(SurfaceMaterial surfaceMaterial)
        {
            if (surfaceMaterial == SurfaceMaterial.None)
                return 1f;

            return instance.GetFloatConfig($"groundsJump{surfaceMaterial}");
        }
        
        public static float GetSurfaceMaterialStaminaDrain(SurfaceMaterial surfaceMaterial)
        {
            if (surfaceMaterial == SurfaceMaterial.None)
                return 1f;

            return instance.GetFloatConfig($"groundsStaminaDrain{surfaceMaterial}");
        }

        public static float GetSurfaceMaterialStaminaRegen(SurfaceMaterial surfaceMaterial)
        {
            if (surfaceMaterial == SurfaceMaterial.None)
                return 1f;

            return instance.GetFloatConfig($"groundsStaminaRegen{surfaceMaterial}");
        }

        public static string GetSurfaceMaterialName(SurfaceMaterial surfaceMaterial)
        {
            return instance.GetStringConfig($"groundsName{(surfaceMaterial == SurfaceMaterial.None ? "Default" : surfaceMaterial.ToString())}");
        }
    }

    public class SurfaceStaminaSpeed
    {
        public enum SurfaceMaterial
        {
            None,
            Grass,
            Mud,
            Snow,
            Wood,
            HardWood,
            Stone,
            Marble,
            Metal,
            Cultivated,
            Cleared,
            Paved,
            Ash,
            Lava,
        }

        public static SurfaceMaterial currentSurface = SurfaceMaterial.None;
        public static Dictionary<Collider, WearNTear> colliderPieces = new Dictionary<Collider, WearNTear>();
        public static Dictionary<Collider, Heightmap> colliderHeightmaps = new Dictionary<Collider, Heightmap>();
        public static int colliderCacheCleared = 0;
        public static int pieceLayer;

        private static bool IsUpgradedMaterial(Collider collider)
        {
            WearNTear componentInParent = GetPiece(collider);
            return componentInParent != null && (componentInParent.m_materialType == MaterialType.HardWood || 
                                                 componentInParent.m_materialType == MaterialType.Marble || 
                                                 componentInParent.m_materialType == MaterialType.Ashstone || 
                                                 componentInParent.m_materialType == MaterialType.Ancient);
        }

        private static WearNTear GetPiece(Collider collider)
        {
            if ((int)(ZNet.instance.GetTimeSeconds() / 300f) != colliderCacheCleared)
            {
                colliderCacheCleared = (int)(ZNet.instance.GetTimeSeconds() / 300f);
                colliderPieces.Clear();
                colliderHeightmaps.Clear();
            }

            if (collider == null)
                return null;

            if (pieceLayer == 0)
                pieceLayer = LayerMask.NameToLayer("piece");

            if (collider.gameObject.layer != pieceLayer)
                return null;

            if (!colliderPieces.ContainsKey(collider))
                colliderPieces.Add(collider, collider.GetComponentInParent<WearNTear>());

            return colliderPieces[collider];
        }

        private static Heightmap GetHeightmap(Collider collider)
        {
            if ((int)(ZNet.instance.GetTimeSeconds() / 300f) != colliderCacheCleared)
            {
                colliderCacheCleared = (int)(ZNet.instance.GetTimeSeconds() / 300f);
                colliderPieces.Clear();
                colliderHeightmaps.Clear();
            }

            if (collider == null)
                return null;

            if (!colliderHeightmaps.ContainsKey(collider))
                colliderHeightmaps.Add(collider, collider.GetComponent<Heightmap>());

            return colliderHeightmaps[collider];
        }

        private static SurfaceMaterial GetModifiedGround(Collider collider, Vector3 worldPos)
        {
            Heightmap heightmap = GetHeightmap(collider);
            if (heightmap == null)
                return SurfaceMaterial.None;

            heightmap.WorldToVertex(worldPos, out int x, out int y);
            if (heightmap.m_paintMask.GetPixel(x, y).g > 0.5f)
                return SurfaceMaterial.Cultivated;

            worldPos.x = (float)(worldPos.x - 0.5);
            worldPos.z = (float)(worldPos.z - 0.5);
            heightmap.WorldToVertex(worldPos, out x, out y);

            Color pixel = heightmap.m_paintMask.GetPixel(x, y);
            if (pixel.r > 0.5f)
                return SurfaceMaterial.Cleared;
            else if (pixel.b > 0.5f)
                return SurfaceMaterial.Paved;
            else if (pixel.g > 0.5f)
                return SurfaceMaterial.Cultivated;

            return SurfaceMaterial.None;
        }

        private static void CheckCurrentGround(Character character)
        {
            Collider collider = character.GetLastGroundCollider();
            if (collider == null)
                return;

            currentSurface = GetModifiedGround(collider, character.transform.position);
            if (currentSurface != SurfaceMaterial.None)
                return;

            if (collider.gameObject.layer != pieceLayer)
                return;

            WearNTear componentInParent = GetPiece(collider);
            if (componentInParent != null)
                currentSurface = GetSurfaceMaterial(componentInParent.m_materialType);
        }

        public static SurfaceMaterial GetSurfaceMaterial(MaterialType materialType)
        {
            return materialType switch
            {
                MaterialType.Wood => SurfaceMaterial.Wood,
                MaterialType.Stone => SurfaceMaterial.Stone,
                MaterialType.Marble => SurfaceMaterial.Marble,
                MaterialType.HardWood => SurfaceMaterial.HardWood,
                MaterialType.Iron => SurfaceMaterial.Metal,
                MaterialType.Ashstone => SurfaceMaterial.Marble,
                MaterialType.Ancient => SurfaceMaterial.Marble,
                _ => SurfaceMaterial.None
            };
        }

        [HarmonyPatch(typeof(FootStep), nameof(FootStep.GetGroundMaterial))]
        private static class FootStep_GetGroundMaterial_GroundDetection
        {
            private static void Postfix(Character character, Vector3 point, GroundMaterial __result)
            {
                if (!modEnabled.Value)
                    return;

                if (character != Player.m_localPlayer)
                    return;

                Collider collider = character.GetLastGroundCollider();
                if (collider == null)
                {
                    currentSurface = SurfaceMaterial.None;
                    return;
                }

                switch (__result)
                {
                    case GroundMaterial.None:
                    case GroundMaterial.Water:
                    case GroundMaterial.Tar:
                        currentSurface = SurfaceMaterial.None;
                        return;
                    case GroundMaterial.Ashlands:
                        currentSurface = SurfaceMaterial.Ash;
                        return;
                    case GroundMaterial.Lava:
                        currentSurface = SurfaceMaterial.Lava;
                        return;
                    case GroundMaterial.Snow:
                        currentSurface = SurfaceMaterial.Snow;
                        return;
                    case GroundMaterial.Mud:
                        currentSurface = GetModifiedGround(collider, point);
                        if (currentSurface == SurfaceMaterial.None)
                            currentSurface = SurfaceMaterial.Mud;
                        return;
                    case GroundMaterial.Grass:
                        currentSurface = GetModifiedGround(collider, point);
                        if (currentSurface == SurfaceMaterial.None)
                            currentSurface = SurfaceMaterial.Grass;
                        return;
                    case GroundMaterial.Metal:
                        currentSurface = SurfaceMaterial.Metal;
                        return;
                    case GroundMaterial.Wood:
                        currentSurface = IsUpgradedMaterial(collider) ? SurfaceMaterial.HardWood : SurfaceMaterial.Wood;
                        return;
                    case GroundMaterial.Stone:
                        currentSurface = IsUpgradedMaterial(collider) ? SurfaceMaterial.Marble : SurfaceMaterial.Stone;
                        return;
                    case GroundMaterial.GenericGround:
                        currentSurface = GetModifiedGround(collider, point);
                        if (currentSurface == SurfaceMaterial.None && IsPlainsGrass())
                            currentSurface = SurfaceMaterial.Grass;
                        return;
                    case GroundMaterial.Default:
                        WearNTear componentInParent = GetPiece(collider);
                        if (componentInParent != null)
                            currentSurface = GetSurfaceMaterial(componentInParent.m_materialType);
                        return;
                }

                currentSurface = SurfaceMaterial.None;

                bool IsPlainsGrass()
                {
                    Heightmap heightmap = GetHeightmap(collider);
                    if (heightmap == null)
                        return false;

                    if (heightmap.GetBiome(point) != Heightmap.Biome.Plains)
                        return false;

                    return Mathf.Acos(Mathf.Clamp01(character.GetLastGroundNormal().y)) * 57.29578f < 40f;
                }
            }
        }

        [HarmonyPatch(typeof(ObjectDB), nameof(ObjectDB.Awake))]
        public static class ObjectDB_Awake_AddStatusEffects
        {
            public static void AddCustomStatusEffects(ObjectDB odb)
            {
                if (odb.m_StatusEffects.Count > 0)
                {
                    if (!odb.m_StatusEffects.Any(se => se.name == statusEffectSurfaceName))
                    {
                        SE_Surface seasonEffect = ScriptableObject.CreateInstance<SE_Surface>();
                        seasonEffect.name = statusEffectSurfaceName;
                        seasonEffect.m_nameHash = statusEffectSurfaceHash;
                        seasonEffect.m_icon = SE_Surface.statusEffectIcon;

                        odb.m_StatusEffects.Add(seasonEffect);
                    }
                }
            }

            private static void Postfix(ObjectDB __instance)
            {
                AddCustomStatusEffects(__instance);
            }
        }

        [HarmonyPatch(typeof(ObjectDB), nameof(ObjectDB.CopyOtherDB))]
        public static class ObjectDB_CopyOtherDB_SE_Season
        {
            private static void Postfix(ObjectDB __instance)
            {
                ObjectDB_Awake_AddStatusEffects.AddCustomStatusEffects(__instance);
            }
        }

        [HarmonyPatch(typeof(Skills), nameof(Skills.Awake))]
        public static class Skills_Awake_SE_Season
        {
            private static void Postfix(Skills __instance)
            {
                if (SE_Surface.statusEffectIcon == null)
                    __instance.m_skills.DoIf(skill => skill.m_skill == Skills.SkillType.Run, skill => SE_Surface.statusEffectIcon = skill.m_icon);
            }
        }

        [HarmonyPatch(typeof(TextsDialog), nameof(TextsDialog.AddActiveEffects))]
        public static class TextsDialog_AddActiveEffects_StatusTooltipWhenBuffDisabled
        {
            public static bool isActiveEffectsListCall = false;

            private static void Prefix()
            {
                isActiveEffectsListCall = groundsEnabled.Value && !groundsShowStatusEffect.Value;
            }

            private static void Postfix()
            {
                isActiveEffectsListCall = false;
            }
        }

        [HarmonyPatch(typeof(SEMan), nameof(SEMan.GetHUDStatusEffects))]
        public static class SEMan_GetHUDStatusEffects_SeasonTooltipWhenBuffDisabled
        {
            private static void Postfix(Character ___m_character, List<StatusEffect> ___m_statusEffects, List<StatusEffect> effects)
            {
                if (TextsDialog_AddActiveEffects_StatusTooltipWhenBuffDisabled.isActiveEffectsListCall && Player.m_localPlayer != null && ___m_character == Player.m_localPlayer && !effects.Any(effect => effect is SE_Surface))
                {
                    StatusEffect seasonStatusEffect = ___m_statusEffects.Find(se => se is SE_Surface);
                    if (seasonStatusEffect != null)
                        effects.Add(seasonStatusEffect);
                }
            }
        }

        [HarmonyPatch(typeof(Character), nameof(Character.UpdateGroundContact))]
        public static class Character_UpdateGroundContact_GroundDetectionOnContact
        {
            private static void Prefix(Character __instance, float ___m_maxAirAltitude, ref float __state)
            {
                if (!groundsEnabled.Value || __instance != Player.m_localPlayer)
                    return;

                __state = Mathf.Max(0f, ___m_maxAirAltitude - __instance.transform.position.y);
            }

            private static void Postfix(Character __instance, float ___m_maxAirAltitude, float __state)
            {
                if (__state == 0f)
                    return;

                if (__state >= 0.2f && ___m_maxAirAltitude == __instance.transform.position.y && __instance.IsOnGround() && currentSurface == SurfaceMaterial.None)
                    CheckCurrentGround(__instance);
            }
        }
    }
}
