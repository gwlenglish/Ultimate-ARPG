
using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Items.com;

using UnityEngine;
namespace GWLPXL.ARPGCore.CanvasUI.com
{

    public class PlayerInvCanvasUser : MonoBehaviour, IUseInvCanvas, IUseCanvas
    {

        [SerializeField]
        bool freezeMover = true;
        [SerializeField]
        GameObject InvCanvasPrefab = null;

        IInventoryCanvas canvas = null;
        IActorHub actorHub = null;
        private void Awake()
        {
            

        }
        public IActorHub GetActorHub() => actorHub;
     

        public void SetUserToCanvas()
        {
            if (InvCanvasPrefab == null) return;
            GameObject newCanvas = Instantiate(InvCanvasPrefab.gameObject, transform);
            IInventoryCanvas instance = newCanvas.GetComponent<IInventoryCanvas>();
            instance.SetUser(this);
            canvas = instance;
        }

        //
        public void ToggleCanvas()
        {
            canvas.TogglePlayerInventoryUI();
        }

        public IInventoryUser GetUser()
        {
            return actorHub.MyInventory;
        }

        public IInventoryCanvas GetInvUI()
        {
            return canvas;
        }

        public bool GetCanvasEnabled()
        {
            if (canvas == null) return false;
            return canvas.GetCanvasEnabled();
        }



        public void EnableCanvas()
        {
            if (GetInvUI() == null) return;
            GetInvUI().EnablePlayerInventoryUI(true);
        }

        public void DisableCanvas()
        {
            if (GetInvUI() == null) return;

            GetInvUI().EnablePlayerInventoryUI(false);

        }

        public void SetCanvasPrefab(GameObject newprefab) => InvCanvasPrefab = newprefab;

        public bool GetFreezeMover() => freezeMover;

        public void SetActorHub(IActorHub hub)
        {
            actorHub = hub;
        }
    }
}
