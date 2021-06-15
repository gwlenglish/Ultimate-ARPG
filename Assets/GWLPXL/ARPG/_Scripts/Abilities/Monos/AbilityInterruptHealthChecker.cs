using GWLPXL.ARPGCore.Attributes.com;
using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Combat.com;

using UnityEngine;


namespace GWLPXL.ARPGCore.Abilities.com
{


    public class AbilityInterruptHealthChecker : MonoBehaviour, IInterruptAbilityChecker
    {
        public Ability ToInterrupt;
        public System.Action<Ability> OnInterrupt;
        public AbilityInterruptOptions Options;
        public IActorHub Hub;
        bool interrupted = false;


        private void Start()
        {
            OnInterrupt += Interrupted;
            if (Options.OnCasterDamaged)
            {

                if (Hub != null && Hub.MyStats != null)
                {
                    Hub.MyStats.GetRuntimeAttributes().OnResourceChanged += ResourceChanged;
                }
            }
        }

        void Interrupted(Ability ability)
        {
            interrupted = true;
        }

        void ResourceChanged(int resource)
        {
            if (resource == (int)Hub.MyHealth.GetHealthResource())
            {
                if (Options.OnCasterDamaged)
                {
                    OnInterrupt.Invoke(ToInterrupt);
                }

                if (Options.OnCasterDied)
                {
                    if (Hub.MyHealth.IsDead())
                    {
                        OnInterrupt.Invoke(ToInterrupt);
                    }
                }
               
            }
        }
       

        public void Remove()
        {
            OnInterrupt -= Interrupted;
            if (Hub != null && Hub.MyStats != null)
            {
                Hub.MyStats.GetRuntimeAttributes().OnResourceChanged -= ResourceChanged;
            }
            Destroy(this);
        }
    }
}