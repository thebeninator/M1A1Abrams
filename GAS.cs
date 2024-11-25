using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reticle;
using UnityEngine;

namespace M1A1Abrams
{
    public class GAS
    {
        public static ReticleSO reticleSO_heat;
        public static ReticleMesh.CachedReticle reticle_cached_heat;

        public static ReticleSO reticleSO_ap;
        public static ReticleMesh.CachedReticle reticle_cached_ap;

        public static void Create(AmmoCodexScriptable sabot, AmmoCodexScriptable heat) {
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
                reticle_range_ap.projectile = sabot;
                reticle_range_ap.UpdateBC();

                ReticleTree.Text reticle_text_ap = boresight.elements[0]
                    as ReticleTree.Text;

                reticle_text_ap.text = "APFSDS-T\nMETERS";

                reticleSO_heat = ScriptableObject.Instantiate(ReticleMesh.cachedReticles["M1_105_GAS_HEAT"].tree);
                reticleSO_heat.name = "120mm_gas_heat";

                Util.ShallowCopy(reticle_cached_heat, ReticleMesh.cachedReticles["M1_105_GAS_HEAT"]);
                reticle_cached_heat.tree = reticleSO_heat;

                ReticleTree.Angular boresight_heat = ((reticleSO_heat.planes[0]
                    as ReticleTree.FocalPlane).elements[0]
                    as ReticleTree.Angular);

                ReticleTree.VerticalBallistic reticle_range_heat = boresight_heat.elements[4]
                    as ReticleTree.VerticalBallistic;
                reticle_range_heat.projectile = heat;
                reticle_range_heat.UpdateBC();

                ReticleTree.Text reticle_text_heat = boresight_heat.elements[0]
                    as ReticleTree.Text;

                string heat_name = heat.AmmoType.Name;
                reticle_text_heat.text = heat_name + "\nMETERS";
            }
        }
    }
}
