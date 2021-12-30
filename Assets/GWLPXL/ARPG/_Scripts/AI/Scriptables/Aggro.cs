using GWLPXL.ARPGCore.Abilities.com;
using GWLPXL.ARPGCore.AI.com;
using GWLPXL.ARPGCore.com;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ARPGCore.States.com
{
    [CreateAssetMenu(menuName = "GWLPXL/ARPG/States/AI/Aggro")]


    public class Aggro : AIStateSO
    {
        public AggroVars Vars;
      
        public override void SetState(IStateMachine onMachine, IAIEntity forEntity)
        {
            GenericAggroState state = new GenericAggroState(forEntity, Vars);
            Func<bool> Condition() => () => this.GetTransition(forEntity);
            onMachine.AddAnyTransition(state, Condition());
            stateDic.Add(forEntity, state);
            
        }

       
    }

    [System.Serializable]
    public class AggroVars
    {
        public string AbilityStateName = string.Empty;
        public Ability Ability = null;
        public string ChaseStateName = "Walk";
  
    }

   
    public class GenericAggroState : IState
    {
        public IAIEntity Entity;
        protected AggroVars vars;

        public GenericAggroState(IAIEntity entity, AggroVars vars)
        {
            this.vars = vars;
            Entity = entity;
        }


        public void Enter()
        {
          

        }

       
        public void Exit()
        {
    
         


        }


        protected virtual void EndAbility(Ability ability)
        {
            if (ability == vars.Ability)
            {
                Entity.GetActorHub().MyAbilities.GetRuntimeController().OnAbilityEnd -= EndAbility;
               
            }
        }
        public void Tick()
        {
            if (Entity.GetAttackTarget() == null)
            {
                Debug.LogWarning("In Aggro state but no attack target is assigned");
                return;
            }

            if (Entity.GetActorHub().MyAbilities.GetRuntimeController().GetAbilityActive(vars.Ability)) return;

            Vector3 direction = Entity.GetAttackTarget().transform.position - Entity.GetActorHub().MyTransform.position;
            float sqrdmag = direction.sqrMagnitude;

            if (sqrdmag > vars.Ability.GetRangeSquaredWithBuffer() || //no range
                vars.Ability.HasSight(Entity.GetActorHub(), Entity.GetAttackTarget().transform, EditorPhysicsType.Unity3D) == false)//no sight
            {
                Entity.GetActorHub().MyAnimator.Play(vars.ChaseStateName);
                Entity.GetActorHub().MyMover.SetDesiredDestination(Entity.GetAttackTarget().transform.position, vars.Ability.GetRangeWithBuffer());
            }
            else
            {
                bool success = Entity.GetActorHub().MyAbilities.TryCastAbility(vars.Ability);
                if (success)
                {
                    
                    Entity.GetActorHub().MyAnimator.Play(vars.AbilityStateName, 0, 0f);//used for nonlooping animation to restart the animation from the beginning
                    Entity.GetActorHub().MyAbilities.GetRuntimeController().OnAbilityEnd += EndAbility;
                }
            }
            
           
        }
    }
}