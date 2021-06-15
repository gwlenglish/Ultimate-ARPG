using GWLPXL.ARPGCore.com;

using GWLPXL.ARPGCore.Types.com;

using UnityEngine;

namespace GWLPXL.ARPGCore.Factions.com
{
    [System.Serializable]
    public class RelationChange
    {
        [Tooltip("The faction to affect")]
        public FactionTypes ForFaction;
        [Tooltip("Negative for losing faction rep, positive for gaining")]
        public int AmountToChange;
    }
    /// <summary>
    /// to do, use a matrix for the rep worth, allows to increase some reps and decrease others.
    /// </summary>
    public class EnemyFactionMember : MonoBehaviour, IFactionMember
    {
        public ActorFactionEvents FactionEvents;
        [SerializeField]
        RelationChange[] factionChangesOnDeath = new RelationChange[0];
        [SerializeField]
        FactionTypes myFaction = FactionTypes.None;
        EnemyHealth hp = null;

        private void Awake()
        {
            hp = GetComponent<EnemyHealth>();
        }
        private void OnEnable()
        {
            if (hp != null)
            {
                hp.OnDeath +=ModifyPlayerRep;
            }
        }

        private void OnDisable()
        {
            if (hp != null)
            {
                hp.OnDeath -= ModifyPlayerRep;
            }
        }

        void ModifyPlayerRep()
        {
            FactionManager.Instance.ModifyPlayerRep(factionChangesOnDeath);
           
        }
        public void DecreaseRep(FactionTypes withFaction, int amount)
        {
            FactionManager.Instance.DecreaseFactionRep(GetFaction(), withFaction, amount);
            FactionEvents.SceneEvents.OnRepDecreased.Invoke(withFaction, amount);
            FactionEvents.SceneEvents.OnAnyRepModified.Invoke();
        }

        public FactionTypes GetFaction() => myFaction;

        public int GetFactionRep(FactionTypes withFaction)
        {
            return FactionManager.Instance.GetRepValue(GetFaction(), withFaction);
        }

        public void IncreaseRep(FactionTypes withFaction, int amount)
        {
            FactionManager.Instance.IncreaseFactionRep(GetFaction(), withFaction, amount);
            FactionEvents.SceneEvents.OnRepIncreased.Invoke(withFaction, amount);
            FactionEvents.SceneEvents.OnAnyRepModified.Invoke();
        }
    }
}