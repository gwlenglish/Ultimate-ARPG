using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Combat.com;
using GWLPXL.ARPGCore.StatusEffects.com;
using GWLPXL.ARPGCore.Types.com;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ARPGCore.Statics.com
{


    public static class SoTHelper
    {
        public static System.Action<IActorHub, ModifyResourceVars> OnDotApplied;
        public static System.Action<IActorHub, ModifyResourceVars> OnDoTRemoved;
        public static System.Action<IActorHub, int, ResourceType, ElementType> OnRegenResource;
        public static System.Action<IActorHub, int, ResourceType, ElementType> OnReduceResource;
        public static Dictionary<IActorHub, Dictionary<ModifyResourceVars, ModifyResourceDoTState>> Dotdic => dotdic;
        static Dictionary<IActorHub, Dictionary<ModifyResourceVars, ModifyResourceDoTState>> dotdic = new Dictionary<IActorHub, Dictionary<ModifyResourceVars, ModifyResourceDoTState>>();

        public static bool CanApplySoT(IActorHub owner, IActorHub target, IDoActorDamage damage, IApplySOT sots)
        {
            if (target == null) return false;
            if (target.MyStatusEffects == null) return false;
            if (damage.GetActorDamageData().DamageVar.DamageOptions.InflictSoT == false) return false;
            if (damage.GetActorDamageData().DamageVar.CombatHandler.DetermineAttackable(target, owner, damage.GetActorDamageData().DamageVar.SoTOptions.FriendlyFIre) == false) return false;//cant attack
            if (sots.GetSoTAppliedList().Contains(target)) return false;//only allow one application per active swing
            return true;
        }

        public static List<ModifyResourceDoTState> GetAllAppliedDots(IActorHub self)
        {
            List<ModifyResourceDoTState> dots = new List<ModifyResourceDoTState>();
            if (dotdic.ContainsKey(self))
            {
                foreach (var kvp in dotdic)
                {
                    Dictionary<ModifyResourceVars, ModifyResourceDoTState> _ = kvp.Value;
                    foreach (var jvp in _)
                    {
                        dots.Add(jvp.Value);
                    }
                }
            }
            return dots;
        }
        public static void ReduceResource(IActorHub target, int dmgamount, ResourceType type)
        {
            ReduceResource(target, dmgamount, type, ElementType.None);
        }
        public static void ReduceResource(IActorHub target, int dmgamount, ResourceType type, ElementType elementRegen)
        {
            if (target.MyHealth.IsDead()) return;

            //since this is a reduce call, ensure that we are passing a negative
            if (dmgamount > 0)
            {
                dmgamount *= -1;
            }

            target.MyStats.GetRuntimeAttributes().ModifyNowResource(type, dmgamount);
            target.MyHealth.CheckDeath();
            OnReduceResource?.Invoke(target, dmgamount, type, elementRegen);


        }
        public static void RegenResource(IActorHub target, int healAmount, ResourceType type)
        {
            RegenResource(target, healAmount, type, ElementType.None);
        }
        public static void RegenResource(IActorHub target, int healAmount, ResourceType type, ElementType elementRegen)
        {
            //do something with element type
            if (target.MyHealth.IsDead()) return;

            healAmount = Mathf.Abs(healAmount);
            target.MyStats.GetRuntimeAttributes().ModifyNowResource(type, healAmount);//we dont call into take damage because we dont want to trigger the iframe cooldown due to a DOT
            target.MyHealth.CheckDeath();
            OnRegenResource?.Invoke(target, healAmount, type, elementRegen);

        }
        public static void AddDoT(IActorHub target, ModifyResourceVars vars)
        {
            if (dotdic.ContainsKey(target) == false)
            {
                dotdic[target] = new Dictionary<ModifyResourceVars, ModifyResourceDoTState>();
            }

            Dictionary<ModifyResourceVars, ModifyResourceDoTState> value = dotdic[target];
            if (value.ContainsKey(vars) == false)
            {

                ModifyResourceDoTState newone = new ModifyResourceDoTState(target, vars);
                newone.ApplyDoT();
                value[vars] = newone;

            }
            else
            {
                value[vars].ApplyDoT();
            }
            OnDotApplied?.Invoke(target, vars);


        }
        /// <summary>
        /// remove damage over time
        /// </summary>
        /// <param name="vars"></param>
        public static void RemoveDot(IActorHub target, ModifyResourceVars vars)
        {
            if (dotdic.ContainsKey(target) == false)
            {
                Debug.LogWarning("Trying to remove a dot when none are in the dictionary", target.MyTransform);
                return;
            }
            Dictionary<ModifyResourceVars, ModifyResourceDoTState> value = dotdic[target];
            value.Remove(vars);
            dotdic[target] = value;
            OnDoTRemoved?.Invoke(target, vars);

        }

    }
}