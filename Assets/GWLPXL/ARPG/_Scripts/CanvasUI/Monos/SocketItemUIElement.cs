using GWLPXL.ARPGCore.Items.com;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace GWLPXL.ARPGCore.CanvasUI.com
{
    public interface ISocketItemUIElement
    {
        void SetSocketItem(int slot, ActorInventory inventory, System.Action<GameObject> cleanupCallback);
        ItemStack GetSocketItem();
        void UpdateItem();
        void UpdateItem(int slot);

    }

    public class SocketItemUIElement : MonoBehaviour, ISocketItemUIElement
    {
        
        public Image ThingImage = default;
        public TextMeshProUGUI ThingNameText = default;
        public TextMeshProUGUI ThingDescriptionText = default;
        int slot;

        System.Action<GameObject> OnCleanUp;
        ActorInventory inventory = null;

        public ItemStack GetSocketItem()
        {
            return inventory.GetItemStackBySlot(slot);
        }

      
        
        public void SetSocketItem(int stackSlot, ActorInventory inventory, System.Action<GameObject> cleanupCallback)
        {
            this.slot = stackSlot;
            this.inventory = inventory;
            OnCleanUp = cleanupCallback;
            Setup();
        }

        public void UpdateItem()
        {
            Setup();
        }

        public void UpdateItem(int slot)
        {
            if (this.slot !=slot) return;
            Setup();

        }

        protected virtual void Setup()
        {
            ItemStack stack = inventory.GetItemStackBySlot(slot);

            if (stack.CurrentStackSize <= 0 || stack.Item == null)
            {
                DestroyMe();
                return;
            }
            ThingImage.sprite = stack.Item.GetSprite();
            ThingNameText.SetText(stack.Item.GetGeneratedItemName());
            ThingDescriptionText.SetText(stack.Item.GetUserDescription());
        }

        protected virtual void DestroyMe()
        {
            inventory.OnSlotChange -= UpdateItem;
            OnCleanUp?.Invoke(this.gameObject);

        }

    }
}