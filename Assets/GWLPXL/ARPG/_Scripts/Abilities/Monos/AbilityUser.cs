﻿using GWLPXL.ARPGCore.Attributes.com;
using GWLPXL.ARPGCore.DebugHelpers.com;
using UnityEngine;
using GWLPXL.ARPGCore.com;
using System.Collections.Generic;

namespace GWLPXL.ARPGCore.Abilities.com
{
  

    public class AbilityUser : MonoBehaviour, IAbilityUser, ISubscribeEvents
    {
 
        [SerializeField]
        [Tooltip("Scene specific events")]
        ActorAbilityEvents abilityEvents = new ActorAbilityEvents();
        [SerializeField]
        AbilityController abilityControllerTemplate = null;
        [SerializeField]
        bool syncAnimationSpeedWithAbility = true;

        AbilityController runtime = null;
        Ability lastIntended = null;
        Transform me;
        IActorHub actorhub;
        bool basicattack = false;
        Ability charged = null;
        private void Awake()
        {
            me = GetComponent<Transform>();

            if (abilityControllerTemplate == null)
            {
                Debug.LogError("No ability controller, can't use abilities", this);
                return;
            }
            SetRuntimeController(Instantiate(GetTemplate()));


          
        }


        private void Update()
        {
            //if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            //{
            //    GetRuntimeController().InterruptAllAbilities();
            //    actorhub.MyAnim.SetAnimatorSpeed(1);
            //    actorhub.MyAnim.GetAnimator().Play("Movement");

            //}
        }
        //subscribe events
        public void SubscribeEvents()
        {
            GetRuntimeController().OnAbilityStart += AbilityStartEvent;
            GetRuntimeController().OnLearnedAbility += AbilityLearnEvent;
            GetRuntimeController().OnForgetAbility += AbilityForgetEvent;
            GetRuntimeController().OnAbilityEnd += AbiltiyEndEvent;

        }

        public void UnSubscribeEvents()
        {
            GetRuntimeController().OnAbilityStart -= AbilityStartEvent;
            GetRuntimeController().OnLearnedAbility -= AbilityLearnEvent;
            GetRuntimeController().OnForgetAbility -= AbilityForgetEvent;
            GetRuntimeController().OnAbilityEnd -= AbiltiyEndEvent;

        }

        void AbiltiyEndEvent(Ability ability)
        {
            abilityEvents.SceneEvents.OnAbilityEnd.Invoke(ability);
        }
        void AbilityForgetEvent(Ability ability, int slot)
        {
            abilityEvents.SceneEvents.OnAbilityForgot.Invoke(ability);
        }
        void AbilityLearnEvent(Ability ability, int slot)
        {
            abilityEvents.SceneEvents.OnAbilityLearned.Invoke(ability);
        }
        void AbilityStartEvent(Ability ability)
        {
            abilityEvents.SceneEvents.OnAbilityTriggered.Invoke(ability);
        }
        public Transform GetParentTransform() => me;
       


        public void SetTemplate(AbilityController newTemplate)
        {
            abilityControllerTemplate = newTemplate;
        }

        public bool TryCastAbility(Ability toCast)
        {
            if (actorhub.MyHealth != null)
            {
                if (actorhub.MyHealth.IsDead())
                {
                    //we dead
                    return false;
                }
            }

            

            if (toCast.Data.ResourceCost > 0)//only check if we have resource cost
            {
                if (actorhub.MyStats == null)
                {
                    ARPGDebugger.DebugMessage("Ability has a COST, but gameobject doesn't have a stat user. Can't cast", this);
                    return false;
                }

                
                if (actorhub.MyStats.GetRuntimeAttributes().DoWeHaveResources(toCast.Data.ResourceType, Mathf.FloorToInt(toCast.Data.ResourceCost)) == false)
                {
                    ARPGDebugger.DebugMessage("Ability has a COST, but gameobject doesn't have enough resources. Can't cast", this);
                    abilityEvents.SceneEvents.OnAbilityFailedCost.Invoke();
                    return false;
                }
            }

      


            bool success = GetRuntimeController().TryCastAbility(actorhub, toCast);

            ARPGDebugger.DebugMessage(this.gameObject.name + " " + success, this);

            if (success)
            {
                if (toCast.Data.ResourceCost > 0)
                {
                    actorhub.MyStats.GetRuntimeAttributes().ModifyNowResource(toCast.Data.ResourceType, -Mathf.FloorToInt(toCast.Data.ResourceCost));
                }
                TriggerAbilityAnimation(toCast);

                if (syncAnimationSpeedWithAbility)
                {
                    if (actorhub.MyAnim!=null)
                    {
                        actorhub.MyAnim.SetAnimatorSpeed(toCast.Duration / GetRuntimeController().GetClampedAbilitySpeedCooldown(toCast));
                        GetRuntimeController().OnAbilityEnd += ResetAnimator;
                    }
                    else
                    {
                        Debug.LogWarning("Can't sync animator without an animator on the object.");
                    }
                 
                }


            }
            
            return success;

        }

     
      void ResetAnimator(Ability ability)
        {
            actorhub.MyAnim.SetAnimatorSpeed(1);
            GetRuntimeController().OnAbilityEnd -= ResetAnimator;
        }
        private void TriggerAbilityAnimation(Ability toCast)
        {

            if (actorhub.MyAnim != null)
            {
                for (int i = 0; i < GetRuntimeController().BasicAttacks.Length; i++)
                {
                    if (toCast == GetRuntimeController().BasicAttacks[i])
                    {
                        actorhub.MyAnim.TriggerBasicAttackAnimation(toCast.Data.AnimationTrigger, toCast.Data.AnimationIndex, toCast.CanLoop);
                        return;
                    }
                }
                actorhub.MyAnim.TriggerAbilityAnimation(toCast.Data.AnimationTrigger, toCast.Data.AnimationIndex, toCast.CanLoop);


            }
        }

        public bool GetInCooldown()
        {
            AbilityDurationTimer timer = GetRuntimeController().GetTimer(GetLastIntendedAbility());
            return timer != null;
        }
        public Ability GetLastIntendedAbility()
        {
            return lastIntended;
        }

        public void SetIntendedAbility(Ability ability)
        {
            if (ability == null)
            {
                Ability previous = GetLastIntendedAbility();
                if (previous != null)
                {
                    if (previous.CanLoop && actorhub.MyAnim != null)
                    {
                        actorhub.MyAnim.SetLooping(false);
                        actorhub.MyAnim.GetAnimator().ResetTrigger(previous.Data.AnimationTrigger);
                        
                    }
                }
            }

            for (int i = 0; i < GetRuntimeController().BasicAttacks.Length; i++)
            {
                if (ability == GetRuntimeController().BasicAttacks[i])
                {
                    basicattack = true;
                    break;
                }
            }

            
            lastIntended = ability;
        }

        public void SetIntendedAbility(int equippedAbilitySlot)
        {
            Ability ability = runtime.GetEquippedAbility(equippedAbilitySlot);
            if (ability == GetLastIntendedAbility()) return;//same thing, dont bother looking it up
            if (ability != null )
            {
                basicattack = false;
             
                if (GetInCooldown() == false)
                {
                    SetIntendedAbility(ability);
                }
            }
            else
            {
                ARPGDebugger.DebugMessage("Ability at slot " + equippedAbilitySlot + " is NULL. Can't set as intended ability", this);
            }
          
        }

        public AbilityController GetRuntimeController()
        {
            return runtime;
        }

        public AbilityController GetTemplate()
        {
            return abilityControllerTemplate;
        }

        public void SetRuntimeController(AbilityController abilityController)
        {
            if (runtime != null)
            {
                UnSubscribeEvents();
            }
            runtime = abilityController;

            if (runtime != null)
            {
                SubscribeEvents();
            }
        }

        public void SetIntendedBasicAttack()
        {
            Ability ability = GetRuntimeController().GetBasicAttack(actorhub);
            if (ability == GetLastIntendedAbility()) return;//same thing, dont bother looking it up
            if (ability != null)
            {
                basicattack = true;
             
                if (GetInCooldown() == false)
                {
                    SetIntendedAbility(ability);
                }


            }
            else
            {
              //  ARPGDebugger.DebugMessage("Ability at slot " + equippedAbilitySlot + " is NULL. Can't set as intended ability", this);
            }
        }

        /// <summary>
        /// call into to modify the ability speed.
        /// </summary>
        /// <param name="byAmount"></param>
        public void ModifyAbilityMulti(float byAmount)//modifiables
        {
           
            GetRuntimeController().ModifyAbilityMulti(byAmount);
           
        }

        public void SetActorHub(IActorHub hub) => actorhub = hub;

        public IActorHub GetActorHub() => actorhub;

        public Ability GetChargedAbility() => charged;
       

        public void SetChargedAbility(Ability ability) => charged = ability;
       
    }


    
}