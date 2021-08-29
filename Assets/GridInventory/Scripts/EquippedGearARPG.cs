using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.InventoryGrid
{


    /// <summary>
    /// manager for the equipment
    /// </summary>
    [System.Serializable]
    public class EquippedGearARPG
    {
        public System.Action<IInventoryPiece> OnGearRemoved;
        public System.Action<IInventoryPiece> OnGearPlaced;
        public System.Action<IInventoryPiece, IInventoryPiece> OnGearSwap;
        public System.Action<IGearSlot> OnSlotCreated;
        public System.Action<IGearSlot> OnSlotRemoved;

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

        #region public virtual
        /// <summary>
        /// call to initialize the equipment manager
        /// </summary>
        public virtual void Setup()
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
        /// checks if piece has same ID as slot
        /// </summary>
        /// <param name="slotInstance"></param>
        /// <param name="piece"></param>
        /// <returns></returns>
        public virtual bool CanFitSlot(GameObject slotInstance, IInventoryPiece piece)
        {
            int size = 0;
            int pieceids = piece.EquipmentIdentifier.Length;
            for (int i = 0; i < pieceids; i++)
            {
                if (gearSlotIDDic.ContainsKey(piece.EquipmentIdentifier[i]) == false) return false;

                IGearSlot slot = gearSlotIDDic[piece.EquipmentIdentifier[i]];

                for (int j = 0; j < piece.EquipmentIdentifier.Length; j++)
                {
                    if (slot.Identifier == piece.EquipmentIdentifier[j])
                    {
                        size++;
                    }
                }
            }

            if (size == piece.EquipmentIdentifier.Length)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// swaps newpiece with piece already in slot, used for swapping dragged equipment
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="newpiece"></param>
        /// <returns></returns>
        public virtual bool Swap(IGearSlot slot, IInventoryPiece newpiece)
        {
            if (CanFitSlot(slot.SlotInstance, newpiece))
            {
                Swap(newpiece, slot);
                return true;
            }
            return false;

        }

        /// <summary>
        /// will place if nothing in slot, will try to swap if something is in.
        /// </summary>
        /// <param name="slotInstance"></param>
        /// <param name="piece"></param>
        /// <returns></returns>
        public virtual bool PlaceInSlot(GameObject slotInstance, IInventoryPiece piece)
        {

            if (CanFitSlot(slotInstance, piece))
            {
                int[] id = piece.EquipmentIdentifier;
                for (int i = 0; i < id.Length; i++)
                {
                    IGearSlot slot = gearSlotIDDic[id[i]];
                    if (slot.Piece == null || slot.Piece.Instance == null)
                    {
                        Place(piece, slot);
                    }
                    else if (slot.Piece.Instance != null)
                    {
                        Swap(piece, slot);
                        return true;
                    }
                }
               
            }
            return false;

        }



        /// <summary>
        /// will remove if piece is present
        /// </summary>
        /// <param name="slotInstance"></param>
        /// <param name="piece"></param>
        /// <returns></returns>
        public virtual bool RemoveFromSlot(GameObject slotInstance, IInventoryPiece piece)
        {
            if (gearSlotDic.ContainsKey(slotInstance))
            {
                IGearSlot slot = gearSlotDic[slotInstance];
                if (slot.Piece != null)
                {
                    Remove(slot);
                    return true;
                }

            }
            return false;
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
                GearSlot slot = new GearSlot(instance, identifier);
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
        protected virtual void Place(IInventoryPiece piece, IGearSlot slot)
        {
            slot.Piece = piece;
            slot.Piece.Instance.transform.position = slot.SlotInstance.transform.position;
            OnGearPlaced?.Invoke(slot.Piece);
        }

        /// <summary>
        /// swap with piece already in slot, no checks
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="slot"></param>
        protected virtual void Swap(IInventoryPiece piece, IGearSlot slot)
        {
            IInventoryPiece oldpiece = slot.Piece;
            slot.Piece = piece;
            piece.Instance.transform.position = slot.SlotInstance.transform.position;
            OnGearSwap?.Invoke(oldpiece, piece);
        }
        /// <summary>
        /// remove piece from slot, no checks
        /// </summary>
        /// <param name="slot"></param>
        protected virtual void Remove(IGearSlot slot)
        {
            IInventoryPiece removedPiece = slot.Piece;
            slot.Piece = null;
            OnGearRemoved?.Invoke(removedPiece);
        }

        #endregion
    }
}