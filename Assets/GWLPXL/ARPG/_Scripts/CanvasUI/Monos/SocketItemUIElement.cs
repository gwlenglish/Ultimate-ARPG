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
        void SetSocketItem(SocketItem item);
        SocketItem GetSocketItem();
    }

    public class SocketItemUIElement : MonoBehaviour, ISocketItemUIElement
    {
        public Image ThingImage = default;
        public TextMeshProUGUI ThingNameText = default;
        public TextMeshProUGUI ThingDescriptionText = default;
        SocketItem item = null;
        public SocketItem GetSocketItem()
        {
            return item;
        }

        public void SetSocketItem(SocketItem item)
        {
            this.item = item;
            Setup(item);
        }


        protected virtual void Setup(SocketItem item)
        {
            if (item == null)
            {
                Debug.LogWarning("Item shouldn't be null and have an instance of it. Something went wrong");
                return;
            }

            ThingImage.sprite = item.GetSprite();
            ThingNameText.SetText(item.GetGeneratedItemName());
            ThingDescriptionText.SetText(item.GetUserDescription());
        }

       
    }
}