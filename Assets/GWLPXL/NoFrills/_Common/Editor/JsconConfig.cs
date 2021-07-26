#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;

namespace GWLPXL.NoFrills.Common.com
{
    /// <summary>
    /// Editor only saving and loading from json
    /// </summary>
    public static class JsconConfig 
    {
        static string extension = ".json";

        public static string[] FindAllHoldersOfType(ISaveJsonConfig holder)
        {
            string typeKey = holder.GetObject().GetType().Name;
            string[] guids = AssetDatabase.FindAssets("t:" + typeKey, null);
            return guids;
        }

        public static void OverwriteJson(ISaveJsonConfig holder)
        {
            if (File.Exists(AssetDatabase.GetAssetPath(holder.GetTextAsset())))
            {
                TextAsset textasset = holder.GetTextAsset();

                string path = AssetDatabase.GetAssetPath(textasset);
                string savedJson = JsonUtility.ToJson(holder.GetObject());
                File.WriteAllText(path, savedJson);

                holder.SetTextAsset(textasset);
                AssetDatabase.Refresh();
            }
        }

        public static void SaveJson(ISaveJsonConfig holder)
        {
            string dpath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(holder.GetObject()));
            string exportName = "\\" + holder.GetObject().name + extension;

            RenameOrDeleteOld(dpath, exportName);

            string savedJson = JsonUtility.ToJson(holder);
            File.WriteAllText(dpath + exportName, savedJson);
        
            AssetDatabase.Refresh();

            TextAsset created = AssetDatabase.LoadAssetAtPath(dpath + exportName, typeof(UnityEngine.Object)) as TextAsset;
            holder.SetTextAsset(created);

            //necessary to save the instance id of inherited types from abstract
            OverwriteJson(holder);
        }

        public static void LoadJson(ISaveJsonConfig holder)
        {
            if (File.Exists(AssetDatabase.GetAssetPath(holder.GetTextAsset())))
            {
                TextAsset textasset = holder.GetTextAsset();
                string textFile = File.ReadAllText(AssetDatabase.GetAssetPath(holder.GetTextAsset()));
                JsonUtility.FromJsonOverwrite(textFile, holder.GetObject());
                holder.SetTextAsset(textasset);
                AssetDatabase.Refresh();
            }
        }
        static void RenameOrDeleteOld(string dpath, string exportName)
        {
            //try to move the old file. 
            if (File.Exists(dpath + exportName))
            {
                //only allows up to save the two versions, the old and then the new
                if (File.Exists(dpath + "\\" + "_old" + extension))
                {
                    File.Delete(dpath + "\\" + "_old" + extension);
                }
                File.Move(dpath + exportName, dpath + "\\" + "_old" + extension);
            }
        }
    }
}

#endif