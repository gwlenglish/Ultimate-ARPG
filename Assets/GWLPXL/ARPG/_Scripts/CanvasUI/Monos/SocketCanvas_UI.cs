using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Items.com;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ARPGCore.CanvasUI.com
{
    public interface ISocketSmithCanvas
    {
        void Open(IUseSocketSmithCanvas user);
        void Close();
        void Toggle();
        void SetStation(SocketStation station);
        bool GetCanvasEnabled();
        bool GetFreezeMover();
    }
    public interface IUseSocketSmithCanvas
    {
        bool GetFreezeMover();
        ISocketSmithCanvas GetSocketSmithCanvas();
        void SetCanvasSmithCanvas(ISocketSmithCanvas socketsmithcanvas);
    }
    public interface ISocketItemUIElement
    {
        void SetEnchant(SocketItem item);
        SocketItem GetEnchant();
    }
    public interface ISocketableUIElement
    {
        void SetSocketable(Item item);
        Item GetSocketable();
    }
    /// <summary>
    /// TO do, prefabs and layout
    /// TO DO, dragging
    /// </summary>
    public class SocketCanvas_UI : MonoBehaviour, ISocketSmithCanvas
    {
        public SocketUIEvents SceneEvents = new SocketUIEvents();
        public GameObject MainPanel = default;
        public bool FreezeDungeon = true;
        [Header("Socketable")]
        public GameObject SocketablePrefab = default;
        public Transform SocketableContentParent = default;
        [Header("Sockets")]
        public GameObject SocketItemPrefab = default;
        public Transform SocketContentParent = default;
        [Header("Preview")]
        public Transform SocketablePreviewInstance = default;
        ISocketHolderUIElement previewholder = null;

        protected Dictionary<Item, ISocketableUIElement> uidic = new Dictionary<Item, ISocketableUIElement>();
        protected List<GameObject> socketableableUIElements = new List<GameObject>();
        protected List<GameObject> socketUIElements = new List<GameObject>();
        protected IUseSocketSmithCanvas user = null;
        SocketStation station = default;

        protected virtual void Awake()
        {
            previewholder = SocketablePreviewInstance.GetComponent<ISocketHolderUIElement>();
        }
        public void Close()
        {
            uidic.Clear();
            MainPanel.gameObject.SetActive(false);
            if (station != null)
            {
                station.OnAddSocketable -= UpdateEquipmentUI;

            }

            for (int i = 0; i < socketableableUIElements.Count; i++)
            {
                Destroy(socketableableUIElements[i]);
            }
            for (int i = 0; i < socketUIElements.Count; i++)
            {
                Destroy(socketUIElements[i]);
            }

            socketUIElements.Clear();
            socketUIElements.Clear();
            station = null;

            if (FreezeDungeon)
            {
                DungeonMaster.Instance.GetDungeonUISceneRef().SetDungeonState(1);
            }
        }

        public bool GetCanvasEnabled()
        {
            return MainPanel.activeInHierarchy;
        }

        public bool GetFreezeMover()
        {
            return FreezeDungeon;
        }

        public void Open(IUseSocketSmithCanvas user)
        {
            Setup(user);
        }
        protected virtual void Setup(IUseSocketSmithCanvas user)
        {
            this.user = user;
            this.user.SetCanvasSmithCanvas(this);
            MainPanel.gameObject.SetActive(true);
            station.OnAddSocketable += UpdateEquipmentUI;

            List<Equipment> equipped = station.GetEquippedEquipmentWithSockets();
            for (int i = 0; i < equipped.Count; i++)
            {
                Item item = equipped[i];
                CreateUISocketableElement(item);
            }

            List<Equipment> inventory = station.GetEquipmentInInventoryWithSockets();
            for (int i = 0; i < inventory.Count; i++)
            {
                CreateUISocketableElement(inventory[i]);
            }

            List<SocketItem> socketableItems = station.GetAllSocketables();
            for (int i = 0; i < socketableItems.Count; i++)//rethink...
            {
                CreateUISocketElement(socketableItems[i]);
            }

            if (FreezeDungeon)
            {
                DungeonMaster.Instance.GetDungeonUISceneRef().SetDungeonState(0);
            }

        }
        protected virtual void CreateUISocketElement(SocketItem socket)
        {
            GameObject instance = Instantiate(SocketItemPrefab, SocketContentParent);
            instance.GetComponent<ISocketItemUIElement>().SetEnchant(socket);
            socketUIElements.Add(instance);
        }
        protected virtual void CreateUISocketableElement(Item item)
        {
            GameObject instance = Instantiate(SocketablePrefab, SocketableContentParent);
            ISocketableUIElement element = instance.GetComponent<ISocketableUIElement>();
            element.SetSocketable(item);
            socketableableUIElements.Add(instance);
            uidic[item] = element;
        }
        protected virtual void UpdateEquipmentUI(Item item)
        {
            if (uidic.ContainsKey(item))
            {
                uidic[item].SetSocketable(item);
            }
        }
        public void SetStation(SocketStation station)
        {
            this.station = station;
        }

        public void Toggle()
        {
            if (MainPanel.activeInHierarchy)
            {
                Close();
            }
            else
            {
                Open(user);
            }
        }
    }
}