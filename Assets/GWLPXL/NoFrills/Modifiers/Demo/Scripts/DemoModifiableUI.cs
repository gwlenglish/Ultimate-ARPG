
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace GWLPXL.NoFrills.Modifiers.com
{

    /// <summary>
    /// Used for Demo purposes. 
    /// </summary>
    public class DemoModifiableUI : MonoBehaviour
    {
        public Modifiable Modifiable;
        public DemoStat DemoStats;

        public Text StatName;
        public Text OriginalValue;
        public Text ModValue;
        public Text CurrentValue;

        StringBuilder sb = new StringBuilder();
      
        private void Update()
        {
            ///obviously you wouldnt write this every frame in a real game, here just for demo. 

            DemoStats.StatRunningValue = DemoStats.StatInitialValue + 
                Modifiable.GetModValue((int)ModifierType.Stats, (int)DemoStats.StatType, DemoStats.StatInitialValue);
        }
        private void LateUpdate()
        {
            ///obviously you wouldnt write this every frame in a real game, here just for demo. 
            UpdateModListText();
            UpdateText();
        }

        void UpdateText()
        {
            StatName.text = DemoStats.StatType.ToString();
            OriginalValue.text = DemoStats.StatInitialValue.ToString();
            CurrentValue.text = DemoStats.StatRunningValue.ToString();
        }
        void UpdateModListText()
        {

            List<ModBase> _temp = Modifiable.GetAllMods((int)ModifierType.Stats, (int)DemoStats.StatType);
            sb.Clear();
            for (int i = 0; i < _temp.Count; i++)
            {
                string pos = " + ";
                if (_temp[i].GetAppliedValue() < 0)
                {
                    pos = " - ";
                }
                sb.Append(pos + _temp[i].GetAppliedValue() + " Mods" + "\n");


            }
            ModValue.text = sb.ToString();
        }

    }
}