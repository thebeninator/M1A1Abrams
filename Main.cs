using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using M1A1Abrams;
using GHPC.State;
using System.Collections;
using UnityEngine;
using GHPC.Camera;
using GHPC.Player;
using FMODUnity;
using FMOD;
using FMODUnityResonance;

[assembly: MelonInfo(typeof(M1A1AbramsMod), "M1A1 Abrams", "1.1.0", "ATLAS")]
[assembly: MelonGame("Radian Simulations LLC", "GHPC")]

namespace M1A1Abrams
{
    public class M1A1AbramsMod : MelonMod {

        public static GameObject[] vic_gos;
        public static GameObject gameManager;
        public static CameraManager camManager;
        public static PlayerInput playerManager;

        public IEnumerator GetVics(GameState _)
        {
            vic_gos = GameObject.FindGameObjectsWithTag("Vehicle");

            yield break;
        }

        public override void OnInitializeMelon()
        {
            MelonPreferences_Category cfg = MelonPreferences.CreateCategory("M1A1Config");
            M1A1.Config(cfg);
        }

        public override void OnLateUpdate()
        {
            M1A1.LateUpdate();
        }

        public override void OnSceneWasLoaded(int idx, string scene_name) {
            if (scene_name == "MainMenu2_Scene" || scene_name == "LOADER_MENU" || scene_name == "LOADER_INITIAL" || scene_name == "t64_menu") return;

            gameManager = GameObject.Find("_APP_GHPC_");
            camManager = gameManager.GetComponent<CameraManager>();
            playerManager = gameManager.GetComponent<PlayerInput>();

            StateController.RunOrDefer(GameState.GameReady, new GameStateEventHandler(GetVics), GameStatePriority.Medium);
            M1A1.Init();
            MPAT.Init();
        }
    }
}
