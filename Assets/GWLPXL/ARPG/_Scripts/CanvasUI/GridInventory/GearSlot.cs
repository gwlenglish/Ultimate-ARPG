using GWLPXL.ARPGCore.Items.com;
using GWLPXL.ARPGCore.Types.com;
using UnityEngine;

namespace GWLPXL.ARPGCore.CanvasUI.com
{
    public interface IGearSlot
    {
        GameObject SlotInstance { get; set; }
        int Identifier { get; set; }
        IInventoryPiece Piece { get; set; }
        Equipment Equipment { get; set; }
    }


    [System.Serializable]
    public class ARPGGearSlot : IGearSlot
    {
        public Equipment Equipment { get => equipment; set => equipment = value; }
        public GameObject SlotInstance { get => slotInstance; set => slotInstance = value; }
        public int Identifier { get => (int)identifier; set => identifier = (EquipmentSlotsType)value; }
        public IInventoryPiece Piece { get => piece; set => piece = value; }

        [SerializeField]
        protected GameObject slotInstance = default;
        [SerializeField]
        protected EquipmentSlotsType identifier = EquipmentSlotsType.None;
        protected IInventoryPiece piece = null;
        protected Equipment equipment;
       
        public ARPGGearSlot(GameObject iteminstance, Equipment equipment, int identifier)
        {
            this.equipment = equipment;
            slotInstance = iteminstance;
            this.identifier = (EquipmentSlotsType)identifier;
        }
    }
}