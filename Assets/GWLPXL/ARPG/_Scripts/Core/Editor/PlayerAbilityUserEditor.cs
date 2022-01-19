using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GWLPXL.ARPGCore.com;
namespace GWLPXL.ARPGCore.Abilities.com
{

    [CustomEditor(typeof(PlayerAbilityUser))]
    public class PlayerAbilityUserEditor : Editor
    {

        EditorInspectorDraw runtimeed;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            if (Application.isPlaying)
            {
                ShowRuntimeVersion();
            }
            else
            {
                ShowEditorVersion();
            }
            serializedObject.ApplyModifiedProperties();
        }
        protected void ShowEditorVersion()
        {
            base.OnInspectorGUI();
        }

        protected void ShowRuntimeVersion()
        {
            PlayerAbilityUser ability = (PlayerAbilityUser)target;
            SerializedProperty runtime = serializedObject.FindProperty("runtime");
            if (runtimeed == null)
            {
                runtimeed = EditorHelper.CreateEditor();
                
            }
           
            runtimeed.Draw(ability.GetRuntimeController());

        }
    }
}
