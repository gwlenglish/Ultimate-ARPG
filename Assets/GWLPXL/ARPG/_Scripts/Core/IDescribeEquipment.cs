
using GWLPXL.ARPGCore.Items.com;

namespace GWLPXL.ARPGCore.com
{

    public interface IDescribeEquipment
    {
        void SetHighlightedItem(Item highlightedItem);
        void SetMyEquipment(Equipment myequipment);
        void EnableComparisonPanel();
        void DisableComparisonPanel();
        void DescribeEquipment(string description);
        void DescribeComparisonEquipment(string description);

    }
}