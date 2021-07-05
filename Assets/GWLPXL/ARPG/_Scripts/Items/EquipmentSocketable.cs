using GWLPXL.ARPGCore.Traits.com;

using UnityEngine;

namespace GWLPXL.ARPGCore.Items.com
{
    /// <summary>
    /// in progress
    /// </summary>
    [CreateAssetMenu(menuName ="GWLPXL/ARPG/Socketables/New Equipment Socketable")]

    public class EquipmentSocketable : SocketItem
    {
        public EquipmentTrait EquipmentTraitSocketable = null;


        public override string GetUserDescription()
        {
            string generated = GetGeneratedItemName();
            if (string.IsNullOrEmpty(generated) == false)
            {
                return GetGeneratedItemName() + EquipmentTraitSocketable.GetTraitUIDescription();
            }

            return GetBaseItemName() + EquipmentTraitSocketable.GetTraitUIDescription();
        }
    }
}