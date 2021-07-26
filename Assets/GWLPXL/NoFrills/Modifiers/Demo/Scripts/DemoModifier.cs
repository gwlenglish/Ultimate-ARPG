

namespace GWLPXL.NoFrills.Modifiers.com
{
    /// <summary>
    /// Demo example of how to extend the modifers to your own system. Inherit ModBase, return the appropriate enum value as an int.
    /// </summary>
    [System.Serializable]
    public class Demo_Modifier : ModBase
    {
        //insert your own Enum here. 
        public DemoStats Stat;

        public override int GetAttributeToModify()
        {
            return (int)Stat;
        }

        public override void SetAttributeToModify(int enumKey)
        {
            Stat = (DemoStats)enumKey;
        }
    }
}