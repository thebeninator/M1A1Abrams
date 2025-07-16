using GHPC.Camera;
using GHPC.Equipment.Optics;
using GHPC.Player;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using GHPC.Weapons;
using System.Reflection;
using GHPC.Utility;
using GHPC;

namespace M1A1Abrams
{
    public class CITVManager : MonoBehaviour {
        public GameObject monitor;
        public GameObject wfov;
        public GameObject nfov;
        private Transform canvas;
        private Transform _3d_reticle;
        private FieldInfo snapp;
        private FieldInfo max_distance; 

        private Vector3 CITVCurrentAimPoint
        {
            get
            {
                var cam_follow = CameraManager.Instance.CameraFollow;
                float _max_distance = (float)max_distance.GetValue(cam_follow.BufferedCamera);

                Ray ray = new Ray(cam_follow.BufferedCamera.transform.position, cam_follow.CurrentAimVector);
                RaycastHit raycastHit;

                if (!Physics.Raycast(ray, out raycastHit, _max_distance, ConstantsAndInfoManager.Instance.LaserRangefinderLayerMask.value))
                {
                    return cam_follow.BufferedCamera.transform.position + ray.direction * _max_distance;
                }

                return raycastHit.point;
            }
        }

        public static void Init() {
            if (GameObject.Find("CITV manager") != null) return;

            GameObject manager_obj = new GameObject("CITV manager");
            manager_obj.transform.SetParent(GameObject.Find("_APP_GHPC_").transform);
            CITVManager manager = manager_obj.AddComponent<CITVManager>();           
        }

        void Awake() {
            canvas = GameObject.Find("_APP_GHPC_").transform.Find("UIHUDCanvas");
            _3d_reticle = canvas.Find("3P aim reticle");

            monitor = GameObject.Instantiate(Assets.citv_monitor, GameObject.Find("_APP_GHPC_").transform);
            monitor.transform.localPosition = new Vector3(0f, 0f, 0f);
            monitor.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            monitor.GetComponent<Canvas>().sortingOrder = -1;
            monitor.SetActive(false);

            nfov = monitor.transform.Find("nfov").gameObject;
            wfov = monitor.transform.Find("wfov").gameObject;

            snapp = typeof(GHPC.Camera.BufferedCameraFollow).GetField("SNAPPINESS", BindingFlags.Static | BindingFlags.NonPublic);
            max_distance = typeof(GHPC.Camera.BufferedCameraFollow).GetField("MAX_AIM_DISTANCE", BindingFlags.Static | BindingFlags.NonPublic);
        }

        void Update() {
            CameraSlot cam_slot = CameraManager.Instance._currentCamSlot;
            if (cam_slot != null && cam_slot.GetComponent<CITV>() != null)
            {
                monitor.SetActive(true);
                _3d_reticle.gameObject.SetActive(false);

                nfov.gameObject.SetActive(cam_slot.CurrentFov < 10f);
                wfov.gameObject.SetActive(cam_slot.CurrentFov >= 10f);

                if (M1A1.citv_smooth.Value) {
                    snapp.SetValue(null, 0.20f);
                }

                if (InputUtil.MainPlayer.GetButton("Smooth Aim")) {
                    FireControlSystem fcs = PlayerInput.Instance.CurrentPlayerWeapon.Weapon.FCS;
                    fcs.SetAimWorldPosition(this.CITVCurrentAimPoint);
                }
            }
            else {
                monitor.SetActive(false);
                _3d_reticle.gameObject.SetActive(true);

                if (M1A1.citv_smooth.Value)
                    snapp.SetValue(null, 10f);
            }
        }
    } 

    public class CITV : MonoBehaviour
    {
        private static GHPC.Camera.BufferedCameraFollow camera;
        public GameObject model; 
        private CameraSlot nods;
        private int id;
        private float curr_sens = 5f;

        void Awake() {
            if (camera == null)
                camera = CameraManager.Instance.CameraFollow;

            nods = gameObject.GetComponent<CameraSlot>();
            id = gameObject.GetInstanceID();

            nods.transform.localPosition = new Vector3(-1.1638f, -0.3435f, 1.2273f);
            nods.VisionType = NightVisionType.Thermal;
            nods.IsExterior = false;
            nods.BaseBlur = 0f;
            nods.OverrideFLIRResolution = true;
            nods.CanToggleFlirPolarity = true;
            nods.FLIRWidth = 1024;
            nods.FLIRHeight = 576;
            nods.DefaultFov = 15f;
            nods.OtherFovs = new float[] {10f, 4f};
            nods.SpriteType = CameraSpriteManager.SpriteType.DefaultScope;
            nods.FLIRBlitMaterialOverride = Assets.flir_blit_mat_green_no_scan;
            SimpleNightVision nods_snv = nods.GetComponent<SimpleNightVision>();
            Component.Destroy(nods.GetComponent<PostProcessVolume>());
            GameObject post = GameObject.Instantiate(Assets.flir_post, nods.transform);
            post.transform.Find("MainCam Volume").gameObject.SetActive(false);

            PostProcessVolume post_vol = post.transform.Find("FLIR Only Volume").GetComponent<PostProcessVolume>();
            post_vol.enabled = true;
            nods_snv._postVolume = post_vol.GetComponent<PostProcessVolume>();
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

            curr_sens = 1.25f * (nods.CurrentFov / 60f);
            camera.aimSensitivity3P = curr_sens;
            model.SetActive(false);
        }
    }
}
