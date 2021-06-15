using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GWLPXL.ARPGCore.States.com
{

    [CreateAssetMenu(menuName = "GWLPXL/ARPG/States/Player/3D/NavMesh Idle")]

    public class NavMeshIdle : PlayerMovementState
    {
        public override bool GetTransition(IStateMachineEntity forEntity)
        {
            return forEntity.GetActorHub().MyMover.GetMoverEnabled() == true 
                && forEntity.GetActorHub().MyMover.GetVelocitySquaredMag() < 1 && 
                forEntity.GetActorHub().MyAbilities.GetRuntimeController().IsUsingAbility() == false;

        }

        public override void SetState(IStateMachine onMachine, IStateMachineEntity forEntity)
        {
            IdleState state = new IdleState(forEntity, Vars);
            Func<bool> Condition() => () => this.GetTransition(forEntity);
            onMachine.AddAnyTransition(state, Condition());
            stateDic.Add(forEntity, state);
        }
    }


    public class IdleState : IState
    {
        MoveVars vars;
        IStateMachineEntity hub;
        public IdleState(IStateMachineEntity hub, MoveVars vars)
        {
            this.vars = vars;
            this.hub = hub;

        }
        public void Enter()
        {
            if (hub.GetActorHub().MyAnimator != null)
            {
                hub.GetActorHub().MyAnimator.Play(vars.AnimatorStateName);
            }

        }

        public void Exit()
        {
 

        }

        public void Tick()
        {

        }
    }
}
