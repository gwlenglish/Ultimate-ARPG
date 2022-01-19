using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace GWLPXL.ARPGCore.com
{


    public abstract class FlippedEditor : Editor
    {
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

        protected abstract void ShowEditorVersion();
        protected abstract void ShowRuntimeVersion();
    }
}