using GWLPXL.ARPGCore.Items.com;
using UnityEngine;

namespace GWLPXL.InventoryGrid
{
    public interface IInventoryPiece
    {
        GameObject Instance { get; set; }
        GameObject PreviewInstance { get; set; }
        Item ItemStack { get; set; }
        IPattern Pattern { get; set; }
        int[] EquipmentIdentifier { get; set; }
        void CleanUP();
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
        public Item ItemStack { get => itemstack; set => itemstack = value; }

        [SerializeField]
        protected GameObject instance;
        [SerializeField]
        protected  GameObject previewInstance;
        [SerializeField]
        protected IPattern pattern;
        [SerializeField]
        protected int[] equipmentIdentifier = new int[0];
        [SerializeField]
        protected Item itemstack;
        public InventoryPiece(GameObject instance, GameObject previewInstance, Item item)
        {
            this.itemstack = item;
            equipmentIdentifier = item.EquipmentIdentifier;
            this.pattern = item.UIPattern.Pattern;
            this.instance = instance;
            this.previewInstance = previewInstance;
            previewInstance.SetActive(false);
        }


        public virtual void CleanUP()
        {
            GameObject.Destroy(instance);
            GameObject.Destroy(previewInstance);
            pattern = null;
            itemstack = null;
            equipmentIdentifier = new int[0];
        }


    }
}