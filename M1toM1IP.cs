using System;
using GHPC.AI;
using GHPC.Mission.Data;
using GHPC.Mission;
using GHPC;
using UnityEngine;
using HarmonyLib;
using ModUtil;
using Presets;
using M1A1Abrams.Presets;
using VanillaAssetUtil.AssetUtil;

namespace M1A1Abrams
{
    public class PreviouslyM1 : MonoBehaviour {}

    public class TransmogState<T>
    {
        public bool TransmogNeeded { get; set; }
        public T Preset { get; set; }
    }

    [HarmonyPatch(typeof(UnitSpawner), "SpawnUnit", new Type[] { typeof(string), typeof(UnitMetaData), typeof(WaypointHolder), typeof(Transform) })]
    public static class M1toM1IP
    {
        private static void Prefix(out TransmogState<M1Preset> __state, UnitSpawner __instance, ref string uniqueName)
        {
            __state = new TransmogState<M1Preset>();

            if (uniqueName == "M1")
            {
                bool use_preset = M1A1.preset_manager_m1 != null;
                bool transmog = M1A1.m1_to_m1ip.Value;
                M1Preset preset = null;

                if (use_preset)
                {
                    PresetManager<M1Preset> preset_manager = M1A1.preset_manager_m1;
                    preset = preset_manager.ChoosePreset();
                    transmog = preset.ConvertToM1IP;
                }

                if (transmog)
                {
                    __state.TransmogNeeded = true;
                    __state.Preset = preset;
                    AssetUtilApi.LoadVanillaVehicle("M1", temp: true);
                    uniqueName = "M1IP";
                }
            }
        }

        private static void Postfix(TransmogState<M1Preset> __state, ref IUnit __result)
        {
            if (__state.TransmogNeeded)
            {
                PreviouslyM1 comp = __result.transform.gameObject.AddComponent<PreviouslyM1>();
                comp.enabled = false;

                if (__state.Preset != null)
                {
                    PresetMarker marker = __result.transform.gameObject.AddComponent<PresetMarker>();
                    marker.Preset = __state.Preset;
                }
            }
        }
    }
}
