using GWLPXL.ARPGCore.Abilities.com;
using GWLPXL.ARPGCore.Attributes.com;
using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.DebugHelpers.com;
using GWLPXL.ARPGCore.Items.com;
using GWLPXL.ARPGCore.Statics.com;
using GWLPXL.ARPGCore.Types.com;
using System;
using System.Collections.Generic;
using System.Linq;
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


        /// <summary>
        /// used for attack value description, calculates damage from equipment and stats
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public override PhysicalAttackResults GetAttackValue(IActorHub user, bool cancrit)
        {

            //ability mods

            int baseStatFactor = user.MyStats.GetRuntimeAttributes().GetStatForCombat(CombatStatType.Damage);//current base stat value divide by 100
            float baseWpnFactor = user.MyInventory.GetInventoryRuntime().GetDamageFromEquipment();
            float baseSkill = 0;
            float skillMods = 0;
            //ability mods
            if (user.MyAbilities != null)
            {
                if (user.MyAbilities.GetLastIntendedAbility() != null)
                {
                    baseSkill = user.MyAbilities.GetLastIntendedAbility().GetDamageMultiplier();
                    baseSkill = Mathf.Round(baseSkill * Formulas.Hundred) / Formulas.Hundred;
                    skillMods = user.MyStats.GetRuntimeAttributes().GetAbilityMod(user.MyAbilities.GetLastIntendedAbility());
                    skillMods = baseSkill + Mathf.Round(skillMods * Formulas.Hundred) / Formulas.Hundred;
                }

            }

            float elementMods = user.MyStats.GetRuntimeAttributes().GetAllElementAttackValues();
            elementMods = elementMods / Formulas.Hundred;

            float critFactor = 1;

            if (cancrit)
            {
                int critRando = UnityEngine.Random.Range(0, 101);
                int critChance = user.MyStats.GetRuntimeAttributes().GetOtherAttributeNowValue(OtherAttributeType.CriticalHitChance);
                if (critRando <= (critChance))
                {

                    int critdamage = user.MyStats.GetRuntimeAttributes().GetOtherAttributeNowValue(OtherAttributeType.CriticalHitDamage);

                    critFactor = 1 + (((float)critChance * (float)critdamage)) / Formulas.Hundred;

                }
            }




            float result = ((baseWpnFactor) + baseStatFactor + (1 * (float)skillMods) + (1 * (float)elementMods)) * critFactor;//main dmg formula
            int rounded = Mathf.FloorToInt(result);



            PhysicalAttackResults phys = new PhysicalAttackResults(rounded, critFactor > 1, "Attack Value");
            return phys;
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



        public override int GetElementResistValue(IActorHub attacker, IActorHub self, ElementType type)
        {
            int resist = self.MyStats.GetRuntimeAttributes().GetElementResist(type);//do something with level differences if you want
            return resist;
        }

        public override CombatResults TakeDamageFormula(AttackValues values, IActorHub self)
        {
            IActorHub attacker = values.Attacker;
            List<ElementAttackResults> elements = values.ElementAttacks;
            List<PhysicalAttackResults> phys = values.PhysicalAttack;

            int totald = 0;

            List<string> sources = new List<string>();
            for (int i = 0; i < phys.Count; i++)
            {
                sources.Add(phys[i].Source);
                totald += phys[i].PhysicalDamage;

            }
            int resisted = GetArmorValue(attacker, self);
            int reduced = totald - resisted;

            PhysicalDamageReport physreport = new PhysicalDamageReport(totald, resisted, reduced, sources);
            List<ElementalDamageReport> elereport = new List<ElementalDamageReport>();
            foreach (ElementType pieceType in Enum.GetValues(typeof(ElementType)))
            {
                for (int i = 0; i < elements.Count; i++)
                {
                    if (elements[i].Type == pieceType)
                    {
                        //found it
                        bool exists = false;
                        for (int j = 0; j < elereport.Count; j++)
                        {
                            if (elereport[j].Type == elements[i].Type)
                            {
                                //has already
                                exists = true;
                                elereport[j].TotalDamage += elements[i].Damage;
                                elereport[j].Sources.Add(elements[i].Source);
                                break;
                            }

                            
                        }

                        if (exists == false)
                        {
                            elereport.Add(new ElementalDamageReport(elements[i].Type, elements[i].Damage, new List<string>(1) { elements[i].Source }));
                        }
                    }
                }
            }

            for (int i = 0; i < elereport.Count; i++)
            {
                elereport[i].Resisted = GetElementResistValue(attacker, self, elereport[i].Type);
                elereport[i].ReducedDamage = elereport[i].TotalDamage - elereport[i].Resisted;
            }

            DamageValues damagevalues = new DamageValues(physreport, elereport, self.MyHealth);
            CombatResults results = new CombatResults(values, damagevalues);
            return results;
            //elements = GetReducedElementalResults(values.Attacker, elements, self);//calculates resist and new reduced values
            //phys = GetReducedPhysical(values.Attacker, phys, self);
            //return values;
        }
    }


}