

using GWLPXL.ARPGCore.Abilities.com;
using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Combat.com;
using GWLPXL.ARPGCore.Statics.com;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ARPGCore.Abilities.Mods.com
{

    /// <summary>
    /// Applies an Explosive mod to the user's melee weapons
    /// </summary>
    [CreateAssetMenu(menuName = "GWLPXL/ARPG/Abilities/Mods/New_ExplosiveMod")]

    public class ApplyExplosiveMod : AbilityLogic
    {
        public ExplosionVars Vars;
        [System.NonSerialized]
        Dictionary<Transform, ExplosiveMod> buffed = new Dictionary<Transform, ExplosiveMod>();



        public override bool CheckLogicPreRequisites(IActorHub forUser)
        {
            return true;
        }

        public override void EndCastLogic(IActorHub skillUser, Ability theSkill)
        {
            ModHelper.RemoveExplosiveMod(skillUser, buffed);
        }

      

        public override void StartCastLogic(IActorHub skillUser, Ability theSkill)
        {
            ModHelper.ApplyExplosiveMod(skillUser, Vars, buffed);
        }
    }
}
