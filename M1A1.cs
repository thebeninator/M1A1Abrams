using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GHPC.Camera;
using GHPC.Player;
using MelonLoader;
using UnityEngine;
using GHPC.State;
using System.Collections;
using GHPC.Weapons;
using GHPC.Equipment.Optics;
using GHPC.Vehicle;
using System.Reflection;
using Reticle;
using FMODUnity;
using FMOD.Studio;
using System.Reflection.Emit;
using HarmonyLib;
using GHPC.Audio;
using FMOD;
using static MelonLoader.MelonLogger;
using MelonLoader.Utils;
using GHPC;
using NWH.VehiclePhysics;
using static Reticle.ReticleTree;
using GHPC.Utility;
using UnityEngine.Rendering.PostProcessing;
using System.IO;

namespace M1A1Abrams
{
    public static class M1A1
    {
        static MelonPreferences_Entry<int> m829Count;
        static MelonPreferences_Entry<int> m830Count;
        static MelonPreferences_Entry<bool> rotateAzimuth;
        static MelonPreferences_Entry<bool> m1e1;
        static MelonPreferences_Entry<int> randomChanceNum;
        static MelonPreferences_Entry<bool> randomChance;
        static MelonPreferences_Entry<bool> useSuperSabot;
        static MelonPreferences_Entry<bool> useSuperHeat;
        static MelonPreferences_Entry<bool> citv;
        static MelonPreferences_Entry<bool> alt_flir_colour;
        public static MelonPreferences_Entry<bool> perfect_citv;
        public static MelonPreferences_Entry<bool> citv_reticle;
        public static MelonPreferences_Entry<bool> citv_smooth;
        public static MelonPreferences_Entry<bool> perfect_override;

        static WeaponSystemCodexScriptable gun_m256;

        static AmmoClipCodexScriptable clip_codex_m829;
        static AmmoType.AmmoClip clip_m829;
        static AmmoCodexScriptable ammo_codex_m829;
        static AmmoType ammo_m829;

        static AmmoClipCodexScriptable clip_codex_m829a1;
        static AmmoType.AmmoClip clip_m829a1;
        static AmmoCodexScriptable ammo_codex_m829a1;
        static AmmoType ammo_m829a1;

        static AmmoClipCodexScriptable clip_codex_m830;
        static AmmoType.AmmoClip clip_m830;
        static AmmoCodexScriptable ammo_codex_m830;
        static AmmoType ammo_m830;

        static AmmoClipCodexScriptable clip_codex_m830a1;
        public static AmmoType.AmmoClip clip_m830a1;
        static AmmoCodexScriptable ammo_codex_m830a1;
        public static AmmoType ammo_m830a1;
        public static AmmoType m830a1_forward_frag = new AmmoType();

        static AmmoClipCodexScriptable clip_codex_m1028;
        static AmmoType.AmmoClip clip_m1028;
        static AmmoCodexScriptable ammo_codex_m1028;
        static AmmoType ammo_m1028;

        static AmmoType ammo_m833;
        static AmmoType ammo_m456;

        static GameObject ammo_m829_vis = null;
        static GameObject ammo_m829a1_vis = null;
        static GameObject ammo_m830_vis = null;
        static GameObject ammo_m830a1_vis = null;
        static GameObject ammo_m1028_vis = null;

        // gas
        static ReticleSO reticleSO_heat;
        static ReticleMesh.CachedReticle reticle_cached_heat;

        static ReticleSO reticleSO_ap;
        static ReticleMesh.CachedReticle reticle_cached_ap;

        public static AmmoType canister_ball = AmmoType.CopyOf(GHPC.Weapons.LiveRound.SpallAmmoType);

        public static void Config(MelonPreferences_Category cfg)
        {
            m829Count = cfg.CreateEntry<int>("M829", 22);
            m829Count.Description = "How many rounds of M829 (APFSDS) or M830 (HEAT) each M1A1 should carry. Maximum of 40 rounds total. Bring in at least one M829 round.";
            m830Count = cfg.CreateEntry<int>("M830", 18);

            useSuperSabot = cfg.CreateEntry<bool>("Use M829A1", false);
            useSuperSabot.Description = "In case 600mm of RHA penetration is not working out for you...";

            useSuperHeat = cfg.CreateEntry<bool>("Use M830A1", false);
            useSuperHeat.Description = "MPAT; sabot-like ballistics and toggleable (middle mouse) proximity fuse.";

            rotateAzimuth = cfg.CreateEntry<bool>("Rotate Azimuth", false);
            rotateAzimuth.Description = "Horizontal stabilization of M1A1 sights when applying lead.";

            citv = cfg.CreateEntry<bool>("CITV", false);
            citv.Description = "Replaces commander's NVGs with variable-zoom thermals.";

            perfect_citv = cfg.CreateEntry<bool>("No Blur CITV", false);
            citv_reticle = cfg.CreateEntry<bool>("CITV Reticle", true);

            perfect_override = cfg.CreateEntry<bool>("Perfect CITV Override", false);
            perfect_override.Comment = "Basically lets you point-n-shoot with the CITV.";

            citv_smooth = cfg.CreateEntry<bool>("Smooth CITV Panning", true);
            citv_smooth.Comment = "Makes CITV feel more like a camera.";

            alt_flir_colour = cfg.CreateEntry<bool>("Alternate GPS FLIR Colour", false);
            alt_flir_colour.Description = "[Requires CITV to be enabled] Gives the gunner's sight FLIR the same colour palette as the CITV.";

            m1e1 = cfg.CreateEntry<bool>("M1E1", false);
            m1e1.Description = "Convert M1s to M1E1s (i.e: they get the 120mm gun).";

            randomChance = cfg.CreateEntry<bool>("Random", false);
            randomChance.Description = "M1IPs/M1s will have a random chance of being converted to M1A1s/M1E1s.";
            randomChanceNum = cfg.CreateEntry<int>("ConversionChance", 50);
        }
        public static IEnumerator Convert(GameState _)
        {
            foreach (GameObject vic_go in M1A1AbramsMod.vic_gos)
            {
                Vehicle vic = vic_go.GetComponent<Vehicle>();

                if (vic == null) continue;
                if (vic_go.GetComponent<Util.AlreadyConverted>() != null) continue;
                if (vic.FriendlyName != "M1IP" && !(m1e1.Value && vic.FriendlyName == "M1")) continue;

                //if (vic_go.GetComponent<LockOnLead>() == null)
                //    vic_go.AddComponent<LockOnLead>();

                vic_go.AddComponent<Util.AlreadyConverted>();

                int rand = (randomChance.Value) ? UnityEngine.Random.Range(1, 100) : 0;

                if (rand <= randomChanceNum.Value)
                {
                    vic._friendlyName = (vic.FriendlyName == "M1IP") ? "M1A1" : "M1E1";

                    vic_go.AddComponent<MPAT_Switch>();

                    WeaponsManager weaponsManager = vic.GetComponent<WeaponsManager>();
                    WeaponSystemInfo mainGunInfo = weaponsManager.Weapons[0];
                    WeaponSystem mainGun = mainGunInfo.Weapon;
                    UsableOptic optic = Util.GetDayOptic(mainGun.FCS);

                    if (rotateAzimuth.Value)
                    {
                        optic.RotateAzimuth = true;
                        optic.slot.LinkedNightSight.PairedOptic.RotateAzimuth = true;
                        optic.slot.VibrationShakeMultiplier = 0f;
                        optic.slot.VibrationPreBlur = false;
                    }

                    if (citv.Value)
                    {
                        vic.DesignatedCameraSlots[0].LinkedNightSight.gameObject.AddComponent<CITV>();

                        if (alt_flir_colour.Value)
                            optic.slot.LinkedNightSight.PairedOptic.post.profile.settings[2] = vic.DesignatedCameraSlots[0].LinkedNightSight.gameObject.
                                GetComponent<SimpleNightVision>()._postVolume.profile.settings[1];
                        ChromaticAberration s = optic.post.profile.AddSettings<ChromaticAberration>();
                        s.active = true; 
                        s.intensity.overrideState = true;
                        s.intensity.value = 0.35f;

                        vic._friendlyName += "+";
                    }
                    
                    if (reticleSO_ap == null)
                    {
                        reticleSO_ap = ScriptableObject.Instantiate(ReticleMesh.cachedReticles["M1_105_GAS_APFSDS"].tree);
                        reticleSO_ap.name = "120mm_gas_ap";

                        Util.ShallowCopy(reticle_cached_ap, ReticleMesh.cachedReticles["M1_105_GAS_APFSDS"]);
                        reticle_cached_ap.tree = reticleSO_ap;

                        ReticleTree.Angular boresight = ((reticleSO_ap.planes[0]
                            as ReticleTree.FocalPlane).elements[0]
                            as ReticleTree.Angular);

                        ReticleTree.VerticalBallistic reticle_range_ap = boresight.elements[4] as ReticleTree.VerticalBallistic;
                        reticle_range_ap.projectile = (useSuperSabot.Value) ? ammo_codex_m829a1 : ammo_codex_m829;
                        reticle_range_ap.UpdateBC();

                        ReticleTree.Text reticle_text_ap = boresight.elements[0]
                            as ReticleTree.Text;

                        string ap_name = (useSuperSabot.Value) ? "M829A1" : "M829";
                        reticle_text_ap.text = "120-MM\nAPFSDS-T " + ap_name + "\nMETERS";

                        reticleSO_heat = ScriptableObject.Instantiate(ReticleMesh.cachedReticles["M1_105_GAS_HEAT"].tree);
                        reticleSO_heat.name = "120mm_gas_heat";

                        Util.ShallowCopy(reticle_cached_heat, ReticleMesh.cachedReticles["M1_105_GAS_HEAT"]);
                        reticle_cached_heat.tree = reticleSO_heat;

                        ReticleTree.Angular boresight_heat = ((reticleSO_heat.planes[0]
                            as ReticleTree.FocalPlane).elements[0]
                            as ReticleTree.Angular);

                        ReticleTree.VerticalBallistic reticle_range_heat = boresight_heat.elements[4]
                            as ReticleTree.VerticalBallistic;
                        reticle_range_heat.projectile = (useSuperHeat.Value) ? ammo_codex_m830a1 : ammo_codex_m830;
                        reticle_range_heat.UpdateBC();

                        ReticleTree.Text reticle_text_heat = boresight_heat.elements[0]
                            as ReticleTree.Text;

                        string heat_name = (useSuperHeat.Value) ? "MPAT-T" : "HEAT-MP-T";
                        reticle_text_heat.text = "120-MM \n "+ heat_name + "\nMETERS";
                    }

                    ReticleMesh gas_ap = vic.transform.Find("IPM1_rig/HULL/TURRET/GUN/Gun Scripts/Aux sight (GAS)/Reticle Mesh").gameObject.GetComponent<ReticleMesh>();
                    gas_ap.reticleSO = reticleSO_ap;
                    gas_ap.reticle = reticle_cached_ap;
                    gas_ap.SMR = null;
                    gas_ap.Load();

                    ReticleMesh gas_heat = vic.transform.Find("IPM1_rig/HULL/TURRET/GUN/Gun Scripts/Aux sight (GAS)/Reticle Mesh HEAT").gameObject.GetComponent<ReticleMesh>();
                    gas_heat.reticleSO = reticleSO_heat;
                    gas_heat.reticle = reticle_cached_heat;
                    gas_heat.SMR = null;
                    gas_heat.Load();
                    

                    Transform muzzleFlashes = mainGun.MuzzleEffects[1].transform;
                    muzzleFlashes.GetChild(1).transform.localScale = new Vector3(1.3f, 1.3f, 1f);
                    muzzleFlashes.GetChild(2).transform.localScale = new Vector3(1.3f, 1.3f, 1f);
                    muzzleFlashes.GetChild(4).transform.localScale = new Vector3(1.3f, 1.3f, 1f);

                    mainGunInfo.Name = "120mm gun M256";
                    mainGun.Impulse = 68000;
                    mainGun.CodexEntry = gun_m256;

                    GameObject gunTube = vic_go.transform.Find("IPM1_rig/HULL/TURRET/GUN/gun_recoil").gameObject;
                    gunTube.transform.localScale = new Vector3(1.4f, 1.4f, 0.98f);

                    // convert ammo
                    AmmoClipCodexScriptable sabotClipCodex = (useSuperSabot.Value) ? clip_codex_m829a1 : clip_codex_m829;
                    AmmoClipCodexScriptable heatClipCodex = (useSuperHeat.Value) ? clip_codex_m830a1 : clip_codex_m830;
                    LoadoutManager loadoutManager = vic.GetComponent<LoadoutManager>();
                    loadoutManager.TotalAmmoCounts = new int[] { m829Count.Value, m830Count.Value };
                    loadoutManager.LoadedAmmoTypes = new AmmoClipCodexScriptable[] { sabotClipCodex, heatClipCodex };
                    loadoutManager._totalAmmoCount = 40;

                    for (int i = 0; i <= 2; i++)
                    {
                        GHPC.Weapons.AmmoRack rack = loadoutManager.RackLoadouts[i].Rack;
                        rack.ClipCapacity = i == 2 ? 4 : 18;
                        rack.ClipTypes = new AmmoType.AmmoClip[] { sabotClipCodex.ClipType, heatClipCodex.ClipType };
                        Util.EmptyRack(rack);
                    }

                    loadoutManager.SpawnCurrentLoadout();
                    mainGun.Feed.AmmoTypeInBreech = null;
                    mainGun.Feed.Start();
                    loadoutManager.RegisterAllBallistics();
                }
            }

            yield break;
        }

        public static void LateUpdate()
        {
            if (M1A1AbramsMod.gameManager == null) return;

            CameraSlot cam = M1A1AbramsMod.camManager._currentCamSlot;

            if (cam == null) return;
            if (cam.name != "Aux sight (GAS)") return;
            if (M1A1AbramsMod.playerManager.CurrentPlayerWeapon.Name != "120mm gun M256") return;

            AmmoType currentAmmo = M1A1AbramsMod.playerManager.CurrentPlayerWeapon.FCS.CurrentAmmoType;
            int reticleId = currentAmmo.Name.Contains("M829") ? 0 : 2;

            GameObject reticle = cam.transform.GetChild(reticleId).gameObject;

            if (!reticle.activeSelf)
            {
                reticle.SetActive(true);
            }
        }

        public static void Init()
        {
            if (gun_m256 == null)
            {
                foreach (AmmoCodexScriptable s in Resources.FindObjectsOfTypeAll(typeof(AmmoCodexScriptable)))
                {
                    if (s.AmmoType.Name == "M833 APFSDS-T")
                    {
                        ammo_m833 = s.AmmoType;
                    }

                    if (s.AmmoType.Name == "M456 HEAT-FS-T")
                    {
                        ammo_m456 = s.AmmoType;
                    }

                    if (ammo_m456 != null && ammo_m833 != null) break;
                }

                // m256
                gun_m256 = ScriptableObject.CreateInstance<WeaponSystemCodexScriptable>();
                gun_m256.name = "gun_m256";
                gun_m256.CaliberMm = 120;
                gun_m256.FriendlyName = "120mm Gun M256";
                gun_m256.Type = WeaponSystemCodexScriptable.WeaponType.LargeCannon;

                // m829 
                ammo_m829 = new AmmoType();
                Util.ShallowCopy(ammo_m829, ammo_m833);
                ammo_m829.Name = "M829 APFSDS-T";
                ammo_m829.Caliber = 120;
                ammo_m829.RhaPenetration = 570f;
                ammo_m829.MuzzleVelocity = 1670f;
                ammo_m829.Mass = 3.9f;
                ammo_m829.SectionalArea = 0.0009f;

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
                clip_codex_m829.CompatibleWeaponSystems[0] = gun_m256;
                clip_codex_m829.ClipType = clip_m829;

                // m829a1
                ammo_m829a1 = new AmmoType();
                Util.ShallowCopy(ammo_m829a1, ammo_m833);
                ammo_m829a1.Name = "M829A1 APFSDS-T";
                ammo_m829a1.Caliber = 120;
                ammo_m829a1.RhaPenetration = 650;
                ammo_m829a1.SpallMultiplier = 1.2f;
                ammo_m829a1.MuzzleVelocity = 1575;
                ammo_m829a1.Mass = 4.6f;
                ammo_m829a1.SectionalArea = 0.00082f;

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
                clip_codex_m829a1.CompatibleWeaponSystems[0] = gun_m256;
                clip_codex_m829a1.ClipType = clip_m829a1;

                // m830
                ammo_m830 = new AmmoType();
                Util.ShallowCopy(ammo_m830, ammo_m456);
                ammo_m830.Name = "M830 HEAT-MP-T";
                ammo_m830.Caliber = 120;
                ammo_m830.RhaPenetration = 480;
                ammo_m830.TntEquivalentKg = 1.814f;
                ammo_m830.MuzzleVelocity = 1140f;
                ammo_m830.Mass = 13.5f;
                ammo_m830.CertainRicochetAngle = 8.0f;
                ammo_m830.ShatterOnRicochet = false;
                ammo_m830.DetonateSpallCount = 25;
                ammo_m830.SectionalArea = 0.0095f;

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
                clip_codex_m830.CompatibleWeaponSystems[0] = gun_m256;
                clip_codex_m830.ClipType = clip_m830;

                // m830a1
                ammo_m830a1 = new AmmoType();
                Util.ShallowCopy(ammo_m830a1, ammo_m456);
                ammo_m830a1.Name = "M830A1 MPAT-T";
                ammo_m830a1.Coeff = ammo_m829.Coeff;
                ammo_m830a1.Caliber = 120;
                ammo_m830a1.RhaPenetration = 450;
                ammo_m830a1.TntEquivalentKg = 1.814f;
                ammo_m830a1.MuzzleVelocity = 1400f;
                ammo_m830a1.Mass = 11.4f;
                ammo_m830a1.CertainRicochetAngle = 5.0f;
                ammo_m830a1.ShatterOnRicochet = false;
                ammo_m830a1.AlwaysProduceBlast = true;
                ammo_m830a1.DetonateSpallCount = 60;
                ammo_m830a1.MaxSpallRha = 35f;
                ammo_m830a1.SectionalArea = 0.0055f;

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
                clip_codex_m830a1.CompatibleWeaponSystems[0] = gun_m256;
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
                m830a1_forward_frag.ImpactTypeUnfuzed = GHPC.Effects.ParticleEffectsManager.EffectVisualType.BulletImpact;
                m830a1_forward_frag.ImpactTypeUnfuzedTerrain = GHPC.Effects.ParticleEffectsManager.EffectVisualType.BulletImpactTerrain;

                MPAT.AddMPATFuse(ammo_m830a1);

                // canister
                ammo_m1028 = new AmmoType();
                Util.ShallowCopy(ammo_m1028, ammo_m456);
                ammo_m1028.Name = "M1028 Canister";
                ammo_m1028.Category = AmmoType.AmmoCategory.ShapedCharge;
                ammo_m1028.Caliber = 120;
                ammo_m1028.RhaPenetration = 0;
                ammo_m1028.TntEquivalentKg = 0f;
                ammo_m1028.MuzzleVelocity = 1640f;
                ammo_m1028.Mass = 0.80f;
                ammo_m1028.DetonateSpallCount = 0;
                ammo_m1028.AlwaysProduceBlast = false;
                ammo_m1028.NoPenSpall = true;
                ammo_m1028.SpallMultiplier = 0f;
                ammo_m1028.ImpactTypeFuzed = GHPC.Effects.ParticleEffectsManager.EffectVisualType.None;
                ammo_m1028.ImpactTypeUnfuzed = GHPC.Effects.ParticleEffectsManager.EffectVisualType.None;
                ammo_m1028.ImpactTypeFuzedTerrain = GHPC.Effects.ParticleEffectsManager.EffectVisualType.None;
                ammo_m1028.ImpactTypeUnfuzedTerrain = GHPC.Effects.ParticleEffectsManager.EffectVisualType.None;
                ammo_m1028.SectionalArea = 0.0008f;
                ammo_m1028.EdgeSetback = 0.001f;
                ammo_m1028.Coeff = 0.22f;
                ammo_m1028.RangedFuseTime = 0.0001f;

                ammo_codex_m1028 = ScriptableObject.CreateInstance<AmmoCodexScriptable>();
                ammo_codex_m1028.AmmoType = ammo_m1028;
                ammo_codex_m1028.name = "ammo_m1028";

                clip_m1028 = new AmmoType.AmmoClip();
                clip_m1028.Capacity = 1;
                clip_m1028.Name = "M1028 Canister";
                clip_m1028.MinimalPattern = new AmmoCodexScriptable[1];
                clip_m1028.MinimalPattern[0] = ammo_codex_m1028;

                clip_codex_m1028 = ScriptableObject.CreateInstance<AmmoClipCodexScriptable>();
                clip_codex_m1028.name = "clip_m1028";
                clip_codex_m1028.CompatibleWeaponSystems = new WeaponSystemCodexScriptable[1];
                clip_codex_m1028.CompatibleWeaponSystems[0] = gun_m256;
                clip_codex_m1028.ClipType = clip_m1028;

                canister_ball.Name = "Canister Ball";
                canister_ball.RhaPenetration = 120f;
                canister_ball.MuzzleVelocity = 1640f;
                // 800 gram pellet doesn't really make sense but any lower and it will drop like a rock
                canister_ball.Mass = 0.80f;
                canister_ball.SectionalArea = 0.0008f;
                canister_ball.EdgeSetback = 0.001f;
                canister_ball.Coeff = 0.22f;
                canister_ball.UseTracer = true;
                canister_ball.CertainRicochetAngle = 10f;
                canister_ball.SpallMultiplier = 0f;
                canister_ball.Caliber = 1f;
                canister_ball.NoPenSpall = true;
                canister_ball.ImpactTypeUnfuzed = GHPC.Effects.ParticleEffectsManager.EffectVisualType.BulletImpact;
                canister_ball.ImpactTypeUnfuzedTerrain = GHPC.Effects.ParticleEffectsManager.EffectVisualType.BulletImpactTerrain;

                ammo_m829_vis = GameObject.Instantiate(ammo_m833.VisualModel);
                ammo_m829_vis.name = "M829 visual";
                ammo_m829.VisualModel = ammo_m829_vis;
                ammo_m829.VisualModel.GetComponent<AmmoStoredVisual>().AmmoType = ammo_m829;
                ammo_m829.VisualModel.GetComponent<AmmoStoredVisual>().AmmoScriptable = ammo_codex_m829;

                ammo_m829a1_vis = GameObject.Instantiate(ammo_m833.VisualModel);
                ammo_m829a1_vis.name = "M829A1 visual";
                ammo_m829a1.VisualModel = ammo_m829a1_vis;
                ammo_m829a1.VisualModel.GetComponent<AmmoStoredVisual>().AmmoType = ammo_m829a1;
                ammo_m829a1.VisualModel.GetComponent<AmmoStoredVisual>().AmmoScriptable = ammo_codex_m829a1;

                ammo_m830_vis = GameObject.Instantiate(ammo_m456.VisualModel);
                ammo_m830_vis.name = "M830 visual";
                ammo_m830.VisualModel = ammo_m830_vis;
                ammo_m830.VisualModel.GetComponent<AmmoStoredVisual>().AmmoType = ammo_m830;
                ammo_m830.VisualModel.GetComponent<AmmoStoredVisual>().AmmoScriptable = ammo_codex_m830;

                ammo_m830a1_vis = GameObject.Instantiate(ammo_m456.VisualModel);
                ammo_m830a1_vis.name = "M830A1 visual";
                ammo_m830a1.VisualModel = ammo_m830a1_vis;
                ammo_m830a1.VisualModel.GetComponent<AmmoStoredVisual>().AmmoType = ammo_m830a1;
                ammo_m830a1.VisualModel.GetComponent<AmmoStoredVisual>().AmmoScriptable = ammo_codex_m830a1;

                ammo_m1028_vis = GameObject.Instantiate(ammo_m833.VisualModel);
                ammo_m1028_vis.name = "M1028 visual";
                ammo_m1028.VisualModel = ammo_m1028_vis;
                ammo_m1028.VisualModel.GetComponent<AmmoStoredVisual>().AmmoType = ammo_m1028;
                ammo_m1028.VisualModel.GetComponent<AmmoStoredVisual>().AmmoScriptable = ammo_codex_m1028;
            }

            StateController.RunOrDefer(GameState.GameReady, new GameStateEventHandler(Convert), GameStatePriority.Medium);
        }
    }

    [HarmonyPatch(typeof(GHPC.Weapons.LiveRound), "createExplosion")]
    public class Canister
    {
        public static void Prefix(GHPC.Weapons.LiveRound __instance, ref int spallCount)
        {
            if (__instance.Info.Name == "M1028 Canister")
            {
                spallCount = 0;

                for (int i = 0; i < 30; i++)
                {
                    GHPC.Weapons.LiveRound component;
                    component = LiveRoundMarshaller.Instance.GetRoundOfVisualType(LiveRoundMarshaller.LiveRoundVisualType.Shell)
                        .GetComponent<GHPC.Weapons.LiveRound>();

                    component.Info = M1A1.canister_ball;
                    component.CurrentSpeed = 1640f;
                    component.MaxSpeed = 1640f;
                    component.IsSpall = false;
                    component.Shooter = __instance.Shooter;
                    component.transform.position = __instance.transform.position;
                    component.transform.forward = Quaternion.Euler(
                        UnityEngine.Random.Range(-0.03f, 0.03f),
                        UnityEngine.Random.Range(-0.10f, 0.10f),
                        UnityEngine.Random.Range(-0.06f, 0.06f)) * __instance.transform.forward;
                    component.Init(__instance, null);
                    component.name = "blyat" + i;
                }
            }
        }
    }
}
