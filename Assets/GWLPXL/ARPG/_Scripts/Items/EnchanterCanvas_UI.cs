﻿using System.Collections;
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
        public bool IsNative = true;
        public int Ilevel = 1;
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
        protected IUseEnchanterCanvas user;
        protected IEnchantableUIElement draggableenchantable;
        protected IEnchantUIElement draggableEnchant;
        protected IEnchantUIElement previewEnchant;
        protected IEnchantableUIElement successEnchantable;
        protected IEnchantableUIElement decisionPreview;
        protected IEnchantableUIElement enchantablePreview;
        protected bool draggingEnchantable;
        protected bool draggingEnchant;
        protected Vector3 homeposition;
        protected bool active;

        Item preview = null;
        protected virtual void Awake()
        {
            previewEnchant = EnchantPreviewInstance.GetComponent<IEnchantUIElement>();
            enchantablePreview = EnchantablePreviewInstance.GetComponentInChildren<IEnchantableUIElement>();
            decisionPreview = DecisionBoxPreview.GetComponentInChildren<IEnchantableUIElement>();
            successEnchantable = SuccessBoxShowcase.GetComponentInChildren<IEnchantableUIElement>();
            homeposition = DraggableEnchantableInstance.transform.position;
            draggableenchantable = DraggableEnchantableInstance.GetComponent<IEnchantableUIElement>();
            draggableEnchant = DraggableEnchantInstance.GetComponent<IEnchantUIElement>();

        }
        protected virtual void Start()
        {
            Close();
        }
        public void Close()
        {
            CloseDown();

        }

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
            EquipmentTrait trait = previewEnchant.GetEnchant();
            if (item == null || trait == null)
            {
                //failed
                return;
            }

            if (item is Equipment)
            {
                Equipment eq = item as Equipment;
                EquipmentTrait[] natives = eq.GetStats().GetNativeTraits();
                Debug.Log(natives.Length);

                //get current traits, make copies
                EquipmentTrait[] copynatives = new EquipmentTrait[natives.Length];
                for (int i = 0; i < copynatives.Length; i++)
                {
                    copynatives[i] = Instantiate(natives[i]);
                }
                EquipmentTrait[] randos = eq.GetStats().GetRandomTraits();
                Debug.Log(randos.Length);
                EquipmentTrait[] copyrandos = new EquipmentTrait[randos.Length];
                for (int i = 0; i < copyrandos.Length; i++)
                {
                    copyrandos[i] = Instantiate(randos[i]);
                }

                Equipment copy = Instantiate(eq);
                copy.GetStats().SetNativeTraits(copynatives);//assign copies to copy
                copy.GetStats().SetRandomTraits(copyrandos);

                EquipmentTrait copyenchant = Instantiate(trait);

                station.Enchant(copy, copyenchant, Ilevel, IsNative, true);
                Debug.Log(eq);
                preview = copy;
                DecisionBoxPreview.SetActive(true);
                decisionPreview.SetEnchantable(preview);
            }


         
        }
        public virtual void Enchant()
        {
            Item item = enchantablePreview.GetEnchantable();
            EquipmentTrait trait = previewEnchant.GetEnchant();
            if (item == null || trait == null)
            {
                //failed
                return;
            }
            ClosePreview();
            station.Enchant(item as Equipment, trait, Ilevel, IsNative);
            SuccessBoxShowcase.SetActive(true);
            successEnchantable.SetEnchantable(item);
            enchantablePreview.SetEnchantable(null);
            previewEnchant.SetEnchant(null);
            draggableEnchant.SetEnchant(null);
            draggableenchantable.SetEnchantable(null);

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
        protected virtual void TryPlaceInSlot(EquipmentTrait enchant)
        {
            draggableEnchant.SetEnchant(enchant);
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
                    EnchantablePreviewInstance.GetComponent<IEnchantableUIElement>().SetEnchantable(draggableenchantable.GetEnchantable());
                
                }

            }
        }
       

        protected virtual void TryPlaceInSlot(Item item)
        {
            draggableenchantable.SetEnchantable(item);
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
            station = null;

            if (FreezeDungeon)
            {
                DungeonMaster.Instance.GetDungeonUISceneRef().SetDungeonState(1);
            }
        }

        public void Open(IUseEnchanterCanvas user)
        {
            Setup(user);
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

            List<EquipmentTrait> enchants = station.GetAllEnchants();
            for (int i = 0; i < enchants.Count; i++)
            {
                CreateUIEnchantElement(enchants[i]);
            }

            if (FreezeDungeon)
            {
                DungeonMaster.Instance.GetDungeonUISceneRef().SetDungeonState(0);
            }
        }

        protected virtual void CreateUIEnchantElement(EquipmentTrait enchant)
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

        public void SetStation(EnchantingStation station)
        {
            this.station = station;
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
      
    }
}