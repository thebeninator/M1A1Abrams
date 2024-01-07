using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GHPC.Camera;
using GHPC.Player;
using MelonLoader;
using UnityEngine;
using GHPC.State;
using System.Collections;
using GHPC.Weapons;
using GHPC.Equipment.Optics;
using GHPC.Vehicle;
using System.Reflection;
using Reticle;

namespace M1A1Abrams
{
    public static class M1A1
    {
        static MelonPreferences_Entry<int> m829Count;
        static MelonPreferences_Entry<int> m830Count;
        static MelonPreferences_Entry<bool> rotateAzimuth;
        static MelonPreferences_Entry<bool> m1e1;
        static MelonPreferences_Entry<int> randomChanceNum;
        static MelonPreferences_Entry<bool> randomChance;
        static MelonPreferences_Entry<bool> useSuperSabot;

        static WeaponSystemCodexScriptable gun_m256;

        static AmmoClipCodexScriptable clip_codex_m829;
        static AmmoType.AmmoClip clip_m829;
        static AmmoCodexScriptable ammo_codex_m829;
        static AmmoType ammo_m829;

        static AmmoClipCodexScriptable clip_codex_m829a1;
        static AmmoType.AmmoClip clip_m829a1;
        static AmmoCodexScriptable ammo_codex_m829a1;
        static AmmoType ammo_m829a1;

        static AmmoClipCodexScriptable clip_codex_m830;
        static AmmoType.AmmoClip clip_m830;
        static AmmoCodexScriptable ammo_codex_m830;
        static AmmoType ammo_m830;

        static AmmoType ammo_m833;
        static AmmoType ammo_m456;

        static GameObject ammo_m829_vis = null;
        static GameObject ammo_m829a1_vis = null;
        static GameObject ammo_m830_vis = null;

        // gas
        static ReticleSO reticleSO_heat;
        static ReticleMesh.CachedReticle reticle_cached_heat;

        static ReticleSO reticleSO_ap;
        static ReticleMesh.CachedReticle reticle_cached_ap;

        public static void Config(MelonPreferences_Category cfg) {
            m829Count = cfg.CreateEntry<int>("M829", 22);
            m829Count.Description = "How many rounds of M829 (APFSDS) or M830 (HEAT) each M1A1 should carry. Maximum of 40 rounds total. Bring in at least one M829 round.";
            m830Count = cfg.CreateEntry<int>("M830", 18);

            useSuperSabot = cfg.CreateEntry<bool>("UseM829A1", false);
            useSuperSabot.Description = "In case 600mm of RHA penetration is not working out for you...";

            rotateAzimuth = cfg.CreateEntry<bool>("RotateAzimuth", false);
            rotateAzimuth.Description = "Horizontal stabilization of M1A1 sights when applying lead.";

            m1e1 = cfg.CreateEntry<bool>("M1E1", false);
            m1e1.Description = "Convert M1s to M1E1s (i.e: they get the 120mm gun).";

            randomChance = cfg.CreateEntry<bool>("Random", false);
            randomChance.Description = "M1IPs/M1s will have a random chance of being converted to M1A1s/M1E1s.";
            randomChanceNum = cfg.CreateEntry<int>("ConversionChance", 50);
        }
        public static IEnumerator Convert(GameState _) {
            foreach (GameObject vic_go in M1A1AbramsMod.vic_gos)
            {
                Vehicle vic = vic_go.GetComponent<Vehicle>();

                if (vic == null) continue;

                if (vic.FriendlyName == "M1IP" || (m1e1.Value && vic.FriendlyName == "M1"))
                {
                    int rand = (randomChance.Value) ? UnityEngine.Random.Range(1, 100) : 0;

                    if (rand <= randomChanceNum.Value)
                    {
                        vic._friendlyName = (vic.FriendlyName == "M1IP") ? "M1A1" : "M1E1";

                        WeaponsManager weaponsManager = vic.GetComponent<WeaponsManager>();
                        WeaponSystemInfo mainGunInfo = weaponsManager.Weapons[0];
                        WeaponSystem mainGun = mainGunInfo.Weapon;

                        if (rotateAzimuth.Value)
                        {
                            UsableOptic optic = Util.GetDayOptic(mainGun.FCS);
                            optic.RotateAzimuth = true;
                            optic.slot.LinkedNightSight.PairedOptic.RotateAzimuth = true;
                        }

                        if (reticleSO_ap == null) {
                            reticleSO_ap = ScriptableObject.Instantiate(ReticleMesh.cachedReticles["M1_105_GAS_APFSDS"].tree);
                            reticleSO_ap.name = "120mm_gas_ap";

                            Util.ShallowCopy(reticle_cached_ap, ReticleMesh.cachedReticles["M1_105_GAS_APFSDS"]);
                            reticle_cached_ap.tree = reticleSO_ap;

                            ReticleTree.VerticalBallistic reticle_range_ap = (reticleSO_ap.planes[0]
                                as ReticleTree.FocalPlane).elements[1]
                                as ReticleTree.VerticalBallistic;
                            reticle_range_ap.projectile = (useSuperSabot.Value) ? ammo_codex_m829a1: ammo_codex_m829;
                            reticle_range_ap.UpdateBC();

                            ReticleTree.Text reticle_text_ap = (((reticleSO_ap.planes[0] 
                                as ReticleTree.FocalPlane).elements[0])
                                as ReticleTree.Angular).elements[0]
                                as ReticleTree.Text;

                            string ap_name = (useSuperSabot.Value) ? "M829A1" : "M829";
                            reticle_text_ap.text = "120-MM\nAPFSDS-T " + ap_name + "\nMETERS";

                            reticleSO_heat = ScriptableObject.Instantiate(ReticleMesh.cachedReticles["M1_105_GAS_HEAT"].tree);
                            reticleSO_heat.name = "120mm_gas_heat";

                            Util.ShallowCopy(reticle_cached_heat, ReticleMesh.cachedReticles["M1_105_GAS_HEAT"]);
                            reticle_cached_heat.tree = reticleSO_heat;

                            ReticleTree.VerticalBallistic reticle_range_heat = (reticleSO_heat.planes[0]
                                as ReticleTree.FocalPlane).elements[1]
                                as ReticleTree.VerticalBallistic;
                            reticle_range_heat.projectile = ammo_codex_m830;
                            reticle_range_heat.UpdateBC();

                            ReticleTree.Text reticle_text_heat = (((reticleSO_heat.planes[0]
                                as ReticleTree.FocalPlane).elements[0])
                                as ReticleTree.Angular).elements[0]
                                as ReticleTree.Text;

                            reticle_text_heat.text = "120-MM\nHEAT-MP-T M830\nMETERS";
                        }
          
                        ReticleMesh gas_ap = vic.transform.Find("IPM1_rig/HULL/TURRET/GUN/Gun Scripts/Aux sight (GAS)/Reticle Mesh").gameObject.GetComponent<ReticleMesh>();
                        gas_ap.reticleSO = reticleSO_ap;
                        gas_ap.reticle = reticle_cached_ap;
                        gas_ap.SMR = null;
                        gas_ap.Load();

                        ReticleMesh gas_heat = vic.transform.Find("IPM1_rig/HULL/TURRET/GUN/Gun Scripts/Aux sight (GAS)/Reticle Mesh HEAT").gameObject.GetComponent<ReticleMesh>();
                        gas_heat.reticleSO = reticleSO_heat;
                        gas_heat.reticle = reticle_cached_heat;
                        gas_heat.SMR = null;
                        gas_heat.Load();

                        Transform muzzleFlashes = mainGun.MuzzleEffects[1].transform;
                        muzzleFlashes.GetChild(1).transform.localScale = new Vector3(2f, 2f, 2f);
                        muzzleFlashes.GetChild(2).transform.localScale = new Vector3(2f, 2f, 2f);
                        muzzleFlashes.GetChild(4).transform.localScale = new Vector3(2f, 2f, 2f);

                        mainGunInfo.Name = "120mm gun M256";
                        mainGun.Impulse = 68000;
                        mainGun.CodexEntry = gun_m256;

                        GameObject gunTube = vic_go.transform.Find("IPM1_rig/HULL/TURRET/GUN/gun_recoil").gameObject;
                        gunTube.transform.localScale = new Vector3(1.4f, 1.4f, 0.98f);

                        // convert ammo
                        AmmoClipCodexScriptable sabotClipCodex = (useSuperSabot.Value) ? clip_codex_m829a1 : clip_codex_m829;
                        LoadoutManager loadoutManager = vic.GetComponent<LoadoutManager>();
                        loadoutManager.TotalAmmoCounts = new int[] { m829Count.Value, m830Count.Value };
                        loadoutManager.LoadedAmmoTypes = new AmmoClipCodexScriptable[] { sabotClipCodex, clip_codex_m830 };
                        loadoutManager._totalAmmoCount = 40;

                        for (int i = 0; i <= 2; i++)
                        {
                            GHPC.Weapons.AmmoRack rack = loadoutManager.RackLoadouts[i].Rack;
                            rack.ClipCapacity = i == 2 ? 4 : 18;
                            rack.ClipTypes = new AmmoType.AmmoClip[] { sabotClipCodex.ClipType, clip_m830 };
                            Util.EmptyRack(rack);
                        }

                        loadoutManager.SpawnCurrentLoadout();
                        mainGun.Feed.AmmoTypeInBreech = null;
                        mainGun.Feed.Start();
                        loadoutManager.RegisterAllBallistics();
                    }
                }
            }

            yield break; 
        }

        public static void LateUpdate() {
            if (M1A1AbramsMod.gameManager == null) return;

            CameraSlot cam = M1A1AbramsMod.camManager._currentCamSlot;

            if (cam == null) return;
            if (cam.name != "Aux sight (GAS)") return;
            if (M1A1AbramsMod.playerManager.CurrentPlayerWeapon.Name != "120mm gun M256") return;

            AmmoType currentAmmo = M1A1AbramsMod.playerManager.CurrentPlayerWeapon.FCS.CurrentAmmoType;
            int reticleId = (currentAmmo.Name == "M829 APFSDS-T" || currentAmmo.Name == "M829A1 APFSDS-T") ? 0 : 2;

            GameObject reticle = cam.transform.GetChild(reticleId).gameObject;

            if (!reticle.activeSelf)
            {
                reticle.SetActive(true);
            }
        }

        public static void Init() {
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
                Util.ShallowCopy(ammo_m829, ammo_m833);
                ammo_m829.Name = "M829 APFSDS-T";
                ammo_m829.Caliber = 120;
                ammo_m829.RhaPenetration = 600;
                ammo_m829.MuzzleVelocity = 1670f;
                ammo_m829.Mass = 3.9f;

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

                // m829a1
                ammo_m829a1 = new AmmoType();
                Util.ShallowCopy(ammo_m829a1, ammo_m833);
                ammo_m829a1.Name = "M829A1 APFSDS-T";
                ammo_m829a1.Caliber = 120;
                ammo_m829a1.RhaPenetration = 700;
                ammo_m829a1.MuzzleVelocity = 1575;
                ammo_m829a1.Mass = 4.6f;

                ammo_codex_m829a1 = ScriptableObject.CreateInstance<AmmoCodexScriptable>();
                ammo_codex_m829a1.AmmoType = ammo_m829a1;
                ammo_codex_m829a1.name = "ammo_m829a1";

                clip_m829a1 = new AmmoType.AmmoClip();
                clip_m829a1.Capacity = 1;
                clip_m829a1.Name = "M829A1 APFSDS-T";
                clip_m829a1.MinimalPattern = new AmmoCodexScriptable[1];
                clip_m829a1.MinimalPattern[0] = ammo_codex_m829a1;

                clip_codex_m829a1 = ScriptableObject.CreateInstance<AmmoClipCodexScriptable>();
                clip_codex_m829a1.name = "clip_m829a1";
                clip_codex_m829a1.CompatibleWeaponSystems = new WeaponSystemCodexScriptable[1];
                clip_codex_m829a1.CompatibleWeaponSystems[0] = gun_m256;
                clip_codex_m829a1.ClipType = clip_m829a1;

                // m830
                ammo_m830 = new AmmoType();
                Util.ShallowCopy(ammo_m830, ammo_m456);
                ammo_m830.Name = "M830 HEAT-MP-T";
                ammo_m830.Caliber = 120;
                ammo_m830.RhaPenetration = 480;
                ammo_m830.TntEquivalentKg = 1.814f;
                ammo_m830.MuzzleVelocity = 1140f;
                ammo_m830.Mass = 13.5f;
                ammo_m830.CertainRicochetAngle = 8.0f;
                ammo_m830.ShatterOnRicochet = false;

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

                ammo_m829_vis = GameObject.Instantiate(ammo_m833.VisualModel);
                ammo_m829_vis.name = "M829 visual";
                ammo_m829.VisualModel = ammo_m829_vis;
                ammo_m829.VisualModel.GetComponent<AmmoStoredVisual>().AmmoType = ammo_m829;
                ammo_m829.VisualModel.GetComponent<AmmoStoredVisual>().AmmoScriptable = ammo_codex_m829;

                ammo_m829a1_vis = GameObject.Instantiate(ammo_m833.VisualModel);
                ammo_m829a1_vis.name = "M829A1 visual";
                ammo_m829a1.VisualModel = ammo_m829a1_vis;
                ammo_m829a1.VisualModel.GetComponent<AmmoStoredVisual>().AmmoType = ammo_m829a1;
                ammo_m829a1.VisualModel.GetComponent<AmmoStoredVisual>().AmmoScriptable = ammo_codex_m829a1;

                ammo_m830_vis = GameObject.Instantiate(ammo_m456.VisualModel);
                ammo_m830_vis.name = "M830 visual";
                ammo_m830.VisualModel = ammo_m830_vis;
                ammo_m830.VisualModel.GetComponent<AmmoStoredVisual>().AmmoType = ammo_m830;
                ammo_m830.VisualModel.GetComponent<AmmoStoredVisual>().AmmoScriptable = ammo_codex_m830;
            }

            StateController.RunOrDefer(GameState.GameReady, new GameStateEventHandler(Convert), GameStatePriority.Lowest);
        }
    }
}
