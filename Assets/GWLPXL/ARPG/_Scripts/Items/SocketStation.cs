using GWLPXL.ARPGCore.Traits.com;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ARPGCore.Items.com
{

    /// <summary>
    /// in progress
    /// </summary>
    public class SocketStation
    {
        public SocketTypeReader SocketTypeReader = default;
        public System.Action<Equipment> OnAddSocketable;
        public System.Action<Equipment> OnRemoveSocketable;
        public System.Action<SocketStation> OnStationSetup;
        public System.Action<SocketStation> OnStationClosed;
        public System.Action OnFailToAddStorageIssue;
        public System.Action OnFailToAddTypeMisMatch;
        public bool DestroyRemovedSockets = false;
        List<SocketItem> socketables = new List<SocketItem>();//this needs to turn into an item.
        ActorInventory userInventory = null;

        public virtual void CloseStation()
        {
            this.socketables = new List<SocketItem>();
            this.userInventory = null;
            OnStationClosed?.Invoke(this);

        }
        public virtual List<SocketItem> GetAllSocketables()
        {   //from user inventory?, nah we load it. 
            return socketables;
        }
        public virtual void SetupStation(ActorInventory userInventory, List<SocketItem> enchants)
        {
            this.socketables = enchants;
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

        public virtual void RemoveSocketable(Equipment equipment, int atindex, bool isPreview = false)
        {
            Socket socket = GetSocket(equipment, atindex);
            if (socket == null)
            {
                DebugHelpers.com.ARPGDebugger.DebugMessage("No socket found" + equipment.GetUserDescription(), equipment);
                return;
            }
            bool handled = HandleRemoval(equipment, socket);

        }

        public virtual void AddSocketable(Equipment equipment, SocketItem newSocketable,  int atindex, bool isPreview = false)
        {
            Socket socket = GetSocket(equipment, atindex);
            if (socket == null)
            {
                DebugHelpers.com.ARPGDebugger.DebugMessage("No socket found" + equipment.GetUserDescription(), equipment);
                return;
            }

            if (socket.SocketType != newSocketable.GetSocketType())
            {
                DebugHelpers.com.ARPGDebugger.DebugMessage("Socket Type Mismatch, can't add" + equipment.GetUserDescription(), equipment);
                OnFailToAddTypeMisMatch?.Invoke();
                return;
            }
                

            bool handled = HandleRemoval(equipment, socket);
            if (handled == false)
            {
                DebugHelpers.com.ARPGDebugger.DebugMessage("Unable to handle existing" + equipment.GetUserDescription(), equipment);
                return;
            }

            socket.SocketedThing = newSocketable;
            OnAddSocketable?.Invoke(equipment);

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
                int count = equipment.GetStats().GetSockets().Count;
                if (count > 0)
                {
                    socket = equipment.GetStats().GetSocket(atindex);
                }
            }

            return socket;
        }

        protected virtual bool HandleRemoval(Equipment equipment, Socket socket)
        {
            if (socket.SocketedThing != null)
            {
                if (DestroyRemovedSockets)
                {
                    //just make it null, no need to add back
                    socket.SocketedThing = null;
                    OnRemoveSocketable?.Invoke(equipment);
                }
                else
                {
                    //add it back to inventory, then null it.
                    bool canwe = userInventory.CanWeAddItem(socket.SocketedThing);
                    if (canwe == false)
                    {
                        OnFailToAddStorageIssue?.Invoke();
                        return false;
                    }
                    else
                    {
                        //add it back to inventory, null it.
                        userInventory.AddItemsToInventory(socket.SocketedThing, 1);
                        socket.SocketedThing = null;
                        OnRemoveSocketable?.Invoke(equipment);
                    }
                }

            }
            return true;
        }
    }
}