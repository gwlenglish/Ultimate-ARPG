using UnityEngine;

namespace GWLPXL.InventoryGrid
{
    public interface IInventoryPiece
    {
        GameObject Instance { get; set; }
        GameObject PreviewInstance { get; set; }
        IPattern Pattern { get; set; }
        int[] EquipmentIdentifier { get; set; }
    }

    /// <summary>
    /// defines an inventory piece
    /// </summary>
    [System.Serializable]
    public class InventoryPiece : IInventoryPiece
    {
        public GameObject Instance { get =>instance; set => instance = value; }
        public GameObject PreviewInstance { get => previewInstance; set => previewInstance = value; }
        public IPattern Pattern { get => pattern; set => pattern = value; }
        public int[] EquipmentIdentifier { get => equipmentIdentifier; set =>equipmentIdentifier = value; }

        [SerializeField]
        protected GameObject instance;
        [SerializeField]
        protected  GameObject previewInstance;
        [SerializeField]
        protected IPattern pattern;
        [SerializeField]
        protected int[] equipmentIdentifier = new int[0];
        public InventoryPiece(GameObject instance, GameObject previewInstance, IPattern pattern, int[] equipID)
        {
            equipmentIdentifier = equipID;
            this.pattern = pattern;
            this.instance = instance;
            this.previewInstance = previewInstance;
            previewInstance.SetActive(false);
        }


    }
}