using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GWLPXL.ARPGCore.Items.com;
using UnityEngine.UI;
using TMPro;
namespace GWLPXL.ARPGCore.CanvasUI.com
{
    public interface IEnchantableUIElement
    {
        void SetEnchantable(Item item);
        Item GetEnchantable();
    }

    public class EnchantableUIElement : MonoBehaviour, IEnchantableUIElement
    {
        public TextMeshProUGUI ItemDescriptionText = null;
        public Image ItemImage;
        public string EmptyText = "Empty";
        public Sprite EmptySprite = null;
        Item item;

        private void Awake()
        {
            ItemDescriptionText.SetText(string.Empty);
            ItemImage.sprite = null;
        }
        public void SetEnchantable(Item item)
        {
            this.item = item;
            if (item == null)
            {
                ItemImage.sprite = EmptySprite;
                ItemDescriptionText.SetText(EmptyText);
            }
            else
            {
                ItemImage.sprite = item.GetSprite();
                ItemDescriptionText.SetText(item.GetUserDescription());
            }


        }

        public Item GetEnchantable()
        {
            return item;
        }
    }
}