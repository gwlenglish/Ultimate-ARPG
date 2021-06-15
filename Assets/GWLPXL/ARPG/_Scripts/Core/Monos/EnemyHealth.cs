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


namespace GWLPXL.ARPGCore.com
{
    
    public class EnemyHealth : MonoBehaviour, IReceiveDamage
    {
        public System.Action OnDeath;
        public System.Action<GameObject> OnDeathAttacker;
        public System.Action<IActorHub> OnDamagedMe;
        [SerializeField]
        [Tooltip("Null will default to the built in formulas.")]
        EnemyCombatFormulas combatHandler = null;
        [SerializeField]
        [Tooltip("Scene specific events")]
        UnityHealthEvents healthEvents = new UnityHealthEvents();
        [SerializeField]
        ResourceType healthResource = ResourceType.Health;
        [SerializeField]
        float iFrameTime = .25f;

        CombatGroupType[] combatGroups = new CombatGroupType[1] { CombatGroupType.Enemy };
        bool isDead = false;
        bool canBeAttacked = true;
        IGiveXP giveXp = null;
        IKillTracked[] killedTracked = new IKillTracked[0];
        IUseFloatingText dungeoncanvas = null;
        IActorHub lastcharacterHitMe = null;
        IActorHub owner = null;
        [SerializeField]
        bool immortal = false;

        private void Awake()
        {
            giveXp = GetComponent<IGiveXP>();
            killedTracked = GetComponents<IKillTracked>();
            dungeoncanvas = GetComponent<IUseFloatingText>();
            if (combatHandler == null) combatHandler = ScriptableObject.CreateInstance<EnemyDefault>();

        }

        public void SetImmortal(bool isImmortal) => immortal = isImmortal;

        public void Die()
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

        IEnumerator DropLootSequence(float delay, IDropLoot[] lootdropper)
        {
            for (int i = 0; i < lootdropper.Length; i++)
            {
                yield return new WaitForSeconds(delay/lootdropper.Length);
                lootdropper[i].DropLoot();
            }
          
        }

        public Transform GetInstance()
        {
            return transform;
        }

        public bool IsDead()
        {
            return isDead;
        }

        /// <summary>
        /// doesn't respect the iframe timer
        /// </summary>
        /// <param name="damageAmount"></param>
        /// <param name="type"></param>
        public void TakeDamage(int damageAmount, ElementType type)
        {
            int damage = combatHandler.GetElementalDamageResistChecks(owner.MyStats, type, damageAmount);
            if (damage > 0 && immortal == false)//prevent dmg if immortal, but show everything else
            {
                owner.MyStats.GetRuntimeAttributes().ModifyNowResource(healthResource, -damage);
            }

            NotifyUI(type, damage);
            RaiseUnityDamageEvent(damage);
            if (isDead) return;

            OnDamagedMe?.Invoke(lastcharacterHitMe);
            CheckDeath();
        }
        /// <summary>
        /// override to remember who hit last, respects the iframe timer
        /// </summary>
        /// <param name="damageAmount"></param>
        /// <param name="type"></param>
        /// <param name="owner"></param>
        //this one has an iframe timer. I wonder if we need that on enemy tho.
        public void TakeDamage(int damageAmount, IActorHub damageDealer)
        {
            if (isDead) return;
            if (canBeAttacked == false) return;


            int wpndmg = combatHandler.GetReducedPhysical(owner.MyStats, damageAmount);
            if (wpndmg > 0)
            {
                TakeDamage(wpndmg, ElementType.None);
            }

            Dictionary<ElementType, ElementAttackResults> results = combatHandler.GetElementalDamageResistChecks(owner.MyStats, damageDealer.MyStats);
            foreach (var kvp in results)
            {
                TakeDamage(kvp.Value.Damage, kvp.Key);
            }

            SetCharacterThatHitMe(damageDealer);


            OnDamagedMe?.Invoke(damageDealer);
            CheckDeath();
            StartCoroutine(CanBeAttackedCooldown(iFrameTime));//we are invulnerable for a short time



        }

        IEnumerator CanBeAttackedCooldown(float duration)
        {
            canBeAttacked = false;
            yield return new WaitForSeconds(duration);
            canBeAttacked = true;
        }

        public void CheckDeath()
        {
            if (owner.MyStats.GetRuntimeAttributes().GetResourceNowValue(healthResource) <= 0)
            {
                Die();
            }
           
        }

      
        public bool IsHurt()
        {
            return !canBeAttacked;
        }

        public ResourceType GetHealthResource()
        {
            return healthResource;
        }

      

        private void RaiseUnityDamageEvent(int dmg)
        {
            if (healthEvents.OnDamageTaken != null)
            {
                healthEvents.OnDamageTaken.Invoke(dmg);
            }
        }

        private void NotifyUI(ElementType type, int damage)
        {
            if (dungeoncanvas == null) return;
            dungeoncanvas.CreateUIDamageText("-" + damage.ToString(), type);

        }

        public CombatGroupType[] GetMyCombatGroup()
        {
            return combatGroups;
        }

        public void SetCharacterThatHitMe(IActorHub user)
        {
            lastcharacterHitMe = user;

        }

        public void SetUser(IActorHub forUser)
        {
            owner = forUser;
           
          
        }

        public void SetInvincible(bool isImmoratal) => immortal = isImmoratal;
       
    }
}
