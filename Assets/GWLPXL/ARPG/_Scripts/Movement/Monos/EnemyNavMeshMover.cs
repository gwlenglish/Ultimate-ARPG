
using GWLPXL.ARPGCore.com;

using GWLPXL.ARPGCore.DebugHelpers.com;

using GWLPXL.ARPGCore.States.com;


using UnityEngine;
using UnityEngine.AI;

namespace GWLPXL.ARPGCore.Movement.com
{
    


    //this seems like too much. separate out the ieenemyai and states.
    public class EnemyNavMeshMover : MonoBehaviour, IMover, IChangeStates, ITick, INavMeshMover
    {
        public NavMeshAgent Agent;
        float speedMulit = 1;
        float distanceToPlayerSquared;
        Vector3 startingPosition;
        float originalSpeed;
        float originalAccel;
        float baseheight = 0;
        IActorHub hub;


        private void Awake()
        {
            baseheight = Agent.baseOffset;
            originalAccel = Agent.acceleration;
            originalSpeed = Agent.speed;

        }
        private void OnDestroy()
        {
            RemoveTicker();
        }

        protected void Start()
        {
            SetUpMover();
            hub.MyAbilities.SetIntendedAbility(0);
        }

 

     

        public void SetDesiredDestination(Vector3 newDestination, float stoppingDistance)
        {
            if (Agent.isActiveAndEnabled)
            {
                NavMeshHit navHit;
                if (NavMesh.SamplePosition(newDestination, out navHit, stoppingDistance, NavMesh.AllAreas))
                {
                    newDestination = navHit.position;
                    //we need to check for our rotation. Using NavAgent rotatation makes it like we're steering a car.
                    SetDesiredRotation(newDestination, stoppingDistance);

                    //NavMeshAgent calls
                    Agent.stoppingDistance = stoppingDistance;
                    Agent.SetDestination(newDestination);

                }
                else
                {
                    ARPGDebugger.DebugMessage("No suitable NavMesh Path Found", this);
                }
                //Debug.Log("Desired called");

                //Agent.stoppingDistance = stoppingDistance;
                //Agent.SetDestination(newDestination);
            }

        }
        public void SetDesiredRotation(Vector3 towards, float stoppingDistance)
        {
            Vector3 lookPositon = towards - transform.position;
            float sqrdMag = lookPositon.sqrMagnitude;
            if (sqrdMag > stoppingDistance * stoppingDistance)//this check is to prevent look rotations of Vector3.zero (if the player's position is on top of the destination). I use stopping distance just because it's available, 1f hard code works fine too.
            {
                //does a bit of a tilt
                lookPositon.y = 0;
                transform.rotation = Quaternion.LookRotation(lookPositon);
            }
        }
        public void StopAgentMovement(bool isStopped)
        {
            if (Agent.isActiveAndEnabled)
            {
                Agent.isStopped = isStopped;
                Agent.velocity = Vector3.zero;
            }



        }


        public void DisableNavMeshAgent()
        {
            Agent.enabled = false;
        }


        public void SetNewSpeed(float newTopSpeed, float newAcceleration)
        {
            Agent.speed = newTopSpeed;
            Agent.acceleration = newAcceleration;
        }

        public void ResetSpeed()
        {
            SetNewSpeed(originalSpeed, originalAccel);
        }


        public void ResetState()
        {
           
        }

        public void SetVelocity(Vector3 newVel)
        {
            Agent.velocity = newVel;
        }

        public void ChangeState(IState newstate)
        {
           


        }

        public void DefaultMoveState()
        {

        }

        public GameObject GetGameObject()
        {
            return this.gameObject;
        }

        public void AddTicker()
        {
            TickManager.Instance.AddTicker(this);

        }

        public void DoTick()
        {
          

        }

        public void RemoveTicker()
        {
            TickManager.Instance.RemoveTicker(this);

        }

        public float GetTickDuration()
        {
            return Time.deltaTime;
        }

        public void SetUpMover()
        {
            AddTicker();
            startingPosition = transform.position;

         
        }

      
        public IActorHub GetHub() => hub;
      

        public void SetActorHub(IActorHub newHub) => hub = newHub;
       
        public void DisableMovement(bool isStopped)
        {
            StopAgentMovement(isStopped);
        }

        public float GetVelocitySquaredMag()
        {
            return Agent.velocity.sqrMagnitude;
        }

        public void ModifySpeedMultiplier(float byAmount)
        {
            speedMulit += byAmount;
            Agent.speed = (originalSpeed * speedMulit);
            Agent.acceleration = (originalAccel* speedMulit);
        }

        public bool GetMoverEnabled()
        {
            return Agent.isActiveAndEnabled;
        }

        public NavMeshAgent GetAgent()
        {
            return Agent;
        }

        public void SetAgentBaseHeight(float newheight)
        {
            Agent.baseOffset = newheight;
            Debug.Log("Base height " + Agent.baseOffset);
        }

        public void ResetBaseHeight()
        {
            Agent.baseOffset = baseheight;
        }

        public void EnableUpdate(bool updatePosition, bool updateRotation)
        {
            Agent.updatePosition = updatePosition;
            Agent.updateRotation = updateRotation;
        }

        public void SetAgentPositionRotaion(Vector3 newpos, Quaternion newRot)
        {
            Agent.nextPosition = newpos;
        }

        public float GetSpeedMultiplier() => speedMulit;

        public Vector3 GetVelocityDirection() => GetAgent().velocity.normalized;
       
    }
}