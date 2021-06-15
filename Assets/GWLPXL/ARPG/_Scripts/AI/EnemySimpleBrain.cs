using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Statics.com;
using UnityEngine;


namespace GWLPXL.ARPGCore.AI.com
{

    /// <summary>
    /// simple brain that aggros the last thing that hit it and idles after a certain amount of time
    /// </summary>
    public class EnemySimpleBrain : MonoBehaviour, ITick
    {
        public GameObject ActorHub = null;
        [Tooltip("If greater than 0, will aggro based on sight.")]
        public float AggroDetectRange = 1;
        [Tooltip("Field of vision")]
        public float AggroSightAngle = 45;
        [Tooltip("Layers that will block sight completely.")]
        public LayerMask SightBlockingLayers;
        public string DefaultKey = "Aggro";
        public string AggroKey = "Aggro";
        public float AggroDuration = 7f;

        public string IdleKey = "Idle";
        public string DeathKey = "Death";
        public string HurtKey = "Hurt";
        [Range(0, 1f)]
        [Tooltip("How often to play the hurt state. At 1 (100%), you can stunlock so you probably don't want that.")]
        public float RandomizedHurtChance = .10f;
        bool hurt = true;
        IAIEntity bb = null;
        float aggrotimer = 0;
        bool aggroed = false;

        bool dead;

        private void Awake()
        {
            bb = GetComponent<IAIEntity>();
        }

        
        private void Start()
        {
            if (ActorHub != null)
            {
                EnemyHealth health = ActorHub.GetComponent<IActorHub>().MyHealth as EnemyHealth;
                if (health != null)
                {
                    health.OnDeath += Dead;
                    health.OnDamagedMe += AggroPlayer;
                    health.OnDamagedMe += TookDamage;
                }
            }

            for (int i = 0; i < DungeonMaster.Instance.GetAllSceneReferences().Length; i++)
            {
                if (bb.GetAttackTarget() == null)
                {
                    bb.SetAttackTarget(DungeonMaster.Instance.GetAllSceneReferences()[i].SceneRef.gameObject);
                }
                if (bb.GetMoveTarget() == null)
                {
                    bb.SetMoveTarget(DungeonMaster.Instance.GetAllSceneReferences()[i].SceneRef.gameObject);
                }

            }
            bb.SetStateKey(DefaultKey);
            AddTicker();
        }

        private void OnDestroy()
        {
            EnemyHealth health = ActorHub.GetComponent<IActorHub>().MyHealth as EnemyHealth;
            if (health != null)
            {
                health.OnDamagedMe -= AggroPlayer;
                health.OnDeath -= Dead;
                health.OnDamagedMe -= TookDamage;
            }
            RemoveTicker();
        }

        void Dead()
        {
            dead = true;
            bb.SetStateKey(DeathKey);
        }

        void Idle()
        {
            if (dead) return;
            bb.SetStateKey(IdleKey);
        }

   
        void TookDamage(IActorHub aggrotarget)
        {
            if (dead) return;
            int rando = Random.Range(0, 101);
            float percent = (float)rando / (float)Formulas.Hundred;
            //Debug.Log("Hurt Chance " + percent);
            if (percent <= RandomizedHurtChance)
            {
                hurt = true;
                bb.SetStateKey(HurtKey);
            }
            else
            {
                hurt = false;
            }

        }


        void AggroPlayer(IActorHub aggrotarget)
        {
            if (dead) return;
            if (aggrotarget != null)
            {
                bb.SetAttackTarget(aggrotarget.MyTransform.gameObject);

            }
            Aggro();
        }

      
        private void Aggro()
        {
            bb.SetStateKey(AggroKey);
            aggroed = true;
            aggrotimer = 0;
        }

        public void AddTicker()
        {
            TickManager.Instance.AddTicker(this);
        }

        bool CheckAggro()
        {
            return bb.GetAttackTarget() != null && AggroSightAngle > 0 && AggroDetectRange > 0;
        }
        public void DoTick()
        {
            if (dead) return;

            if (bb.GetActorHub().MyHealth.IsHurt() && hurt)
            {
                bb.SetStateKey(HurtKey);
                return;
            }
            else
            {
                hurt = false;
            }

            
            if (CheckAggro())
            {
                //check sight and range
                Vector3 direction = bb.GetAttackTarget().transform.position - this.transform.position;
                float sqrdmag = direction.sqrMagnitude;
                if (sqrdmag <= AggroDetectRange * AggroDetectRange)
                {
                    if (CombatHelper.HasSight(bb.GetActorHub(), bb.GetAttackTarget().transform, EditorPhysicsType.Unity3D, AggroSightAngle))
                    {
                        if (CombatHelper.HasLineOfSight(bb.GetActorHub().MyTransform.gameObject, bb.GetAttackTarget(), SightBlockingLayers, AggroDetectRange + 1))//+1 is just a buffer.
                        {
                            Aggro();
                        }
                    }
                }

            }

            if (aggroed)
            {
                aggrotimer += GetTickDuration();
                if (aggrotimer >= AggroDuration)
                {
                    aggrotimer = 0;
                    Idle();
                    aggroed = false;
                }
            }
        }

       
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, AggroDetectRange);
        }
        public void RemoveTicker()
        {
            TickManager.Instance.RemoveTicker(this);
        }

        public float GetTickDuration() => Time.deltaTime;

    }
}