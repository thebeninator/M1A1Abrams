using System;
using GHPC.AI;
using GHPC.Mission.Data;
using GHPC.Mission;
using GHPC;
using UnityEngine;
using HarmonyLib;

namespace M1A1Abrams
{
    public class PreviouslyM1 : MonoBehaviour { }

    [HarmonyPatch(typeof(UnitSpawner), "SpawnUnit", new Type[] { typeof(GameObject), typeof(UnitMetaData), typeof(WaypointHolder), typeof(Transform) })]
    public static class M1toM1IP
    {
        private static void Prefix(out bool __state, UnitSpawner __instance, ref GameObject prefab)
        {
            __state = false;

            bool conversion_reqd = M1A1.m1_to_m1ip.Value;

            if (prefab.name == "_M1 (variant)" && conversion_reqd)
            {
                __state = true;
                prefab = __instance.GetPrefabByUniqueName("M1IP");
            }
        }

        private static void Postfix(bool __state, ref IUnit __result)
        {
            if (__state)
            {
                PreviouslyM1 comp = __result.transform.gameObject.AddComponent<PreviouslyM1>();
                comp.enabled = false;
            }
        }
    }
}
