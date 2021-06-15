
using UnityEngine;
using GWLPXL.ARPGCore.States.com;
using GWLPXL.ARPGCore.com;

namespace GWLPXL.ARPGCore.AI.com
{
    
    /// <summary>
    /// 
    /// </summary>
    public class EnemyStateMachine : MonoBehaviour, ITick, IStateMachineEntity
    {
        public GameObject BlackBoard = null;
        public AIStateSO[] States = new AIStateSO[0];

        IStateMachine machine;
        IActorHub hub;
        IAIEntity ai;
        I2DStateMachine state2d = null;
        public I2DStateMachine Machine2D { get; set; }

        private void Awake()
        {
            state2d = GetComponent<I2DStateMachine>();
            if (BlackBoard == null) ai = GetComponent<IAIEntity>();
            if (BlackBoard != null) ai = BlackBoard.GetComponent<IAIEntity>();
        }
        private void Start()
        {
            machine = new IStateMachine();
            for (int i = 0; i < States.Length; i++)
            {
                States[i].SetState(machine, ai);
            }
         
           
            AddTicker();
        }

   
        private void OnDestroy()
        {
            RemoveTicker();
        }
        public void AddTicker()
        {
            TickManager.Instance.AddTicker(this);
        }

        public void DoTick()
        {
           
           // Debug.Log("Enemy" + machine.GetCurrentlyRunnnig());
            machine.Tick();
        }

        public float GetTickDuration()
        {
            return Time.deltaTime;
        }

        public void RemoveTicker()
        {
            TickManager.Instance.RemoveTicker(this);
        }

        public Transform GetInstance() => this.transform;

     

        public IActorHub GetActorHub() => hub;

        public I2DStateMachine Get2D() => state2d;
     

        public void SetActorHub(IActorHub newHub)
        {
            hub = newHub;
        }

        public IAIEntity GetAI()
        {
            return ai;
        }
    }
}