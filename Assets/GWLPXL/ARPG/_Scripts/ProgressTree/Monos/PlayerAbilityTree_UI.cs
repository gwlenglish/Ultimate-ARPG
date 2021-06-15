using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.DebugHelpers.com;

using UnityEngine;


namespace GWLPXL.ARPGCore.ProgressTree.com
{
    public class PlayerAbilityTree_UI : MonoBehaviour, IProgressTree
    {
        public bool FreezeDungeon = true;
        [SerializeField]
        GameObject mainPanel = null;
        [SerializeField]
        ProgressTreeHolder abilityTree = null;

        [SerializeField]
        AbilityUnlocks[] abilityUnlocks = new AbilityUnlocks[0];
        IUseAbilityTreeCanvas myUser = null;

        
        public void EnableUI(bool isEnabled)
        {
            mainPanel.SetActive(isEnabled);
        }

        private void OnEnable()
        {
            GetComponent<Canvas>().worldCamera = Camera.main;//set screen space camera
            GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        }
        public void SetUser(IUseAbilityTreeCanvas newUser)
        {

            if (myUser == null)
            {
                //first time
                for (int i = 0; i < abilityUnlocks.Length; i++)
                {
                    abilityUnlocks[i].AbilityUser = newUser.GetUser();
                    TreeNodeHolder runtime = abilityTree.GetRuntimeNode(abilityUnlocks[i].NodeUnlock);
                    runtime.Available += ModifySkillToUser;
                }
            }

            else if (myUser != null && myUser != newUser)
            {
                //replacing user
                for (int i = 0; i < abilityUnlocks.Length; i++)
                {
                    TreeNodeHolder runtime = abilityTree.GetRuntimeNode(abilityUnlocks[i].NodeUnlock);
                    runtime.Available -= ModifySkillToUser;
                }

                for (int i = 0; i < abilityUnlocks.Length; i++)
                {
                    abilityUnlocks[i].AbilityUser = newUser.GetUser();
                    TreeNodeHolder runtime = abilityTree.GetRuntimeNode(abilityUnlocks[i].NodeUnlock);
                    runtime.Available += ModifySkillToUser;
                }

            }


            myUser = newUser;

        }

        private void OnDestroy()
        {
            for (int i = 0; i < abilityUnlocks.Length; i++)
            {
                TreeNodeHolder runtime = abilityTree.GetRuntimeNode(abilityUnlocks[i].NodeUnlock);
                if (runtime == null) continue;
                runtime.Available -= ModifySkillToUser;
            }
        }


        void ModifySkillToUser(TreeNodeHolder nodeKey, bool isAvailable)
        {
            //ARPGDebugger.DebugMessage("Raised event", this);
            for (int i = 0; i < abilityUnlocks.Length; i++)
            {
                TreeNodeHolder runtime = abilityTree.GetRuntimeNode(abilityUnlocks[i].NodeUnlock);
                if (runtime == nodeKey)
                {
                    //ARPGDebugger.DebugMessage("found nodekey", this);

                    if (isAvailable)
                    {
                        //ARPGDebugger.DebugMessage("added skill", this);

                        myUser.GetUser().GetRuntimeController().LearnAbility(abilityUnlocks[i].AbilityToUnlock);
                    }
                    else
                    {
                        //ARPGDebugger.DebugMessage("removed skill", this);
                        myUser.GetUser().GetRuntimeController().ForgetAbility(abilityUnlocks[i].AbilityToUnlock);

                    }

                    break;
                }
            }
        }
        public void ToggleUI()
        {
            EnableUI(!mainPanel.activeInHierarchy);
            if (FreezeDungeon && mainPanel.activeInHierarchy)
            {
                DungeonMaster.Instance.GetDungeonUISceneRef().SetDungeonState(0);
            }
            else if (FreezeDungeon && !mainPanel.activeInHierarchy)
            {
                DungeonMaster.Instance.GetDungeonUISceneRef().SetDungeonState(1);

            }
        }

        public bool GetEnabled()
        {
            return mainPanel.activeInHierarchy;
        }
    }
}