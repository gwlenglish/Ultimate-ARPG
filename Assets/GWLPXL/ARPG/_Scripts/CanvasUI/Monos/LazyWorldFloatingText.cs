using GWLPXL.ARPGCore.CanvasUI.com;
using GWLPXL.ARPGCore.Combat.com;
using GWLPXL.ARPGCore.Types.com;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ARPGCore.CanvasUI.com
{


    public class LazyWorldFloatingText : MonoBehaviour, IFloatTextCanvas
    {
        public GameObject DamageTextPrefab = default;
        public GameObject DoTTextPrefab = default;
        public GameObject RegenTextPrefab = default;
        public void CreateDamagedText(IReceiveDamage damageTaker, Vector3 position, string text, ElementType type, bool isCritical = false)
        {
            GameObject newtext = Instantiate(DamageTextPrefab);
            newtext.transform.position = position;
            Rigidbody rb = newtext.AddComponent<Rigidbody>();
            rb.AddForce(Vector3.up * Random.Range(10, 15), ForceMode.Impulse);
            Destroy(newtext, 3);
        }

        public void CreateDoTText(IReceiveDamage damageTaker, string text, Vector3 position, ElementType type, bool isCritical = false)
        {
            GameObject newtext = Instantiate(DoTTextPrefab);
            newtext.transform.position = position;
        }

        public void CreateNewFloatingText(IReceiveDamage damageTaker, ElementUI variables, Vector3 atPosition, string text, FloatingTextType type, bool isCritical = false)
        {
            switch (type)
            {
                case FloatingTextType.Damage:
                    CreateDamagedText(damageTaker, atPosition, text, variables.Type, isCritical);
                    break;
                case FloatingTextType.DoTs:
                    CreateDoTText(damageTaker, text, atPosition, variables.Type, isCritical);
                    break;
                case FloatingTextType.Regen:
                    CreateRegenText(damageTaker, text, atPosition, ResourceType.Health, isCritical);
                    break;
            }
        }

        public void CreateRegenText(IReceiveDamage damageTaker, string text, Vector3 position, ResourceType type, bool isCritical = false)
        {
            GameObject newtext = Instantiate(RegenTextPrefab);
            newtext.transform.position = position;
        }


    }
}