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
using GHPC.Vehicle;

[assembly: MelonInfo(typeof(M1A1AbramsMod), "M1A1 Abrams", "1.1.8B", "ATLAS")]
[assembly: MelonGame("Radian Simulations LLC", "GHPC")]

namespace M1A1Abrams
{
    public class M1A1AbramsMod : MelonMod {

        public static Vehicle[] vics;
        public static GameObject gameManager;
        public static CameraManager camManager;
        public static PlayerInput playerManager;

        public IEnumerator GetVics(GameState _)
        {
            vics = GameObject.FindObjectsByType<Vehicle>(FindObjectsSortMode.None);

            yield break;
        }

        public override void OnInitializeMelon()
        {
            MelonPreferences_Category cfg = MelonPreferences.CreateCategory("M1A1Config");
            M1A1.Config(cfg);
        }

        public override void OnSceneWasLoaded(int idx, string scene_name) {
            if (Util.menu_screens.Contains(scene_name)) return;

            gameManager = GameObject.Find("_APP_GHPC_");
            camManager = gameManager.GetComponent<CameraManager>();
            playerManager = gameManager.GetComponent<PlayerInput>();

            StateController.RunOrDefer(GameState.GameReady, new GameStateEventHandler(GetVics), GameStatePriority.Medium);
            CITV.Init();
            CROWS.Init();
            M1A1.Init();
            MPAT.Init();
        }
    }
}
