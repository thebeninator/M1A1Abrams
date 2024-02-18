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

namespace M1A1Abrams
{
    public class CITV : MonoBehaviour
    {
        private GHPC.Camera.BufferedCameraFollow camera;
        private CameraSlot nods;
        private int id;
        private float curr_sens = 5f;

        void Awake() {
            camera = GameObject.Find("_APP_GHPC_").GetComponent<CameraManager>().CameraFollow;

            nods = gameObject.GetComponent<CameraSlot>();
            id = gameObject.GetInstanceID();

            nods.gameObject.transform.localPosition = new Vector3(-1.0409f, -0.272f, 1.264f);
            nods.VisionType = NightVisionType.Thermal;
            nods.IsExterior = false;
            nods.BaseBlur = M1A1.perfect_citv.Value ? 0f : 0.001f;
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
            bloom.threshold.value = 0.86f;
            bloom.threshold.overrideState = true;
            grain.intensity.value = 0.2f;
            grain.size.value = 0.1f;
        }

        void Update() {
            if (camera.commanderHead == null || camera.commanderHead.GetComponent<CITV>() == null)
            {
                camera.aimSensitivity3P = 5f;
                return;
            }

            if (camera.commanderHead.gameObject.GetInstanceID() != id)
                return;

            curr_sens = 3.5f * (nods.CurrentFov / 60f);
            camera.aimSensitivity3P = curr_sens;
        }
    }
}
