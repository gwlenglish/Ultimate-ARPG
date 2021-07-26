using UnityEngine;

namespace GWLPXL.NoFrills.Modifiers.com
{

    /// <summary>
    /// Generic mod. See the DemoModifier to see an example of how to extend ModBase and link it to your current enum typed stats.
    /// </summary>
    [System.Serializable]
    public class GenericModifier : ModBase
    {
        [SerializeField]
        protected int AttributeKey;

        public override void SetAttributeToModify(int enumKey)
        {
            AttributeKey = enumKey;
        }
        public override int GetAttributeToModify()
        {
            return AttributeKey;
        }
      

     
    }



   
}