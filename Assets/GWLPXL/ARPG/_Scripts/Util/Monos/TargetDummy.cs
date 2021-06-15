
using GWLPXL.ARPGCore.Attributes.com;
using GWLPXL.ARPGCore.CanvasUI.com;
using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Combat.com;
using GWLPXL.ARPGCore.DebugHelpers.com;
using GWLPXL.ARPGCore.Leveling.com;
using GWLPXL.ARPGCore.Statics.com;
using GWLPXL.ARPGCore.Types.com;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ARPGCore.Demo.com
{


    public class TargetDummy : MonoBehaviour, IReceiveDamage, IScale
    {
        public int Level;
        public IAttributeUser MyStats { get; set; }
        [SerializeField]
        ResourceType healthResource = ResourceType.Health;
        [SerializeField]
        CombatGroupType[] combatGroups = new CombatGroupType[1] { CombatGroupType.Enemy };
        bool canhit;
        IActorHub owner = null;
        IUseFloatingText dungeoncanvas = null;
        bool immortal = true;
        private void Awake()
        {
            MyStats = GetComponent<IAttributeUser>();
            dungeoncanvas = GetComponent<IUseFloatingText>();
        }

        private void Start()
        {
            canhit = true;
            MyStats.GetRuntimeAttributes().LevelUp(Level);
        }
        public void Die()
        {
            //never die
        }

        public Transform GetInstance()
        {
            return this.transform;
        }

        public bool IsDead()
        {
            return false;
        }


        public void TakeDamage(int damageAmount, IActorHub damageDealer)
        {
            int wpndmg = EnemyCombatResolution.DoEnemyPhysicalReducedChecks(MyStats, damageAmount, healthResource);
            NotifyUI(ElementType.None, wpndmg);

            Dictionary<ElementType, ElementAttackResults> results = EnemyCombatResolution.DoEnemyElementalDamageResistChecks(MyStats, damageDealer.MyStats, healthResource);
            foreach (var kvp in results)
            {
                TakeDamage(kvp.Value.Damage, kvp.Key);
            }

            CheckDeath();

        }
     
        public void TakeDamage(int damageAmount, ElementType type)
        {
            int damage = EnemyCombatResolution.DoEnemyElementalDamageWithResistChecks(MyStats, type, damageAmount, healthResource);
            NotifyUI(type, damage);
            CheckDeath();

        }

        private void NotifyUI(ElementType type, int damage)
        {
            if (dungeoncanvas == null) return;
            dungeoncanvas.CreateUIDamageText("-" + damage.ToString(), type);

        }

        public int GetScaledLevel()
        {
            return Formulas.GetEnemyLevel(MyStats.GetRuntimeAttributes().MyLevel);
        }

        public int GetUNScaledLevel()
        {
            return Level;
        }

        public bool IsHurt()
        {
            return !canhit;
        }

        public void CheckDeath()
        {
           //target dummy doesn't really die
        }

        public ResourceType GetHealthResource()
        {
            return healthResource;
        }

        public void TakeDamage(int damageAmount, ElementType type, IAttributeUser owner)
        {
            TakeDamage(damageAmount, type);
        }

        public CombatGroupType[] GetMyCombatGroup()
        {
            return combatGroups;
        }

        public void SetUNScaledLevel(int unscaled)
        {
            MyStats.GetRuntimeAttributes().LevelUp(unscaled);
        }

        public void SetCharacterThatHitMe(IActorHub user)
        {
           //we dont really care about saving this
        }

        public void SetUser(IActorHub forUser)
        {
            owner = forUser;
        }

        public void SetActorHub(IActorHub newHub) => owner = newHub;

        public void SetInvincible(bool isImmoratal) => immortal = isImmoratal;
       



        //bypasses the iframes

    }
}
