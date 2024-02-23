using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GHPC.Camera;
using GHPC.Equipment.Optics;
using GHPC.Player;
using Reticle;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using MelonLoader;
using MelonLoader.Utils;
using System.IO;
using GHPC.Weapons;
using System.Reflection;
using GHPC.Utility;

namespace M1A1Abrams
{
    public class CITVCrosshair : MonoBehaviour {
        private CameraManager cam_manager;
        private PlayerInput player_manager;
        private Transform canvas;
        private FieldInfo snapp;

        void Awake() {
            cam_manager = GameObject.Find("_APP_GHPC_").GetComponent<CameraManager>();
            player_manager = GameObject.Find("_APP_GHPC_").GetComponent<PlayerInput>();
            canvas = GameObject.Find("_APP_GHPC_").transform.Find("UIHUDCanvas");

            snapp = typeof(GHPC.Camera.BufferedCameraFollow).GetField("SNAPPINESS", BindingFlags.Static | BindingFlags.NonPublic);
        }

        void Update() {
            if (cam_manager._currentCamSlot != null && cam_manager._currentCamSlot.gameObject.GetComponent<CITV>() != null)
            {
                gameObject.GetComponent<UnityEngine.UI.Image>().enabled = true;
                canvas.Find("3P aim reticle").gameObject.SetActive(false);
      
                if (M1A1.citv_smooth.Value)
                    snapp.SetValue(null, 0.25f);

                if (M1A1.perfect_override.Value && InputUtil.MainPlayer.GetButton("Smooth Aim")) {
                    FireControlSystem fcs = player_manager.CurrentPlayerWeapon.Weapon.FCS;
                    fcs.SetAimWorldPosition(cam_manager.CameraFollow.CurrentAimPoint);
                }
            }
            else {    
                gameObject.GetComponent<UnityEngine.UI.Image>().enabled = false;
                canvas.Find("3P aim reticle").gameObject.SetActive(true);

                if (M1A1.citv_smooth.Value)
                    snapp.SetValue(null, 10f);
            }
        }
    } 

    public class CITV : MonoBehaviour
    {
        private static GHPC.Camera.BufferedCameraFollow camera;
        private static Sprite citv_crosshair;
        private static GameObject citv_crosshair_go;
        private static GameObject active_crosshair_instance;
        public GameObject model; 
        private CameraSlot nods;
        private int id;
        private float curr_sens = 5f;

        public static void Init() {
            if (citv_crosshair == null)
            {
                var citv_bundle = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory + "/m1a1CITV/", "citv_crosshair"));

                citv_crosshair = citv_bundle.LoadAsset<Sprite>("citv_crosshair");
                citv_crosshair.hideFlags = HideFlags.DontUnloadUnusedAsset;

                citv_crosshair_go = new GameObject("citv crosshair canvas");
                citv_crosshair_go.AddComponent<CanvasRenderer>();
                citv_crosshair_go.AddComponent<CITVCrosshair>();
                UnityEngine.UI.Image s = citv_crosshair_go.AddComponent<UnityEngine.UI.Image>();
                s.sprite = citv_crosshair;
            }
        }

        void Awake() {
            if (camera == null)
                camera = GameObject.Find("_APP_GHPC_").GetComponent<CameraManager>().CameraFollow;

            nods = gameObject.GetComponent<CameraSlot>();
            id = gameObject.GetInstanceID();

            var canvas = GameObject.Find("_APP_GHPC_").transform.Find("UIHUDCanvas");

            if (active_crosshair_instance == null && M1A1.citv_reticle.Value) {
                active_crosshair_instance = GameObject.Instantiate(citv_crosshair_go, canvas);
                active_crosshair_instance.transform.localPosition = new Vector3(0f, 0f, 572f);
                active_crosshair_instance.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                active_crosshair_instance.transform.localScale = new Vector3(6f, 3.1f, 1f);
                active_crosshair_instance.GetComponent<UnityEngine.UI.Image>().enabled = false;
                active_crosshair_instance.transform.SetAsFirstSibling();
            }

            nods.gameObject.transform.localPosition = new Vector3(-1.1638f, -0.3435f, 1.2273f);
            nods.VisionType = NightVisionType.Thermal;
            nods.IsExterior = false;
            nods.BaseBlur = M1A1.perfect_citv.Value ? 0f : 0.05f;
            nods.OtherFovs = new float[] {40f, 30f, 20f, 10f, 4f, 3.25f};
            nods.SpriteType = CameraSpriteManager.SpriteType.DefaultScope;
            PostProcessProfile nods_profile = nods.gameObject.GetComponent<SimpleNightVision>()._postVolume.profile;

            ColorGrading color_grading = nods_profile.settings[1] as ColorGrading;
            Bloom bloom = nods_profile.settings[0] as Bloom;
            Grain grain = nods_profile.settings[2] as Grain;

            color_grading.postExposure.value = 0f;
            color_grading.colorFilter.value = new RGB(0, 1, 0.2198f);
            color_grading.contrast.value = 100f;
            color_grading.tonemapper.value = Tonemapper.Neutral;
            color_grading.tonemapper.overrideState = true;
            bloom.intensity.value = 1f;
            bloom.softKnee.value = 0.4f;
            bloom.softKnee.overrideState = true;
            bloom.threshold.value = 0.55f;
            bloom.threshold.overrideState = true;
            grain.intensity.value = 0.2f;
            grain.size.value = 0.1f;
        }

        void Update() {
            if (camera.commanderHead == null || camera.commanderHead.GetComponent<CITV>() == null)
            {
                model.SetActive(true);
                camera.aimSensitivity3P = 5f;
                return;
            }

            if (camera.commanderHead.gameObject.GetInstanceID() != id)
            {
                model.SetActive(true);
                return;
            }

            curr_sens = 1.5f * (nods.CurrentFov / 60f);
            camera.aimSensitivity3P = curr_sens;
            model.SetActive(false);
        }
    }
}
