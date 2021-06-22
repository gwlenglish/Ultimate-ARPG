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
    public class Enchanter : MonoBehaviour, IInteract
    {
        public EnchantingStation EnchantingStation = new EnchantingStation();
        public List<EquipmentEnchant> Enchants;
        public UnityEnchanterEvents EnchantEvents;
        public float Range = 3;
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

            EnchantingStation.OnEnchanted += Enchanted;
            EnchantingStation.OnStationSetup += StationReady;
            EnchantingStation.OnStationClosed += StationClosed;
        }

        private void OnDestroy()
        {
            EnchantingStation.OnEnchanted -= Enchanted;
            EnchantingStation.OnStationSetup -= StationReady;
            EnchantingStation.OnStationClosed -= StationClosed;
        }
        #endregion

        #region scene events
        void StationClosed(EnchantingStation station)
        {
            EnchantEvents.SceneEvents.OnStationClosed?.Invoke(station);
            Debug.Log("Station Closed");
        }
        void StationReady(EnchantingStation station)
        {
            EnchantEvents.SceneEvents.OnStationSetup?.Invoke(station);
            Debug.Log("Station Ready");
        }
        void Enchanted(Equipment equipment)
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

            EnchantingStation.SetupStation(interactor.GetComponent<IActorHub>().MyInventory.GetInventoryRuntime(), Enchants);
            if (canvas != null)
            {
                canvas.SetStation(EnchantingStation);
                canvas.Open(user);
            }
            return true;
        }

        public bool IsInRange(GameObject interactor)
        {
            Vector3 dir = interactor.transform.position - this.transform.position;
            float sqrd = dir.sqrMagnitude;
            if (sqrd <= Range * Range)
            {
                return true;
            }
            return false;
        }

       
    }
}