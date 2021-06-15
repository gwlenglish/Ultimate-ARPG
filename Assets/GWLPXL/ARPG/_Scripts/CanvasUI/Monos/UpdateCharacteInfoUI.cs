
using GWLPXL.ARPGCore.Abilities.com;
using GWLPXL.ARPGCore.Attributes.com;
using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Items.com;
using GWLPXL.ARPGCore.Statics.com;
using TMPro;
using UnityEngine;

namespace GWLPXL.ARPGCore.CanvasUI.com
{
    //for the player

    public class UpdateCharacteInfoUI : MonoBehaviour, IDescribePlayerStats
    {
        [SerializeField]
        TextMeshProUGUI text;

        public void DisplayStats(IAttributeUser _stats)
        {
            if (_stats == null) return;
            IInventoryUser inv = _stats.GetInstance().GetComponent<IInventoryUser>();
            IAbilityUser abilities = _stats.GetInstance().GetComponent<IAbilityUser>();
            text.text = PlayerDescription.GetCharacterInfoDescription(_stats, inv, abilities);

        }


    }
}
