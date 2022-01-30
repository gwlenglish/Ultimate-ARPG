using GWLPXL.ARPGCore.com;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace GWLPXL.ARPGCore.Combat.com
{
    /// <summary>
    /// temp log for now to see results, eventually translate results into a string.
    /// </summary>
    public static class CombatLogger
    {
        public static event Action<DamageResults> OnResultAdded;

        static List<DamageResults> log = new List<DamageResults>();
        public static void AddResult(DamageResults results)
        {
            log.Add(results);
            OnResultAdded?.Invoke(results);
        }

       
        


    }
    /// <summary>
    /// used to expose the log to the inspector for the time being
    /// </summary>
    public class CombatLog : MonoBehaviour
    {
        private void OnEnable()
        {
            CombatLogger.OnResultAdded += log.Add;
        }

        private void OnDisable()
        {
            CombatLogger.OnResultAdded -= log.Add;

        }

        [Tooltip("Read only log of combat results.")]
        [SerializeField]
        protected List<DamageResults> log;
    }
}