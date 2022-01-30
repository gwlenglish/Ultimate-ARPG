﻿

using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Combat.com;
using GWLPXL.ARPGCore.Types.com;
using UnityEngine;

namespace GWLPXL.ARPGCore.CanvasUI.com
{
    /// <summary>
    ///     /// class that sends information to the floating text canvas
    /// </summary>
    public class EnemyDungeonCanvasUser : MonoBehaviour, IUseFloatingText
    {
        [Tooltip("Will create custom floating text if enabled")]
        [SerializeField]
        protected FloatingTextOverride overrideDmgText;
        [Tooltip("Will create custom floating text if enabled")]
        [SerializeField]
        protected FloatingTextOverride overrideRegenText;
        [Tooltip("Will create custom floating text if enabled")]
        [SerializeField]
        protected FloatingTextOverride overrideDotText;
        [SerializeField]
        [Tooltip("Adjusts the start position of the floating text.")]
        protected Vector3 floatingTextOffset = new Vector3(0, 2, 0);//change the y value to move the hp bar up and down
        [SerializeField]
        protected bool combine = false;
        protected IActorHub hub;

        #region public interface

      
        public virtual void DamageResults(DamageResults args)
        {
            if (combine)
            {
                int dmg = 0;
                for (int i = 0; i < args.ElementResults.Count; i++)
                {
                    dmg += args.ElementResults[i].Reduced;
                }

                for (int i = 0; i < args.PhysicalResult.Count; i++)
                {
                    dmg += args.PhysicalResult[i].PhysicalReduced;
                }
              
                CreateUIDamageText(dmg.ToString(), ElementType.None, false);
            }
            else
            {
                for (int i = 0; i < args.ElementResults.Count; i++)
                {
                    CreateUIDamageText(args.ElementResults[i].Reduced.ToString(), args.ElementResults[i].Type, args.PhysicalResult[i].PhysicalCrit);
                }

                for (int i = 0; i < args.PhysicalResult.Count; i++)
                {
                    CreateUIDamageText(args.PhysicalResult[i].PhysicalReduced.ToString(), ElementType.None, args.PhysicalResult[i].PhysicalCrit);

                }
            }
        }
        public virtual Vector3 GetHPBarOffset()
        {
            return floatingTextOffset;
        }

       

        public virtual void CreateUIDamageText(string message, ElementType type, bool isCritical)
        {
            DefaultDamageText(message, type, isCritical);
            
        }

        public virtual void CreateUIRegenText(string message, ResourceType type)
        {
            DefaultRegenText(message, type);
        }


        public virtual void CreateUIDoTText(string message, ElementType type)
        {
            DefaultDoTText(message, type);
        }

      
        public virtual void SetActorHub(IActorHub newhub)
        {
            hub = newhub;
        }
        #endregion

        #region protected virtual 
        protected virtual void DefaultDoTText(string message, ElementType type)
        {
            if (overrideDotText.UseOverride)
            {
                DungeonMaster.Instance.GetFloatTextCanvas().CreateNewFloatingText(hub.MyHealth, overrideDotText.Override, transform.position + GetHPBarOffset(), message, FloatingTextType.DoTs);
            }
            else
            {
                DungeonMaster.Instance.GetFloatTextCanvas().CreateDoTText(hub.MyHealth, message, transform.position + GetHPBarOffset(), type);
            }

        }

        protected virtual void DefaultRegenText(string message, ResourceType type)
        {
            if (overrideRegenText.UseOverride)
            {
                DungeonMaster.Instance.GetFloatTextCanvas().CreateNewFloatingText(hub.MyHealth, overrideRegenText.Override, transform.position + GetHPBarOffset(), message, FloatingTextType.Regen);

            }
            else
            {
                DungeonMaster.Instance.GetFloatTextCanvas().CreateRegenText(hub.MyHealth, message, transform.position + GetHPBarOffset(), type);
            }
        }

        protected virtual void DefaultDamageText(string message, ElementType type, bool isCritical)
        {
            
            if (overrideDmgText.UseOverride)
            {
                DungeonMaster.Instance.GetFloatTextCanvas().CreateNewFloatingText(hub.MyHealth, overrideDmgText.Override, transform.position + GetHPBarOffset(), message, FloatingTextType.Damage);

            }
            else
            {
                DungeonMaster.Instance.GetFloatTextCanvas().CreateDamagedText(hub.MyHealth, transform.position, message, type, isCritical);

            }
        }


        #endregion
    }
}




