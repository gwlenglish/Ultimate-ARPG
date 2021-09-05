using GWLPXL.ARPGCore.Attributes.com;
using GWLPXL.ARPGCore.CanvasUI.com;
using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.DebugHelpers.com;
using GWLPXL.ARPGCore.Items.com;
using GWLPXL.ARPGCore.Statics.com;
using GWLPXL.ARPGCore.Types.com;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace GWLPXL.ARPGCore.CanvasUI.com
{


    /// <summary>
    /// ui example using Board class to generate the inventory grid/board
    /// </summary>
    public class GridInventory_UI : MonoBehaviour, IInventoryCanvas
    {
        public GridInventoryEvents Events;
        public PatternHolder DefaultPattern;
        public System.Action<List<RaycastResult>> OnTryRemove;
        public System.Action<List<RaycastResult>> ONTryHighlight;
        public System.Action<IInventoryPiece> OnStartDraggingPiece;
        public System.Action OnStopDragging;
        public System.Action<List<RaycastResult>, IInventoryPiece> OnTryPlace;

        public enum InteractState
        {
            Empty = 0,//no dragging
            HasDraggable = 1,//dragging piece not yet placed on the board
            HasInventoryPiece = 2//dragging piece that was placed on the board
        }

        public bool FreezeDungeon = true;

        [Tooltip("Responsible for the inventory")]
        public Board TheBoard;
        [Tooltip("Responsible for the equipment")]
        public EquippedGear_UI GearUI;
        [Header("Inventory UI")]
        public Transform MainPanel;//on ui
        public RectTransform GridTransform;//on ui
        public GameObject CellPrefab;//on ui
        public Transform PanelGrid;//on ui
        [SerializeField]
        protected Color neutral = Color.white;
        [SerializeField]
        protected Color transparent = new Color(255, 255, 255, 0);
        [SerializeField]
        protected Color valid = Color.green;
        [SerializeField]
        protected Color invalid = Color.red;

        protected EventSystem eventSystem;
        protected InteractState state = InteractState.Empty;
        protected GraphicRaycaster m_Raycaster;
        protected PointerEventData m_PointerEventData;
        protected IInventoryPiece draginstance = null;
        protected IInventoryPiece inventorydragging = null;
        protected List<RaycastResult> results = new List<RaycastResult>();
        protected Camera main;
        protected Canvas canvas;
        protected bool showCellCoordinate = false;


        #region ARPG interface fields
        protected IActorHub user = null;
        protected IDescribePlayerStats describeStats = null;
        protected IDescribeEquipment describeEquipment = null;
        protected bool hasStateDisplay = false;
        protected bool hasHoverOverPanel = false;
        protected ActorInventory inv = null;
        protected Dictionary<IInventoryPiece, ItemStack> piecestackdic = new Dictionary<IInventoryPiece, ItemStack>();
        protected Dictionary<GameObject, IInventoryPiece> piecesdic = new Dictionary<GameObject, IInventoryPiece>();
        protected Dictionary<ItemStack, GameObject> stackdic = new Dictionary<ItemStack, GameObject>();
        #endregion

        #region virtual unity calls
        /// <summary>
        /// initialization
        /// </summary>
        protected virtual void Awake()
        {
            eventSystem = FindObjectOfType<EventSystem>();
            m_Raycaster = GetComponent<GraphicRaycaster>();
            m_PointerEventData = new PointerEventData(eventSystem);
            main = Camera.main;
            canvas = GetComponent<Canvas>();
            canvas.worldCamera = main;

        }
        /// <summary>
        /// sub events
        /// </summary>
        protected virtual void OnEnable()
        {
            TheBoard.OnBoardSlotCreated += CreateBoardSlotInstance;
            TheBoard.OnPieceRemoved += Carried;
            TheBoard.OnNewPiecePlaced += Placed;
            TheBoard.OnNewPiecePlaced += ResetColors;

            TheBoard.OnPieceSwapped += SwappedInventory;

            GearUI.Events.OnEquipmentHighlighted += DescribeItem;
        }

       

        /// <summary>
        /// unsub events
        /// </summary>
        protected virtual void OnDisable()
        {
            TheBoard.OnBoardSlotCreated -= CreateBoardSlotInstance;
            TheBoard.OnPieceSwapped -= SwappedInventory;
            TheBoard.OnPieceRemoved -= Carried;

  
            TheBoard.OnNewPiecePlaced -= Placed;
            TheBoard.OnNewPiecePlaced -= ResetColors;

            GearUI.Events.OnEquipmentHighlighted -= DescribeItem;
        }
        /// <summary>
        /// creation
        /// </summary>
     

        /// <summary>
        /// mouse navigation
        /// </summary>
        protected virtual void LateUpdate()
        {
            if (MainPanel.gameObject.activeInHierarchy == false) return;
            //need to set the pointer data.
            m_PointerEventData.position = Input.mousePosition;

            switch (state)
            {
                case InteractState.Empty:
                    if (RemoveTrigger())
                    {
                        TryRemoveDraggableFromGrid();//click mousedown
                    }
                    //detect hover over displays.
                    if (hasHoverOverPanel)
                    {
                        TryHighlight();
                    }
       
                    break;
                case InteractState.HasDraggable:
                    MoveDraggable(draginstance);
                    PreviewHighlight(draginstance);
                    TryPlaceDraggable(draginstance);
                    break;
                case InteractState.HasInventoryPiece:
                    MoveDraggable(inventorydragging);
                    PreviewHighlight(inventorydragging);
                    TryPlaceDraggable(inventorydragging);
                    break;
            }




        }
        #endregion

        #region public virtual
        public virtual void CreateGrid()
        {
            TheBoard.CreateGrid();

            //just resets the color
            foreach (var kvp in TheBoard.IDS)
            {
                kvp.Key.GetComponent<Image>().color = transparent;
            }
        }
        /// <summary>
        /// clear state
        /// </summary>
        public virtual void NoPieces()
        {

            draginstance = null;
            inventorydragging = null;
            state = InteractState.Empty;
           
            OnStopDragging?.Invoke();
        }
        /// <summary>
        /// create draggable piece that isn't already on the grid
        /// </summary>
        /// <param name="pattern"></param>
        public virtual void CreateNewInventoryPiece(Item pattern, int[] equipID)
        {

            draginstance = CreatePiece(pattern);
            state = InteractState.HasDraggable;
        }

    
        public virtual IInventoryPiece CreatePiece(Item itemstack)
        {
            if (itemstack.UIPattern == null)
            {
                itemstack.UIPattern = DefaultPattern;
            }
            InventoryPiece piece  = new InventoryPiece(Instantiate(itemstack.UIPattern.Pattern.PatternPrefab, MainPanel), Instantiate(itemstack.UIPattern.Pattern.PatternPrefab, MainPanel), itemstack);
            piece.Instance.name = itemstack.GetGeneratedItemName() + " Instance";
            piece.PreviewInstance.name = itemstack.GetGeneratedItemName() + " Preview";
            piece.PreviewInstance.transform.position = piece.Instance.transform.position;
            piece.Instance.GetComponentInChildren<Image>().sprite = itemstack.GetSprite();
            piece.PreviewInstance.GetComponentInChildren<Image>().sprite = itemstack.GetSprite();
            piece.Instance.SetActive(true);
            piece.PreviewInstance.SetActive(false);
            return piece;
        }

        /// <summary>
        /// remove draggable piece that is already on the grid
        /// </summary>
        /// <param name="piece"></param>
        public virtual void CarryRemovedPiece(IInventoryPiece piece)
        {

            if (piece == null || piece.Instance == null) NoPieces();
            
            inventorydragging = piece;
            state = InteractState.HasInventoryPiece;
            OnStartDraggingPiece?.Invoke(piece);
    

        }

      
        #endregion

        #region protected virtual
        protected virtual void Carried(IInventoryPiece piece, List<IBoardSlot> slots)
        {

            CarryRemovedPiece(piece);
        }

        protected virtual void ResetColors(IInventoryPiece piece, List<IBoardSlot> slots)
        {
            piece.Instance.GetComponentInChildren<Image>().color = neutral;
            piece.PreviewInstance.GetComponentInChildren<Image>().color =  neutral;
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].Instance.GetComponentInChildren<Image>().color = transparent;
            }
        }
        protected virtual void Placed(IInventoryPiece piece, List<IBoardSlot> slots)
        {
            NoPieces();
        }
        protected virtual void SwappedEquipmentPiece(IInventoryPiece old, IInventoryPiece newpiece)
        {

            CarryRemovedPiece(old);
        }
        protected virtual void EquippedPiece(IInventoryPiece piece)
        {
      
            NoPieces();

        }
        protected virtual void SwappedInventory(IInventoryPiece old, IInventoryPiece newpiece)
        {
            CarryRemovedPiece(old);
        }
        /// <summary>
        /// creates the gameobjects that represent the grid slots
        /// </summary>
        /// <param name="slot"></param>
        protected virtual void CreateBoardSlotInstance(IBoardSlot slot)
        {
            if (slot.Instance != null)
            {
                Destroy(slot.Instance);
            }
            GameObject instance = UnityEngine.GameObject.Instantiate(CellPrefab, PanelGrid);
            instance.name = "Cell " + "X:" + slot.Cell.Cell.x + " Y:" + slot.Cell.Cell.y;
            TMPro.TextMeshProUGUI text = instance.GetComponentInChildren<TMPro.TextMeshProUGUI>();

            if (text != null)
            {
                if (showCellCoordinate)
                {
                    text.SetText((slot.Cell.Cell.x + " , " + slot.Cell.Cell.y));
                }
                else
                {
                    text.SetText("");
                }
            }
           
            TheBoard.IDS.Add(instance, slot.Cell.Cell);
            TheBoard.Preview.CellDictionary[slot] = instance;
            slot.Instance = instance;
        }

        /// <summary>
        /// override to detect custom screen position
        /// </summary>
        /// <returns></returns>
        protected virtual Vector3 GetScreenPosition()
        {
            return Input.mousePosition;
        }

        /// <summary>
        /// draggable movement
        /// </summary>
        /// <param name="dragginginstance"></param>
        protected virtual void MoveDraggable(IInventoryPiece dragginginstance)
        {

            Vector3 screenPoint = GetScreenPosition();
            screenPoint.z = canvas.planeDistance;
            dragginginstance.Instance.transform.position = main.ScreenToWorldPoint(screenPoint);// + new Vector3(xoffset, 0)) ;
        }


        /// <summary>
        /// try place in grid slot
        /// </summary>
        /// <param name="piece"></param>
        protected virtual void TryPlaceDraggable(IInventoryPiece piece)
        {
            if (Input.GetMouseButtonDown(0) == false) return;
            if (m_PointerEventData == null) return;//nothing clicked, nothing ventured

            //Create a list of Raycast Results
            results.Clear();

            //Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, results);//can be null...


            //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
            foreach (RaycastResult result in results)
            {
                ARPGDebugger.DebugMessage("Hit " + result.gameObject.name, result.gameObject);
                if (TheBoard.IDS.ContainsKey(result.gameObject) == false) continue;

                
                bool placed = TryPlaceOnBoard(result.gameObject, piece, true);
                if (placed)
                {
                    Events.SceneEvents.OnPiecePlaced?.Invoke(piece);
                    return;
                }
            }

            //if didn't place on bard, try passing to other systems
            OnTryPlace?.Invoke(results, piece);

        }

        /// <summary>
        /// trigger to detect if we should try remove an item
        /// </summary>
        /// <returns></returns>
        protected virtual bool RemoveTrigger()
        {
            return Input.GetMouseButtonDown(0);
        }

        /// <summary>
        /// try remove from grid
        /// </summary>
        protected virtual void TryRemoveDraggableFromGrid()
        {

            if (m_PointerEventData == null) return;//nothing clicked, nothing ventured

            //Create a list of Raycast Results
            results.Clear();

            //Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, results);//can be null...


            //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
            foreach (RaycastResult result in results)
            {
                if (TheBoard.IDS.ContainsKey(result.gameObject) == false) continue;
                IInventoryPiece removed = TryRemoveDraggableFromGrid(result.gameObject);
                if (removed != null)
                {
                    Events.SceneEvents.OnPieceRemoved?.Invoke(removed);
                    return;
                }

            }

            OnTryRemove?.Invoke(results);

        }

        protected virtual void DescribeItem(Item item)
        {
            if (hasHoverOverPanel == false) return;
            describeEquipment.SetMyEquipment(item);
            describeEquipment.SetHighlightedItem(null);
        }
        protected virtual void TryHighlight()
        {

            describeEquipment.SetHighlightedItem(null);
            describeEquipment.SetMyEquipment(null);
            if (m_PointerEventData == null) return;//nothing clicked, nothing ventured

            //Create a list of Raycast Results
            results.Clear();

            //Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, results);//can be null...


            //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
            foreach (RaycastResult result in results)
            {
                GameObject keyinstance = result.gameObject;
                if (TheBoard.PieceInSlot.ContainsKey(keyinstance))
                {
                    IInventoryPiece piece = TheBoard.PieceInSlot[keyinstance];
                    Debug.Log("Highlight " + piece.ItemStack.GetGeneratedItemName());
                    if (piece.ItemStack is Equipment)
                    {
                        Equipment equipment = null;
                        describeEquipment.SetHighlightedItem(piece.ItemStack);
                        equipment = user.MyInventory.GetInventoryRuntime().GetEquipmentAtSlot((EquipmentSlotsType)piece.EquipmentIdentifier[0]).EquipmentInSlots;

                        describeEquipment.SetMyEquipment(equipment);
                        describeEquipment.EnableComparisonPanel();
                    }
                    else
                    {
                        DescribeItem(piece.ItemStack);
                    }


                    return;
                }
                


            }

           ONTryHighlight?.Invoke(results);

        }

        protected virtual IInventoryPiece TryRemoveDraggableFromGrid(GameObject key)
        {
            return TheBoard.RemovePiece(TheBoard.IDS[key]);

        }


        /// <summary>
        /// try place draggable piece on board
        /// </summary>
        /// <param name="slotKey"></param>
        /// <param name="newPiece"></param>
        /// <returns></returns>
        protected virtual bool TryPlaceOnBoard(GameObject slotKey, IInventoryPiece newPiece, bool allowswap)
        {
            List<IBoardSlot> placed = TheBoard.PlaceOnBoard(newPiece, TheBoard.IDS[slotKey], allowswap);
            if (placed.Count > 0)//means success
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// preview stuff could use a cell class that we can enable/disable preview stuff, clean up later
        /// </summary>
        /// <param name="piece"></param>
        protected virtual void PreviewHighlight(IInventoryPiece piece)
        {
            if (piece == null) return;


            for (int i = 0; i < TheBoard.Preview.PreviewList.Count; i++)
            {
                TheBoard.Preview.PreviewList[i].Cell.Preview = false;
                TheBoard.Preview.CellDictionary[TheBoard.Preview.PreviewList[i]].GetComponent<Image>().color = transparent;
            }


            List<RaycastResult> preview = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, preview);
            piece.PreviewInstance.SetActive(false);
            foreach (RaycastResult result in preview)
            {
                if (TheBoard.IDS.ContainsKey(result.gameObject) == false) continue;
                //else, show preview

                PreviewPieceOnGameObject(piece, result.gameObject);
            }

            OnGridOrOff(piece);

        }

        private void OnGridOrOff(IInventoryPiece piece)
        {
            if (TheBoard.Preview.PreviewList.Count > 0)
            {
                for (int i = 0; i < TheBoard.Preview.PreviewList.Count; i++)
                {
                    if (TheBoard.Preview.PreviewList[i].Cell.Preview == false)
                    {

                        TheBoard.Preview.CellDictionary[TheBoard.Preview.PreviewList[i]].GetComponent<Image>().color = transparent;
                        TheBoard.Preview.PreviewList.RemoveAt(i);
                    }
                    else
                    {
                        TheBoard.Preview.CellDictionary[TheBoard.Preview.PreviewList[i]].GetComponent<Image>().color = valid;
                    }
                }
            }
            else
            {
                //not on the board, so just drag it around
                ARPGDebugger.DebugMessage("Off Grid", piece.Instance.gameObject);
                piece.Instance.transform.position = GetScreenPosition();
            }
        }

        protected virtual void PreviewPieceOnGameObject(IInventoryPiece piece, GameObject uiobject)
        {
            TheBoard.Preview.PreviewList = TheBoard.TryPlace(piece, TheBoard.IDS[uiobject]);
            for (int i = 0; i < TheBoard.Preview.PreviewList.Count; i++)
            {
                IBoardSlot slot = TheBoard.Preview.PreviewList[i];
                if (slot == null)
                {
                    TheBoard.Preview.PreviewList.RemoveAt(i);
                    continue;
                }
                else if (slot.Cell.Occupied)
                {
                    piece.PreviewInstance.GetComponentInChildren<Image>().color = invalid;


                }
                else
                {
                    piece.PreviewInstance.GetComponentInChildren<Image>().color = valid;

                }
            }


            Vector3 newpos = uiobject.transform.position;
            piece.PreviewInstance.transform.position = newpos;
            piece.PreviewInstance.SetActive(true);

            for (int i = 0; i < TheBoard.Preview.PreviewList.Count; i++)
            {
                if (TheBoard.Preview.PreviewList[i] == null)
                {
                    TheBoard.Preview.PreviewList.RemoveAt(i);
                    continue;
                }
                TheBoard.Preview.PreviewList[i].Cell.Preview = true;

            }
        }

        #endregion

        #region ARPG interface

        public virtual void SetUser(IUseInvCanvas newUser)
        {
            describeStats = GetComponent<IDescribePlayerStats>();
            describeEquipment = GetComponent<IDescribeEquipment>();
            hasHoverOverPanel = describeEquipment != null;
            hasStateDisplay = describeStats != null;
            if (hasHoverOverPanel)
            {
                describeEquipment.SetMyEquipment(null);
                describeEquipment.SetHighlightedItem(null);
                describeEquipment.DisableComparisonPanel();
            }
            user = newUser.GetActorHub();
            inv = user.MyInventory.GetInventoryRuntime();
            GridSetup(newUser);
            DisplayStats();
        }

        protected virtual void GridSetup(IUseInvCanvas newUser)
        {

            CreateGrid();
            GearUI.CreateGear(newUser.GetActorHub());
            EnablePlayerInventoryUI(true);

            List<ItemStack> stack = inv.GetAllUniqueStacks();
            for (int i = 0; i < stack.Count; i++)
            {
                UpdateUI(stack[i].SlotID, stack[i]);

            }

            inv.OnSlotChanged += UpdateUI;
            inv.OnEquipmentSlotChanged += RefreshDisplay;

            //ugly use due to unity needing a frame to update the grid layout...
            StartCoroutine(Waitaframe());
        }

        protected virtual void RefreshDisplay(EquipmentSlot equipment)
        {
            DisplayStats();
        }
        
        protected virtual void UpdateUI(int slot, ItemStack stack)
        {
            if (stackdic.ContainsKey(stack))
            {
                //we have a stack
                GameObject key = stackdic[stack];
                if (piecesdic.ContainsKey(key))
                {
                    //we have a ui piece on the board
                    if (stack.Item == null || stack.CurrentStackSize == 0)
                    {
                        //ddestroy piece
                        stackdic.Remove(stack);
                        IInventoryPiece piece = piecesdic[key];

                        TheBoard.RemovePiece(piece);
                        piecestackdic.Remove(piece);
                        piece.CleanUP();
                        piece = null;

                        piecesdic.Remove(key);
                        Destroy(key);
                    }

                }
                else
                {
                    CreatePieceFromInventory(stack);
                }
            }
            else
            {
                CreatePieceFromInventory(stack);
            }


      
        }

        protected virtual bool CreatePieceFromInventory(ItemStack stack)
        {

            if (stack.Item == null) return false;

            IInventoryPiece piece = CreatePiece(stack.Item);
            bool placed = false;

            for (int j = 0; j < TheBoard.Slots.Count; j++)
            {
                GameObject instance = TheBoard.Slots[j].Instance;
                if (piecesdic.ContainsKey(instance) == false)
                {
                    PreviewPieceOnGameObject(piece, instance);
                    placed = TryPlaceOnBoard(instance, piece, false);
                    if (placed)
                    {
                        piece.Instance.transform.position = piece.PreviewInstance.transform.position;
                        piece.Instance.SetActive(true);
                        piece.PreviewInstance.SetActive(false);
                        instance.name = stack.Item.GetGeneratedItemName();
                        piecesdic[instance] = piece;
                        stackdic[stack] = instance;
                        piecestackdic[piece] = stack;
                        ARPGDebugger.DebugMessage("Item " + stack.Item + " placed " + placed + " at " + instance.name, instance);
                        placed = true;
                        break;
                    }
                }
            }

            if (placed == false)
            {
                //couldnt fit it
                piece.CleanUP();
                ItemHandler.DropItem(stack.Item, user.MyInventory, stack.SlotID);
            }
            else
            {
                Image image = piece.Instance.GetComponentInChildren<Image>();
                Image prev = piece.PreviewInstance.GetComponentInChildren<Image>();
                image.sprite = stack.Item.GetSprite();
                prev.sprite = stack.Item.GetSprite();
            }
            return placed;
        }
       
        /// <summary>
        /// ugly but required in order for unity to refresh its grid component on initialization
        /// </summary>
        /// <returns></returns>
       protected IEnumerator Waitaframe()
        {
            yield return 0;
            TogglePlayerInventoryUI();
        }

        protected virtual void OnDestroy()
        {
            inv.OnSlotChanged -= UpdateUI;
            inv.OnEquipmentSlotChanged -= RefreshDisplay;

        }
       

        public virtual void DisableHoverOver()
        {
           //not implemented in this version
        }

        public virtual void EnableHoverOverInstance(Transform atPos, Item item, bool enableComparison)
        {
            //not implemented in this version
        }

        public virtual void EnablePlayerInventoryUI(bool isEnabled)
        {
            MainPanel.gameObject.SetActive(isEnabled);
            if (isEnabled)
            {
                RefreshInventoryUI();
                DisplayStats();
            }
        }

        public virtual void TogglePlayerInventoryUI()
        {
            bool toggle = !MainPanel.gameObject.activeInHierarchy;
            EnablePlayerInventoryUI(toggle);
            if (FreezeDungeon && MainPanel.gameObject.activeInHierarchy)
            {
                DungeonMaster.Instance.GetDungeonUISceneRef().SetDungeonState(0);
            }
            else if (FreezeDungeon && !MainPanel.gameObject.activeInHierarchy)
            {
                DungeonMaster.Instance.GetDungeonUISceneRef().SetDungeonState(1);

            }
        }

        public virtual void RefreshInventoryUI()
        {

            foreach (var kvp in piecesdic)
            {

                if (TheBoard.SlotsOccupied.ContainsKey(kvp.Value))
                {
                    if (TheBoard.SlotsOccupied[kvp.Value].Count > 0)
                    {
                        kvp.Value.Instance.transform.position = TheBoard.SlotsOccupied[kvp.Value][0].Instance.transform.position;
                    }

                }
                else
                {
                    kvp.Value.Instance.transform.position = kvp.Key.transform.position;
                }


                if (kvp.Value.Instance != null)
                {
                    kvp.Value.Instance.SetActive(true);
                    kvp.Value.PreviewInstance.SetActive(false);
                }

         
            }

           
        }

        public virtual bool GetCanvasEnabled()
        {
            return MainPanel.gameObject.activeInHierarchy;
        }


        public virtual void DisplayStats()
        {
            if (hasStateDisplay == false) return;
            describeStats.DisplayStats(user);
        }


        #endregion
    }
}