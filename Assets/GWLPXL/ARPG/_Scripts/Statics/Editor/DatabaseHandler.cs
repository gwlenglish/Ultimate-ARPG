#if UNITY_EDITOR
using GWLPXL.ARPGCore.Abilities.com;
using GWLPXL.ARPGCore.Auras.com;
using GWLPXL.ARPGCore.Attributes.com;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using GWLPXL.ARPGCore.Classes.com;
using GWLPXL.ARPGCore.Items.com;
using GWLPXL.ARPGCore.Quests.com;
using GWLPXL.ARPGCore.Traits.com;
using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Saving.com;
using GWLPXL.ARPGCore.Looting.com;
using System.Text;
using GWLPXL.ARPGCore.Types.com;
using GWLPXL.ARPGCore.Combat.com;
using System.IO;

/// <summary>
/// I dislike the repetition of code, but works for now. Maybe v2 will change. 
/// 
/// </summary>
namespace GWLPXL.ARPGCore.Statics.com
{
    #region reload classes
    public static class MeleeDB
    {
        public static void AddAllToList(MeleeDataDatabase saveSystem)
        {
            List<MeleeData> temp = FindAttributes(saveSystem);

            //sort by ID
            temp.Sort((x, y) => x.GetID().ID.CompareTo(y.GetID().ID));
            MeleeData[] arr = temp.ToArray();
            MeleeDataDatabaseSlot[] dbArr = new MeleeDataDatabaseSlot[arr.Length];

            //resolve any conflicts
            List<MeleeData> conflics = RemoveConflicts(temp, arr);

            //re-sort without conflicts
            temp.Sort((x, y) => x.GetID().ID.CompareTo(y.GetID().ID));
            arr = temp.ToArray();

            //add the sort without conflicts to new array
            for (int i = 0; i < arr.Length; i++)
            {
                dbArr[i] = new MeleeDataDatabaseSlot(arr[i].GetID(), arr[i]);

            }

            //handle the conflicted
            for (int i = 0; i < dbArr.Length; i++)
            {
                if (dbArr[i] == null)
                {
                    if (conflics.Count > 0)
                    {
                        conflics[0].GetID().ID = i;
                        dbArr[i] = new MeleeDataDatabaseSlot(conflics[0].GetID(), conflics[0]);
                        conflics.RemoveAt(0);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            MeleeData[] theRest = conflics.ToArray();
            for (int i = 0; i < theRest.Length; i++)
            {
                System.Array.Resize(ref dbArr, dbArr.Length + 1);
                MeleeDataID newID = new MeleeDataID(theRest[i].name, dbArr.Length - 1, theRest[i]);
                MeleeDataDatabaseSlot newSlot = new MeleeDataDatabaseSlot(newID, theRest[i]);
                dbArr[dbArr.Length - 1] = newSlot;
            }

            for (int i = 0; i < dbArr.Length; i++)
            {
                ISaveJsonConfig jsonSave = dbArr[i].Instance;
                SaveToJson(jsonSave);
            }

            //assign to db
            saveSystem.SetSlots(dbArr);

            //save
            EditorUtility.SetDirty(saveSystem);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        static List<MeleeData> RemoveConflicts(List<MeleeData> temp, MeleeData[] arr)
        {
            List<MeleeData> conflics = new List<MeleeData>();
            List<int> used = new List<int>();
            for (int i = 0; i < arr.Length; i++)
            {
                if (used.Contains(arr[i].GetID().ID))
                {
                    //conflict
                    conflics.Add(arr[i]);
                }
                else
                {
                    //not conflict
                    used.Add(arr[i].GetID().ID);
                }
            }

            //remove conflicts from sort
            for (int i = 0; i < conflics.Count; i++)
            {
                temp.Remove(conflics[i]);
            }

            return conflics;
        }
        static List<MeleeData> FindAttributes(MeleeDataDatabase saveSystem)
        {
            MeleeData template = ScriptableObject.CreateInstance<MeleeData>();
            string key = template.GetType().Name;//magic string, since it's abstract and don't feel like reflection
            string[] folders = saveSystem.GetSearchFolders();
            string[] percents = UnityEditor.AssetDatabase.FindAssets("t:" + key, folders);//specific if you want by putting t:armor or t:equipment, etc.
            List<MeleeData> temp = new List<MeleeData>();
            foreach (var guid in percents)
            {
                string obj = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                MeleeData newItem = UnityEditor.AssetDatabase.LoadAssetAtPath(obj, typeof(MeleeData)) as MeleeData;
                if (newItem != null)
                {
                    temp.Add(newItem);
                    //  Debug.Log("Added");
                }
            }

            return temp;
        }
        static void SaveToJson(ISaveJsonConfig jsonSave)
        {
            if (jsonSave.GetTextAsset() != null)
            {

                JsconConfig.OverwriteJson(jsonSave);
            }
            else
            {
                JsconConfig.SaveJson(jsonSave);
            }
        }
    }
    public static class ProjecileDB
    {
        public static void AddAllToList(ProjectileDataDatabase saveSystem)
        {
            List<ProjectileData> temp = FindAttributes(saveSystem);

            //sort by ID
            temp.Sort((x, y) => x.GetID().ID.CompareTo(y.GetID().ID));
            ProjectileData[] arr = temp.ToArray();
            ProjectileDataDatabaseSlot[] dbArr = new ProjectileDataDatabaseSlot[arr.Length];

            //resolve any conflicts
            List<ProjectileData> conflics = RemoveConflicts(temp, arr);

            //re-sort without conflicts
            temp.Sort((x, y) => x.GetID().ID.CompareTo(y.GetID().ID));
            arr = temp.ToArray();

            //add the sort without conflicts to new array
            for (int i = 0; i < arr.Length; i++)
            {
                dbArr[i] = new ProjectileDataDatabaseSlot(arr[i].GetID(), arr[i]);

            }

            //handle the conflicted
            for (int i = 0; i < dbArr.Length; i++)
            {
                if (dbArr[i] == null)
                {
                    if (conflics.Count > 0)
                    {
                        conflics[0].GetID().ID = i;
                        dbArr[i] = new ProjectileDataDatabaseSlot(conflics[0].GetID(), conflics[0]);
                        conflics.RemoveAt(0);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            ProjectileData[] theRest = conflics.ToArray();
            for (int i = 0; i < theRest.Length; i++)
            {
                System.Array.Resize(ref dbArr, dbArr.Length + 1);
                ProjectileDataID newID = new ProjectileDataID(theRest[i].name, dbArr.Length - 1, theRest[i]);
                ProjectileDataDatabaseSlot newSlot = new ProjectileDataDatabaseSlot(newID, theRest[i]);
                dbArr[dbArr.Length - 1] = newSlot;
            }

            for (int i = 0; i < dbArr.Length; i++)
            {
                ISaveJsonConfig jsonSave = dbArr[i].Instance;
                SaveToJson(jsonSave);
            }

            //assign to db
            saveSystem.SetSlots(dbArr);

            //save
            EditorUtility.SetDirty(saveSystem);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        static List<ProjectileData> RemoveConflicts(List<ProjectileData> temp, ProjectileData[] arr)
        {
            List<ProjectileData> conflics = new List<ProjectileData>();
            List<int> used = new List<int>();
            for (int i = 0; i < arr.Length; i++)
            {
                if (used.Contains(arr[i].GetID().ID))
                {
                    //conflict
                    conflics.Add(arr[i]);
                }
                else
                {
                    //not conflict
                    used.Add(arr[i].GetID().ID);
                }
            }

            //remove conflicts from sort
            for (int i = 0; i < conflics.Count; i++)
            {
                temp.Remove(conflics[i]);
            }

            return conflics;
        }
        static List<ProjectileData> FindAttributes(ProjectileDataDatabase saveSystem)
        {
            ProjectileData template = ScriptableObject.CreateInstance<ProjectileData>();
            string key = template.GetType().Name;//magic string, since it's abstract and don't feel like reflection
            string[] folders = saveSystem.GetSearchFolders();
            string[] percents = UnityEditor.AssetDatabase.FindAssets("t:" + key, folders);//specific if you want by putting t:armor or t:equipment, etc.
            List<ProjectileData> temp = new List<ProjectileData>();
            foreach (var guid in percents)
            {
                string obj = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                ProjectileData newItem = UnityEditor.AssetDatabase.LoadAssetAtPath(obj, typeof(ProjectileData)) as ProjectileData;
                if (newItem != null)
                {
                    temp.Add(newItem);
                    //  Debug.Log("Added");
                }
            }

            return temp;
        }
        static void SaveToJson(ISaveJsonConfig jsonSave)
        {
            if (jsonSave.GetTextAsset() != null)
            {

                JsconConfig.OverwriteJson(jsonSave);
            }
            else
            {
                JsconConfig.SaveJson(jsonSave);
            }
        }
    }
    public static class ActorDamageDB
    {
        public static void AddAllToList(ActorDamageDatabase saveSystem)
        {
            List<ActorDamageData> temp = FindAttributes(saveSystem);

            //sort by ID
            temp.Sort((x, y) => x.GetID().ID.CompareTo(y.GetID().ID));
            ActorDamageData[] arr = temp.ToArray();
            ActorDamageDatabaseSlot[] dbArr = new ActorDamageDatabaseSlot[arr.Length];

            //resolve any conflicts
            List<ActorDamageData> conflics = RemoveConflicts(temp, arr);

            //re-sort without conflicts
            temp.Sort((x, y) => x.GetID().ID.CompareTo(y.GetID().ID));
            arr = temp.ToArray();

            //add the sort without conflicts to new array
            for (int i = 0; i < arr.Length; i++)
            {
                dbArr[i] = new ActorDamageDatabaseSlot(arr[i].GetID(), arr[i]);

            }

            //handle the conflicted
            for (int i = 0; i < dbArr.Length; i++)
            {
                if (dbArr[i] == null)
                {
                    if (conflics.Count > 0)
                    {
                        conflics[0].GetID().ID = i;
                        dbArr[i] = new ActorDamageDatabaseSlot(conflics[0].GetID(), conflics[0]);
                        conflics.RemoveAt(0);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            ActorDamageData[] theRest = conflics.ToArray();
            for (int i = 0; i < theRest.Length; i++)
            {
                System.Array.Resize(ref dbArr, dbArr.Length + 1);
                ActorDamageID newID = new ActorDamageID(theRest[i].name, dbArr.Length - 1, theRest[i]);
                ActorDamageDatabaseSlot newSlot = new ActorDamageDatabaseSlot(newID, theRest[i]);
                dbArr[dbArr.Length - 1] = newSlot;
            }

            for (int i = 0; i < dbArr.Length; i++)
            {
                ISaveJsonConfig jsonSave = dbArr[i].Instance;
                SaveToJson(jsonSave);
            }

            //assign to db
            saveSystem.SetSlots(dbArr);

            //save
            EditorUtility.SetDirty(saveSystem);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        static List<ActorDamageData> RemoveConflicts(List<ActorDamageData> temp, ActorDamageData[] arr)
        {
            List<ActorDamageData> conflics = new List<ActorDamageData>();
            List<int> used = new List<int>();
            for (int i = 0; i < arr.Length; i++)
            {
                if (used.Contains(arr[i].GetID().ID))
                {
                    //conflict
                    conflics.Add(arr[i]);
                }
                else
                {
                    //not conflict
                    used.Add(arr[i].GetID().ID);
                }
            }

            //remove conflicts from sort
            for (int i = 0; i < conflics.Count; i++)
            {
                temp.Remove(conflics[i]);
            }

            return conflics;
        }
        static List<ActorDamageData> FindAttributes(ActorDamageDatabase saveSystem)
        {
            ActorDamageData template = ScriptableObject.CreateInstance<ActorDamageData>();
            string key = template.GetType().Name;//magic string, since it's abstract and don't feel like reflection
            string[] folders = saveSystem.GetSearchFolders();
            string[] percents = UnityEditor.AssetDatabase.FindAssets("t:" + key, folders);//specific if you want by putting t:armor or t:equipment, etc.
            List<ActorDamageData> temp = new List<ActorDamageData>();
            foreach (var guid in percents)
            {
                string obj = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                ActorDamageData newItem = UnityEditor.AssetDatabase.LoadAssetAtPath(obj, typeof(ActorDamageData)) as ActorDamageData;
                if (newItem != null)
                {
                    temp.Add(newItem);
                    //  Debug.Log("Added");
                }
            }

            return temp;
        }
        static void SaveToJson(ISaveJsonConfig jsonSave)
        {
            if (jsonSave.GetTextAsset() != null)
            {

                JsconConfig.OverwriteJson(jsonSave);
            }
            else
            {
                JsconConfig.SaveJson(jsonSave);
            }
        }
    }
    public static class QuestChainDB
    {
        public static void AddAllToList(QuestchainDatabase saveSystem)
        {
            List<Questchain> temp = FindAttributes(saveSystem);

            //sort by ID
            temp.Sort((x, y) => x.GetID().ID.CompareTo(y.GetID().ID));
            Questchain[] arr = temp.ToArray();
            QuestchainDdatabaseSlot[] dbArr = new QuestchainDdatabaseSlot[arr.Length];

            //resolve any conflicts
            List<Questchain> conflics = RemoveConflicts(temp, arr);

            //re-sort without conflicts
            temp.Sort((x, y) => x.GetID().ID.CompareTo(y.GetID().ID));
            arr = temp.ToArray();

            //add the sort without conflicts to new array
            for (int i = 0; i < arr.Length; i++)
            {
                dbArr[i] = new QuestchainDdatabaseSlot(arr[i].GetID(), arr[i]);

            }

            //handle the conflicted
            for (int i = 0; i < dbArr.Length; i++)
            {
                if (dbArr[i] == null)
                {
                    if (conflics.Count > 0)
                    {
                        conflics[0].GetID().ID = i;
                        dbArr[i] = new QuestchainDdatabaseSlot(conflics[0].GetID(), conflics[0]);
                        conflics.RemoveAt(0);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            Questchain[] theRest = conflics.ToArray();
            for (int i = 0; i < theRest.Length; i++)
            {
                System.Array.Resize(ref dbArr, dbArr.Length + 1);
                QuestchainID newID = new QuestchainID(theRest[i].GetQuestName(), dbArr.Length - 1, theRest[i]);
                QuestchainDdatabaseSlot newSlot = new QuestchainDdatabaseSlot(newID, theRest[i]);
                dbArr[dbArr.Length - 1] = newSlot;
            }

            for (int i = 0; i < dbArr.Length; i++)
            {
                ISaveJsonConfig jsonSave = dbArr[i].Questchain;
                SaveToJson(jsonSave);
            }

            //assign to db
            saveSystem.SetSlots(dbArr);

            //save
            EditorUtility.SetDirty(saveSystem);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        static List<Questchain> RemoveConflicts(List<Questchain> temp, Questchain[] arr)
        {
            List<Questchain> conflics = new List<Questchain>();
            List<int> used = new List<int>();
            for (int i = 0; i < arr.Length; i++)
            {
                if (used.Contains(arr[i].GetID().ID))
                {
                    //conflict
                    conflics.Add(arr[i]);
                }
                else
                {
                    //not conflict
                    used.Add(arr[i].GetID().ID);
                }
            }

            //remove conflicts from sort
            for (int i = 0; i < conflics.Count; i++)
            {
                temp.Remove(conflics[i]);
            }

            return conflics;
        }
        static List<Questchain> FindAttributes(QuestchainDatabase saveSystem)
        {
            Questchain template = ScriptableObject.CreateInstance<Questchain>();
            string key = template.GetType().Name;//magic string, since it's abstract and don't feel like reflection
            string[] folders = saveSystem.GetSearchFolders();
            string[] percents = UnityEditor.AssetDatabase.FindAssets("t:" + key, folders);//specific if you want by putting t:armor or t:equipment, etc.
            List<Questchain> temp = new List<Questchain>();
            foreach (var guid in percents)
            {
                string obj = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                Questchain newItem = UnityEditor.AssetDatabase.LoadAssetAtPath(obj, typeof(Questchain)) as Questchain;
                if (newItem != null)
                {
                    temp.Add(newItem);
                    //  Debug.Log("Added");
                }
            }

            return temp;
        }
        static void SaveToJson(ISaveJsonConfig jsonSave)
        {
            if (jsonSave.GetTextAsset() != null)
            {

                JsconConfig.OverwriteJson(jsonSave);
            }
            else
            {
                JsconConfig.SaveJson(jsonSave);
            }
        }
    }
    /// <summary>
    /// trait is abstract, contains magic string
    /// </summary>
    public static class TraitDB
    {
        public static void AddAllToList(EquipmentTraitDatabase saveSystem)
        {
            List<EquipmentTrait> temp = FindAttributes(saveSystem);

            //sort by ID
            temp.Sort((x, y) => x.GetID().ID.CompareTo(y.GetID().ID));
            EquipmentTrait[] arr = temp.ToArray();
            TraitDatabaseSlot[] dbArr = new TraitDatabaseSlot[arr.Length];

            //resolve any conflicts
            List<EquipmentTrait> conflics = RemoveConflicts(temp, arr);

            //re-sort without conflicts
            temp.Sort((x, y) => x.GetID().ID.CompareTo(y.GetID().ID));
            arr = temp.ToArray();

            //add the sort without conflicts to new array
            for (int i = 0; i < arr.Length; i++)
            {
                dbArr[i] = new TraitDatabaseSlot(arr[i].GetID(), arr[i]);

            }

            //handle the conflicted
            for (int i = 0; i < dbArr.Length; i++)
            {
                if (dbArr[i] == null)
                {
                    if (conflics.Count > 0)
                    {
                        conflics[0].GetID().ID = i;
                        dbArr[i] = new TraitDatabaseSlot(conflics[0].GetID(), conflics[0]);
                        conflics.RemoveAt(0);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            EquipmentTrait[] theRest = conflics.ToArray();
            for (int i = 0; i < theRest.Length; i++)
            {
                System.Array.Resize(ref dbArr, dbArr.Length + 1);
                EquipmentTraitID newID = new EquipmentTraitID(theRest[i].GetTraitName(), dbArr.Length - 1, theRest[i]);
                TraitDatabaseSlot newSlot = new TraitDatabaseSlot(newID, theRest[i]);
                dbArr[dbArr.Length - 1] = newSlot;
            }

            for (int i = 0; i < dbArr.Length; i++)
            {
                ISaveJsonConfig jsonSave = dbArr[i].Trait;
                SaveToJson(jsonSave);
            }

            //assign to db
            saveSystem.SetSlots(dbArr);

            //save
            EditorUtility.SetDirty(saveSystem);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        static List<EquipmentTrait> RemoveConflicts(List<EquipmentTrait> temp, EquipmentTrait[] arr)
        {
            List<EquipmentTrait> conflics = new List<EquipmentTrait>();
            List<int> used = new List<int>();
            for (int i = 0; i < arr.Length; i++)
            {
                if (used.Contains(arr[i].GetID().ID))
                {
                    //conflict
                    conflics.Add(arr[i]);
                }
                else
                {
                    //not conflict
                    used.Add(arr[i].GetID().ID);
                }
            }

            //remove conflicts from sort
            for (int i = 0; i < conflics.Count; i++)
            {
                temp.Remove(conflics[i]);
            }

            return conflics;
        }
        static List<EquipmentTrait> FindAttributes(EquipmentTraitDatabase saveSystem)
        {
            string key = saveSystem.GetMagicString();//magic string, since it's abstract and don't feel like reflection
            string[] folders = saveSystem.GetSearchFolders();
            string[] percents = UnityEditor.AssetDatabase.FindAssets("t:" + key, folders);//specific if you want by putting t:armor or t:equipment, etc.
            List<EquipmentTrait> temp = new List<EquipmentTrait>();
            foreach (var guid in percents)
            {
                string obj = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                EquipmentTrait newItem = UnityEditor.AssetDatabase.LoadAssetAtPath(obj, typeof(EquipmentTrait)) as EquipmentTrait;
                if (newItem != null)
                {
                    temp.Add(newItem);
                    //  Debug.Log("Added");
                }
            }

            return temp;
        }
        static void SaveToJson(ISaveJsonConfig jsonSave)
        {
            if (jsonSave.GetTextAsset() != null)
            {

                JsconConfig.OverwriteJson(jsonSave);
            }
            else
            {
                JsconConfig.SaveJson(jsonSave);
            }
        }
    }
    public static class QuestLogDB
    {
        public static void AddAllToList(QuestLogDatabase saveSystem)
        {
            List<QuestLog> temp = FindAttributes(saveSystem);

            //sort by ID
            temp.Sort((x, y) => x.GetID().ID.CompareTo(y.GetID().ID));
            QuestLog[] arr = temp.ToArray();
            QuestLogdatabaseSlot[] dbArr = new QuestLogdatabaseSlot[arr.Length];

            //resolve any conflicts
            List<QuestLog> conflics = RemoveConflicts(temp, arr);

            //re-sort without conflicts
            temp.Sort((x, y) => x.GetID().ID.CompareTo(y.GetID().ID));
            arr = temp.ToArray();

            //add the sort without conflicts to new array
            for (int i = 0; i < arr.Length; i++)
            {
                dbArr[i] = new QuestLogdatabaseSlot(arr[i].GetID(), arr[i]);

            }

            //handle the conflicted
            for (int i = 0; i < dbArr.Length; i++)
            {
                if (dbArr[i] == null)
                {
                    if (conflics.Count > 0)
                    {
                        conflics[0].GetID().ID = i;
                        dbArr[i] = new QuestLogdatabaseSlot(conflics[0].GetID(), conflics[0]);
                        conflics.RemoveAt(0);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            QuestLog[] theRest = conflics.ToArray();
            for (int i = 0; i < theRest.Length; i++)
            {
                System.Array.Resize(ref dbArr, dbArr.Length + 1);
                QuestLogID newID = new QuestLogID(theRest[i].GetID().Name, dbArr.Length - 1, theRest[i]);
                QuestLogdatabaseSlot newSlot = new QuestLogdatabaseSlot(newID, theRest[i]);
                dbArr[dbArr.Length - 1] = newSlot;
            }

            for (int i = 0; i < dbArr.Length; i++)
            {
                ISaveJsonConfig jsonSave = dbArr[i].Quest;
                SaveToJson(jsonSave);
            }

            //assign to db
            saveSystem.SetSlots(dbArr);

            //save
            EditorUtility.SetDirty(saveSystem);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        static List<QuestLog> RemoveConflicts(List<QuestLog> temp, QuestLog[] arr)
        {
            List<QuestLog> conflics = new List<QuestLog>();
            List<int> used = new List<int>();
            for (int i = 0; i < arr.Length; i++)
            {
                if (used.Contains(arr[i].GetID().ID))
                {
                    //conflict
                    conflics.Add(arr[i]);
                }
                else
                {
                    //not conflict
                    used.Add(arr[i].GetID().ID);
                }
            }

            //remove conflicts from sort
            for (int i = 0; i < conflics.Count; i++)
            {
                temp.Remove(conflics[i]);
            }

            return conflics;
        }
        static List<QuestLog> FindAttributes(QuestLogDatabase saveSystem)
        {
            QuestLog template = ScriptableObject.CreateInstance<QuestLog>();
            string key = template.GetType().Name;//magic string, since it's abstract and don't feel like reflection
            string[] folders = saveSystem.GetSearchFolders();
            string[] percents = UnityEditor.AssetDatabase.FindAssets("t:" + key, folders);//specific if you want by putting t:armor or t:equipment, etc.
            List<QuestLog> temp = new List<QuestLog>();
            foreach (var guid in percents)
            {
                string obj = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                QuestLog newItem = UnityEditor.AssetDatabase.LoadAssetAtPath(obj, typeof(QuestLog)) as QuestLog;
                if (newItem != null)
                {
                    temp.Add(newItem);
                    //  Debug.Log("Added");
                }
            }
            UnityEngine.Object.DestroyImmediate(template);
            return temp;
        }
        static void SaveToJson(ISaveJsonConfig jsonSave)
        {
            if (jsonSave.GetTextAsset() != null)
            {

                JsconConfig.OverwriteJson(jsonSave);
            }
            else
            {
                JsconConfig.SaveJson(jsonSave);
            }
        }
    }
    public static class QuestDB
    {
        public static void AddAllToList(QuestDatabase saveSystem)
        {
            List<Quest> temp = FindAttributes(saveSystem);

            //sort by ID
            temp.Sort((x, y) => x.GetID().ID.CompareTo(y.GetID().ID));
            Quest[] arr = temp.ToArray();
            QuestDdatabaseSlot[] dbArr = new QuestDdatabaseSlot[arr.Length];

            //resolve any conflicts
            List<Quest> conflics = RemoveConflicts(temp, arr);

            //re-sort without conflicts
            temp.Sort((x, y) => x.GetID().ID.CompareTo(y.GetID().ID));
            arr = temp.ToArray();

            //add the sort without conflicts to new array
            for (int i = 0; i < arr.Length; i++)
            {
                dbArr[i] = new QuestDdatabaseSlot(arr[i].GetID(), arr[i]);

            }

            //handle the conflicted
            for (int i = 0; i < dbArr.Length; i++)
            {
                if (dbArr[i] == null)
                {
                    if (conflics.Count > 0)
                    {
                        conflics[0].GetID().ID = i;
                        dbArr[i] = new QuestDdatabaseSlot(conflics[0].GetID(), conflics[0]);
                        conflics.RemoveAt(0);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            Quest[] theRest = conflics.ToArray();
            for (int i = 0; i < theRest.Length; i++)
            {
                System.Array.Resize(ref dbArr, dbArr.Length + 1);
                QuestID newID = new QuestID(theRest[i].GetQuestName(), dbArr.Length - 1, theRest[i]);
                QuestDdatabaseSlot newSlot = new QuestDdatabaseSlot(newID, theRest[i]);
                dbArr[dbArr.Length - 1] = newSlot;
            }

            for (int i = 0; i < dbArr.Length; i++)
            {
                ISaveJsonConfig jsonSave = dbArr[i].Quest;
                SaveToJson(jsonSave);
            }

            //assign to db
            saveSystem.SetSlots(dbArr);

            //save
            EditorUtility.SetDirty(saveSystem);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        static List<Quest> RemoveConflicts(List<Quest> temp, Quest[] arr)
        {
            List<Quest> conflics = new List<Quest>();
            List<int> used = new List<int>();
            for (int i = 0; i < arr.Length; i++)
            {
                if (used.Contains(arr[i].GetID().ID))
                {
                    //conflict
                    conflics.Add(arr[i]);
                }
                else
                {
                    //not conflict
                    used.Add(arr[i].GetID().ID);
                }
            }

            //remove conflicts from sort
            for (int i = 0; i < conflics.Count; i++)
            {
                temp.Remove(conflics[i]);
            }

            return conflics;
        }
        static List<Quest> FindAttributes(QuestDatabase saveSystem)
        {
            Quest template = ScriptableObject.CreateInstance<Quest>();
            string key = template.GetType().Name;//magic string, since it's abstract and don't feel like reflection
            string[] folders = saveSystem.GetSearchFolders();
            string[] percents = UnityEditor.AssetDatabase.FindAssets("t:" + key, folders);//specific if you want by putting t:armor or t:equipment, etc.
            List<Quest> temp = new List<Quest>();
            foreach (var guid in percents)
            {
                string obj = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                Quest newItem = UnityEditor.AssetDatabase.LoadAssetAtPath(obj, typeof(Quest)) as Quest;
                if (newItem != null)
                {
                    temp.Add(newItem);
                    //  Debug.Log("Added");
                }
            }

            return temp;
        }
        static void SaveToJson(ISaveJsonConfig jsonSave)
        {
            if (jsonSave.GetTextAsset() != null)
            {

                JsconConfig.OverwriteJson(jsonSave);
            }
            else
            {
                JsconConfig.SaveJson(jsonSave);
            }
        }
    }
    /// <summary>
    /// item is abstract, contains magic string
    /// </summary>
    public static class ItemDB
    {
        public static void AddAllToList(ItemDatabase saveSystem)
        {
            List<Item> temp = FindAttributes(saveSystem);

            //sort by ID
            temp.Sort((x, y) => x.GetID().ID.CompareTo(y.GetID().ID));
            Item[] arr = temp.ToArray();
            ItemDatabaseSlot[] dbArr = new ItemDatabaseSlot[arr.Length];

            //resolve any conflicts
            List<Item> conflics = RemoveConflicts(temp, arr);

            //re-sort without conflicts
            temp.Sort((x, y) => x.GetID().ID.CompareTo(y.GetID().ID));
            arr = temp.ToArray();

            //add the sort without conflicts to new array
            for (int i = 0; i < arr.Length; i++)
            {
                dbArr[i] = new ItemDatabaseSlot(arr[i].GetID(), arr[i]);

            }

            //handle the conflicted
            for (int i = 0; i < dbArr.Length; i++)
            {
                if (dbArr[i] == null)
                {
                    if (conflics.Count > 0)
                    {
                        conflics[0].GetID().ID = i;
                        dbArr[i] = new ItemDatabaseSlot(conflics[0].GetID(), conflics[0]);
                        conflics.RemoveAt(0);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            Item[] theRest = conflics.ToArray();
            for (int i = 0; i < theRest.Length; i++)
            {
                System.Array.Resize(ref dbArr, dbArr.Length + 1);
                ItemID newID = new ItemID(theRest[i].GetGeneratedItemName(), dbArr.Length - 1, theRest[i]);
                ItemDatabaseSlot newSlot = new ItemDatabaseSlot(newID, theRest[i]);
                dbArr[dbArr.Length - 1] = newSlot;
            }

            for (int i = 0; i < dbArr.Length; i++)
            {
                
                ISaveJsonConfig jsonSave = dbArr[i].Item;
                SaveToJson(jsonSave);
            }

            //assign to db
            saveSystem.SetSlots(dbArr);

            //save
            EditorUtility.SetDirty(saveSystem);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        static List<Item> RemoveConflicts(List<Item> temp, Item[] arr)
        {
            List<Item> conflics = new List<Item>();
            List<int> used = new List<int>();
            for (int i = 0; i < arr.Length; i++)
            {
                if (used.Contains(arr[i].GetID().ID))
                {
                    //conflict
                    conflics.Add(arr[i]);
                }
                else
                {
                    //not conflict
                    used.Add(arr[i].GetID().ID);
                }
            }

            //remove conflicts from sort
            for (int i = 0; i < conflics.Count; i++)
            {
                temp.Remove(conflics[i]);
            }

            return conflics;
        }
        static List<Item> FindAttributes(ItemDatabase saveSystem)
        {
            string key = saveSystem.GetMagicString();//magic string, since it's abstract and don't feel like reflection
            string[] folders = saveSystem.GetSearchFolders();
            string[] percents = UnityEditor.AssetDatabase.FindAssets("t:" + key, folders);//specific if you want by putting t:armor or t:equipment, etc.
            List<Item> temp = new List<Item>();
            foreach (var guid in percents)
            {
                string obj = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                Item newItem = UnityEditor.AssetDatabase.LoadAssetAtPath(obj, typeof(Item)) as Item;
                if (newItem != null)
                {
                    temp.Add(newItem);
                    //  Debug.Log("Added");
                }
            }

            return temp;
        }
        static void SaveToJson(ISaveJsonConfig jsonSave)
        {
            if (jsonSave.GetTextAsset() != null)
            {

                JsconConfig.OverwriteJson(jsonSave);
            }
            else
            {
                JsconConfig.SaveJson(jsonSave);
            }
        }
    }
    public static class InvDatabase
    {
        public static void AddAllToList(InventoryDatabase saveSystem)
        {
            List<ActorInventory> temp = FindAttributes(saveSystem);

            //sort by ID
            temp.Sort((x, y) => x.GetID().ID.CompareTo(y.GetID().ID));
            ActorInventory[] arr = temp.ToArray();
            InventoryDatabaseSlot[] dbArr = new InventoryDatabaseSlot[arr.Length];

            //resolve any conflicts
            List<ActorInventory> conflics = RemoveConflicts(temp, arr);

            //re-sort without conflicts
            temp.Sort((x, y) => x.GetID().ID.CompareTo(y.GetID().ID));
            arr = temp.ToArray();

            //add the sort without conflicts to new array
            for (int i = 0; i < arr.Length; i++)
            {
                dbArr[i] = new InventoryDatabaseSlot(arr[i].GetID(), arr[i]);

            }

            //handle the conflicted
            for (int i = 0; i < dbArr.Length; i++)
            {
                if (dbArr[i] == null)
                {
                    if (conflics.Count > 0)
                    {
                        conflics[0].GetID().ID = i;
                        dbArr[i] = new InventoryDatabaseSlot(conflics[0].GetID(), conflics[0]);
                        conflics.RemoveAt(0);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            ActorInventory[] theRest = conflics.ToArray();
            for (int i = 0; i < theRest.Length; i++)
            {
                System.Array.Resize(ref dbArr, dbArr.Length + 1);
                InventoryID newID = new InventoryID(theRest[i].GetName(), dbArr.Length - 1, theRest[i]);
                InventoryDatabaseSlot newSlot = new InventoryDatabaseSlot(newID, theRest[i]);
                dbArr[dbArr.Length - 1] = newSlot;
            }

            for (int i = 0; i < dbArr.Length; i++)
            {
                ISaveJsonConfig jsonSave = dbArr[i].ActorInv;
                SaveToJson(jsonSave);
            }

            //assign to db
            saveSystem.SetSlots(dbArr);

            //save
            EditorUtility.SetDirty(saveSystem);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        static List<ActorInventory> RemoveConflicts(List<ActorInventory> temp, ActorInventory[] arr)
        {
            List<ActorInventory> conflics = new List<ActorInventory>();
            List<int> used = new List<int>();
            for (int i = 0; i < arr.Length; i++)
            {
                if (used.Contains(arr[i].GetID().ID))
                {
                    //conflict
                    conflics.Add(arr[i]);
                }
                else
                {
                    //not conflict
                    used.Add(arr[i].GetID().ID);
                }
            }

            //remove conflicts from sort
            for (int i = 0; i < conflics.Count; i++)
            {
                temp.Remove(conflics[i]);
            }

            return conflics;
        }
        static List<ActorInventory> FindAttributes(InventoryDatabase saveSystem)
        {
            ActorInventory template = ScriptableObject.CreateInstance<ActorInventory>();
            string key = template.GetType().Name;
            string[] folders = saveSystem.GetSearchFolders();
            string[] percents = UnityEditor.AssetDatabase.FindAssets("t:" + key, folders);//specific if you want by putting t:armor or t:equipment, etc.
            List<ActorInventory> temp = new List<ActorInventory>();
            foreach (var guid in percents)
            {
                string obj = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                ActorInventory newItem = UnityEditor.AssetDatabase.LoadAssetAtPath(obj, typeof(ActorInventory)) as ActorInventory;
                if (newItem != null)
                {
                    temp.Add(newItem);
                    //  Debug.Log("Added");
                }
            }

            return temp;
        }
        static void SaveToJson(ISaveJsonConfig jsonSave)
        {
            if (jsonSave.GetTextAsset() != null)
            {

                JsconConfig.OverwriteJson(jsonSave);
            }
            else
            {
                JsconConfig.SaveJson(jsonSave);
            }
        }
    }

    public static class ClassDatabase
    {
        public static void AddAllToList(ActorClassDatabase saveSystem)
        {
            List<ActorClass> temp = FindAttributes(saveSystem);

            //sort by ID
            temp.Sort((x, y) => x.GetID().ID.CompareTo(y.GetID().ID));
            ActorClass[] arr = temp.ToArray();
            ClassDatabaseSlot[] dbArr = new ClassDatabaseSlot[arr.Length];

            //resolve any conflicts
            List<ActorClass> conflics = RemoveConflicts(temp, arr);

            //re-sort without conflicts
            temp.Sort((x, y) => x.GetID().ID.CompareTo(y.GetID().ID));
            arr = temp.ToArray();

            //add the sort without conflicts to new array
            for (int i = 0; i < arr.Length; i++)
            {
                dbArr[i] = new ClassDatabaseSlot(arr[i].GetID(), arr[i]);

            }

            //handle the conflicted
            for (int i = 0; i < dbArr.Length; i++)
            {
                if (dbArr[i] == null)
                {
                    if (conflics.Count > 0)
                    {
                        conflics[0].GetID().ID = i;
                        dbArr[i] = new ClassDatabaseSlot(conflics[0].GetID(), conflics[0]);
                        conflics.RemoveAt(0);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            ActorClass[] theRest = conflics.ToArray();
            for (int i = 0; i < theRest.Length; i++)
            {
                System.Array.Resize(ref dbArr, dbArr.Length + 1);
                ClassID newID = new ClassID(theRest[i].GetClassName(), dbArr.Length - 1, theRest[i]);
                ClassDatabaseSlot newSlot = new ClassDatabaseSlot(newID, theRest[i]);
                dbArr[dbArr.Length - 1] = newSlot;
            }

            for (int i = 0; i < dbArr.Length; i++)
            {
                ISaveJsonConfig jsonSave = dbArr[i].ActorClass;
                SaveToJson(jsonSave);
            }

            //assign to db
            saveSystem.SetSlots(dbArr);

            //save
            EditorUtility.SetDirty(saveSystem);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        static List<ActorClass> RemoveConflicts(List<ActorClass> temp, ActorClass[] arr)
        {
            List<ActorClass> conflics = new List<ActorClass>();
            List<int> used = new List<int>();
            for (int i = 0; i < arr.Length; i++)
            {
                if (used.Contains(arr[i].GetID().ID))
                {
                    //conflict
                    conflics.Add(arr[i]);
                }
                else
                {
                    //not conflict
                    used.Add(arr[i].GetID().ID);
                }
            }

            //remove conflicts from sort
            for (int i = 0; i < conflics.Count; i++)
            {
                temp.Remove(conflics[i]);
            }

            return conflics;
        }
        static List<ActorClass> FindAttributes(ActorClassDatabase saveSystem)
        {
            ActorClass template = ScriptableObject.CreateInstance<ActorClass>();
            string key = template.GetType().Name;
            string[] folders = saveSystem.GetSearchFolders();
            string[] percents = UnityEditor.AssetDatabase.FindAssets("t:" + key, folders);//specific if you want by putting t:armor or t:equipment, etc.
            List<ActorClass> temp = new List<ActorClass>();
            foreach (var guid in percents)
            {
                string obj = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                ActorClass newItem = UnityEditor.AssetDatabase.LoadAssetAtPath(obj, typeof(ActorClass)) as ActorClass;
                if (newItem != null)
                {
                    temp.Add(newItem);
                    //  Debug.Log("Added");
                }
            }

            return temp;
        }
        static void SaveToJson(ISaveJsonConfig jsonSave)
        {
            if (jsonSave.GetTextAsset() != null)
            {

                JsconConfig.OverwriteJson(jsonSave);
            }
            else
            {
                JsconConfig.SaveJson(jsonSave);
            }
        }
    }

    public static class AuraControllersDatabase
    {
        public static void AddAllToList(AuraControllerDatabase saveSystem)
        {
            List<AuraController> temp = FindAttributes(saveSystem);

            //sort by ID
            temp.Sort((x, y) => x.GetID().ID.CompareTo(y.GetID().ID));
            AuraController[] arr = temp.ToArray();
            AuraControllerDatabaseSlot[] dbArr = new AuraControllerDatabaseSlot[arr.Length];

            //resolve any conflicts
            List<AuraController> conflics = RemoveConflicts(temp, arr);

            //re-sort without conflicts
            temp.Sort((x, y) => x.GetID().ID.CompareTo(y.GetID().ID));
            arr = temp.ToArray();

            //add the sort without conflicts to new array
            for (int i = 0; i < arr.Length; i++)
            {
                dbArr[i] = new AuraControllerDatabaseSlot(arr[i].GetID(), arr[i]);

            }

            //handle the conflicted
            for (int i = 0; i < dbArr.Length; i++)
            {
                if (dbArr[i] == null)
                {
                    if (conflics.Count > 0)
                    {
                        conflics[0].GetID().ID = i;
                        dbArr[i] = new AuraControllerDatabaseSlot(conflics[0].GetID(), conflics[0]);
                        conflics.RemoveAt(0);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            AuraController[] theRest = conflics.ToArray();
            for (int i = 0; i < theRest.Length; i++)
            {
                System.Array.Resize(ref dbArr, dbArr.Length + 1);
                AuraControllerData newID = new AuraControllerData(theRest[i].GetID().Name, dbArr.Length - 1, theRest[i]);
                AuraControllerDatabaseSlot newSlot = new AuraControllerDatabaseSlot(newID, theRest[i]);
                dbArr[dbArr.Length - 1] = newSlot;
            }

            for (int i = 0; i < dbArr.Length; i++)
            {
                ISaveJsonConfig jsonSave = dbArr[i].Aura;
                SaveToJson(jsonSave);
            }

            //assign to db
            saveSystem.SetSlots(dbArr);

            //save
            EditorUtility.SetDirty(saveSystem);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        static List<AuraController> RemoveConflicts(List<AuraController> temp, AuraController[] arr)
        {
            List<AuraController> conflics = new List<AuraController>();
            List<int> used = new List<int>();
            for (int i = 0; i < arr.Length; i++)
            {
                if (used.Contains(arr[i].GetID().ID))
                {
                    //conflict
                    conflics.Add(arr[i]);
                }
                else
                {
                    //not conflict
                    used.Add(arr[i].GetID().ID);
                }
            }

            //remove conflicts from sort
            for (int i = 0; i < conflics.Count; i++)
            {
                temp.Remove(conflics[i]);
            }

            return conflics;
        }
        static List<AuraController> FindAttributes(AuraControllerDatabase saveSystem)
        {
            AuraController template = ScriptableObject.CreateInstance<AuraController>();
            string key = template.GetType().Name;
            string[] folders = saveSystem.GetSearchFolders();
            string[] percents = UnityEditor.AssetDatabase.FindAssets("t:" + key, folders);//specific if you want by putting t:armor or t:equipment, etc.
            List<AuraController> temp = new List<AuraController>();
            foreach (var guid in percents)
            {
                string obj = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                AuraController newItem = UnityEditor.AssetDatabase.LoadAssetAtPath(obj, typeof(AuraController)) as AuraController;
                if (newItem != null)
                {
                    temp.Add(newItem);
                    //  Debug.Log("Added");
                }
            }

            return temp;
        }
        static void SaveToJson(ISaveJsonConfig jsonSave)
        {
            if (jsonSave.GetTextAsset() != null)
            {

                JsconConfig.OverwriteJson(jsonSave);
            }
            else
            {
                JsconConfig.SaveJson(jsonSave);
            }
        }
    }

    public static class LootDatabase
    {
        public static void AddAllToList(LootDropsDatabase saveSystem)
        {
            List<LootDrops> temp = FindAttributes(saveSystem);

            //sort by ID
            temp.Sort((x, y) => x.GetID().ID.CompareTo(y.GetID().ID));
            LootDrops[] arr = temp.ToArray();
            LootDropsDatabaseSlot[] dbArr = new LootDropsDatabaseSlot[arr.Length];

            //resolve any conflicts
            List<LootDrops> conflics = RemoveConflicts(temp, arr);

            //re-sort without conflicts
            temp.Sort((x, y) => x.GetID().ID.CompareTo(y.GetID().ID));
            arr = temp.ToArray();

            //add the sort without conflicts to new array
            for (int i = 0; i < arr.Length; i++)
            {
                dbArr[i] = new LootDropsDatabaseSlot(arr[i].GetID(), arr[i]);

            }

            //handle the conflicted
            for (int i = 0; i < dbArr.Length; i++)
            {
                if (dbArr[i] == null)
                {
                    if (conflics.Count > 0)
                    {
                        conflics[0].GetID().ID = i;
                        dbArr[i] = new LootDropsDatabaseSlot(conflics[0].GetID(), conflics[0]);
                        conflics.RemoveAt(0);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            LootDrops[] theRest = conflics.ToArray();
            for (int i = 0; i < theRest.Length; i++)
            {
                System.Array.Resize(ref dbArr, dbArr.Length + 1);
                LootID newID = new LootID(theRest[i].GetID().Name, dbArr.Length - 1, theRest[i]);
                LootDropsDatabaseSlot newSlot = new LootDropsDatabaseSlot(newID, theRest[i]);
                dbArr[dbArr.Length - 1] = newSlot;
            }

            for (int i = 0; i < dbArr.Length; i++)
            {
                ISaveJsonConfig jsonSave = dbArr[i].LootDrops;
                SaveToJson(jsonSave);
            }

            //assign to db
            saveSystem.SetSlots(dbArr);

            //save
            EditorUtility.SetDirty(saveSystem);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        static List<LootDrops> RemoveConflicts(List<LootDrops> temp, LootDrops[] arr)
        {
            List<LootDrops> conflics = new List<LootDrops>();
            List<int> used = new List<int>();
            for (int i = 0; i < arr.Length; i++)
            {
                if (used.Contains(arr[i].GetID().ID))
                {
                    //conflict
                    conflics.Add(arr[i]);
                }
                else
                {
                    //not conflict
                    used.Add(arr[i].GetID().ID);
                }
            }

            //remove conflicts from sort
            for (int i = 0; i < conflics.Count; i++)
            {
                temp.Remove(conflics[i]);
            }

            return conflics;
        }
        static List<LootDrops> FindAttributes(LootDropsDatabase saveSystem)
        {
            LootDrops template = ScriptableObject.CreateInstance<LootDrops>();
            string key = template.GetType().Name;
            string[] folders = saveSystem.GetSearchFolders();
            string[] percents = UnityEditor.AssetDatabase.FindAssets("t:" + key, folders);//specific if you want by putting t:armor or t:equipment, etc.
            List<LootDrops> temp = new List<LootDrops>();
            foreach (var guid in percents)
            {
                string obj = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                LootDrops newItem = UnityEditor.AssetDatabase.LoadAssetAtPath(obj, typeof(LootDrops)) as LootDrops;
                if (newItem != null)
                {
                    temp.Add(newItem);
                    //  Debug.Log("Added");
                }
            }

            return temp;
        }
        static void SaveToJson(ISaveJsonConfig jsonSave)
        {
            if (jsonSave.GetTextAsset() != null)
            {

                JsconConfig.OverwriteJson(jsonSave);
            }
            else
            {
                JsconConfig.SaveJson(jsonSave);
            }
        }
    }
    public static class AurasDatabase
    {
        public static void AddAllToList(AuraDatabase saveSystem)
        {
            List<Aura> temp = FindAttributes(saveSystem);

            //sort by ID
            temp.Sort((x, y) => x.GetID().ID.CompareTo(y.GetID().ID));
            Aura[] arr = temp.ToArray();
            AuraDatabaseSlot[] dbArr = new AuraDatabaseSlot[arr.Length];

            //resolve any conflicts
            List<Aura> conflics = RemoveConflicts(temp, arr);

            //re-sort without conflicts
            temp.Sort((x, y) => x.GetID().ID.CompareTo(y.GetID().ID));
            arr = temp.ToArray();

            //add the sort without conflicts to new array
            for (int i = 0; i < arr.Length; i++)
            {
                dbArr[i] = new AuraDatabaseSlot(arr[i].GetID(), arr[i]);

            }

            //handle the conflicted
            for (int i = 0; i < dbArr.Length; i++)
            {
                if (dbArr[i] == null)
                {
                    if (conflics.Count > 0)
                    {
                        conflics[0].GetID().ID = i;
                        dbArr[i] = new AuraDatabaseSlot(conflics[0].GetID(), conflics[0]);
                        conflics.RemoveAt(0);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            Aura[] theRest = conflics.ToArray();
            for (int i = 0; i < theRest.Length; i++)
            {
                System.Array.Resize(ref dbArr, dbArr.Length + 1);
                AuraID newID = new AuraID(theRest[i].AuraData.AuraName, dbArr.Length - 1, theRest[i]);
                AuraDatabaseSlot newSlot = new AuraDatabaseSlot(newID, theRest[i]);
                dbArr[dbArr.Length - 1] = newSlot;
            }

            for (int i = 0; i < dbArr.Length; i++)
            {
                ISaveJsonConfig jsonSave = dbArr[i].Aura;
                SaveToJson(jsonSave);
            }

            //assign to db
            saveSystem.SetSlots(dbArr);

            //save
            EditorUtility.SetDirty(saveSystem);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        static List<Aura> RemoveConflicts(List<Aura> temp, Aura[] arr)
        {
            List<Aura> conflics = new List<Aura>();
            List<int> used = new List<int>();
            for (int i = 0; i < arr.Length; i++)
            {
                if (used.Contains(arr[i].GetID().ID))
                {
                    //conflict
                    conflics.Add(arr[i]);
                }
                else
                {
                    //not conflict
                    used.Add(arr[i].GetID().ID);
                }
            }

            //remove conflicts from sort
            for (int i = 0; i < conflics.Count; i++)
            {
                temp.Remove(conflics[i]);
            }

            return conflics;
        }
        static List<Aura> FindAttributes(AuraDatabase saveSystem)
        {
            Aura template = ScriptableObject.CreateInstance<Aura>();
            string key = template.GetType().Name;
            string[] folders = saveSystem.GetSearchFolders();
            string[] percents = UnityEditor.AssetDatabase.FindAssets("t:" + key, folders);//specific if you want by putting t:armor or t:equipment, etc.
            List<Aura> temp = new List<Aura>();
            foreach (var guid in percents)
            {
                string obj = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                Aura newItem = UnityEditor.AssetDatabase.LoadAssetAtPath(obj, typeof(Aura)) as Aura;
                if (newItem != null)
                {
                    temp.Add(newItem);
                    //  Debug.Log("Added");
                }
            }

            return temp;
        }
        static void SaveToJson(ISaveJsonConfig jsonSave)
        {
            if (jsonSave.GetTextAsset() != null)
            {

                JsconConfig.OverwriteJson(jsonSave);
            }
            else
            {
                JsconConfig.SaveJson(jsonSave);
            }
        }
    }

    public static class AttributesDatabase
    {
        public static void AddAllToList(ActorAttributesDatabase saveSystem)
        {
            List<ActorAttributes> temp = FindAttributes(saveSystem);

            //sort by ID
            temp.Sort((x, y) => x.GetID().ID.CompareTo(y.GetID().ID));
            ActorAttributes[] arr = temp.ToArray();
            AttributesDatabaseSlot[] dbArr = new AttributesDatabaseSlot[arr.Length];

            //resolve any conflicts
            List<ActorAttributes> conflics = RemoveConflicts(temp, arr);

            //re-sort without conflicts
            temp.Sort((x, y) => x.GetID().ID.CompareTo(y.GetID().ID));
            arr = temp.ToArray();

            //add the sort without conflicts to new array
            for (int i = 0; i < arr.Length; i++)
            {
                dbArr[i] = new AttributesDatabaseSlot(arr[i].GetID(), arr[i]);

            }

            //handle the conflicted
            for (int i = 0; i < dbArr.Length; i++)
            {
                if (dbArr[i] == null)
                {
                    if (conflics.Count > 0)
                    {
                        conflics[0].GetID().ID = i;
                        dbArr[i] = new AttributesDatabaseSlot(conflics[0].GetID(), conflics[0]);
                        conflics.RemoveAt(0);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            ActorAttributes[] theRest = conflics.ToArray();
            for (int i = 0; i < theRest.Length; i++)
            {
                System.Array.Resize(ref dbArr, dbArr.Length + 1);
                AttributesID newID = new AttributesID(theRest[i].ActorName, dbArr.Length - 1, theRest[i]);
                AttributesDatabaseSlot newSlot = new AttributesDatabaseSlot(newID, theRest[i]);
                dbArr[dbArr.Length - 1] = newSlot;
            }

            for (int i = 0; i < dbArr.Length; i++)
            {
                ISaveJsonConfig jsonSave = dbArr[i].ActorStats;
                SaveToJson(jsonSave);
            }

            //assign to db
            saveSystem.SetSlots(dbArr);

            //save
            EditorUtility.SetDirty(saveSystem);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        static List<ActorAttributes> RemoveConflicts(List<ActorAttributes> temp, ActorAttributes[] arr)
        {
            List<ActorAttributes> conflics = new List<ActorAttributes>();
            List<int> used = new List<int>();
            for (int i = 0; i < arr.Length; i++)
            {
                if (used.Contains(arr[i].GetID().ID))
                {
                    //conflict
                    conflics.Add(arr[i]);
                }
                else
                {
                    //not conflict
                    used.Add(arr[i].GetID().ID);
                }
            }

            //remove conflicts from sort
            for (int i = 0; i < conflics.Count; i++)
            {
                temp.Remove(conflics[i]);
            }

            return conflics;
        }
        static List<ActorAttributes> FindAttributes(ActorAttributesDatabase saveSystem)
        {
            ActorAttributes template = ScriptableObject.CreateInstance<ActorAttributes>();
            string key = template.GetType().Name;
            string[] folders = saveSystem.GetSearchFolders();
            string[] percents = UnityEditor.AssetDatabase.FindAssets("t:" + key, folders);//specific if you want by putting t:armor or t:equipment, etc.
            List<ActorAttributes> temp = new List<ActorAttributes>();
            foreach (var guid in percents)
            {
                string obj = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                ActorAttributes newItem = UnityEditor.AssetDatabase.LoadAssetAtPath(obj, typeof(ActorAttributes)) as ActorAttributes;
                if (newItem != null)
                {
                    temp.Add(newItem);
                    //  Debug.Log("Added");
                }
            }

            return temp;
        }
        static void SaveToJson(ISaveJsonConfig jsonSave)
        {
            if (jsonSave.GetTextAsset() != null)
            {

                JsconConfig.OverwriteJson(jsonSave);
            }
            else
            {
                JsconConfig.SaveJson(jsonSave);
            }
        }
    }
    public static class AbilityDatabase
    {
        public static void AddAllToList(AbilitiesDatabase saveSystem)
        {
            List<Ability> temp = FindAbilities(saveSystem);

            //sort by ID
            temp.Sort((x, y) => x.GetID().ID.CompareTo(y.GetID().ID));
            Ability[] arr = temp.ToArray();
            AbilityDatabaseSlot[] dbArr = new AbilityDatabaseSlot[arr.Length];

            //resolve any conflicts
            List<Ability> conflics = RemoveConflicts(temp, arr);

            //re-sort without conflicts
            temp.Sort((x, y) => x.GetID().ID.CompareTo(y.GetID().ID));
            arr = temp.ToArray();

            //add the sort without conflicts to new array
            for (int i = 0; i < arr.Length; i++)
            {
                dbArr[i] = new AbilityDatabaseSlot(arr[i].GetID(), arr[i]);

            }

            //handle the conflicted
            for (int i = 0; i < dbArr.Length; i++)
            {
                if (dbArr[i] == null)
                {
                    if (conflics.Count > 0)
                    {
                        conflics[0].GetID().ID = i;
                        dbArr[i] = new AbilityDatabaseSlot(conflics[0].GetID(), conflics[0]);
                        conflics.RemoveAt(0);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            Ability[] theRest = conflics.ToArray();
            for (int i = 0; i < theRest.Length; i++)
            {
                System.Array.Resize(ref dbArr, dbArr.Length + 1);
                AbilityID newID = new AbilityID(theRest[i].GetName(), dbArr.Length - 1, theRest[i]);
                AbilityDatabaseSlot newSlot = new AbilityDatabaseSlot(newID, theRest[i]);
                dbArr[dbArr.Length - 1] = newSlot;
            }

            for (int i = 0; i < dbArr.Length; i++)
            {
                ISaveJsonConfig jsonSave = dbArr[i].ID.Ability;
                SaveToJson(jsonSave);
            }

            //assign to db
            saveSystem.SetSlots(dbArr);

            //save
            EditorUtility.SetDirty(saveSystem);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        static List<Ability> RemoveConflicts(List<Ability> temp, Ability[] arr)
        {
            List<Ability> conflics = new List<Ability>();
            List<int> used = new List<int>();
            for (int i = 0; i < arr.Length; i++)
            {
                if (used.Contains(arr[i].GetID().ID))
                {
                    //conflict
                    conflics.Add(arr[i]);
                }
                else
                {
                    //not conflict
                    used.Add(arr[i].GetID().ID);
                }
            }

            //remove conflicts from sort
            for (int i = 0; i < conflics.Count; i++)
            {
                temp.Remove(conflics[i]);
            }

            return conflics;
        }
        static List<Ability> FindAbilities(AbilitiesDatabase saveSystem)
        {
            Ability template = ScriptableObject.CreateInstance<Ability>();
            string key = template.GetType().Name;
            string[] folders = saveSystem.GetSearchFolders();
            string[] percents = UnityEditor.AssetDatabase.FindAssets("t:" + key, folders);//specific if you want by putting t:armor or t:equipment, etc.
            List<Ability> temp = new List<Ability>();
            foreach (var guid in percents)
            {
                string obj = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                Ability newItem = UnityEditor.AssetDatabase.LoadAssetAtPath(obj, typeof(Ability)) as Ability;
                if (newItem != null)
                {
                    temp.Add(newItem);
                    //  Debug.Log("Added");
                }
            }

            return temp;
        }
        static void SaveToJson(ISaveJsonConfig jsonSave)
        {
            if (jsonSave.GetTextAsset() != null)
            {

                JsconConfig.OverwriteJson(jsonSave);
            }
            else
            {
                JsconConfig.SaveJson(jsonSave);
            }
        }
    }
    public static class AbilityControllersDatabase
    {
        public static void AddAllToList(AbilityControllerDatabase saveSystem)
        {
            List<AbilityController> temp = FindAbilities(saveSystem);

            //sort by ID
            temp.Sort((x, y) => x.GetID().ID.CompareTo(y.GetID().ID));
            AbilityController[] arr = temp.ToArray();
            AbilityControllerDatabaseSlot[] dbArr = new AbilityControllerDatabaseSlot[arr.Length];

            //resolve any conflicts
            List<AbilityController> conflics = RemoveConflicts(temp, arr);

            //re-sort without conflicts
            temp.Sort((x, y) => x.GetID().ID.CompareTo(y.GetID().ID));
            arr = temp.ToArray();

            //add the sort without conflicts to new array
            for (int i = 0; i < arr.Length; i++)
            {
                dbArr[i] = new AbilityControllerDatabaseSlot(arr[i].GetID(), arr[i]);

            }

            //handle the conflicted
            for (int i = 0; i < dbArr.Length; i++)
            {
                if (dbArr[i] == null)
                {
                    if (conflics.Count > 0)
                    {
                        conflics[0].GetID().ID = i;
                        dbArr[i] = new AbilityControllerDatabaseSlot(conflics[0].GetID(), conflics[0]);
                        conflics.RemoveAt(0);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            AbilityController[] theRest = conflics.ToArray();
            for (int i = 0; i < theRest.Length; i++)
            {
                System.Array.Resize(ref dbArr, dbArr.Length + 1);
                AbilityControllerID newID = new AbilityControllerID(theRest[i].Data.Name, dbArr.Length - 1, theRest[i]);
                AbilityControllerDatabaseSlot newSlot = new AbilityControllerDatabaseSlot(newID, theRest[i]);
                dbArr[dbArr.Length - 1] = newSlot;
            }

            for (int i = 0; i < dbArr.Length; i++)
            {
                ISaveJsonConfig jsonSave = dbArr[i].ID.AbilityController;
                SaveToJson(jsonSave);
            }

            //assign to db
            saveSystem.SetSlots(dbArr);

            //save
            EditorUtility.SetDirty(saveSystem);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        static List<AbilityController> RemoveConflicts(List<AbilityController> temp, AbilityController[] arr)
        {
            List<AbilityController> conflics = new List<AbilityController>();
            List<int> used = new List<int>();
            for (int i = 0; i < arr.Length; i++)
            {
                if (used.Contains(arr[i].GetID().ID))
                {
                    //conflict
                    conflics.Add(arr[i]);
                }
                else
                {
                    //not conflict
                    used.Add(arr[i].GetID().ID);
                }
            }

            //remove conflicts from sort
            for (int i = 0; i < conflics.Count; i++)
            {
                temp.Remove(conflics[i]);
            }

            return conflics;
        }
        static List<AbilityController> FindAbilities(AbilityControllerDatabase saveSystem)
        {
            AbilityController template = ScriptableObject.CreateInstance<AbilityController>();
            string key = template.GetType().Name;
            string[] folders = saveSystem.GetSearchFolders();
            string[] percents = UnityEditor.AssetDatabase.FindAssets("t:" + key, folders);//specific if you want by putting t:armor or t:equipment, etc.
            List<AbilityController> temp = new List<AbilityController>();
            foreach (var guid in percents)
            {
                string obj = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                AbilityController newItem = UnityEditor.AssetDatabase.LoadAssetAtPath(obj, typeof(AbilityController)) as AbilityController;
                if (newItem != null)
                {
                    temp.Add(newItem);
                    //  Debug.Log("Added");
                }
            }
            UnityEngine.Object.DestroyImmediate(template);
            return temp;
        }
        static void SaveToJson(ISaveJsonConfig jsonSave)
        {
            if (jsonSave.GetTextAsset() != null)
            {

                JsconConfig.OverwriteJson(jsonSave);
            }
            else
            {
                JsconConfig.SaveJson(jsonSave);
            }
        }
    }
    public static class ARPGGameDatabase
    {
        public static void ReloadGameDatabase(GameDatabase gamedatabase)
        {
            List<IDatabase> databases = FindAttributes(gamedatabase);
            SetDatabase(databases.ToArray(), gamedatabase);
        }
        static void SetDatabase(IDatabase[] databases, GameDatabase gamedatabase)
        {
            List<DatabaseID> list = new List<DatabaseID>();
            List<IDatabase> databseslist = new List<IDatabase>();
            foreach (DatabaseID pieceType in DatabaseID.GetValues(typeof(DatabaseID)))
            {
                list.Add(pieceType);
            }

            gamedatabase.allPossibledatabaseTypes = list.ToArray();
            databseslist = new List<IDatabase>();
            databseslist.Add(gamedatabase);//force the game database to be first
            for (int i = 0; i < databases.Length; i++)
            {
                if (databases[i].GetDatabaseEntry() == DatabaseID.GameDatabase) continue;
                databseslist.Add(databases[i]);
            }
            gamedatabase.databases = new IDatabase[databseslist.Count];
            gamedatabase.databases = databseslist.ToArray();

            gamedatabase.names = new string[gamedatabase.databases.Length];
            gamedatabase.currentTypesInGame = new DatabaseID[gamedatabase.databases.Length];
            for (int i = 0; i < gamedatabase.names.Length; i++)
            {
                gamedatabase.names[i] = gamedatabase.databases[i].GetDatabaseEntry().ToString();
                gamedatabase.currentTypesInGame[i] = gamedatabase.databases[i].GetDatabaseEntry();
            }


            for (int i = 0; i < databases.Length; i++)
            {
                DatabaseID id = databases[i].GetDatabaseEntry();
                IDatabase idatabase = databases[i];
                bool assigned = AssignDatabase(id, idatabase, gamedatabase);

                if (assigned)
                {
                    list.Remove(id);
                }
            }

            //can only create in the editor
            for (int i = 0; i < list.Count; i++)
            {

                //does not have, wish to create?
                if (CreateDatabase(list[i], gamedatabase))
                {
                    Debug.LogWarning("Database not found. Creating a new one to add to the game database");

                }
            }
        }
        private static bool AssignDatabase(DatabaseID id, IDatabase idatabase, GameDatabase gamedatabase)
        {
            switch (id)
            {
                case DatabaseID.GameDatabase:
                    return true;
                case DatabaseID.AbilityControllers:
                    gamedatabase.AbilityControllers = idatabase as AbilityControllerDatabase;
                    return true;
                case DatabaseID.Abilities:
                    gamedatabase.Abilities = idatabase as AbilitiesDatabase;
                    return true;
                case DatabaseID.ActorDamageDealers:
                    gamedatabase.ActorDamageTypes = idatabase as ActorDamageDatabase;
                    return true;
                case DatabaseID.Attributes:
                    gamedatabase.Attributes = idatabase as ActorAttributesDatabase;
                    return true;
                case DatabaseID.Auras:
                    gamedatabase.Auras = idatabase as AuraDatabase;
                    return true;
                case DatabaseID.AuraControllers:
                    gamedatabase.AuraControllers = idatabase as AuraControllerDatabase;
                    return true;
                case DatabaseID.Classes:
                    gamedatabase.Classes = idatabase as ActorClassDatabase;
                    return true;
                case DatabaseID.Inventories:
                    gamedatabase.Inventories = idatabase as InventoryDatabase;
                    return true;
                case DatabaseID.Items:
                    gamedatabase.Items = idatabase as ItemDatabase;
                    return true;
                case DatabaseID.LootDrops:
                    gamedatabase.Loot = idatabase as LootDropsDatabase;
                    return true;
                case DatabaseID.Melees:
                    gamedatabase.Melee = idatabase as MeleeDataDatabase;
                    return true;
                case DatabaseID.Projectiles:
                    gamedatabase.Projectiles = idatabase as ProjectileDataDatabase;
                    return true;
                case DatabaseID.Questchains:
                    gamedatabase.Questchains = idatabase as QuestchainDatabase;
                    return true;
                case DatabaseID.Quests:
                    gamedatabase.Quests = idatabase as QuestDatabase;
                    return true;
                case DatabaseID.QuestLogs:
                    gamedatabase.QuestLog = idatabase as QuestLogDatabase;
                    return true;
                case DatabaseID.EquipmentTraits:
                    gamedatabase.Traits = idatabase as EquipmentTraitDatabase;
                    return true;

            }
            return false;
        }

        
        private static bool CreateDatabase(DatabaseID id, GameDatabase gamedatabase)
        {
            ScriptableObject scriptable = null;
            switch (id)
            {
                case DatabaseID.AuraControllers:
                    scriptable = ScriptableObject.CreateInstance<AuraControllerDatabase>() as AuraControllerDatabase;
                    gamedatabase.AuraControllers = scriptable as AuraControllerDatabase;
                    break;
                case DatabaseID.Abilities:
                    scriptable = ScriptableObject.CreateInstance<AbilitiesDatabase>() as AbilitiesDatabase;
                    gamedatabase.Abilities = scriptable as AbilitiesDatabase;
                    break;
                case DatabaseID.ActorDamageDealers:
                    scriptable = ScriptableObject.CreateInstance<ActorDamageDatabase>() as ActorDamageDatabase;
                    gamedatabase.ActorDamageTypes = scriptable as ActorDamageDatabase;
                    break;
                case DatabaseID.AbilityControllers:
                    scriptable = ScriptableObject.CreateInstance<AbilityControllerDatabase>() as AbilityControllerDatabase;
                    gamedatabase.AbilityControllers = scriptable as AbilityControllerDatabase;
                    break;
                case DatabaseID.Attributes:
                    scriptable = ScriptableObject.CreateInstance<ActorAttributesDatabase>() as ActorAttributesDatabase;
                    gamedatabase.Attributes = scriptable as ActorAttributesDatabase;
                    break;
                case DatabaseID.Auras:
                    scriptable = ScriptableObject.CreateInstance<AuraDatabase>() as AuraDatabase;
                    gamedatabase.Auras = scriptable as AuraDatabase;
                    break;
                case DatabaseID.Classes:
                    scriptable = ScriptableObject.CreateInstance<ActorClassDatabase>() as ActorClassDatabase;
                    gamedatabase.Classes = scriptable as ActorClassDatabase;
                    break;
                case DatabaseID.Inventories:
                    scriptable = ScriptableObject.CreateInstance<InventoryDatabase>() as InventoryDatabase;
                    gamedatabase.Inventories = scriptable as InventoryDatabase;
                    break;
                case DatabaseID.LootDrops:
                    scriptable = ScriptableObject.CreateInstance<LootDropsDatabase>() as LootDropsDatabase;
                    gamedatabase.Loot = scriptable as LootDropsDatabase;
                    break;
                case DatabaseID.Items:
                    scriptable = ScriptableObject.CreateInstance<ItemDatabase>() as ItemDatabase;
                    gamedatabase.Items = scriptable as ItemDatabase;
                    break;
                case DatabaseID.Melees:
                    scriptable = ScriptableObject.CreateInstance<MeleeDataDatabase>() as MeleeDataDatabase;
                    gamedatabase.Melee = scriptable as MeleeDataDatabase;
                    break;
                case DatabaseID.Projectiles:
                    scriptable = ScriptableObject.CreateInstance<ProjectileDataDatabase>() as ProjectileDataDatabase;
                    gamedatabase.Projectiles = scriptable as ProjectileDataDatabase;
                    break;
                case DatabaseID.Questchains:
                    scriptable = ScriptableObject.CreateInstance<QuestchainDatabase>() as QuestchainDatabase;
                    gamedatabase.Questchains = scriptable as QuestchainDatabase;
                    break;
                case DatabaseID.Quests:
                    scriptable = ScriptableObject.CreateInstance<QuestDatabase>() as QuestDatabase;
                    gamedatabase.Quests = scriptable as QuestDatabase;
                    break;
                case DatabaseID.QuestLogs:
                    scriptable = ScriptableObject.CreateInstance<QuestLogDatabase>() as QuestLogDatabase;
                    gamedatabase.QuestLog = scriptable as QuestLogDatabase;
                    break;
                case DatabaseID.EquipmentTraits:
                    scriptable = ScriptableObject.CreateInstance<EquipmentTraitDatabase>() as EquipmentTraitDatabase;
                    gamedatabase.Traits = scriptable as EquipmentTraitDatabase;
                    break;

            }

            if (scriptable != null)
            {
                //store the string in the same place as the game database
                IDatabase database = scriptable as IDatabase;

                string gamedatabaselocation = AssetDatabase.GetAssetPath(gamedatabase);
                //truncate path to just the folder

                string defaultdatabasename = "Database_" + database.GetDatabaseEntry().ToString();
                string defaultdatabaseextension = ".asset";

                string[] splitpath = gamedatabaselocation.Split('/');//split
                splitpath[splitpath.Length - 1] = string.Empty;
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < splitpath.Length; i++)
                {
                    sb.Append(splitpath[i]);
                    if (i != splitpath.Length - 1)
                    {
                        sb.Append("/");
                    }
                }

                string truncatedsavepath = sb.ToString();
              
                string compiledstring = truncatedsavepath + defaultdatabasename + defaultdatabaseextension;

                string searchpath = truncatedsavepath.Substring(0, truncatedsavepath.LastIndexOf('/'));

                database.SetSearchFolders(new string[1] { searchpath });//default the search folders to where the database was created;

                UnityEditor.AssetDatabase.CreateAsset(scriptable, compiledstring);
                UnityEditor.AssetDatabase.SaveAssets();
                System.Array.Resize(ref gamedatabase.currentTypesInGame, gamedatabase.currentTypesInGame.Length + 1);
                UnityEngine.Object newDatabase = UnityEditor.AssetDatabase.LoadAssetAtPath(compiledstring, typeof(UnityEngine.Object));
                IDatabase databse = newDatabase as IDatabase;

                gamedatabase.currentTypesInGame[gamedatabase.currentTypesInGame.Length - 1] = databse.GetDatabaseEntry();
                return true;
            }
            return false;
        }


        static List<IDatabase> FindAttributes(GameDatabase saveSystem)
        {

            string[] folders = saveSystem.GetDatabaseFolders;
            string key = "ScriptableObject";
            string[] percents = UnityEditor.AssetDatabase.FindAssets("t:" + key, folders);//specific if you want by putting t:armor or t:equipment, etc.
            List<IDatabase> temp = new List<IDatabase>();
            foreach (var guid in percents)
            {
                string obj = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                ScriptableObject newItem = UnityEditor.AssetDatabase.LoadAssetAtPath(obj, typeof(ScriptableObject)) as ScriptableObject;
                if (newItem is IDatabase)
                {
                    temp.Add(newItem as IDatabase);
                }
            }

            return temp;
        }
    }
    #endregion
    public static class DatabaseHandler
    {
        public static void CreateCopies(IDatabase database, string path)
        {
            string extension = ".asset";

            for (int i = 0; i < database.GetJsons().Length; i++)
            {
                Object objectToCopy = database.GetDatabaseObjectBySlotIndex(i);
                if (objectToCopy == null)
                {
                    UnityEngine.Debug.Log("Object is null at " + database.GetDatabaseEntry().ToString() + " index " + i.ToString());
                    continue;
                }
                Object obj = ScriptableObject.Instantiate(objectToCopy);
                string exportName = "\\" + obj.name + extension;
                AssetDatabase.CreateAsset(obj, path + exportName);

                //removes reference to text asset.
                Object ob = AssetDatabase.LoadAssetAtPath(path + exportName, typeof(Object));
                ISaveJsonConfig saver = ob as ISaveJsonConfig;
                saver.SetTextAsset(null);

            }
        }


          

        

        public static GameDatabase CreateNewGameDatabse(string pathwithname, bool withnewsubdatabases, string name)
        {
           GameDatabase gamedatase = ScriptableObject.CreateInstance<GameDatabase>();
           IDatabase idatabase = gamedatase as IDatabase;


            string[] splitpath = pathwithname.Split('/');//split
            splitpath[splitpath.Length - 1] = string.Empty;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < splitpath.Length; i++)
            {
                sb.Append(splitpath[i]);
                if (i != splitpath.Length - 1)
                {
                    sb.Append("/");
                }
            }

            string test = sb.ToString();
            string searchpath = test.Substring(0, test.LastIndexOf('/'));

            idatabase.SetSearchFolders(new string[1] { searchpath });

            UnityEditor.AssetDatabase.CreateAsset(gamedatase, pathwithname);
            UnityEditor.AssetDatabase.SaveAssets();


            //project settings
            string settingspathwithname = sb.ToString() + "Settings_" + name + ".asset";
            ProjectSettings newsettings = ScriptableObject.CreateInstance<ProjectSettings>();
            UnityEditor.AssetDatabase.CreateAsset(newsettings, settingspathwithname);
            UnityEditor.AssetDatabase.SaveAssets();


            UnityEditor.AssetDatabase.Refresh();
            UnityEngine.Object newgamedatabaseasset = UnityEditor.AssetDatabase.LoadAssetAtPath(pathwithname, typeof(UnityEngine.Object));
            GameDatabase gamed = newgamedatabaseasset as GameDatabase;
            UnityEngine.Object newgamesettings = UnityEditor.AssetDatabase.LoadAssetAtPath(settingspathwithname, typeof(UnityEngine.Object));
            ProjectSettings newsettingsgame = newgamesettings as ProjectSettings;
            gamed.Settings = newsettingsgame;

            UnityEditor.AssetDatabase.SaveAssets();
            if (withnewsubdatabases)
            {
                ReloadDatabase(newgamedatabaseasset);
            }

            return gamed;

        }
        public static void ReloadDatabase(UnityEngine.Object database)
        {
            if (database == null) return;
            IDatabase idatabase = (IDatabase)database;
            if (idatabase == null) return;
            DatabaseID id = idatabase.GetDatabaseEntry();
            switch (id)
            {
                case DatabaseID.Abilities:
                    ReloadDatabase((AbilitiesDatabase)database);
                    break;
                case DatabaseID.Attributes:
                    ReloadDatabase(database as ActorAttributesDatabase);
                    break;
                case DatabaseID.ActorDamageDealers:
                    ReloadDatabase(database as ActorDamageDatabase);
                    break;
                case DatabaseID.Auras:
                    ReloadDatabase(database as AuraDatabase);
                    break;
                case DatabaseID.Classes:
                    ReloadDatabase(database as ActorClassDatabase);
                    break;
                case DatabaseID.EquipmentTraits:
                    ReloadDatabase(database as EquipmentTraitDatabase);
                    break;
                case DatabaseID.GameDatabase:
                    ReloadDatabase(database as GameDatabase);
                    break;
                case DatabaseID.Inventories:
                    ReloadDatabase(database as InventoryDatabase);
                    break;
                case DatabaseID.Items:
                    ReloadDatabase(database as ItemDatabase);
                    break;
                case DatabaseID.LootDrops:
                    ReloadDatabase(database as LootDropsDatabase);
                    break;
                case DatabaseID.Melees:
                    ReloadDatabase(database as MeleeDataDatabase);
                    break;
                case DatabaseID.Projectiles:
                    ReloadDatabase(database as ProjectileDataDatabase);
                    break;
                case DatabaseID.Questchains:
                    ReloadDatabase(database as QuestchainDatabase);
                    break;
                case DatabaseID.Quests:
                    ReloadDatabase(database as QuestDatabase);
                    break;
                case DatabaseID.AuraControllers:
                    ReloadDatabase(database as AuraControllerDatabase);
                    break;
                case DatabaseID.AbilityControllers:
                    ReloadDatabase(database as AbilityControllerDatabase);
                    break;
                case DatabaseID.QuestLogs:
                    ReloadDatabase(database as QuestLogDatabase);
                    break;
                


            }
        }
        public static void ReloadDatabase(MeleeDataDatabase database)
        {
            MeleeDB.AddAllToList(database);
        }
        public static void ReloadDatabase(ProjectileDataDatabase database)
        {
            ProjecileDB.AddAllToList(database);
        }
        public static void ReloadDatabase(ActorDamageDatabase database)
        {
            ActorDamageDB.AddAllToList(database);
        }
        public static void ReloadDatabase(QuestLogDatabase database)
        {
            QuestLogDB.AddAllToList(database);
        }
        public static void ReloadDatabase(AbilityControllerDatabase database)
        {
            AbilityControllersDatabase.AddAllToList(database);
        }
        public static void ReloadDatabase(LootDropsDatabase database)
        {
            LootDatabase.AddAllToList(database);
          
        }
        public static void ReloadDatabase(AuraControllerDatabase database)
        {
            AuraControllersDatabase.AddAllToList(database);
        }
        public static void ReloadDatabase(GameDatabase database)
        {
            ARPGGameDatabase.ReloadGameDatabase(database);
        }
        public static void ReloadDatabase(AbilitiesDatabase database)
        {
            AbilityDatabase.AddAllToList(database);
        }
       
        public static void ReloadDatabase(ActorAttributesDatabase database)
        {
            AttributesDatabase.AddAllToList(database);
        }
        public static void ReloadDatabase(AuraDatabase database)
        {
            AurasDatabase.AddAllToList(database);
        }

        public static void ReloadDatabase(ActorClassDatabase database)
        {
            ClassDatabase.AddAllToList(database);
        }
        public static void ReloadDatabase(InventoryDatabase database)
        {
            InvDatabase.AddAllToList(database);
        }
        public static void ReloadDatabase(ItemDatabase database)
        {
            ItemDB.AddAllToList(database);
        }
        public static void ReloadDatabase(QuestchainDatabase database)
        {
            QuestChainDB.AddAllToList(database);
        }
        public static void ReloadDatabase(QuestDatabase database)
        {
            QuestDB.AddAllToList(database);
        }

        public static void ReloadDatabase(EquipmentTraitDatabase database)
        {
            TraitDB.AddAllToList(database);
        }


        public static void ReloadAll(GameDatabase gamedatabase)
        {
            DatabaseHandler.ReloadDatabase(gamedatabase.Abilities);
            DatabaseHandler.ReloadDatabase(gamedatabase.AbilityControllers);
            DatabaseHandler.ReloadDatabase(gamedatabase.ActorDamageTypes);
            DatabaseHandler.ReloadDatabase(gamedatabase.Attributes);
            DatabaseHandler.ReloadDatabase(gamedatabase.Auras);
            DatabaseHandler.ReloadDatabase(gamedatabase.AuraControllers);
            DatabaseHandler.ReloadDatabase(gamedatabase.Classes);
            DatabaseHandler.ReloadDatabase(gamedatabase.Inventories);
            DatabaseHandler.ReloadDatabase(gamedatabase.Items);
            DatabaseHandler.ReloadDatabase(gamedatabase.Loot);
            DatabaseHandler.ReloadDatabase(gamedatabase.Melee);
            DatabaseHandler.ReloadDatabase(gamedatabase.Projectiles);
            DatabaseHandler.ReloadDatabase(gamedatabase.Traits);
            DatabaseHandler.ReloadDatabase(gamedatabase.Questchains);
            DatabaseHandler.ReloadDatabase(gamedatabase.Quests);
            DatabaseHandler.ReloadDatabase(gamedatabase.QuestLog);

        }

        static string GetImportText()
        {
            string path = EditorUtility.OpenFilePanel("Import", "Assets", "txt");
            string text = System.IO.File.ReadAllText(path);
            //Debug.Log(text);
            //Debug.Log(path);
            return text;
        }

     
        


        public static string GetDBCSV(IDatabase forDatabase)
        {
            switch (forDatabase.GetDatabaseEntry())
            {
                case DatabaseID.Abilities:
                    return GetAbilityDBCSV(forDatabase as AbilitiesDatabase);
                case DatabaseID.ActorDamageDealers:
                    return GetActorDamageDBCSV(forDatabase as ActorDamageDatabase);
                case DatabaseID.Projectiles:
                    return GetProjectileDataDBCSV(forDatabase as ProjectileDataDatabase);
                case DatabaseID.Attributes:
                    return GetActorAttributesDBCSV(forDatabase as ActorAttributesDatabase);
                case DatabaseID.EquipmentTraits:
                    return GetTraitDBCSV(forDatabase as EquipmentTraitDatabase);
                case DatabaseID.Items:
                    return GetItemDBCSV(forDatabase as ItemDatabase);


            }

            return string.Empty;
        }

        public static void ImportDBCSV(IDatabase forDatabase)
        {
            switch (forDatabase.GetDatabaseEntry())
            {
                case DatabaseID.ActorDamageDealers:
                    ImportActorDamageDBCSV(forDatabase as ActorDamageDatabase);
                    break;
                case DatabaseID.Projectiles:
                    ImportProjectileDataDBCSV(forDatabase as ProjectileDataDatabase);
                    break;
                case DatabaseID.Abilities:
                    ImportAbilityDBText(forDatabase as AbilitiesDatabase);
                    break;
                case DatabaseID.Attributes:
                    ImportActorAttributesDBCSV(forDatabase as ActorAttributesDatabase);
                    break;
                case DatabaseID.EquipmentTraits:
                    ImportTraitDataDBCSV(forDatabase as EquipmentTraitDatabase);
                    break;
            }

            AssetDatabase.Refresh();
        }

        static void ImportAbilityDBText(AbilitiesDatabase database)
        {
            string text = GetImportText();
            AbilityDatabaseCSV empty = new AbilityDatabaseCSV(string.Empty, 0, null);
            UnityEngine.JsonUtility.FromJsonOverwrite(text, empty);

            //Debug.Log(empty);

            if (empty.DatabaseID == (int)database.GetDatabaseEntry())
            {
                //unpack
                for (int i = 0; i < empty.Entries.Length; i++)
                {
                    AbilityCSV csv = empty.Entries[i];
                    int id = csv.Entry;
                    Ability ability = database.FindAbilityByID(id);
                    if (ability == null)
                    {
                        Debug.LogWarning("No entry with ID " + id.ToString() + " found in database");
                        continue;
                    }
                    Debug.Log("Found " + ability);
                    ability.Data.Name = csv.Name;
                    ability.AutoName(csv.Name);

                    ability.Data.Description = csv.Description;
                    ability.Data.DamageMultiplier = csv.DamageMultipler;
                    ability.Data.Range = csv.Range;
                    ability.Data.ResourceCost = csv.ResourceCost;

                    ResourceType yourEnum;
                    if (System.Enum.TryParse<ResourceType>(csv.ResourceType, out yourEnum))
                    {
                        ability.Data.ResourceType = yourEnum;
                    }

                }
            }
        }

        static void ImportActorAttributesDBCSV(ActorAttributesDatabase database)
        {
            string textfile = EditorUtility.OpenFilePanel("Open File", "Assets", "txt");
            if (textfile.Length > 0)
            {
                ActorAttributesDatabaseCSV loadCSV = new ActorAttributesDatabaseCSV(null, 0, null);
                string read = System.IO.File.ReadAllText(textfile);
                UnityEngine.JsonUtility.FromJsonOverwrite(read, loadCSV);

                if (loadCSV.DatabaseID == (int)database.GetDatabaseEntry())
                {
                    //valid
                    for (int i = 0; i < loadCSV.Entries.Length; i++)
                    {
                        ActorAttributesCSV csv = loadCSV.Entries[i];
                        int id = csv.Entry;
                        ActorAttributes data = database.FindActorStatsByID(id);
                        if (data != null)
                        {
                            data.ActorName = csv.Name;
                            Dictionary<AttributeType, Attribute[]> all = data.GetAllAttributesByType();
                            AttributeCSVValue[] values = csv.AttributeValues;
                            for (int j = 0; j < values.Length; j++)
                            {
                                AttributeCSVValue value = values[j];
                                AttributeType type = (AttributeType)value.AttributeCategoryType;
                                int starting = value.StartingValue;
                                int max = value.MaxValue;
                                int subtype = value.AttributeSubType;

                                all.TryGetValue(type, out Attribute[] attvalues);
                                for (int k = 0; k < attvalues.Length; k++)
                                {
                                    if (attvalues[k].GetSubType() == subtype)
                                    {
                                        //found it
                                        attvalues[k].Level1Value = starting;
                                        attvalues[k].Level99Max = max;
                                    }
                                }
                            }


                            EditorUtility.SetDirty(data);
                        }
                        else
                        {
                            //print some warning
                        }
                    }
                }

            }
        }
        static void ImportActorDamageDBCSV(ActorDamageDatabase database)
        {
            string textfile = EditorUtility.OpenFilePanel("Open File", "Assets", "txt");
            if (textfile.Length > 0)
            {
                ActorDamageDatabaseCSV loadCSV = new ActorDamageDatabaseCSV(null, 0, null);
                string read = System.IO.File.ReadAllText(textfile);
                UnityEngine.JsonUtility.FromJsonOverwrite(read, loadCSV);

                if (loadCSV.DatabaseID == (int)database.GetDatabaseEntry())
                {
                    //valid
                    for (int i = 0; i < loadCSV.Entries.Length; i++)
                    {
                        int id = loadCSV.Entries[i].Entry;
                        ActorDamageData data = database.FindInstanceByID(id);
                        if (data != null)
                        {
                            DamageOptions newoptions = loadCSV.Entries[i].DamageOptions;
                            DamageMultiplers_Actor importdm = loadCSV.Entries[i].DamageMultiplers;
                            StatusOverTimeOptions sotoptions = loadCSV.Entries[i].SoTOptions;
                            DamageOverTimeMultipliers sotmulties = loadCSV.Entries[i].SoTMultipliers;
                            CombatFormulas handler = data.DamageVar.CombatHandler;
                            string newname = loadCSV.Entries[i].Name;
                            data.DamageVar = new DamageDealerForActor(newname, handler, newoptions, importdm, sotoptions, sotmulties);
                            EditorUtility.SetDirty(data);
                        }
                        else
                        {
                            //print some warning
                        }
                    }
                }

            }
        }
        static void ImportProjectileDataDBCSV(ProjectileDataDatabase database)
        {
            string textfile = EditorUtility.OpenFilePanel("Open File", "Assets", "txt");
            if (textfile.Length > 0)
            {
                ProjectileDataDatabaseCSV loadCSV = new ProjectileDataDatabaseCSV(null, 0, null);
                string read = System.IO.File.ReadAllText(textfile);
                UnityEngine.JsonUtility.FromJsonOverwrite(read, loadCSV);

                if (loadCSV.DatabaseID == (int)database.GetDatabaseEntry())
                {
                    //valid
                    for (int i = 0; i < loadCSV.Entries.Length; i++)
                    {
                        int id = loadCSV.Entries[i].Entry;
                        ProjectileData data = database.FindInstanceByID(id);
                        if (data != null)
                        {
                            ProjectileOptions impot = loadCSV.Entries[i].Options;
                            data.ProjectileVars = impot;
                            EditorUtility.SetDirty(data);
                        }
                        else
                        {
                            //print some warning
                        }
                    }
                }

            }
        }

        static void ImportTraitDataDBCSV(EquipmentTraitDatabase database)
        {
            string textfile = EditorUtility.OpenFilePanel("Open File", "Assets", "txt");
            if (textfile.Length > 0)
            {
                TraitDatabaseCSV loadCSV = new TraitDatabaseCSV(null, 0, null);
                string read = System.IO.File.ReadAllText(textfile);
                UnityEngine.JsonUtility.FromJsonOverwrite(read, loadCSV);

                if (loadCSV.DatabaseID == (int)database.GetDatabaseEntry())
                {
                    //valid
                    for (int i = 0; i < loadCSV.Entries.Length; i++)
                    {
                        int id = loadCSV.Entries[i].Entry;
                        EquipmentTrait data = database.FindTraitByID(id);
                        if (data != null)
                        {
                            TraitDatabaseCSVValue load = loadCSV.Entries[i].Value;

                            if (load.TraitCategoryType != (int)data.GetTraitType())
                            {
                                //warnign message, dont do the override unless permission granted
                            }

                            data.SetTraitName(load.TraitName);
                            data.SetPrefixes(load.Prefixes);
                            data.SetSuffixes(load.Suffixes);
                            data.SetMulti(load.ILevelMulti);
                            data.SetWeight(load.Weight);

                          
                            EditorUtility.SetDirty(data);
                        }
                        else
                        {
                            //print some warning
                        }
                    }
                }

            }
        }

        static string GetItemDBCSV(ItemDatabase database)
        {

            ItemCSV[] ntriesCSV = new ItemCSV[database.Slots.Length];
            for (int i = 0; i < database.Slots.Length; i++)
            {
                Item instance = database.Slots[i].Item;

                ItemCSV newcsv = new ItemCSV();

                newcsv.Entry = instance.GetID().ID;
                newcsv.Name = instance.GetID().Name;
                ItemCSVValue value = new ItemCSVValue(
                    instance.GetItemType().ToString(),
                    (int)instance.GetItemType(),
                    instance.GetGeneratedItemName());
                newcsv.ItemValue = value;

                ItemType type = instance.GetItemType();
                switch (type)
                {
                    case ItemType.Equipment:
                        Equipment equipment = instance as Equipment;

                        EquipmentType eqtype = equipment.GetEquipmentType();
                        switch (eqtype)
                        {
                            case EquipmentType.Weapon:
                                Weapon weapon = equipment as Weapon;
                                WeaponType wtype = weapon.GetWeaponType();

                                break;
                        }

                        EquipmentSlotsType[] slots = equipment.GetEquipmentSlot();
                        string[] slotnames = new string[slots.Length];
                        int[] slottypes = new int[slots.Length];
                        for (int j = 0; j < slots.Length; j++)
                        {
                            slotnames[j] = slots[j].ToString();
                            slottypes[j] = (int)slots[j];
                        }
                        EquipmentCSVValue eqvalue = new EquipmentCSVValue(
                            equipment.GetEquipmentType().ToString(),
                            (int)equipment.GetEquipmentType(),
                            equipment.GetGeneratedItemName(),
                            slotnames,
                            slottypes,
                            equipment.GetStats().GetBaseType().ToString(),
                            (int)equipment.GetStats().GetBaseType(),
                            equipment.GetStats().GetIlevel());

                        newcsv.EquipmentValue = eqvalue;
                        break;
                    case ItemType.Potions:
                        Potion potion = instance as Potion;
                        PotionCSVValue potvalue = new PotionCSVValue(
                            potion.GetPotionType().ToString(),
                            (int)potion.GetPotionType());
                        newcsv.PotionValue = potvalue;
                        break;

                }

                ntriesCSV[i] = newcsv;
            }

            ItemDatabaseCSV csvEntries = new ItemDatabaseCSV(database.GetDatabaseEntry().ToString(), (int)database.GetDatabaseEntry(), ntriesCSV);
            string savedJson = UnityEngine.JsonUtility.ToJson(csvEntries, true);


            WriteFile(savedJson, database.name);



            return savedJson;
        }
        static string GetTraitDBCSV(EquipmentTraitDatabase database)
        {

            TraitCSV[] ntriesCSV = new TraitCSV[database.Slots.Length];
            for (int i = 0; i < database.Slots.Length; i++)
            {
                EquipmentTrait instance = database.Slots[i].Trait;

                TraitCSV newcsv = new TraitCSV();

                newcsv.Entry = instance.GetID().ID;
                newcsv.Name = instance.GetID().Name;
                TraitDatabaseCSVValue value = new TraitDatabaseCSVValue(
                    instance.GetTraitType().ToString(),
                    (int)instance.GetTraitType(),
                    instance.GetTraitName(),
                    instance.GetPrefixes(),
                    instance.GetSuffixes(),
                    instance.GetMyLevelMultRaw(),
                    instance.GetWeight());

                newcsv.Value = value;


                ntriesCSV[i] = newcsv;
            }

            TraitDatabaseCSV csvEntries = new TraitDatabaseCSV(database.GetDatabaseEntry().ToString(), (int)database.GetDatabaseEntry(), ntriesCSV);
            string savedJson = UnityEngine.JsonUtility.ToJson(csvEntries, true);


            WriteFile(savedJson, database.name);



            return savedJson;
        }

        static string GetActorDamageDBCSV(ActorDamageDatabase database)
        {

            ActorDamageCSV[] ntriesCSV = new ActorDamageCSV[database.Slots.Length];
            for (int i = 0; i < database.Slots.Length; i++)
            {
                ActorDamageData instance = database.Slots[i].Instance;

                ActorDamageCSV newcsv = new ActorDamageCSV();

                newcsv.Entry = instance.GetID().ID;
                newcsv.Name = instance.GetName();
                newcsv.DamageOptions = instance.DamageVar.DamageOptions;
                newcsv.DamageMultiplers = instance.DamageVar.DamageMultipliers;
                newcsv.SoTOptions = instance.DamageVar.SoTOptions;
                newcsv.SoTMultipliers = instance.DamageVar.SoTOverTimeMultipliers;
                //save what i want here



                ntriesCSV[i] = newcsv;
            }

            ActorDamageDatabaseCSV csvEntries = new ActorDamageDatabaseCSV(database.GetDatabaseEntry().ToString(), (int)database.GetDatabaseEntry(), ntriesCSV);
            string savedJson = UnityEngine.JsonUtility.ToJson(csvEntries, true);


            WriteFile(savedJson, database.name);


            return savedJson;
        }
        static string GetProjectileDataDBCSV(ProjectileDataDatabase database)
        {

            ProjectileDataCSV[] ntriesCSV = new ProjectileDataCSV[database.Slots.Length];
            for (int i = 0; i < database.Slots.Length; i++)
            {
                ProjectileData instance = database.Slots[i].Instance;

                ProjectileDataCSV newcsv = new ProjectileDataCSV();

                newcsv.Entry = instance.GetID().ID;
                newcsv.Options = new ProjectileOptions
                    (
                    instance.ProjectileVars.Name,
                    instance.ProjectileVars.Description,
                    instance.ProjectileVars.Speed,
                    instance.ProjectileVars.DisableOnTouch,
                    instance.ProjectileVars.LifeTime,
                    instance.ProjectileVars.FriendlyFire
                    );

                //save what i want here



                ntriesCSV[i] = newcsv;
            }

            ProjectileDataDatabaseCSV csvEntries = new ProjectileDataDatabaseCSV(database.GetDatabaseEntry().ToString(), (int)database.GetDatabaseEntry(), ntriesCSV);
            string savedJson = UnityEngine.JsonUtility.ToJson(csvEntries, true);
            WriteFile(savedJson, database.name);
            return savedJson;
        }

       

        static string GetActorAttributesDBCSV(ActorAttributesDatabase database)
        {
      

            ActorAttributesCSV[] ntriesCSV = new ActorAttributesCSV[database.Slots.Length];
            for (int i = 0; i < database.Slots.Length; i++)
            {
                ActorAttributes instance = database.Slots[i].ActorStats;

                ActorAttributesCSV newcsv = new ActorAttributesCSV();

                newcsv.Entry = instance.GetID().ID;
                newcsv.Name = instance.ActorName;
                Dictionary<AttributeType, Attribute[]> attributes = instance.GetAllAttributesByType();
                List<AttributeCSVValue> _temp = new List<AttributeCSVValue>();
                foreach (var kvp in attributes)
                {
                    int id = (int)kvp.Key;
                    string attname = kvp.Key.ToString();
                    for (int j = 0; j < kvp.Value.Length; j++)
                    {
                        int starting = kvp.Value[j].Level1Value;
                        int max = kvp.Value[j].Level99Max;
                        int subtype = kvp.Value[j].GetSubType();
                        string attributeName = kvp.Value[j].GetDescriptiveName();
                        Attribute attribute = kvp.Value[j];
                       
                        AttributeCSVValue newvalues = new AttributeCSVValue(attname, id, attributeName, subtype, starting, max);
                        _temp.Add(newvalues);
                    }

                }

                newcsv.AttributeValues = _temp.ToArray();
                //save what i want here
                ntriesCSV[i] = newcsv;
            }

            ActorAttributesDatabaseCSV csvEntries = new ActorAttributesDatabaseCSV(database.GetDatabaseEntry().ToString(), (int)database.GetDatabaseEntry(), ntriesCSV);
            string savedJson = UnityEngine.JsonUtility.ToJson(csvEntries, true);

            string name = database.name;
            WriteFile(savedJson, name);

            return savedJson;
        }
        static string GetAbilityDBCSV(AbilitiesDatabase database)
        {

            AbilityCSV[] ntriesCSV = new AbilityCSV[database.Slots.Length];
            for (int i = 0; i < database.Slots.Length; i++)
            {
                Ability ability = database.Slots[i].Ability;

                AbilityCSV newcsv = new AbilityCSV();

                newcsv.Entry = ability.GetID().ID;
                newcsv.Name = ability.Data.Name;
                newcsv.Description = ability.Data.Description;
                newcsv.DamageMultipler = ability.Data.DamageMultiplier;
                newcsv.Range = ability.Data.Range;
                newcsv.ResourceCost = ability.Data.ResourceCost;
                newcsv.ResourceType = ability.Data.ResourceType.ToString();
                //save what i want here



                ntriesCSV[i] = newcsv;
            }

            AbilityDatabaseCSV csvEntries = new AbilityDatabaseCSV(database.GetDatabaseEntry().ToString(), (int)database.GetDatabaseEntry(), ntriesCSV);
            string savedJson = UnityEngine.JsonUtility.ToJson(csvEntries, true);

            string name = database.name;
            WriteFile(savedJson, name);

            return savedJson;
        }

        private static void WriteFile(string savedJson, string name)
        {
            string newPath = EditorUtility.SaveFilePanelInProject("Export Text", name + "_Export", "txt", "Export?");
            if (newPath.Length > 0)
            {
                System.IO.File.WriteAllText(newPath, savedJson);
            }

            AssetDatabase.SaveAssets();

        }
    }

  
}
#endif