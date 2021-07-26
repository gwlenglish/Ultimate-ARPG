
using UnityEngine;

namespace GWLPXL.NoFrills.Modifiers.com
{

    /// <summary>
    /// Abstract class to derive all Mods from. Use the GenericModifier or extend to create your own. See example of the DemoModifier. 
    /// </summary>
    [System.Serializable]
    public abstract class ModBase
    {
        [Tooltip("When percent, will take the amount and divide by one hundred. 25/100 = .25.")]
        [SerializeField]
        protected int Amount = 0;
        [SerializeField]
        protected ModValueType type = ModValueType.Flat;
        protected int amountApplied = 0;
        protected int attributeApplied = 0;
        public abstract void SetAttributeToModify(int enumKey);
        public abstract int GetAttributeToModify();
        public virtual int GetAmount()
        {
            return Amount;
        }
        public virtual void SetAmount(int newAmount)
        {
            Amount = newAmount;
        }
        public virtual int GetAppliedValue()
        {
            return amountApplied;
        }
        public virtual int GetAppliedAttribute()
        {
            return attributeApplied;
        }
        public virtual ModValueType GetModType()
        {
            return type;
        }
        public virtual void SetModType(ModValueType newType)
        {
            type = newType;
        }

        public virtual int ApplyModValue(int baseValue)
        {


            switch (GetModType())
            {
                case ModValueType.Percent:
                    amountApplied = Mathf.FloorToInt(baseValue * (1 + ((float)Amount / 100f)) - baseValue);//10 * 1 + .1 - 10 = .1.
                    break;
                case ModValueType.Flat:
                    amountApplied = Amount;
                    break;
            }

            Debug.Log("Amount applied " + amountApplied);
            return amountApplied;
        }


    }
}