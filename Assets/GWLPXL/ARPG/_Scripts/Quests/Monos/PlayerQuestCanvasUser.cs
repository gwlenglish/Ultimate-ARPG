
using UnityEngine;
using GWLPXL.ARPGCore.CanvasUI.com;
using GWLPXL.ARPGCore.com;

namespace GWLPXL.ARPGCore.Quests.com
{

    /// <summary>
    /// Link between quester and the canvas UI
    /// </summary>
    public class PlayerQuestCanvasUser : MonoBehaviour, IUseCanvas, IUseQuesterCanvas
    {
        [SerializeField]
        bool freezeMover = false;

        [SerializeField]
        GameObject questCanvasPrefab = null;
        GameObject questCanvasInstance = null;

        IQuesterCanvas questCanvas = null;
        IActorHub actorhub = null;
       
        public bool GetCanvasEnabled()
        {
            if (questCanvas == null) return false;
            return questCanvas.GetCanvasEnabled();
        }

        public IQuestUser GetQuester()
        {
            return actorhub.MyQuests;
        }

        public IQuesterCanvas GetQuesterUI()
        {
            return questCanvas;
        }

        public void SetUserToCanvas()
        {
            if (questCanvasInstance != null)
            {
                Destroy(questCanvasInstance);
            }
            if (questCanvasPrefab == null) return;

            questCanvasInstance = Instantiate(questCanvasPrefab, transform);
            questCanvas = questCanvasInstance.GetComponent<IQuesterCanvas>();
            questCanvas.SetUser(this);

        }

        public void ToggleCanvas()
        {
            GetQuesterUI().ToggleQuesterUI();
        }

        public bool GetFreezeMover()
        {
            return freezeMover;
        }

        public void SetPrefabCanvas(GameObject newprefab) => questCanvasPrefab = newprefab;

        public void SetActorHub(IActorHub hub) => actorhub = hub;
      
    }
}