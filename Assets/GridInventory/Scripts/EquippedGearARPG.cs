using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Items.com;
using GWLPXL.ARPGCore.Types.com;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GWLPXL.InventoryGrid
{


    /// <summary>
    /// manager for the equipment
    /// </summary>
    [System.Serializable]
    public class EquippedGearARPG
    {
        
        public PatternHolder DefaultPattern;
        public System.Action<IInventoryPiece> OnGearRemoved;
        public System.Action<IInventoryPiece> OnGearPlaced;
        public System.Action<IInventoryPiece, IInventoryPiece> OnGearSwap;
        public System.Action<IGearSlot> OnSlotCreated;
        public System.Action<IGearSlot> OnSlotRemoved;
        public System.Action<IGearSlot> OnSlotModified;

        public Dictionary<GameObject, IGearSlot> GearSlotDic => gearSlotDic;
        public Dictionary<int, IGearSlot> GearSlotIDDic => gearSlotIDDic;
        /// <summary>
        /// design time, set in the editor
        /// </summary>
        [Tooltip("Set in the editor")]
        public List<ARPGGearSlot> Slots = new List<ARPGGearSlot>();

        /// <summary>
        /// values used at runtime
        /// </summary>
        [SerializeField]
        [Tooltip("Runtime values")]
        protected List<IGearSlot> registeredSlots = new List<IGearSlot>();

        protected Dictionary<GameObject, IGearSlot> gearSlotDic = new Dictionary<GameObject, IGearSlot>();
        protected Dictionary<int, IGearSlot> gearSlotIDDic = new Dictionary<int, IGearSlot>();

        IActorHub user;
        #region public virtual
        /// <summary>
        /// call to initialize the equipment manager
        /// </summary>
        public virtual void Setup(IActorHub user)
        {

            registeredSlots.Clear();
            gearSlotDic.Clear();
            gearSlotIDDic.Clear();
            for (int i = 0; i < Slots.Count; i++)
            {
                AddEquippedGearSlot(Slots[i].SlotInstance, Slots[i].Identifier);
            }



        }

   

 

        

  
       

      


      




        /// <summary>
        /// remove an existing slot completely
        /// </summary>
        /// <param name="instance"></param>
        public virtual void RemoveEquippedGearSlot(GameObject instance)
        {
            if (gearSlotDic.ContainsKey(instance))
            {
                int id = gearSlotDic[instance].Identifier;
                if (gearSlotIDDic.ContainsKey(id))
                {
                    gearSlotIDDic.Remove(id);
                }
                IGearSlot slot = gearSlotDic[instance];
                gearSlotDic.Remove(instance);
                registeredSlots.Remove(slot);
                OnSlotRemoved?.Invoke(slot);

            }
        }

        /// <summary>
        /// add a new gear slot
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="identifier"></param>
        public virtual void AddEquippedGearSlot(GameObject instance, int identifier)
        {
            if (gearSlotDic.ContainsKey(instance) == false)
            {
                ARPGGearSlot slot = new ARPGGearSlot(instance, null, identifier);
                gearSlotDic.Add(instance, slot);
                gearSlotIDDic.Add(slot.Identifier, slot);
              
                registeredSlots.Add(slot);
                OnSlotCreated?.Invoke(slot);
            }


        }

        #endregion

        #region protected virtual

        /// <summary>
        /// place piece in slot, no checks
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="slot"></param>
        public virtual void Place(IInventoryPiece piece, IGearSlot slot)
        {

            slot.Piece = piece;
            slot.Equipment = piece.ItemStack as Equipment;

            slot.Piece.Instance.transform.position = slot.SlotInstance.transform.position;
            OnGearPlaced?.Invoke(slot.Piece);
            OnSlotModified?.Invoke(slot);
        }

        /// <summary>
        /// swap with piece already in slot, no checks
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="slot"></param>
        public virtual void Swap(IInventoryPiece piece, IGearSlot slot)
        {

            IInventoryPiece oldpiece = slot.Piece;


            slot.Piece = piece;
            slot.Equipment = piece.ItemStack as Equipment;

            piece.Instance.transform.position = slot.SlotInstance.transform.position;
            OnGearSwap?.Invoke(oldpiece, piece);
            OnSlotModified?.Invoke(slot);
        }
        /// <summary>
        /// remove piece from slot, no checks
        /// </summary>
        /// <param name="slot"></param>
        public virtual void Remove(IGearSlot slot)
        {

            IInventoryPiece removedPiece = slot.Piece;
            slot.Piece = null;
            slot.Equipment = null;

            OnGearRemoved?.Invoke(removedPiece);
            OnSlotModified?.Invoke(slot);
        }

        #endregion
    }
}