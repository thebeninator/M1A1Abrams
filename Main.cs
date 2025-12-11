using System.Linq;
using MelonLoader;
using M1A1Abrams;
using GHPC.State;
using System.Collections;
using UnityEngine;
using GHPC.Vehicle;

[assembly: MelonInfo(typeof(M1A1AbramsMod), "M1A1 Abrams", "1.3.1B3", "ATLAS")]
[assembly: MelonGame("Radian Simulations LLC", "GHPC")]

namespace M1A1Abrams
{
    public class M1A1AbramsMod : MelonMod {

        public static Vehicle[] vics;
        public static GameObject gameManager;

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
            if (!Assets.done && (scene_name == "MainMenu2_Scene" || scene_name == "MainMenu2-1_Scene" || scene_name == "t64_menu"))
            {
                Assets.Load();
            }

            if (Util.menu_screens.Contains(scene_name)) return;


            gameManager = GameObject.Find("_APP_GHPC_");

            if (gameManager == null) return;

            StateController.RunOrDefer(GameState.GameReady, new GameStateEventHandler(GetVics), GameStatePriority.Medium);

            Ammo_120mm.Init();
            DUArmour.Init();
            //CROWS.Init();
            MPAT.Init();
            CITVManager.Init();
            M1A1.Init();
        }
    }
}
