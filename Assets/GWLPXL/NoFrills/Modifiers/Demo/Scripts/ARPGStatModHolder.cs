using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GWLPXL.NoFrills.Modifiers.com;
using GWLPXL.ARPGCore.Types.com;
namespace GWLPXL.ARPGCore.com
{
    [System.Serializable]
    public class ARPGStatModifier : ModBase
    {
        [SerializeField]
        protected StatType attributeKey;

        public override void SetAttributeToModify(int enumKey)
        {
            attributeKey = (StatType)enumKey;
        }
        public override int GetAttributeToModify()
        {
            return (int)attributeKey;
        }



    }


    [CreateAssetMenu(menuName = "GWLPXL/ARPG/Modifiers/New Stat Mod")]
    public class ARPGStatModHolder : BaseModHolder
    {

        AttributeType type = AttributeType.Stat;
        public ARPGStatModifier[] StatMods = new ARPGStatModifier[0];

        public override IList<ModBase> GetAllModifiers()
        {
            return StatMods;
        }

        public override int GetAttributeType()
        {
            return (int)type;
        }
    }
}