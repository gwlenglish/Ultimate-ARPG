
using GWLPXL.ARPGCore.Attributes.com;
using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Types.com;
using UnityEngine;

namespace GWLPXL.ARPGCore.Combat.com
{

    public interface IReceiveDamage
    {
        void SetCharacterThatHitMe(IActorHub user);
        CombatGroupType[] GetMyCombatGroup();
        void CheckDeath();
        Transform GetInstance();
        bool IsDead();

        /// <summary>
        /// elemental damage without an owner, i.e. environmental hazard
        /// </summary>
        /// <param name="damageAmount"></param>
        /// <param name="type"></param>
        /// 
        //[System.Obsolete]
        //void TakeDamage(int damageAmount, ElementType type);
        ///// <summary>
        ///// damage with player / enemy formula defaults
        ///// </summary>
        ///// <param name="damageAmount"></param>
        ///// <param name="damageDealer"></param>
        ///// 

        //[System.Obsolete]
        //void TakeDamage(int damageAmount, IActorHub damageDealer);

        void TakeDamage(AttackValues values);
        void Die();
        bool IsHurt();
        ResourceType GetHealthResource();
        void SetUser(IActorHub forUser);
        IActorHub GetUser();
        /// <summary>
        /// used to make an actor immortal, e.g. target dummies or invincibility
        /// </summary>
        /// <param name="isInvincible"></param>
        void SetInvincible(bool isInvincible);

    }
}