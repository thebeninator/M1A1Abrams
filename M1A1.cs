using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GHPC.Camera;
using MelonLoader;
using UnityEngine;
using GHPC.State;
using System.Collections;
using GHPC.Weapons;
using GHPC.Equipment.Optics;
using GHPC.Vehicle;
using Reticle;
using HarmonyLib;
using MelonLoader.Utils;
using GHPC.Utility;
using System.IO;
using GHPC.Equipment;
using GHPC;
using Thermals;

namespace M1A1Abrams
{
    public static class M1A1
    {
        static MelonPreferences_Entry<string> sabot_m1;
        static MelonPreferences_Entry<string> sabot_m1ip;
        static MelonPreferences_Entry<string> heat_m1;
        static MelonPreferences_Entry<string> heat_m1ip;

        static MelonPreferences_Entry<int> m829Count;
        static MelonPreferences_Entry<int> m830Count;
        static MelonPreferences_Entry<bool> rotateAzimuth;
        static MelonPreferences_Entry<bool> m1e1;
        static MelonPreferences_Entry<int> randomChanceNum;
        static MelonPreferences_Entry<bool> randomChance;
        static MelonPreferences_Entry<bool> citv;
        static MelonPreferences_Entry<bool> alt_flir_colour;
        static MelonPreferences_Entry<bool> du_package;
        public static MelonPreferences_Entry<bool> perfect_citv;
        public static MelonPreferences_Entry<bool> citv_reticle;
        public static MelonPreferences_Entry<bool> citv_smooth;
        public static MelonPreferences_Entry<bool> perfect_override;

        static WeaponSystemCodexScriptable gun_m256;

        static AmmoClipCodexScriptable clip_codex_m827;
        static AmmoType.AmmoClip clip_m827;
        static AmmoCodexScriptable ammo_codex_m827;
        static AmmoType ammo_m827;

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

        static AmmoType ammo_m833;
        static AmmoType ammo_m456;

        static GameObject ammo_m829_vis = null;
        static GameObject ammo_m829a1_vis = null;
        static GameObject ammo_m830_vis = null;
        static GameObject ammo_m830a1_vis = null;

        static Material du_aar_mat;
        static ArmorCodexScriptable du_armor_codex; 

        // gas
        static ReticleSO reticleSO_heat;
        static ReticleMesh.CachedReticle reticle_cached_heat;

        static ReticleSO reticleSO_ap;
        static ReticleMesh.CachedReticle reticle_cached_ap;

        public static AmmoType canister_ball = AmmoType.CopyOf(GHPC.Weapons.LiveRound.SpallAmmoType);

        static GameObject citv_obj;
        static GameObject m256_obj;

        static Dictionary<string, AmmoClipCodexScriptable> ap;
        static Dictionary<string, AmmoClipCodexScriptable> heat;

        public static void Config(MelonPreferences_Category cfg)
        {
            m829Count = cfg.CreateEntry<int>("M829", 22);
            m829Count.Description = "How many rounds of M829 (APFSDS) or M830 (HEAT) each M1A1 should carry. Maximum of 40 rounds total. Bring in at least one M829 round.";
            m830Count = cfg.CreateEntry<int>("M830", 18);

            sabot_m1 = cfg.CreateEntry<string>("AP Round (M1)", "M827");
            sabot_m1.Description = "Customize which rounds M1A1s/M1E1s use";
            sabot_m1.Comment = "M827, M829, M829A1";
            sabot_m1ip = cfg.CreateEntry<string>("AP Round (M1IP)", "M829");

            heat_m1 = cfg.CreateEntry<string>("HEAT Round (M1)", "M830");
            heat_m1.Comment = "M830, M830A1 (has proximity fuse that can be toggled using middle mouse)";
            heat_m1ip = cfg.CreateEntry<string>("HEAT Round (M1IP)", "M830");

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

            du_package = cfg.CreateEntry<bool>("DU Armour", false);
            du_package.Description = "Exclusively upgrades M1A1s to M1A1HAs. Increased weight.";

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

                vic_go.AddComponent<Util.AlreadyConverted>();

                int rand = (randomChance.Value) ? UnityEngine.Random.Range(1, 100) : 0;
                if (rand > randomChanceNum.Value) continue;
                
                vic._friendlyName = (vic.FriendlyName == "M1IP") ? "M1A1" : "M1E1";

                vic_go.AddComponent<MPAT_Switch>();

                vic_go.GetComponent<Rigidbody>().mass = du_package.Value && vic.FriendlyName == "M1A1" ? 62781.3776f : 57152.6386f;

                WeaponsManager weaponsManager = vic.GetComponent<WeaponsManager>();
                WeaponSystemInfo mainGunInfo = weaponsManager.Weapons[0];
                WeaponSystem mainGun = mainGunInfo.Weapon;
                UsableOptic optic = vic.transform.Find("IPM1_rig/HULL/TURRET/Turret Scripts/GPS/Optic").GetComponent<UsableOptic>();

                if (du_package.Value && vic._friendlyName == "M1A1") {
                    vic._friendlyName += "HA";

                    GameObject turret_cheeks = vic.transform.Find("IPM1_rig/HULL/TURRET").GetComponent<LateFollowTarget>()
                        ._lateFollowers[0].transform.Find("Turret_Armor/cheeks composite arrays").gameObject;

                    VariableArmor var_armour = turret_cheeks.GetComponent<VariableArmor>();
                    var_armour._armorType = du_armor_codex;
                    var_armour.AverageRha = 330f;

                    AarVisual cheek_visual = turret_cheeks.GetComponent<AarVisual>();

                    if (du_aar_mat == null)
                    {
                        du_aar_mat = Material.Instantiate(cheek_visual.AarMaterial);
                        du_aar_mat.name = "du green";
                        du_aar_mat.shaderKeywords = new string[] { "_ALPHAPREMULTIPLY_ON" };
                        du_aar_mat.color = new Color(0.1919f, 1f, 0.1083f, 0.3922f);
                    }

                    cheek_visual.AarMaterial = du_aar_mat;
                }

                if (rotateAzimuth.Value)
                {
                    optic.RotateAzimuth = true;
                    optic.slot.LinkedNightSight.PairedOptic.RotateAzimuth = true;
                    optic.slot.VibrationShakeMultiplier = 0f;
                    optic.slot.VibrationPreBlur = false;
                }

                if (citv.Value)
                {
                    GameObject c = GameObject.Instantiate(citv_obj, vic.transform.Find("IPM1_rig/HULL/TURRET"));
                    c.transform.localPosition = new Vector3(-0.6794f, 0.9341f, 0.4348f);
                    c.transform.localEulerAngles = new Vector3(0f, 0f, 0f);

                    CITV citv_component = vic.DesignatedCameraSlots[0].LinkedNightSight.gameObject.AddComponent<CITV>();
                    citv_component.model = c;

                    c.transform.Find("assembly").GetComponent<VariableArmor>().Unit = vic;
                    c.transform.Find("glass").GetComponent<VariableArmor>().Unit = vic;

                    if (alt_flir_colour.Value)
                        optic.slot.LinkedNightSight.PairedOptic.post.profile.settings[2] = vic.DesignatedCameraSlots[0].LinkedNightSight.gameObject.
                            GetComponent<SimpleNightVision>()._postVolume.profile.settings[1];
                    //ChromaticAberration s = optic.post.profile.AddSettings<ChromaticAberration>();
                    //s.active = true; 
                    //s.intensity.overrideState = true;
                    //s.intensity.value = 0.35f;

                    vic._friendlyName += "+";
                }

                if (vic.FriendlyName == "M1A1HA+" && rotateAzimuth.Value)
                    vic._friendlyName = "M1A2";

                string ap_idx = vic.UniqueName == "M1IP" ? sabot_m1ip.Value : sabot_m1.Value;
                string heat_idx = vic.UniqueName == "M1IP" ? heat_m1ip.Value : heat_m1.Value;
                AmmoClipCodexScriptable sabotClipCodex = ap[ap_idx];
                AmmoClipCodexScriptable heatClipCodex = heat[heat_idx];

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
                    reticle_range_ap.projectile = sabotClipCodex.ClipType.MinimalPattern[0];
                    reticle_range_ap.UpdateBC();

                    ReticleTree.Text reticle_text_ap = boresight.elements[0]
                        as ReticleTree.Text;

                    string ap_name = ap_idx;
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
                    reticle_range_heat.projectile = heatClipCodex.ClipType.MinimalPattern[0];
                    reticle_range_heat.UpdateBC();

                    ReticleTree.Text reticle_text_heat = boresight_heat.elements[0]
                        as ReticleTree.Text;

                    string heat_name = heat_idx;
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
                gunTube.transform.localScale = new Vector3(0f, 0f, 0f);
                LateFollow tube_follower = gunTube.GetComponent<LateFollowTarget>()._lateFollowers[0];
                tube_follower.transform.Find("Gun Breech.001").GetComponent<MeshRenderer>().enabled = false;
                GameObject _m256_obj = GameObject.Instantiate(m256_obj, tube_follower.transform);
                _m256_obj.transform.localPosition = new Vector3(0f, 0.0064f, -1.9416f);

                // convert ammo
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
            
            yield break;
        }
        
        public static void LateUpdate()
        {
            if (M1A1AbramsMod.gameManager == null) return;

            CameraSlot cam = M1A1AbramsMod.camManager._currentCamSlot;

            if (cam == null) return;
            if (cam.name != "Aux sight (GAS)") return;

            if (M1A1AbramsMod.playerManager.CurrentPlayerUnit.WeaponsManager.Weapons[0].Name != "120mm gun M256") return;

            AmmoType currentAmmo = M1A1AbramsMod.playerManager.CurrentPlayerUnit.WeaponsManager.Weapons[0].Weapon.FCS.CurrentAmmoType;

            if (currentAmmo == null) return;    

            int reticleId = currentAmmo.Name.Contains("M829") || currentAmmo.Name.Contains("M827") ? 0 : 2;

            GameObject reticle = cam.transform.GetChild(reticleId).gameObject;

            if (!reticle.activeSelf)
            {
                reticle.SetActive(true);
            }
        }

        public static void Init()
        {
            if (citv_obj == null) {
                var bundle = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory + "/m1a1assets/", "citv"));
                citv_obj = bundle.LoadAsset<GameObject>("citv.prefab");
                citv_obj.hideFlags = HideFlags.DontUnloadUnusedAsset;
                citv_obj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

                GameObject assem = citv_obj.transform.Find("assembly").gameObject;
                GameObject glass = citv_obj.transform.Find("glass").gameObject;

                assem.tag = "Penetrable";
                glass.tag = "Penetrable";

                assem.GetComponent<MeshRenderer>().material.shader = Shader.Find("Standard (FLIR)");
                glass.GetComponent<MeshRenderer>().material.shader = Shader.Find("Standard (FLIR)");
                assem.AddComponent<HeatSource>();
                glass.AddComponent<HeatSource>();

                VariableArmor assem_armour = assem.AddComponent<VariableArmor>();
                VariableArmor glass_armour = glass.AddComponent<VariableArmor>();
                assem_armour.AverageRha = 40f;
                assem_armour._name = "CITV";
                glass_armour._name = "CITV glass";

                var bundle2 = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory + "/m1a1assets/", "m256"));
                m256_obj = bundle2.LoadAsset<GameObject>("m256.prefab");
                m256_obj.hideFlags = HideFlags.DontUnloadUnusedAsset;
                m256_obj.transform.localScale = new Vector3(0.75f, 0.75f, 0.8f);
                m256_obj.GetComponent<MeshRenderer>().material.shader = Shader.Find("Standard (FLIR)");
                m256_obj.AddComponent<HeatSource>();
            }

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

                du_armor_codex = ScriptableObject.CreateInstance<ArmorCodexScriptable>();
                du_armor_codex.name = "Abrams DU composite";

                ArmorType du = new ArmorType();
                du.Name = "special armor";
                du.CanRicochet = true;
                du.CanShatterLongRods = true;
                du.NormalizesHits = true;
                du.ThicknessSource = ArmorType.RhaSource.Multipliers;
                du.SpallAngleMultiplier = 1;
                du.SpallPowerMultiplier = 0.80f;
                du.RhaeMultiplierCe = 1.4f;
                du.RhaeMultiplierKe = 0.75f;
                du_armor_codex.ArmorType = du;

                // m256
                gun_m256 = ScriptableObject.CreateInstance<WeaponSystemCodexScriptable>();
                gun_m256.name = "gun_m256";
                gun_m256.CaliberMm = 120;
                gun_m256.FriendlyName = "120mm Gun M256";
                gun_m256.Type = WeaponSystemCodexScriptable.WeaponType.LargeCannon;

                // xm827
                ammo_m827 = new AmmoType();
                Util.ShallowCopy(ammo_m827, ammo_m833);
                ammo_m827.Name = "M827 APFSDS-T";
                ammo_m827.Caliber = 120;
                ammo_m827.RhaPenetration = 520f;
                ammo_m827.MuzzleVelocity = 1650f;
                ammo_m827.Mass = 4.64f;
                ammo_m827.SectionalArea = 0.0012f;
                ammo_m827.SpallMultiplier = 1.15f;

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
                clip_codex_m827.CompatibleWeaponSystems[0] = gun_m256;
                clip_codex_m827.ClipType = clip_m827;

                // m829 
                ammo_m829 = new AmmoType();
                Util.ShallowCopy(ammo_m829, ammo_m833);
                ammo_m829.Name = "M829 APFSDS-T";
                ammo_m829.Caliber = 120;
                ammo_m829.RhaPenetration = 550f;
                ammo_m829.MuzzleVelocity = 1670f;
                ammo_m829.Mass = 4.27f;
                ammo_m829.SectionalArea = 0.0009f;
                ammo_m829.SpallMultiplier = 1.15f;

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
                ammo_m829a1.RhaPenetration = 600;
                ammo_m829a1.SpallMultiplier = 1.15f;
                ammo_m829a1.MaxSpallRha = 22f;
                ammo_m829a1.MinSpallRha = 5f;
                ammo_m829a1.MuzzleVelocity = 1575;
                ammo_m829a1.Mass = 4.6f;
                ammo_m829a1.SectionalArea = 0.00082f;
                ammo_m829a1.Coeff /= 1.5f;

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
                ammo_m830.DetonateSpallCount = 40;
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

                ap = new Dictionary<string, AmmoClipCodexScriptable>()
                {
                    ["M827"] = clip_codex_m827,
                    ["M829"] = clip_codex_m829,
                    ["M829A1"] = clip_codex_m829a1,
                };

                heat = new Dictionary<string, AmmoClipCodexScriptable>()
                {
                    ["M830"] = clip_codex_m830,
                    ["M830A1"] = clip_codex_m830a1,
                };
            }

            StateController.RunOrDefer(GameState.GameReady, new GameStateEventHandler(Convert), GameStatePriority.Medium);
        }
    }
}
