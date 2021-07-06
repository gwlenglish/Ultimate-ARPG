using UnityEngine.Events;
using UnityEngine;

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
        [Header("Canvas")]
        public UnityEngine.Events.UnityEvent OnOpen;
        public UnityEngine.Events.UnityEvent OnClose;
        [Header("Socket Holders")]
        public UnityEngine.Events.UnityEvent OnStartDragSocketHolder;
        public UnityEngine.Events.UnityEvent OnStopDragSocketHolder;
        [Header("Socket Items")]
        public UnityEngine.Events.UnityEvent OnStartDragSocketItem;
        public UnityEngine.Events.UnityEvent OnStopDragSocketItem;
        [Header("Socket Inserts")]
        public UnityEngine.Events.UnityEvent OnStartDragSocketInsert;
        public UnityEngine.Events.UnityEvent OnSocketInsertSuccess;
        public UnityEngine.Events.UnityEvent OnSocketInsertReturnedToInventory;
        public UnityEngine.Events.UnityEvent OnSocketInsertMoveFail;
        [Header("Preview")]
        public UnityEngine.Events.UnityEvent OnPreviewSetHolder;
        public UnityEngine.Events.UnityEvent OnPreviewSetSocketItem;
        public UnityEquipmentSocketEvent OnSocketSuccess;
    }

}