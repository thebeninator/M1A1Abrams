using GHPC.Weapons;
using UnityEngine;
using HarmonyLib;

namespace M1A1Abrams
{
    [HarmonyPatch(typeof(GHPC.Weapons.LiveRound), "createExplosion")]
    public class Canister
    {
        public static void Prefix(GHPC.Weapons.LiveRound __instance, ref int spallCount)
        {
            if (__instance.Info.Name == "M1028 APERS")
            {
                spallCount = 0;

                for (int i = 0; i < 250; i++)
                {
                    GHPC.Weapons.LiveRound component;
                    component = LiveRoundMarshaller.Instance.GetRoundOfVisualType(LiveRoundMarshaller.LiveRoundVisualType.Invisible)
                        .GetComponent<GHPC.Weapons.LiveRound>();

                    component.Info = Ammo_120mm.canister_ball;
                    component.CurrentSpeed = 1640f;
                    component.MaxSpeed = 1640f;
                    component.IsSpall = false;
                    component.Shooter = __instance.Shooter;
                    component.transform.position = __instance.transform.position;

                    Vector3 radius = UnityEngine.Random.insideUnitSphere;
                    radius = new Vector3(radius.x * 0.1f, radius.y * 0.9f, radius.z * 0.25f);

                    component.transform.forward = Quaternion.Euler(radius) * __instance.transform.forward;
                    //component.transform.forward = Quaternion.Euler(
                    //    UnityEngine.Random.Range(-num * 0.1f, num * 0.1f), 
                    //    UnityEngine.Random.Range(-num * 0.5f, num * 0.5f), 
                    //    UnityEngine.Random.Range(-num * 0.2f, num * 0.2f)) * __instance.transform.forward;

                    component.Init(null, null);
                    component.name = "blyat" + i;
                }
            }
        }
    }
}
