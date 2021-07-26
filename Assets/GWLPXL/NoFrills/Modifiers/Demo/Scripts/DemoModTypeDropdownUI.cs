
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace GWLPXL.NoFrills.Modifiers.com
{


    public class DemoModTypeDropdownUI : MonoBehaviour
    {
        public Dropdown Dropdown;
        // Start is called before the first frame update
        void Start()
        {
            List<string> enums = new List<string>();
            enums = System.Enum.GetNames(typeof(ModValueType)).ToList();
            Dropdown.ClearOptions();
            Dropdown.AddOptions(enums);
        }


    }
}