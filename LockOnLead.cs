using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GHPC.Utility;
using GHPC;
using GHPC.Vehicle;
using UnityEngine;
using HarmonyLib;
using MelonLoader;
using GHPC.Weapons;

namespace M1A1Abrams
{
    public class LockOnLead : MonoBehaviour
    {
        public Vehicle target;
        public Transform target2; 
        public FireControlSystem fcs;
        private float cd = 0f;
        public bool engaged = false;
        public float dirx, diry = 1f;
        public float dirx2, diry2 = 1f;
        public bool yes = false;

        public void Awake() { 
           fcs = this.GetComponentInChildren<FireControlSystem>();
        }

        public void Up() {
            fcs._averageTraverseRate = new Vector2(
                fcs.Mounts[1].CurrentTraverseRateDegreesPerSecond * Math.Sign(dirx2 - dirx),
                fcs.Mounts[0].CurrentTraverseRateDegreesPerSecond * Math.Sign(diry2 - diry)
            );
        }

        public void Update()
        {
            if (fcs == null) return;

            cd -= Time.deltaTime;

            if (Input.GetKey(KeyCode.Mouse2) && cd <= 0f)
            {
                fcs.RecordTraverseRateBuffer = false;
                cd = 0.2f;
                engaged = true;
                fcs.DoLase();
            }

            if (target == null) return;

            fcs.FinalSetAimWorldPosition(target.Center.position);
            //fcs.FinalSetAimWorldPosition(target2.position);
            Invoke("Up", 0.1f);
        }
    }

    /*
    [HarmonyPatch(typeof(GHPC.Weapons.FireControlSystem), "AdjustAimByRatio")]
    public static class GetLayingDirection {
        private static void Postfix(GHPC.Weapons.FireControlSystem __instance, object[] __args)
        {
            LockOnLead lead = __instance.gameObject.GetComponentInParent<LockOnLead>();

            if (lead == null) return;

            // MelonLogger.Msg("d1 " + __instance.AimWorldVector);
            lead.dirx = Math.Sign((float)__args[0]);
            lead.diry = Math.Sign((float)__args[1]);
        }
    }*/


    [HarmonyPatch(typeof(GHPC.Weapons.FireControlSystem), "FinalSetAimWorldPosition")]
    public static class fgbddfgffghfgh
    {
        private static void Prefix(GHPC.Weapons.FireControlSystem __instance)
        {
            LockOnLead lead = __instance.gameObject.GetComponentInParent<LockOnLead>();

            if (lead != null) {
                lead.dirx = __instance.AimWorldVectorScaled.x;
                lead.diry = __instance.AimWorldVectorScaled.y;
            }
        }

        private static void Postfix(GHPC.Weapons.FireControlSystem __instance)
        {
            LockOnLead lead = __instance.gameObject.GetComponentInParent<LockOnLead>();

            if (lead != null)
            {
                lead.dirx2 = __instance.AimWorldVectorScaled.x;
                lead.diry2 = __instance.AimWorldVectorScaled.y;
            }
        }
    }

    /*
    [HarmonyPatch(typeof(GHPC.Weapons.WeaponSystem), "Fire")]
    public static class BLYAT
    {
        private static void Prefix(GHPC.Weapons.WeaponSystem __instance)
        {
            FireControlSystem fcs = __instance.FCS;

            LockOnLead lead = fcs.gameObject.GetComponentInParent<LockOnLead>();

            if (lead != null && lead.engaged) {
                fcs.RecordTraverseRateBuffer = false;
                lead.yes = true;
            }
        }
        
        private static void Postfix(GHPC.Weapons.WeaponSystem __instance)
        {
            FireControlSystem fcs = __instance.FCS;

            LockOnLead lead = fcs.gameObject.GetComponentInParent<LockOnLead>();

            if (lead != null && lead.engaged)
            {
                fcs.RecordTraverseRateBuffer = true;
                fcs._averageTraverseRate = new Vector2(0, 0);
            }
        }
        
    }
    */
    [HarmonyPatch(typeof(GHPC.Weapons.WeaponSystem), "StopFiring")]
    public static class BLYAT2
    {
        private static void Postfix(GHPC.Weapons.WeaponSystem __instance)
        {
            FireControlSystem fcs = __instance.FCS;

            LockOnLead lead = fcs.gameObject.GetComponentInParent<LockOnLead>();

            if (lead != null && lead.engaged)
            {
                MelonLogger.Msg("blyat");
            }
        }
    }

    [HarmonyPatch(typeof(GHPC.Weapons.FireControlSystem), "DoLase")]
    public static class LockTarget
    {
        private static void Postfix(GHPC.Weapons.FireControlSystem __instance)
        {
            LockOnLead lead = __instance.gameObject.GetComponentInParent<LockOnLead>();

            if (lead == null) return;

            if (!lead.engaged) return;

            if (lead.target != null) {
                lead.target = null;
                lead.target2 = null;
                lead.engaged = false;
                lead.fcs.DumpLead();
                lead.fcs.RecordTraverseRateBuffer = true;

                return;
            }

            float num = -1f;
            int layerMask = 1 << CodeUtils.LAYER_MASK_VISIBILITYONLY;
            RaycastHit raycastHit;
            if (Physics.Raycast(__instance.LaserOrigin.position, __instance.LaserOrigin.forward, out raycastHit, __instance.MaxLaserRange, layerMask) && raycastHit.collider.tag == "Smoke")
            {
                return;
            }
            if (Physics.Raycast(__instance.LaserOrigin.position, __instance.LaserOrigin.forward, out raycastHit, __instance.MaxLaserRange, ConstantsAndInfoManager.Instance.LaserRangefinderLayerMask.value) && (raycastHit.distance < num || num == -1f))
            {
                num = raycastHit.distance;
            }

            GameObject raycast_hit = raycastHit.transform.gameObject;

            lead.target = (Vehicle)raycast_hit.GetComponent<IArmor>().Unit;
           // lead.target = raycast_hit.GetComponent<IArmor>().Transform;

            return;
        }
    }
}