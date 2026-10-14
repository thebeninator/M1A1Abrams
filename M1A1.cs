using MelonLoader;
using UnityEngine;
using GHPC.State;
using System.Collections;
using GHPC.Weapons;
using GHPC.Equipment.Optics;
using GHPC.Vehicle;
using GHPC.Utility;
using GHPC;
using GHPC.Camera;
using GHPC.Player;
using GHPC.Weaponry;
using ModUtil;
using Presets;
using M1A1Abrams.Presets;

namespace M1A1Abrams
{
    public class M1A1 : Module
    {
        static MelonPreferences_Entry<bool> use_preset_pool;

        internal static PresetManager<M1Preset> preset_manager_m1;
        static PresetManager<M1Preset> preset_manager_m1ip;

        static MelonPreferences_Entry<string> target_preset_m1;
        static MelonPreferences_Entry<string> target_preset_m1ip;

        static MelonPreferences_Entry<bool> has_m256_m1;
        static MelonPreferences_Entry<bool> has_m256_m1ip;

        static MelonPreferences_Entry<string> sabot_m1;
        static MelonPreferences_Entry<string> sabot_m1ip;
        static MelonPreferences_Entry<string> heat_m1;
        static MelonPreferences_Entry<string> heat_m1ip;

        static MelonPreferences_Entry<int> flir_gen_m1;
        static MelonPreferences_Entry<int> flir_gen_m1ip;

        static MelonPreferences_Entry<int> m829_count_m1;
        static MelonPreferences_Entry<int> m830_count_m1;

        static MelonPreferences_Entry<int> m829_count_m1ip;
        static MelonPreferences_Entry<int> m830_count_m1ip;

        static MelonPreferences_Entry<bool> rotate_azimuth_m1;
        static MelonPreferences_Entry<bool> rotate_azimuth_m1ip;

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

        public static MelonPreferences_Entry<bool> original_rack_size_m1;
        public static MelonPreferences_Entry<bool> original_rack_size_m1ip;

        static WeaponSystemCodexScriptable gun_m256;
        static WeaponSystemCodexScriptable gun_m256a1;

        static GameObject addon_turret;
        static GameObject addon_hull;
        static GameObject addon_turret_l55;

        public static Vector2Int flir_gen2_res = new Vector2Int(1024, 576);
        public static Vector2Int flir_gen3_res = new Vector2Int(1366, 768);

        public static void Config(MelonPreferences_Category cfg)
        {
            use_preset_pool = cfg.CreateEntry<bool>("Use Preset Bundle", false);
            target_preset_m1 = cfg.CreateEntry<string>("Preset Bundle Path (M1)", "m1a1assets/PresetBundles/ExampleM1");
            target_preset_m1ip = cfg.CreateEntry<string>("Preset Bundle Path (M1IP)", "m1a1assets/PresetBundles/ExampleM1IP");

            has_m256_m1 = cfg.CreateEntry<bool>("M256 Gun (M1)", false);
            has_m256_m1.Description = "120mm cannon (Note: anything relating to ammo is ignored if this is false)";
            has_m256_m1ip = cfg.CreateEntry<bool>("M256 Gun (M1IP)", true);

            m829_count_m1 = cfg.CreateEntry<int>("AP Round Count (M1E1)", 22);
            m829_count_m1.Description = "Maximum of 40 rounds total. Bring in at least one AP round.";
            m830_count_m1 = cfg.CreateEntry<int>("HEAT Round Count (M1E1)", 18);

            m829_count_m1ip = cfg.CreateEntry<int>("AP Round Count (M1A1)", 22);
            m830_count_m1ip = cfg.CreateEntry<int>("HEAT Round Count (M1A1)", 18);

            original_rack_size_m1 = cfg.CreateEntry<bool>("Vanilla Ammo Capacity (M1E1)", false);
            original_rack_size_m1.Description = "Carry a total of 52 rounds instead of 40.";
            original_rack_size_m1ip = cfg.CreateEntry<bool>("Vanilla Ammo Capacity (M1A1)", false);

            sabot_m1 = cfg.CreateEntry<string>("AP Round (M1E1)", "M827");
            sabot_m1.Description = "Customize which rounds M1A1s/M1E1s use";
            sabot_m1.Comment = "M827, M829, M829A1, M829A2, M829A3";
            sabot_m1ip = cfg.CreateEntry<string>("AP Round (M1A1)", "M829");

            heat_m1 = cfg.CreateEntry<string>("HEAT Round (M1E1)", "M830");
            heat_m1.Comment = "M830, M830A1 (has proximity fuse that can be toggled by double tapping the ammo 2 key)";
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
            de_fixed_reticle_size = cfg.CreateEntry<bool>("Fixed Reticle Size (Global)", false);
            de_fixed_reticle_size.Comment = "Digitally enhanced zoom levels will not increase the size of the reticle.";

            citv_m1e1 = cfg.CreateEntry<bool>("CITV (M1E1)", false);
            citv_m1e1.Description = "Replaces commander's NVGs with variable-zoom thermals.";
            citv_m1a1 = cfg.CreateEntry<bool>("CITV (M1A1)", false);

            citv_smooth = cfg.CreateEntry<bool>("Smooth CITV Panning (Global)", true);
            citv_smooth.Comment = "Makes CITV feel more like a camera.";

            du_package_m1ip = cfg.CreateEntry<bool>("DU Armour (M1A1)", false);
            du_package_m1ip.Description = "DU inserts for the composite turret cheeks: increased KE protection. M1A1/M1IP exclusive. Increased weight.";
            du_gen_m1ip = cfg.CreateEntry<int>("DU Generation (M1A1)", 1);
            du_gen_m1ip.Comment = "Higher generation = more KE protection (1-3, integer)";

            du_package_m1 = cfg.CreateEntry<bool>("DU Armour (M1E1)", false);
            du_gen_m1 = cfg.CreateEntry<int>("DU Generation (M1E1)", 1);
            du_package_m1.Comment = "Doesn't apply to M1/M1E1, only those converted to M1A1/M1IP";

            m1_to_m1ip = cfg.CreateEntry<bool>("M1 -> M1IP", false);
            m1_to_m1ip.Description = "Convert all M1s to M1IPs (will still use M1E1 settings)";

            if (use_preset_pool.Value)
            {
                preset_manager_m1 = new PresetManager<M1Preset>(target_preset_m1.Value);
                preset_manager_m1ip = new PresetManager<M1Preset>(target_preset_m1ip.Value);
            }
        }

        private static void HandleConversion(Vehicle vic)
        {
            if (vic == null) return;

            GameObject vic_go = vic.gameObject;

            if (vic.FriendlyName != "M1IP" && vic.FriendlyName != "M1 Abrams") return;

            WeaponsManager weapons_manager = vic.GetComponent<WeaponsManager>();
            WeaponSystemInfo main_gun_info = weapons_manager.Weapons[0];
            WeaponSystem main_gun = main_gun_info.Weapon;
            UsableOptic optic = vic.transform.Find("IPM1_rig/HULL/TURRET/Turret Scripts/GPS/Optic").GetComponent<UsableOptic>();
            UsableOptic night_optic = optic.slot.LinkedNightSight.PairedOptic;
            Transform gas = vic.transform.Find("IPM1_rig/HULL/TURRET/GUN/Gun Scripts/Aux sight (GAS)");

            optic.slot.ExclusiveWeapons = new WeaponSystem[] { weapons_manager.Weapons[0].Weapon, weapons_manager.Weapons[1].Weapon };
            night_optic.slot.ExclusiveWeapons = new WeaponSystem[] { weapons_manager.Weapons[0].Weapon, weapons_manager.Weapons[1].Weapon };

            MPATManager mpat_manager = vic_go.AddComponent<MPATManager>();
            mpat_manager.AmmoCachedIdx = Ammo_120mm.ammo_m830a1.CachedIndex;
            mpat_manager.AmmoKeyIdx = 1;

            bool is_m1ip = vic.UniqueName == "M1IP Abrams" && vic.GetComponent<PreviouslyM1>() == null;
            bool player_controlled = vic.GetInstanceID() == PlayerInput.Instance.CurrentPlayerUnit.GetInstanceID();

            int cfg_flir_gen = is_m1ip ? flir_gen_m1ip.Value : flir_gen_m1.Value;
            bool cfg_digital_enhancement = is_m1ip ? digital_enchancement_m1a1.Value : digital_enchancement_m1e1.Value;
            bool cfg_du_package = is_m1ip ? du_package_m1ip.Value : du_package_m1.Value;
            int cfg_du_gen = is_m1ip ? du_gen_m1ip.Value : du_gen_m1.Value;
            bool cfg_rotate_azimuth = is_m1ip ? rotate_azimuth_m1ip.Value : rotate_azimuth_m1.Value;
            bool cfg_citv = is_m1ip ? citv_m1a1.Value : citv_m1e1.Value;
            bool cfg_m256 = is_m1ip ? has_m256_m1ip.Value : has_m256_m1.Value;
            bool cfg_vanilla_ammo_cap = is_m1ip ? original_rack_size_m1ip.Value : original_rack_size_m1.Value;
            int cfg_m829_count = is_m1ip ? m829_count_m1ip.Value : m829_count_m1.Value;
            int cfg_m830_count = is_m1ip ? m830_count_m1ip.Value : m830_count_m1.Value;
            string cfg_ap_type = is_m1ip ? sabot_m1ip.Value : sabot_m1.Value;
            string cfg_heat_type = is_m1ip ? heat_m1ip.Value : heat_m1.Value;

            if (use_preset_pool.Value)
            {
                PresetManager<M1Preset> preset_manager = is_m1ip ? preset_manager_m1ip : preset_manager_m1;
                M1Preset preset = preset_manager.ChoosePreset();
                PresetMarker marker = vic.GetComponent<PresetMarker>();

                if (marker != null)
                {
                    preset = (M1Preset)marker.Preset;
                }

                if (preset_manager.HasPlayerReservedPreset && player_controlled)
                {
                    preset = preset_manager.PlayerReservedPreset;
                }

                cfg_flir_gen = preset.FLIRGeneration;
                cfg_digital_enhancement = preset.DigitalEnhancement;
                cfg_du_gen = preset.DUGeneration;
                cfg_du_package = preset.DUArmour;
                cfg_citv = preset.CITV;
                cfg_m256 = preset.M256Gun;
                cfg_vanilla_ammo_cap = preset.VanillaAmmoCapacity;
                cfg_m829_count = preset.APCount;
                cfg_m830_count = preset.HEATCount;
                cfg_ap_type = preset.APRound;
                cfg_heat_type = preset.HEATRound;
                cfg_rotate_azimuth = preset.RotateAzimuth;

                Component.Destroy(marker);
            }

            if (cfg_m256)
            {
                vic._friendlyName = (vic.FriendlyName == "M1IP") ? "M1A1" : "M1E1";
            }

            if (cfg_flir_gen > 1)
            {
                Vector2Int resolution = cfg_flir_gen == 2 ? flir_gen2_res : flir_gen3_res;
                night_optic.slot.VibrationShakeMultiplier = 0.0f;
                night_optic.slot.FLIRWidth = resolution.x;
                night_optic.slot.FLIRHeight = resolution.y;
                night_optic.slot.FLIRBlitMaterialOverride = Assets.flir_blit_mat_green_no_scan;
            }

            if (cfg_digital_enhancement)
            {
                DigitalEnhancement digital_enhance = main_gun.FCS.gameObject.AddComponent<DigitalEnhancement>();
                digital_enhance.original_blur = night_optic.slot.BaseBlur;
                digital_enhance.slot = night_optic.slot;
                digital_enhance.reticle_plane = night_optic.slot.transform.Find("Reticle Mesh/FFP");
                digital_enhance.Add(2.4f, 0.01f, 0.69f);
                digital_enhance.Add(1f, 0.02f, 0.29f);
            }

            if (cfg_du_package && vic._uniqueName == "M1IP Abrams")
            {
                vic_go.GetComponent<Rigidbody>().mass = 62781.3776f;
                vic._friendlyName += cfg_du_gen > 1 ? "HC" : "HA";

                GameObject turret_cheeks = vic.transform.Find("IPM1_rig/HULL/TURRET").GetComponent<LateFollowTarget>()
                    ._lateFollowers[0].transform.Find("Turret_Armor/cheeks composite arrays").gameObject;

                VariableArmor var_armour = turret_cheeks.GetComponent<VariableArmor>();
                var_armour._armorType = DUArmour.du_armor_codexes[cfg_du_gen - 1];

                AarVisual cheek_visual = turret_cheeks.GetComponent<AarVisual>();

                cheek_visual.AarMaterial = DUArmour.du_aar_mats[cfg_du_gen - 1];
            }

            if (cfg_rotate_azimuth)
            {
                optic.RotateAzimuth = true;
                optic.slot.LinkedNightSight.PairedOptic.RotateAzimuth = true;
                optic.slot.VibrationShakeMultiplier = 0f;
                optic.slot.VibrationPreBlur = false;
                optic.Alignment = OpticAlignment.BoresightStabilized;
                optic.slot.LinkedNightSight.PairedOptic.Alignment = OpticAlignment.BoresightStabilized;
            }

            if (cfg_citv)
            {
                GameObject c = GameObject.Instantiate(Assets.citv_obj, vic.transform.Find("IPM1_rig/HULL/TURRET"));
                c.transform.localPosition = new Vector3(-0.6794f, 0.9341f, 0.4348f);
                c.transform.localEulerAngles = new Vector3(0f, 0f, 0f);

                c.transform.SetParent(vic.transform.Find("IPM1_rig/HULL/TURRET").GetComponent<LateFollowTarget>()
                    ._lateFollowers[0].transform, true);

                CITV citv_component = vic.DesignatedCameraSlots[0].LinkedNightSight.gameObject.AddComponent<CITV>();
                citv_component.model = c;

                if (vic.GetInstanceID() == PlayerInput.Instance.CurrentPlayerUnit.GetComponent<Vehicle>().GetInstanceID())
                {
                    CameraManager.Instance.UpdateLightMode(mode: NightVisionType.Thermal);
                    CameraSlot.ActiveInstance.ThisActiveChanged(true);
                }

                c.transform.Find("assembly").GetComponent<UniformArmor>().Unit = vic;
                c.transform.Find("glass").GetComponent<UniformArmor>().Unit = vic;

                vic._targetSpotterSettings._periscopeFOV = 120f;
                vic.TargetSpotterSettings._sightDistance = 4500f;
                vic.TargetSpotterSettings._nightSightDistanceIdeal = 4500f;
                vic.TargetSpotterSettings._nightSightDistancePassive = 4500f;
                vic._friendlyName += "+";

                if (vic.UniqueName == "M1IP Abrams")
                {
                    vic_go.transform.Find("IPM1_rig/HULL/TURRET/sparewheel_roof").gameObject.SetActive(false);
                } 
                else
                { 
                    vic_go.transform.Find("IPM1_rig/HULL/TURRET/M1A0 sparewheel_roof").gameObject.SetActive(false);
                }
            }

            if ((vic.FriendlyName == "M1A1HA+" || vic.FriendlyName == "M1A1HC+") && cfg_rotate_azimuth)
            {
                vic._friendlyName = "M1A2";
            }

            if (vic.FriendlyName == "M1A2" && cfg_flir_gen > 2)
            {
                vic._friendlyName += " SEP";
            }

            if (cfg_m256)
            {
                main_gun_info.Name = "120mm gun M256";
                main_gun.Impulse = 68000;
                main_gun.CodexEntry = gun_m256;
                main_gun.WeaponSound.SingleShotEventPaths[0] = "event:/Weapons/canon_120mm-RH120";

                GameObject dummy_tube = new GameObject("dummy tube");
                dummy_tube.transform.parent = vic_go.transform.Find("IPM1_rig/HULL/TURRET/GUN");
                dummy_tube.transform.localScale = new Vector3(0f, 0f, 0f);

                Transform smr_path = (vic.UniqueName == "M1") ? vic.transform.Find("M1A0_mesh/M1A0 turret") : vic.transform.Find("M1IP_mesh/M1IP_turret");
                int gun_recoil_idx = (vic.UniqueName == "M1") ? 13 : 9;
                SkinnedMeshRenderer smr = smr_path.GetComponent<SkinnedMeshRenderer>();
                Transform[] bones = smr.bones;
                bones[gun_recoil_idx] = dummy_tube.transform;
                smr.bones = bones;

                GameObject gunTube = vic_go.transform.Find("IPM1_rig/HULL/TURRET/GUN/gun_recoil").gameObject;
                gunTube.transform.Find("GUN/Gun Breech.001").GetComponent<MeshRenderer>().enabled = false;

                GameObject _m256_obj = GameObject.Instantiate(Assets.m256_obj, gunTube.transform);
                _m256_obj.transform.localPosition = new Vector3(0f, 0.0064f, -1.9416f);

                Transform muzzleFlashes = main_gun.MuzzleEffects[1].transform;
                muzzleFlashes.GetChild(1).transform.localScale = new Vector3(1.3f, 1.3f, 1f);
                muzzleFlashes.GetChild(2).transform.localScale = new Vector3(1.3f, 1.3f, 1f);
                muzzleFlashes.GetChild(4).transform.localScale = new Vector3(1.3f, 1.3f, 1f);

                // convert ammo
                string ap_idx = cfg_ap_type.ToUpper();
                string heat_idx = cfg_heat_type.ToUpper();
                AmmoClipCodexScriptable sabotClipCodex = Ammo_120mm.ap[ap_idx];
                AmmoClipCodexScriptable heatClipCodex = Ammo_120mm.heat[heat_idx];

                LoadoutManager loadoutManager = vic.GetComponent<LoadoutManager>();
                loadoutManager.TotalAmmoCounts = new int[] { cfg_m829_count, cfg_m830_count };
                loadoutManager.LoadedAmmoList.AmmoClips = new AmmoClipCodexScriptable[] { sabotClipCodex, heatClipCodex };

                if (!cfg_vanilla_ammo_cap)
                {
                    loadoutManager._totalAmmoCount = 40;

                    for (int i = 0; i <= 2; i++)
                    {
                        GHPC.Weapons.AmmoRack rack = loadoutManager.RackLoadouts[i].Rack;
                        rack.ClipCapacity = i == 2 ? 4 : 18;
                        Util.EmptyRack(rack);
                    }
                }

                loadoutManager.SpawnCurrentLoadout();
                main_gun.Feed.AmmoTypeInBreech = null;
                main_gun.Feed.Start();
                loadoutManager.RegisterAllBallistics();

                vic_go.transform.Find("IPM1_rig/HULL/TURRET/GUN/turret_gun").gameObject.SetActive(false);

                if (vic.UniqueName == "M1IP Abrams")
                {
                    vic_go.transform.Find("IPM1_rig/HULL/TURRET/M1 camo net/turret_gun").gameObject.SetActive(false);
                }

                GAS.Create(Ammo_120mm.ap[sabot_m1ip.Value].ClipType.MinimalPattern[0], Ammo_120mm.heat[heat_m1ip.Value].ClipType.MinimalPattern[0]);
                GAS.Add(gas, optic.slot.ExclusiveWeapons);
            }
        }

        public static IEnumerator Convert(GameState _)
        {
            foreach (Vehicle vic in M1A1AbramsMod.vics)
            {
                HandleConversion(vic);
            }

            yield break;
        }

        public static void Init()
        {            
            if (gun_m256 == null)
            {
                gun_m256 = ScriptableObject.CreateInstance<WeaponSystemCodexScriptable>();
                gun_m256.name = "gun_m256";
                gun_m256.CaliberMm = 120;
                gun_m256.FriendlyName = "120mm Gun M256";
                gun_m256.Type = WeaponSystemCodexScriptable.WeaponType.LargeCannon;
            }

            StateController.RunOrDefer(GameState.PlayerReady, new GameStateEventHandler(Convert), GameStatePriority.Medium);
        }
    }
}