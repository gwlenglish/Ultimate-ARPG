

using System.Collections.Generic;
using UnityEngine;


namespace GWLPXL.NoFrills.Modifiers.com
{
    /// <summary>
    /// Demo class to show how to add/remove mods. 
    /// </summary>
    public class Demo_ModifyStat : MonoBehaviour
    {
        public DemoStat DemoStat;
        public Modifiable Moddable = null;
        public GenericModifierHolder[] Modifiers = new GenericModifierHolder[0];

        public KeyCode ToggleModsKey;

        bool applied = false;

        private void Start()
        {
            DemoStat.StatRunningValue = DemoStat.StatInitialValue;
        }


        void Update()
        {
            if (Modifiers == null)
            {
                Debug.LogError("Need a ModHolder in order to use Mods", this);
                return;
            }

            if (Input.GetKeyDown(ToggleModsKey))
            {
                //toggles the mods for demo purposes. look to see how they are added/removed
                ToggleMods();

                //update our value based on our toggle result
                //this line right here is what you would do with your own system. YOu get the base value and then add the mods to it.
                DemoStat.StatRunningValue = DemoStat.StatInitialValue + 
                    Moddable.GetModValue((int)ModifierType.Stats, (int)DemoStat.StatType, DemoStat.StatInitialValue);

        
            }
        }

        //this needs refactoring to a new script
        //knock this down into two scripts, one to remove, one to add.
        void ToggleMods()
        {
            applied = !applied;
            if (applied)
            {
                Moddable.AddModifiers(Modifiers);
            }
            else
            {
                Moddable.RemoveModifiers(Modifiers);
            }
        }
    }
}