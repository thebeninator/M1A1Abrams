using System.Linq;
using MelonLoader;
using M1A1Abrams;
using GHPC.State;
using System.Collections;
using UnityEngine;
using GHPC.Vehicle;
using ModUtil;

[assembly: MelonInfo(typeof(M1A1AbramsMod), "M1A1 Abrams", "1.3.2", "ATLAS")]
[assembly: MelonGame("Radian Simulations LLC", "GHPC")]

namespace M1A1Abrams
{
    public class M1A1AbramsMod : MelonMod {
        private ModuleManager module_manager;
        public static Vehicle[] vics;
        public static GameObject gameManager;

        public IEnumerator OnGameReady(GameState _)
        {
            vics = GameObject.FindObjectsByType<Vehicle>(FindObjectsSortMode.None);

            module_manager.LoadAllDynamicAssets();

            yield break;
        }

        public override void OnInitializeMelon()
        {
            module_manager = new ModuleManager("M1A1");
            MelonPreferences_Category cfg = MelonPreferences.CreateCategory("M1A1Config");
            M1A1.Config(cfg);

            module_manager.Add("AMMO_120MM", new Ammo_120mm());
            module_manager.Add("DUArmour", new DUArmour());
            module_manager.Add("Assets", new Assets());
            module_manager.Add("GAS", new GAS());
        }

        public override void OnSceneWasLoaded(int idx, string scene_name) {
            module_manager.UnloadAllDynamicAssets();

            if (scene_name == "MainMenu2_Scene" || scene_name == "MainMenu2-1_Scene" || scene_name == "t64_menu")
            {
                module_manager.LoadAllStaticAssets();
                AssetUtil.ReleaseVanillaAssets();
            }

            if (Util.menu_screens.Contains(scene_name)) return;

            gameManager = GameObject.Find("_APP_GHPC_");

            if (gameManager == null) return;

            StateController.RunOrDefer(GameState.GameReady, new GameStateEventHandler(OnGameReady), GameStatePriority.Medium);

            MPAT.Init();
            CITVManager.Init();
            M1A1.Init();
        }
    }
}
