using GWLPXL.ARPGCore.Combat.com;
using GWLPXL.ARPGCore.Types.com;
using UnityEngine;

namespace GWLPXL.ARPGCore.CanvasUI.com
{
    public interface IFloatTextCanvas
    {
        void CreateDamagedText(IReceiveDamage damageTaker, Vector3 position, string text, ElementType type);
        void CreateRegenText(IReceiveDamage damageTaker, string text, Vector3 position, ResourceType type);
        void CreateDoTText(IReceiveDamage damageTaker, string text, Vector3 position, ElementType type);

    }
}