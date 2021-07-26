
using UnityEngine;
using GWLPXL.ARPGCore.Types.com;

using GWLPXL.ARPGCore.com;

namespace GWLPXL.ARPGCore.Auras.com
{
  

    public class PlayerAuraReceiver : MonoBehaviour, ITakeAura
    {
        [SerializeField]
        PlayerAuraReceiveEvents receiveEvents = new PlayerAuraReceiveEvents();
        [SerializeField]
        AuraTargetGroup[] mygroups = new AuraTargetGroup[1] { AuraTargetGroup.Friendly };


        IActorHub hub = null;
       
        public void AuraModifyCurrentResource(int resourceType, int byAmount)
        {
            if (hub.MyHealth != null)
            {
                if (hub.MyHealth.IsDead()) return;
            }
            hub.MyStats.GetRuntimeAttributes().ModifyNowResource((ResourceType)resourceType, byAmount);
            receiveEvents.SceneEvents.OnReceiveResourceCurrent.Invoke((ResourceType)resourceType, byAmount);
        }

        public void AuraModifyMaxResource(int resourceType, int byAmount)
        {
            if (hub.MyHealth != null)
            {
                if (hub.MyHealth.IsDead()) return;
            }
            hub.MyStats.GetRuntimeAttributes().ModifyMaxResource((ResourceType)resourceType, byAmount);
            receiveEvents.SceneEvents.OnReceiveResourceAuraMax.Invoke((ResourceType)resourceType, byAmount);

        }

        public void AuraBuffSat(int statType, int byAmount)
        {
            if (hub.MyHealth != null)
            {
                if (hub.MyHealth.IsDead()) return;
            }
            hub.MyStats.GetRuntimeAttributes().ModifyNowStatValue((StatType)statType, byAmount);
            receiveEvents.SceneEvents.OnReceiveStatAura.Invoke((StatType)statType, byAmount);

        }

        public AuraTargetGroup[] GetAuraGroups()
        {
            return mygroups;
        }

        public GameObject GetGameObjectInstance()
        {
            return this.gameObject;
        }

        public void SetActorHub(IActorHub newHub) => hub = newHub;

        public IActorHub GetActorHub() => hub;
 
    }
}