
using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Items.com;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GWLPXL.ARPGCore.CanvasUI.com
{
    
    public class HoverOverInfo : MonoBehaviour, IDescribeEquipment
    {
        [SerializeField]
        TextMeshProUGUI highlightedText = null;
        [SerializeField]
        TextMeshProUGUI comparison = null;
        [SerializeField]
        Image equipmentImage = null;
        [SerializeField]
        Image comparisonImage = null;
        [SerializeField]
        Image mine = null;
        [SerializeField]
        Image highlighted = null;
      
        private void Awake()
        {
            mine.sprite = null;
            highlighted.sprite = null;
        }
        /// <summary>
        /// my equipment
        /// </summary>
        /// <param name="description"></param>
        public void DescribeComparisonEquipment(string description)
        {
            comparison.text = description;
            if (string.IsNullOrEmpty(description))
            {
                comparison.text = "None";
                float alpha = 1;
                Color temp = comparisonImage.color;
                temp.a = alpha;
                comparisonImage.color = temp;
            }
            else
            {
                float alpha = 1;
                Color temp = comparisonImage.color;
                temp.a = alpha;
                comparisonImage.color = temp;
            }
        }

        public void DescribeEquipment(string description)
        {
            highlightedText.SetText(description);
        }

        public void DisableComparisonPanel()
        {
            comparisonImage.gameObject.SetActive(false);
        }

        public void EnableComparisonPanel()
        {
            comparisonImage.gameObject.SetActive(true);
        }

        public void SetHighlightedItem(Item highlighteditem)
        {
            if (highlighteditem == null)
            {
                highlighted.sprite = null;
                DescribeEquipment(string.Empty);
                return;
            }
            highlighted.sprite = highlighteditem.GetSprite();
        }

        public void SetMyEquipment(Equipment myequipment)
        {
            if (myequipment == null)
            {
                mine.sprite = null;
                DescribeComparisonEquipment(string.Empty);
                return;
            }
            mine.sprite = myequipment.GetSprite();
        }
    }
}