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
        public enum DraggingState
        {
            None = 0,
            SocketHolder = 1,
            SocketItem = 2,
            SocketInsert = 3
        }
        DraggingState state = DraggingState.None;
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


        protected Dictionary<int, GameObject> iteminstancesdic = new Dictionary<int, GameObject>();

        protected Dictionary<Item, ISocketHolderUIElement> uidic = new Dictionary<Item, ISocketHolderUIElement>();

        protected List<GameObject> socketableableUIElements = new List<GameObject>();
        protected List<GameObject> socketUIElements = new List<GameObject>();
        protected IUseSocketSmithCanvas user = null;
        SocketStation station = default;


        Vector3 homePosition;

        List<GameObject> socketInserts = new List<GameObject>();
        Dictionary<RectTransform, ISocketItemUIElementInsert> socketinsertsdic = new Dictionary<RectTransform, ISocketItemUIElementInsert>();


        //socket items.
        Dictionary<int, GameObject> slotPerUIDic = new Dictionary<int, GameObject>();
        Dictionary<GameObject, SocketItemUI> socketitemsdic = new Dictionary<GameObject, SocketItemUI>();
        [System.Serializable]
        public class SocketItemUI
        {
            public ItemStack ItemStack;
            public GameObject UIObject;
            public RectTransform RectTransform;
            public ISocketItemUIElement Interface;
            public SocketItemUI(ItemStack stack, GameObject ob)
            {
                ItemStack = stack;
                UIObject = ob;
                RectTransform = ob.GetComponent<RectTransform>();
                Interface = ob.GetComponent<ISocketItemUIElement>();
            }

           

        }

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
        protected virtual void OnDisable()
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
            DraggingBehavior();

        }

        protected virtual void DraggingBehavior()
        {
            if (GetCanvasEnabled() == false) return;

            switch (state)
            {
                case DraggingState.None:
                    CheckDraggingStartDraggingHolder();
                    CheckDraggingStartDraggingSocketItem();
                    CheckDraggingSlotInserts();
                    break;
                case DraggingState.SocketHolder:
                    DoDraggingSocketHolder();
                    CheckStopDraggingHolder();
                    break;
                case DraggingState.SocketInsert:
                    DoDraggingSocketInsert();
                    CheckStopDraggingItemInsert();
                    break;
                case DraggingState.SocketItem:
                    DoDraggingSocketItem();
                    CheckStopDraggingSocketItem();
                    break;

            }
        }

        protected virtual void CheckDraggingSlotInserts()
        {
            if (socketInserts.Count <= 0) return;//no inserts, no dragging
            if (state != DraggingState.None) return;

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
                        state = DraggingState.SocketInsert;
                        break;


                    }
                }
            }
        }
        protected virtual void CheckStopDraggingItemInsert()
        {

            if (Input.GetButtonUp(InteractButton))
            {

                for (int i = 0; i < socketInserts.Count; i++)
                {
                    //check if we put it back. 
                    RectTransform rectitem = SocketItemPanel.GetComponent<RectTransform>();//this doesn't work, 
                    Vector2 localmouse = rectitem.InverseTransformPoint(Input.mousePosition);
                    ISocketItemUIElementInsert insert = socketInserts[i].GetComponent<ISocketItemUIElementInsert>();


                    if (rectitem.rect.Contains(localmouse))
                    {
                        //add back to inventory.
                        if (insert.GetSocket().SocketedThing == null) continue;

                        bool canAdd = station.Inventory.CanWeAddItem(insert.GetSocket().SocketedThing);
                        if (canAdd)
                        {
                            station.Inventory.AddItemsToInventory(insert.GetSocket().SocketedThing, 1);
                            station.RemoveSocketable(insert.GetHolder() as Equipment, insert.GetIndex(), true);
                            //need to update visuals now. 
                            insert.UpdateSocket();
                            socketHolderDraggable.RefreshSockets();//also need to refresh the thing in the item panel...
                            uidic[insert.GetHolder()].RefreshSockets();//doesn't seem to work...
                            previewholder.SetSockets(previewholder.GetSocketHolder());
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
                        bool swap = station.SwapSocketItem(insert.GetHolder() as Equipment, insert.GetIndex(), socketInsert.GetIndex());
                        if (swap)
                        {
                            insert.UpdateSocket();
                            socketHolderDraggable.RefreshSockets();//also need to refresh the thing in the item panel...
                            uidic[insert.GetHolder()].RefreshSockets();//doesn't seem to work...
                            previewholder.SetSockets(previewholder.GetSocketHolder());
                            socketInsert.UpdateSocket();
                            Debug.Log("Swap");
                            break;
                        }
               
                    }

                }

              

                SocketItemInsertDraggableInstnace.position = homePosition;
                state = DraggingState.None;

            }
            }
        protected virtual void DoDraggingSocketInsert()
        {
            SocketItemInsertDraggableInstnace.position = Input.mousePosition;
            
        }
        protected virtual void DoDraggingSocketItem()
        {
            SocketItemDraggableInstance.position = Input.mousePosition;
            
        }
        protected virtual void CheckStopDraggingSocketItem()
        {


            if (Input.GetButtonUp(InteractButton))
            {
                SocketItem socketitem = socketItemDraggable.GetSocketItem().Item as SocketItem;
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
                                bool added = station.AddSocketable(eq, socketitem, index, true);//doesn't remove from inventory...
                                socketItemDraggable.UpdateItem();
                                socketinsert.UpdateSocket();
                                previewholder.SetSockets(eq);
                                break;
                            }
                        }
           
                        
                       
                    }
                    

                }
                SocketItemDraggableInstance.position = homePosition;
                state = DraggingState.None;

            }

        }

        protected virtual void CheckDraggingStartDraggingSocketItem()
        {

            if (state != DraggingState.None) return;

            foreach (var kvp in socketitemsdic)
            {
                Vector2 localMousePos = kvp.Value.RectTransform.InverseTransformPoint(Input.mousePosition);
                if (kvp.Value.RectTransform.rect.Contains(localMousePos))
                {
                    if (Input.GetButtonDown(InteractButton))
                    {
                        ItemStack stack = kvp.Value.Interface.GetSocketItem();
                        socketItemDraggable.SetSocketItem(stack.SlotID, station.Inventory);
                        SceneEvents.OnStartDragSocketItem?.Invoke();
                        state = DraggingState.SocketItem;
                        break;
                    }
                }
            }


        }
        protected virtual void DoDraggingSocketHolder()
        {
            SocketHolderDraggableInstance.position = Input.mousePosition;
           
        }
        protected virtual void CheckStopDraggingHolder()
        {


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


                state = DraggingState.None;
                Debug.Log("Drag Reset");
            }
        }
       
        protected virtual void CheckDraggingStartDraggingHolder()
        {

            if (state != DraggingState.None) return;

            foreach (var kvp in sockerHolderRectsDic)
            {
                Vector2 localMousePos = kvp.Key.InverseTransformPoint(Input.mousePosition);
                if (kvp.Key.rect.Contains(localMousePos))
                {
                    if (Input.GetButtonDown(InteractButton))
                    {
                        if (kvp.Value.GetSocketHolder() == null) continue;
                        Item item = kvp.Value.GetSocketHolder();
                        socketHolderDraggable.SetSockets(item as Equipment);
                        SceneEvents.OnStartDragSocketHolder?.Invoke();
                        state = DraggingState.SocketHolder;
                        break;

                    }
                }
            }


        }

       

        
        

        protected virtual void ShutDown()
        {
            if (station != null)
            {
                station.OnSmithClosed?.Invoke();
                station.Inventory.OnSlotChanged -= UpdateUI;
                station.OnAddSocketable -= UpdateEquipmentUI;
            }

            uidic.Clear();
            MainPanel.gameObject.SetActive(false);


            for (int i = 0; i < socketableableUIElements.Count; i++)
            {
                Destroy(socketableableUIElements[i]);
            }
           

            foreach (var kvp in socketitemsdic)
            {
                station.Inventory.OnSlotChange -= kvp.Value.Interface.UpdateItem;
                slotPerUIDic.Remove(kvp.Value.ItemStack.SlotID);
                Destroy(kvp.Key);
            }

            socketUIElements.Clear();
            sockerHolderRectsDic.Clear();


            socketitemsdic.Clear();
            slotPerUIDic.Clear();

          

            station = null;
            user = null;
            if (FreezeDungeon)
            {
                DungeonMaster.Instance.GetDungeonUISceneRef().SetDungeonState(1);
            }
        }
        public void Close()
        {
            ShutDown();
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
        protected virtual void UpdateUI(int slot, ItemStack stack)
        {
            
            if (slotPerUIDic.ContainsKey(slot))
            {
                //update
                GameObject obj = slotPerUIDic[slot];
                if (stack.Item == null || stack.Item is EquipmentSocketable == false || stack.CurrentStackSize <= 0)
                {
                  
                    RemoveSocketItemUI(obj);


                }
                else
                {
                    socketitemsdic[obj].Interface.UpdateItem();
                }



            }
            else
            {
                CreateUISocketElement(stack);
            }
            
        }
        protected virtual void Setup(IUseSocketSmithCanvas user)
        {
           
            this.user = user;
            this.user.SetCanvasSmithCanvas(this);
            MainPanel.gameObject.SetActive(true);

            station.OnAddSocketable += UpdateEquipmentUI;
            station.Inventory.OnSlotChanged += UpdateUI;

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

            List<ItemStack> socketableItems = station.Inventory.GetAllUniqueStacks();
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
            station.Inventory.OnSlotChange += socketitem.UpdateItem;
            socketitem.SetSocketItem(itemStack.SlotID, station.Inventory);
            SocketItemUI newui = new SocketItemUI(itemStack, instance);
            socketitemsdic[instance] = newui;
            slotPerUIDic[itemStack.SlotID] = instance;

        }

        protected virtual void RemoveSocketItemUI(GameObject key)
        {
            if (socketitemsdic.ContainsKey(key))
            {
                //do the removal
                station.Inventory.OnSlotChange -= socketitemsdic[key].Interface.UpdateItem;
                slotPerUIDic.Remove(socketitemsdic[key].ItemStack.SlotID);
                socketitemsdic.Remove(key);

                Destroy(key);
 

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