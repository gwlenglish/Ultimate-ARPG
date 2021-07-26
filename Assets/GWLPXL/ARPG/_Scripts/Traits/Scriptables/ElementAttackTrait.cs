
using GWLPXL.ARPGCore.Attributes.com;
using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Types.com;
using UnityEngine;

namespace GWLPXL.ARPGCore.Traits.com
{

    [CreateAssetMenu(menuName = "GWLPXL/ARPG/Equipment/Traits/NEW_ElementAttackTrait")]

    [System.Serializable]
    public class ElementAttackTrait : EquipmentTrait
    {
        public ElementType Type;
        protected TraitType type = TraitType.ElementAttack;
        int addedvalue = 0;
        public override void ApplyTrait(IAttributeUser toActor)
        {
            addedvalue = GetLeveledValue();
            toActor.GetRuntimeAttributes().ModifyElementAttackBaseValue(Type, addedvalue);

        }

        public override void RemoveTrait(IAttributeUser toActor)
        {
            toActor.GetRuntimeAttributes().ModifyElementAttackBaseValue(Type, -addedvalue);
            addedvalue = 0;

        }



        public override TraitType GetTraitType()
        {
            return type;
        }


       

    }


}