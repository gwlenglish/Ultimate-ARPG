
using UnityEngine;
using GWLPXL.ARPGCore.Types.com;
using GWLPXL.ARPGCore.Attributes.com;
using System.Collections.Generic;
using GWLPXL.ARPGCore.com;

namespace GWLPXL.ARPGCore.Auras.com
{


    public class EnemyAuraReceiver : MonoBehaviour, ITakeAura
    {
        [SerializeField]
        EnemyAuraReceiveEvents receiveEvents = new EnemyAuraReceiveEvents();
        [SerializeField]
        AuraTargetGroup[] mygroups = new AuraTargetGroup[1] { AuraTargetGroup.Enemies };

        IActorHub hub = null;
        List<Aura> affectedList = new List<Aura>();
 
     
        public void AuraModifyCurrentResource(int resourceType, int byAmount)
        {
            hub.MyStats.GetRuntimeAttributes().ModifyNowResource((ResourceType)resourceType, byAmount);
            receiveEvents.SceneEvents.OnReceiveResourceCurrent.Invoke((ResourceType)resourceType, byAmount);
        }

        public void AuraModifyMaxResource(int resourceType, int byAmount)
        {
            hub.MyStats.GetRuntimeAttributes().ModifyMaxResource((ResourceType)resourceType, byAmount);
            receiveEvents.SceneEvents.OnReceiveResourceAuraMax.Invoke((ResourceType)resourceType, byAmount);

        }
        public void AuraBuffSat(int statType, int byAmount)
        {
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

       

      

        public void SceneCleanUp()
        {
            for (int i = 0; i < affectedList.Count; i++)
            {
                affectedList[i].Remove(this);
            }
        }

        public void SetActorHub(IActorHub newHub)
        {
            hub = newHub;
        }
    }
}