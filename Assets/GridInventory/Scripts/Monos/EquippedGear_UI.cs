using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GWLPXL.InventoryGrid
{


    /// <summary>
    /// example UI class that uses the EquippedGear class and subs to GridInventory_UI
    /// </summary>
    public class EquippedGear_UI : MonoBehaviour
    {
        public System.Action<IInventoryPiece> OnEquippedPiece;
        public System.Action<IInventoryPiece> OnUnEquipPiece;
        public System.Action<IInventoryPiece, IInventoryPiece> OnSwappedPiece;
        public GridInventory_UI InventoryUI;
        [SerializeField]
        protected EquippedGearARPG gear = new EquippedGearARPG();

        #region unity virtual calls
        protected virtual void OnEnable()
        {
            InventoryUI.OnTryPlace += TryPlace;
            InventoryUI.OnTryRemove += TryRemove;
            InventoryUI.OnDraggingPiece += CheckDragging;
            InventoryUI.OnStopDragging += StopCheckDragging;
            gear.OnGearPlaced += GearEquipped;
            gear.OnGearRemoved += GearUnEquipped;
            gear.OnGearSwap += GearSwapped;
        }

        protected virtual void OnDisable()
        {
            InventoryUI.OnTryPlace -= TryPlace;
            InventoryUI.OnTryRemove -= TryRemove;
            InventoryUI.OnDraggingPiece -= CheckDragging;
            InventoryUI.OnStopDragging -= StopCheckDragging;
            gear.OnGearPlaced -= GearEquipped;
            gear.OnGearRemoved -= GearUnEquipped;
            gear.OnGearSwap -= GearSwapped;
        }

        protected virtual void Start()
        {
            CreateGear();
        }

        #endregion

        #region public virtual
        public virtual void CreateGear()
        {
            gear.Setup();
        }
        #endregion

        #region protected virtual

        protected virtual void GearSwapped(IInventoryPiece old, IInventoryPiece newpiece)
        {

            Debug.Log("Swapped Gear " + old + " and " + newpiece);
            OnSwappedPiece?.Invoke(old, newpiece);
        }
        protected virtual void GearEquipped(IInventoryPiece piece)
        {
            Debug.Log("Placed Gear " + piece.Instance.name);
            OnEquippedPiece?.Invoke(piece);
        }

        protected virtual void GearUnEquipped(IInventoryPiece piece)
        {
            Debug.Log("Removed Gear " + piece.Instance.name);
            OnUnEquipPiece?.Invoke(piece);

        }

        protected virtual void CheckDragging(IInventoryPiece dragging)
        {
            List<IGearSlot> slots = new List<IGearSlot>(0);
            List<IGearSlot> illegal = new List<IGearSlot>();
            foreach (var kvp in gear.GearSlotIDDic)
            {

                for (int i = 0; i < dragging.EquipmentIdentifier.Length; i++)
                {
                    int current = dragging.EquipmentIdentifier[i];

                    if (current == kvp.Key)
                    {
                        slots.Add(kvp.Value);
                    }
                    else
                    {
                        illegal.Add(kvp.Value);
                    }

                }

                for (int i = 0; i < slots.Count; i++)
                {
                    slots[i].SlotInstance.GetComponentInParent<Image>().color = Color.green;
                }

                for (int i = 0; i < illegal.Count; i++)
                {
                    illegal[i].SlotInstance.GetComponentInParent<Image>().color = Color.red;
                }

                

            }
        }
        protected virtual void StopCheckDragging()
        {
            foreach (var kvp in gear.GearSlotIDDic)
            {
                kvp.Value.SlotInstance.GetComponentInParent<Image>().color = Color.white;

            }

        }


        protected virtual void TryRemove(List<RaycastResult> results)
        {
            foreach (RaycastResult result in results)
            {
                if (gear.GearSlotDic.ContainsKey(result.gameObject) == false) continue;
                Debug.Log("Hit Gear " + result.gameObject.name, result.gameObject);
                IGearSlot slot = gear.GearSlotDic[result.gameObject];
                IInventoryPiece piece = slot.Piece;
                if (piece == null) continue;

                bool unequip = gear.RemoveFromSlot(result.gameObject, piece);

                if (unequip)
                {
                    return;
                }

            }
        }
        protected virtual void TryPlace(List<RaycastResult> results, IInventoryPiece piece)
        {
            foreach (RaycastResult result in results)
            {

                if (gear.GearSlotDic.ContainsKey(result.gameObject) == false) continue;

                IGearSlot slot = gear.GearSlotDic[result.gameObject];
                if (slot.Piece != null)
                {
                    bool placed = gear.Swap(slot, piece);
                    if (placed)
                    {
                        return;
                    }
                }
                else
                {
                    bool placed = gear.PlaceInSlot(result.gameObject, piece);
                    if (placed)
                    {
                        return;
                    }
                }

            }
        }


        #endregion


    }
}