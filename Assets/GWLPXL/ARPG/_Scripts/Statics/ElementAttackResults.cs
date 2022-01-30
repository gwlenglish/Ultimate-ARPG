using GWLPXL.ARPGCore.Combat.com;
using GWLPXL.ARPGCore.Types.com;
using UnityEngine;
namespace GWLPXL.ARPGCore.Statics.com
{
    [System.Serializable]
    public class ElementAttackResults
    {
        public string Source;
        public ElementType Type;
        [Tooltip("Damage sent by attacker")]
        public int Damage;//initial damage
        [Tooltip("Resist from defender")]
        public int Resisted;//resist amount
        [Tooltip("Damage - resisted")]
        public int Reduced;//result of damage - resisted
        [Tooltip("Was a crit?")]
        public bool WasCrit = false;
        public ElementAttackResults(ElementType type, int dmg, string source)
        {
            Source = source;
            Type = type;
            Damage = dmg;

        }
    }
}
