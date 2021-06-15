
using GWLPXL.ARPGCore.Abilities.com;
using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.DebugHelpers.com;
using GWLPXL.ARPGCore.Statics.com;
using UnityEngine;

namespace GWLPXL.ARPGCore.Abilities.Mods.com
{

    /// <summary>
    /// Elemental Cleave mod for a weapon
    /// </summary>
    public class ElementalWeapon_AOE : MonoBehaviour, IWeaponModification
    {
        public AoEWeapoNVars Vars;
        bool active = false;
        IActorHub myDamage = null;
        
        public void DoModification(IActorHub other)
        {
            if (IsActive() == false) return;

            CombatHelper.DoElementalCleave(myDamage, transform.position, Vars);

        }

      

        public bool DoChange(Transform other)
        {
            return false;
        }

        public Transform GetTransform() => this.transform;
    
        public bool IsActive()
        {
            return active;
        }

        public void SetActive(bool isEnabled)
        {
            active = isEnabled;
        }

        public void SetUser(IActorHub myself)
        {
            myDamage = myself;
        }
    }
}