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
  


    /// <summary>
    /// to do, socket items dragging
    /// </summary>
    public class SocketCanvas_UI : MonoBehaviour, ISocketSmithCanvas
    {
        public SocketUIEvents SceneEvents = new SocketUIEvents();
        public GameObject MainPanel = default;
        public bool FreezeDungeon = true;
        [Header("Interaction")]
        public string InteractButton = "Fire1";
        [Header("Socketable")]
        public GameObject SocketablePrefab = default;
        public Transform SocketableContentParent = default;
        [Header("Sockets")]
        public GameObject SocketItemPrefab = default;
        public Transform SocketContentParent = default;
        [Header("Preview")]
        public Transform SocketablePreviewInstance = default;
        ISocketHolderUIElement previewholder = null;

        [Header("Draggable")]
        public Transform SocketItemDraggableInstance = default;
        ISocketItemUIElement socketItemDraggable = null;
        public Transform SocketHolderDraggableInstance = default;
        ISocketHolderUIElement socketHolderDraggable = null;
        protected Dictionary<RectTransform, ISocketHolderUIElement> sockerHolderRectsDic = new Dictionary<RectTransform, ISocketHolderUIElement>();
        protected Dictionary<RectTransform, ISocketItemUIElement> socketItemsRectsDic = new Dictionary<RectTransform, ISocketItemUIElement>();

        protected Dictionary<Item, ISocketHolderUIElement> uidic = new Dictionary<Item, ISocketHolderUIElement>();
        protected List<GameObject> socketableableUIElements = new List<GameObject>();
        protected List<GameObject> socketUIElements = new List<GameObject>();
        protected IUseSocketSmithCanvas user = null;
        SocketStation station = default;

        bool draggingholder = false;
        bool draggingitem = false;
        Vector3 homePosition;
        protected virtual void Awake()
        {
            previewholder = SocketablePreviewInstance.GetComponent<ISocketHolderUIElement>();
            socketItemDraggable = SocketItemDraggableInstance.GetComponent<ISocketItemUIElement>();
            socketHolderDraggable = SocketHolderDraggableInstance.GetComponent<ISocketHolderUIElement>();
            homePosition = SocketHolderDraggableInstance.position;
        }

        protected virtual void LateUpdate()
        {
            //check mouse pos
            if (GetCanvasEnabled() == false) return;

            CheckDraggingStartDraggingHolder();
            CheckStopDraggingHolder();
            DoDraggingEnchantable();

           

            //if (draggingitem == false)
            //{
            //    CheckDraggingOnEnchant();
            //    CheckStopDraggingEnchant();
            //    DoDraggingEnchant();
            //}
        }

        protected virtual void DoDraggingEnchantable()
        {
            if (draggingholder)
            {
                SocketHolderDraggableInstance.position = Input.mousePosition;
            }
        }
        protected virtual void CheckStopDraggingHolder()
        {
            if (socketHolderDraggable.GetSocketHolder() == null) return;
            if (draggingholder == false) return;
            if (Input.GetButtonUp(InteractButton))
            {
                //maybe we get a center point instead of the entire thing...
                if (SocketablePreviewInstance.GetComponent<RectTransform>().rect.Overlaps(SocketHolderDraggableInstance.GetComponent<RectTransform>().rect))
                {
                    //placed
                    previewholder.SetSockets(socketHolderDraggable.GetSocketHolder());
                    SceneEvents.OnPreviewSetHolder?.Invoke();
                }

                SocketHolderDraggableInstance.position = homePosition;
                draggingholder = false;//also check if trying to place in slot
                Debug.Log("Drag Reset");
            }
        }
        protected virtual void CheckStopDraggingSocketItem()
        {
            if (socketItemDraggable.GetSocketItem() == null) return;
            if (draggingitem == false) return;
            if (previewholder.GetSocketHolder() == null) return;

            if (Input.GetButtonUp(InteractButton))
            {
                //add the socket...
                //station.AddSocketable(previewholder.GetSocketHolder(), socketItemDraggable.GetSocketItem().Item);//need to minus from inventory...
                ////maybe we get a center point instead of the entire thing...
                //if (SocketablePreviewInstance.GetComponent<RectTransform>().rect.Overlaps(SocketHolderDraggableInstance.GetComponent<RectTransform>().rect))
                //{
                //    //placed
                //    previewholder.SetSockets(socketHolderDraggable.GetSocketHolder());
                //    SceneEvents.OnPreviewSetHolder?.Invoke();
                //}

                //SocketHolderDraggableInstance.position = homePosition;
                //draggingholder = false;//also check if trying to place in slot
                //Debug.Log("Drag Reset");
            }
        }
        protected virtual void CheckDraggingStartDraggingHolder()
        {
            if (draggingholder == true) return;

            foreach (var kvp in sockerHolderRectsDic)
            {
                Vector2 localMousePos = kvp.Key.InverseTransformPoint(Input.mousePosition);
                if (kvp.Key.rect.Contains(localMousePos))
                {
                    if (Input.GetButtonDown(InteractButton))
                    {
                        draggingholder = true;
                        TryPlaceInSlot(kvp.Value.GetSocketHolder());
                        break;

                    }
                }
            }


        }

        protected virtual void CheckDraggingStartDraggingSocketItem()
        {
            if (draggingitem == true) return;

            foreach (var kvp in socketItemsRectsDic)
            {
                Vector2 localMousePos = kvp.Key.InverseTransformPoint(Input.mousePosition);
                if (kvp.Key.rect.Contains(localMousePos))
                {
                    if (Input.GetButtonDown(InteractButton))
                    {
                        draggingitem = true;
                        TryPlaceInSlot(kvp.Value.GetSocketItem());
                        break;

                    }
                }
            }


        }

        protected virtual void TryPlaceInSlot(ItemStack item)
        {
            socketItemDraggable.SetSocketItem(item);
            if (item == null) return;
            SceneEvents.OnStartDragSocketItem?.Invoke();
            Debug.Log("Drag Start Success");
        }
        protected virtual void TryPlaceInSlot(Item item)
        {
            socketHolderDraggable.SetSockets(item as Equipment);
            if (item == null) return;
            SceneEvents.OnStartDragSocketHolder?.Invoke();
            Debug.Log("Drag Start Success");
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
            sockerHolderRectsDic.Clear();
            socketItemsRectsDic.Clear();
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

            List<ItemStack> socketableItems = station.GetAllSocketables();
            for (int i = 0; i < socketableItems.Count; i++)//rethink...
            {
                CreateUISocketElement(socketableItems[i]);
            }

            if (FreezeDungeon)
            {
                DungeonMaster.Instance.GetDungeonUISceneRef().SetDungeonState(0);
            }

        }
        protected virtual void CreateUISocketElement(ItemStack itemStack)
        {
            GameObject instance = Instantiate(SocketItemPrefab, SocketContentParent);
            ISocketItemUIElement socketitem = instance.GetComponent<ISocketItemUIElement>();
            socketitem.SetSocketItem(itemStack);
            socketUIElements.Add(instance);
            socketItemsRectsDic[instance.GetComponent<RectTransform>()] = socketitem;
        }
        protected virtual void CreateUISocketableElement(Item item)
        {
            GameObject instance = Instantiate(SocketablePrefab, SocketableContentParent);
            ISocketHolderUIElement element = instance.GetComponent<ISocketHolderUIElement>();
            element.SetSockets(item as Equipment);
            socketableableUIElements.Add(instance);
            uidic[item] = element;

            sockerHolderRectsDic[instance.GetComponent<RectTransform>()] = element;
        }
        protected virtual void UpdateEquipmentUI(Item item)
        {
            if (uidic.ContainsKey(item))
            {
                uidic[item].SetSockets(item as Equipment);
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