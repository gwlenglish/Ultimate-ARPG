using GWLPXL.ARPGCore.Attributes.com;
using GWLPXL.ARPGCore.DebugHelpers.com;
using GWLPXL.ARPGCore.Leveling.com;
using GWLPXL.ARPGCore.Statics.com;
using GWLPXL.ARPGCore.Types.com;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ARPGCore.Combat.com
{
    /// <summary>
    /// enemy default combat formulas. Inherit and override to create your own.
    /// </summary>
    [CreateAssetMenu(menuName ="GWLPXL/ARPG/Combat/EnemyDefaultFormulas")]
    public class EnemyDefault : EnemyCombatFormulas
    {
        public override Dictionary<ElementType, ElementAttackResults> GetElementalDamageResistChecks(IAttributeUser defender, IAttributeUser attacker)
        {
            Dictionary<ElementType, ElementAttackResults> attackDic = new Dictionary<ElementType, ElementAttackResults>();
            Attribute[] attackElements = attacker.GetRuntimeAttributes().GetAttributes(AttributeType.ElementAttack);
            for (int i = 0; i < attackElements.Length; i++)
            {
                ElementAttack eleAttack = (ElementAttack)attackElements[i];
                ElementType eletype = eleAttack.Type;
                int attack = eleAttack.NowValue;
                if (attack <= 0) continue;//we didn't do any dmg with this element

                int resist = defender.GetRuntimeAttributes().GetElementResist(eletype);

                int newDmg = attack - resist;
                int resistAmount = attack - resist;//if want to notify resist amounts

                if (newDmg < 0)
                {
                    newDmg = 0;
                }

                if (newDmg > 0)
                {

                    attackDic.TryGetValue(eletype, out ElementAttackResults value);
                    if (value == null)
                    {
                        value = new ElementAttackResults(eletype, newDmg, resistAmount);
                        attackDic[eletype] = value;
                    }
                    else
                    {
                        value.Damage += newDmg;
                        value.Resisted += resistAmount;
                    }
                }

            }

            return attackDic;
        }

        public override int GetArmor(IAttributeUser enemy)
        {
            int scaled = 1;
            IScale scaler = enemy.GetInstance().GetComponent<IScale>();
            if (scaler != null)
            {
                scaled = scaler.GetScaledLevel();
            }
            int armorvalue = scaled + enemy.GetRuntimeAttributes().GetStatForCombat(CombatStatType.Armor);

            ARPGDebugger.CombatDebugMessage(enemy.GetRuntimeAttributes().ActorName + " Armor Value=" + armorvalue, enemy.GetInstance());
            return armorvalue;
        }

        public override int GetReducedPhysical(IAttributeUser enemyStats, int damageAmount)
        {
            int reducedWeaponDamage = GetArmor(enemyStats);
            int dmgValue = damageAmount - reducedWeaponDamage;
            int resisted = damageAmount - dmgValue;
            ARPGDebugger.CombatDebugMessage(reducedWeaponDamage + " reduced weap", null);
            ARPGDebugger.CombatDebugMessage(resisted + ARPGDebugger.GetColorForResist(" resisted amount"), null);


            return dmgValue;
        }

        public override int GetTotalAttackDamage(IAttributeUser enemy)
        {
            return enemy.GetRuntimeAttributes().GetStatForCombat(CombatStatType.Damage);
        }

        public override int GetElementalDamageResistChecks(IAttributeUser enemy, ElementType _type, int elementDamage)
        {
            int resist = enemy.GetRuntimeAttributes().GetElementResist(_type);
            int newDmg = elementDamage - resist;
            if (newDmg < 0) newDmg = 0;

            ARPGDebugger.CombatDebugMessage(enemy.GetRuntimeAttributes().ActorName + ARPGDebugger.GetColorForResist(" Resist Amount= ") + resist + "Damaged Amount=" + newDmg, enemy.GetInstance());
            return newDmg;
        }
    }


}