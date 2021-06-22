using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GWLPXL.ARPGCore.Traits.com;
namespace GWLPXL.ARPGCore.Items.com
{
    
    /// <summary>
    /// class that performs the enchanting. Adds a trait to equipment.
    /// </summary>
    public class EnchantingStation
    {
        public System.Action<Equipment> OnEnchanted;
        public System.Action<EnchantingStation> OnStationSetup;
        public System.Action<EnchantingStation> OnStationClosed;
        List<EquipmentEnchant> enchants = new List<EquipmentEnchant>();
        ActorInventory userInventory;

        #region ini cycle
        public virtual void CloseStation()
        {
            this.enchants.Clear();
            this.userInventory = null;
            OnStationClosed?.Invoke(this);

        }
        public virtual void SetupStation(ActorInventory userInventory, List<EquipmentEnchant> enchants)
        {
            this.enchants = enchants;
            this.userInventory = userInventory;
            OnStationSetup?.Invoke(this);
        }
        #endregion

        #region getters
        public virtual List<Equipment> GetEquippedEquipment()
        {
            Dictionary<Types.com.EquipmentSlotsType, EquipmentSlot> temp = userInventory.GetEquippedEquipment();
            List<Equipment> _temp = new List<Equipment>();
            foreach (var kvp in temp)
            {
                if (kvp.Value.EquipmentInSlots == null) continue;
                if (_temp.Contains(kvp.Value.EquipmentInSlots) == false)//check so we dont double add 2handers and such
                {
                    _temp.Add(kvp.Value.EquipmentInSlots);
                }
            }
            return _temp;
        }
        public virtual List<Equipment> GetEquipmentInInventory()
        {
            List<ItemStack> stack = userInventory.GetAllUniqueStacks();
            List<Equipment> _temp = new List<Equipment>();
            for (int i = 0; i < stack.Count; i++)
            {
                if (stack[i].Item is Equipment)
                {
                    _temp.Add(stack[i].Item as Equipment);
                }
            }
            return _temp;
        }
        public virtual List<EquipmentEnchant> GetAllEnchants()
        {
            return enchants;
        }
        public virtual List<string> GetAllEnchantsNames()
        {
            List<string> names = new List<string>();
            for (int i = 0; i < enchants.Count; i++)
            {
                names.Add(enchants[i].EnchantName);
                

            }
            return names;
        }
       
        #endregion

        #region enchant overrides
        /// <summary>
        /// takes a list of enchants
        /// </summary>
        /// <param name="equipment"></param>
        /// <param name="enchants"></param>
        /// <param name="isNative"></param>
        /// <param name="isPreview"></param>
        public virtual void Enchant(Equipment equipment, List<EquipmentEnchant> enchants, bool isNative = true, bool isPreview = false)
        {
            for (int i = 0; i < enchants.Count; i++)
            {
                List<EquipmentTrait> traits = enchants[i].EnchantTraits;
                for (int j = 0; j < traits.Count; j++)
                {
                    Enchant(equipment, traits[j], enchants[i].EnchantLevel, isNative, true);//make true to bypass the event to raise
                }
            }

            if (isPreview == false)//raise event here, so it only happens once
            {
                OnEnchanted?.Invoke(equipment);
            }
        }
        /// <summary>
        /// takes a single enchant
        /// </summary>
        /// <param name="equipment"></param>
        /// <param name="enchant"></param>
        /// <param name="isNative"></param>
        /// <param name="isPreview"></param>
        public virtual void Enchant(Equipment equipment, EquipmentEnchant enchant, bool isNative = true, bool isPreview = false)
        {
            List<EquipmentTrait> traits = enchant.EnchantTraits;
            for (int j = 0; j < traits.Count; j++)
            {
                Enchant(equipment, traits[j], enchants[j].EnchantLevel, isNative, true);//make true to bypass the event to raise
            }
            
            if (isPreview == false)
            {
                OnEnchanted?.Invoke(equipment);
            }
        }
        /// <summary>
        /// Modifies an existing item and adds a trait to it at the ilevel. Can overload to put in the native or random slots. Preview will not raise the Enchant event.
        /// </summary>
        /// <param name="equipment"></param>
        /// <param name="newEnchantTrait"></param>
        /// <param name="traitIlevel"></param>
        /// <param name="isNative"></param>
        public virtual void Enchant(Equipment equipment, EquipmentTrait newEnchantTrait, int traitIlevel, bool isNative = true, bool isPreview = false)
        {
            if (equipment == null)
            {
                DebugHelpers.com.ARPGDebugger.DebugMessage("Trying to enchant equipment which isn't enchantable ", null);
                return;
            }
            if (equipment.CanEnchant() == false)
            {
                DebugHelpers.com.ARPGDebugger.DebugMessage("Trying to enchant equipment which isn't enchantable " + equipment.GetUserDescription(), equipment);
                return;
            }
            EquipmentTrait[] existing = new EquipmentTrait[0];
            if (isNative)
            {
                existing = equipment.GetStats().GetNativeTraits();

            }
            else
            {
                existing = equipment.GetStats().GetRandomTraits();
            }

            EquipmentTrait copy = ScriptableObject.Instantiate(newEnchantTrait);
            copy.SetRandomValue(traitIlevel);
            System.Array.Resize(ref existing, existing.Length + 1);
            existing[existing.Length - 1] = copy;

            if (isNative)
            {
                equipment.GetStats().SetNativeTraits(existing);
            }
            else
            {
                equipment.GetStats().SetRandomTraits(existing);
            }
            
            if (isPreview == false)
            {
                OnEnchanted?.Invoke(equipment);

            }

        }
        #endregion
    }
}
