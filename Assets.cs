using System.IO;
using System.Linq;
using GHPC.Camera;
using GHPC.Vehicle;
using MelonLoader.Utils;
using UnityEngine;
using GHPC.Weaponry;
using ModUtil;
using GHPC.Thermals;
using GHPC;
using GHPC.Weapons;

namespace M1A1Abrams
{
    internal class Assets : Module
    {
        public static AmmoType ammo_m833;
        public static AmmoType ammo_m456;

        public static GameObject flir_post;
        public static Material flir_blit_mat_green;
        public static Material flir_blit_mat_green_no_scan;

        public static GameObject citv_monitor;

        public static GameObject citv_obj;
        public static GameObject m256_obj;

        public override void UnloadDynamicAssets()
        {
            Material.DestroyImmediate(flir_blit_mat_green_no_scan);
        }

        public override void LoadDynamicAssets()
        {
            if (!AssetUtil.VehicleInMission("_M1IP (variant)") && !AssetUtil.VehicleInMission("_M1 (variant)")) return;

            Vehicle vic;
            if (AssetUtil.VehicleInMission("_M1IP (variant)"))
            {
                vic = AssetUtil.LoadVanillaVehicle("M1IP");
            } 
            else
            {
                vic = AssetUtil.LoadVanillaVehicle("M1");
            }

            Transform flir = vic.GetComponentInChildren<FireControlSystem>(true).transform.Find("FLIR");

            flir_post = flir.Find("FLIR Post Processing - Green").gameObject;
            flir_blit_mat_green = flir.GetComponent<CameraSlot>().FLIRBlitMaterialOverride;
            flir_blit_mat_green_no_scan = new Material(Shader.Find("Blit (FLIR)/Blit Simple"));
            flir_blit_mat_green_no_scan.CopyPropertiesFromMaterial(flir_blit_mat_green);
            flir_blit_mat_green_no_scan.SetTexture("_PixelCookie", null);
        }

        public override void LoadStaticAssets()
        {
            ammo_m833 = Resources.FindObjectsOfTypeAll<AmmoCodexScriptable>().Where(o => o.name == "ammo_M833").First().AmmoType;
            ammo_m456 = Resources.FindObjectsOfTypeAll<AmmoCodexScriptable>().Where(o => o.name == "ammo_M456").First().AmmoType;

            var citv_bundle = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory + "/m1a1assets/", "citv_monitor"));
            citv_monitor = citv_bundle.LoadAsset<GameObject>("CITV MONITOR");
            citv_monitor.hideFlags = HideFlags.DontUnloadUnusedAsset;

            var citv_obj_bundle = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory + "/m1a1assets/", "citv"));
            citv_obj = citv_obj_bundle.LoadAsset<GameObject>("citv.prefab");
            citv_obj.hideFlags = HideFlags.DontUnloadUnusedAsset;
            citv_obj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

            GameObject assem = citv_obj.transform.Find("assembly").gameObject;
            GameObject glass = citv_obj.transform.Find("glass").gameObject;

            assem.GetComponent<MeshRenderer>().material.shader = Shader.Find("Standard (FLIR)");
            glass.GetComponent<MeshRenderer>().material.shader = Shader.Find("Standard (FLIR)");
            citv_obj.AddComponent<HeatSource>().heat = 0.5f;

            GameObject assem_armour = assem.transform.Find("ARMOUR").gameObject;
            GameObject glass_armour = glass.transform.Find("ARMOUR").gameObject;

            assem_armour.tag = "Penetrable";
            glass_armour.tag = "Penetrable";
            assem_armour.layer = 8;
            glass_armour.layer = 8;

            UniformArmor assem_u_armour = assem.AddComponent<UniformArmor>();
            UniformArmor glass_u_armour = glass.AddComponent<UniformArmor>();
            assem_u_armour.PrimarySabotRha = 40f;
            assem_u_armour.PrimaryHeatRha = 40f;

            glass_u_armour.PrimarySabotRha = 5f;
            glass_u_armour.PrimaryHeatRha = 5f;

            assem_u_armour._name = "CITV";
            glass_u_armour._name = "CITV glass";

            var m256_bundle = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory + "/m1a1assets/", "m256"));
            m256_obj = m256_bundle.LoadAsset<GameObject>("m256.prefab");
            m256_obj.hideFlags = HideFlags.DontUnloadUnusedAsset;
            m256_obj.transform.localScale = new Vector3(0.75f, 0.75f, 0.8f);
            m256_obj.GetComponent<MeshRenderer>().material.shader = Shader.Find("Standard (FLIR)");
            m256_obj.AddComponent<HeatSource>().heat = 0.5f;
        }
    }
}
