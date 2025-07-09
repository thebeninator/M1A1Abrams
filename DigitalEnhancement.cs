using System.Collections.Generic;
using UnityEngine;
using GHPC.Camera;

namespace M1A1Abrams
{
    public class DigitalEnhancement : MonoBehaviour
    {
        private Dictionary<float, float[]> zoom_levels = new Dictionary<float, float[]>();
        public CameraSlot slot;
        public Transform reticle_plane;
        public float original_blur;

        public void Add(float zoom, float blur, float reticle_scale) {
            slot.OtherFovs = Util.AppendToArray(slot.OtherFovs, zoom);
            zoom_levels.Add(zoom, new float[] { blur, reticle_scale});
        }

        void Update() {
            if (M1A1AbramsMod.playerManager.CurrentPlayerWeapon.FCS.GetInstanceID() != slot.PairedOptic.FCS.GetInstanceID()) return;

            if (!zoom_levels.ContainsKey(slot.CurrentFov)) {
                reticle_plane.localScale = Vector3.one;
                slot.BaseBlur = original_blur;
                return;
            };

            slot.BaseBlur = zoom_levels[slot.CurrentFov][0];
            float scale = M1A1.de_fixed_reticle_size.Value ? zoom_levels[slot.CurrentFov][1] : 1f;
            reticle_plane.localScale = new Vector3(scale, scale, scale);
        }
    }
}
