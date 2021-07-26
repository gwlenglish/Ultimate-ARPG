using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace GWLPXL.NoFrills.Modifiers.com
{
    /// <summary>
    /// Used in demo. 
    /// </summary>
    public class DemoDropdownCreate : MonoBehaviour
    {
        public Dropdown DropDown;

        void Start()
        {
            List<string> enums = new List<string>();
            enums = System.Enum.GetNames(typeof(DemoStats)).ToList();
            DropDown.ClearOptions();
            DropDown.AddOptions(enums);
        }

      
    }
}