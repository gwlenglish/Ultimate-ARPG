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
}