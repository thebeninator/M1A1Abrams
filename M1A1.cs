using MelonLoader;
using UnityEngine;
using GHPC.State;
using System.Collections;
using GHPC.Weapons;
using GHPC.Equipment.Optics;
using GHPC.Vehicle;
using GHPC.Utility;
using System.IO;
using GHPC;
using GHPC.Thermals;
using GHPC.AI.Sensors;
using MelonLoader.Utils;

namespace M1A1Abrams
{
    public static class M1A1
    {
        static MelonPreferences_Entry<string> sabot_m1;
        static MelonPreferences_Entry<string> sabot_m1ip;
        static MelonPreferences_Entry<string> heat_m1;
        static MelonPreferences_Entry<string> heat_m1ip;

        static MelonPreferences_Entry<int> flir_gen_m1;
        static MelonPreferences_Entry<int> flir_gen_m1ip;

        static MelonPreferences_Entry<int> m829Count;
        static MelonPreferences_Entry<int> m830Count;

        static MelonPreferences_Entry<bool> rotate_azimuth_m1;
        static MelonPreferences_Entry<bool> rotate_azimuth_m1ip;

        static MelonPreferences_Entry<bool> m1e1;
        static MelonPreferences_Entry<int> randomChanceNum;
        static MelonPreferences_Entry<bool> randomChance;
        static MelonPreferences_Entry<bool> citv_m1a1;
        static MelonPreferences_Entry<bool> citv_m1e1;

        static MelonPreferences_Entry<bool> du_package_m1;
        static MelonPreferences_Entry<int> du_gen_m1;
        static MelonPreferences_Entry<bool> du_package_m1ip;
        static MelonPreferences_Entry<int> du_gen_m1ip;

        public static MelonPreferences_Entry<bool> citv_smooth;

        public static MelonPreferences_Entry<bool> digital_enchancement_m1a1;
        public static MelonPreferences_Entry<bool> digital_enchancement_m1e1;

        public static MelonPreferences_Entry<bool> de_fixed_reticle_size;

        public static MelonPreferences_Entry<bool> crows_m1e1;
        public static MelonPreferences_Entry<bool> crows_m1a1;
        public static MelonPreferences_Entry<bool> crows_raufoss;
        public static MelonPreferences_Entry<bool> crows_alt_placement;

        public static MelonPreferences_Entry<bool> m1_to_m1ip;

        static WeaponSystemCodexScriptable gun_m256;
        static WeaponSystemCodexScriptable gun_m256a1;

        static GameObject citv_obj;
        static GameObject m256_obj;
        static GameObject addon_turret;
        static GameObject addon_hull;
        static GameObject addon_turret_l55;

        public static Vector2Int flir_gen2_res = new Vector2Int(1024, 576);
        public static Vector2Int flir_gen3_res = new Vector2Int(1366, 768);

        public static void Config(MelonPreferences_Category cfg)
        {
            m829Count = cfg.CreateEntry<int>("M829", 22);
            m829Count.Description = "How many rounds of M829 (APFSDS), M830 (HEAT) each M1A1 should carry. Maximum of 40 rounds total. Bring in at least one M829 round.";
            m830Count = cfg.CreateEntry<int>("M830", 18);

            sabot_m1 = cfg.CreateEntry<string>("AP Round (M1E1)", "M827");
            sabot_m1.Description = "Customize which rounds M1A1s/M1E1s use";
            sabot_m1.Comment = "M827, M829, M829A1, M829A2, M829A3";
            sabot_m1ip = cfg.CreateEntry<string>("AP Round (M1A1)", "M829");

            heat_m1 = cfg.CreateEntry<string>("HEAT Round (M1E1)", "M830");
            heat_m1.Comment = "M830, M830A1 (has proximity fuse that can be toggled using middle mouse)";
            heat_m1ip = cfg.CreateEntry<string>("HEAT Round (M1A1)", "M830");

            rotate_azimuth_m1 = cfg.CreateEntry<bool>("Rotate Azimuth (M1E1)", false);
            rotate_azimuth_m1.Description = "Horizontal stabilization of sights when applying lead.";
            rotate_azimuth_m1ip = cfg.CreateEntry<bool>("Rotate Azimuth (M1A1)", false);

            flir_gen_m1 = cfg.CreateEntry<int>("FLIR Generation (M1E1)", 1);
            flir_gen_m1.Description = "Settings for the gunner's thermals";
            flir_gen_m1.Comment = "Higher generation = higher resolution (1-3, integer)";
            flir_gen_m1ip = cfg.CreateEntry<int>("FLIR Generation (M1A1)", 1);

            digital_enchancement_m1e1 = cfg.CreateEntry<bool>("Digital Enhancement (M1E1)", false);
            digital_enchancement_m1e1.Comment = "Additional zoom levels for thermals.";
            digital_enchancement_m1a1 = cfg.CreateEntry<bool>("Digital Enhancement (M1A1)", false);
            de_fixed_reticle_size = cfg.CreateEntry<bool>("Fixed Reticle Size", false);
            de_fixed_reticle_size.Comment = "Digitally enhanced zoom levels will not increase the size of the reticle.";

            citv_m1e1 = cfg.CreateEntry<bool>("CITV (M1E1)", false);
            citv_m1e1.Description = "Replaces commander's NVGs with variable-zoom thermals.";
            citv_m1a1 = cfg.CreateEntry<bool>("CITV (M1A1)", false);

            citv_smooth = cfg.CreateEntry<bool>("Smooth CITV Panning", true);
            citv_smooth.Comment = "Makes CITV feel more like a camera.";

            du_package_m1ip = cfg.CreateEntry<bool>("DU Armour (M1A1)", false);
            du_package_m1ip.Description = "DU inserts for the composite turret cheeks: increased KE protection. M1A1 exclusive. Increased weight.";
            du_gen_m1ip = cfg.CreateEntry<int>("DU Generation (M1A1)", 1);
            du_gen_m1ip.Comment = "Higher generation = more KE protection (1-3, integer)";

            du_package_m1 = cfg.CreateEntry<bool>("DU Armour (M1E1)", false);
            du_gen_m1 = cfg.CreateEntry<int>("DU Generation (M1E1)", 1);
            du_package_m1.Comment = "Doesn't apply to M1E1, only those converted to M1A1";

            crows_m1e1 = cfg.CreateEntry<bool>("CROWS (M1E1)", false);
            crows_m1e1.Description = "Remote weapons system equipped with a .50 caliber M2HB; 400 rounds, automatic lead, thermals.";
            crows_m1a1 = cfg.CreateEntry<bool>("CROWS (M1A1)", false);

            crows_alt_placement = cfg.CreateEntry<bool>("Alternative Position", false);
            crows_alt_placement.Comment = "Moves the CROWS to the right side of the commander instead of directly in front.";

            crows_raufoss = cfg.CreateEntry<bool>("Use Mk 211 Mod 0", false);
            crows_raufoss.Comment = "Loads the CROWS M2HB with high explosive rounds.";

            m1e1 = cfg.CreateEntry<bool>("M1E1", false);
            m1e1.Description = "Convert M1s to M1E1s (i.e: they get the 120mm gun).";

            m1_to_m1ip = cfg.CreateEntry<bool>("M1E1 -> M1A1", false);
            m1_to_m1ip.Description = "Convert all M1E1s to M1A1s (will still use M1E1 settings)";

            randomChance = cfg.CreateEntry<bool>("Random", false);
            randomChance.Description = "M1IPs/M1s will have a random chance of being converted to M1A1s/M1E1s.";
            randomChanceNum = cfg.CreateEntry<int>("ConversionChance", 50);
        }
        public static IEnumerator Convert(GameState _)
        {
            foreach (Vehicle vic in M1A1AbramsMod.vics)
            {
                if (vic == null) continue;

                GameObject vic_go = vic.gameObject;

                if (vic_go.GetComponent<Util.AlreadyConverted>() != null) continue;
                if (vic.FriendlyName != "M1IP" && !(m1e1.Value && vic.FriendlyName == "M1")) continue;

                int rand = (randomChance.Value) ? UnityEngine.Random.Range(1, 100) : 0;
                if (rand > randomChanceNum.Value) continue;

                WeaponsManager weaponsManager = vic.GetComponent<WeaponsManager>();
                WeaponSystemInfo mainGunInfo = weaponsManager.Weapons[0];
                WeaponSystem mainGun = mainGunInfo.Weapon;
                UsableOptic optic = vic.transform.Find("IPM1_rig/HULL/TURRET/Turret Scripts/GPS/Optic").GetComponent<UsableOptic>();
                UsableOptic night_optic = optic.slot.LinkedNightSight.PairedOptic;
                Transform gas = vic.transform.Find("IPM1_rig/HULL/TURRET/GUN/Gun Scripts/Aux sight (GAS)");
                bool is_m1ip = vic.UniqueName == "M1IP" && vic.GetComponent<PreviouslyM1>() == null;

                vic._friendlyName = (vic.FriendlyName == "M1IP") ? "M1A1" : "M1E1";

                optic.slot.ExclusiveWeapons = new WeaponSystem[] { weaponsManager.Weapons[0].Weapon, weaponsManager.Weapons[1].Weapon };
                night_optic.slot.ExclusiveWeapons = new WeaponSystem[] { weaponsManager.Weapons[0].Weapon, weaponsManager.Weapons[1].Weapon };

                vic_go.AddComponent<MPAT_Switch>();

                GAS.Create(Ammo_120mm.ap[sabot_m1ip.Value].ClipType.MinimalPattern[0], Ammo_120mm.heat[heat_m1ip.Value].ClipType.MinimalPattern[0]);
                GAS.Add(gas, optic.slot.ExclusiveWeapons);

                Vector3 crows_pos = crows_alt_placement.Value ? new Vector3(1.4f, 1.1164f, -0.5873f) : new Vector3(0.7855f, 1.2855f, 0.5182f);
                bool has_crows = is_m1ip ? crows_m1a1.Value : crows_m1e1.Value;
                if (has_crows) {
                    vic.transform.Find("IPM1_rig/HULL/TURRET/CUPOLA/CUPOLA_GUN").localScale = Vector3.zero;
                    CROWS.Add(vic, vic.transform.Find("IPM1_rig/HULL/TURRET"), crows_pos);

                    if (!crows_alt_placement.Value)
                        vic.DesignatedCameraSlots[0].transform.localPosition = new Vector3(-0.1538f, 0.627f, -0.05f);
                }

                int flir_gen = is_m1ip ? flir_gen_m1ip.Value : flir_gen_m1.Value;
                if (flir_gen > 1) {
                    Vector2Int resolution = flir_gen == 2 ? flir_gen2_res : flir_gen3_res;
                    night_optic.slot.VibrationShakeMultiplier = 0.0f;
                    night_optic.slot.FLIRWidth = resolution.x;
                    night_optic.slot.FLIRHeight = resolution.y;
                    night_optic.slot.FLIRBlitMaterialOverride = Assets.flir_blit_mat_green_no_scan;
                }

                bool has_digital_enhancement = is_m1ip ? digital_enchancement_m1e1.Value : digital_enchancement_m1a1.Value;
                if (has_digital_enhancement) {
                    DigitalEnhancement digital_enhance = mainGun.FCS.gameObject.AddComponent<DigitalEnhancement>();
                    digital_enhance.original_blur = night_optic.slot.BaseBlur;
                    digital_enhance.slot = night_optic.slot;
                    digital_enhance.reticle_plane = night_optic.slot.transform.Find("Reticle Mesh/FFP");
                    digital_enhance.Add(2.4f, 0.01f, 0.69f);
                    digital_enhance.Add(1f, 0.02f, 0.29f);
                }
                bool has_du_package = is_m1ip ? du_package_m1ip.Value : du_package_m1.Value;
                int du_gen = is_m1ip ? du_gen_m1ip.Value : du_gen_m1.Value;
                vic_go.GetComponent<Rigidbody>().mass = has_du_package && vic.FriendlyName == "M1A1" ? 62781.3776f : 57152.6386f;
                if (has_du_package && vic._friendlyName == "M1A1") {
                    vic._friendlyName += du_gen > 1 ? "HC" : "HA";

                    GameObject turret_cheeks = vic.transform.Find("IPM1_rig/HULL/TURRET").GetComponent<LateFollowTarget>()
                        ._lateFollowers[0].transform.Find("Turret_Armor/cheeks composite arrays").gameObject;

                    VariableArmor var_armour = turret_cheeks.GetComponent<VariableArmor>();
                    var_armour._armorType = DUArmour.du_armor_codexes[du_gen - 1];

                    AarVisual cheek_visual = turret_cheeks.GetComponent<AarVisual>();

                    cheek_visual.AarMaterial = DUArmour.du_aar_mats[du_gen - 1];
                }

                bool rotate_azimuth = is_m1ip ? rotate_azimuth_m1ip.Value : rotate_azimuth_m1.Value;
                if (rotate_azimuth) {
                    optic.RotateAzimuth = true;
                    optic.slot.LinkedNightSight.PairedOptic.RotateAzimuth = true;
                    optic.slot.VibrationShakeMultiplier = 0f;
                    optic.slot.VibrationPreBlur = false;
                    optic.Alignment = OpticAlignment.BoresightStabilized;
                    optic.slot.LinkedNightSight.PairedOptic.Alignment = OpticAlignment.BoresightStabilized;
                }

                bool has_citv = is_m1ip ? citv_m1a1.Value : citv_m1e1.Value;
                if (has_citv) {
                    GameObject c = GameObject.Instantiate(citv_obj, vic.transform.Find("IPM1_rig/HULL/TURRET"));
                    c.transform.localPosition = new Vector3(-0.6794f, 0.9341f, 0.4348f);
                    c.transform.localEulerAngles = new Vector3(0f, 0f, 0f);

                    c.transform.SetParent(vic.transform.Find("IPM1_rig/HULL/TURRET").GetComponent<LateFollowTarget>()
                        ._lateFollowers[0].transform, true);

                    CITV citv_component = vic.DesignatedCameraSlots[0].LinkedNightSight.gameObject.AddComponent<CITV>();
                    citv_component.model = c;

                    c.transform.Find("assembly").GetComponent<UniformArmor>().Unit = vic;
                    c.transform.Find("glass").GetComponent<UniformArmor>().Unit = vic;

                    vic._targetSpotterSettings._periscopeFOV = 120f;
                    vic.TargetSpotterSettings._sightDistance = 4500f;
                    vic.TargetSpotterSettings._nightSightDistanceIdeal = 4500f;
                    vic.TargetSpotterSettings._nightSightDistancePassive = 4500f;
                    vic.transform.Find("Unit AI").GetComponent<TankTargetSensor>()._spotTimeMax = 2f;

                    vic._friendlyName += "+";
                }

                if ((vic.FriendlyName == "M1A1HA+" || vic.FriendlyName == "M1A1HC+") && rotate_azimuth) {
                    vic._friendlyName = "M1A2";
                }

                if (vic.FriendlyName == "M1A2" && (flir_gen > 2 || crows_m1a1.Value)) {
                    vic._friendlyName += " SEP";
                }

                mainGunInfo.Name = "120mm gun M256";
                mainGun.Impulse = 68000;
                mainGun.CodexEntry = gun_m256;
                mainGun.WeaponSound.SingleShotEventPaths[0] = "event:/Weapons/canon_125mm-2A46";

                GameObject dummy_tube = new GameObject("dummy tube");
                dummy_tube.transform.parent = vic_go.transform.Find("IPM1_rig/HULL/TURRET/GUN");
                dummy_tube.transform.localScale = new Vector3(0f, 0f, 0f);

                Transform smr_path = (vic.UniqueName == "M1") ? vic.transform.Find("M1_rig/M1_skinned") : vic.transform.Find("IPM1_rig/M1IP_skinned");
                int gun_recoil_idx = (vic.UniqueName == "M1") ? 46 : 56;
                SkinnedMeshRenderer smr = smr_path.GetComponent<SkinnedMeshRenderer>();
                Transform[] bones = smr.bones;
                bones[gun_recoil_idx] = dummy_tube.transform;
                smr.bones = bones;

                GameObject gunTube = vic_go.transform.Find("IPM1_rig/HULL/TURRET/GUN/gun_recoil").gameObject;
                gunTube.transform.Find("GUN/Gun Breech.001").GetComponent<MeshRenderer>().enabled = false;
                GameObject _m256_obj = GameObject.Instantiate(m256_obj, gunTube.transform);
                _m256_obj.transform.localPosition = new Vector3(0f, 0.0064f, -1.9416f);

                Transform muzzleFlashes = mainGun.MuzzleEffects[1].transform;
                muzzleFlashes.GetChild(1).transform.localScale = new Vector3(1.3f, 1.3f, 1f);
                muzzleFlashes.GetChild(2).transform.localScale = new Vector3(1.3f, 1.3f, 1f);
                muzzleFlashes.GetChild(4).transform.localScale = new Vector3(1.3f, 1.3f, 1f);

                // convert ammo
                string ap_idx = is_m1ip ? sabot_m1ip.Value : sabot_m1.Value;
                string heat_idx = is_m1ip ? heat_m1ip.Value : heat_m1.Value;
                AmmoClipCodexScriptable sabotClipCodex = Ammo_120mm.ap[ap_idx];
                AmmoClipCodexScriptable heatClipCodex = Ammo_120mm.heat[heat_idx];

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
                vic_go.AddComponent<Util.AlreadyConverted>();
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
                assem.layer = 8;
                glass.layer = 8;

                assem.GetComponent<MeshRenderer>().material.shader = Shader.Find("Standard (FLIR)");
                glass.GetComponent<MeshRenderer>().material.shader = Shader.Find("Standard (FLIR)");
                citv_obj.AddComponent<HeatSource>().heat = 5f;

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
                m256_obj.AddComponent<HeatSource>().heat = 5f;
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