
using GWLPXL.ARPGCore.Abilities.com;
using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Statics.com;
using UnityEngine;

namespace GWLPXL.ARPGCore.Abilities.Mods.com
{
    /// <summary>
    /// base class to add Ability mods. Inherit to create your own
    /// </summary>
    public abstract class AbilityLogic : ScriptableObject, ISaveJsonConfig
    {
        [SerializeField]
        protected TextAsset config;

        /// <summary>
        /// checked before the ability is triggered
        /// </summary>
        /// <param name="forUser"></param>
        /// <returns></returns>
        public abstract bool CheckLogicPreRequisites(IActorHub forUser);
        /// <summary>
        /// Called after the delay period, if any.
        /// </summary>
        /// <param name="skillUser"></param>
        /// <param name="theSkill"></param>
        public abstract void StartCastLogic(IActorHub skillUser, Ability theSkill);


        /// <summary>
        /// Called after the cooldown period, if any.
        /// </summary>
        /// <param name="skillUser"></param>
        /// <param name="theSkill"></param>
        public abstract void EndCastLogic(IActorHub skillUser, Ability theSkill);

        public void SetTextAsset(TextAsset textAsset)
        {
            config = textAsset;
        }

        public TextAsset GetTextAsset()
        {
            return config;
        }

        public Object GetObject()
        {
            return this;
        }
    }
}