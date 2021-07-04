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
    /// to do, inserts dragging
    /// to do, replace preview items (need to delete its socket items)
    /// </summary>
    public class SocketCanvas_UI : MonoBehaviour, ISocketSmithCanvas
    {

        public SocketStation Station => station;

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
        public Transform SocketItemPanel = default;
[Header("Preview")]
        public Transform SocketablePreviewInstance = default;
        ISocketHolderUIElement previewholder = null;

        [Header("Draggable")]
        public Transform SocketItemDraggableInstance = default;
        ISocketItemUIElement socketItemDraggable = null;
        public Transform SocketHolderDraggableInstance = default;
        ISocketHolderUIElement socketHolderDraggable = null;
        public Transform SocketItemInsertDraggableInstnace = default;
        ISocketItemUIElementInsert socketInsert = null;
        protected Dictionary<RectTransform, ISocketHolderUIElement> sockerHolderRectsDic = new Dictionary<RectTransform, ISocketHolderUIElement>();
        protected Dictionary<RectTransform, ISocketItemUIElement> socketItemsRectsDic = new Dictionary<RectTransform, ISocketItemUIElement>();
        

        protected Dictionary<Item, ISocketHolderUIElement> uidic = new Dictionary<Item, ISocketHolderUIElement>();
        protected List<GameObject> socketableableUIElements = new List<GameObject>();
        protected List<GameObject> socketUIElements = new List<GameObject>();
        protected IUseSocketSmithCanvas user = null;
        SocketStation station = default;

        bool draggingholder = false;
        bool draggingitem = false;
        bool draggingitemfromsocket = false;
        Vector3 homePosition;

        List<GameObject> socketInserts = new List<GameObject>();
        Dictionary<RectTransform, ISocketItemUIElementInsert> socketinsertsdic = new Dictionary<RectTransform, ISocketItemUIElementInsert>();
      
        protected virtual void Awake()
        {
            socketInsert = SocketItemInsertDraggableInstnace.GetComponent<ISocketItemUIElementInsert>();
            previewholder = SocketablePreviewInstance.GetComponent<ISocketHolderUIElement>();
            socketItemDraggable = SocketItemDraggableInstance.GetComponent<ISocketItemUIElement>();
            socketHolderDraggable = SocketHolderDraggableInstance.GetComponent<ISocketHolderUIElement>();
            homePosition = SocketHolderDraggableInstance.position;
        }
        private void OnEnable()
        {
            SocketHolderUIElement uielem = SocketablePreviewInstance.GetComponent<SocketHolderUIElement>();
            if (uielem != null)
            {
                uielem.OnInsertablesCreated += SocketInserts;
            }


        }

        void SocketInserts(List<GameObject> inserts)
        {
            socketInserts = inserts;
            socketinsertsdic.Clear();
            for (int i = 0; i < socketInserts.Count; i++)
            {
                socketinsertsdic[socketInserts[i].GetComponent<RectTransform>()] = socketInserts[i].GetComponent<ISocketItemUIElementInsert>();
            }
        }
        private void Disable()
        {
            SocketHolderUIElement uielem = SocketablePreviewInstance.GetComponent<SocketHolderUIElement>();
            if (uielem != null)
            {
                uielem.OnInsertablesCreated -= SocketInserts;
            }
        }
        protected virtual void LateUpdate()
        {
            //check mouse pos
            if (GetCanvasEnabled() == false) return;

            CheckDraggingStartDraggingHolder();
            CheckStopDraggingHolder();
            DoDraggingSocketHolder();

            //rethink how we can also add the inserts to the dictionary, so we can go the other way.
            CheckDraggingStartDraggingSocketItem();
            CheckStopDraggingSocketItem();
            DoDraggingSocketItem();

            //check dragging from slot inserts

            CheckDraggingSlotInserts();
            CheckStopDraggingItemInsert();
            DoDraggingSocketInsert();

        }

        protected virtual void CheckDraggingSlotInserts()
        {
            if (socketInserts.Count <= 0) return;//no inserts, no dragging
            if (draggingitemfromsocket) return;
            
            foreach (var kvp in socketinsertsdic)
            {
           
                Vector2 localMousePos = kvp.Key.InverseTransformPoint(Input.mousePosition);
                if (kvp.Key.rect.Contains(localMousePos))
                {
                    if (Input.GetButtonDown(InteractButton))
                    {
                        if (kvp.Value.GetSocket().SocketedThing == null) continue;//dont add a null one...

                        socketInsert.SetIndex(kvp.Value.GetIndex());
                        socketInsert.SetSocket(kvp.Value.GetSocket(), kvp.Value.GetHolder());
                        Debug.Log("Drag Start Success");
                        draggingitemfromsocket = true;
                        break;


                    }
                }
            }
        }
        protected virtual void CheckStopDraggingItemInsert()
        {
            if (draggingitemfromsocket == false) return;


            if (Input.GetButtonUp(InteractButton))
            {

                for (int i = 0; i < socketInserts.Count; i++)
                {
                    //check if we put it back. 
                    RectTransform rectitem = SocketItemPanel.GetComponent<RectTransform>();//this doesn't work, 
                    Vector2 localmouse = rectitem.InverseTransformPoint(Input.mousePosition);
                    if (rectitem.rect.Contains(localmouse))
                    {
                        //add back to inventory.
                        ISocketItemUIElementInsert insert = socketInserts[i].GetComponent<ISocketItemUIElementInsert>();
                        bool canAdd = station.Inventory.CanWeAddItem(insert.GetSocket().SocketedThing);
                        if (canAdd)
                        {
                            station.Inventory.AddItemsToInventory(insert.GetSocket().SocketedThing, 1);
                            station.RemoveSocketable(insert.GetHolder() as Equipment, insert.GetIndex());
                            //need to update visuals now. 
                            insert.UpdateSocket();
                            //needs to create its own ui element or add to existing...
                            Debug.Log("Return to Inventory");
                        }
                        else
                        {
                            //reset
                            Debug.Log("Return to Insert");
                        }
                        break;
                    }

                    GameObject obj = socketInserts[i];
                    RectTransform recT = obj.GetComponent<RectTransform>();
                    Rect rect = recT.rect;
                    Vector2 localMousePos = recT.InverseTransformPoint(Input.mousePosition);
                    if (rect.Contains(localMousePos))
                    {
                        //swap them.

                        Debug.Log("Swap");
                        break;
                    }

                }

              

                SocketItemInsertDraggableInstnace.position = homePosition;
                draggingitemfromsocket = false;

            }
            }
        protected virtual void DoDraggingSocketInsert()
        {
            if (draggingitemfromsocket)
            {
                SocketItemInsertDraggableInstnace.position = Input.mousePosition;
            }
        }
        protected virtual void DoDraggingSocketItem()
        {
            if (draggingitem)
            {
                SocketItemDraggableInstance.position = Input.mousePosition;
            }
        }
        protected virtual void CheckStopDraggingSocketItem()
        {
          //  if (socketItemDraggable.GetSocketItem() == null) return;
            if (draggingitem == false) return;
            if (previewholder.GetSocketHolder() == null) return;
            SocketItem socketitem = socketItemDraggable.GetSocketItem().Item as SocketItem;
            if (socketitem == null) return;

            if (Input.GetButtonUp(InteractButton))
            {
                //check, are we on a socket insert?
                //if so, we need to switch socket things around. 
                for (int i = 0; i < socketInserts.Count; i++)
                {
                    GameObject obj = socketInserts[i];
                    RectTransform recT = obj.GetComponent<RectTransform>();
                    Rect rect = recT.rect;
                    Vector2 localMousePos = recT.InverseTransformPoint(Input.mousePosition);
                    if (rect.Contains(localMousePos))
                    {
                        ISocketItemUIElementInsert socketinsert = obj.GetComponent<ISocketItemUIElementInsert>();
                        Debug.Log(socketinsert);
                        Equipment eq = previewholder.GetSocketHolder();
                        Debug.Log(eq);
                        int index = socketinsert.GetIndex();
                        Debug.Log("Index " + index);
  
                        if (station.CanAdd(eq, index, socketitem))
                        {
                            bool removefrominventory = station.Inventory.RemoveItemFromSlot(socketitem, socketItemDraggable.GetSocketItem().SlotID);
                            if (removefrominventory)
                            {
                                bool added = station.AddSocketable(eq, socketitem, index);//doesn't remove from inventory...
                                socketItemDraggable.UpdateItem();
                                socketinsert.UpdateSocket();
                                previewholder.SetSockets(eq);
                                break;
                            }
                        }
           
                        
                       
                    }
                    

                }
                SocketItemDraggableInstance.position = homePosition;
                draggingitem = false;

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

                        bool placed = TryPlaceInSlot(kvp.Value.GetSocketItem());
                       if (placed)
                        {
                            break;
                        }
    

                    }
                }
            }


        }
        protected virtual void DoDraggingSocketHolder()
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
                        bool placed = TryPlaceInSlot(kvp.Value.GetSocketHolder());
                        if (placed)
                        {
                            break;
                        }
              

                    }
                }
            }


        }

       

        protected virtual bool TryPlaceInSlot(ItemStack item)
        {
            socketItemDraggable.SetSocketItem(item.SlotID, station.Inventory, RemoveSocketItemUI);
            if (item == null) return draggingitem;
            SceneEvents.OnStartDragSocketItem?.Invoke();
            Debug.Log("Drag Start Success");
            draggingitem = true;
            return draggingitem;
        }
        protected virtual bool TryPlaceInSlot(Item item)
        {
            socketHolderDraggable.SetSockets(item as Equipment);
            if (item == null) return draggingholder;
            SceneEvents.OnStartDragSocketHolder?.Invoke();
            Debug.Log("Drag Start Success");
            draggingholder = true;
            return draggingholder;
        }

        public void Close()
        {
            if (station != null)
            {
                station.OnSmithClosed?.Invoke();
            }

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
            station.OnSmithOpen?.Invoke();
        }

        
        protected virtual void CreateUISocketElement(ItemStack itemStack)
        {

            GameObject instance = Instantiate(SocketItemPrefab, SocketContentParent);
            ISocketItemUIElement socketitem = instance.GetComponent<ISocketItemUIElement>();
            socketitem.SetSocketItem(itemStack.SlotID, station.Inventory, RemoveSocketItemUI);
            socketUIElements.Add(instance);
            socketItemsRectsDic[instance.GetComponent<RectTransform>()] = socketitem;

            station.Inventory.OnSlotChange += socketitem.UpdateItem;


        }

        protected virtual void RemoveSocketItemUI(GameObject key)
        {
            RectTransform t = key.GetComponent<RectTransform>();
            if (socketItemsRectsDic.ContainsKey(t))
            {
                socketItemsRectsDic[t] = null;
                socketItemsRectsDic.Remove(t);
                Destroy(t.gameObject);
            }
            
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