using GWLPXL.ARPGCore.Abilities.com;
using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Combat.com;
using GWLPXL.ARPGCore.Statics.com;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ARPGCore.Abilities.Mods.com
{

    [System.Serializable]
    public class KnockbackAOEVars
    {
        public AoEWeapoNVars AOE;
        public KnockBackVars Vars;

        public KnockbackAOEVars(AoEWeapoNVars aoe, KnockBackVars vars)
        {
            AOE = aoe;
            Vars = vars;
        }
    }


    /// <summary>
    /// Applies a knockback effect the player's melee weapons
    /// </summary>

    [CreateAssetMenu(menuName = "GWLPXL/ARPG/Abilities/Mods/New_AOE_Knockback")]

    public class ApplyAoEKnockback : AbilityLogic
    {
        public KnockbackAOEVars Vars;
        [System.NonSerialized]
        Dictionary<Transform, Knockback_AOE> knockbacks = new Dictionary<Transform, Knockback_AOE>();
       

      
        public override bool CheckLogicPreRequisites(IActorHub forUser)
        {
            return true;
        }

        public override void EndCastLogic(IActorHub skillUser, Ability theSkill)
        {
            ModHelper.RemoveKnockbackAOEMod(skillUser, knockbacks);
        }

      

        public override void StartCastLogic(IActorHub skillUser, Ability theSkill)
        {
            ModHelper.ApplyKnockbackAOEMod(skillUser, Vars, knockbacks);
        }
    }
}