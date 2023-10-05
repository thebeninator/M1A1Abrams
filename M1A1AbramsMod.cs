using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection; 
using MelonLoader;
using UnityEngine;
using GHPC.Weapons;
using GHPC.Vehicle;
using GHPC.Equipment;
using GHPC.Equipment.Optics;

namespace M1A1Abrams
{
    public class M1A1AbramsMod : MelonMod
    {
        MelonPreferences_Category cfg;
        MelonPreferences_Entry<int> m829Count;
        MelonPreferences_Entry<int> m830Count;

        GameObject[] vic_gos;

        WeaponSystemCodexScriptable gun_m256;

        AmmoClipCodexScriptable clip_codex_m829;
        AmmoType.AmmoClip clip_m829;
        AmmoCodexScriptable ammo_codex_m829;
        AmmoType ammo_m829;

        AmmoClipCodexScriptable clip_codex_m830;
        AmmoType.AmmoClip clip_m830;
        AmmoCodexScriptable ammo_codex_m830;
        AmmoType ammo_m830;

        AmmoType ammo_m833;
        AmmoType ammo_m456;

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

        public static void EmptyRack(GHPC.Weapons.AmmoRack rack)
        {
            MethodInfo removeVis = typeof(GHPC.Weapons.AmmoRack).GetMethod("RemoveAmmoVisualFromSlot", BindingFlags.Instance | BindingFlags.NonPublic);

            PropertyInfo stored_clips = typeof(GHPC.Weapons.AmmoRack).GetProperty("StoredClips");
            stored_clips.SetValue(rack, new List<AmmoType.AmmoClip>());

            rack.SlotIndicesByAmmoType = new Dictionary<AmmoType, List<byte>>();

            foreach (Transform transform in rack.VisualSlots) {
                AmmoStoredVisual vis = transform.GetComponentInChildren<AmmoStoredVisual>();

                if (vis != null && vis.AmmoType != null)
                {
                    removeVis.Invoke(rack, new object[] {transform});
                }
            }
        }

        public override void OnInitializeMelon() {
            cfg = MelonPreferences.CreateCategory("M1A1Config");
            m829Count = cfg.CreateEntry<int>("M829", 22);
            m829Count.Description = "How many rounds of M829 (APFSDS) or M830 (HEAT) each M1A1 should carry. Maximum of 40 rounds total. Bring in at least one M829 round.";
            m830Count = cfg.CreateEntry<int>("M830", 18);
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            // note to self: not all vehicles are tagged with "vehicle"
            //               probably will not convert retroactively added m1ips to m1a1s 

            if (sceneName == "LOADER_INITIAL" || sceneName == "MainMenu2_Scene") return;

            vic_gos = GameObject.FindGameObjectsWithTag("Vehicle");

            if (gun_m256 == null)
            {

                foreach (AmmoCodexScriptable s in Resources.FindObjectsOfTypeAll(typeof(AmmoCodexScriptable)))
                {
                    if (s.AmmoType.Name == "M833 APFSDS-T")
                    {
                        ammo_m833 = s.AmmoType;
                    }

                    if (s.AmmoType.Name == "M456 HEAT-FS-T")
                    {
                        ammo_m456 = s.AmmoType;
                    }
                }

                // m256
                gun_m256 = ScriptableObject.CreateInstance<WeaponSystemCodexScriptable>();
                gun_m256.name = "gun_m256";
                gun_m256.CaliberMm = 120;
                gun_m256.FriendlyName = "120mm Gun M256";
                gun_m256.Type = WeaponSystemCodexScriptable.WeaponType.LargeCannon;

                // m829 
                ammo_m829 = new AmmoType();
                ShallowCopy(ammo_m829, ammo_m833);
                ammo_m829.Name = "M829 APFSDS-T";
                ammo_m829.Caliber = 120;
                ammo_m829.RhaPenetration = 600;
                ammo_m829.MuzzleVelocity = 1670f;
                ammo_m829.Mass = 4.9f;

                ammo_codex_m829 = ScriptableObject.CreateInstance<AmmoCodexScriptable>();
                ammo_codex_m829.AmmoType = ammo_m829;
                ammo_codex_m829.name = "ammo_m829";

                clip_m829 = new AmmoType.AmmoClip();
                clip_m829.Capacity = 1;
                clip_m829.Name = "M829 APFSDS-T";
                clip_m829.MinimalPattern = new AmmoCodexScriptable[1];
                clip_m829.MinimalPattern[0] = ammo_codex_m829;

                clip_codex_m829 = ScriptableObject.CreateInstance<AmmoClipCodexScriptable>();
                clip_codex_m829.name = "clip_m829";
                clip_codex_m829.CompatibleWeaponSystems = new WeaponSystemCodexScriptable[1];
                clip_codex_m829.CompatibleWeaponSystems[0] = gun_m256;
                clip_codex_m829.ClipType = clip_m829;

                // m830
                ammo_m830 = new AmmoType();
                ShallowCopy(ammo_m830, ammo_m456);
                ammo_m830.Name = "M830 HEAT-MP-T";
                ammo_m830.Caliber = 120;
                ammo_m830.RhaPenetration = 480;
                ammo_m830.TntEquivalentKg = 1.814f;
                ammo_m830.MuzzleVelocity = 1140f;
                ammo_m830.Mass = 13.5f;
                ammo_m830.CertainRicochetAngle = 8.0f;

                ammo_codex_m830 = ScriptableObject.CreateInstance<AmmoCodexScriptable>();
                ammo_codex_m830.AmmoType = ammo_m830;
                ammo_codex_m830.name = "ammo_m830";

                clip_m830 = new AmmoType.AmmoClip();
                clip_m830.Capacity = 1;
                clip_m830.Name = "M830 HEAT-MP-T";
                clip_m830.MinimalPattern = new AmmoCodexScriptable[1];
                clip_m830.MinimalPattern[0] = ammo_codex_m830;

                clip_codex_m830 = ScriptableObject.CreateInstance<AmmoClipCodexScriptable>();
                clip_codex_m830.name = "clip_m830";
                clip_codex_m830.CompatibleWeaponSystems = new WeaponSystemCodexScriptable[1];
                clip_codex_m830.CompatibleWeaponSystems[0] = gun_m256;
                clip_codex_m830.ClipType = clip_m830;
            }

            foreach (GameObject vic_go in vic_gos) {
                Vehicle vic = vic_go.GetComponent<Vehicle>();

                if (vic == null) continue;

                if (vic.FriendlyName == "M1IP") {
                    // generate visual models 
                    GameObject ammo_m829_vis = null;
                    GameObject ammo_m830_vis = null;

                    if (ammo_m829_vis == null)
                    {
                        ammo_m829_vis = GameObject.Instantiate(ammo_m833.VisualModel);
                        ammo_m829_vis.name = "M829 visual";
                        ammo_m829.VisualModel = ammo_m829_vis;
                        ammo_m829.VisualModel.GetComponent<AmmoStoredVisual>().AmmoType = ammo_m829;
                        ammo_m829.VisualModel.GetComponent<AmmoStoredVisual>().AmmoScriptable = ammo_codex_m829;

                        ammo_m830_vis = GameObject.Instantiate(ammo_m456.VisualModel);
                        ammo_m830_vis.name = "M830 visual";
                        ammo_m830.VisualModel = ammo_m830_vis;
                        ammo_m830.VisualModel.GetComponent<AmmoStoredVisual>().AmmoType = ammo_m830;
                        ammo_m830.VisualModel.GetComponent<AmmoStoredVisual>().AmmoScriptable = ammo_codex_m830;
                    }

                    // rename to m1a1
                    FieldInfo friendlyName = typeof(GHPC.Unit).GetField("_friendlyName", BindingFlags.NonPublic | BindingFlags.Instance);
                    friendlyName.SetValue(vic, "M1A1");

                    FieldInfo uniqueName = typeof(GHPC.Unit).GetField("_uniqueName", BindingFlags.NonPublic | BindingFlags.Instance);
                    uniqueName.SetValue(vic, "M1A1");

                    // convert to m256 gun
                    WeaponsManager weaponsManager = vic.GetComponent<WeaponsManager>();
                    WeaponSystem mainGun = weaponsManager.Weapons[0].Weapon;

                    mainGun.Impulse = 68000;
                    FieldInfo codex = typeof(WeaponSystem).GetField("CodexEntry", BindingFlags.NonPublic | BindingFlags.Instance);
                    codex.SetValue(mainGun, gun_m256);

                    GameObject gunTube = vic_go.transform.Find("IPM1_rig/HULL/TURRET/GUN/gun_recoil").gameObject;
                    gunTube.transform.localScale = new Vector3(1.35f, 1.35f, 0.98f);

                    // convert ammo
                    LoadoutManager loadoutManager = vic.GetComponent<LoadoutManager>();

                    loadoutManager.TotalAmmoCounts = new int[] {m829Count.Value, m830Count.Value};
                    loadoutManager.LoadedAmmoTypes = new AmmoClipCodexScriptable[] {clip_codex_m829, clip_codex_m830};

                    FieldInfo totalAmmoCount = typeof(LoadoutManager).GetField("_totalAmmoCount", BindingFlags.NonPublic | BindingFlags.Instance);
                    totalAmmoCount.SetValue(loadoutManager, 40);

                    for (int i = 0; i <= 2; i++) { 
                        GHPC.Weapons.AmmoRack rack = loadoutManager.RackLoadouts[i].Rack;
                        rack.ClipCapacity = i == 2 ? 4 : 18;
                        rack.ClipTypes = new AmmoType.AmmoClip[] {clip_m829, clip_m830};
                        EmptyRack(rack);
                    }

                    loadoutManager.SpawnCurrentLoadout();

                    PropertyInfo roundInBreech = typeof(AmmoFeed).GetProperty("AmmoTypeInBreech"); // clear preloaded M833 from breech
                    roundInBreech.SetValue(mainGun.Feed, null);

                    MethodInfo refreshBreech = typeof(AmmoFeed).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic); // silently load M829
                    refreshBreech.Invoke(mainGun.Feed, new object[] {});

                    // update ballistic computer
                    MethodInfo registerAllBallistics = typeof(LoadoutManager).GetMethod("RegisterAllBallistics", BindingFlags.Instance | BindingFlags.NonPublic);
                    registerAllBallistics.Invoke(loadoutManager, new object[] {});

                }
            }
        }
    }
}
