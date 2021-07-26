

using System.Collections.Generic;

using UnityEngine;

namespace GWLPXL.NoFrills.Modifiers.com
{
    /// <summary>
    /// Everything that can be modded needs a Modifiable, this tracks what mods are currently applied. 
    /// </summary>
    [System.Serializable]
    public class Modifiable
    {
        [Tooltip("The limits are the int min and max. Uses a long to compare.")]
        public int ModCapUpperLimit = int.MaxValue;
        public int ModCapLowerLimit = int.MinValue;
        [System.NonSerialized]
        protected Dictionary<int, ModifiableDictionary> modifiabledictionaries = new Dictionary<int, ModifiableDictionary>();


        public virtual IReadOnlyDictionary<int, ModifiableDictionary> GetAllModifiers()
        {
            return modifiabledictionaries;
        }
        /// <summary>
        /// Base value is used to apply percents, so 10% mod would be 10% of of the base value. It's not used for flat values.
        /// </summary>
        /// <param name="forType"></param>
        /// <param name="whichEnum"></param>
        /// <param name="baseValue"></param>
        /// <returns></returns>
        public virtual int GetModValue(int forType, int whichEnum, int baseValue)
        {
            ModifiableDictionary value = GetDicValues(forType);
            return GetModValue(value, whichEnum, baseValue);
        }
        /// <summary>
        /// returns modifiers
        /// </summary>
        /// <param name="whichType"></param>
        /// <param name="whichStat"></param>
        /// <returns></returns>
        public virtual List<ModBase> GetAllMods(int whichType, int whichStat)
        {
            ModifiableDictionary value = GetDicValues(whichType);
            List<ModBase> mods = GetMods(whichStat, value);
            return mods;
        }
        /// <summary>
        /// Removes the ModHolder and its associated mods based on Unique ID. 
        /// </summary>
        /// <param name="newMod"></param>
        public virtual bool RemoveModifier(BaseModHolder newMod)
        {
            ModifiableDictionary value = GetDicValues(newMod.GetAttributeType());
            if (value.ModHoldersDic.ContainsKey(newMod) == false) return false;//don't have it to remove

            IList<ModBase> mods = value.ModHoldersDic[newMod].GetAllModifiers();
            for (int i = 0; i < mods.Count; i++)
            {
                List<ModBase> modList = GetMods(mods[i].GetAppliedAttribute(), value);
                value.ModifiersDic[mods[i].GetAppliedAttribute()] = modList;
                modList.Remove(mods[i]);
            }

            value.ModHoldersDic.Remove(newMod);
            
            modifiabledictionaries[(int)newMod.GetAttributeType()] = value;
  
            value.Dirty = true;
            return true;

        }
        /// <summary>
        /// Returns the amount that were unable to be removed. 
        /// </summary>
        /// <param name="removeMods"></param>
        /// <returns></returns>
        public virtual int RemoveModifiers(BaseModHolder[] removeMods)
        {
            int removed = 0;
            for (int i = 0; i < removeMods.Length; i++)
            {
                if (RemoveModifier(removeMods[i]))
                {
                    removed += 1;
                }
            }
            return removed;
        }
        /// <summary>
        /// Return the amounts that were unable to be added.
        /// </summary>
        /// <param name="newMods"></param>
        /// <returns></returns>
        public virtual int AddModifiers(BaseModHolder[] newMods)
        {
            int leftover = newMods.Length;
            for (int i = 0; i < newMods.Length; i++)
            {
                if (AddModifier(newMods[i]))
                {
                    leftover -= 1;
                }
            }
            return leftover;
        }
        /// <summary>
        /// adds a copy of the modholder based on unique ID. Must have unique ID's to work.
        /// </summary>
        /// <param name="newMod"></param>
        public virtual bool AddModifier(BaseModHolder newMod)
        {
            //grab our dictionaries
            ModifiableDictionary value = GetDicValues(newMod.GetAttributeType());
            if (value.ModHoldersDic.ContainsKey(newMod)) return false;//can't add anymore of this mod, already added

            //make a copy so it won't be affected if the template is modified somehow
            BaseModHolder copy = ScriptableObject.Instantiate(newMod);
            //add it all
            IList<ModBase> mods = copy.GetAllModifiers();
            Debug.Log("Mod Count " + mods.Count);
            for (int i = 0; i < mods.Count; i++)
            {
                List<ModBase> modList = GetMods(mods[i].GetAttributeToModify(), value);
                modList.Add(mods[i]);
                value.ModifiersDic[mods[i].GetAttributeToModify()] = modList;
            }

            //update the dictionaries.
            //value.held.Add(newMod);
            value.ModHoldersDic.Add(newMod, copy);
            value.Dirty = true;
            modifiabledictionaries[(int)newMod.GetAttributeType()] = value;

            return true;
        }
        /// <summary>
        /// Returns the cumulative value of the mods
        /// </summary>
        /// <param name="inDic"></param>
        /// <param name="whichEnum"></param>
        /// <param name="baseValue"></param>
        /// <returns></returns>
        protected virtual int GetModValue(ModifiableDictionary inDic, int whichEnum, int baseValue)
        {
            if (inDic.Dirty == false && inDic.QuickValuesDic.ContainsKey(whichEnum))
            {
                return inDic.QuickValuesDic[whichEnum];
            }
            long moddedValue = 0;//using 64bit long for an easy way to catch limits, wont play nicely with 32bit.
            List<ModBase> valueList = GetMods(whichEnum, inDic);
            Debug.Log("Mod List Count " + valueList.Count);
            for (int i = 0; i < valueList.Count; i++)
            {
                moddedValue += valueList[i].ApplyModValue(baseValue);
            }

            //clamps to stop overflow, int min and max are hard limits
            if (moddedValue > ModCapUpperLimit)
            {
                moddedValue = ModCapUpperLimit;
            }
            else if (moddedValue < ModCapLowerLimit)
            {
                moddedValue = ModCapLowerLimit;
            }
            int cappedMod = (int)moddedValue;
            Debug.Log("Applied value " + moddedValue);
            inDic.QuickValuesDic[whichEnum] = cappedMod;
            inDic.Dirty = false;
            return cappedMod;

        }


        protected virtual List<ModBase> GetMods(int whichStat, ModifiableDictionary value)
        {
            value.ModifiersDic.TryGetValue(whichStat, out List<ModBase> mods);
            if (mods == null)
            {
                mods = new List<ModBase>();
                value.ModifiersDic[whichStat] = mods;
            }

            return mods;
        }
        protected virtual ModifiableDictionary GetDicValues(int whichType)
        {
            modifiabledictionaries.TryGetValue((int)whichType, out ModifiableDictionary value);
            if (value == null)
            {
                value = new ModifiableDictionary();
                modifiabledictionaries[(int)whichType] = value;
            }

            return value;
        }



    }

    #region helpers
    /// <summary>
    /// /dictionary class for the mods, tracking based on modifier Type in Modifiable class.
    /// </summary>
    /// 
    [System.Serializable]
    public class ModifiableDictionary
    {
        /// <summary>
        /// the original template values aren't used, the add method copies the original and we use the copy as the applied values.
        /// </summary>
        public Dictionary<BaseModHolder, BaseModHolder> ModHoldersDic = new Dictionary<BaseModHolder, BaseModHolder>();//orignal template, heldcopy
        public Dictionary<int, List<ModBase>> ModifiersDic = new Dictionary<int, List<ModBase>>();//list of mods on each enum / stat
        public Dictionary<int, int> QuickValuesDic = new Dictionary<int, int>();
        public bool Dirty = true;

        public ModifiableDictionary()
        {
            //held = new List<BaseModHolder>();
            ModHoldersDic = new Dictionary<BaseModHolder, BaseModHolder>();//unique ID, modholder
            ModifiersDic = new Dictionary<int, List<ModBase>>();//list of mods on each enum / stat
            QuickValuesDic = new Dictionary<int, int>();//saved enum/value so doens't recalculate if not dirty

        }
    }
    //save the modholder
    #endregion
}