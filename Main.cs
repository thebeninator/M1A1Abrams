using System.Linq;
using MelonLoader;
using M1A1Abrams;
using GHPC.State;
using System.Collections;
using UnityEngine;
using GHPC.Vehicle;
using ModUtil;
using Presets;

[assembly: MelonInfo(typeof(M1A1AbramsMod), "M1A1 Abrams", "1.3.3B", "ATLAS")]
[assembly: MelonGame("Radian Simulations LLC", "GHPC")]

namespace M1A1Abrams
{
    public class M1A1AbramsMod : MelonMod {
        private ModuleManager module_manager;
        public static Vehicle[] vics;
        public static GameObject game_manager;
        private int valid_scene_count = 0;

        public IEnumerator OnPlayerReady(GameState _)
        {
            vics = GameObject.FindObjectsByType<Vehicle>(FindObjectsSortMode.None);
            game_manager = GameObject.Find("_APP_GHPC_");

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

            PresetManager.LoadAllPresets();

            //M1Preset template = new M1Preset();
            //string toml_string = TomletMain.TomlStringFrom(template);
            //File.WriteAllText(Path.Combine(MelonEnvironment.ModsDirectory + "/m1a1assets/PresetBundles", "m1 template.cfg"), toml_string);

            //M1IPPreset template2 = new M1IPPreset();
            //string toml_string2 = TomletMain.TomlStringFrom(template2);
            //File.WriteAllText(Path.Combine(MelonEnvironment.ModsDirectory + "/m1a1assets/PresetBundles", "m1ip template.cfg"), toml_string2);
        }

        public override void OnSceneWasLoaded(int build_idx, string scene_name) {
            if (Util.MenuScreens.Contains(scene_name))
            {
                module_manager.LoadAllStaticAssets();
            }

            if (Util.MenuScreensWithLoader.Contains(scene_name)) return;

            valid_scene_count++;

            if (valid_scene_count == 1)
            {
                module_manager.UnloadAllDynamicAssets();
            }

            if (valid_scene_count == 2)
            {
                module_manager.LoadAllDynamicAssets();
                StateController.RunOrDefer(GameState.PlayerReady, new GameStateEventHandler(OnPlayerReady), GameStatePriority.Medium);

                CITVManager.Init();
                M1A1.Init();
                valid_scene_count = 0;
            }
        }
    }
}
