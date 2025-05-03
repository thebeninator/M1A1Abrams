using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GHPC;
using GHPC.Utility;
using GHPC.Vehicle;
using MelonLoader.Utils;
using UnityEngine;
using GHPC.Weapons;
using HarmonyLib;
using GHPC.Equipment.Optics;
using MelonLoader;
using Reticle;
using static UnityEngine.Rendering.PostProcessing.SubpixelMorphologicalAntialiasing;
using UnityEngine.UI;
using Thermals;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.Rendering.PostProcessing;
using GHPC.Effects;

namespace M1A1Abrams
{
    public class CROWS
    {
        static GameObject crows;
        static GameObject m2_browning;
        static GameObject gun_sight;
        static GameObject fcs_go;

        static AmmoCodexScriptable ammo_codex_m8;
        static AmmoCodexScriptable ammo_codex_50tracer;

        static AmmoClipCodexScriptable clip_codex_50cal_400rnd;
        static AmmoType.AmmoClip clip_50cal_400rnd;

        static AmmoType ammo_raufoss;
        static AmmoType ammo_raufoss_tracer;
        static AmmoCodexScriptable ammo_codex_raufoss;
        static AmmoCodexScriptable ammo_codex_raufoss_tracer;
        static AmmoClipCodexScriptable clip_codex_raufoss_400rnd;
        static AmmoType.AmmoClip clip_raufoss_400rnd;

        static GameObject box_canvas;
        static ReticleSO reticleSO_hq;
        static ReticleMesh.CachedReticle reticle_cached_hq;

        static List<List<Vector3>> borders = new List<List<Vector3>>() {
            new List<Vector3> {new Vector3(0f, -410f, 0f), new Vector3(0f, 0f, 180f)},
            new List<Vector3> {new Vector3(0f, 410f, 0f), new Vector3(0f, 0f, 0f)},
            new List<Vector3> {new Vector3(480f, 0f, 0f), new Vector3(0f, 0f, 270f)},
            new List<Vector3> {new Vector3(-480f, 0f, 0f), new Vector3(0f, 0f, 90f)}
        };


        public class StaticWhenNotInUse : MonoBehaviour {
            public FireControlSystem fcs;

            void Update() { 
                fcs.SetStabsActive(fcs.GetInstanceID() == M1A1AbramsMod.playerManager.CurrentPlayerWeapon.FCS.GetInstanceID());
            }
        }

        public static void Add(Vehicle vic, Transform origin, Vector3 position)
        {
            GameObject _crows = GameObject.Instantiate(crows, origin);

            _crows.transform.localPosition = position;

            GameObject mount = _crows.transform.Find("MOUNT").gameObject;
            mount.transform.localEulerAngles = Vector3.zero;
            GameObject gun = _crows.transform.Find("GUN").gameObject;

            GameObject gun_vis = gun.transform.Find("GUN VIS").gameObject;
            GameObject mount_vis = mount.transform.Find("MOUNT VIS").gameObject;

            GameObject gun_col = gun.transform.Find("GUN COLLIDER").gameObject;
            GameObject mount_col = mount.transform.Find("MOUNT COLLIDER").gameObject;
            gun_col.AddComponent<Reparent>();
            mount_col.AddComponent<Reparent>();

            UniformArmor gun_armour = gun_col.AddComponent<UniformArmor>();
            gun_armour.PrimaryHeatRha = 5f;
            gun_armour.PrimarySabotRha = 5f;
            gun_armour.SetName("M2HB machine gun");
            gun_armour.Unit = vic;

            UniformArmor mount_armour = mount_col.AddComponent<UniformArmor>();
            mount_armour.PrimaryHeatRha = 15f;
            mount_armour.PrimarySabotRha = 15f;
            mount_armour.SetName("CROWS");
            mount_armour.Unit = vic;

            LateFollowTarget lft_mount = mount_vis.AddComponent<LateFollowTarget>();
            LateFollowTarget lft_gun = gun_vis.AddComponent<LateFollowTarget>();

            LateFollow lf_mount = mount_col.AddComponent<LateFollow>();
            lf_mount.FollowTarget = mount_vis.transform;
            lf_mount._parentUnit = vic;
            lf_mount.enabled = true;
            lf_mount.Awake();

            LateFollow lf_gun = gun_col.AddComponent<LateFollow>();
            lf_gun.FollowTarget = gun_vis.transform;
            lf_gun._parentUnit = vic;
            lf_gun.enabled = true;
            lf_gun.Awake();

            AimablePlatform aimable_mount = mount.AddComponent<AimablePlatform>();
            aimable_mount.Axis = GHPC.Utility.EnumHelpers.Axis3.Y;
            aimable_mount.Transform = _crows.transform;
            aimable_mount._unit = vic;
            aimable_mount._equipmentManager = vic._equipmentManager;
            aimable_mount._stabActive = true;
            aimable_mount._stabMode = StabilizationMode.Vector;
            aimable_mount.ProtectFromBadProjection = true;
            aimable_mount.SpeedPowered = 40f;
            aimable_mount.enabled = true;

            GameObject gun_scripts = mount.transform.Find("GUN SCRIPTS").gameObject;
            AimablePlatform aimable_gun = gun_scripts.AddComponent<AimablePlatform>();
            aimable_gun.Axis = GHPC.Utility.EnumHelpers.Axis3.X;
            aimable_gun.Transform = gun.transform;
            aimable_gun._unit = vic;
            aimable_gun._equipmentManager = vic._equipmentManager;
            aimable_gun._stabActive = true;
            aimable_gun._stabMode = StabilizationMode.Vector;
            aimable_gun.enabled = true;
            aimable_gun.ProtectFromBadProjection = true;
            aimable_gun.ReverseLocalEuler = true;
            aimable_gun.LocalEulerLimits = new Vector2(-15f, 60f);
            gun.transform.localEulerAngles = Vector3.zero;

            GameObject _fcs_go = GameObject.Instantiate(fcs_go, mount.transform);
            GameObject _m2_browning = GameObject.Instantiate(m2_browning, gun.transform);
            _m2_browning.transform.SetParent(gun.transform);
            _m2_browning.transform.localPosition = new Vector3(0f, 0f, 1.201f);

            GameObject _gun_sight = GameObject.Instantiate(gun_sight, gun.transform);
            _gun_sight.transform.SetParent(gun.transform);
            _gun_sight.transform.localPosition = new Vector3(0f, -0.085f, 0.85f);

            GameObject ammo_rack_go = new GameObject("crows ammo rack");
            ammo_rack_go.transform.parent = gun.transform;
            GHPC.Weapons.AmmoRack ammo_rack = ammo_rack_go.AddComponent<GHPC.Weapons.AmmoRack>();

            AmmoType.AmmoClip clip = M1A1.crows_raufoss.Value ? clip_raufoss_400rnd : clip_50cal_400rnd;

            ammo_rack.ClipTypes = new AmmoType.AmmoClip[] { clip };
            ammo_rack._initialClipCounts = new int[] { 2 };
            ammo_rack.Awake();

            WeaponSystem wsys_m2_browning = _m2_browning.GetComponent<WeaponSystem>();
            wsys_m2_browning.Feed.ReadyRack = ammo_rack;
            wsys_m2_browning._cycleTimeSeconds = 0.1091f;
            wsys_m2_browning.Feed._totalCycleTime = 0.1091f;
            
            wsys_m2_browning.WeaponSound.LoopEventPath = "event:/Weapons/MG_m2_550rmp";
            wsys_m2_browning.WeaponSound.Awake();
            
            GameObject lase = new GameObject("lase");
            lase.transform.parent = gun.transform;
            lase.transform.localPosition = Vector3.zero;
            lase.transform.localEulerAngles = Vector3.zero;

            FireControlSystem fcs = _fcs_go.GetComponent<FireControlSystem>();
            fcs.Mounts = new AimablePlatform[] { aimable_mount, aimable_gun };
            fcs.MainOptic = _gun_sight.GetComponent<UsableOptic>();
            fcs.MainOptic.FCS = fcs;
            fcs.MainOptic.slot._pairedOptic = fcs.MainOptic;
            fcs.MainOptic.slot.ExclusiveWeapons = new WeaponSystem[] { wsys_m2_browning };
            fcs.MainOptic.slot.VibrationShakeMultiplier = 0f;
            fcs.MainOptic.slot.VibrationBlurScale = 0f;
            fcs.MainOptic.slot.DefaultFov = 6.5f;
            fcs.MainOptic.slot.OtherFovs = new float[] { 6f, 4.5f, 4f, 3.5f };
            fcs.MainOptic.slot._reparentTarget = gun.transform;
            fcs.MainOptic.Alignment = OpticAlignment.BoresightStabilized;
            fcs.MainOptic.RotateAzimuth = true;
            fcs.MainOptic.RotateElevation = true;
            fcs.MainOptic.LocalElevationLimits = new Vector2(-15f, 60f);
            fcs.LaserOrigin = lase.transform;
            fcs.LaserAim = LaserAimMode.ImpactPoint;
            fcs.MaxLaserRange = 4000f;
            fcs.LinkedWeaponSystems = new WeaponSystem[] { wsys_m2_browning };
            fcs.SuperelevateWeapon = true;
            fcs.StabsActive = true;
            fcs.CurrentStabMode = StabilizationMode.Vector;
            fcs.SuperleadWeapon = true;
            fcs.RecordTraverseRateBuffer = true;
            fcs.TraverseBufferSeconds = 0.1f;
            fcs.DynamicLead = true;
            fcs._autoDumpViaPalmSwitches = true;
            wsys_m2_browning.FCS = fcs;

            fcs.MainOptic.reticleMesh.reticleSO = reticleSO_hq;
            fcs.MainOptic.reticleMesh.reticle = reticle_cached_hq;
            fcs.MainOptic.reticleMesh.SMR = null;
            fcs.MainOptic.reticleMesh.Load();

            for (int i = 0; i <= 3; i++)
            {
                GameObject t = GameObject.Instantiate(box_canvas, fcs.MainOptic.transform);
                t.transform.GetChild(0).localPosition = borders[i][0];
                t.transform.GetChild(0).localEulerAngles = borders[i][1];
                if (i == 2 || i == 3)
                    t.GetComponent<CanvasScaler>().screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;
                t.SetActive(true);
            }

            GameObject gun_sight_flir = GameObject.Instantiate(_gun_sight);
            UsableOptic flir_optic = gun_sight_flir.GetComponent<UsableOptic>();
            gun_sight_flir.transform.localScale = new Vector3(1f, 1f, 1f);
            gun_sight_flir.transform.localPosition = new Vector3(0f, -0.085f, 0.85f);
            flir_optic.slot.DefaultFov = 6f;
            flir_optic.slot.OtherFovs = new float[] { 3f };
            flir_optic.slot.VisionType = NightVisionType.Thermal;
            flir_optic.slot.IsLinkedNightSight = true;
            flir_optic.slot.LinkedDaySight = fcs.MainOptic.slot;
            flir_optic.slot.BaseBlur = 0f;

            fcs.MainOptic.slot.LinkedNightSight = flir_optic.slot;
            fcs.NightOptic = flir_optic;

            WeaponSystemInfo wsi_m2_browning = new WeaponSystemInfo();
            wsi_m2_browning.Name = ".50 caliber machine gun M2HB";
            wsi_m2_browning.FCS = fcs; 
            wsi_m2_browning.Weapon = wsys_m2_browning;
            wsi_m2_browning.Role = WeaponSystemRole.RoofGun;
            wsi_m2_browning.PreAimWeapon = WeaponSystemRole.MainGun;
                         
            (vic.InfoBroker._crewManager.GetCrewBrain(GHPC.Crew.CrewPosition.Gunner) as GHPC.Crew.GunnerBrain).Weapons.Add(wsi_m2_browning);
            vic._weaponsManager.Weapons = Util.AppendToArray(vic._weaponsManager.Weapons, wsi_m2_browning);
            vic._designatedCameraSlots = Util.AppendToArray(vic._designatedCameraSlots, fcs.MainOptic.slot);
            vic.AimablePlatforms = Util.AppendToArray(vic.AimablePlatforms, aimable_mount);
            vic.AimablePlatforms = Util.AppendToArray(vic.AimablePlatforms, aimable_gun);

            if (vic.GetInstanceID() == M1A1AbramsMod.playerManager.CurrentPlayerUnit.GetComponent<Vehicle>().GetInstanceID())
                M1A1AbramsMod.camManager.RescanCamSlots(vic.DesignatedCameraSlots);


            StaticWhenNotInUse fix = fcs.gameObject.AddComponent<StaticWhenNotInUse>();
            fix.fcs = fcs;

        }

        public static void Reticle() {
            if (reticleSO_hq != null) return;

            reticleSO_hq = ScriptableObject.Instantiate(ReticleMesh.cachedReticles["T55-NVS"].tree);
            reticleSO_hq.name = "CROWS-RETICLE";

            Util.ShallowCopy(reticle_cached_hq, ReticleMesh.cachedReticles["T55-NVS"]);
            reticle_cached_hq.tree = reticleSO_hq;

            reticle_cached_hq.tree.lights = new List<ReticleTree.Light>() {
                new ReticleTree.Light()
            };

            reticle_cached_hq.tree.lights[0].type = ReticleTree.Light.Type.Powered;
            reticle_cached_hq.tree.lights[0].color = new RGB(3f, 0f, 0f, true);

            ReticleTree.Angular reticle_hq = (reticleSO_hq.planes[0].elements[0] as ReticleTree.Angular).elements[0] as ReticleTree.Angular;
            (reticleSO_hq.planes[0].elements[0] as ReticleTree.Angular).align = ReticleTree.GroupBase.Alignment.Impact;
            reticle_hq.align = ReticleTree.GroupBase.Alignment.Impact;
            reticle_cached_hq.mesh = null;

            reticle_hq.elements.RemoveAt(4);
            reticle_hq.elements.RemoveAt(1);
            reticle_hq.elements.RemoveAt(0);

            ReticleTree.Line line1 = reticle_hq.elements[0] as ReticleTree.Line;
            line1.rotation.mrad = 0;
            line1.position.x = 0;
            line1.position.y = 0;
            line1.length.mrad = 13.0944f;
            line1.thickness.mrad /= 1.7f;
            line1.illumination = ReticleTree.Light.Type.Powered;
            line1.visualType = ReticleTree.VisualElement.Type.Painted;

            ReticleTree.Line line2 = reticle_hq.elements[1] as ReticleTree.Line;
            line2.position.y = 0;
            line2.length.mrad = 13.0944f;
            line2.thickness.mrad /= 1.7f;
            line2.illumination = ReticleTree.Light.Type.Powered;
            line2.visualType = ReticleTree.VisualElement.Type.Painted;

            List<Vector3> box_pos = new List<Vector3>() {
                new Vector3(0, -7.344f),
                new Vector3(0, 7.344f),
                new Vector3(7.344f, 0),
                new Vector3(-7.344f,0),
            };

            foreach (Vector3 pos in box_pos)
            {
                ReticleTree.Line box = new ReticleTree.Line();
                box.roundness = 0f;
                box.thickness.mrad = line2.thickness.mrad * 2.8f;
                box.length.mrad = 6f;
                box.thickness.unit = AngularLength.AngularUnit.MIL_USSR;
                box.length.unit = AngularLength.AngularUnit.MIL_USSR;
                box.rotation.mrad = pos.x == 0 ? line2.rotation.mrad : line1.rotation.mrad;

                box.position = new ReticleTree.Position(pos.x, pos.y, AngularLength.AngularUnit.MIL_NATO, LinearLength.LinearUnit.M);
                box.visualType = ReticleTree.VisualElement.Type.Painted;
                box.illumination = ReticleTree.Light.Type.Powered;

                reticle_hq.elements.Add(box);
            }
        }

        public static void Init() {
            if (crows == null)
            {
                var bundle = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory + "/m1a1assets/", "crows"));
                crows = bundle.LoadAsset<GameObject>("CROWS.prefab");
                crows.hideFlags = HideFlags.DontUnloadUnusedAsset;
                crows.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);

                GameObject gun = crows.transform.Find("GUN/GUN VIS").gameObject;
                GameObject mount = crows.transform.Find("MOUNT/MOUNT VIS").gameObject;

                crows.transform.Find("GUN/GUN COLLIDER").tag = "Penetrable";
                crows.transform.Find("MOUNT/MOUNT COLLIDER").tag = "Penetrable";
                crows.transform.Find("GUN/GUN COLLIDER").gameObject.layer = 7;
                crows.transform.Find("MOUNT/MOUNT COLLIDER").gameObject.layer = 7;

                gun.GetComponent<MeshRenderer>().material.shader = Shader.Find("Standard (FLIR)");
                mount.GetComponent<MeshRenderer>().material.shader = Shader.Find("Standard (FLIR)");
                gun.AddComponent<HeatSource>();
                mount.AddComponent<HeatSource>();
            }

            foreach (Vehicle obj in Resources.FindObjectsOfTypeAll(typeof(Vehicle)))
            {
                if (!ReticleMesh.cachedReticles.ContainsKey("T55-NVS") && obj.name == "T55A")
                {
                    UsableOptic night_optic = obj.transform.Find("Gun Scripts/Sights (and FCS)/NVS").GetComponent<UsableOptic>();
                    night_optic.reticleMesh.Load();
                }

                if (obj.name == "M60A3")
                {
                    m2_browning = obj.transform.Find("Cupola Scripts/12.7mm Machine Gun M85").gameObject;
                    gun_sight = obj.transform.Find("Cupola Scripts/M85 gunsight").gameObject;
                    fcs_go = obj.transform.Find("Cupola Scripts/M85 FCS").gameObject;
                }

                if (obj.name == "M2 Bradley")
                {
                    box_canvas = GameObject.Instantiate(obj.transform.Find("FCS and sights/GPS Optic/M2 Bradley GPS canvas").gameObject);
                    GameObject.Destroy(box_canvas.transform.GetChild(2).gameObject);
                    box_canvas.SetActive(false);
                    box_canvas.hideFlags = HideFlags.DontUnloadUnusedAsset;
                    box_canvas.name = "boxy";
                }

                if (box_canvas != null && m2_browning != null && ReticleMesh.cachedReticles.ContainsKey("T55-NVS")) break;
            }

            if (clip_50cal_400rnd == null) {
                foreach (AmmoCodexScriptable s in Resources.FindObjectsOfTypeAll(typeof(AmmoCodexScriptable)))
                {
                    if (s.name == "ammo_M8")
                    {
                        ammo_codex_m8 = s;
                    }

                    if (s.name == "ammo_50_tracer")
                    {
                        ammo_codex_50tracer = s;
                    }

                    if (ammo_codex_m8 != null && ammo_codex_50tracer != null) break;
                }

                clip_50cal_400rnd = new AmmoType.AmmoClip();
                clip_50cal_400rnd.Capacity = 400;
                clip_50cal_400rnd.Name = "M8 API";
                clip_50cal_400rnd.MinimalPattern = new AmmoCodexScriptable[] {
                    ammo_codex_m8,
                    ammo_codex_m8,
                    ammo_codex_m8,
                    ammo_codex_m8,
                    ammo_codex_50tracer,
                };

                clip_codex_50cal_400rnd = ScriptableObject.CreateInstance<AmmoClipCodexScriptable>();
                clip_codex_50cal_400rnd.name = "clip_50cal_400rnd";
                clip_codex_50cal_400rnd.ClipType = clip_50cal_400rnd;
            }

            ammo_raufoss = new AmmoType();
            Util.ShallowCopy(ammo_raufoss, ammo_codex_m8.AmmoType);
            ammo_raufoss.Name = "Mk 211 Mod 0 HEIAP";
            ammo_raufoss.UseTracer = false;
            ammo_raufoss.TntEquivalentKg = 0.035f;
            ammo_raufoss.RhaPenetration = 32f;
            ammo_raufoss.ImpactFuseTime = 0.002f;
            ammo_raufoss.MuzzleVelocity = 979f;
            ammo_raufoss.ImpactAudio = GHPC.Audio.ImpactAudioType.AutocannonExplosive;
            ammo_raufoss.Category = AmmoType.AmmoCategory.Explosive;
            ammo_raufoss.ImpactEffectDescriptor = new ParticleEffectsManager.ImpactEffectDescriptor()
            {
                HasImpactEffect = true,
                ImpactCategory = ParticleEffectsManager.Category.Kinetic,
                EffectSize = ParticleEffectsManager.EffectSize.Bullet,
                RicochetType = ParticleEffectsManager.RicochetType.None,
                Flags =  ParticleEffectsManager.ImpactModifierFlags.Incendiary | ParticleEffectsManager.ImpactModifierFlags.VeryLarge,
                MinFilterStrictness = ParticleEffectsManager.FilterStrictness.Low
            };
            ammo_raufoss.CachedIndex = -1;

            ammo_codex_raufoss = ScriptableObject.CreateInstance<AmmoCodexScriptable>();
            ammo_codex_raufoss.AmmoType = ammo_raufoss;
            ammo_codex_raufoss.name = "ammo_raufoss";

            ammo_raufoss_tracer = new AmmoType();
            Util.ShallowCopy(ammo_raufoss_tracer, ammo_raufoss);
            ammo_raufoss_tracer.Name = "Mk 30 Mod 0 HEIAP-T";
            ammo_raufoss_tracer.UseTracer = true;
            ammo_raufoss_tracer.ImpactEffectDescriptor.RicochetType = ParticleEffectsManager.RicochetType.LargeTracer;
            ammo_raufoss_tracer.CachedIndex = -1;

            ammo_codex_raufoss_tracer = ScriptableObject.CreateInstance<AmmoCodexScriptable>();
            ammo_codex_raufoss_tracer.AmmoType = ammo_raufoss_tracer;
            ammo_codex_raufoss_tracer.name = "ammo_raufoss_tracer";

            clip_raufoss_400rnd = new AmmoType.AmmoClip();
            clip_raufoss_400rnd.Capacity = 400;
            clip_raufoss_400rnd.Name = "Mk 211 Mod 0 HEIAP";
            clip_raufoss_400rnd.MinimalPattern = new AmmoCodexScriptable[] {
                ammo_codex_raufoss,
                ammo_codex_raufoss,
                ammo_codex_raufoss,
                ammo_codex_raufoss,
                ammo_codex_raufoss_tracer,
            };

            clip_codex_raufoss_400rnd = ScriptableObject.CreateInstance<AmmoClipCodexScriptable>();
            clip_codex_raufoss_400rnd.name = "clip_raufoss_400rnd";
            clip_codex_raufoss_400rnd.ClipType = clip_raufoss_400rnd;
        
            Reticle();
        }
    }
}
