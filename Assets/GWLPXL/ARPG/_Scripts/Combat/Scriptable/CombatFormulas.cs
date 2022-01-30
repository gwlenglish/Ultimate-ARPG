using GWLPXL.ARPGCore.Abilities.com;
using GWLPXL.ARPGCore.Attributes.com;
using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Items.com;
using GWLPXL.ARPGCore.Statics.com;
using GWLPXL.ARPGCore.Types.com;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GWLPXL.ARPGCore.DebugHelpers.com;

using GWLPXL.ARPGCore.StatusEffects.com;
using System.Linq;


namespace GWLPXL.ARPGCore.Combat.com
{
    /// <summary>
    /// Formulas for combat. Inherit and override to create your own.
    /// </summary>
    /// 
    [CreateAssetMenu(menuName = "GWLPXL/ARPG/Combat/CombatFormulas")]
    public class CombatFormulas : CommonCombatFormulas
    {
        public PlayerCombatFormulas PlayerCombat = null;
        public EnemyCombatFormulas EnemyCombat = null;


        #region public methods
        /// <summary>
        /// convenience function to determine if gameobject is player or enemy and get the total damage value
        /// </summary>
        /// <param name="toGet"></param>
        /// <returns></returns>
        public override int GetTotalActorDamage(GameObject toGet)
        {
            if (toGet == null) return 0;
            IActorHub actorhub = toGet.GetComponent<IActorHub>();//hub for the actor
            IAttributeUser stats = actorhub.MyStats;
            if (actorhub.PlayerControlled != null)
            {
                IInventoryUser playerInv = actorhub.MyInventory;
                IAbilityUser playerAbilities = actorhub.MyAbilities;
                return PlayerCombat.GetTotalAttackDamage(stats, playerInv, playerAbilities);
            }
            else
            {
                return EnemyCombat.GetTotalAttackDamage(stats);
            }
        }
        /// <summary>
        /// performs the projectile damage
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="damageDealer"></param>
        /// <param name="damageTarget"></param>
        /// <param name="damage"></param>
        /// <param name="projectileOptions"></param>
        public override AttackValues GetProjectileDamage(AttackValues values, IActorHub owner, IDoDamage damageDealer, IActorHub damageTarget, IDoActorDamage damage, IProjectile projectileOptions)
        {
         


            if (damage.GetActorDamageData().DamageVar.DamageOptions.InflictPhysicalDmg)
            {
                int phys = damage.GetActorDamageData().DamageVar.DamageMultipliers.PhysMultipler.GetPhysicalDamageAmount(owner);
                values.PhysicalAttack.Add(new PhysicalAttackResults(phys, false, "Projectile"));
                damage.GetActorDamageEvents().SceneEvents.OnPhysicalDamagedOther.Invoke(phys, damageTarget.MyHealth);

            }

            if (damage.GetActorDamageData().DamageVar.DamageOptions.InfictElementalDmg)
            {
                for (int i = 0; i < damage.GetActorDamageData().DamageVar.DamageMultipliers.ElementMultiplers.Length; i++)
                {
                    ElementDamageMultiplierActor elev = damage.GetActorDamageData().DamageVar.DamageMultipliers.ElementMultiplers[i];
                    values.ElementAttacks.Add(new ElementAttackResults(elev.DamageType, elev.GetElementDamageAmount(owner), "Projectile"));
                    damage.GetActorDamageEvents().SceneEvents.OnElementalDamageOther.Invoke(elev.GetElementDamageAmount(owner), damageTarget.MyHealth);
                }
    
             

            }

            int modlength = damageDealer.GetWeaponMods().Length;
            for (int i = 0; i < modlength; i++)
            {
                damageDealer.GetWeaponMods()[i].DoModification(values);
            }

            damageDealer.GetDamagedList().Add(damageTarget);

            if (projectileOptions.GetProjectileData().ProjectileVars.DisableOnTouch && projectileOptions.Disabled == false)//buffered destroy, destroy happens in the dotick check
            {
                projectileOptions.Disabled = true;
            }

            damageTarget.MyHealth.SetCharacterThatHitMe(owner);
            return values;
        }
        /// <summary>
        /// performs the SoTs damage 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="damage"></param>
        public override void SoTsDamageLogic(IActorHub target, IDoActorDamage damage)
        {
            int eleDmg = damage.GetActorDamageData().DamageVar.CombatHandler.DoElementalDamageOverTime(target.MyHealth, damage.GetActorDamageData().DamageVar.SoTOverTimeMultipliers.ElementalMultipliers);
            damage.GetActorDamageEvents().SceneEvents.OnElementalDamageOther.Invoke(eleDmg, target.MyHealth);
        }
        /// <summary>
        /// performs the sots apply logic
        /// </summary>
        /// <param name="target"></param>
        /// <param name="damage"></param>
        /// <param name="sots"></param>
        public override void SoTsApplyLogic(IActorHub target, IDoActorDamage damage, IApplySOT sots)
        {
            bool dotsadded = damage.GetActorDamageData().DamageVar.CombatHandler.AddDOTS(target, damage.GetActorDamageData().DamageVar.SoTOptions.AdditionalDOTs);
            if (dotsadded)
            {
                sots.GetSOTEvents().SceneEvents.OnSoTApply.Invoke(target.MyStatusEffects);
            }
        }
        /// <summary>
        /// performs any exit logic
        /// </summary>
        /// <param name="target"></param>
        /// <param name="damage"></param>
        /// <param name="sots"></param>
        public override void OnExitSoTLogic(IActorHub target, IDoActorDamage damage, IApplySOT sots)
        {
            if (target == null) return;
            if (damage.GetActorDamageData().DamageVar.DamageOptions.InflictSoT == false) return;
            if (damage.GetActorDamageData().DamageVar.SoTOptions.ApplyAtExit == false) return;

            for (int i = 0; i < sots.GetSOTS().Count; i++)
            {
                if (sots.GetSOTS()[i].Attackable == target)
                {
                    //damage
                    SoTsDamageLogic(target, damage);
                    SoTsApplyLogic(target, damage, sots);
                    sots.GetSOTS().RemoveAt(i);
                    break;
                }
            }
        }
        /// <summary>
        /// performs enter logic
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="target"></param>
        /// <param name="damage"></param>
        /// <param name="sots"></param>
        public override void OnEnterSotLogic(IActorHub owner, IActorHub target, IDoActorDamage damage, IApplySOT sots)
        {
            if (target == null) return;
            if (target.MyStatusEffects == null) return;
            if (damage.GetActorDamageData().DamageVar.DamageOptions.InflictSoT == false) return;
            if (damage.GetActorDamageData().DamageVar.CombatHandler.DetermineAttackable(target.MyHealth, owner.MyHealth, damage.GetActorDamageData().DamageVar.SoTOptions.FriendlyFIre) == false) return;//cant attack
            if (sots.GetSoTAppliedList().Contains(target)) return;//only allow one application per active swing

            bool foundonlist = false;
            for (int i = 0; i < sots.GetSOTS().Count; i++)
            {
                if (sots.GetSOTS()[i].Attackable == target.MyHealth)
                {
                    foundonlist = true;
                    //alraedy on the list, reapply?
                    bool dotsadded = damage.GetActorDamageData().DamageVar.CombatHandler.AddDOTS(sots.GetSOTS()[i].ActorHub, damage.GetActorDamageData().DamageVar.SoTOptions.AdditionalDOTs);
                    if (dotsadded)
                    {
                        sots.GetSOTEvents().SceneEvents.OnSoTApply.Invoke(sots.GetSOTS()[i].StatusChange);
                    }
                    break;
                }
            }

            if (foundonlist == false)
            {

                SOT newDot = new SOT(target);
                sots.GetSOTS().Add(newDot);
                if (damage.GetActorDamageData().DamageVar.SoTOptions.ApplyAtEnter)
                {
                    int eleDmg = damage.GetActorDamageData().DamageVar.CombatHandler.DoElementalDamageOverTime(target.MyHealth, damage.GetActorDamageData().DamageVar.SoTOverTimeMultipliers.ElementalMultipliers);

                    //int eleDmg = CombatResolution.DoElementalDamageOverTime(attacked, statusOverTimeMultiplers.ElementalMultipliers);
                    damage.GetActorDamageEvents().SceneEvents.OnElementalDamageOther.Invoke(eleDmg, target.MyHealth);
                }
                if (damage.GetActorDamageData().DamageVar.SoTOptions.ApplyDotAtEnter)
                {
                    bool dotsadded = damage.GetActorDamageData().DamageVar.CombatHandler.AddDOTS(target, damage.GetActorDamageData().DamageVar.SoTOptions.AdditionalDOTs);
                    if (dotsadded)
                    {
                        sots.GetSOTEvents().SceneEvents.OnSoTApply.Invoke(target.MyStatusEffects);
                    }
                }

            }
            sots.GetSoTAppliedList().Add(target);

        }

        public override int GetPhysicalAttackValue(IActorHub self)
        {
            if (self.PlayerControlled != null)
            {
                return PlayerCombat.GetAttackValue(self);
            }
            else
            {
                return EnemyCombat.GetAttackValue(self);
            }
        }


        /// <summary>
        /// performs melee damage (defined by the melee damage box)
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="damager"></param>
        /// <param name="attacked"></param>
        /// <param name="actorDmg"></param>
        /// <param name="meleeOptions"></param>
        public override AttackValues GetMeleeActorDamageLogic(AttackValues results, IActorHub owner, IDoDamage damager, IActorHub attacked, IDoActorDamage actorDmg, IMeleeWeapon meleeOptions)
        {
           
            if (actorDmg.GetActorDamageData().DamageVar.DamageOptions.InflictPhysicalDmg)
            {

                int physdmg = actorDmg.GetActorDamageData().DamageVar.DamageMultipliers.PhysMultipler.GetPhysicalDamageAmount(owner);
                results.PhysicalAttack.Add(new PhysicalAttackResults(physdmg, false, "Melee"));
                actorDmg.GetActorDamageEvents().SceneEvents.OnPhysicalDamagedOther.Invoke(physdmg, attacked.MyHealth);

            }

            if (actorDmg.GetActorDamageData().DamageVar.DamageOptions.InfictElementalDmg)
            {
                for (int i = 0; i < actorDmg.GetActorDamageData().DamageVar.DamageMultipliers.ElementMultiplers.Length; i++)
                {
                    results.ElementAttacks.Add(new ElementAttackResults(actorDmg.GetActorDamageData().DamageVar.DamageMultipliers.ElementMultiplers[i].DamageType, 
                        actorDmg.GetActorDamageData().DamageVar.DamageMultipliers.ElementMultiplers[i].GetElementDamageAmount(owner), 
                        "Melee"));
                    actorDmg.GetActorDamageEvents().SceneEvents.OnElementalDamageOther.Invoke(actorDmg.GetActorDamageData().DamageVar.DamageMultipliers.ElementMultiplers[i].GetElementDamageAmount(owner), attacked.MyHealth);
                }
              

            }


            int modlength = damager.GetWeaponMods().Length;
            for (int i = 0; i < modlength; i++)//try moving this to the damage logic. hmmm
            {
                damager.GetWeaponMods()[i].DoModification(results);
            }

            damager.GetDamagedList().Add(attacked);
            return results;
            actorDmg.GetActorDamageEvents().SceneEvents.OnDamagedOther.Invoke();
            attacked.MyHealth.SetCharacterThatHitMe(owner);
           // Debug.Log("Ran attack for " + owner.MyTransform.gameObject.name);
        }
        /// <summary>
        /// performs melee damage (defined by the melee damage box)
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="damageTarget"></param>
        /// <returns></returns>
        public override int DoMeleePhysicalDamage(IActorHub attacker, IActorHub damageTarget)
        {
            if (attacker == null || attacker.MyTransform == null) return 0;
            int phys = GetTotalActorDamage(attacker.MyTransform.gameObject);
            return DefaultPhysicalDamageBehavior(attacker, damageTarget, phys);
        }
        /// <summary>
        /// performs melee damage (defined by the melee damage box)
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="damageTarget"></param>
        /// <param name="mod"></param>
        /// <returns></returns>
        public override int DoMeleePhysicalDamage(IActorHub attacker, IActorHub damageTarget, PhysicalDamageMultiplier mod)
        {
            if (attacker == null || attacker.MyTransform== null) return 0;
            return DefaultPhysicalDamageBehavior(attacker, damageTarget, mod);
        }
        /// <summary>
        /// performs melee element damage (defined by the melee damage box)
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="damageTarget"></param>
        /// <param name="mods"></param>
        /// <returns></returns>
        public override int DoMeleeElementalDamage(IActorHub attacker, IActorHub damageTarget, ElementDamageMultiplierActor[] mods)
        {
            if (attacker == null || damageTarget == null) return 0;
            return DefaultElementalDamageBehavior(attacker, damageTarget.MyHealth, mods);
        }
        /// <summary>
        /// performs projectile non elemental damage (projectile damage box)
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="damageTarget"></param>
        /// <param name="mod"></param>
        /// <returns></returns>
        public override int DoProjectilePhysicalDamage(IActorHub attacker, IActorHub damageTarget, PhysicalDamageMultiplier mod)
        {
            if (attacker == null || attacker.MyTransform == null) return 0;
            return DefaultPhysicalDamageBehavior(attacker, damageTarget, mod);
        }
        /// <summary>
        ///  performs projectile elemental damage (projectile damage box)
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="damageTarget"></param>
        /// <param name="mods"></param>
        /// <returns></returns>
        public override int DoProjectileElementalDamage(IActorHub attacker, IActorHub damageTarget, ElementDamageMultiplierActor[] mods)
        {
            return DefaultElementalDamageBehavior(attacker, damageTarget.MyHealth, mods);
        }
        /// <summary>
        /// performs melee damage (defined by the melee damage box)
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="damageTarget"></param>
        /// <param name="mod"></param>
        /// <returns></returns>
        public override int DoPhysicalDamage(IActorHub attacker, IActorHub damageTarget, PhysicalDamageMultiplier mod)
        {
            if (attacker == null || attacker.MyTransform == null) return 0;
            return DefaultPhysicalDamageBehavior(attacker, damageTarget, mod);
        }
        /// <summary>
        /// performs melee element damage (defined by the melee damage box)
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="damageTarget"></param>
        /// <returns></returns>
        public override int DoElementalDamage(IAttributeUser attacker, IReceiveDamage damageTarget)
        {
            List<ElementAttackValues> values = GetActorElementAttackValues(attacker);
            return DefaultElementalDamageBehavior(damageTarget, values.ToArray());
        }
        /// <summary>
        /// performs melee element damage (defined by the melee damage box)
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="damageTarget"></param>
        /// <param name="mods"></param>
        /// <returns></returns>
        public override int DoElementalDamage(IActorHub attacker, IReceiveDamage damageTarget, ElementDamageMultiplierActor[] mods)
        {
            return DefaultElementalDamageBehavior(attacker, damageTarget, mods);
        }
        /// <summary>
        /// performs melee element damage over time (defined by the melee damage box)
        /// </summary>
        /// <param name="damageTarget"></param>
        /// <param name="damageArray"></param>
        /// <returns></returns>
        public override int DoElementalDamageOverTime(IReceiveDamage damageTarget, ElementDamageMultiplierNoActor[] damageArray)
        {
            return DefaultElementalDamageBehavior(damageTarget, damageArray);
        }
        /// <summary>
        /// applies DoTs to target
        /// </summary>
        /// <param name="target"></param>
        /// <param name="damageOverTimeOptions"></param>
        /// <returns></returns>
        public override bool AddDOTS(IActorHub target, ModifyResourceVars[] damageOverTimeOptions)
        {
            return DefaultApplyDots(target, damageOverTimeOptions);

        }
        /// <summary>
        /// determines if can attack
        /// </summary>
        /// <param name="target"></param>
        /// <param name="attacker"></param>
        /// <param name="friendlyFire"></param>
        /// <returns></returns>
        public override bool DetermineAttackable(IReceiveDamage target, IReceiveDamage attacker, bool friendlyFire)
        {
            return DefaultDetermineLogic(target, attacker, friendlyFire);

        }
        /// <summary>
        /// determines if can attack
        /// </summary>
        /// <param name="attackerGroups"></param>
        /// <param name="targetGroupTypes"></param>
        /// <returns></returns>
        public override bool DetermineAttackable(CombatGroupType[] attackerGroups, CombatGroupType[] targetGroupTypes)
        {
            return CustomGroupCheck(attackerGroups, targetGroupTypes);
        }


        #endregion

        #region protected
        protected virtual bool CustomGroupCheck(CombatGroupType[] attackerGroups, CombatGroupType[] targetGroupTypes)
        {

            if (attackerGroups.Intersect(targetGroupTypes).Any()) return false;

            return true;
        }

        protected virtual bool DefaultDetermineLogic(IReceiveDamage target, IReceiveDamage attacker, bool friendlyFire)
        {
            if (attacker == null) return false;
            if (target == null) return false;

            if (friendlyFire == true) return true;
            CombatGroupType[] attackedgroup = target.GetMyCombatGroup();
            CombatGroupType[] attackergroup = attacker.GetMyCombatGroup();
            if (attackergroup.Intersect(attackedgroup).Any()) return false;

            return true;
        }
        protected virtual bool DefaultApplyDots(IActorHub target, ModifyResourceVars[] damageOverTimeOptions)
        {
            if (target == null) return false;

            for (int i = 0; i < damageOverTimeOptions.Length; i++)
            {
                SoTHelper.AddDoT(target, damageOverTimeOptions[i]);
            }
            return true;
        }
        protected virtual List<ElementAttackValues> GetActorElementAttackValues(IAttributeUser forUser)
        {
            List<ElementAttackValues> temp = new List<ElementAttackValues>();
            if (forUser == null) return temp;
            foreach (ElementType pieceType in System.Enum.GetValues(typeof(ElementType)))
            {
                if (pieceType == ElementType.None) continue;
                int attack = GetActorElementDamage(forUser, pieceType);
                temp.Add(new ElementAttackValues(pieceType, attack));
            }
            return temp;

        }

        protected virtual int GetActorElementDamage(IAttributeUser forUser, ElementType damageType)
        {
            return forUser.GetRuntimeAttributes().GetElementAttack(damageType);
        }

        [System.Obsolete]
        protected virtual int DefaultPhysicalDamageBehavior(IActorHub attacker, IActorHub damageTarget, int dmg)
        {
           // damageTarget.MyHealth.TakeDamage(dmg, attacker);
            return dmg;
        }

        protected virtual int DefaultPhysicalDamageBehavior(IActorHub attacker, IActorHub damageTarget, PhysicalDamageMultiplier mod)
        {
            int phys = GetTotalActorDamage(attacker.MyTransform.gameObject);//get total
            phys += mod.GetPhysicalDamageAmount(attacker);//apply physical mod
            for (int i = 0; i < CritHelper.Crits.Count; i++)
            {
                if (attacker.MyStats == CritHelper.Crits[i].Attacker)
                {
                    CritHelper.Crits[i].Amount = phys;
                }
            }
            return DefaultPhysicalDamageBehavior(attacker, damageTarget, phys);
        }

        protected virtual int DefaultElementalDamageBehavior(IActorHub caster, IReceiveDamage damageTarget, ElementDamageMultiplierActor[] mods)
        {
            ElementAttackValues[] damageArray = ApplyMods(caster, mods);
            return DefaultElementalDamageBehavior(damageTarget, damageArray);
        }

        protected virtual ElementAttackValues[] ApplyMods(IActorHub caster, ElementDamageMultiplierActor[] mods)
        {
            List<ElementAttackValues> actorele = GetActorElementAttackValues(caster.MyStats);
            //apply the mods
            for (int i = 0; i < actorele.Count; i++)
            {
                ElementType type = actorele[i].Type;
                for (int j = 0; j < mods.Length; j++)
                {
                    if (type == mods[j].DamageType)
                    {
                        actorele[i].AttackDamage += mods[j].GetElementDamageAmount(caster);
                    }
                   
                }
            }
            ElementAttackValues[] damageArray = actorele.ToArray();
            return damageArray;
        }
       
        [System.Obsolete]
        protected virtual int DefaultElementalDamageBehavior(IReceiveDamage damageTarget, ElementAttackValues[] damageArray)
        {
            
            int total = 0;
            for (int i = 0; i < damageArray.Length; i++)
            {
                int baseElement = damageArray[i].AttackDamage;
                ElementType type = damageArray[i].Type;
                if (baseElement > 0)
                {
                    ARPGDebugger.CombatDebugMessage(ColorForDamage("Damage Sent to ") + damageTarget + " for " + baseElement.ToString() + " " + type.ToString(), null);
                  //  damageTarget.TakeDamage(baseElement, type);
                    total += baseElement;
                }
            }

            return total;
        }

        protected string ColorForDamage(string toColor)
        {
            return "<color=red>" + toColor + "</color>";
        }
        protected string ColorForResist(string toColor)
        {
            return "<color=red>" + toColor + "</color>";
        }

        [System.Obsolete]
        protected virtual int DefaultElementalDamageBehavior(IReceiveDamage damageTarget, ElementDamageMultiplierNoActor[] damageArray)
        {

            int total = 0;
            for (int i = 0; i < damageArray.Length; i++)
            {
                int baseprojectile = damageArray[i].BaseElementDamage;
                ElementType type = damageArray[i].DamageType;


                if (baseprojectile > 0)
                {
                   // damageTarget.TakeDamage(baseprojectile, type);
                    total += baseprojectile;
                }
            }

            return total;
        }

        public override List<ElementAttackResults> GetReducedResultsElemental(List<ElementAttackResults> attackvalues, IActorHub self)
        {
            if (self.PlayerControlled != null)
            {
                //player one
                return PlayerCombat.GetReducedResults(attackvalues, self);
            }
            else
            {
                return EnemyCombat.GetReducedResults(attackvalues, self);
            }
     
        }

        public override List<PhysicalAttackResults> GetReducedResultsPhysical(List<PhysicalAttackResults> results, IActorHub self)
        {
            if (self.PlayerControlled != null)
            {
                return PlayerCombat.GetReducedPhysical(results, self);
            }
            else
            {
                return EnemyCombat.GetReducedPhysical(results, self);
            }
        }

        public override bool CanMeleeAttack(IActorHub owner, IDoDamage damager, IDoActorDamage actorDmg, IActorHub attacked, IMeleeWeapon meleeOptions)
        {
            if (owner == null) return false;
            if (attacked == null) return false;
            if (attacked == owner) return false;
            if (actorDmg.GetActorDamageData().DamageVar.DamageOptions.InfictElementalDmg == false && actorDmg.GetActorDamageData().DamageVar.DamageOptions.InflictPhysicalDmg == false) return false;
            if (actorDmg.GetActorDamageData().DamageVar.CombatHandler.DetermineAttackable(attacked.MyHealth, owner.MyHealth, meleeOptions.GetMeleeOptions().MeleeVars.FriendlyFire) == false) return false;//cant attack
            if (damager.GetDamagedList().Contains(attacked) == true) return false;
            return true;
        }

        public override bool CanProjectileAttack(IActorHub owner, IDoDamage damageDealer, IActorHub damageTarget, IDoActorDamage damage, IProjectile projectileOptions)
        {
            if (damage.GetActorDamageData().DamageVar.DamageOptions.InflictPhysicalDmg == false && damage.GetActorDamageData().DamageVar.DamageOptions.InfictElementalDmg == false) return false;//no dmg to do
            if (damageTarget == null) return false;
            if (damageTarget == owner) return false;

            if (damage.GetActorDamageData().DamageVar.CombatHandler.DetermineAttackable(damageTarget.MyHealth, owner.MyHealth, projectileOptions.GetProjectileData().ProjectileVars.FriendlyFire) == false) return false; //can't attack
            if (damageDealer.GetDamagedList().Contains(damageTarget) == true) return false;
            return true;
        }

       

        #endregion
    }



    public abstract class PlayerCombatFormulas : ScriptableObject
    {
        public abstract int GetArmorValue(IActorHub user);
        public abstract int GetAttackValue(IActorHub user);
        public abstract int GetTotalAttackDamage(IAttributeUser playerStats, IInventoryUser playerInv, IAbilityUser playerAbilities);
        [System.Obsolete]
        public abstract int GetReducedPhysical(IAttributeUser playerStats, IInventoryUser playerInv, int enemyLevel, float fullDamageAmount);
        public abstract int GetArmor(IAttributeUser player, IInventoryUser playerInv);
        public abstract Dictionary<ElementType, ElementAttackResults> GetElementalDamageResistChecks(IAttributeUser defender, IAttributeUser attacker);
        public abstract int GetElementalDamageResistChecks(IAttributeUser enemy, ElementType _type, int elementDamage);

        public abstract List<ElementAttackResults> GetReducedResults(List<ElementAttackResults> attackvalues, IActorHub self);

        public abstract List<PhysicalAttackResults> GetReducedPhysical(List<PhysicalAttackResults> results, IActorHub self);
    }


    public abstract class CommonCombatFormulas :ScriptableObject
    {
        public abstract int GetPhysicalAttackValue(IActorHub self);
        public abstract bool CanMeleeAttack(IActorHub owner, IDoDamage damager, IDoActorDamage actorDmg, IActorHub attacked, IMeleeWeapon meleeOptions);

        public abstract bool CanProjectileAttack(IActorHub owner, IDoDamage damageDealer, IActorHub damageTarget, IDoActorDamage damage, IProjectile projectileOptions);

        public abstract AttackValues GetMeleeActorDamageLogic(AttackValues results, IActorHub owner, IDoDamage damager, IActorHub attacked, IDoActorDamage actorDmg, IMeleeWeapon meleeOptions);

        public abstract AttackValues GetProjectileDamage(AttackValues results, IActorHub owner, IDoDamage damageDealer, IActorHub damageTarget, IDoActorDamage damage, IProjectile projectileOptions);

        public abstract List<ElementAttackResults> GetReducedResultsElemental(List<ElementAttackResults> attackvalues, IActorHub self);

        public abstract List<PhysicalAttackResults> GetReducedResultsPhysical(List<PhysicalAttackResults> results, IActorHub self);



        public abstract bool AddDOTS(IActorHub target, ModifyResourceVars[] damageOverTimeOptions);
        public abstract void SoTsDamageLogic(IActorHub target, IDoActorDamage damage);
        public abstract void SoTsApplyLogic(IActorHub target, IDoActorDamage damage, IApplySOT sots);
        public abstract void OnEnterSotLogic(IActorHub owner, IActorHub target, IDoActorDamage damage, IApplySOT sots);
        public abstract void OnExitSoTLogic(IActorHub target, IDoActorDamage damage, IApplySOT sots);


        public abstract int DoMeleePhysicalDamage(IActorHub attacker, IActorHub damageTarget, PhysicalDamageMultiplier mod);
        public abstract int DoMeleeElementalDamage(IActorHub attacker, IActorHub damageTarget, ElementDamageMultiplierActor[] mods);
        public abstract int DoProjectilePhysicalDamage(IActorHub attacker, IActorHub damageTarget, PhysicalDamageMultiplier mod);
        public abstract int DoProjectileElementalDamage(IActorHub attacker, IActorHub damageTarget, ElementDamageMultiplierActor[] mods);
        public abstract int DoPhysicalDamage(IActorHub attacker, IActorHub damageTarget, PhysicalDamageMultiplier mod);
        public abstract int DoElementalDamage(IAttributeUser attacker, IReceiveDamage damageTarget);
        public abstract int DoElementalDamage(IActorHub attacker, IReceiveDamage damageTarget, ElementDamageMultiplierActor[] mods);
        public abstract int DoElementalDamageOverTime(IReceiveDamage damageTarget, ElementDamageMultiplierNoActor[] damageArray);
        public abstract int DoMeleePhysicalDamage(IActorHub attacker, IActorHub damageTarget);


        public abstract int GetTotalActorDamage(GameObject toGet);
        public abstract bool DetermineAttackable(IReceiveDamage target, IReceiveDamage attacker, bool friendlyFire);
        public abstract bool DetermineAttackable(CombatGroupType[] attackerGroups, CombatGroupType[] targetGroupTypes);



    }

    public abstract class EnemyCombatFormulas : ScriptableObject
    {
        public abstract int GetAttackValue(IActorHub user);
      
        public abstract int GetTotalAttackDamage(IAttributeUser enemy);
        [System.Obsolete]
        public abstract Dictionary<ElementType, ElementAttackResults> GetElementalDamageResistChecks(IAttributeUser enemy, IAttributeUser player);

        public abstract List<ElementAttackResults> GetReducedResults(List<ElementAttackResults> attackvalues, IActorHub self);

        public abstract int GetElementalDamageResistChecks(IAttributeUser enemy, ElementType _type, int elementDamage);

        [System.Obsolete]
        public abstract int GetReducedPhysical(IAttributeUser enemyStats, int damageAmount);

        public abstract int GetArmor(IAttributeUser enemy);


        public abstract List<PhysicalAttackResults> GetReducedPhysical(List<PhysicalAttackResults> results, IActorHub self);
    }

    /// <summary>
    /// used to track crits, the attacker and amount is the key
    /// </summary>
    public class CritLog
    {
        public IAttributeUser Attacker;
        public int Amount;
        public CritLog(IAttributeUser attacker, int amount)
        {
            Amount = amount;
            Attacker = attacker;
        }
    }
    /// <summary>
    /// used to record critical hits to pass that info on to whoever needs it, such as the UI
    /// </summary>
    public static class CritHelper
    {
        public static List<CritLog> Crits = new List<CritLog>();

       
        /// <summary>
        /// Removes crit from log if returned true.
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static bool WasCrit(IAttributeUser attacker, int amount, bool removeOnTrue = true)
        {
            for (int i = 0; i < Crits.Count; i++)
            {
                CritLog crit = Crits[i];
                if (crit.Amount == amount && crit.Attacker == attacker)
                {
                    if (removeOnTrue)
                    {
                        Crits.RemoveAt(i);
                    }

                    return true;
                }
            }
            return false;
        }
    }
}