using GWLPXL.ARPGCore.Traits.com;
using GWLPXL.ARPGCore.Types.com;

using UnityEngine;

namespace GWLPXL.ARPGCore.Items.com
{
    [System.Serializable]
    public class SocketableVars
    {
        public string BaseName = string.Empty;
        public int StackingAmount = 5;
        public string GeneratedName = string.Empty;
        public EquipmentTrait Socketable = null;
        public SocketableVars(EquipmentTrait socketable, string basename, int stacking = 1)
        {
            Socketable = socketable;
            BaseName = basename;
            StackingAmount = stacking;
        }
    }
    /// <summary>
    /// in progress
    /// </summary>
    [CreateAssetMenu(menuName ="GWLPXL/ARPG/Socketables/New Socketable")]
    public class Socketable : Item
    {
        public SocketableVars Vars;
        public override string GetBaseItemName()
        {
            return Vars.BaseName;
        }

        public override string GetGeneratedItemName()
        {
            return Vars.GeneratedName;
        }

        public override ItemType GetItemType()
        {
            return ItemType.Socketable;
        }

        public override int GetStackingAmount()
        {
            return Vars.StackingAmount;
        }

        public override string GetUserDescription()
        {
            string generated = GetGeneratedItemName();
            if (string.IsNullOrEmpty(generated) == false)
            {
                return GetGeneratedItemName() + Vars.Socketable.GetTraitUIDescription();
            }

            return GetBaseItemName() + Vars.Socketable.GetTraitUIDescription();
        }

        public override bool IsStacking()
        {
            return Vars.StackingAmount > 1;
        }

        public override void SetGeneratedItemName(string newName)
        {
            Vars.GeneratedName = newName;
        }
    }
}