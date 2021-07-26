
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GWLPXL.NoFrills.Modifiers.com;
using GWLPXL.ARPGCore.Types.com;

namespace GWLPXL.ARPGCore.com
{


    public class ModTester : MonoBehaviour
    {
        public KeyCode Keycode = KeyCode.Space;
        public BaseModHolder Mod;
        bool applied;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(Keycode))
            {
                if (applied)
                {
                    //remove
                    applied = false;
                    GetComponent<IActorHub>().MyStats.GetRuntimeAttributes().RemoveMod(Mod);
                }
                else
                {
                    GetComponent<IActorHub>().MyStats.GetRuntimeAttributes().AddMod(Mod);

                    applied = true;
                }
            }

            for (int i = 0; i < Mod.GetAllModifiers().Count; i++)
            {
                GetComponent<IActorHub>().MyStats.GetRuntimeAttributes().GetStatNowValue((StatType)Mod.GetAllModifiers()[i].GetAttributeToModify());
            }
  
            IReadOnlyDictionary<int, ModifiableDictionary> dic =  GetComponent<IActorHub>().MyStats.GetRuntimeAttributes().Modifiable.GetAllModifiers();
            foreach (var kvp in dic)
            {
                Debug.Log("Attribute " + (AttributeType)kvp.Key);

                Debug.Log("Mods " + kvp.Value.ModHoldersDic.Count);
            }



        }
    }
}