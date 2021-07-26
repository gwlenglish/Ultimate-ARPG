
using System.Collections.Generic;
using GWLPXL.ARPG._Scripts.Attributes.com;
using GWLPXL.ARPGCore.Types.com;
using UnityEngine;

namespace GWLPXL.ARPGCore.Attributes.com
{
    /// <summary>
    /// base attribute class for the actor atttributes (strength, hp, etc)
    /// </summary>
    [System.Serializable]
    public abstract class Attribute
    {
	    public int BaseValue = 0;
        public int Level1Value = 0;
        [Tooltip("Stats and Resources use this, Elements currently do not")]
        public AnimationCurve LevelCurve = default;
        public int Level99Max = 100;
        
        public int NowValue = 0;

        private List<AttributeModifier> _statModifiers;
        public List<AttributeModifier> StatModifiers
        {
	        get
	        {
		        if (_statModifiers == null)
			        _statModifiers = new List<AttributeModifier>();
		        return _statModifiers;
	        }
        }

        public abstract AttributeType GetAttributeType();
        public abstract int GetSubType();
        public abstract string GetDescriptiveName();
        public abstract string GetFullDescription();
        protected virtual int GetLeveledValue(int forLevel, int myMaxLevel)
        {
            float leveledStat = Mathf.Lerp(Level1Value, Level99Max, (float)forLevel / (float)myMaxLevel); //default is linear lerp
            if (LevelCurve != null)
            {
                float percent = LevelCurve.Evaluate((float)forLevel / (float)myMaxLevel);//get the curve
                leveledStat = Mathf.Lerp((float)Level1Value, (float)Level99Max, percent);//find the new stat on the curve
            }
            int rounded = Mathf.FloorToInt(leveledStat);//this is returning back, rounded down

            return rounded;
        }
        
        // todo Yashik: needs check, if base value is modified by another source, we can get negative-value levelup
		public virtual void Level(int newLevel, int maxLevel)
		{
			int current = BaseValue;
			int newvalue = GetLeveledValue(newLevel, maxLevel);
			ModifyBaseValue(newvalue + -current);
		}
		
		public virtual void ModifyBaseValue(int byHowMuch)
		{
			int newValue = BaseValue + byHowMuch;
			SetBaseValue(newValue);
		}

		public virtual void SetBaseValue(int newValue)
		{
			BaseValue = newValue;
			UpdateValue();
		}

		public virtual void AddModifier(AttributeModifier mod)
		{
			//isDirty = true;
			StatModifiers.Add(mod);
			UpdateValue();
		}

		public virtual bool RemoveModifier(AttributeModifier mod)
		{
			if (StatModifiers.Remove(mod))
			{
				// isDirty = true;
				UpdateValue();
				return true;
			}
			return false;
		}

		public virtual bool RemoveAllModifiersFromSource(object source)
		{
			int numRemovals = StatModifiers.RemoveAll(mod => mod.Source == source);

			if (numRemovals > 0)
			{
				//isDirty = true;
				UpdateValue();
				return true;
			}
			return false;
		}

		protected virtual void UpdateValue()
		{
			NowValue = CalculateFinalValue();
		}

		protected virtual int CompareModifierOrder(AttributeModifier a, AttributeModifier b)
		{
			if (a.Order < b.Order)
				return -1;
			if (a.Order > b.Order)
				return 1;
			return 0;
		}
		
		protected virtual int CalculateFinalValue()
		{
			float finalValue = BaseValue;
			float sumPercentAdd = 0;

			StatModifiers.Sort(CompareModifierOrder);

			for (int i = 0; i < StatModifiers.Count; i++)
			{
				AttributeModifier mod = StatModifiers[i];

				if (mod.Type == StatModType.Flat)
				{
					finalValue += mod.Value;
				}
				else if (mod.Type == StatModType.PercentAdd)
				{
					sumPercentAdd += mod.Value;

					if (i + 1 >= StatModifiers.Count || StatModifiers[i + 1].Type != StatModType.PercentAdd)
					{
						finalValue *= 1 + sumPercentAdd;
						sumPercentAdd = 0;
					}
				}
				else if (mod.Type == StatModType.PercentMult)
				{
					finalValue *= 1 + mod.Value;
				}
			}
			return Mathf.FloorToInt(finalValue);
		}

    }
}