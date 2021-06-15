

using GWLPXL.ARPGCore.AI.com;
using GWLPXL.ARPGCore.com;

using UnityEngine;

namespace GWLPXL.ARPGCore.States.com
{


    public class ActorStateMachine : MonoBehaviour, IStateMachineEntity, ITick
    {
        public GameObject ActorHub;
        IStateMachine machine;
        [Tooltip("These have priority over movement states.")]
        public MovementStates MovementStates = null;

        IActorHub actorhub = null;
        IAIEntity ai = null;

        public I2DStateMachine Machine2D { get; set; }

        protected virtual void Awake()
        {
            ai = GetComponent<IAIEntity>();
            if (ActorHub == null)
            {
                actorhub = GetComponent<IActorHub>();
            }
            else
            {
                actorhub = ActorHub.GetComponent<IActorHub>();
            }
           
        }


        protected virtual void Start()
        {
            machine = new IStateMachine();

           

            if (MovementStates != null)
            {
                //these have priority over idle
                for (int i = 0; i < MovementStates.Moving.Length; i++)
                {
                    MovementStates.Moving[i].SetState(machine, this);

                }

                for (int i = 0; i < MovementStates.Idle.Length; i++)
                {
                    MovementStates.Idle[i].SetState(machine, this);
                }
            }
         

            AddTicker();


        }

        protected virtual void OnDestroy()
        {
            RemoveTicker();
        }

        public I2DStateMachine Get2D() => null;

        public IActorHub GetActorHub() => actorhub;
      
  
      

     

        public void SetActorHub(IActorHub newHub)
        {
            actorhub = newHub;
        }

        public void AddTicker()
        {
            TickManager.Instance.AddTicker(this);
        }

        public void DoTick()
        {

            machine.Tick();
            //Debug.Log(this.gameObject.name+ " " + machine.GetCurrentlyRunnnig());
        }

        public void RemoveTicker()
        {
            TickManager.Instance.RemoveTicker(this);
        }

        public float GetTickDuration()
        {
            return Time.deltaTime;
        }

        public IAIEntity GetAI() => ai;
       
    }
}
