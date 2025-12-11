using System.IO;
using System.Linq;
using GHPC.Camera;
using GHPC.Equipment.Optics;
using GHPC.Vehicle;
using MelonLoader.Utils;
using Reticle;
using UnityEngine;
using GHPC.Weaponry;

namespace M1A1Abrams
{
    internal class Assets
    {
        public static bool done = false;
        public static AmmoCodexScriptable ammo_codex_m8;
        public static AmmoCodexScriptable ammo_codex_50tracer;

        public static AmmoType ammo_m833;
        public static AmmoType ammo_m456;

        public static GameObject m2_browning;
        public static GameObject m2_gun_sight;
        public static GameObject m2_fcs;
        public static GameObject box_canvas;

        public static GameObject flir_post;
        public static Material flir_blit_mat_green;
        public static Material flir_blit_mat_green_no_scan;

        public static GameObject citv_monitor;

        public static void Load() {
            ammo_codex_m8 = Resources.FindObjectsOfTypeAll<AmmoCodexScriptable>().Where(o => o.name == "ammo_M8").First();
            ammo_codex_50tracer = Resources.FindObjectsOfTypeAll<AmmoCodexScriptable>().Where(o => o.name == "ammo_50_tracer").First();
            ammo_m833 = Resources.FindObjectsOfTypeAll<AmmoCodexScriptable>().Where(o => o.name == "ammo_M833").First().AmmoType;
            ammo_m456 = Resources.FindObjectsOfTypeAll<AmmoCodexScriptable>().Where(o => o.name == "ammo_M456").First().AmmoType;

            if (!ReticleMesh.cachedReticles.ContainsKey("T55-NVS")) {
                Vehicle t55 = Resources.FindObjectsOfTypeAll<Vehicle>().Where(o => o.name == "T55A").First();
                t55.transform.Find("Gun Scripts/Sights (and FCS)/NVS").GetComponent<UsableOptic>().reticleMesh.Load();
            }

            if (!ReticleMesh.cachedReticles.ContainsKey("M1_105_GAS_APFSDS"))
            {
                Vehicle m1ip = Resources.FindObjectsOfTypeAll<Vehicle>().Where(o => o.name == "_M1IP (variant)").First();
                m1ip.transform.Find("Gun Scripts/Aux sight (GAS)").GetComponent<UsableOptic>().reticleMesh.Load();
            }

            Vehicle m60a3 = Resources.FindObjectsOfTypeAll<Vehicle>().Where(o => o.name == "M60A3 TTS").First();
            m2_browning = m60a3.transform.Find("Cupola Scripts/12.7mm Machine Gun M85").gameObject;
            m2_gun_sight = m60a3.transform.Find("Cupola Scripts/M85 gunsight").gameObject;
            m2_fcs = m60a3.transform.Find("Cupola Scripts/M85 FCS").gameObject;
            flir_post = m60a3.transform.Find("Turret Scripts/Sights/FLIR/FLIR Post Processing - Green").gameObject;
            flir_blit_mat_green = m60a3.transform.Find("Turret Scripts/Sights/FLIR").GetComponent<CameraSlot>().FLIRBlitMaterialOverride;

            flir_blit_mat_green_no_scan = new Material(Shader.Find("Blit (FLIR)/Blit Simple"));
            flir_blit_mat_green_no_scan.CopyPropertiesFromMaterial(flir_blit_mat_green);
            flir_blit_mat_green_no_scan.SetTexture("_PixelCookie", null);

            Vehicle m2_bradley = Resources.FindObjectsOfTypeAll<Vehicle>().Where(o => o.name == "M2 Bradley").First();
            box_canvas = GameObject.Instantiate(m2_bradley.transform.Find("FCS and sights/GPS Optic/M2 Bradley GPS canvas").gameObject);
            GameObject.Destroy(box_canvas.transform.GetChild(2).gameObject);
            box_canvas.SetActive(false);
            box_canvas.hideFlags = HideFlags.DontUnloadUnusedAsset;
            box_canvas.name = "boxy";

            var citv_bundle = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory + "/m1a1assets/", "citv_monitor"));
            citv_monitor = citv_bundle.LoadAsset<GameObject>("CITV MONITOR");
            citv_monitor.hideFlags = HideFlags.DontUnloadUnusedAsset;

            done = true;
        }
    }
}
