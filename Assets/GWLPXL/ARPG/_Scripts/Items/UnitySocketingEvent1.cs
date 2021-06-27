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
        public UnityEquipmentSocketEvent OnEquipmentSocketed;
    }
    [System.Serializable]
    public class UnitySocketSmithEvents
    {
        public UnitySocketEvents SceneEvents = new UnitySocketEvents();
    }

    [System.Serializable]
    public class SocketUIEvents
    {
        public UnityEngine.Events.UnityEvent OnStartDragSocketHolder;
        public UnityEngine.Events.UnityEvent OnStartDragSocketItem;
        public UnityEngine.Events.UnityEvent OnPreviewSetHolder;
        public UnityEngine.Events.UnityEvent OnPreviewSetSocketItem;
        public UnityEquipmentSocketEvent OnSocketSuccess;
    }

}