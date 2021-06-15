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

    public class PlayerDefault : PlayerCombatFormulas
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



        public override int GetArmor(IAttributeUser player, IInventoryUser playerInv)
        {
            int armorAmount = 0;
            armorAmount += player.GetRuntimeAttributes().GetStatForCombat(CombatStatType.Armor);
            armorAmount += playerInv.GetInventoryRuntime().GetArmorFromEquipment();
            return armorAmount;
        }

        public override int GetReducedPhysical(IAttributeUser playerStats, IInventoryUser playerInv, int enemyLevel, float fullDamageAmount)
        {
            float mobMulti = Formulas.MobLevelMultiplier;
            if (DungeonMaster.Instance != null)
            {
                mobMulti = DungeonMaster.Instance.Variables.MobLevelMultiplier;
            }

            float DR = GetArmor(playerStats, playerInv) / (mobMulti * enemyLevel + GetArmor(playerStats, playerInv));
            float reducedPhysical = fullDamageAmount - DR;
            int rounded = Mathf.RoundToInt(reducedPhysical);
        
            return rounded;
        }

        public override int GetTotalAttackDamage(IAttributeUser playerStats, IInventoryUser playerInv, IAbilityUser playerAbilities)
        {
            int baseStatFactor;
            float baseWpnFactor;
            float critFactor;

            //base stat factor * base damage factor * crit factor * skill factor
            ActorAttributes stats = playerStats.GetRuntimeAttributes();

            //not implemented meaningfully yet
            #region crits
            //crits
            float baseCrit = stats.GetOtherAttributeNowValue(OtherAttributeType.CriticalHitChance);
            baseCrit = baseCrit / Formulas.Hundred;
            float critdamage = stats.GetOtherAttributeNowValue(OtherAttributeType.CriticalHitDamage);
            critdamage = critdamage / Formulas.Hundred;
            critFactor = 1 + (baseCrit * critdamage);//this seems wrong, but work on it later
            // Debug.Log(baseCrit + " base crit");
            //Debug.Log(critdamage + " crit dmg");
            //Debug.Log(critFactor + "critfactor");
            int critRando = Random.Range(0, 101);
            float critChance = stats.GetOtherAttributeNowValue(OtherAttributeType.CriticalHitChance);
            if (critRando <= (critChance * Formulas.Hundred))
            {
                //we crit;
                //ARPGDebugger.DebugMessage("Crit!");
            }
            // Debug.Log("Crit Rando: " + critRando);
            //  Debug.Log("crit chance: " + critChance);
            //   Debug.Log("Converted: " + critChance * hundred);
            #endregion

            //from primary stat
            //baseStatFactor = baseStatFactor / Hundred;

            //from equipment
            //Debug.Log(baseStatFactor + " Base stat factor");

            //resolve

            //from abilities
            float baseSkill = 0;
            float skillMods = 0;
            //ability mods
            if (playerAbilities != null)
            {
                if (playerAbilities.GetLastIntendedAbility() != null)
                {
                    baseSkill = playerAbilities.GetLastIntendedAbility().GetDamageMultiplier();
                    baseSkill = Mathf.Round(baseSkill * Formulas.Hundred) / Formulas.Hundred;
                    skillMods = stats.GetAbilityMod(playerAbilities.GetLastIntendedAbility());
                    skillMods = baseSkill + Mathf.Round(skillMods * Formulas.Hundred) / Formulas.Hundred;
                }

            }


            //from elements
            float elementMods = stats.GetAllElementAttackValues();
            elementMods = elementMods / Formulas.Hundred;
            baseStatFactor = stats.GetStatForCombat(CombatStatType.Damage);//current base stat value divide by 100
            baseWpnFactor = playerInv.GetInventoryRuntime().GetDamageFromEquipment();


            float result = (baseWpnFactor) + baseStatFactor + (1 * (float)skillMods) + (1 * (float)elementMods);//main dmg formula
            int rounded = Mathf.FloorToInt(result);

            return rounded;
        }

        public override int GetElementalDamageResistChecks(IAttributeUser enemy, ElementType _type, int elementDamage)
        {
            int resist = enemy.GetRuntimeAttributes().GetElementResist(_type);
            int newDmg = elementDamage - resist;
            int rounded = Mathf.FloorToInt(newDmg);
            if (rounded < 0) rounded = 0;

            ARPGDebugger.CombatDebugMessage(enemy.GetRuntimeAttributes().ActorName + " Resist Amount=" + resist + "Damaged Amount=" + newDmg, enemy.GetInstance());
            return rounded;
        }
    }


}