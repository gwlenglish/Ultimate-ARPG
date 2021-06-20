using UnityEngine.Events;


namespace GWLPXL.ARPGCore.Items.com
{
 

    [System.Serializable]
    public class UnitySocketingEvent : UnityEvent<SocketStation> { }
    [System.Serializable]
    public class UnityEquipmentSocketEvent : UnityEvent<Equipment> { }

    [System.Serializable]
    public class UnitySocketEvents
    {
        public UnitySocketingEvent OnStationSetup;
        public UnitySocketingEvent OnStationClosed;
        public UnityEquipmentSocketEvent OnEquipmentEnchanted;
    }
    [System.Serializable]
    public class UnitySocketerEvents
    {
        public UnitySocketEvents SceneEvents = new UnitySocketEvents();
    }

    [System.Serializable]
    public class SocketUIEvents
    {
        public UnityEngine.Events.UnityEvent OnStartDragEnchantable;
        public UnityEngine.Events.UnityEvent OnStartDragEnchant;
        public UnityEngine.Events.UnityEvent OnPreviewSetEnchantable;
        public UnityEngine.Events.UnityEvent OnPreviewSetEnchant;
        public UnityEquipmentSocketEvent OnSocketSuccess;
    }

}