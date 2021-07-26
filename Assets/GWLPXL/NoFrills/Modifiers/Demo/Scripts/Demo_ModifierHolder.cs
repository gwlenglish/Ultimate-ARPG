
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.NoFrills.Modifiers.com
{


    /// <summary>
    /// An example class that shows you how to extend the base mod holder to fit your own game's unique context. 
    /// </summary>
    [CreateAssetMenu(menuName = "GWLPXL/NoFrills/Modifiers/NEW DEMO ModifierHolder")]
    public class Demo_ModifierHolder : BaseModHolder
    {
        [SerializeField]
        ModifierType Type = ModifierType.Stats;//we must define a type. 
        [SerializeField]
        Demo_Modifier[] DemoMods;//use the base class you created from ModBase.

        //must override and return our new class
        public override IList<ModBase> GetAllModifiers()
        {
            return DemoMods;
        }

        //must return modifier type
        public override int GetAttributeType()
        {
            return (int)Type;
        }

    }
}