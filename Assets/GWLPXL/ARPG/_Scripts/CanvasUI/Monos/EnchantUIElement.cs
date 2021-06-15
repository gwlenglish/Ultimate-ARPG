using GWLPXL.ARPGCore.Traits.com;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
namespace GWLPXL.ARPGCore.CanvasUI.com
{
    public interface IEnchantUIElement
    {
        void SetEnchant(EquipmentTrait item);
        EquipmentTrait GetEnchant();
    }

    public class EnchantUIElement : MonoBehaviour, IEnchantUIElement
    {
        public TextMeshProUGUI ItemDescriptionText = null;
        public Image ItemImage;
        public string EmptyText = string.Empty;
        public Sprite EmptySprite = null;
        EquipmentTrait enchant;

        private void Awake()
        {
            ItemDescriptionText.SetText(string.Empty);
            ItemImage.sprite = null;
        }
      

        public void SetEnchant(EquipmentTrait enchant)
        {
            this.enchant = enchant;
            if (enchant == null)
            {
                ItemImage.sprite = EmptySprite;
                ItemDescriptionText.SetText(EmptyText);
            }
            else
            {
                ItemImage.sprite = enchant.GetSprite();
                ItemDescriptionText.SetText(enchant.GetTraitUIDescription());
            }


        }

        public EquipmentTrait GetEnchant()
        {
            return this.enchant;
        }
    }
}