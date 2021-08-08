using GWLPXL.ARPGCore.Types.com;
namespace GWLPXL.ARPGCore.Attributes.com
{
    /// <summary>
    /// element resist
    /// </summary>
    [System.Serializable]
    public class ElementResist : Attribute
    {
        public ElementType Type;
        readonly string resist = "Resist: ";
        public override AttributeType GetAttributeType()
        {
            return AttributeType.ElementResist;
        }

        public override string GetDescriptiveName()
        {
            return Type.ToString();
        }

        public override string GetFullDescription()
        {
            return GetDescriptiveName() + resist + NowValue.ToString();
        }

        //do nothing, elements don't level.
        public override void Level(int newLevel, int maxLevel)
        {
            //do nothing, elements don't level.
        }
        //do nothing, elements don't level.
        protected override int GetLeveledValue(int forLevel, int myMaxLevel)
        {
            return NowValue;
        }

        public override int GetSubType() => (int)Type;
        
    }


}