using GWLPXL.ARPGCore.Attributes.com;
using GWLPXL.ARPGCore.Combat.com;
using GWLPXL.ARPGCore.GameEvents.com;
using GWLPXL.ARPGCore.Items.com;
using GWLPXL.ARPGCore.Leveling.com;
using GWLPXL.ARPGCore.Statics.com;
using GWLPXL.ARPGCore.Types.com;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GWLPXL.ARPGCore.com
{
 

    public class PlayerHealth : MonoBehaviour, IReceiveDamage
    {
        [SerializeField]
        PlayerCombatFormulas combatHandler = null;
        [SerializeField]
        PlayerHealthEvents healthEvents = new PlayerHealthEvents();
        [SerializeField]
        ResourceType healthResource = ResourceType.Health;
        [SerializeField]
        float iFrameTime = .25f;
        bool isDead = false;
        bool canBeAttacked = false;
        CombatGroupType[] combatGroups = new CombatGroupType[1] { CombatGroupType.Friendly };
        IActorHub owner = null;
        IActorHub lastCharacterToHitMe;
        bool immortal = false;
        private void Awake()
        {

            canBeAttacked = true;
            if (combatHandler == null) combatHandler = ScriptableObject.CreateInstance<PlayerDefault>();
        }



        public void Die()
        {
            if (isDead) return;

            isDead = true;
            NotifyCustomDeathEvent();

        }

        private void NotifyCustomDeathEvent()
        {
            healthEvents.SceneEvents.OnDie.Invoke();
            if (healthEvents.GameEvents.DeathEvent == null) return;

            healthEvents.GameEvents.DeathEvent.DiedObj = this.gameObject;
            GameEventHandler.RaiseDeathEvent(healthEvents.GameEvents.DeathEvent);

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
        /// not effected by iframes, only takes into account ele type damage and resist
        /// </summary>
        /// <param name="damageAmount"></param>
        /// <param name="type"></param>
        public void TakeDamage(int damageAmount, ElementType type)
        {
            int damage = combatHandler.GetElementalDamageResistChecks(owner.MyStats, type, damageAmount);
            if (damage > 0 && immortal == false)
            {
                owner.MyStats.GetRuntimeAttributes().ModifyNowResource(healthResource, -damage);
            }

            //int eleDmgTaken = CombatResolution.DoPlayerElementDamageWithResistChecks(stats, type, damageAmount, healthResource);

            NotifyCustomDamageEvent(type, damage);
            CheckDeath();
            //raise event, dmgtaken and type


        }

        private void NotifyCustomDamageEvent(ElementType type, int eleDmgTaken)
        {
            healthEvents.SceneEvents.OnDamageTaken.Invoke(eleDmgTaken);
            if (healthEvents.GameEvents.TookDamageEvent == null) return;

            healthEvents.GameEvents.TookDamageEvent.EventVars = new DamageEvent(this, eleDmgTaken, type, eleDmgTaken.ToString(), transform.position + Vector3.up * 2f);
            GameEventHandler.RaisePlayerDamageEvent(healthEvents.GameEvents.TookDamageEvent);

        }

        /// <summary>
        /// effected by iframes, checks physical and ele damage
        /// </summary>
        /// <param name="damageAmount"></param>
        /// <param name="damageDealer"></param>
        public void TakeDamage(int damageAmount, IActorHub damageDealer)
        {
            if (canBeAttacked == false) return;

            int scaledLevel = 1;
            if (damageDealer != null)
            {
                //IScale scaledD = damageDealer.GetInstance().GetComponent<IScale>();
                //if (scaledD != null)
                //{
                //    scaledLevel = scaledD.GetScaledLevel();
                //}
            }

            //check physical
            int physicalDmgTaken = combatHandler.GetReducedPhysical(owner.MyStats, owner.MyInventory, scaledLevel, damageAmount);
            if (physicalDmgTaken > 0)
            {
                TakeDamage(physicalDmgTaken, ElementType.None);
            }

            //int physicalDmgTaken = CombatResolution.DoPlayerReducedPhysical(stats, inv, scaledLevel, damageAmount);
            //do event with physical dmg
           // NotifyCustomDamageEvent(ElementType.None, physicalDmgTaken);
            //do elemental
           // int eleDmgTake = CombatResolution.DoPlayerReducedElemental(this, damageDealer, healthResource, healthEvents.GameEvents.TookDamageEvent);//event can be null

            Dictionary<ElementType, ElementAttackResults> results = combatHandler.GetElementalDamageResistChecks(owner.MyStats, damageDealer.MyStats);
            foreach (var kvp in results)
            {
                TakeDamage(kvp.Value.Damage, kvp.Key);
            }


            CheckDeath();
            StartCoroutine(CanBeAttackedCooldown(iFrameTime));//we are invulnerable for a short time



        }

       

        public void CheckDeath()
        {
            if (owner.MyStats.GetRuntimeAttributes().GetResourceNowValue(ResourceType.Health) <= 0)
            {
                Die();
            }
        }
        IEnumerator CanBeAttackedCooldown(float duration)
        {
            canBeAttacked = false;
            yield return new WaitForSeconds(duration);
            canBeAttacked = true;
        }

        public bool IsHurt()
        {
            return !canBeAttacked;
        }

        public ResourceType GetHealthResource()
        {
            return healthResource;
        }


        public CombatGroupType[] GetMyCombatGroup()
        {
            return combatGroups;
        }

        public void SetCharacterThatHitMe(IActorHub user)
        {
            lastCharacterToHitMe = user;
        }

        public void SetUser(IActorHub forUser)
        {
            owner = forUser;
        }

        public void SetInvincible(bool isImmoratal) => immortal = isImmoratal;
       
    }


  
}