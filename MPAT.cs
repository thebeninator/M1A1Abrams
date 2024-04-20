using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using GHPC.Weapons;
using HarmonyLib;
using MelonLoader;
using M1A1Abrams;
using static MelonLoader.MelonLogger;
using GHPC.Camera;
using GHPC.Equipment.Optics;
using GHPC.Player;

namespace M1A1Abrams
{
    public class MPAT_Switch : MonoBehaviour
    {
        public bool activated = false;
        private float cd = 0f;
        private WeaponSystem weapon = null;
        private PlayerInput player_manager;

        void Awake()
        {
            weapon = GetComponent<WeaponsManager>().Weapons[0].Weapon;
            player_manager = GameObject.Find("_APP_GHPC_").GetComponent<PlayerInput>();
        }

        void Update()
        {
            cd -= Time.deltaTime;

            if (player_manager.CurrentPlayerUnit.gameObject.GetInstanceID() != gameObject.GetInstanceID()) return;

            M1A1.clip_m830a1.Name = activated ? "M830A1 MPAT-T [Proximity]" : "M830A1 MPAT-T";

            if (Input.GetKey(KeyCode.Mouse2) && cd <= 0f && weapon.CurrentAmmoType == M1A1.ammo_m830a1)
            {
                cd = 0.2f;

                activated = !activated;
            }
        }
    }

    public class MPAT : MonoBehaviour
    {
        private GHPC.Weapons.LiveRound live_round;
        private static GameObject prox_fuse;
        private static HashSet<string> prox_ammos = new HashSet<string>();
        private bool detonated = false;

        // must be called at least once 
        public static void Init()
        {
            if (prox_fuse) return;
            prox_fuse = new GameObject("mpat prox fuse");
            prox_fuse.layer = 8;
            prox_fuse.SetActive(false);
            prox_fuse.AddComponent<MPAT>();
        }

        public static void AddMPATFuse(AmmoType ammo_type)
        {
            if (prox_ammos.Contains(ammo_type.Name)) return;
            prox_ammos.Add(ammo_type.Name);
        }

        void Detonate()
        {
            if (detonated) return;
            live_round._rangedFuseActive = true;
            live_round._rangedFuseCountdown = 0f;
            detonated = true;
        }

        void Update()
        {
            if (!live_round) return;

            RaycastHit hit;
            Vector3 pos = live_round.transform.position;

            if (Physics.Raycast(pos, live_round.transform.forward, out hit, 10f, 1 << 8))
                if (hit.collider.CompareTag("Penetrable"))
                    Detonate();

            RaycastHit hit2;
            if (Physics.Raycast(pos, Vector3.down, out hit2, 30f, 1 << 8))
                if (hit2.collider.CompareTag("Penetrable"))
                    Detonate();

            RaycastHit hit3;
            if (Physics.SphereCast(pos, 3f, live_round.transform.forward, out hit3, 0.1f, 1 << 8))
                if (hit3.collider.CompareTag("Penetrable"))
                    Detonate();

        }

        [HarmonyPatch(typeof(GHPC.Weapons.LiveRound), "Start")]
        public static class SpawnProximityFuse
        {
            private static void Prefix(GHPC.Weapons.LiveRound __instance)
            {
                if (prox_ammos.Contains(__instance.Info.Name) && __instance.gameObject.transform.Find("mpat prox fuse(Clone)") == null)
                {
                    GameObject p = GameObject.Instantiate(prox_fuse, __instance.transform);
                    p.GetComponent<MPAT>().live_round = __instance;
                    p.SetActive(__instance.Shooter.gameObject.GetComponent<MPAT_Switch>().activated);
                }
                else if (__instance.gameObject.transform.Find("mpat prox fuse(Clone)"))
                {
                    GameObject.DestroyImmediate(__instance.gameObject.transform.Find("mpat prox fuse(Clone)").gameObject);
                }
            }
        }

        [HarmonyPatch(typeof(GHPC.Weapons.LiveRound), "createExplosion")]
        public class ForwardBurst
        {
            private static bool Prefix(GHPC.Weapons.LiveRound __instance)
            {
                if (__instance.Info.Name != "M830A1 MPAT-T") return true;
                if (!__instance.gameObject.GetComponentInChildren<MPAT>()) return true;
                if (!__instance.gameObject.GetComponentInChildren<MPAT>().detonated) return true;

                for (int i = 0; i < 25; i++)
                {
                    GHPC.Weapons.LiveRound component;
                    component = LiveRoundMarshaller.Instance.GetRoundOfVisualType(LiveRoundMarshaller.LiveRoundVisualType.Spall)
                        .GetComponent<GHPC.Weapons.LiveRound>();
                    component.Info = M1A1.m830a1_forward_frag;
                    component.CurrentSpeed = 600f;
                    component.MaxSpeed = 600f;
                    component.IsSpall = false;
                    component.Shooter = __instance.Shooter;
                    component.transform.position = __instance.transform.position;
                    component.transform.forward = Quaternion.Euler(
                        UnityEngine.Random.Range(-5f, 5f),
                        UnityEngine.Random.Range(-5f, 5f),
                        UnityEngine.Random.Range(-5f, 5f)) * __instance.transform.forward;
                    component.Init(__instance, null);
                    component.name = "mpat forward frag " + i;
                }

                return true;
            }
        }
    }
}