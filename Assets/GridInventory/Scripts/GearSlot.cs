using GWLPXL.ARPGCore.Types.com;
using UnityEngine;

namespace GWLPXL.InventoryGrid
{
    public interface IGearSlot
    {
        GameObject SlotInstance { get; set; }
        int Identifier { get; set; }
        IInventoryPiece Piece { get; set; }
    }

    /// <summary>
    /// defines a gear slot
    /// </summary>
    [System.Serializable]
    public class GearSlot: IGearSlot
    {
        public GameObject SlotInstance { get => slotInstance; set => slotInstance = value; }
        public int Identifier { get =>identifier; set => identifier = value; }
        public IInventoryPiece Piece { get => piece; set => piece = value; }
        [SerializeField]
        protected GameObject slotInstance = default;
        [SerializeField]
        protected int identifier = 0;
        protected IInventoryPiece piece = null;
        public GearSlot(GameObject iteminstance, int identifier)
        {
            slotInstance = iteminstance;
            Identifier = identifier;
        }


    }

    [System.Serializable]
    public class ARPGGearSlot : IGearSlot
    {
        public GameObject SlotInstance { get => slotInstance; set => slotInstance = value; }
        public int Identifier { get => (int)identifier; set => identifier = (EquipmentSlotsType)value; }
        public IInventoryPiece Piece { get => piece; set => piece = value; }

        [SerializeField]
        protected GameObject slotInstance = default;
        [SerializeField]
        protected EquipmentSlotsType identifier = EquipmentSlotsType.None;
        protected IInventoryPiece piece = null;

       
        public ARPGGearSlot(GameObject iteminstance, int identifier)
        {
            slotInstance = iteminstance;
            this.identifier = (EquipmentSlotsType)identifier;
        }
    }
}