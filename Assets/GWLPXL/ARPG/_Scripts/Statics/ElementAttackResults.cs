using GWLPXL.ARPGCore.Combat.com;
using GWLPXL.ARPGCore.Types.com;

namespace GWLPXL.ARPGCore.Statics.com
{
    [System.Serializable]
    public class ElementAttackResults
    {
        public string Source;
        public ElementType Type;
        public int Damage;//initial damage
        public int Resisted;//resist amount
        public int Reduced;//result of damage - resisted
        public ElementAttackResults(ElementType type, int dmg, string source)
        {
            Source = source;
            Type = type;
            Damage = dmg;

        }
    }
}
