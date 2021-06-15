
using GWLPXL.ARPGCore.Types.com;

namespace GWLPXL.ARPGCore.Attributes.com
{
    /// <summary>
    /// element attack attribute
    /// </summary>
    [System.Serializable]
    public class ElementAttack : Attribute
    {
        public ElementType Type;
        readonly string attack = "Attack: ";
        public ElementAttack(ElementType type)
        {
            Type = type;
        }
        public override AttributeType GetAttributeType()
        {
            return AttributeType.ElementAttack;
        }

        public override string GetDescriptiveName()
        {
            return Type.ToString();
        }

        public override string GetFullDescription()
        {
            return GetDescriptiveName() + attack + NowValue.ToString();
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