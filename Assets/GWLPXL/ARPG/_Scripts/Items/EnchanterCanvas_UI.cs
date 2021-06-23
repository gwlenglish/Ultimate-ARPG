using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GWLPXL.ARPGCore.Items.com;
using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Traits.com;

namespace GWLPXL.ARPGCore.CanvasUI.com
{
   

    public class EnchanterCanvas_UI : MonoBehaviour, IEnchanterCanvas
    {
        public bool FreezeDungeon = true;
        public Transform MainPanel = null;
        public EnchantUIEvents SceneEvents;
        public bool IsNative = true;
        [Header("Dragging Input")]
        [SerializeField]
        protected string interactButton = "Fire1";
        public GameObject DecisionBoxPreview = null;
        public GameObject SuccessBoxShowcase = null;
        [Header("Enchantables")]
        public GameObject EnchantableUIElementPrefab = null;
        public Transform EnchantableContentParent = null;
        public GameObject DraggableEnchantableInstance = null;
        public GameObject EnchantablePreviewInstance = null;
        [Header("Enchants")]
        public GameObject EnchantUIElementPrefab = null;
        public Transform EnchantContentParent = null;
        public GameObject DraggableEnchantInstance = null;
        public GameObject EnchantPreviewInstance = null;
        protected EnchantingStation station = null;
        protected List<GameObject> enchantableUIElements = new List<GameObject>();
        protected List<GameObject> enchantUIElements = new List<GameObject>();

        protected Dictionary<Item, IEnchantableUIElement> uidic = new Dictionary<Item, IEnchantableUIElement>();
        protected IUseEnchanterCanvas user = null;
        protected IEnchantableUIElement draggableenchantable = null;
        protected IEnchantUIElement draggableEnchant = null;
        protected IEnchantUIElement previewEnchant = null;
        protected IEnchantableUIElement successEnchantable = null;
        protected IEnchantableUIElement decisionPreview = null;
        protected IEnchantableUIElement enchantablePreview = null;
        protected bool draggingEnchantable = false;
        protected bool draggingEnchant = false;
        protected Vector3 homeposition = new Vector3(0, 0, 0);
        protected bool active = false;

        protected Item preview = null;

        #region unity calls
        protected virtual void Awake()
        {
            SetupUI();

        }
        protected virtual void Start()
        {
            Close();
        }
        protected virtual void LateUpdate()
        {
            if (active == false) return;

            //check mouse pos
            if (draggingEnchant == false)
            {
                CheckDraggingOnEnchantable();
                CheckStopDraggingEnchantable();
                DoDraggingEnchantable();
            }

            if (draggingEnchantable == false)
            {
                CheckDraggingOnEnchant();
                CheckStopDraggingEnchant();
                DoDraggingEnchant();
            }
        }
        #endregion


        public void Close()
        {
            CloseDown();

        }
        public void Open(IUseEnchanterCanvas user)
        {
            Setup(user);
        }
        public void SetStation(EnchantingStation station)
        {
            if (this.station != null)
            {
                this.station.OnEnchanted -= EnchantSuccess;
            }
            this.station = station;
            this.station.OnEnchanted += EnchantSuccess;


        }
       
        public void Toggle()
        {
            if (MainPanel.gameObject.activeInHierarchy)
            {
                Close();
            }
            else
            {
                Open(user);
            }
        }

        public bool GetCanvasEnabled()
        {
            return MainPanel.gameObject.activeInHierarchy;
        }

        public bool GetFreezeMover() => FreezeDungeon;

     
        public virtual void ClosePreview()
        {
            preview = null;
            decisionPreview.SetEnchantable(null);
            previewEnchant.SetEnchant(null);
            enchantablePreview.SetEnchantable(null);

            DecisionBoxPreview.SetActive(false);
        }
        public virtual void DisplayPreview()
        {
            Item item = enchantablePreview.GetEnchantable();
            EquipmentEnchant enchant = previewEnchant.GetEnchant();
            if (item == null || enchant == null)
            {
                //failed
                return;
            }

            if (item is Equipment)
            {
                Equipment eq = item as Equipment;
                EquipmentTrait[] natives = eq.GetStats().GetNativeTraits();

                //get current traits, make copies
                EquipmentTrait[] copynatives = new EquipmentTrait[natives.Length];
                for (int i = 0; i < copynatives.Length; i++)
                {
                    copynatives[i] = Instantiate(natives[i]);
                }
                EquipmentTrait[] randos = eq.GetStats().GetRandomTraits();

                EquipmentTrait[] copyrandos = new EquipmentTrait[randos.Length];
                for (int i = 0; i < copyrandos.Length; i++)
                {
                    copyrandos[i] = Instantiate(randos[i]);
                }

                Equipment copy = Instantiate(eq);
                copy.GetStats().SetNativeTraits(copynatives);//assign copies to copy
                copy.GetStats().SetRandomTraits(copyrandos);

                //EquipmentTrait copyenchant = Instantiate(enchant.EnchantTrait);//clean up if owrsk, remove

                station.Enchant(copy, enchant, IsNative, true);
                Debug.Log(eq);
                preview = copy;
                DecisionBoxPreview.SetActive(true);
                decisionPreview.SetEnchantable(preview);
            }


         
        }
        public virtual void Enchant()
        {
            Item item = enchantablePreview.GetEnchantable();
            EquipmentEnchant trait = previewEnchant.GetEnchant();
            if (item == null || trait == null)
            {
                //failed
                return;
            }
            ClosePreview();
            station.Enchant(item as Equipment, trait,  IsNative);
            SuccessBoxShowcase.SetActive(true);
            successEnchantable.SetEnchantable(item);
            enchantablePreview.SetEnchantable(null);
            previewEnchant.SetEnchant(null);
            draggableEnchant.SetEnchant(null);
            draggableenchantable.SetEnchantable(null);

        }

        protected virtual void EnchantSuccess(Equipment enchanted)
        {
            SceneEvents.OnEnchantSuccess?.Invoke(enchanted);
        }
        protected virtual void SetupUI()
        {
            previewEnchant = EnchantPreviewInstance.GetComponent<IEnchantUIElement>();
            enchantablePreview = EnchantablePreviewInstance.GetComponentInChildren<IEnchantableUIElement>();
            decisionPreview = DecisionBoxPreview.GetComponentInChildren<IEnchantableUIElement>();
            successEnchantable = SuccessBoxShowcase.GetComponentInChildren<IEnchantableUIElement>();
            homeposition = DraggableEnchantableInstance.transform.position;
            draggableenchantable = DraggableEnchantableInstance.GetComponent<IEnchantableUIElement>();
            draggableEnchant = DraggableEnchantInstance.GetComponent<IEnchantUIElement>();
        }
        protected virtual void DoDraggingEnchant()
        {
            if (draggingEnchant == false) return;
            DraggableEnchantInstance.transform.position = Input.mousePosition;
           
        }
        protected virtual void CheckStopDraggingEnchant()
        {
            if (draggableEnchant.GetEnchant() == null) return;

            if (Input.GetButtonUp(interactButton))
            {
                DraggableEnchantInstance.transform.position = homeposition;
                draggingEnchant = false;//also check if trying to place in slot
                if (DraggableEnchantInstance.GetComponent<RectTransform>().rect.Overlaps(EnchantPreviewInstance.GetComponent<RectTransform>().rect))
                {
                    EnchantPreviewInstance.GetComponent<IEnchantUIElement>().SetEnchant(draggableEnchant.GetEnchant());
                    SceneEvents.OnPreviewSetEnchant?.Invoke();
                }

            }
        }
        protected virtual void CheckDraggingOnEnchant()
        {
            for (int i = 0; i < enchantUIElements.Count; i++)
            {
                RectTransform rect = enchantUIElements[i].GetComponent<RectTransform>();
                Vector2 localMousePosition = rect.InverseTransformPoint(Input.mousePosition);
                if (rect.rect.Contains(localMousePosition))
                {
                    if (Input.GetButtonDown(interactButton))
                    {
                        draggingEnchant = true;
                        TryPlaceInSlot(enchantUIElements[i].GetComponent<IEnchantUIElement>().GetEnchant());
                        break;
                    }
                }
            }


        }
        protected virtual void TryPlaceInSlot(EquipmentEnchant enchant)
        {
            draggableEnchant.SetEnchant(enchant);
            if (enchant == null) return;
            SceneEvents.OnStartDragEnchant?.Invoke();
        }
        protected virtual void UpdateEquipmentUI(Item item)
        {
            if (uidic.ContainsKey(item))
            {
                uidic[item].SetEnchantable(item);
            }
        }
        protected virtual void CheckDraggingOnEnchantable()
        {
            for (int i = 0; i < enchantableUIElements.Count; i++)
            {
                RectTransform rect = enchantableUIElements[i].GetComponent<RectTransform>();
                Vector2 localMousePosition = rect.InverseTransformPoint(Input.mousePosition);
                if (rect.rect.Contains(localMousePosition))
                {
                    if (Input.GetButtonDown(interactButton))
                    {
                        draggingEnchantable = true;
                        TryPlaceInSlot(enchantableUIElements[i].GetComponent<IEnchantableUIElement>().GetEnchantable());
                        break;
                    }
                }
            }


        }

        protected virtual void DoDraggingEnchantable()
        {
            if (draggingEnchantable)
            {
                DraggableEnchantableInstance.transform.position = Input.mousePosition;
            }
        }

        protected virtual void CheckStopDraggingEnchantable()
        {
            if (draggableenchantable.GetEnchantable() == null) return;
            if (Input.GetButtonUp(interactButton))
            {
                DraggableEnchantableInstance.transform.position = homeposition;
                draggingEnchantable = false;//also check if trying to place in slot
                if (DraggableEnchantableInstance.GetComponent<RectTransform>().rect.Overlaps(EnchantablePreviewInstance.GetComponent<RectTransform>().rect))
                {
                    //placed
                    EnchantablePreviewInstance.GetComponent<IEnchantableUIElement>().SetEnchantable(draggableenchantable.GetEnchantable());
                    SceneEvents.OnPreviewSetEnchantable?.Invoke();
                }

            }
        }
       

        protected virtual void TryPlaceInSlot(Item item)
        {
            draggableenchantable.SetEnchantable(item);
            if (item == null) return;
            SceneEvents.OnStartDragEnchantable?.Invoke();
        }

        protected virtual void CloseDown()
        {
            active = false;
            DecisionBoxPreview.SetActive(false);
            SuccessBoxShowcase.SetActive(false);
            decisionPreview.SetEnchantable(null);
            previewEnchant.SetEnchant(null);
            enchantablePreview.SetEnchantable(null);

            uidic.Clear();
            MainPanel.gameObject.SetActive(false);

            if (station != null)
            {
                station.OnEnchanted -= UpdateEquipmentUI;
            }

            for (int i = 0; i < enchantUIElements.Count; i++)
            {
                Destroy(enchantUIElements[i].gameObject);
            }
            for (int i = 0; i < enchantableUIElements.Count; i++)
            {
                Destroy(enchantableUIElements[i].gameObject);
            }
            enchantableUIElements.Clear();
            enchantUIElements.Clear();
            station = null;

            if (FreezeDungeon)
            {
                DungeonMaster.Instance.GetDungeonUISceneRef().SetDungeonState(1);
            }
        }



        protected virtual void Setup(IUseEnchanterCanvas user)
        {
            active = true;
            this.user = user;
            this.user.SetShopCanvass(this);
            MainPanel.gameObject.SetActive(true);
            station.OnEnchanted += UpdateEquipmentUI;
          

            List<Equipment> equipped = station.GetEquippedEquipment();

            for (int i = 0; i < equipped.Count; i++)
            {
                Item item = equipped[i];
                CreateUIEnchantableElement(item);
            }

            List<Equipment> invs = station.GetEquipmentInInventory();
            for (int i = 0; i < invs.Count; i++)
            {
                CreateUIEnchantableElement(invs[i]);
            }

            List<EquipmentEnchant> enchants = station.GetAllEnchants();
            for (int i = 0; i < enchants.Count; i++)
            {
                CreateUIEnchantElement(enchants[i]);
            }

            if (FreezeDungeon)
            {
                DungeonMaster.Instance.GetDungeonUISceneRef().SetDungeonState(0);
            }
        }

        protected virtual void CreateUIEnchantElement(EquipmentEnchant enchant)
        {
            GameObject instance = Instantiate(EnchantUIElementPrefab, EnchantContentParent);
            instance.GetComponent<IEnchantUIElement>().SetEnchant(enchant);
            enchantUIElements.Add(instance);
        }
        protected virtual void CreateUIEnchantableElement(Item item)
        {
            GameObject instance = Instantiate(EnchantableUIElementPrefab, EnchantableContentParent);
            IEnchantableUIElement element = instance.GetComponent<IEnchantableUIElement>();
            element.SetEnchantable(item);
            enchantableUIElements.Add(instance);
            uidic[item] = element;
        }

        
      
    }
}