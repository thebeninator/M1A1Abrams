using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using GHPC.Effects;
using GHPC.Weapons;
using MelonLoader;
using UnityEngine;

namespace M1A1Abrams
{
    public class Ammo_120mm
    {
        public static Dictionary<string, AmmoClipCodexScriptable> ap;
        public static Dictionary<string, AmmoClipCodexScriptable> heat;

        public static AmmoClipCodexScriptable clip_codex_m830;
        public static AmmoType.AmmoClip clip_m830;
        public static AmmoCodexScriptable ammo_codex_m830;
        public static AmmoType ammo_m830;

        public static AmmoClipCodexScriptable clip_codex_m830a1;
        public static AmmoType.AmmoClip clip_m830a1;
        public static AmmoCodexScriptable ammo_codex_m830a1;
        public static AmmoType ammo_m830a1;
        public static AmmoType m830a1_forward_frag = new AmmoType();

        public static AmmoClipCodexScriptable clip_codex_m827;
        public static AmmoType.AmmoClip clip_m827;
        public static AmmoCodexScriptable ammo_codex_m827;
        public static AmmoType ammo_m827;

        public static AmmoClipCodexScriptable clip_codex_m829;
        public static AmmoType.AmmoClip clip_m829;
        public static AmmoCodexScriptable ammo_codex_m829;
        public static AmmoType ammo_m829;

        public static AmmoClipCodexScriptable clip_codex_m829a1;
        public static AmmoType.AmmoClip clip_m829a1;
        public static AmmoCodexScriptable ammo_codex_m829a1;
        public static AmmoType ammo_m829a1;

        public static AmmoClipCodexScriptable clip_codex_m829a2;
        public static AmmoType.AmmoClip clip_m829a2;
        public static AmmoCodexScriptable ammo_codex_m829a2;
        public static AmmoType ammo_m829a2;

        public static AmmoClipCodexScriptable clip_codex_m829a3;
        public static AmmoType.AmmoClip clip_m829a3;
        public static AmmoCodexScriptable ammo_codex_m829a3;
        public static AmmoType ammo_m829a3;

        public static AmmoClipCodexScriptable clip_codex_m1028;
        public static AmmoType.AmmoClip clip_m1028;
        public static AmmoCodexScriptable ammo_codex_m1028;
        public static AmmoType ammo_m1028;

        public static GameObject ammo_m827_vis = null;
        public static GameObject ammo_m829_vis = null;
        public static GameObject ammo_m829a1_vis = null;
        public static GameObject ammo_m829a2_vis = null;
        public static GameObject ammo_m829a3_vis = null;


        public static GameObject ammo_m1028_vis = null;
        public static GameObject ammo_m830_vis = null;
        public static GameObject ammo_m830a1_vis = null;

        public static AmmoType canister_ball;

        public static void Init()
        {
            if (ammo_m827 != null) return;

            // xm827
            ammo_m827 = new AmmoType();
            Util.ShallowCopy(ammo_m827, Assets.ammo_m833);
            ammo_m827.Name = "M827 APFSDS-T";
            ammo_m827.Caliber = 120;
            ammo_m827.RhaPenetration = 500f;
            ammo_m827.MuzzleVelocity = 1650f;
            ammo_m827.Mass = 4.64f;
            ammo_m827.SectionalArea = 0.0012f;
            ammo_m827.SpallMultiplier = 1.15f;
            ammo_m827.CachedIndex = -1;

            ammo_codex_m827 = ScriptableObject.CreateInstance<AmmoCodexScriptable>();
            ammo_codex_m827.AmmoType = ammo_m827;
            ammo_codex_m827.name = "ammo_m827";

            clip_m827 = new AmmoType.AmmoClip();
            clip_m827.Capacity = 1;
            clip_m827.Name = "M827 APFSDS-T";
            clip_m827.MinimalPattern = new AmmoCodexScriptable[1];
            clip_m827.MinimalPattern[0] = ammo_codex_m827;

            clip_codex_m827 = ScriptableObject.CreateInstance<AmmoClipCodexScriptable>();
            clip_codex_m827.name = "clip_m827";
            clip_codex_m827.CompatibleWeaponSystems = new WeaponSystemCodexScriptable[1];
            clip_codex_m827.ClipType = clip_m827;

            // m829 
            ammo_m829 = new AmmoType();
            Util.ShallowCopy(ammo_m829, Assets.ammo_m833);
            ammo_m829.Name = "M829 APFSDS-T";
            ammo_m829.Caliber = 120;
            ammo_m829.RhaPenetration = 530f;
            ammo_m829.MuzzleVelocity = 1670f;
            ammo_m829.Mass = 4.27f;
            ammo_m829.SectionalArea = 0.0009f;
            ammo_m829.SpallMultiplier = 1.15f;
            ammo_m829.Coeff = 0.20f;
            ammo_m829.CachedIndex = -1;

            ammo_codex_m829 = ScriptableObject.CreateInstance<AmmoCodexScriptable>();
            ammo_codex_m829.AmmoType = ammo_m829;
            ammo_codex_m829.name = "ammo_m829";

            clip_m829 = new AmmoType.AmmoClip();
            clip_m829.Capacity = 1;
            clip_m829.Name = "M829 APFSDS-T";
            clip_m829.MinimalPattern = new AmmoCodexScriptable[1];
            clip_m829.MinimalPattern[0] = ammo_codex_m829;

            clip_codex_m829 = ScriptableObject.CreateInstance<AmmoClipCodexScriptable>();
            clip_codex_m829.name = "clip_m829";
            clip_codex_m829.CompatibleWeaponSystems = new WeaponSystemCodexScriptable[1];
            clip_codex_m829.ClipType = clip_m829;

            // m829a1
            ammo_m829a1 = new AmmoType();
            Util.ShallowCopy(ammo_m829a1, Assets.ammo_m833);
            ammo_m829a1.Name = "M829A1 APFSDS-T";
            ammo_m829a1.Caliber = 120;
            ammo_m829a1.RhaPenetration = 580;
            ammo_m829a1.SpallMultiplier = 1.15f;
            ammo_m829a1.MaxSpallRha = 22f;
            ammo_m829a1.MinSpallRha = 5f;
            ammo_m829a1.MuzzleVelocity = 1575;
            ammo_m829a1.Mass = 4.6f;
            ammo_m829a1.SectionalArea = 0.00082f;
            ammo_m829a1.Coeff = 0.20f;
            ammo_m829a1.CachedIndex = -1;

            ammo_codex_m829a1 = ScriptableObject.CreateInstance<AmmoCodexScriptable>();
            ammo_codex_m829a1.AmmoType = ammo_m829a1;
            ammo_codex_m829a1.name = "ammo_m829a1";

            clip_m829a1 = new AmmoType.AmmoClip();
            clip_m829a1.Capacity = 1;
            clip_m829a1.Name = "M829A1 APFSDS-T";
            clip_m829a1.MinimalPattern = new AmmoCodexScriptable[1];
            clip_m829a1.MinimalPattern[0] = ammo_codex_m829a1;

            clip_codex_m829a1 = ScriptableObject.CreateInstance<AmmoClipCodexScriptable>();
            clip_codex_m829a1.name = "clip_m829a1";
            clip_codex_m829a1.CompatibleWeaponSystems = new WeaponSystemCodexScriptable[1];
            clip_codex_m829a1.ClipType = clip_m829a1;

            // m829a2 
            ammo_m829a2 = new AmmoType();
            Util.ShallowCopy(ammo_m829a2, Assets.ammo_m833);
            ammo_m829a2.Name = "M829A2 APFSDS-T";
            ammo_m829a2.Caliber = 120;
            ammo_m829a2.RhaPenetration = 605f;
            ammo_m829a2.SpallMultiplier = 1.15f;
            ammo_m829a2.MaxSpallRha = 22f;
            ammo_m829a2.MinSpallRha = 5f;
            ammo_m829a2.MuzzleVelocity = 1680f;
            ammo_m829a2.Mass = 4.6f;
            ammo_m829a2.SectionalArea = 0.00082f;
            ammo_m829a2.Coeff = 0.20f;
            ammo_m829a2.CachedIndex = -1;

            ammo_codex_m829a2 = ScriptableObject.CreateInstance<AmmoCodexScriptable>();
            ammo_codex_m829a2.AmmoType = ammo_m829a2;
            ammo_codex_m829a2.name = "ammo_m829a2";

            clip_m829a2 = new AmmoType.AmmoClip();
            clip_m829a2.Capacity = 1;
            clip_m829a2.Name = "M829A2 APFSDS-T";
            clip_m829a2.MinimalPattern = new AmmoCodexScriptable[1];
            clip_m829a2.MinimalPattern[0] = ammo_codex_m829a2;

            clip_codex_m829a2 = ScriptableObject.CreateInstance<AmmoClipCodexScriptable>();
            clip_codex_m829a2.name = "clip_m829a2";
            clip_codex_m829a2.CompatibleWeaponSystems = new WeaponSystemCodexScriptable[1];
            clip_codex_m829a2.ClipType = clip_m829a2;

            // m829a3
            ammo_m829a3 = new AmmoType();
            Util.ShallowCopy(ammo_m829a3, Assets.ammo_m833);
            ammo_m829a3.Name = "M829A3 APFSDS-T";
            ammo_m829a3.Caliber = 120;
            ammo_m829a3.RhaPenetration = 690f;
            ammo_m829a3.SpallMultiplier = 1.2f;
            ammo_m829a3.MaxSpallRha = 22f;
            ammo_m829a3.MinSpallRha = 7f;
            ammo_m829a3.MuzzleVelocity = 1555f;
            ammo_m829a3.Mass = 5.2f;
            ammo_m829a3.SectionalArea = 0.00086f;
            ammo_m829a3.Coeff = 0.22f;
            ammo_m829a3.CachedIndex = -1;

            string era_schema = Assembly.CreateQualifiedName("PactIncreasedLethality", "PactIncreasedLethality.EraSchema");
            string k5 = Assembly.CreateQualifiedName("PactIncreasedLethality", "PactIncreasedLethality.Kontakt5");
            string relikt = Assembly.CreateQualifiedName("PactIncreasedLethality", "PactIncreasedLethality.Relikt");
            Type era_schema_type = Type.GetType(era_schema);
            Type k5_type = Type.GetType(k5);
            Type relikt_type = Type.GetType(relikt);
            if (k5_type != null)
            {
                BindingFlags flags = BindingFlags.Static | BindingFlags.Public;
                FieldInfo era_schema_so = era_schema_type.GetField("era_so", BindingFlags.Public | BindingFlags.Instance);
                Func<Type, ArmorCodexScriptable> get_codex = type => (ArmorCodexScriptable)era_schema_so.GetValue(type.GetField("schema", flags).GetValue(null));

                ammo_m829a3.ArmorOptimizations = new AmmoType.ArmorOptimization[] {
                    new AmmoType.ArmorOptimization() {
                        Armor = get_codex(k5_type),
                        RhaRatio = 0.25f
                    },
                    
                    new AmmoType.ArmorOptimization() {
                        Armor = get_codex(relikt_type),
                        RhaRatio = 0.25f
                    }                   
                };
            }
                    
            ammo_codex_m829a3 = ScriptableObject.CreateInstance<AmmoCodexScriptable>();
            ammo_codex_m829a3.AmmoType = ammo_m829a3;
            ammo_codex_m829a3.name = "ammo_m829a3";

            clip_m829a3 = new AmmoType.AmmoClip();
            clip_m829a3.Capacity = 1;
            clip_m829a3.Name = "M829A3 APFSDS-T";
            clip_m829a3.MinimalPattern = new AmmoCodexScriptable[1];
            clip_m829a3.MinimalPattern[0] = ammo_codex_m829a3;

            clip_codex_m829a3 = ScriptableObject.CreateInstance<AmmoClipCodexScriptable>();
            clip_codex_m829a3.name = "clip_m829a3";
            clip_codex_m829a3.CompatibleWeaponSystems = new WeaponSystemCodexScriptable[1];
            clip_codex_m829a3.ClipType = clip_m829a3;

            // m830
            ammo_m830 = new AmmoType();
            Util.ShallowCopy(ammo_m830, Assets.ammo_m456);
            ammo_m830.Name = "M830 HEAT-MP-T";
            ammo_m830.Caliber = 120;
            ammo_m830.RhaPenetration = 550;
            ammo_m830.TntEquivalentKg = 1.814f;
            ammo_m830.MuzzleVelocity = 1140f;
            ammo_m830.Mass = 13.5f;
            ammo_m830.CertainRicochetAngle = 8.0f;
            ammo_m830.ShatterOnRicochet = false;
            ammo_m830.DetonateSpallCount = 40;
            ammo_m830.SectionalArea = 0.0095f;
            ammo_m830.CachedIndex = -1;

            ammo_codex_m830 = ScriptableObject.CreateInstance<AmmoCodexScriptable>();
            ammo_codex_m830.AmmoType = ammo_m830;
            ammo_codex_m830.name = "ammo_m830";

            clip_m830 = new AmmoType.AmmoClip();
            clip_m830.Capacity = 1;
            clip_m830.Name = "M830 HEAT-MP-T";
            clip_m830.MinimalPattern = new AmmoCodexScriptable[1];
            clip_m830.MinimalPattern[0] = ammo_codex_m830;

            clip_codex_m830 = ScriptableObject.CreateInstance<AmmoClipCodexScriptable>();
            clip_codex_m830.name = "clip_m830";
            clip_codex_m830.CompatibleWeaponSystems = new WeaponSystemCodexScriptable[1];
            clip_codex_m830.ClipType = clip_m830;

            // m830a1
            ammo_m830a1 = new AmmoType();
            Util.ShallowCopy(ammo_m830a1, Assets.ammo_m456);
            ammo_m830a1.Name = "M830A1 MPAT-T";
            ammo_m830a1.Coeff = ammo_m829.Coeff;
            ammo_m830a1.Caliber = 120;
            ammo_m830a1.RhaPenetration = 500;
            ammo_m830a1.TntEquivalentKg = 1.814f;
            ammo_m830a1.MuzzleVelocity = 1400f;
            ammo_m830a1.Mass = 11.4f;
            ammo_m830a1.CertainRicochetAngle = 5.0f;
            ammo_m830a1.ShatterOnRicochet = false;
            ammo_m830a1.AlwaysProduceBlast = true;
            ammo_m830a1.DetonateSpallCount = 60;
            ammo_m830a1.MaxSpallRha = 35f;
            ammo_m830a1.SectionalArea = 0.0055f;
            ammo_m830a1.CachedIndex = -1;

            ammo_codex_m830a1 = ScriptableObject.CreateInstance<AmmoCodexScriptable>();
            ammo_codex_m830a1.AmmoType = ammo_m830a1;
            ammo_codex_m830a1.name = "ammo_m830a1";

            clip_m830a1 = new AmmoType.AmmoClip();
            clip_m830a1.Capacity = 1;
            clip_m830a1.Name = "M830A1 MPAT-T";
            clip_m830a1.MinimalPattern = new AmmoCodexScriptable[1];
            clip_m830a1.MinimalPattern[0] = ammo_codex_m830a1;

            clip_codex_m830a1 = ScriptableObject.CreateInstance<AmmoClipCodexScriptable>();
            clip_codex_m830a1.name = "clip_m830a1";
            clip_codex_m830a1.CompatibleWeaponSystems = new WeaponSystemCodexScriptable[1];
            clip_codex_m830a1.ClipType = clip_m830a1;

            m830a1_forward_frag.Name = "mpat forward frag";
            m830a1_forward_frag.RhaPenetration = 250f;
            m830a1_forward_frag.MuzzleVelocity = 600f;
            m830a1_forward_frag.Category = AmmoType.AmmoCategory.Penetrator;
            m830a1_forward_frag.Mass = 0.80f;
            m830a1_forward_frag.SectionalArea = 0.0055f;
            m830a1_forward_frag.Coeff = 0.5f;
            m830a1_forward_frag.UseTracer = false;
            m830a1_forward_frag.CertainRicochetAngle = 10f;
            m830a1_forward_frag.SpallMultiplier = 0.2f;
            m830a1_forward_frag.Caliber = 5f;
            m830a1_forward_frag.DetonateEffect = Resources.FindObjectsOfTypeAll<GameObject>().Where(o => o.name == "Sabot Impact").First();
            m830a1_forward_frag.ImpactEffectDescriptor = new ParticleEffectsManager.ImpactEffectDescriptor()
            {
                HasImpactEffect = true,
                ImpactCategory = ParticleEffectsManager.Category.Kinetic,
                EffectSize = ParticleEffectsManager.EffectSize.Bullet,
                RicochetType = ParticleEffectsManager.RicochetType.None,
                Flags = ParticleEffectsManager.ImpactModifierFlags.Small,
                MinFilterStrictness = ParticleEffectsManager.FilterStrictness.Low
            };

            MPAT.AddMPATFuse(ammo_m830a1);

            ammo_m827_vis = GameObject.Instantiate(Assets.ammo_m833.VisualModel);
            ammo_m827_vis.name = "M829 visual";
            ammo_m827.VisualModel = ammo_m827_vis;
            ammo_m827.VisualModel.GetComponent<AmmoStoredVisual>().AmmoType = ammo_m827;
            ammo_m827.VisualModel.GetComponent<AmmoStoredVisual>().AmmoScriptable = ammo_codex_m827;

            ammo_m829_vis = GameObject.Instantiate(Assets.ammo_m833.VisualModel);
            ammo_m829_vis.name = "M829 visual";
            ammo_m829.VisualModel = ammo_m829_vis;
            ammo_m829.VisualModel.GetComponent<AmmoStoredVisual>().AmmoType = ammo_m829;
            ammo_m829.VisualModel.GetComponent<AmmoStoredVisual>().AmmoScriptable = ammo_codex_m829;

            ammo_m829a1_vis = GameObject.Instantiate(Assets.ammo_m833.VisualModel);
            ammo_m829a1_vis.name = "M829A1 visual";
            ammo_m829a1.VisualModel = ammo_m829a1_vis;
            ammo_m829a1.VisualModel.GetComponent<AmmoStoredVisual>().AmmoType = ammo_m829a1;
            ammo_m829a1.VisualModel.GetComponent<AmmoStoredVisual>().AmmoScriptable = ammo_codex_m829a1;

            ammo_m829a2_vis = GameObject.Instantiate(Assets.ammo_m833.VisualModel);
            ammo_m829a2_vis.name = "M829A2 visual";
            ammo_m829a2.VisualModel = ammo_m829a2_vis;
            ammo_m829a2.VisualModel.GetComponent<AmmoStoredVisual>().AmmoType = ammo_m829a2;
            ammo_m829a2.VisualModel.GetComponent<AmmoStoredVisual>().AmmoScriptable = ammo_codex_m829a2;

            ammo_m829a3_vis = GameObject.Instantiate(Assets.ammo_m833.VisualModel);
            ammo_m829a3_vis.name = "M829A3 visual";
            ammo_m829a3.VisualModel = ammo_m829a3_vis;
            ammo_m829a3.VisualModel.GetComponent<AmmoStoredVisual>().AmmoType = ammo_m829a3;
            ammo_m829a3.VisualModel.GetComponent<AmmoStoredVisual>().AmmoScriptable = ammo_codex_m829a3;

            ammo_m830_vis = GameObject.Instantiate(Assets.ammo_m456.VisualModel);
            ammo_m830_vis.name = "M830 visual";
            ammo_m830.VisualModel = ammo_m830_vis;
            ammo_m830.VisualModel.GetComponent<AmmoStoredVisual>().AmmoType = ammo_m830;
            ammo_m830.VisualModel.GetComponent<AmmoStoredVisual>().AmmoScriptable = ammo_codex_m830;

            ammo_m830a1_vis = GameObject.Instantiate(Assets.ammo_m456.VisualModel);
            ammo_m830a1_vis.name = "M830A1 visual";
            ammo_m830a1.VisualModel = ammo_m830a1_vis;
            ammo_m830a1.VisualModel.GetComponent<AmmoStoredVisual>().AmmoType = ammo_m830a1;
            ammo_m830a1.VisualModel.GetComponent<AmmoStoredVisual>().AmmoScriptable = ammo_codex_m830a1;

            // canister         
            ammo_m1028 = new AmmoType();
            Util.ShallowCopy(ammo_m1028, ammo_m827);
            ammo_m1028.CachedIndex = -1;
            ammo_m1028.Name = "M1028 APERS";
            ammo_m1028.Category = AmmoType.AmmoCategory.Penetrator;
            ammo_m1028.Caliber = 120;
            ammo_m1028.RhaPenetration = 0;
            ammo_m1028.TntEquivalentKg = 0f;
            ammo_m1028.MuzzleVelocity = 800f;
            ammo_m1028.Mass = 0.80f;
            ammo_m1028.DetonateSpallCount = 0;
            ammo_m1028.AlwaysProduceBlast = false;
            ammo_m1028.NoPenSpall = true;
            ammo_m1028.SpallMultiplier = 0f;
            ammo_m1028.SectionalArea = 0.0008f;
            ammo_m1028.EdgeSetback = 0.001f;
            ammo_m1028.Coeff = 0.22f;
            ammo_m1028.RangedFuseTime = 0.005f;
            ammo_m1028.DetonateEffect = null;
            ammo_m1028.TerrainImpactEffect = null;
            ammo_m1028.ImpactEffectDescriptor = new ParticleEffectsManager.ImpactEffectDescriptor()
            {
                HasImpactEffect = false
            };
            ammo_m1028.ImpactAudio = GHPC.Audio.ImpactAudioType.Generic;

            ammo_codex_m1028 = ScriptableObject.CreateInstance<AmmoCodexScriptable>();
            ammo_codex_m1028.AmmoType = ammo_m1028;
            ammo_codex_m1028.name = "ammo_m1028";

            clip_m1028 = new AmmoType.AmmoClip();
            clip_m1028.Capacity = 1;
            clip_m1028.Name = "M1028 APERS";
            clip_m1028.MinimalPattern = new AmmoCodexScriptable[1];
            clip_m1028.MinimalPattern[0] = ammo_codex_m1028;

            clip_codex_m1028 = ScriptableObject.CreateInstance<AmmoClipCodexScriptable>();
            clip_codex_m1028.name = "clip_m1028";
            clip_codex_m1028.CompatibleWeaponSystems = new WeaponSystemCodexScriptable[1];
            clip_codex_m1028.ClipType = clip_m1028;

            ammo_m1028_vis = GameObject.Instantiate(Assets.ammo_m833.VisualModel);
            ammo_m1028_vis.name = "M1028 visual";
            ammo_m1028.VisualModel = ammo_m1028_vis;
            ammo_m1028.VisualModel.GetComponent<AmmoStoredVisual>().AmmoType = ammo_m1028;
            ammo_m1028.VisualModel.GetComponent<AmmoStoredVisual>().AmmoScriptable = ammo_codex_m1028;

            canister_ball = new AmmoType();
            Util.ShallowCopy(canister_ball, Resources.FindObjectsOfTypeAll<AmmoCodexScriptable>().Where(o => o.name == "ammo_762NATO").First().AmmoType);
            canister_ball.Name = "Canister Ball";
            canister_ball.CachedIndex = -1;
            canister_ball.RhaPenetration = 120f;
            canister_ball.MuzzleVelocity = 800f;
            // 800 gram pellet doesn't really make sense but any lower and it will drop like a rock
            canister_ball.Mass = 0.80f;
            canister_ball.SectionalArea = 0.0008f;
            canister_ball.Coeff = 0.22f;
            canister_ball.UseTracer = false;
            canister_ball.CertainRicochetAngle = 10f;
            canister_ball.SpallMultiplier = 0f;
            canister_ball.Caliber = 1f;
            canister_ball.NoPenSpall = true;
            canister_ball.ImpactEffectDescriptor = new ParticleEffectsManager.ImpactEffectDescriptor()
            {
                HasImpactEffect = false,
            };

            canister_ball.VisualType = LiveRoundMarshaller.LiveRoundVisualType.Invisible;
                
            ap = new Dictionary<string, AmmoClipCodexScriptable>()
            {
                ["M827"] = clip_codex_m827,
                ["M829"] = clip_codex_m829,
                ["M829A1"] = clip_codex_m829a1,
                ["M829A2"] = clip_codex_m829a2,
                ["M829A3"] = clip_codex_m829a3,
            };

            heat = new Dictionary<string, AmmoClipCodexScriptable>()
            {
                ["M830"] = clip_codex_m830,
                ["M830A1"] = clip_codex_m830a1,
            };
        }
    }
}
