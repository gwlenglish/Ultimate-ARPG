﻿

using GWLPXL.ARPGCore.Abilities.com;
using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Statics.com;
using GWLPXL.ARPGCore.StatusEffects.com;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ARPGCore.Abilities.Mods.com
{

    /// <summary>
    /// Applies a generate resource on hit mod to the user's melee weapons
    /// </summary>
    [CreateAssetMenu(menuName = "GWLPXL/ARPG/Abilities/Mods/New_GenerateResource_Hit")]

    public class ApplyGenerateResouceOnHit : AbilityLogic
    {
        public GenerateRourseOnHitVars Vars = new GenerateRourseOnHitVars();

        [System.NonSerialized]
        Dictionary<Transform, GenerateResourceOnHit> buffed = new Dictionary<Transform, GenerateResourceOnHit>();

   

        public override bool CheckLogicPreRequisites(IActorHub forUser)
        {
            return true;
        }

        public override void EndCastLogic(IActorHub skillUser, Ability theSkill)
        {
            ModHelper.RemoveGenerateResourceOnHitMod(skillUser, buffed);
        }



        public override void StartCastLogic(IActorHub skillUser, Ability theSkill)
        {
            ModHelper.ApplyGenerateResourceOnHitMod(skillUser, Vars, buffed);
        }
    }
}
