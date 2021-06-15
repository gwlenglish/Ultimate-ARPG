
using GWLPXL.ARPGCore.com;
using UnityEngine;

namespace GWLPXL.ARPGCore.Auras.com
{


    public class EnemyAuraUser : MonoBehaviour, IUseAura
    {
        [SerializeField]
        EnemyAuraEvents auraEvents = new EnemyAuraEvents();
        [SerializeField]
        AuraController AuraControllerTemplate = null;

        AuraController runtime = null;

        IActorHub hub = null;
        void Awake()
        {
            SetRuntimeAuraController(Instantiate(AuraControllerTemplate));


        }
        public void SetTemplate(AuraController newTemplate)
        {
            AuraControllerTemplate = newTemplate;
        }
        public void SubscribeEvents()
        {
            GetAuraControllerRuntime().OnEquippedAura += OnEquipped;
            GetAuraControllerRuntime().OnEquippedAura += OnLearned;
            GetAuraControllerRuntime().OnEquippedAura += OnForgot;

        }

        void OnEquipped(Aura aura, int slot)
        {
            auraEvents.SceneEvents.OnAuraEquipped.Invoke(aura);
        }
        void OnLearned(Aura aura, int slot)
        {
            auraEvents.SceneEvents.OnAuraLearned.Invoke(aura);
        }
        void OnForgot(Aura aura, int slot)
        {
            auraEvents.SceneEvents.OnAuraForgot.Invoke(aura);
        }
        public void UnSubscribeEvents()
        {
            if (GetAuraControllerRuntime() == null) return;
            GetAuraControllerRuntime().OnEquippedAura -= OnEquipped;
            GetAuraControllerRuntime().OnEquippedAura -= OnLearned;
            GetAuraControllerRuntime().OnEquippedAura -= OnForgot;
        }


        public AuraController GetAuraControllerRuntime()
        {
            return runtime;
        }

        public void ToggleAura(Aura aura)
        {
            GetAuraControllerRuntime().ToggleEquippedAura(aura, hub.MyAuraTaker);
        }
        public void ToggleAura(int atEquippedSlot)
        {
            GetAuraControllerRuntime().ToggleEquippedAura(atEquippedSlot, hub.MyAuraTaker);
        }

        public AuraController GetAuraControllerTemplate()
        {
            return AuraControllerTemplate;
        }

        public void SetRuntimeAuraController(AuraController controller)
        {
            runtime = controller;
            runtime.TryInitialize();

        }

        public void SetActorHub(IActorHub newHub)
        {
            hub = newHub;
        }
    }
}