using GWLPXL.ARPGCore.Abilities.com;
using GWLPXL.ARPGCore.AI.com;
using GWLPXL.ARPGCore.Attributes.com;
using GWLPXL.ARPGCore.CanvasUI.com;
using GWLPXL.ARPGCore.Combat.com;
using GWLPXL.ARPGCore.Items.com;
using GWLPXL.ARPGCore.Leveling.com;
using GWLPXL.ARPGCore.Looting.com;
using GWLPXL.ARPGCore.Movement.com;
using GWLPXL.ARPGCore.Quests.com;
using GWLPXL.ARPGCore.Statics.com;
using GWLPXL.ARPGCore.Types.com;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
namespace GWLPXL.ARPGCore.com
{
    [System.Serializable]
    public class PhysicalAttackResults
    {
        public string Source;
        public int PhysicalDamage;
        public int PhysicalResisted;
        public int PhysicalReduced;
        public bool PhysicalCrit;
        public PhysicalAttackResults(int damage, bool crit, string source)
        {
            Source = source;
            PhysicalDamage = damage;
            PhysicalCrit = crit;
        }

        public void AddDamage(int amount)
        {
            PhysicalDamage += amount;
        }
    }

    [System.Serializable]
    public class DamageResults : EventArgs
    {
        public List<ElementAttackResults> ElementResults;
        public List<PhysicalAttackResults> PhysicalResult;
        public IReceiveDamage Target;


        public DamageResults(List<ElementAttackResults> e, List<PhysicalAttackResults> phys, IReceiveDamage target)
        {
            Target = target;
            PhysicalResult = phys;
            ElementResults = e;
        }
    }
    public class EnemyHealth : MonoBehaviour, IReceiveDamage
    {
        public Action<DamageResults> OnTakeDamage;
        public System.Action OnDeath;
        public System.Action<GameObject> OnDeathAttacker;
        public System.Action<IActorHub> OnDamagedMe;
        [SerializeField]
        [Tooltip("Null will default to the built in formulas.")]
        protected EnemyCombatFormulas combatHandler = null;
        [SerializeField]
        [Tooltip("Scene specific events")]
        protected UnityHealthEvents healthEvents = new UnityHealthEvents();
        [SerializeField]
        protected ResourceType healthResource = ResourceType.Health;
        [SerializeField]
        protected float iFrameTime = .25f;
        [SerializeField]
        protected bool immortal = false;

        protected CombatGroupType[] combatGroups = new CombatGroupType[1] { CombatGroupType.Enemy };
        protected bool isDead = false;
        protected  bool canBeAttacked = true;
        protected  IGiveXP giveXp = null;
        protected IKillTracked[] killedTracked = new IKillTracked[0];
        protected IUseFloatingText dungeoncanvas = null;
        protected IActorHub lastcharacterHitMe = null;
        protected int lastNonMitagatedHitAmount = 0;
        protected IActorHub owner = null;

        #region unity calls
        protected virtual void Awake()
        {
            Setup();

        }
        #endregion

        #region public interfaces
        public virtual void Die()
        {
            DefaultDie();

        }
        public virtual CombatGroupType[] GetMyCombatGroup()
        {
            return combatGroups;
        }

        public virtual void SetCharacterThatHitMe(IActorHub user)
        {
            lastcharacterHitMe = user;

        }

        public virtual void SetUser(IActorHub forUser)
        {
            owner = forUser;


        }
        public virtual Transform GetInstance()
        {
            return transform;
        }

        public virtual bool IsDead()
        {
            return isDead;
        }
        public virtual bool IsHurt()
        {
            return !canBeAttacked;
        }

        public virtual ResourceType GetHealthResource()
        {
            return healthResource;
        }
        public virtual void SetInvincible(bool isImmoratal) => immortal = isImmoratal;
        /// <summary>
        /// doesn't respect the iframe timer
        /// </summary>
        /// <param name="damageAmount"></param>
        /// <param name="type"></param>
        /// 
        [System.Obsolete]
        public virtual void TakeDamage(int damageAmount, ElementType type)
        {
            DefaultTakeDamage(damageAmount, type);
        }
        /// <summary>
        /// override to remember who hit last, respects the iframe timer
        /// </summary>
        /// <param name="damageAmount"></param>
        /// <param name="type"></param>
        /// <param name="owner"></param>
        //this one has an iframe timer. I wonder if we need that on enemy tho.
        [System.Obsolete]
        public virtual void TakeDamage(int damageAmount, IActorHub damageDealer)
        {
            DefaultTakeActorDamage(damageAmount, damageDealer);

        }
        List<DamageResults> results = new List<DamageResults>();
        public void TakeDamage(AttackValues values)
        {
            IActorHub attacker = values.Attacker;
            List<ElementAttackResults> elements = values.ElementAttacks;
            List<PhysicalAttackResults> phys = values.PhysicalAttack;

            elements = combatHandler.GetReducedResults(elements, owner);//calculates resist and new reduced values
            phys = combatHandler.GetReducedPhysical(phys, owner);

            if (immortal == false)
            {
                for (int i = 0; i < elements.Count; i++)
                {
                    if (elements[i].Reduced > 0)//prevent dmg if immortal, but show everything else
                    {
                        owner.MyStats.GetRuntimeAttributes().ModifyNowResource(healthResource, -elements[i].Reduced);
                    }

                }

                for (int i = 0; i < phys.Count; i++)
                {
                    if (phys[i].PhysicalReduced > 0)
                    {
                        owner.MyStats.GetRuntimeAttributes().ModifyNowResource(healthResource, -phys[i].PhysicalReduced);
                    }
                }
               
            }

            DamageResults d = new DamageResults(elements, phys, this);
            results.Add(d);
            OnTakeDamage?.Invoke(d);
            NotifyUI(d);
            CheckDeath();
            StartCoroutine(CanBeAttackedCooldown(iFrameTime));//we are invulnerable for a short time
            SetCharacterThatHitMe(values.Attacker);
        }


        public virtual void CheckDeath()
        {
            DefaultCheckDeath();

        }
        #endregion

        #region protected virtuals

        protected virtual void Setup()
        {
            giveXp = GetComponent<IGiveXP>();
            killedTracked = GetComponents<IKillTracked>();
            dungeoncanvas = GetComponent<IUseFloatingText>();
            if (combatHandler == null) combatHandler = ScriptableObject.CreateInstance<EnemyDefault>();
        }
        protected virtual void DefaultDie()
        {
            if (isDead) return;

            if (giveXp != null)
            {
                giveXp.GiveXP();
            }


            gameObject.layer = 0;
            canBeAttacked = false;
            isDead = true;

            IDropLoot[] loot = GetComponents<IDropLoot>();
            float dropDelay = 1f;
            if (loot != null && loot.Length > 0)
            {
                StartCoroutine(DropLootSequence(dropDelay * loot.Length, loot));
            }

            Destroy(owner.MyTransform.gameObject, dropDelay + 5f);


            owner.MyMover.DisableMovement(true);


            if (owner.MyMelee != null)//if we are combatant
            {
                if (owner.MyMelee.GetMeleeDamageBoxes() != null || owner.MyMelee.GetMeleeDamageBoxes().Length > 0)//if we have dmg boxes
                {
                    for (int i = 0; i < owner.MyMelee.GetMeleeDamageBoxes().Length; i++)//disable each active melee dmg box
                    {
                        if (owner.MyMelee.GetMeleeDamageBoxes()[i] == null) continue;
                        owner.MyMelee.GetMeleeDamageBoxes()[i].EnableDamageComponent(false, null);
                    }

                }
            }



            if (killedTracked.Length > 0 && lastcharacterHitMe != null)
            {
                for (int i = 0; i < killedTracked.Length; i++)
                {
                    killedTracked[i].UpdateQuest(lastcharacterHitMe.MyQuests);
                }
            }

            OnDeath?.Invoke();
            OnDeathAttacker?.Invoke(lastcharacterHitMe.MyTransform.gameObject);
            healthEvents.OnDie.Invoke();
        }

        protected virtual IEnumerator DropLootSequence(float delay, IDropLoot[] lootdropper)
        {
            for (int i = 0; i < lootdropper.Length; i++)
            {
                yield return new WaitForSeconds(delay/lootdropper.Length);
                lootdropper[i].DropLoot();
            }
          
        }

        protected virtual void DefaultTakeDamage(int damageAmount, ElementType type)
        {
            int damage = combatHandler.GetElementalDamageResistChecks(owner.MyStats, type, damageAmount);
            
            if (damage > 0 && immortal == false)//prevent dmg if immortal, but show everything else
            {
                owner.MyStats.GetRuntimeAttributes().ModifyNowResource(healthResource, -damage);
            }

           // NotifyUI(type, damage);
            RaiseUnityDamageEvent(damage);
            if (isDead) return;

            OnDamagedMe?.Invoke(lastcharacterHitMe);
            CheckDeath();
        }

        [System.Obsolete]
        protected virtual void DefaultTakeActorDamage(int damageAmount, IActorHub damageDealer)
        {
            if (isDead) return;
            if (canBeAttacked == false) return;
            lastNonMitagatedHitAmount = damageAmount;
            SetCharacterThatHitMe(damageDealer);

            int wpndmg = combatHandler.GetReducedPhysical(owner.MyStats, damageAmount);
            if (wpndmg > 0)
            {
                TakeDamage(wpndmg, ElementType.None);//since this is calling ui multiple
            }

            Dictionary<ElementType, ElementAttackResults> results = combatHandler.GetElementalDamageResistChecks(owner.MyStats, damageDealer.MyStats);
            foreach (var kvp in results)
            {
                int damage = combatHandler.GetElementalDamageResistChecks(owner.MyStats, kvp.Value.Type, damageAmount);

                if (damage > 0 && immortal == false)//prevent dmg if immortal, but show everything else
                {
                    owner.MyStats.GetRuntimeAttributes().ModifyNowResource(healthResource, -damage);
                }
                kvp.Value.Reduced = damage;
                //TakeDamage(kvp.Value.Damage, kvp.Key);//since this is calling ui multiplie
            }

  
            bool crit = false;
            if (lastcharacterHitMe != null)
            {
                crit = CritHelper.WasCrit(lastcharacterHitMe.MyStats, lastNonMitagatedHitAmount);
            }
            if (crit)
            {
                lastNonMitagatedHitAmount = 0;
            }
            OnDamagedMe?.Invoke(damageDealer);

            CheckDeath();
            StartCoroutine(CanBeAttackedCooldown(iFrameTime));//we are invulnerable for a short time
        }

        protected virtual IEnumerator CanBeAttackedCooldown(float duration)
        {
            canBeAttacked = false;
            yield return new WaitForSeconds(duration);
            canBeAttacked = true;
        }

        protected virtual void DefaultCheckDeath()
        {
            if (owner.MyStats.GetRuntimeAttributes().GetResourceNowValue(healthResource) <= 0)
            {
                Die();
            }
        }

        protected virtual void RaiseUnityDamageEvent(int dmg)
        {
            if (healthEvents.OnDamageTaken != null)
            {
                healthEvents.OnDamageTaken.Invoke(dmg);
            }
        }

        protected virtual void NotifyUI(DamageResults results)
        {

            if (dungeoncanvas == null) return;
            dungeoncanvas.DamageResults(results);
            
        }

        /// <summary>
        /// deprecate
        /// </summary>
        /// <param name="type"></param>
        /// <param name="damage"></param>
        protected virtual void NotifyUI(ElementType type, int damage)
        {
            if (dungeoncanvas == null) return;
            bool crit = false;
            if (lastcharacterHitMe != null)
            {
                crit = CritHelper.WasCrit(lastcharacterHitMe.MyStats, lastNonMitagatedHitAmount);
            }
            if (crit)
            {
                lastNonMitagatedHitAmount = 0;
            }
            Debug.Log("Crit " + crit);
            dungeoncanvas.CreateUIDamageText("-" + damage.ToString(), type, crit);

        }

        public IActorHub GetUser()
        {
            return owner;
        }



        #endregion

    }
}
