

using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Types.com;
using UnityEngine;
namespace GWLPXL.ARPGCore.CanvasUI.com
{
    public interface IUseFloatingText
    {

        Vector3 GetHPBarOffset();
        void CreateUIDamageText(string message, ElementType type);
        void CreateUIDoTText(string message, ElementType type);
        void CreateUIRegenText(string message, ResourceType type);
        void SetActorHub(IActorHub newhub);

    }
}
