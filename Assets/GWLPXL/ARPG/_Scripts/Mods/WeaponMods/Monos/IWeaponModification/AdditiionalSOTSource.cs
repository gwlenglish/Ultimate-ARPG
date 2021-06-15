﻿
using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Combat.com;
using GWLPXL.ARPGCore.Statics.com;
using GWLPXL.ARPGCore.StatusEffects.com;
using UnityEngine;


namespace GWLPXL.ARPGCore.Abilities.Mods.com
{
    [System.Serializable]
    public class AdditionalSoTSourceVars
    {
        public StatusOverTimeWeaponStatusOptions StatusOverTimeOptions = new StatusOverTimeWeaponStatusOptions(new ModifyResourceVars[0]);
    }

    public class AdditiionalSOTSource : MonoBehaviour, IWeaponModification
    {
        public AdditionalSoTSourceVars Vars = new AdditionalSoTSourceVars();
        bool active = false;
        IActorHub self = null;
        public void DoModification(IActorHub other)
        {
            if (IsActive() == false) return;
            CombatHelper.DoAddAdditionalSoT(other, Vars);

        }

        public bool DoChange(Transform other)
        {
            return false;
        }

        public Transform GetTransform() => this.transform;

        public bool IsActive() => active;

        public void SetActive(bool isEnabled)
        {
            active = isEnabled;
        }

        public void SetUser(IActorHub myself) => self = myself;
      
       
    }
}