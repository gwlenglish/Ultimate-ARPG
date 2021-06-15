
using GWLPXL.ARPGCore.Types.com;
using UnityEngine;

namespace GWLPXL.ARPGCore.Attributes.com
{


    public class EnemyAttributes : MonoBehaviour, IAttributeUser
    {

        [SerializeField]
        ActorAttributes statsTemplate;
        ActorAttributes runtimeCopy_stats;

        void Awake()
        {
            ActorAttributes temp = Instantiate(statsTemplate);
            SetRuntimeAttributes(temp);

            foreach (StatType pieceType in System.Enum.GetValues(typeof(StatType)))
            {
                GetRuntimeAttributes().GetStatNowValue(pieceType);
            }
        }


        public ActorAttributes GetAttributeTemplate()
        {
            return statsTemplate;
        }
        public void SetRuntimeAttributes(ActorAttributes newStats)
        {
            runtimeCopy_stats = newStats;
        }
        public virtual ActorAttributes GetRuntimeAttributes()
        {
            return runtimeCopy_stats;
        }

        public Transform GetInstance()
        {
            return transform;
        }

        public void SetAttributeTemplate(ActorAttributes newTemplate)
        {
            statsTemplate = newTemplate;
        }
    }
}