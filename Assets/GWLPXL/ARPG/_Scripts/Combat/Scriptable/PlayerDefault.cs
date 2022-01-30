using GWLPXL.ARPGCore.Abilities.com;
using GWLPXL.ARPGCore.Attributes.com;
using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.DebugHelpers.com;
using GWLPXL.ARPGCore.Items.com;
using GWLPXL.ARPGCore.Statics.com;
using GWLPXL.ARPGCore.Types.com;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ARPGCore.Combat.com
{
    /// <summary>
    /// player default combat formulas. Inherit and override to write your own.
    /// </summary>
    [CreateAssetMenu(menuName = "GWLPXL/ARPG/Combat/PlayerDefaultFormulas")]

    public class PlayerDefault : ActorCombatFormulas
    {
   

       
       
        /// <summary>
        //float result = ((baseWpnFactor) + baseStatFactor + (1 * (float)skillMods) + (1 * (float)elementMods)) * critFactor;//main dmg formula
        //return rounded int
        /// </summary>
        /// <param name="playerStats"></param>
        /// <param name="playerInv"></param>
        /// <param name="playerAbilities"></param>
        /// <returns></returns>
        //public override int GetTotalAttackDamage(IAttributeUser playerStats, IInventoryUser playerInv, IAbilityUser playerAbilities)
        //{

        //    //base stat factor * base damage factor * crit factor * skill factor
        //    ActorAttributes stats = playerStats.GetRuntimeAttributes();

        //    //not implemented meaningfully yet
        //    #region crits

        //    float critFactor = 1;
        //    int critRando = Random.Range(0, 101);
        //    int critChance = stats.GetOtherAttributeNowValue(OtherAttributeType.CriticalHitChance);
        //    if (critRando <= (critChance))
        //    {

        //        int critdamage = stats.GetOtherAttributeNowValue(OtherAttributeType.CriticalHitDamage);

        //        critFactor = 1 + (((float)critChance * (float)critdamage))/Formulas.Hundred;

        //        //we crit;
        //        //ARPGDebugger.DebugMessage("Crit!");
        //    }

        //    #endregion

        //    float baseSkill = 0;
        //    float skillMods = 0;
        //    //ability mods
        //    if (playerAbilities != null)
        //    {
        //        if (playerAbilities.GetLastIntendedAbility() != null)
        //        {
        //            baseSkill = playerAbilities.GetLastIntendedAbility().GetDamageMultiplier();
        //            baseSkill = Mathf.Round(baseSkill * Formulas.Hundred) / Formulas.Hundred;
        //            skillMods = stats.GetAbilityMod(playerAbilities.GetLastIntendedAbility());
        //            skillMods = baseSkill + Mathf.Round(skillMods * Formulas.Hundred) / Formulas.Hundred;
        //        }

        //    }


        //    //from elements
        //    float elementMods = stats.GetAllElementAttackValues();
        //    elementMods = elementMods / Formulas.Hundred;
        //    int baseStatFactor = stats.GetStatForCombat(CombatStatType.Damage);//current base stat value divide by 100
        //    float baseWpnFactor = playerInv.GetInventoryRuntime().GetDamageFromEquipment();


        //    float result = ((baseWpnFactor) + baseStatFactor + (1 * (float)skillMods) + (1 * (float)elementMods)) * critFactor;//main dmg formula
        //    int rounded = Mathf.FloorToInt(result);

        //    if (critFactor > 1)
        //    {
        //        //was crit
        //        CritHelper.Crits.Add(new CritLog(playerStats, rounded));//since we're just passing values around, this class will save whose crit this was so we can tell the UI
        //    }
        //    return rounded;
        //}
       
        /// <summary>
        /// used for attack value description, calculates damage from equipment and stats
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public override int GetAttackValue(IActorHub user)
        {

            //ability mods
        
            int baseStatFactor = user.MyStats.GetRuntimeAttributes().GetStatForCombat(CombatStatType.Damage);//current base stat value divide by 100
            float baseWpnFactor = user.MyInventory.GetInventoryRuntime().GetDamageFromEquipment();


            float result = ((baseWpnFactor) + baseStatFactor);
            int rounded = Mathf.FloorToInt(result);
            return rounded;
        }

        /// <summary>
        /// used for armor value description, calculates armor from stats and equipment
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public override int GetArmorValue(IActorHub attacker, IActorHub self)
        {
            //do something with attacker level if you want
            int armorAmount = 0;
            armorAmount += self.MyStats.GetRuntimeAttributes().GetStatForCombat(CombatStatType.Armor);
            armorAmount += self.MyInventory.GetInventoryRuntime().GetArmorFromEquipment();
            return armorAmount;
        }

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

        public override int GetElementResistValue(IActorHub attacker, IActorHub self, ElementType type)
        {
            int resist = self.MyStats.GetRuntimeAttributes().GetElementResist(type);//do something with level differences if you want
            return resist;
        }

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