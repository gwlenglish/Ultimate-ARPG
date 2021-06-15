using GWLPXL.ARPGCore.Abilities.com;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace GWLPXL.ARPGCore.CanvasUI.com
{




    [System.Serializable]
    public class UnityUpdateSlotEventUser : UnityEvent<int, IAbilityUser> { }
    [System.Serializable]
    public class UnityAbilityEvent : UnityEvent<Ability, IAbilityUser> { }

    [System.Serializable]
    public class DraggableEvents
    {
        public UnityEvent OnDraggableEnabled = new UnityEvent();
        public UnityEvent OnDraggableDisabled = new UnityEvent();
    }

    [System.Serializable]
    public class DraggableAbilityEvents
    {
        public DraggableEvents SceneEvents = new DraggableEvents();
    }

    [System.Serializable]
    public class SlotEvents
    {
        public UnityEvent OnUpdated = new UnityEvent();
        public UnityUpdateSlotEventUser OnEquippedSlotUpdated = new UnityUpdateSlotEventUser();


    }
    [System.Serializable]
   public class EquippedAbilityButtonEvents
    {
        public SlotEvents SceneEvents = new SlotEvents();
    }
}