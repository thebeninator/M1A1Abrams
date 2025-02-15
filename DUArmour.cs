using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GHPC.Equipment;
using UnityEngine;
using System.IO;
using MelonLoader.Utils;

namespace M1A1Abrams
{
    public class DUArmour
    {
        static Material gen1_du_aar_mat;
        static ArmorCodexScriptable gen1_du_armor_codex;

        static Material gen2_du_aar_mat;
        static ArmorCodexScriptable gen2_du_armor_codex;

        static Material gen3_du_aar_mat;
        static ArmorCodexScriptable gen3_du_armor_codex;

        public static Material[] du_aar_mats;
        public static ArmorCodexScriptable[] du_armor_codexes;

        public static void Init() {
            if (gen1_du_armor_codex != null) return;

            AssetBundle mats = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory + "/m1a1assets/", "du_mats"));
            gen1_du_aar_mat = mats.LoadAsset<Material>("gen1.mat");
            gen1_du_aar_mat.hideFlags = HideFlags.DontUnloadUnusedAsset;

            gen2_du_aar_mat = mats.LoadAsset<Material>("gen2.mat");
            gen2_du_aar_mat.hideFlags = HideFlags.DontUnloadUnusedAsset;

            gen3_du_aar_mat = mats.LoadAsset<Material>("gen3.mat");
            gen3_du_aar_mat.hideFlags = HideFlags.DontUnloadUnusedAsset;

            gen1_du_armor_codex = ScriptableObject.CreateInstance<ArmorCodexScriptable>();
            gen1_du_armor_codex.name = "Gen 1 Abrams DU composite";

            gen2_du_armor_codex = ScriptableObject.CreateInstance<ArmorCodexScriptable>();
            gen2_du_armor_codex.name = "Gen 2 Abrams DU composite";

            gen3_du_armor_codex = ScriptableObject.CreateInstance<ArmorCodexScriptable>();
            gen3_du_armor_codex.name = "Gen 3 Abrams DU composite";

            ArmorType gen1 = new ArmorType();
            gen1.Name = "special armor";
            gen1.CanRicochet = true;
            gen1.CanShatterLongRods = true;
            gen1.NormalizesHits = true;
            gen1.ThicknessSource = ArmorType.RhaSource.Multipliers;
            gen1.SpallAngleMultiplier = 1;
            gen1.SpallPowerMultiplier = 0.80f;
            gen1.RhaeMultiplierCe = 1.4f;
            gen1.RhaeMultiplierKe = 0.75f;
            gen1_du_armor_codex.ArmorType = gen1;

            ArmorType gen2 = new ArmorType();
            gen2.Name = "special armor";
            gen2.CanRicochet = true;
            gen2.CanShatterLongRods = true;
            gen2.NormalizesHits = true;
            gen2.ThicknessSource = ArmorType.RhaSource.Multipliers;
            gen2.SpallAngleMultiplier = 1;
            gen2.SpallPowerMultiplier = 0.80f;
            gen2.RhaeMultiplierCe = 1.50f;
            gen2.RhaeMultiplierKe = 0.85f;
            gen2_du_armor_codex.ArmorType = gen2;

            ArmorType gen3 = new ArmorType();
            gen3.Name = "special armor";
            gen3.CanRicochet = true;
            gen3.CanShatterLongRods = true;
            gen3.NormalizesHits = true;
            gen3.ThicknessSource = ArmorType.RhaSource.Multipliers;
            gen3.SpallAngleMultiplier = 1;
            gen3.SpallPowerMultiplier = 0.80f;
            gen3.RhaeMultiplierCe = 1.70f;
            gen3.RhaeMultiplierKe = 0.92f;
            gen3_du_armor_codex.ArmorType = gen3;

            du_aar_mats = new Material[] { gen1_du_aar_mat, gen2_du_aar_mat, gen3_du_aar_mat };
            du_armor_codexes = new ArmorCodexScriptable[] { gen1_du_armor_codex, gen2_du_armor_codex, gen3_du_armor_codex };
        }
    }
}
