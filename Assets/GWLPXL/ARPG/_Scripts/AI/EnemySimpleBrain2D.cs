using GWLPXL.ARPGCore.Abilities.com;
using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Saving.com;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ARPGCore.AI.com
{


    public class EnemySimpleBrain2D : MonoBehaviour, ITick
    {
        public GameObject BlackboardObject;
        IAIEntity bb;
        Vector3 direction;
        public float AggroRange = 10;
        GameObject target;
        public float TickRate = .02f;
        IActorHub hub;
        private void Awake()
        {
            hub = GetComponent<IActorHub>();
            if (BlackboardObject == null)
            {
                bb = GetComponent<IAIEntity>();
            }
            else
            {
                bb = BlackboardObject.GetComponent<IAIEntity>();
            }
        }

        private void Start()
        {
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
            direction = new Vector3(0, 0, 0);
            float sqrdmag = 0;
            PlayerSceneReference[] players = DungeonMaster.Instance.GetAllSceneReferences();
            for (int i = 0; i < players.Length; i++)
            {
                Vector3 dir = players[i].SceneRef.transform.position - this.transform.position;
                sqrdmag = dir.sqrMagnitude;
                if (sqrdmag <= AggroRange * AggroRange)
                {
                    target = players[i].SceneRef.gameObject;
                    direction = dir;
                }
            }

            bb.SetMoveTarget(target);
            bb.SetDirection(direction.normalized);
            Ability ability = bb.GetActiveAbility();

            if (ability.GetRangeSquaredWithBuffer() <= sqrdmag)
            {
                bb.SetAttackTarget(target);
                bool success = hub.MyAbilities.TryCastAbility(ability);

            }

        }

        public float GetTickDuration() => TickRate;
      

        public void RemoveTicker()
        {
            TickManager.Instance.RemoveTicker(this);
        }
    }
}