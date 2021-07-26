
using UnityEngine;
using GWLPXL.ARPGCore.Types.com;
using GWLPXL.ARPGCore.Attributes.com;
using System.Collections.Generic;
using GWLPXL.ARPG._Scripts.Attributes.com;
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
            hub.MyStats.GetRuntimeAttributes().ModifyBaseStatValue((StatType)statType, byAmount);
            receiveEvents.SceneEvents.OnReceiveStatAura.Invoke((StatType)statType, byAmount);


        }
        
        // Todo thinking about events with modifiers. Maybe not by amount, just push NowValue
        public void AuraApplyModifierResource(int resourceType, AttributeModifier modifier)
        {
            hub.MyStats.GetRuntimeAttributes().AddModifierResource((ResourceType)resourceType, modifier);
        }

        public void AuraRemoveModifierResource(int resourceType, AttributeModifier modifier)
        {
            hub.MyStats.GetRuntimeAttributes().RemoveModifierResource((ResourceType)resourceType, modifier);
        }

        public void AuraRemoveSourceModifierResource(int resourceType, object source)
        {
            if (hub.MyHealth != null)
            {
                if (hub.MyHealth.IsDead()) return;
            }
            hub.MyStats.GetRuntimeAttributes().RemoveSourceModifierResource((ResourceType)resourceType, source);
        }
        public void AuraApplyModifierStat(int statType, AttributeModifier modifier)
        {
            if (hub.MyHealth != null)
            {
                if (hub.MyHealth.IsDead()) return;
            }
            hub.MyStats.GetRuntimeAttributes().AddModifierStat((StatType)statType, modifier);
        }

        public void AuraRemoveModifierStat(int statType, AttributeModifier modifier)
        {
            if (hub.MyHealth != null)
            {
                if (hub.MyHealth.IsDead()) return;
            }
            hub.MyStats.GetRuntimeAttributes().RemoveModifierStat((StatType)statType, modifier);
        }

        public void AuraRemoveSourceModifierStat(int statType, object source)
        {
            if (hub.MyHealth != null)
            {
                if (hub.MyHealth.IsDead()) return;
            }
            hub.MyStats.GetRuntimeAttributes().RemoveSourceModifierStat((StatType)statType, source);
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