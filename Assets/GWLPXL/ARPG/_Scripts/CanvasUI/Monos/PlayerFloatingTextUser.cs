
using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Types.com;
using UnityEngine;

namespace GWLPXL.ARPGCore.CanvasUI.com
{
    /// <summary>
    /// class that sends information to the floating text canvas
    /// </summary>

    public class PlayerFloatingTextUser : MonoBehaviour, IUseFloatingText
    {

        protected Vector3 hPBarOffset = new Vector3(0, 2, 0);//change the y value to move the hp bar up and down
        protected IActorHub hub;

        #region public interface

        public void CreateUIDamageText(string message, ElementType type, bool isCritical)
        {
            DefaultDamageText(message, type, isCritical);
        }

        public Vector3 GetHPBarOffset()
        {
            return hPBarOffset;
        }
        public void SetActorHub(IActorHub newhub)
        {
            hub = newhub;
        }


        public void CreateUIRegenText(string message, ResourceType type)
        {
            DefaultRegenText(message, type);
        }

        

        public void CreateUIDoTText(string message, ElementType type)
        {
            DefaultDoTText(message, type);

        }
        #endregion

        #region protected virtual
        protected virtual void DefaultDoTText(string message, ElementType type)
        {
            DungeonMaster.Instance.GetFloatTextCanvas().CreateDoTText(hub.MyHealth, message, transform.position + GetHPBarOffset(), type);
        }

        protected virtual void DefaultRegenText(string message, ResourceType type)
        {
            DungeonMaster.Instance.GetFloatTextCanvas().CreateRegenText(hub.MyHealth, message, transform.position + GetHPBarOffset(), type);
        }

        protected virtual void DefaultDamageText(string message, ElementType type, bool isCritical)
        {
            DungeonMaster.Instance.GetFloatTextCanvas().CreateDamagedText(hub.MyHealth, transform.position, message, type, isCritical);
        }
        #endregion
    }
}