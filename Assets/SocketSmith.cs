using GWLPXL.ARPGCore.CanvasUI.com;
using GWLPXL.ARPGCore.com;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ARPGCore.Items.com
{
    [System.Serializable]
    public class SocketSmithVars
    {
        public SocketStation Station = new SocketStation();
        public float InteractRange = 3;
        public List<SocketItem> SocketItems = new List<SocketItem>();

        public SocketSmithVars(float interactrange, List<SocketItem> socketitems)
        {
            Station = new SocketStation();
            InteractRange = interactrange;
            SocketItems = socketitems;
        }

    }

    public class SocketSmith : MonoBehaviour, IInteract
    {
        public SocketSmithVars SocketSmithVars;
        public UnitySocketSmithEvents SocketEvents;
        public GameObject SocketSmithUIPrefab = default;
        GameObject uiinstance = null;
        ISocketSmithCanvas canvas;

        private void Start()
        {
            if (SocketSmithUIPrefab != null)
            {
                uiinstance = Instantiate(SocketSmithUIPrefab);
                canvas = uiinstance.GetComponent<ISocketSmithCanvas>();
            }

            SocketSmithVars.Station.OnAddSocketable += SocketAdded;
            SocketSmithVars.Station.OnStationSetup += StationReady;
            SocketSmithVars.Station.OnStationClosed += StationClosed;
        }

        protected virtual void StationClosed(SocketStation station)
        {
            SocketEvents.SceneEvents.OnStationClosed?.Invoke(station);
            Debug.Log("Station Closed");
        }
        protected virtual void StationReady(SocketStation station)
        {
            SocketEvents.SceneEvents.OnStationSetup?.Invoke(station);
            Debug.Log("Station Ready");
        }
        protected virtual  void SocketAdded(Equipment equipment)
        {
            SocketEvents.SceneEvents.OnEquipmentSocketed?.Invoke(equipment);
            Debug.Log("Item Socketable Added " + equipment.GetGeneratedItemName());
        }
        protected virtual IUseSocketSmithCanvas CheckPreconditions(GameObject obj)
        {
            IActorHub actor = obj.GetComponent<IActorHub>();
            if (actor == null || actor.PlayerControlled == null || actor.PlayerControlled.CanvasHub.EnchanterCanvas == null)
            {
                ARPGCore.DebugHelpers.com.ARPGDebugger.DebugMessage("Can't enchant without an inventory", this);
                return null;
            }
            return actor.PlayerControlled.CanvasHub.SocketCanvas;
        }
        public bool DoInteraction(GameObject interactor)
        {
            IUseSocketSmithCanvas hub = CheckPreconditions(interactor);
            if (hub == null) return false;

            SocketSmithVars.Station.SetupStation(interactor.GetComponent<IActorHub>().MyInventory.GetInventoryRuntime(), SocketSmithVars.SocketItems);
            if (canvas != null)
            {
                canvas.SetStation(SocketSmithVars.Station);
                canvas.Open(hub);
            }
            return true;
        }

        public bool IsInRange(GameObject interactor)
        {
            Vector3 dir = interactor.transform.position - this.transform.position;
            float sqrdmag = dir.sqrMagnitude;
            if (sqrdmag <= SocketSmithVars.InteractRange * SocketSmithVars.InteractRange)
            {
                return true;
            }
            return false;
        }

       
    }
}