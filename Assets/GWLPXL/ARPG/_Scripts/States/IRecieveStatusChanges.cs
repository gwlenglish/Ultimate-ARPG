

using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Types.com;
using UnityEngine;
using System.Collections.Generic;
using GWLPXL.ARPGCore.Statics.com;

namespace GWLPXL.ARPGCore.StatusEffects.com
{
   
    public interface IRecieveStatusChanges
    {
        List<StatusEffectVars> GetCurrentApplied();   
        //void AddDoT(ModifyResourceVars vars);
        //void RemoveDot(ModifyResourceVars vars);
        //void RegenResource(int healAmount, ResourceType type);
        //void RegenResource(int healAmount, ResourceType type, ElementType elementRegen);

        //void ReduceResource(int damageAmount, ResourceType type);
        //void ReduceResource(int damageAmount, ResourceType type, ElementType elementDamage);

        IActorHub GetActorHub();
        void SetActorHub(IActorHub newHub);

        Transform AtLocation();


    }
}