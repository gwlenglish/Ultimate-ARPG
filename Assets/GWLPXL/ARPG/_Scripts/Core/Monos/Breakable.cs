﻿
using GWLPXL.ARPGCore.Abilities.com;
using GWLPXL.ARPGCore.Animations.com;
using GWLPXL.ARPGCore.Attributes.com;
using GWLPXL.ARPGCore.Auras.com;
using GWLPXL.ARPGCore.CanvasUI.com;
using GWLPXL.ARPGCore.Classes.com;
using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Combat.com;
using GWLPXL.ARPGCore.Factions.com;
using GWLPXL.ARPGCore.Items.com;
using GWLPXL.ARPGCore.Leveling.com;
using GWLPXL.ARPGCore.Movement.com;
using GWLPXL.ARPGCore.Quests.com;
using GWLPXL.ARPGCore.States.com;
using GWLPXL.ARPGCore.Statics.com;
using GWLPXL.ARPGCore.StatusEffects.com;
using GWLPXL.ARPGCore.Types.com;
using GWLPXL.ARPGCore.Wearables.com;
using UnityEngine;

namespace GWLPXL.ARPGCore.Looting.com
{
    [System.Serializable]
    public class BreakableVars
    {
        public ResourceType HpType = ResourceType.Health;
        public Transform ParentofOriginalMesh = null;
        public Transform ParentofBrokenVersion = null;
        public CombatGroupType[] Combatgroups = new CombatGroupType[2] { CombatGroupType.Enemy, CombatGroupType.Neutral };
        public BreakableVars(ResourceType hptype, Transform parentoforiginal, Transform parentofbroken, CombatGroupType[] combatgroups)
        {
            HpType = hptype;
            ParentofOriginalMesh = parentoforiginal;
            ParentofBrokenVersion = parentofbroken;
            Combatgroups = combatgroups;
        }
    }
    [RequireComponent(typeof(IAttributeUser))]
    [RequireComponent(typeof(IDropLoot))]
    public class Breakable : MonoBehaviour, IReceiveDamage, IActorHub
    {
        [SerializeField]
        ActorHealthEvents breakableEvents = new ActorHealthEvents();

        [SerializeField]
        BreakableVars vars;
 

        bool isBroken = false;
        bool hasBrokenVersion = false;
        IDropLoot dropLoot = null;

        bool immortal = false;
        //player
        public IPlayerControlled PlayerControlled { get; set; }
        public IPlayerInputHub InputHub { get; set; }
        public ILevel Level { get; set; }
        public IQuestUser MyQuests { get => null; set => MyQuests = value; }
        //combat actor
        public IInventoryUser MyInventory { get => null; set => MyInventory = value; }
        public IMeleeCombatUser MyMelee { get => null; set => MyMelee = value; }
        public IProjectileCombatUser MyProjectiles { get => null; set => MyProjectiles = value; }
        public IUseFloatingText DungUser { get => null; set => DungUser = value; }
        public IWearClothing Clothing { get => null; set => Clothing = value; }
        public IAbilityUser MyAbilities { get => null; set => MyAbilities = value; }
        public IUseAura MyAuraUser { get => null; set => MyAuraUser = value; }
        public ITakeAura MyAuraTaker { get => null; set => MyAuraTaker = value; }
        public IClassUser MyClass { get => null; set => MyClass = value; }
        public IMover MyMover { get => null; set => MyMover = value; }
        public IAnimate MyAnim { get =>null; set => MyAnim = value; }
        public IRecieveStatusChanges MyStatusEffects { get => null; set => MyStatusEffects = value; }
        public IStateMachineEntity MyStateMachine { get => null; set => MyStateMachine = value; }
        public ISubscribeEvents Eventcontroller { get => null; set => Eventcontroller = value; }

        public IAttributeUser MyStats { get; set; }
        public IReceiveDamage MyHealth { get; set; }
        public Animator MyAnimator { get; set; }
        public Transform MyTransform { get; set; }
        public INavMeshMover NavMeshAgent { get; set; }
        public IFactionMember MyFaction { get; set; }

        private void Awake()
        {
            MyTransform = this.transform;
            MyAnimator = null;
            MyHealth = this;
            MyStats = GetComponent<IAttributeUser>();
            dropLoot = GetComponent<IDropLoot>();
            if (vars.ParentofBrokenVersion != null && vars.ParentofOriginalMesh != null)
            {
                hasBrokenVersion = true;
                vars.ParentofBrokenVersion.gameObject.SetActive(false);
                vars.ParentofOriginalMesh.gameObject.SetActive(true);
            }

            IScale scaler = GetComponent<IScale>();
            if (scaler != null)
            {
                scaler.SetActorHub(this);
            }
        }

       public void SetVars(BreakableVars newVars)
        {
            vars = newVars;
        }
        public void Die()
        {
            if (isBroken) return;
            isBroken = true;
            if (hasBrokenVersion)
            {
                vars.ParentofOriginalMesh.gameObject.SetActive(false);
                vars.ParentofBrokenVersion.gameObject.SetActive(true);
            }
            breakableEvents.SceneEvents.OnDie.Invoke();
            dropLoot.DropLoot();
        }

        public ResourceType GetHealthResource()
        {
            return vars.HpType;
        }

        public Transform GetInstance()
        {
            return this.transform;
        }

        public bool IsDead()
        {
            return isBroken;
        }

        public bool IsHurt()
        {
            return false;
        }

        public void TakeDamage(int damageAmount, ElementType type)
        {
            if (IsDead()) return;
            int dmg = 0;
            if (immortal == false)
            {
                 dmg = CombatResolution.DoBreakableDamage(MyStats, Mathf.Abs(damageAmount), vars.HpType);
            }
            breakableEvents.SceneEvents.OnDamageTaken.Invoke(dmg);

            CheckDeath();
        }

        public void TakeDamage(int damageAmount, IActorHub damageDealer)
        {
            if (IsDead()) return;
            int dmg = 0;
            if (immortal == false)
            {
                dmg = CombatResolution.DoBreakableDamage(MyStats, Mathf.Abs(damageAmount), vars.HpType);
            }
            breakableEvents.SceneEvents.OnDamageTaken.Invoke(dmg);
            CheckDeath();
        }


        public void CheckDeath()
        {
            if (MyStats.GetRuntimeAttributes().GetResourceNowValue(vars.HpType) <= 0)
            {
                Die();
            }

        }

        public void TakeDamage(int damageAmount, ElementType type, IActorHub owner)
        {

            if (owner == null)
            {
                TakeDamage(damageAmount, type);
            }
            else
            {
                TakeDamage(damageAmount, owner);

            }
        }

        public CombatGroupType[] GetMyCombatGroup()
        {
            return vars.Combatgroups;
        }

        public void SetCharacterThatHitMe(IActorHub user)
        {
           //we dont really care about saving this
        }

        public void SetUser(IActorHub forUser)
        {
           
        }

        public void SetInvincible(bool isImmoratal) => immortal = isImmoratal;
       
    }
}