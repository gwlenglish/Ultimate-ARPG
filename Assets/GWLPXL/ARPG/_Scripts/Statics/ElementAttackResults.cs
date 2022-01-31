using GWLPXL.ARPGCore.Combat.com;
using GWLPXL.ARPGCore.Types.com;
using UnityEngine;
namespace GWLPXL.ARPGCore.Statics.com
{
    [System.Serializable]
    public class ElementAttackResults
    {
        public string Source;
        public ElementType Type;//none is physical
        [Tooltip("Damage sent by attacker")]
        public int Damage;//initial damage
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
