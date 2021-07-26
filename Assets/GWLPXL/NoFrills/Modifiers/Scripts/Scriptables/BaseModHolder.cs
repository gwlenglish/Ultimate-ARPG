

using GWLPXL.ARPGCore.Statics.com;
using UnityEngine;
using System.Collections.Generic;

namespace GWLPXL.NoFrills.Modifiers.com
{
    /// <summary>
    /// Base abstract class for the modifier holder. Extend this or use the Generic one to create instances. 
    /// </summary>
    /// 
    [System.Serializable]
    public abstract class BaseModHolder : ScriptableObject, ISaveJsonConfig
    {
        [SerializeField]
        protected TextAsset config = null;
        [SerializeField]
        protected ModifierHolderData modifierData;

        public virtual ModifierHolderData GetMyData()
        {
            return modifierData;
        }
        
        public abstract IList<ModBase> GetAllModifiers();//use array as List isn't so nice with deriving types
        public abstract int GetAttributeType();
        public void SetTextAsset(TextAsset textAsset)
        {
            config = textAsset;
        }

        public TextAsset GetTextAsset()
        {
            return config;
        }

        public Object GetObject()
        {
            return this;
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (modifierData.AutoName && modifierData.Name != string.Empty)
            {
                string path = UnityEditor.AssetDatabase.GetAssetPath(this);
                UnityEditor.AssetDatabase.RenameAsset(path, modifierData.Name + "_" + GetAttributeType().ToString());
            }

            if (modifierData.AutoAssignUniqueID)
            {
                modifierData.UniqueID = this.GetInstanceID();
            }
        }

     
#endif
    }
}