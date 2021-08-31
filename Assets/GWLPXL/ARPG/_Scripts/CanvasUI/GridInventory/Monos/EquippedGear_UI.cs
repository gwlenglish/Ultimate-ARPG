using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Items.com;
using GWLPXL.ARPGCore.Types.com;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GWLPXL.ARPGCore.CanvasUI.com
{


    /// <summary>
    /// example UI class that uses the EquippedGear class and subs to GridInventory_UI
    /// </summary>
    public class EquippedGear_UI : MonoBehaviour
    {
        IActorHub user;
        public PatternHolder DefaultPattern;
        public EquippedGearARPG Gear => gear;
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



        #endregion

        #region public virtual
        public virtual void CreateGear(IActorHub user)
        {
            this.user = user;
            gear.Setup(user);

            ActorInventory inv = user.MyInventory.GetInventoryRuntime();
            Dictionary<EquipmentSlotsType, EquipmentSlot> slots = inv.GetEquippedEquipment();
            foreach (var kvp in slots)
            {
                UpdateEquip(kvp.Value);
            }

            inv.OnEquipmentSlotChanged += UpdateEquip;


        }
        #endregion

        Dictionary<EquipmentSlotsType, int> slotId = new Dictionary<EquipmentSlotsType, int>();
        Dictionary<EquipmentSlot, IInventoryPiece> eqpieces = new Dictionary<EquipmentSlot, IInventoryPiece>();
        protected virtual void UpdateEquip(EquipmentSlot ment)
        {
            int key =(int)ment.slot;
            if (gear.GearSlotIDDic.ContainsKey(key))
            {
                IGearSlot slot = gear.GearSlotIDDic[key];
                slot.Equipment = ment.EquipmentInSlots;

                if (ment.EquipmentInSlots == null)
                {
                    if (slot.Piece != null)
                    {
                        slot.Piece.CleanUP();
                        slot.Piece = null;
                    }
                }
                else
                {
                    if (slot.Piece == null)
                    {
                        slot.Piece = InventoryUI.CreatePiece(slot.Equipment);
                        slot.Piece.Instance.transform.position = slot.SlotInstance.transform.position;

                    }
                    else
                    {
                        slot.Piece.ItemStack = slot.Equipment;
                        slot.Piece.Instance.GetComponentInChildren<Image>().sprite = slot.Equipment.GetSprite();
                        slot.Piece.PreviewInstance.GetComponentInChildren<Image>().sprite = slot.Equipment.GetSprite();

                    }
                }

            }
     

            
    
           
        }




        #region protected virtual



        protected virtual void GearSwapped(IInventoryPiece old, IInventoryPiece newpiece)
        {

            Debug.Log("Swapped Gear " + old + " and " + newpiece);

            OnSwappedPiece?.Invoke(old, newpiece);
            StopCheckDragging();
        }
        protected virtual void GearEquipped(IInventoryPiece piece)
        {
            Debug.Log("Placed Gear " + piece.Instance.name);

            InventoryUI.NoPieces();
        }

        protected virtual void GearUnEquipped(IInventoryPiece piece)
        {


            InventoryUI.NoPieces();

        }

       
        protected virtual void CheckDragging(IInventoryPiece dragging)
        {
            if (dragging == null)
            {
                StopCheckDragging();
                return;
            }
            foreach (var kvp in gear.GearSlotIDDic)
            {
                kvp.Value.SlotInstance.GetComponent<Image>().color = Color.red;

            }


            for (int i = 0; i < dragging.EquipmentIdentifier.Length; i++)
            {
                if (gear.GearSlotIDDic.ContainsKey(dragging.EquipmentIdentifier[i]))
                {
                    IGearSlot slot = gear.GearSlotIDDic[dragging.EquipmentIdentifier[i]];
                    slot.SlotInstance.GetComponent<Image>().color = Color.green;
                }
            }
        }
        protected virtual void StopCheckDragging()
        {
            foreach (var kvp in gear.GearSlotIDDic)
            {
                IGearSlot gearslot = kvp.Value;
                gearslot.SlotInstance.GetComponent<Image>().color = Color.white;

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

                if (slot.Equipment != null)
                {
                    slot.Piece.CleanUP();
                    user.MyInventory.GetInventoryRuntime().UnEquip(slot.Equipment);
                    GearUnEquipped(null);
                    break;
                
                }
    


            }
        }
        protected virtual void TryPlace(List<RaycastResult> results, IInventoryPiece piece)
        {
            foreach (RaycastResult result in results)
            {

                if (gear.GearSlotDic.ContainsKey(result.gameObject) == false) continue;

                IGearSlot slot = gear.GearSlotDic[result.gameObject];

             

                bool allowed = false;
                for (int i = 0; i < piece.EquipmentIdentifier.Length; i++)
                {
                    if (piece.EquipmentIdentifier[i] == slot.Identifier)
                    {
                        //allow
                        allowed = true;
                        break;
                    }
                }
                if (allowed)
                {
                    if (slot.Equipment != null)
                    {
                        //already has something\
                        slot.Piece.CleanUP();
                        user.MyInventory.GetInventoryRuntime().UnEquip(slot.Equipment);
                        GearUnEquipped(null);
                    }

                    slot.Piece = piece;
                    slot.Equipment = piece.ItemStack as Equipment;
                    slot.Piece.Instance.transform.position = slot.SlotInstance.transform.position;
                    slot.Piece.PreviewInstance.transform.position = slot.SlotInstance.transform.position;
                    user.MyInventory.GetInventoryRuntime().Equip(piece.ItemStack as Equipment);
                    GearEquipped(piece);
                }


            }
        }


        #endregion


    }
}