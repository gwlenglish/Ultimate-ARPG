
using GWLPXL.ARPGCore.Attributes.com;
using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Types.com;
using UnityEngine;

namespace GWLPXL.ARPGCore.Traits.com
{

    [CreateAssetMenu(menuName = "GWLPXL/ARPG/Equipment/Traits/NEW_ElementResistTrait")]

    [System.Serializable]
    public class ElementResistTrait : EquipmentTrait
    {
        public ElementType Type;

        protected TraitType type = TraitType.ElementResist;



        public override void ApplyTrait(IAttributeUser toActor)
        {
            toActor.GetRuntimeAttributes().ModifyElementResistNowValue(Type, GetLeveledValue());
        }



        public override TraitType GetTraitType()
        {
            return type;
        }


        public override void RemoveTrait(IAttributeUser toActor)
        {
            toActor.GetRuntimeAttributes().ModifyElementResistNowValue(Type, -GetLeveledValue());
        }


    }
}