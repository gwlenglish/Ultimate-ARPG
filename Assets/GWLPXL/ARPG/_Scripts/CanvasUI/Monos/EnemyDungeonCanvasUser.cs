

using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Combat.com;
using GWLPXL.ARPGCore.Types.com;
using UnityEngine;

namespace GWLPXL.ARPGCore.CanvasUI.com
{

    public class EnemyDungeonCanvasUser : MonoBehaviour, IUseFloatingText
    {
        [SerializeField]
        [Tooltip("Adjusts the start position of the floating text.")]
        Vector3 floatingTextOffset = new Vector3(0, 2, 0);//change the y value to move the hp bar up and down
        IActorHub hub;

   


        public Vector3 GetHPBarOffset()
        {
            return floatingTextOffset;
        }


        public void CreateUIDamageText(string message, ElementType type)
        {
            DungeonMaster.Instance.GetFloatTextCanvas().CreateDamagedText(hub.MyHealth, transform.position + floatingTextOffset, message, type);
        }


       

        public void CreateUIRegenText(string message, ResourceType type)
        {
            DungeonMaster.Instance.GetFloatTextCanvas().CreateRegenText(hub.MyHealth, message, transform.position + floatingTextOffset, type);
        }

        public void CreateUIDoTText(string message, ElementType type)
        {
            DungeonMaster.Instance.GetFloatTextCanvas().CreateDoTText(hub.MyHealth, message, transform.position + floatingTextOffset, type);
        }

        public void SetActorHub(IActorHub newhub)
        {
            hub = newhub;
        }
    }
}




