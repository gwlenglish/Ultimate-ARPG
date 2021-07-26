#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using GWLPXL.NoFrills.Common.com;

namespace GWLPXL.NoFrills.Modifiers.com
{

    [CustomEditor(typeof(Modifiable), true)]
    public class ModifiableEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            //GUILayout.Space(25);
            //Modifiable holder = (Modifiable)target;
            //if (GUILayout.Button("Save as NEW Json Config"))
            //{
            //    JsconConfig.SaveJson(holder);
            //}
            //if (GUILayout.Button("Load Json Config"))
            //{
            //    JsconConfig.LoadJson(holder);
            //}
            //if (GUILayout.Button("Overwrite Json Config"))
            //{
            //    JsconConfig.OverwriteJson(holder);
            //}
        }
    }
}
#endif