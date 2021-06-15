
using UnityEngine;
using GWLPXL.ARPGCore.States.com;
using System;
using UnityEngine.AI;

namespace GWLPXL.ARPGCore.AI.com
{


    [System.Serializable]
    public class HurtVars
    {
        public string AnimatorStateName = "Hurt";

    }


    [CreateAssetMenu(menuName = "GWLPXL/ARPG/States/AI/Hurt")]

    public class Hurt : AIStateSO
    {

        public HurtVars Vars;


        public override void SetState(IStateMachine onMachine, IAIEntity forEntity)
        {
            HurtState state = new HurtState(forEntity, Vars);
            Func<bool> HasWalkingTarget() => () => this.GetTransition(forEntity);
            onMachine.AddAnyTransition(state, HasWalkingTarget());
            stateDic.Add(forEntity, state);
        }

      
    }

    [System.Serializable]
    public class HurtState : IState
    {
        public IAIEntity Entity;
        HurtVars vars;


        public HurtState(IAIEntity entity, HurtVars vars)
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