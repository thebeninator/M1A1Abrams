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
using static Reticle.ReticleTree.GroupBase;
using UnityEngine.Rendering.PostProcessing;
using GHPC.Effects;
using static UnityEngine.GraphicsBuffer;
using GHPC.AI.Sensors;

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
        static MelonPreferences_Entry<bool> citv_m1a1;
        static MelonPreferences_Entry<bool> citv_m1e1;
        static MelonPreferences_Entry<bool> alt_flir_colour;

        static MelonPreferences_Entry<bool> du_package;
        static MelonPreferences_Entry<int> du_gen;

        public static MelonPreferences_Entry<bool> perfect_citv;
        public static MelonPreferences_Entry<bool> citv_reticle;
        public static MelonPreferences_Entry<bool> citv_smooth;
        public static MelonPreferences_Entry<bool> perfect_override;

        public static MelonPreferences_Entry<bool> digital_enchancement_m1a1;
        public static MelonPreferences_Entry<bool> digital_enchancement_m1e1;

        public static MelonPreferences_Entry<bool> de_fixed_reticle_size;

        public static MelonPreferences_Entry<bool> crows_m1e1;
        public static MelonPreferences_Entry<bool> crows_m1a1;
        public static MelonPreferences_Entry<bool> crows_raufoss;
        public static MelonPreferences_Entry<bool> crows_alt_placement;

        public static MelonPreferences_Entry<bool> better_flir_m1e1;
        public static MelonPreferences_Entry<bool> better_flir_m1a1;

        static WeaponSystemCodexScriptable gun_m256;
        static WeaponSystemCodexScriptable gun_m256a1;

        static GameObject citv_obj;
        static GameObject m256_obj;
        static GameObject addon_turret;
        static GameObject addon_hull;
        static GameObject addon_turret_l55;

        public class AuxFix : MonoBehaviour
        {
            GameObject heat;
            GameObject apfsds;
            public WeaponSystem main_gun;

            void Awake()
            {
                heat = transform.Find("Reticle Mesh HEAT").gameObject;
                apfsds = transform.Find("Reticle Mesh").gameObject;
            }

            void Update()
            {
                heat.SetActive(main_gun.CurrentAmmoType.Name.Contains("M830"));
                apfsds.SetActive(main_gun.CurrentAmmoType.Name.Contains("M82"));
            }
        }

        public static void Config(MelonPreferences_Category cfg)
        {
            m829Count = cfg.CreateEntry<int>("M829", 22);
            m829Count.Description = "How many rounds of M829 (APFSDS), M830 (HEAT) each M1A1 should carry. Maximum of 40 rounds total. Bring in at least one M829 round.";
            m830Count = cfg.CreateEntry<int>("M830", 18);

            sabot_m1 = cfg.CreateEntry<string>("AP Round (M1E1)", "M827");
            sabot_m1.Description = "Customize which rounds M1A1s/M1E1s use";
            sabot_m1.Comment = "M827, M829, M829A1, M829A2";
            sabot_m1ip = cfg.CreateEntry<string>("AP Round (M1A1)", "M829");

            heat_m1 = cfg.CreateEntry<string>("HEAT Round (M1E1)", "M830");
            heat_m1.Comment = "M830, M830A1 (has proximity fuse that can be toggled using middle mouse)";
            heat_m1ip = cfg.CreateEntry<string>("HEAT Round (M1A1)", "M830");

            rotateAzimuth = cfg.CreateEntry<bool>("Rotate Azimuth", false);
            rotateAzimuth.Description = "Horizontal stabilization of M1A1 sights when applying lead.";

            digital_enchancement_m1e1 = cfg.CreateEntry<bool>("Digital Enhancement (M1E1)", false);
            digital_enchancement_m1e1.Description = "Additional zoom levels for thermals.";
            digital_enchancement_m1a1 = cfg.CreateEntry<bool>("Digital Enhancement (M1A1)", false);
            de_fixed_reticle_size = cfg.CreateEntry<bool>("Fixed Reticle Size", false);
            de_fixed_reticle_size.Comment = "Digitally enhanced zoom levels will not increase the size of the reticle.";

            citv_m1e1 = cfg.CreateEntry<bool>("CITV (M1E1)", false);
            citv_m1e1.Description = "Replaces commander's NVGs with variable-zoom thermals.";

            citv_m1a1 = cfg.CreateEntry<bool>("CITV (M1A1)", false);

            perfect_citv = cfg.CreateEntry<bool>("No Blur CITV", false);
            citv_reticle = cfg.CreateEntry<bool>("CITV Reticle", true);

            perfect_override = cfg.CreateEntry<bool>("Perfect CITV Override", false);
            perfect_override.Comment = "Basically lets you point-n-shoot with the CITV.";

            citv_smooth = cfg.CreateEntry<bool>("Smooth CITV Panning", true);
            citv_smooth.Comment = "Makes CITV feel more like a camera.";

            alt_flir_colour = cfg.CreateEntry<bool>("Alternate GPS FLIR Colour", false);
            alt_flir_colour.Description = "[Requires CITV to be enabled] Gives the gunner's sight FLIR the same colour palette as the CITV.";

            du_package = cfg.CreateEntry<bool>("DU Armour", false);
            du_package.Description = "DU inserts for the composite turret cheeks: increased KE protection. M1A1 exclusive. Increased weight.";
            du_gen = cfg.CreateEntry<int>("DU Generation", 1);
            du_gen.Comment = "Higher generation = more KE protection (1-3, integer)";

            crows_m1e1 = cfg.CreateEntry<bool>("CROWS (M1E1)", false);
            crows_m1e1.Description = "Remote weapons system equipped with a .50 caliber M2HB; 400 rounds, automatic lead, thermals.";
            crows_m1a1 = cfg.CreateEntry<bool>("CROWS (M1A1)", false);

            crows_alt_placement = cfg.CreateEntry<bool>("Alternative Position", false);
            crows_alt_placement.Comment = "Moves the CROWS to the right side of the commander instead of directly in front.";

            crows_raufoss = cfg.CreateEntry<bool>("Use Mk 211 Mod 0", false);
            crows_raufoss.Comment = "Loads the CROWS M2HB with high explosive rounds.";

            better_flir_m1e1 = cfg.CreateEntry<bool>("2nd Generation Thermals (M1E1)", false);
            better_flir_m1e1.Description = "Less blurry thermals for the gunner.";
            better_flir_m1a1 = cfg.CreateEntry<bool>("2nd Generation Thermals (M1A1)", false);

            m1e1 = cfg.CreateEntry<bool>("M1E1", false);
            m1e1.Description = "Convert M1s to M1E1s (i.e: they get the 120mm gun).";

            randomChance = cfg.CreateEntry<bool>("Random", false);
            randomChance.Description = "M1IPs/M1s will have a random chance of being converted to M1A1s/M1E1s.";
            randomChanceNum = cfg.CreateEntry<int>("ConversionChance", 50);
        }
        public static IEnumerator Convert(GameState _)
        {
            GAS.Create(Ammo_120mm.ap[sabot_m1ip.Value].ClipType.MinimalPattern[0], Ammo_120mm.heat[heat_m1ip.Value].ClipType.MinimalPattern[0]);

            foreach (Vehicle vic in M1A1AbramsMod.vics)
            {
                if (vic == null) continue;

                GameObject vic_go = vic.gameObject;

                if (vic_go.GetComponent<Util.AlreadyConverted>() != null) continue;
                if (vic.FriendlyName != "M1IP" && !(m1e1.Value && vic.FriendlyName == "M1")) continue;

                vic_go.AddComponent<Util.AlreadyConverted>();

                /*
                if (vic.FriendlyName == "M12IP") {
                    vic._friendlyName = "M1IPv2";
                    GameObject new_addon_cheeks = GameObject.Instantiate(addon_turret_l55);

                    new_addon_cheeks.transform.parent = vic.transform.Find("IPM1_rig/M1IP_skinned");
                    new_addon_cheeks.transform.localPosition = new Vector3(0f, 0f, 0f);
                    new_addon_cheeks.transform.localEulerAngles = new Vector3(0f, 270f, 0f);

                    new_addon_cheeks.transform.parent = vic.transform.Find("IPM1_rig/HULL/TURRET");
                    new_addon_cheeks.transform.localEulerAngles = new Vector3(0f, 270f, 0f);

                    new_addon_cheeks.transform.Find("mantlet follow").parent = vic.transform.Find("IPM1_rig/HULL/TURRET/GUN");

                    LateFollow turret_follow = new_addon_cheeks.AddComponent<LateFollow>();
                    turret_follow.FollowTarget = vic_go.transform.Find("IPM1_rig/HULL/TURRET");
                    turret_follow.ForceToRoot = true;
                    turret_follow.enabled = true;
                    turret_follow.Awake();
                    new_addon_cheeks.transform.parent = null;

                    LateFollow mantlet_follow = vic.transform.Find("IPM1_rig/HULL/TURRET/GUN/mantlet follow").gameObject.AddComponent<LateFollow>();
                    mantlet_follow.FollowTarget = vic_go.transform.Find("IPM1_rig/HULL/TURRET/GUN");
                    mantlet_follow.ForceToRoot = true;
                    mantlet_follow.enabled = true;
                    mantlet_follow.Awake();
                    vic.transform.Find("IPM1_rig/HULL/TURRET/GUN/mantlet follow").parent = null;

                    GameObject gun_tube = vic_go.transform.Find("IPM1_rig/HULL/TURRET/GUN/gun_recoil").gameObject;
                    //LateFollow _tube_follower = gun_tube.GetComponent<LateFollowTarget>()._lateFollowers[0];
                    //_tube_follower.transform.Find("Gun Breech.001").GetComponent<MeshRenderer>().enabled = false;
                    new_addon_cheeks.transform.Find("rh120 visual").parent = gun_tube.transform;

                    GameObject new_addon_hull = GameObject.Instantiate(addon_hull);
                    new_addon_hull.transform.parent = vic.transform.Find("IPM1_rig/M1IP_hull");

                    new_addon_hull.transform.localPosition = new Vector3(0f, 0f, 0f);
                    new_addon_hull.transform.localEulerAngles = new Vector3(0f, 0f, 0f);

                    LateFollow hull_follow = new_addon_hull.AddComponent<LateFollow>();
                    hull_follow.FollowTarget = vic_go.transform;
                    hull_follow.ForceToRoot = true;
                    hull_follow.enabled = true;
                    hull_follow.Awake();
                    new_addon_hull.transform.parent = null;

                    vic_go.transform.Find("IPM1_rig/HULL/TURRET/GUN/Gun Scripts/turret_gun").gameObject.SetActive(false);
                    vic_go.transform.Find("IPM1_rig/HULL/TURRET/Turret Scripts/M1_camonet").gameObject.SetActive(false);

                    GameObject new_gun_origin = new GameObject();
                    new_gun_origin.transform.parent = vic_go.transform.Find("IPM1_rig/HULL/TURRET/GUN");
                    new_gun_origin.transform.localPosition = new Vector3(-0.0439f, -0.0281f, 3.8632f);
                    new_gun_origin.transform.localEulerAngles = Vector3.zero;
                    new_gun_origin.transform.localScale = Vector3.zero;
                    SkinnedMeshRenderer m1ip_skin = vic_go.transform.Find("IPM1_rig/M1IP_skinned").GetComponent<SkinnedMeshRenderer>();
                    Transform[] new_bones = m1ip_skin.bones;
                    new_bones[56] = new_gun_origin.transform;
                    m1ip_skin.bones = new_bones;

                    vic_go.transform.Find("IPM1_rig/HULL/TURRET/GUN/Gun Scripts/105mm Gun M68").transform.localPosition = new Vector3(0.0174f, 1.8976f, 7.0344f);

                    WeaponsManager weaponsManager2 = vic.GetComponent<WeaponsManager>();
                    WeaponSystemInfo mainGunInfo2 = weaponsManager2.Weapons[0];
                    WeaponSystem mainGun2 = mainGunInfo2.Weapon;

                    Transform muzzleFlashes2 = mainGun2.MuzzleEffects[1].transform;
                    muzzleFlashes2.GetChild(1).transform.localScale = new Vector3(1.3f, 1.3f, 1f);
                    muzzleFlashes2.GetChild(2).transform.localScale = new Vector3(1.3f, 1.3f, 1f);
                    muzzleFlashes2.GetChild(4).transform.localScale = new Vector3(1.3f, 1.3f, 1f);

                    mainGunInfo2.Name = "120mm gun Rh-120 L/55";
                    mainGun2.Impulse = 68000;
                    mainGun2.CodexEntry = gun_m256a1;

                    mainGun2.WeaponSound.SingleShotEventPaths[0] = "event:/Weapons/canon_125mm-2A46";

                    string ap_idx2 = sabot_m1ip.Value;
                    AmmoClipCodexScriptable sabotClipCodex2 = ap_idx2 == "M829" ? Ammo_120mm.clip_codex_m829_l55 : Ammo_120mm.clip_codex_m829a1_l55;

                    LoadoutManager loadoutManager2 = vic.GetComponent<LoadoutManager>();
                    loadoutManager2.TotalAmmoCounts = new int[] { m829Count.Value, m830Count.Value };
                    loadoutManager2.LoadedAmmoTypes = new AmmoClipCodexScriptable[] { sabotClipCodex2, Ammo_120mm.clip_codex_m830_l55 };
                    loadoutManager2._totalAmmoCount = 40;

                    for (int i = 0; i <= 2; i++)
                    {
                        GHPC.Weapons.AmmoRack rack = loadoutManager2.RackLoadouts[i].Rack;
                        rack.ClipCapacity = i == 2 ? 4 : 18;
                        rack.ClipTypes = new AmmoType.AmmoClip[] { sabotClipCodex2.ClipType, Ammo_120mm.clip_codex_m830_l55.ClipType };
                        Util.EmptyRack(rack);
                    }

                    loadoutManager2.SpawnCurrentLoadout();
                    mainGun2.Feed.AmmoTypeInBreech = null;
                    mainGun2.Feed.Start();
                    loadoutManager2.RegisterAllBallistics();

                    UsableOptic optic2 = vic.transform.Find("IPM1_rig/HULL/TURRET/Turret Scripts/GPS/Optic").GetComponent<UsableOptic>();

                    DigitalEnhancement digital_enhance2 = mainGun2.FCS.gameObject.AddComponent<DigitalEnhancement>();
                    digital_enhance2.original_blur = 0.08f;
                    digital_enhance2.slot = optic2.slot.LinkedNightSight;
                    digital_enhance2.reticle_plane = optic2.slot.LinkedNightSight.transform.Find("Reticle Mesh/FFP");
                    digital_enhance2.Add(2.4f, 0.07f, 0.69f);
                    digital_enhance2.Add(1.5f, 0.10f, 0.43f);

                    Grain grain2;
                    optic2.slot.LinkedNightSight.BaseBlur = 0.08f;
                    optic2.slot.LinkedNightSight.PairedOptic.post.profile.TryGetSettings(out grain2);
                    grain2.intensity.value = 0.07f;
                    vic.transform.Find("IPM1_rig/HULL/TURRET/Turret Scripts/GPS/FLIR/Canvas Scanlines").gameObject.SetActive(false);
                    optic2.slot.LinkedNightSight.VibrationShakeMultiplier = 0.0f;

                    optic2.RotateAzimuth = true;
                    optic2.slot.LinkedNightSight.PairedOptic.RotateAzimuth = true;
                    optic2.slot.VibrationShakeMultiplier = 0f;
                    optic2.slot.VibrationPreBlur = false;
                    optic2.Alignment = OpticAlignment.BoresightStabilized;
                    optic2.slot.LinkedNightSight.PairedOptic.Alignment = OpticAlignment.BoresightStabilized;

                    Transform gas2 = vic.transform.Find("IPM1_rig/HULL/TURRET/GUN/Gun Scripts/Aux sight (GAS)");
                    gas2.GetComponent<CameraSlot>().ExclusiveWeapons = optic2.slot.ExclusiveWeapons;
                    AuxFix aux_fix2 = gas2.gameObject.AddComponent<AuxFix>();
                    aux_fix2.main_gun = mainGun2;
                    ReticleMesh gas_ap2 = gas2.Find("Reticle Mesh").gameObject.GetComponent<ReticleMesh>();
                    gas_ap2.reticleSO = GAS.reticleSO_ap;
                    gas_ap2.reticle = GAS.reticle_cached_ap;
                    gas_ap2.SMR = null;
                    gas_ap2.Load();

                    ReticleMesh gas_heat2 = gas2.Find("Reticle Mesh HEAT").gameObject.GetComponent<ReticleMesh>();
                    gas_heat2.reticleSO = GAS.reticleSO_heat;
                    gas_heat2.reticle = GAS.reticle_cached_heat;
                    gas_heat2.SMR = null;
                    gas_heat2.Load();

                    continue;
                }
                */

                int rand = (randomChance.Value) ? UnityEngine.Random.Range(1, 100) : 0;
                if (rand > randomChanceNum.Value) continue;

                WeaponsManager weaponsManager = vic.GetComponent<WeaponsManager>();
                WeaponSystemInfo mainGunInfo = weaponsManager.Weapons[0];
                WeaponSystem mainGun = mainGunInfo.Weapon;
                UsableOptic optic = vic.transform.Find("IPM1_rig/HULL/TURRET/Turret Scripts/GPS/Optic").GetComponent<UsableOptic>();
                optic.slot.ExclusiveWeapons = new WeaponSystem[] { weaponsManager.Weapons[0].Weapon, weaponsManager.Weapons[1].Weapon };
                optic.slot.LinkedNightSight.ExclusiveWeapons = new WeaponSystem[] { weaponsManager.Weapons[0].Weapon, weaponsManager.Weapons[1].Weapon };

                vic._friendlyName = (vic.FriendlyName == "M1IP") ? "M1A1" : "M1E1";

                vic_go.AddComponent<MPAT_Switch>();
                vic_go.GetComponent<Rigidbody>().mass = du_package.Value && vic.FriendlyName == "M1A1" ? 62781.3776f : 57152.6386f;

                mainGun.WeaponSound.SingleShotEventPaths[0] = "event:/Weapons/canon_125mm-2A46";

                Vector3 crows_pos = crows_alt_placement.Value ? new Vector3(1.4f, 1.1164f, -0.5873f) : new Vector3(0.7855f, 1.2855f, 0.5182f);
                if ((crows_m1e1.Value && vic._uniqueName == "M1") || (crows_m1a1.Value && vic._uniqueName == "M1IP"))
                {
                    vic.transform.Find("IPM1_rig/HULL/TURRET/CUPOLA/CUPOLA_GUN").localScale = Vector3.zero;
                    CROWS.Add(vic, vic.transform.Find("IPM1_rig/HULL/TURRET"), crows_pos);

                    if (!crows_alt_placement.Value)
                        vic.DesignatedCameraSlots[0].transform.localPosition = new Vector3(-0.1538f, 0.627f, -0.05f);
                }

                if ((better_flir_m1e1.Value && vic._uniqueName == "M1") || (better_flir_m1a1.Value && vic._uniqueName == "M1IP"))
                {
                    Grain grain;
                    optic.slot.LinkedNightSight.BaseBlur = 0f;
                    optic.slot.LinkedNightSight.PairedOptic.post.profile.TryGetSettings(out grain);
                    grain.intensity.value = 0.07f;
                    vic.transform.Find("IPM1_rig/HULL/TURRET/Turret Scripts/GPS/FLIR/Canvas Scanlines").gameObject.SetActive(false);
                    optic.slot.LinkedNightSight.VibrationShakeMultiplier = 0.0f;
                }

                if ((digital_enchancement_m1e1.Value && vic._uniqueName == "M1") || (digital_enchancement_m1a1.Value && vic._uniqueName == "M1IP"))
                {
                    DigitalEnhancement digital_enhance = mainGun.FCS.gameObject.AddComponent<DigitalEnhancement>();
                    digital_enhance.original_blur = optic.slot.LinkedNightSight.BaseBlur;
                    digital_enhance.slot = optic.slot.LinkedNightSight;
                    digital_enhance.reticle_plane = optic.slot.LinkedNightSight.transform.Find("Reticle Mesh/FFP");
                    digital_enhance.Add(2.4f, 0.02f, 0.69f);
                    digital_enhance.Add(1f, 0.10f, 0.29f);
                }

                if (du_package.Value && vic._friendlyName == "M1A1")
                {
                    vic._friendlyName += du_gen.Value > 1 ? "HC" : "HA";

                    GameObject turret_cheeks = vic.transform.Find("IPM1_rig/HULL/TURRET").GetComponent<LateFollowTarget>()
                        ._lateFollowers[0].transform.Find("Turret_Armor/cheeks composite arrays").gameObject;

                    VariableArmor var_armour = turret_cheeks.GetComponent<VariableArmor>();
                    var_armour._armorType = DUArmour.du_armor_codexes[du_gen.Value - 1];

                    AarVisual cheek_visual = turret_cheeks.GetComponent<AarVisual>();

                    cheek_visual.AarMaterial = DUArmour.du_aar_mats[du_gen.Value - 1];
                }

                if (rotateAzimuth.Value)
                {
                    optic.RotateAzimuth = true;
                    optic.slot.LinkedNightSight.PairedOptic.RotateAzimuth = true;
                    optic.slot.VibrationShakeMultiplier = 0f;
                    optic.slot.VibrationPreBlur = false;
                    optic.Alignment = OpticAlignment.BoresightStabilized;
                    optic.slot.LinkedNightSight.PairedOptic.Alignment = OpticAlignment.BoresightStabilized;
                }

                if ((citv_m1e1.Value && vic._uniqueName == "M1") || (citv_m1a1.Value && vic._uniqueName == "M1IP"))
                {
                    GameObject c = GameObject.Instantiate(citv_obj, vic.transform.Find("IPM1_rig/HULL/TURRET"));
                    c.transform.localPosition = new Vector3(-0.6794f, 0.9341f, 0.4348f);
                    c.transform.localEulerAngles = new Vector3(0f, 0f, 0f);

                    CITV citv_component = vic.DesignatedCameraSlots[0].LinkedNightSight.gameObject.AddComponent<CITV>();
                    citv_component.model = c;

                    c.transform.Find("assembly").GetComponent<UniformArmor>().Unit = vic;
                    c.transform.Find("glass").GetComponent<UniformArmor>().Unit = vic;

                    if (alt_flir_colour.Value)
                        optic.slot.LinkedNightSight.PairedOptic.post.profile.settings[2] = vic.DesignatedCameraSlots[0].LinkedNightSight.gameObject.
                            GetComponent<SimpleNightVision>()._postVolume.profile.settings[1];
                    //ChromaticAberration s = optic.post.profile.AddSettings<ChromaticAberration>();
                    //s.active = true; 
                    //s.intensity.overrideState = true;
                    //s.intensity.value = 0.35f;

                    vic._targetSpotterSettings._periscopeFOV = 120f;
                    vic.TargetSpotterSettings._sightDistance = 4500f;
                    vic.TargetSpotterSettings._nightSightDistanceIdeal = 4500f;
                    vic.TargetSpotterSettings._nightSightDistancePassive = 4500f;
                    vic.transform.Find("Unit AI").GetComponent<TankTargetSensor>()._spotTimeMax = 2f;

                    vic._friendlyName += "+";
                }

                if ((vic.FriendlyName == "M1A1HA+" || vic.FriendlyName == "M1A1HC+") && rotateAzimuth.Value)
                    vic._friendlyName = "M1A2";

                if (vic.FriendlyName == "M1A2" && better_flir_m1a1.Value && crows_m1a1.Value)
                    vic._friendlyName += " SEP";

                string ap_idx = vic.UniqueName == "M1IP" ? sabot_m1ip.Value : sabot_m1.Value;
                string heat_idx = vic.UniqueName == "M1IP" ? heat_m1ip.Value : heat_m1.Value;
                AmmoClipCodexScriptable sabotClipCodex = Ammo_120mm.ap[ap_idx];
                AmmoClipCodexScriptable heatClipCodex = Ammo_120mm.heat[heat_idx];

                Transform gas = vic.transform.Find("IPM1_rig/HULL/TURRET/GUN/Gun Scripts/Aux sight (GAS)");
                gas.GetComponent<CameraSlot>().ExclusiveWeapons = optic.slot.ExclusiveWeapons;
                AuxFix aux_fix = gas.gameObject.AddComponent<AuxFix>();
                aux_fix.main_gun = mainGun;

                ReticleMesh gas_ap = gas.Find("Reticle Mesh").gameObject.GetComponent<ReticleMesh>();
                gas_ap.reticleSO = GAS.reticleSO_ap;
                gas_ap.reticle = GAS.reticle_cached_ap;
                gas_ap.SMR = null;
                gas_ap.Load();

                ReticleMesh gas_heat = gas.Find("Reticle Mesh HEAT").gameObject.GetComponent<ReticleMesh>();
                gas_heat.reticleSO = GAS.reticleSO_heat;
                gas_heat.reticle = GAS.reticle_cached_heat;
                gas_heat.SMR = null;
                gas_heat.Load();

                Transform muzzleFlashes = mainGun.MuzzleEffects[1].transform;
                muzzleFlashes.GetChild(1).transform.localScale = new Vector3(1.3f, 1.3f, 1f);
                muzzleFlashes.GetChild(2).transform.localScale = new Vector3(1.3f, 1.3f, 1f);
                muzzleFlashes.GetChild(4).transform.localScale = new Vector3(1.3f, 1.3f, 1f);

                mainGunInfo.Name = "120mm gun M256";
                mainGun.Impulse = 68000;
                mainGun.CodexEntry = gun_m256;

                GameObject dummy_tube = new GameObject("dummy tube");
                dummy_tube.transform.parent = vic_go.transform.Find("IPM1_rig/HULL/TURRET/GUN");
                dummy_tube.transform.localScale = new Vector3(0f, 0f, 0f);

                Transform smr_path = (vic.UniqueName == "M1") ? vic.transform.Find("M1_rig/M1_skinned") : vic.transform.Find("IPM1_rig/M1IP_skinned");
                SkinnedMeshRenderer smr = smr_path.GetComponent<SkinnedMeshRenderer>();
                Transform[] bones = smr.bones;
                bones[46] = dummy_tube.transform;
                smr.bones = bones;

                GameObject gunTube = vic_go.transform.Find("IPM1_rig/HULL/TURRET/GUN/gun_recoil").gameObject;
                gunTube.transform.Find("GUN/Gun Breech.001").GetComponent<MeshRenderer>().enabled = false;
                GameObject _m256_obj = GameObject.Instantiate(m256_obj, gunTube.transform);
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

                vic_go.transform.Find("IPM1_rig/HULL/TURRET/GUN/Gun Scripts/turret_gun").gameObject.SetActive(false);
            }

            yield break;
        }

        public static void Init()
        {
            if (citv_obj == null)
            {
                var bundle = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory + "/m1a1assets/", "citv"));
                citv_obj = bundle.LoadAsset<GameObject>("citv.prefab");
                citv_obj.hideFlags = HideFlags.DontUnloadUnusedAsset;
                citv_obj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

                GameObject assem = citv_obj.transform.Find("assembly").gameObject;
                GameObject glass = citv_obj.transform.Find("glass").gameObject;

                assem.tag = "Penetrable";
                glass.tag = "Penetrable";
                //assem.layer = 7;
                //glass.layer = 7;

                assem.GetComponent<MeshRenderer>().material.shader = Shader.Find("Standard (FLIR)");
                glass.GetComponent<MeshRenderer>().material.shader = Shader.Find("Standard (FLIR)");
                assem.AddComponent<HeatSource>();
                glass.AddComponent<HeatSource>();

                UniformArmor assem_armour = assem.AddComponent<UniformArmor>();
                UniformArmor glass_armour = glass.AddComponent<UniformArmor>();
                assem_armour.PrimarySabotRha = 40f;
                assem_armour.PrimaryHeatRha = 40f;

                glass_armour.PrimarySabotRha = 5f;
                glass_armour.PrimaryHeatRha = 5f;

                assem_armour._name = "CITV";
                glass_armour._name = "CITV glass";

                var bundle2 = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory + "/m1a1assets/", "m256"));
                m256_obj = bundle2.LoadAsset<GameObject>("m256.prefab");
                m256_obj.hideFlags = HideFlags.DontUnloadUnusedAsset;
                m256_obj.transform.localScale = new Vector3(0.75f, 0.75f, 0.8f);
                m256_obj.GetComponent<MeshRenderer>().material.shader = Shader.Find("Standard (FLIR)");
                m256_obj.AddComponent<HeatSource>();

                ArmorCodexScriptable spaced_so = ScriptableObject.CreateInstance<ArmorCodexScriptable>();
                spaced_so.name = "composite screen";

                ArmorType composite_screen = new ArmorType();
                composite_screen.Name = "composite screen armour";
                composite_screen.CanRicochet = true;
                composite_screen.CanShatterLongRods = true;
                composite_screen.NormalizesHits = true;
                composite_screen.ThicknessSource = ArmorType.RhaSource.Multipliers;
                composite_screen.SpallAngleMultiplier = 1;
                composite_screen.SpallPowerMultiplier = 0.70f;
                composite_screen.RhaeMultiplierCe = 1.35f;
                composite_screen.RhaeMultiplierKe = 1.2f;
                spaced_so.ArmorType = composite_screen;

                ArmorCodexScriptable steel_so = ScriptableObject.CreateInstance<ArmorCodexScriptable>();
                steel_so.name = "speshal steel";

                ArmorType steel = new ArmorType();
                steel.Name = "special steel";
                steel.CanRicochet = true;
                steel.CanShatterLongRods = true;
                steel.NormalizesHits = true;
                steel.ThicknessSource = ArmorType.RhaSource.Multipliers;
                steel.SpallAngleMultiplier = 1;
                steel.SpallPowerMultiplier = 0.70f;
                steel.RhaeMultiplierCe = 1.05f;
                steel.RhaeMultiplierKe = 1.05f;
                steel_so.ArmorType = steel;

                /*
                var bundle3 = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory + "/m1a1assets/", "m1ipv2"));
                addon_turret = bundle3.LoadAsset<GameObject>("m1ipv2turret.prefab");
                addon_hull = bundle3.LoadAsset<GameObject>("m1ipv2hull.prefab");
                addon_turret_l55 = bundle3.LoadAsset<GameObject>("m1a1ipturret.prefab");
                */

                /*
                foreach (GameObject obj in new GameObject[] { addon_turret, addon_turret_l55 })
                {
                    obj.transform.Find("mantlet follow/mantlet").tag = "Penetrable";
                    obj.transform.Find("mantlet follow/mantlet").gameObject.layer = 8;
                    VariableArmor mantlet_armour = obj.transform.Find("mantlet follow/mantlet").gameObject.AddComponent<VariableArmor>();
                    mantlet_armour._name = "addon mantlet composite wedge";
                    mantlet_armour._armorType = spaced_so;
                    mantlet_armour._spallForwardRatio = 0.2f;

                    obj.transform.Find("side brackets").tag = "Penetrable";
                    obj.transform.Find("side brackets").gameObject.layer = 8;
                    VariableArmor side_armour = obj.transform.Find("side brackets").gameObject.AddComponent<VariableArmor>();
                    side_armour._name = "mounting bracket";
                    side_armour._armorType = steel_so;
                    side_armour._spallForwardRatio = 0.2f;

                    obj.transform.Find("cheeks").tag = "Penetrable";
                    obj.transform.Find("cheeks").gameObject.layer = 8;
                    VariableArmor cheeks_armour = obj.transform.Find("cheeks").gameObject.AddComponent<VariableArmor>();
                    cheeks_armour._name = "addon cheek composite wedge";
                    cheeks_armour._armorType = spaced_so;
                    cheeks_armour._spallForwardRatio = 0.2f;

                    obj.transform.Find("roof").tag = "Penetrable";
                    obj.transform.Find("roof").gameObject.layer = 8;
                    VariableArmor roof_armour = obj.transform.Find("roof").gameObject.AddComponent<VariableArmor>();
                    roof_armour._name = "addon roof composite plate";
                    roof_armour._armorType = spaced_so;
                    roof_armour._spallForwardRatio = 0.2f;

                    obj.transform.Find("doghouse").tag = "Penetrable";
                    obj.transform.Find("doghouse").gameObject.layer = 8;
                    UniformArmor doghouse_armour = obj.transform.Find("doghouse").gameObject.AddComponent<UniformArmor>();
                    doghouse_armour._name = "addon GPS doghouse armour";
                    doghouse_armour._armorType = steel_so;
                    doghouse_armour.PrimaryHeatRha = 30f;
                    doghouse_armour.PrimarySabotRha = 30f;
                }

                addon_hull.transform.Find("addon hull/upper").tag = "Penetrable";
                addon_hull.transform.Find("addon hull/upper").gameObject.layer = 8;
                VariableArmor upper_armour = addon_hull.transform.Find("addon hull/upper").gameObject.AddComponent<VariableArmor>();
                upper_armour._name = "addon upper glacis composite plate";
                upper_armour._armorType = spaced_so;
                upper_armour._spallForwardRatio = 0.2f;

                addon_hull.transform.Find("addon hull/lower").tag = "Penetrable";
                addon_hull.transform.Find("addon hull/lower").gameObject.layer = 8;
                VariableArmor lower_armour = addon_hull.transform.Find("addon hull/lower").gameObject.AddComponent<VariableArmor>();
                lower_armour._name = "addon lower glacis plate";
                lower_armour._armorType = steel_so;
                lower_armour._spallForwardRatio = 0.2f;
                */
            }
            
            if (gun_m256 == null)
            {
                // m256
                gun_m256 = ScriptableObject.CreateInstance<WeaponSystemCodexScriptable>();
                gun_m256.name = "gun_m256";
                gun_m256.CaliberMm = 120;
                gun_m256.FriendlyName = "120mm Gun M256";
                gun_m256.Type = WeaponSystemCodexScriptable.WeaponType.LargeCannon;

                gun_m256a1 = ScriptableObject.CreateInstance<WeaponSystemCodexScriptable>();
                gun_m256a1.name = "gun_m256a1";
                gun_m256a1.CaliberMm = 120;
                gun_m256a1.FriendlyName = "120mm Gun Rh-120 L/55";
                gun_m256a1.Type = WeaponSystemCodexScriptable.WeaponType.LargeCannon;
            }

            StateController.RunOrDefer(GameState.GameReady, new GameStateEventHandler(Convert), GameStatePriority.Medium);
        }
    }
}