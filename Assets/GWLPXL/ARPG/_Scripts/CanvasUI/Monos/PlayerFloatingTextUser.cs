
using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Types.com;
using UnityEngine;

namespace GWLPXL.ARPGCore.CanvasUI.com
{
    /// <summary>
    /// will change in the future, works for now
    /// </summary>

    public class PlayerFloatingTextUser : MonoBehaviour, IUseFloatingText
    {

        Vector3 hPBarOffset = new Vector3(0, 2, 0);//change the y value to move the hp bar up and down
        IActorHub hub;
     

        public void CreateUIDamageText(string message, ElementType type)
        {
            DungeonMaster.Instance.GetFloatTextCanvas().CreateDamagedText(hub.MyHealth, transform.position, message, type);
        }

        public Vector3 GetHPBarOffset()
        {
            return hPBarOffset;
        }

   

        public void CreateUIRegenText(string message, ResourceType type)
        {
            DungeonMaster.Instance.GetFloatTextCanvas().CreateRegenText(hub.MyHealth, message, transform.position + GetHPBarOffset(), type);
        }

        public void CreateUIDoTText(string message, ElementType type)
        {
            DungeonMaster.Instance.GetFloatTextCanvas().CreateDoTText(hub.MyHealth, message, transform.position + GetHPBarOffset(), type);

        }

        public void SetActorHub(IActorHub newhub)
        {
            hub = newhub;
        }
    }
}