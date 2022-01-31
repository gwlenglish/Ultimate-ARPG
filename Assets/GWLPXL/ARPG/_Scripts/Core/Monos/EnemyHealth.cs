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
        [Tooltip("Damage sent")]
        public int PhysicalDamage;
        [Tooltip("was a crit?")]
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
    public class PhysicalDamageReport
    {
        public int TotalDamage;
        public int Resisted;
        public int ReducedDamage;
        public List<string> Sources = new List<string>();
        public bool WasCrit = false;
        public PhysicalDamageReport(int total, int resisted, int reduced, List<string> sources, bool crit = false)
        {
            WasCrit = crit;
            Sources = sources;
            TotalDamage = total;
            Resisted = resisted;
            ReducedDamage = reduced;
        }
    }
    [System.Serializable]
    public class ElementalDamageReport
    {
        public ElementType Type;
        public int TotalDamage;
        public int Resisted;
        public int ReducedDamage;
        public List<string> Sources;
        public bool WasCrit = false;
        public ElementalDamageReport(ElementType type, int total, List<string> sources, bool crit = false)
        {
            Sources = sources;
            TotalDamage = total;
            Type = type;
            WasCrit = crit;
        }
    }

    [System.Serializable]
    public class DamageValues
    {
        public List<ElementalDamageReport> ReportElementalDmg;
        public PhysicalDamageReport ReportPhysDmg;
        public IReceiveDamage Target;
        public Transform TargetOwner;

        public DamageValues(PhysicalDamageReport phys, List<ElementalDamageReport> eles, IReceiveDamage target)
        {
            ReportPhysDmg = phys;
            ReportElementalDmg = eles;
            Target = target;
            TargetOwner = target.GetInstance();
            if (Target.GetUser() != null)
            {
                TargetOwner = target.GetUser().MyTransform;
            }
        }
    }
    
    [System.Serializable]
    public class CombatResults : EventArgs
    {
        [Header("Totals")]
        [Tooltip("All damage, regardless of type")]
        public int TotalCombinedDamage;
        [Tooltip("All resist, regardless of type")]
        public int TotalCombinedResist;
        [Tooltip("All damage - all resist, regardless of type")]
        public int TotalReducedDamage;
        [Header("Attack Results")]
        public AttackValues AttackValues;
        [Header("Damage Results")]
        public DamageValues DamageValues;

        public CombatResults(AttackValues attackvalues, DamageValues damagevalues)
        {
            AttackValues = attackvalues;
            DamageValues = damagevalues;

            for (int i = 0; i < attackvalues.PhysicalAttack.Count; i++)
            {
                TotalCombinedDamage += attackvalues.PhysicalAttack[i].PhysicalDamage;
            }
            for (int i = 0; i < attackvalues.ElementAttacks.Count; i++)
            {
                TotalCombinedDamage += attackvalues.ElementAttacks[i].Damage;
            }

            for (int i = 0; i < damagevalues.ReportElementalDmg.Count; i++)
            {
                TotalCombinedResist += damagevalues.ReportElementalDmg[i].Resisted;
            }

            TotalCombinedResist += damagevalues.ReportPhysDmg.Resisted;

            TotalReducedDamage = TotalCombinedDamage - TotalCombinedResist;
        }
    }
    public class EnemyHealth : MonoBehaviour, IReceiveDamage
    {
        public Action<CombatResults> OnTakeDamage;
        public System.Action OnDeath;
        public System.Action<GameObject> OnDeathAttacker;
        public System.Action<IActorHub> OnDamagedMe;
        [SerializeField]
        [Tooltip("Null will default to the built in formulas.")]
        protected EnemyDefault combatHandler = null;
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
        protected Transform lastCharacterThatHitMeT = null;
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
            if (user != null)
            {
                lastCharacterThatHitMeT = user.MyTransform;
            }
     

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
          //  DefaultTakeDamage(damageAmount, type);
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
           // DefaultTakeActorDamage(damageAmount, damageDealer);

        }
        public void TakeDamage(AttackValues values)
        {
            if (canBeAttacked == false)
            {
                return;
            }
            CombatResults results = combatHandler.TakeDamageFormula(values, owner);
            if (immortal == false)
            {

                for (int i = 0; i < results.DamageValues.ReportElementalDmg.Count; i++)
                {
                    if (results.DamageValues.ReportElementalDmg[i].ReducedDamage > 0)//prevent dmg if immortal, but show everything else
                    {
                        owner.MyStats.GetRuntimeAttributes().ModifyNowResource(healthResource, -results.DamageValues.ReportElementalDmg[i].ReducedDamage);
                    }

                }

                if (results.DamageValues.ReportPhysDmg.ReducedDamage > 0)
                {
                    owner.MyStats.GetRuntimeAttributes().ModifyNowResource(healthResource, -results.DamageValues.ReportPhysDmg.ReducedDamage);
                }
               
            }


            CombatLogger.AddResult(results);
            OnTakeDamage?.Invoke(results);
            NotifyUI(results);
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

        protected virtual void NotifyUI(CombatResults results)
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
