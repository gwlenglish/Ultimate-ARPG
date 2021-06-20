using GWLPXL.ARPGCore.Traits.com;

using UnityEngine;

namespace GWLPXL.ARPGCore.Items.com
{
    /// <summary>
    /// in progress
    /// </summary>
    [CreateAssetMenu(menuName ="GWLPXL/ARPG/Socketables/New Equipment Socketable")]

    public class EquipmentSocketable : Socketable
    {
        public EquipmentTrait Socketable = null;
        public override string GetUserDescription()
        {
            string generated = GetGeneratedItemName();
            if (string.IsNullOrEmpty(generated) == false)
            {
                return GetGeneratedItemName() + Socketable.GetTraitUIDescription();
            }

            return GetBaseItemName() + Socketable.GetTraitUIDescription();
        }
    }
}