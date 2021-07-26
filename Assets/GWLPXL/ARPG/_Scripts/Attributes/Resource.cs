
using GWLPXL.ARPGCore.Types.com;
namespace GWLPXL.ARPGCore.Attributes.com
{

    [System.Serializable]
    public class Resource : Attribute
    {
        public ResourceType Type;
        public int CapValue { get; set; }
        readonly string colon = ": ";
        readonly string divisor = " / ";
        public override void Level(int newLevel, int maxLevel)
        {
            int current = Basevalue;
            int oldMax = CapValue;
            int difference = oldMax - current;
            int newvalue = GetLeveledValue(newLevel, maxLevel);
            SetCapValue(newvalue + difference);
            base.Level(newLevel, maxLevel);
        }
        public override void ModifyBaseValue(int byHowMuch)
        {
            base.ModifyBaseValue(byHowMuch);
            if (NowValue > CapValue)
            {
                NowValue = CapValue;
            }
            else if (NowValue < 0)
            {
                NowValue = 0;
            }
        }
        public override AttributeType GetAttributeType()
        {
            return AttributeType.Resource;
        }

        public virtual void SetCapValue(int newValue)
        {
            CapValue = newValue;
            if (NowValue > newValue)
            {
                NowValue = newValue;
            }
        }

        public virtual void ModifyCapValue(int byHowMuch)
        {
            int newValue = CapValue + byHowMuch;
            SetCapValue(newValue);
        }

        public override string GetDescriptiveName()
        {
            return Type.ToString();
        }

        public override string GetFullDescription()
        {
            return GetDescriptiveName() + colon + NowValue.ToString() + divisor + CapValue.ToString();
        }

        public override int GetSubType() => (int)Type;
       
    }
}