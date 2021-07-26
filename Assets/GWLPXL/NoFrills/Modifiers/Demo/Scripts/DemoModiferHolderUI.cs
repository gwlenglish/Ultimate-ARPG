
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GWLPXL.NoFrills.Modifiers.com
{

    /// <summary>
    /// Example of how to add Generic Modifier Holders.
    /// </summary>
    public class DemoModiferHolderUI : MonoBehaviour
    {
        public Modifiable Modifiable;
        public GenericModifierHolder ModHolder;
        [Tooltip("used to reset the values on the blank mods, the ones we can change in the inspector.")]
        public bool ResetMod = false;
        public Text Description;
        [Tooltip("If true, will create a copy and add it to the modifiable. Will result in being able to add as many as you like.")]
        public bool Repeatable = false;

        bool repeatableBacking;
        int newValue = 0;
        int enumValue = 0;
        List<GenericModifierHolder> copies = new List<GenericModifierHolder>();

        private void Start()
        {
            repeatableBacking = Repeatable;
            if (ResetMod)
            {
                SetNewFlatModEnumValue(0);
                ModHolder.GetAllModifiers()[0].SetAmount(0);
                ModHolder.GetAllModifiers()[0].SetAttributeToModify(0);
                SetNewModTypeEnum(0);
            }


            Description.text = ModHolder.GetMyData().Description;
        }
        public void SetNewModTypeEnum(int enumType)
        {
            ModHolder.GetAllModifiers()[0].SetModType((ModValueType)enumType);

        }

        public void SetNewFlatModValue(string _newValue)
        {
            Int32.TryParse(_newValue, out int result);
            newValue = result;
            ModHolder.GetAllModifiers()[0].SetAmount(newValue);

        }
        public void SetNewFlatModEnumValue(int _newValue)
        {
            enumValue = _newValue;
            ModHolder.GetAllModifiers()[0].SetAttributeToModify(enumValue);

        }

        public void AddMod()
        {
            if (repeatableBacking)
            {
                //to add copies of the one
                GenericModifierHolder newCopy = ScriptableObject.Instantiate(ModHolder);//create a copy
                Modifiable.AddModifier(newCopy);//add the copy
                copies.Add(newCopy);//important to maintain a reference to the copy
            }
            else
            {
               //to add just the one
                Modifiable.AddModifier(ModHolder);
            }
        }

        public void RemoveMod()
        {
            if (repeatableBacking)
            {
                //to add copies of the one
                if (copies.Count == 0) return;//nothing to remove

                GenericModifierHolder lastAdded = copies[copies.Count - 1];//grab the last one added
                Modifiable.RemoveModifier(lastAdded);//remove it
                copies.RemoveAt(copies.Count - 1);//remove it from the list
            }
            else
            {
                //to add just the one
                Modifiable.RemoveModifier(ModHolder);
            }

        }

    }
}