using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Traits.com;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ARPGCore.Items.com
{
    /// <summary>
    /// Enchanter actor example
    /// </summary>
    /// 
    [System.Serializable]
    public class EnchanterVars
    {
        public List<EquipmentEnchant> Enchants = new List<EquipmentEnchant>();
        public float InteractRange = 3;
        public EnchantingStation EnchantingStation = new EnchantingStation();
    }
    public class Enchanter : MonoBehaviour, IInteract
    {
        public EnchanterVars EnchanterVars = new EnchanterVars();
        public UnityEnchanterEvents EnchantEvents;
        public GameObject EnchanterUIPrefab;
        GameObject uiinstance = null;
        IEnchanterCanvas canvas;

        #region callbacks
        private void Start()
        {
            if (EnchanterUIPrefab != null)
            {
                uiinstance = Instantiate(EnchanterUIPrefab);
                canvas = uiinstance.GetComponent<IEnchanterCanvas>();
            }

            EnchanterVars. EnchantingStation.OnEnchanted += Enchanted;
            EnchanterVars. EnchantingStation.OnStationSetup += StationReady;
            EnchanterVars. EnchantingStation.OnStationClosed += StationClosed;
        }

        private void OnDestroy()
        {
            EnchanterVars.EnchantingStation.OnEnchanted -= Enchanted;
            EnchanterVars.EnchantingStation.OnStationSetup -= StationReady;
            EnchanterVars. EnchantingStation.OnStationClosed -= StationClosed;
        }
        #endregion

        #region scene events
        protected virtual void StationClosed(EnchantingStation station)
        {
            EnchantEvents.SceneEvents.OnStationClosed?.Invoke(station);
            Debug.Log("Station Closed");
        }
        protected virtual void StationReady(EnchantingStation station)
        {
            EnchantEvents.SceneEvents.OnStationSetup?.Invoke(station);
            Debug.Log("Station Ready");
        }
        protected virtual void Enchanted(Equipment equipment)
        {
            EnchantEvents.SceneEvents.OnEquipmentEnchanted?.Invoke(equipment);
            Debug.Log("Item Enchanted " + equipment.GetGeneratedItemName());
        }
        #endregion
        protected virtual IUseEnchanterCanvas CheckPreconditions(GameObject obj)
        {
            IActorHub actor = obj.GetComponent<IActorHub>();
            if (actor == null || actor.PlayerControlled == null || actor.PlayerControlled.CanvasHub.EnchanterCanvas == null)
            {
                ARPGCore.DebugHelpers.com.ARPGDebugger.DebugMessage("Can't enchant without an inventory", this);
                return null;
            }
            return actor.PlayerControlled.CanvasHub.EnchanterCanvas;
        }
        public bool DoInteraction(GameObject interactor)
        {
            IUseEnchanterCanvas user = CheckPreconditions(interactor);
            if (user == null) return false;

            EnchanterVars.EnchantingStation.SetupStation(interactor.GetComponent<IActorHub>().MyInventory.GetInventoryRuntime(), EnchanterVars.Enchants);
            if (canvas != null)
            {
                canvas.SetStation(EnchanterVars.EnchantingStation);
                canvas.Open(user);
            }
            return true;
        }

        public bool IsInRange(GameObject interactor)
        {
            Vector3 dir = interactor.transform.position - this.transform.position;
            float sqrd = dir.sqrMagnitude;
            if (sqrd <= EnchanterVars.InteractRange * EnchanterVars.InteractRange)
            {
                return true;
            }
            return false;
        }

       
    }
}