using GWLPXL.ARPGCore.CanvasUI.com;
using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Items.com;
using GWLPXL.ARPGCore.Traits.com;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ARPGCore.Statics.com
{


    public static class GenerateEquipmentHelper 
    {

        public static System.Action<EquipmentGenerateClass> OnResultsUpdated;

        public static System.Action<EquipmentGenerateClass, EquipmentTrait> OnNativeTraitAdded;
        public static System.Action<EquipmentGenerateClass, EquipmentTrait> OnLastRandoTraitRemoved;
        public static System.Action<EquipmentGenerateClass, EquipmentTrait> OnLastNativeTraitRemoved;
        public static System.Action<EquipmentGenerateClass, EquipmentTrait> OnRandomTraitAdded;

        public static System.Action<EquipmentGenerateClass> OnClearAllRando;
        public static System.Action<EquipmentGenerateClass> OnClearAllNative;

        public static System.Action<EquipmentGenerateClass, TraitDrops> OnTableUpdate;

        public static void FreezeDungeon(bool shouldFreeze, GameObject mainPanel)
        {
            if (shouldFreeze && mainPanel.activeInHierarchy)
            {
                DungeonMaster.Instance.GetDungeonUISceneRef().SetDungeonState(0);
            }
            else if (shouldFreeze && !mainPanel.activeInHierarchy)
            {
                DungeonMaster.Instance.GetDungeonUISceneRef().SetDungeonState(1);

            }
        }

       

        static void TraitWeightUpdate(EquipmentGenerateClass vars, EquipmentTrait trait, int newwight)
        {
            bool found = false;
            for (int i = 0; i < vars.RandomtraitTemplates.Count; i++)
            {
                if (trait == vars.RandomtraitTemplates[i])
                {
                    vars.RandomtraitTemplates[i].SetWeight(newwight);
                    found = true;
                    Debug.Log("Found");
                    break;
                }
            }
            if (found)
            {
                UpdateTable(vars);
            }
        }

       

        //static EquipmentTrait SetupCopy(EquipmentGenerateClass vars)
        //{
         
        //    EquipmentTrait copy = ScriptableObject.Instantiate(vars.TraitGen.TemplateEmpty[vars.TemplateTrait]);
        //    copy.name = copy.GetTraitName() + " runtime copy";
        //    if (string.IsNullOrEmpty(vars.PrefixString) == false)
        //    {
        //        string[] prefixsplit = vars.PrefixString.Split(',');
        //        copy.SetPrefixes(prefixsplit);
        //    }
        //    if (string.IsNullOrEmpty(vars.SuffixString) == false)
        //    {
        //        string[] suffixsplit = vars.SuffixString.Split(',');
        //        copy.SetSuffixes(suffixsplit);
        //    }


        //    vars.PrefixString = string.Empty;
        //    vars.SuffixString = string.Empty;
        //    //any other values that we need to transfer

        //    return copy;

        //}

        public static void SetTemplate(EquipmentGenerateClass vars, string template)
        {
            vars.SetEquipmentTemplate(template);
        }
        public static void SetILevel(EquipmentGenerateClass vars, string newLevel)
        {
            vars.SetILevel(newLevel);

        }
       
        public static void RemoveLastRando(EquipmentGenerateClass vars)
        {
            vars.RemoveLastRando();

            //if (vars.RandomtraitTemplates.Count == 0)
            //{
            //    Debug.LogWarning("Trying to remove last random but there are no natives to remove", null);
            //    return;
            //}
            //EquipmentTrait removed = vars.RandomtraitTemplates[vars.RandomtraitTemplates.Count - 1];
            //vars.RandomtraitTemplates.RemoveAt(vars.RandomtraitTemplates.Count - 1);
            //OnLastRandoTraitRemoved?.Invoke(vars, removed);
            //UpdateResults(vars);
        }
        public static void RemoveLastNative(EquipmentGenerateClass vars)
        {
            vars.RemoveLastNative();
            //if (vars.NativetraitTemplates.Count == 0)
            //{
            //    Debug.LogWarning("Trying to remove last native but there are no natives to remove", null);
            //    return;
            //}
            //EquipmentTrait removed = vars.NativetraitTemplates[vars.NativetraitTemplates.Count - 1];
            //vars.NativetraitTemplates.RemoveAt(vars.NativetraitTemplates.Count - 1);
            //OnLastNativeTraitRemoved?.Invoke(vars, removed);
            //UpdateResults(vars);
        }
        public static void ClearAllRando(EquipmentGenerateClass vars)
        {
            vars.ClearAllRandom();

            //vars.RandomtraitTemplates.Clear();
            //if (vars.RunTimeGenerated != null)
            //{
            //    vars.RunTimeGenerated.GetTraitTier()[0].PossibleTierDrops.PossibleTraits = new EquipmentTrait[0];
            //    vars.RunTimeGenerated.GetTraitTier()[0].PossibleTierDrops.CreateLootTable();
            //}

            //OnClearAllRando?.Invoke(vars);
            //UpdateResults(vars);
        }
        public static void ClearAllNative(EquipmentGenerateClass vars)
        {
            vars.ClearAllNatives();
            //vars.NativetraitTemplates.Clear();
            //if (vars.RunTimeGenerated != null)
            //{
            //    vars.RunTimeGenerated.GetStats().SetNativeTraits(new EquipmentTrait[0]);
            //}

            
            //OnClearAllNative?.Invoke(vars);
            //UpdateResults(vars);
        }

        public static void AddAsRandomTrait(EquipmentGenerateClass vars)
        {
            vars.AddRandomTrait();
            //if (vars.TemplateTrait == null) return;
            //EquipmentTrait copy = SetupCopy(vars);
            //vars.RandomtraitTemplates.Add(copy);
            //EquipmentTrait[] rando = vars.RandomtraitTemplates.ToArray();
            //vars.RunTimeGenerated.GetTraitTier()[0].PossibleTierDrops.PossibleTraits = rando;
            //OnRandomTraitAdded?.Invoke(vars, copy);
            //UpdateResults(vars);

        }
        public static void AddAsNativeTrait(EquipmentGenerateClass vars)
        {
            vars.AddNativeTrait();
            //if (vars.TemplateTrait == null)
            //{
            //    Debug.LogWarning("Trying to copy trait but dont have any");
            //    return ;
            //}
            //EquipmentTrait copy = SetupCopy(vars);
            //vars.NativetraitTemplates.Add(copy);
            //vars.RunTimeGenerated.GetStats().SetNativeTraits(vars.NativetraitTemplates.ToArray());
            //OnNativeTraitAdded?.Invoke(vars, copy);
            //UpdateResults(vars);
        }

        public static void UpdateResults(EquipmentGenerateClass vars)
        {
            vars.UpdateRuntime();
            //Equipment runtimeCopy = vars.RunTimeGenerated;
            //if (runtimeCopy == null)
            //{
            //    Debug.LogWarning("No runtime copy to generate results");
            //    return;
            //}

            //runtimeCopy.AssignEquipmentTraits(vars.ILevel);
            OnResultsUpdated?.Invoke(vars);

            //UpdateTable(vars);
           
        }

        private static void UpdateTable(EquipmentGenerateClass vars)
        {
            if (vars.RunTimeGenerated == null)
            {
                Debug.LogWarning("Trying to make a trait table but no equipment selected");
                return;
            }
            for (int i = 0; i < vars.RunTimeGenerated.GetTraitTier().Length; i++)
            {
                vars.RunTimeGenerated.GetTraitTier()[i].PossibleTierDrops.CreateLootTable();
                OnTableUpdate?.Invoke(vars, vars.RunTimeGenerated.GetTraitTier()[i].PossibleTierDrops);
            }

          

            
        }
    }
}