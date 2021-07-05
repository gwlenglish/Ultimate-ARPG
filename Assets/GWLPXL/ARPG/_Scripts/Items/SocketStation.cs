using GWLPXL.ARPGCore.Statics.com;
using GWLPXL.ARPGCore.Traits.com;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Text;
namespace GWLPXL.ARPGCore.Items.com
{

    /// <summary>
    /// in progress
    /// </summary>
    public class SocketStation
    {
        public ActorInventory Inventory => userInventory;
        public SocketTypeReader SocketTypeReader = default;
        public AffixReaderSO AffixReaderSO = default;
        public System.Action OnSmithOpen;
        public System.Action OnSmithClosed;
        public System.Action<Equipment> OnAddSocketable;
        public System.Action<Equipment> OnRemoveSocketable;
        public System.Action<SocketStation> OnStationSetup;
        public System.Action<SocketStation> OnStationClosed;
        public System.Action OnFailToAddStorageIssue;
        public System.Action OnFailToAddTypeMisMatch;

        List<ItemStack> stacksofsockets = new List<ItemStack>();
        ActorInventory userInventory = null;
        StringBuilder sb = new StringBuilder();
        public virtual void CloseStation()
        {
            stacksofsockets.Clear();
            this.userInventory = null;
            OnStationClosed?.Invoke(this);

        }
        
        public virtual List<ItemStack> GetAllSocketables()
        {   //from user inventory?, nah we load it. 
            return stacksofsockets;
        }
        /// <summary>
        /// working on getting prefix and suffix seperately next, all at once doens't work so well because it's all prefixes
        /// </summary>
        /// <param name="equipment"></param>
        public virtual void RenameItemWithSocket(Equipment equipment)
        {
            sb.Clear();
            List<string> socketaffixes = equipment.GetStats().GetAllSocketAffixes();
            string newname = PlayerDescription.GetGeneratedName(equipment);
            if (socketaffixes.Count > 0)//we have none
            {
                string[] sep = new string[1] { equipment.GetBaseItemName() };
                string[] result = newname.Split(sep, StringSplitOptions.None);
                Debug.Log("RESult " + result.Length);
                if (result.Length > 0)
                {
                    Debug.Log(result[0]);
                   
                    List<string> generatedprefixes = AffixReaderSO.GetSplitAffixes(result[0]);
                    List<string> socketprefixes = equipment.GetStats().GetAllSocketPrefixes();
                    generatedprefixes.Concat(socketprefixes);
                    string prefixtoname = AffixReaderSO.GetNameWithAffixesPreLoaded(generatedprefixes, equipment.GetBaseItemName(), 0);
                    sb.Append(prefixtoname);
                }
              
                if (result.Length > 1)
                {
                    Debug.Log(result[1]);
                    string postnoun = AffixReaderSO.GetPostNoun(result[1]);
                    List<string> generatedpostnoun = AffixReaderSO.GetSplitAffixes(result[1]);
                    List<string> socketpostprefixes = equipment.GetStats().GetAllSocketAffixes();
                    generatedpostnoun.Concat(socketpostprefixes);
                    if (string.IsNullOrEmpty(postnoun) == false && generatedpostnoun.Count > 0)
                    {
                        string of = " of ";
                        string postnounwithaffixes = AffixReaderSO.GetNameWithAffixesPreLoaded(generatedpostnoun, postnoun, 0);
                        sb.Append(of);
                        sb.Append(postnounwithaffixes);
                    }
                    else
                    {
                        //didn't find noun or didn't find prefixes
                    }

                   
                }

                newname = sb.ToString();
            }

 
            equipment.SetGeneratedItemName(newname);
        }
        public virtual void SetupStation(ActorInventory userInventory)
        {
            stacksofsockets.Clear();
            List<ItemStack> stacks = userInventory.GetAllUniqueStacks();
            for (int i = 0; i < stacks.Count; i++)
            {
                if (stacks[i].Item is SocketItem)
                {
                    //found it
                    stacksofsockets.Add(stacks[i]);
                }
            }
            this.userInventory = userInventory;
            OnStationSetup?.Invoke(this);
        }

        public virtual List<Equipment> GetEquippedEquipmentWithSockets()
        {
            Dictionary<Types.com.EquipmentSlotsType, EquipmentSlot> temp = userInventory.GetEquippedEquipment();
            List<Equipment> _temp = new List<Equipment>();
            foreach (var kvp in temp)
            {
                if (kvp.Value.EquipmentInSlots == null) continue;//no equipment
                if (kvp.Value.EquipmentInSlots.GetStats().GetSockets().Count == 0) continue;//no sockets

                if (_temp.Contains(kvp.Value.EquipmentInSlots) == false)//check so we dont double add 2handers and such
                {

                    _temp.Add(kvp.Value.EquipmentInSlots);
                }
            }
            return _temp;
        }

        public virtual List<Equipment> GetEquipmentInInventoryWithSockets()
        {
            List<ItemStack> stack = userInventory.GetAllUniqueStacks();
            List<Equipment> _temp = new List<Equipment>();
            for (int i = 0; i < stack.Count; i++)
            {
                if (stack[i].Item is Equipment)
                {
                    Equipment equipment = (Equipment)stack[i].Item;
                    if (equipment.GetStats().GetSockets().Count > 0)//has sockets
                    {
                        _temp.Add(stack[i].Item as Equipment);
                    }
     
                }
            }
            return _temp;
        }

        
      public virtual bool CanAdd(Equipment equipment, int atIndex, SocketItem newSocketable)
        {
            Socket socket = GetSocket(equipment, atIndex);
            if (socket == null)
            {
                DebugHelpers.com.ARPGDebugger.DebugMessage("No socket found" + equipment.GetUserDescription(), equipment);
                return false;
            }

            if (newSocketable != null)
            {
                if (socket.SocketType != newSocketable.GetSocketType())
                {
                    DebugHelpers.com.ARPGDebugger.DebugMessage("Socket Type Mismatch, can't add" + equipment.GetUserDescription(), equipment);
                    OnFailToAddTypeMisMatch?.Invoke();
                    return false;
                }
            }
        
            return true;
        }

        public virtual bool SwapSocketItem(Equipment equipment, int a, int b)
        {
            Socket sockA = equipment.GetStats().GetSocket(a);
            Socket sockB = equipment.GetStats().GetSocket(b);
            SocketItem itemA = sockA.SocketedThing;
            SocketItem itemB = sockB.SocketedThing;
            if (CanAdd(equipment, a, itemB) && CanAdd(equipment, b, itemA))
            {
                AddSocketable(equipment, itemB, a);
                AddSocketable(equipment, itemA, b);
                return true;
            }
            return false;
        }
        public virtual bool RemoveSocketable(Equipment equipment, int atIndex, bool andRename = false)
        {
            Socket socket = GetSocket(equipment, atIndex);
            socket.SocketedThing = null;
            equipment.GetStats().SetSocket(atIndex, socket);
            OnRemoveSocketable?.Invoke(equipment);

            if (andRename)
            {
                RenameItemWithSocket(equipment);
            }
            return true;
        }
        public virtual bool AddSocketable(Equipment equipment, SocketItem newSocketable,  int atindex, bool alsoRename = false)
        {

            Socket socket = GetSocket(equipment, atindex);
            socket.SocketedThing = newSocketable;
            equipment.GetStats().SetSocket(atindex, socket);
            OnAddSocketable?.Invoke(equipment);

            if (alsoRename)
            {
                RenameItemWithSocket(equipment);
            }
            return true;

        }
        /// <summary>
        /// returns null if nothing is found.
        /// </summary>
        /// <param name="equipment"></param>
        /// <param name="atindex"></param>
        /// <returns></returns>
        protected virtual Socket GetSocket(Equipment equipment, int atindex)
        {
            Socket socket = null;
            if (equipment != null)
            {
                return equipment.GetStats().GetSocket(atindex);
            }

            return socket;
        }

       /// <summary>
       /// would be ideal, but doesn't seem to be working at the moment
       /// </summary>
       /// <param name="equipment"></param>
       /// <param name="index"></param>
       /// <param name="destroyRemoved"></param>
       /// <returns></returns>
        protected virtual bool HandleRemoval(Equipment equipment, int index, bool destroyRemoved = false)
        {
            if (equipment.GetStats().GetSocket(index) == null)
            {
                return true;
            }

            if (equipment.GetStats().GetSocket(index).SocketedThing != null)
            {
                if (destroyRemoved)
                {
                    //just make it null, no need to add back
                    equipment.GetStats().GetSocket(index).SocketedThing = null;
                    OnRemoveSocketable?.Invoke(equipment);
                }
                else
                {
                    //add it back to inventory, then null it.

                    bool canwe = userInventory.CanWeAddItem(equipment.GetStats().GetSocket(index).SocketedThing);
                    if (canwe == false)
                    {
                        OnFailToAddStorageIssue?.Invoke();
                        return false;
                    }
                    else
                    {
                        Debug.Log("REMOVED FROM INVENTORY");
                        //add it back to inventory, null it.
                        userInventory.AddItemsToInventory(equipment.GetStats().GetSocket(index).SocketedThing, 1);
                        equipment.GetStats().GetSocket(index).SocketedThing = null;
                        OnRemoveSocketable?.Invoke(equipment);
                    }
                }
            }
            return true;
        }
    }
}