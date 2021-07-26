using GWLPXL.ARPG._Scripts.Attributes.com;
using GWLPXL.ARPGCore.Types.com;
using UnityEngine;

namespace GWLPXL.ARPGCore.Auras.com
{
    [CreateAssetMenu(menuName = "GWLPXL/ARPG/Auras/Logic/NEW ARPG Increase Max Resource In Percent")]
    public class ARPG_IncreaseMaxResourceInPercent : AuraLogic
    {
        public AttributeModifier attributeModifier = new AttributeModifier(1, StatModType.PercentAdd);
        [SerializeField]
        ResourceType type = ResourceType.Health;
        public override bool DoApplyLogic(ITakeAura onUser)
        {
            if (onUser == null) return false;

            onUser.AuraApplyModifierResource((int)type, attributeModifier);
            return true;
        }

        public override bool DoRemoveLogic(ITakeAura fromUser)
        {
            if (fromUser == null) return false;

            fromUser.AuraRemoveModifierResource((int)type, attributeModifier);
            return true;
        }
    }
}