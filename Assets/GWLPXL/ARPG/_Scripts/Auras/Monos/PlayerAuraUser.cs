

using UnityEngine;
using GWLPXL.ARPGCore.com;
namespace GWLPXL.ARPGCore.Auras.com
{
    public interface IScenePersist
    {
        void Load();
        void Save();
    }

    
 


    public class PlayerAuraUser : MonoBehaviour, IUseAura, IScenePersist, ISubscribeEvents
    {

        [SerializeField]
        PlayerAuraEvents auraEvents = new PlayerAuraEvents();
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


        void ResetSceneAuras()
        {
            Aura[] auras = GetAuraControllerRuntime().GetSceneAuras();
            for (int i = 0; i < auras.Length; i++)
            {
                ToggleAura(auras[i]);
            }
        }

        void DisableAurasAndSaveThem()
        {
            Aura[] auras = GetAuraControllerRuntime().GetEquippedAndAppliedAuras();
            for (int i = 0; i < auras.Length; i++)
            {
                GetAuraControllerRuntime().ToggleEquippedAura(auras[i], hub.MyAuraTaker);
            }
            GetAuraControllerRuntime().SetSceneAuras(auras);
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
            if (runtime != null)
            {
                UnSubscribeEvents();
            }

            runtime = controller;
            runtime.TryInitialize();

            if (runtime != null)
            {
                SubscribeEvents();
            }    

        }

        public void Load()
        {
            ResetSceneAuras();
        }

        public void Save()
        {
            DisableAurasAndSaveThem();
        }

        public void SetActorHub(IActorHub newHub)
        {
            hub = newHub;
        }
    }
}