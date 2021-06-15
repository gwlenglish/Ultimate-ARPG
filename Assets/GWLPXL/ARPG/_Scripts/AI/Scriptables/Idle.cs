
using UnityEngine;
using GWLPXL.ARPGCore.States.com;
using System;
using UnityEngine.AI;

namespace GWLPXL.ARPGCore.AI.com
{


    [System.Serializable]
    public class IdleVars
    {
        public string AnimatorStateName = "Idle";

    }


    [CreateAssetMenu(menuName = "GWLPXL/ARPG/States/AI/Idle")]

    public class Idle : AIStateSO
    {

        public IdleVars Vars;


        public override void SetState(IStateMachine onMachine, IAIEntity forEntity)
        {
            IdleState state = new IdleState(forEntity, Vars);
            Func<bool> HasWalkingTarget() => () => this.GetTransition(forEntity);
            onMachine.AddAnyTransition(state, HasWalkingTarget());
            stateDic.Add(forEntity, state);
        }

      
    }

    [System.Serializable]
    public class IdleState : IState
    {
        public IAIEntity Entity;
        IdleVars vars;


        public IdleState(IAIEntity entity, IdleVars vars)
        {
            this.vars = vars;
            Entity = entity;
        }


        public void Enter()
        {
            Entity.GetActorHub().MyAnimator.Play(vars.AnimatorStateName);
            Entity.GetActorHub().MyMover.SetDesiredDestination(Entity.GetActorHub().MyTransform.position, 1f);
   
        }

        public void Exit()
        {
          

        }

        public void Tick()
        {

           
        }
    }
}