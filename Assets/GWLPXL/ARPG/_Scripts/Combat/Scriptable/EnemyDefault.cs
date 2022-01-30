using GWLPXL.ARPGCore.Attributes.com;
using GWLPXL.ARPGCore.com;
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
        public override List<PhysicalAttackResults> GetReducedPhysical(List<PhysicalAttackResults> results, IActorHub self)
        {
            if (self.MyStats == null) return results;
            for (int i = 0; i < results.Count; i++)
            {
                results[i].PhysicalResisted = self.MyStats.GetRuntimeAttributes().GetStatForCombat(CombatStatType.Armor);
                results[i].PhysicalReduced = results[i].PhysicalDamage - results[i].PhysicalResisted;
                if (results[i].PhysicalReduced < 0) results[i].PhysicalReduced = 0;
            }

            return results;
        }
        public override List<ElementAttackResults> GetReducedResults(List<ElementAttackResults> attackvalues, IActorHub self)
        {
            if (self.MyStats == null) return attackvalues;
            for (int i = 0; i < attackvalues.Count; i++)
            {
                attackvalues[i].Resisted = self.MyStats.GetRuntimeAttributes().GetElementResist(attackvalues[i].Type);//grab oru resist
                attackvalues[i].Reduced = attackvalues[i].Damage - attackvalues[i].Resisted;
                if (attackvalues[i].Reduced < 0) attackvalues[i].Reduced = 0;
            }
            return attackvalues;
        }
        /// <summary>
        /// simple +- calculation against damage and resist, e.g. 25 damage and 12 resist = 13 result
        /// </summary>
        /// <param name="defender"></param>
        /// <param name="attacker"></param>
        /// <returns></returns>
        /// 
        [System.Obsolete("Use Get Reduced Results")]
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
                        value = new ElementAttackResults(eletype, newDmg, "Deprecated Source");
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

        /// <summary>
        /// returns armor combat value on attributes plus if using scaler
        /// </summary>
        /// <param name="enemy"></param>
        /// <returns></returns>
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
        /// <summary>
        /// damageamount - armor
        /// </summary>
        /// <param name="enemyStats"></param>
        /// <param name="damageAmount"></param>
        /// <returns></returns>
        public override int GetReducedPhysical(IAttributeUser enemyStats, int damageAmount)
        {
            int reducedWeaponDamage = GetArmor(enemyStats);
            int dmgValue = damageAmount - reducedWeaponDamage;
            int resisted = damageAmount - dmgValue;
            ARPGDebugger.CombatDebugMessage(reducedWeaponDamage + " reduced weap", null);
            ARPGDebugger.CombatDebugMessage(resisted + ARPGDebugger.GetColorForResist(" resisted amount"), null);


            return dmgValue;
        }
        /// <summary>
        /// get stat for combat damage
        /// </summary>
        /// <param name="enemy"></param>
        /// <returns></returns>
        public override int GetTotalAttackDamage(IAttributeUser enemy)
        {
            return enemy.GetRuntimeAttributes().GetStatForCombat(CombatStatType.Damage);
        }
        /// <summary>
        /// elementdamage - resist
        /// </summary>
        /// <param name="enemy"></param>
        /// <param name="_type"></param>
        /// <param name="elementDamage"></param>
        /// <returns></returns>
        public override int GetElementalDamageResistChecks(IAttributeUser enemy, ElementType _type, int elementDamage)
        {
            int resist = enemy.GetRuntimeAttributes().GetElementResist(_type);
            int newDmg = elementDamage - resist;
            if (newDmg < 0) newDmg = 0;

            ARPGDebugger.CombatDebugMessage(enemy.GetRuntimeAttributes().ActorName + ARPGDebugger.GetColorForResist(" Resist Amount= ") + resist + "Damaged Amount=" + newDmg, enemy.GetInstance());
            return newDmg;
        }

        public override int GetAttackValue(IActorHub user)
        {

            int baseStatFactor = user.MyStats.GetRuntimeAttributes().GetStatForCombat(CombatStatType.Damage);//current base stat value divide by 100
            float baseWpnFactor = user.MyInventory.GetInventoryRuntime().GetDamageFromEquipment();

            float result = ((baseWpnFactor) + baseStatFactor);
            int rounded = Mathf.FloorToInt(result);
            return rounded;
        }
    }


}