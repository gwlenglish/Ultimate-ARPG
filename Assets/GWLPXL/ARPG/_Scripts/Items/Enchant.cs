using GWLPXL.ARPGCore.Traits.com;
using UnityEngine;
using System.Collections.Generic;
namespace GWLPXL.ARPGCore.Items.com
{
    public abstract class Enchant
    {
        public string EnchantName;
        public string EnchantDescription;
        [Tooltip("The level of the enchant, correlates with ILevel")]
        public int EnchantLevel;
        public Sprite Sprite;
    }

    [System.Serializable]
    public class EquipmentEnchant : Enchant
    {
        public List<EquipmentTrait> EnchantTraits = new List<EquipmentTrait>();
        public EquipmentEnchant(List<EquipmentTrait> enchanttrait, int ilevel, string name, string description)
        {
            EnchantLevel = ilevel;
            EnchantTraits = enchanttrait;
            EnchantName = name;
            EnchantDescription = description;
        }
    }
}
