using GWLPXL.ARPGCore.Items.com;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace GWLPXL.ARPGCore.CanvasUI.com
{
    public interface ISocketItemUIElement
    {
        void SetSocketItem(ItemStack item);
        ItemStack GetSocketItem();
    }

    public class SocketItemUIElement : MonoBehaviour, ISocketItemUIElement
    {
        public Image ThingImage = default;
        public TextMeshProUGUI ThingNameText = default;
        public TextMeshProUGUI ThingDescriptionText = default;
        ItemStack item = null;
        public ItemStack GetSocketItem()
        {
            return item;
        }

        public void SetSocketItem(ItemStack item)
        {
            this.item = item;
            Setup(item);
        }


        protected virtual void Setup(ItemStack item)
        {
            if (item == null)
            {
                Debug.LogWarning("Item shouldn't be null and have an instance of it. Something went wrong");
                return;
            }

            ThingImage.sprite = item.Item.GetSprite();
            ThingNameText.SetText(item.Item.GetGeneratedItemName());
            ThingDescriptionText.SetText(item.Item.GetUserDescription());
        }

       
    }
}