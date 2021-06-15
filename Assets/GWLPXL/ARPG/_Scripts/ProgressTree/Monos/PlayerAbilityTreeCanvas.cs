using GWLPXL.ARPGCore.Abilities.com;
using GWLPXL.ARPGCore.CanvasUI.com;
using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.PlayerInput.com;
using UnityEngine;

namespace GWLPXL.ARPGCore.ProgressTree.com
{
    public interface IUseAbilityTreeCanvas
    {
        void ToggleCanvas();
        IAbilityUser GetUser();
        IProgressTree GetInvUI();
        void SetCanvasPrefab(GameObject newPrefab);
        bool GetFreezeMover();
    }
        

    public class PlayerAbilityTreeCanvas : MonoBehaviour, IUseCanvas, IUseAbilityTreeCanvas
    {

        [SerializeField]
        bool freezeMover = true;
        [SerializeField]
        GameObject InvCanvasPrefab;

        IProgressTree ui = null;
        IActorHub actorhub = null;
        GameObject newCanvas = null;
       

       

        public void SetUserToCanvas()
        {
            if (InvCanvasPrefab == null) return;
            newCanvas = Instantiate(InvCanvasPrefab.gameObject, transform);
            ui = newCanvas.GetComponent<IProgressTree>();
            ui.SetUser(this);
            ui.EnableUI(false);
            //canvas = instance;
        }

      
        public void ToggleCanvas()
        {
            GetInvUI().ToggleUI();
        }

        public IAbilityUser GetUser()
        {
            return actorhub.MyAbilities;
        }

        public IProgressTree GetInvUI()
        {
            return ui;
        }

        public bool GetCanvasEnabled()
        {
            if (ui == null) return false;
            return ui.GetEnabled();
        }

      

        public void SetCanvasPrefab(GameObject newPrefab) => InvCanvasPrefab = newPrefab;

        public bool GetFreezeMover() => freezeMover;

        public void SetActorHub(IActorHub hub) => actorhub = hub;
       
    }
}
