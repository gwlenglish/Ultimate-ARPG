#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using GWLPXL.NoFrills.Common.com;

namespace GWLPXL.NoFrills.Modifiers.com
{

    [CustomEditor(typeof(BaseModHolder), true)]
    public class BaseModHolderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(25);
            BaseModHolder holder = (BaseModHolder)target;
            if (GUILayout.Button("Save as NEW Json Config"))
            {
                JsconConfig.SaveJson(holder as ISaveJsonConfig);
            }
            if (GUILayout.Button("Load Json Config"))
            {
                JsconConfig.LoadJson(holder as ISaveJsonConfig);
            }
            if (GUILayout.Button("Overwrite Json Config"))
            {
                JsconConfig.OverwriteJson(holder as ISaveJsonConfig);
            }
        }
    }
}
#endif