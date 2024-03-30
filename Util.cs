using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GHPC.Weapons;
using GHPC.Equipment.Optics;
using UnityEngine;

namespace M1A1Abrams
{
    /*
    [HarmonyPatch(typeof(GHPC.Audio.FmodGenericAudioManager), "PlayOneShot")]
    public class pee
    {
        public static FMOD.Sound sound;

        public static void change_pitch(ref EventInstance e)
        {
            ChannelGroup channelGroup;
            e.getChannelGroup(out channelGroup);
            channelGroup.setVolumeRamp(true);
            channelGroup.set3DMinMaxDistance(1f, 10f);
            channelGroup.setMode(MODE._3D_WORLDRELATIVE);

            var corSystem = RuntimeManager.CoreSystem;
            sound.set3DMinMaxDistance(1f, 10f);

            FMOD.Channel channel;
            corSystem.playSound(sound, channelGroup, true, out channel);
            channel.set3DMinMaxDistance(1f, 10f);
            channel.setVolume(0.5f);
            channel.setVolumeRamp(true);

            VECTOR pos = new VECTOR();
            pos.x = 0f;
            pos.y = 0f;
            pos.z = 0f;

            VECTOR vel = new VECTOR();
            vel.x = 0f;
            vel.y = 0f;
            vel.z = 0f;

            channel.set3DAttributes(ref pos, ref vel);
            channelGroup.set3DAttributes(ref pos, ref vel);

            channel.setPaused(false);
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var instr = new List<CodeInstruction>(instructions);
            var create_instance = AccessTools.Method(typeof(RuntimeManager), nameof(RuntimeManager.CreateInstance), new Type[] { typeof(string) });

            int event_instance_idx = -1;
            int insert_idx = -1; 

            for (int i = 0; i < instr.Count; i++)
            {
                if (instr[i].opcode == OpCodes.Call && instr[i].operand == (object)create_instance) {
                    event_instance_idx = i;
                    insert_idx = i + 2;
                }
            }

            var custom_instr = new List<CodeInstruction>();
            custom_instr.Add(new CodeInstruction(OpCodes.Ldarga_S, 0));
            custom_instr.Add(new CodeInstruction(OpCodes.Ldloca_S, 0));
            custom_instr.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(pee), nameof(pee.change_pitch))));
            custom_instr.Add(new CodeInstruction(OpCodes.Pop));
            custom_instr.Add(new CodeInstruction(OpCodes.Ret));

            instr.InsertRange(insert_idx, custom_instr);

            return instr; 
        }
    }
    */

    public class Util
    {
        public static string[] menu_screens = new string[] {
            "MainMenu2_Scene",
            "MainMenu2-1_Scene",
            "LOADER_MENU",
            "LOADER_INITIAL",
            "t64_menu"
        };
        public class AlreadyConverted : MonoBehaviour {}

        // https://snipplr.com/view/75285/clone-from-one-object-to-another-using-reflection
        public static void ShallowCopy(System.Object dest, System.Object src)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            FieldInfo[] destFields = dest.GetType().GetFields(flags);
            FieldInfo[] srcFields = src.GetType().GetFields(flags);

            foreach (FieldInfo srcField in srcFields)
            {
                FieldInfo destField = destFields.FirstOrDefault(field => field.Name == srcField.Name);

                if (destField != null && !destField.IsLiteral)
                {
                    if (srcField.FieldType == destField.FieldType)
                        destField.SetValue(dest, srcField.GetValue(src));
                }
            }
        }

        public static UsableOptic GetDayOptic(FireControlSystem fcs)
        {
            if (fcs.MainOptic.slot.IsLinkedNightSight)
            {
                return fcs.MainOptic.slot.LinkedDaySight.PairedOptic;
            }
            else
            {
                return fcs.MainOptic;
            }

        }
        public static void EmptyRack(GHPC.Weapons.AmmoRack rack)
        {
            MethodInfo removeVis = typeof(GHPC.Weapons.AmmoRack).GetMethod("RemoveAmmoVisualFromSlot", BindingFlags.Instance | BindingFlags.NonPublic);

            PropertyInfo stored_clips = typeof(GHPC.Weapons.AmmoRack).GetProperty("StoredClips");
            stored_clips.SetValue(rack, new List<AmmoType.AmmoClip>());

            rack.SlotIndicesByAmmoType = new Dictionary<AmmoType, List<byte>>();

            foreach (Transform transform in rack.VisualSlots)
            {
                AmmoStoredVisual vis = transform.GetComponentInChildren<AmmoStoredVisual>();

                if (vis != null && vis.AmmoType != null)
                {
                    removeVis.Invoke(rack, new object[] { transform });
                }
            }
        }
    }
}
