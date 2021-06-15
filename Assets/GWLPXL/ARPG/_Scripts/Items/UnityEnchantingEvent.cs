using UnityEngine.Events;


namespace GWLPXL.ARPGCore.Items.com
{
 

    [System.Serializable]
    public class UnityEnchantingEvent : UnityEvent<EnchantingStation> { }
    [System.Serializable]
    public class UnityEquipmentEnchantedEvent : UnityEvent<Equipment> { }

    [System.Serializable]
    public class UnityEnchantEvents
    {
        public UnityEnchantingEvent OnStationSetup;
        public UnityEnchantingEvent OnStationClosed;
        public UnityEquipmentEnchantedEvent OnEquipmentEnchanted;
    }
    [System.Serializable]
    public class UnityEnchanterEvents
    {
        public UnityEnchantEvents SceneEvents = new UnityEnchantEvents();
    }

}