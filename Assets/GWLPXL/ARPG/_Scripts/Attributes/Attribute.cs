
using GWLPXL.ARPGCore.Types.com;
using GWLPXL.NoFrills.Modifiers.com;
using UnityEngine;
using System.Collections.Generic;
namespace GWLPXL.ARPGCore.Attributes.com
{
    /// <summary>
    /// base attribute class for the actor atttributes (strength, hp, etc)
    /// </summary>
    [System.Serializable]
    public abstract class Attribute
    {

        public int NowValue = 0;
        public int Basevalue = 0;
        public int Level1Value = 0;
        [Tooltip("Stats and Resources use this, Elements currently do not")]
        public AnimationCurve LevelCurve = default;
        public int Level99Max = 100;
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
        public virtual void Level(int newLevel, int maxLevel)
        {
            int current = Basevalue;
            int newvalue = GetLeveledValue(newLevel, maxLevel);
            SetBaseValue(newvalue);
            ModifyBaseValue(newvalue + -current);
        }

        public virtual void ModifyBaseValue(int byHowMuch)
        {
            int newValue = Basevalue + byHowMuch;
            //Debug.Log("new value, modify value" + newValue + " " + byHowMuch);
            SetBaseValue(newValue);
        }



        public virtual void SetBaseValue(int newValue)
        {

            Basevalue = newValue;

            //update now value


        }

    }
}