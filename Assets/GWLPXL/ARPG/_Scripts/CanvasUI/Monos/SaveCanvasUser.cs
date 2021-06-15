
using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Saving.com;
using UnityEngine;

namespace GWLPXL.ARPGCore.CanvasUI.com
{

    //saving will not work without a dungeon singleton
    //

    public class SaveCanvasUser : MonoBehaviour, IUseSaveCanvas, IUseCanvas
    {
        [SerializeField]
        bool freezeMover = true;
        [SerializeField]
        GameObject SaveCanvasPrefab = null;

        ISaveCanvas saveC = null;
        IActorHub actorhub = null;
        public void SetUserToCanvas()
        {
            if (SaveCanvasPrefab == null) return;

            GameObject newCanvas = Instantiate(SaveCanvasPrefab, transform);
            saveC = newCanvas.GetComponent<ISaveCanvas>();
            if (saveC != null)
            {
                saveC.SetUser(this);
            }


        }

        public void ToggleCanvas()
        {
            saveC.TogglePlayerSaveCanvas();
        }

        public bool GetCanvasEnabled()
        {
            if (saveC == null) return false;
            return saveC.GetCanvasEnabled();
        }


        public void SetCanvasPrefab(GameObject newprefab) => SaveCanvasPrefab = newprefab;

        public ISaveCanvas GetUI() => saveC;

        public bool GetFreezeMover() => freezeMover;

        public void SetActorHub(IActorHub hub) => actorhub = hub;
        
    }
}