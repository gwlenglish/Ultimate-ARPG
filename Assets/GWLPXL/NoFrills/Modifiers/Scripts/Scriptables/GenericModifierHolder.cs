

using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.NoFrills.Modifiers.com
{
    /// <summary>
    /// Asset instance that holds a modifier. 
    /// </summary>
    [CreateAssetMenu(menuName = "GWLPXL/NoFrills/Modifiers/NEW Generic ModifierHolder")]

    public class GenericModifierHolder : BaseModHolder
    {
        public ModifierType Type = ModifierType.Stats;

        [SerializeField]
        GenericModifier[] Mods = new GenericModifier[0];

        public override IList<ModBase> GetAllModifiers()
        {

            return Mods;
                
        }

      

        public override int GetAttributeType()
        {
            return (int)Type;
        }

        public override ModifierHolderData GetMyData()
        {
            return modifierData;
        }



    }
}