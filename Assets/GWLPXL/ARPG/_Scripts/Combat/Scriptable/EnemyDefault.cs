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
    public class EnemyDefault : ActorCombatFormulas
    {
        /// <summary>
        /// calculate the REDUCED damage amount based on the self armor value
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="results"></param>
        /// <param name="self"></param>
        /// <returns></returns>
        public override List<PhysicalAttackResults> GetReducedPhysical(IActorHub attacker, List<PhysicalAttackResults> results, IActorHub self)
        {
            if (self.MyStats == null) return results;
            for (int i = 0; i < results.Count; i++)
            {
                results[i].PhysicalResisted = GetArmorValue(attacker, self);
                results[i].PhysicalReduced = results[i].PhysicalDamage - results[i].PhysicalResisted;
                if (results[i].PhysicalReduced < 0) results[i].PhysicalReduced = 0;
            }

            return results;
        }
        /// <summary>
        /// calculate the REDUCED damage amount based on the self resist value
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="attackvalues"></param>
        /// <param name="self"></param>
        /// <returns></returns>
        public override List<ElementAttackResults> GetReducedElementalResults(IActorHub attacker, List<ElementAttackResults> attackvalues, IActorHub self)
        {
            if (self.MyStats == null) return attackvalues;
            for (int i = 0; i < attackvalues.Count; i++)
            {
                attackvalues[i].Resisted = GetElementResistValue(attacker, self, attackvalues[i].Type);
                attackvalues[i].Reduced = attackvalues[i].Damage - attackvalues[i].Resisted;
                if (attackvalues[i].Reduced < 0) attackvalues[i].Reduced = 0;
            }
            return attackvalues;
        }

       

        /// <summary>
        /// calculate the armor value of the self
        /// </summary>
        /// <param name="enemy"></param>
        /// <returns></returns>
        public override int GetArmorValue(IActorHub attacker, IActorHub self)
        {
            int attackerLevel = attacker.MyStats.GetRuntimeAttributes().MyLevel;
            int scaled = 1;
            IScale scaler = self.MyStats.GetInstance().GetComponent<IScale>();
            if (scaler != null)
            {
                scaled = scaler.GetScaledLevel();
            }
            int armorvalue = scaled + self.MyStats.GetRuntimeAttributes().GetStatForCombat(CombatStatType.Armor);//figure attacker level here somewhere.

            ARPGDebugger.CombatDebugMessage(self.MyStats.GetRuntimeAttributes().ActorName + " Armor Value=" + armorvalue, self.MyStats.GetInstance());
            return armorvalue;
        }

       
        /// <summary>
        /// calculcate the attack value of the self
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public override int GetAttackValue(IActorHub self)
        {

            int baseStatFactor = self.MyStats.GetRuntimeAttributes().GetStatForCombat(CombatStatType.Damage);//current base stat value divide by 100

            float baseWpnFactor = 0;
            if (self.MyInventory != null)
            {
                baseWpnFactor = self.MyInventory.GetInventoryRuntime().GetDamageFromEquipment();
            }
        

            float result = ((baseWpnFactor) + baseStatFactor);
            int rounded = Mathf.FloorToInt(result);
            return rounded;
        }

        /// <summary>
        /// calculate the element resist value of the self
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="self"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public override int GetElementResistValue(IActorHub attacker, IActorHub self, ElementType type)
        {
            int resist = self.MyStats.GetRuntimeAttributes().GetElementResist(type);//do something with level values if you want
            return resist;
        }

        /// <summary>
        /// calculate the take damage formula, the main call that determines physical and element reduction
        /// </summary>
        /// <param name="values"></param>
        /// <param name="self"></param>
        /// <returns></returns>
        public override AttackValues TakeDamageFormula(AttackValues values, IActorHub self)
        {
            IActorHub attacker = values.Attacker;
            List<ElementAttackResults> elements = values.ElementAttacks;
            List<PhysicalAttackResults> phys = values.PhysicalAttack;

            elements = GetReducedElementalResults(values.Attacker, elements, self);//calculates resist and new reduced values
            phys = GetReducedPhysical(values.Attacker, phys, self);
            return values;
        }
    }


}