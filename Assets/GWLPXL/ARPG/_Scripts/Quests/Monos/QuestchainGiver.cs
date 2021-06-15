﻿
using UnityEngine;
using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Types.com;
using GWLPXL.ARPGCore.Attributes.com;


namespace GWLPXL.ARPGCore.Quests.com
{
   

    [RequireComponent(typeof(IAttributeUser))]
    public class QuestchainGiver : MonoBehaviour, IInteract, IQuestGiver
    {
        [SerializeField]
        QuestGiverEvents questGiverEvents = new QuestGiverEvents();
        [SerializeField]
        GameObject questGiverCanvasPrefab = null;
        GameObject canvasInstance = null;
        [SerializeField]
        Questchain[] questchains = new Questchain[0];
        [SerializeField]
        float interactRange = 1f;
        [SerializeField]
        bool openQuesterCanvasUponAccept = true;

        int questChainIndex = 0;
        IActorHub currentQuester = null;
        IQuestGiverCanvas canvas = null;
        IAttributeUser user = null;
        private void Awake()
        {
            canvasInstance = Instantiate(questGiverCanvasPrefab);
            canvas = canvasInstance.GetComponent<IQuestGiverCanvas>();
            user = GetComponent<IAttributeUser>();
            canvas.SetUser(this);
            currentQuester = null;

        }
        //this is broke, sigh
        public void StartQuest()
        {
            currentQuester.MyQuests.GetQuestLogRuntime().UpdateChain(GetCurrentQuestchain(), QuestStatusType.InProgress);
            int chainIndex = GetCurrentQuestchainIndex(currentQuester.MyQuests);
            Quest template = GetCurrentQuestchain().GetQuests()[chainIndex];
            currentQuester.MyQuests.GetQuestLogRuntime().UpdateQuest(template, QuestStatusType.InProgress);

            OpenQuestDialogue(false, currentQuester);

            questGiverEvents.OnQuestchainStarted.Invoke(GetCurrentQuestchain());

           
        }
        public void SetCanvasPrefab(GameObject newPrefab) => questGiverCanvasPrefab = newPrefab;

        public void DeclineQuest()
        {
            OpenQuestDialogue(false, currentQuester);


        }

        public void Return()
        {
            OpenQuestDialogue(false, currentQuester);


        }
      

        public void CollectRewards()
        {

            int questIndex = currentQuester.MyQuests.GetQuestLogRuntime().GetQuestIndexInChain(GetCurrentQuestchain());
            Quest questTemplate = (GetCurrentQuestchain().GetQuests()[questIndex]);
            QuestState state = currentQuester.MyQuests.GetQuestLogRuntime().GetRuntimeQuestState(questTemplate);
            QuestState questchainstate = currentQuester.MyQuests.GetQuestLogRuntime().GetQuestchainState(GetCurrentQuestchain());
            if (state.State == QuestStatusType.Completed)
            {
                if (questTemplate.CanTurnIn(user))
                {
                    questTemplate.CollectRewards(currentQuester.MyQuests);
                    currentQuester.MyQuests.GetQuestLogRuntime().UpdateQuest(questTemplate, QuestStatusType.RewardsCollected);
                }
                    
            }

            else if (questchainstate.State == QuestStatusType.Completed)
            {
                if (GetCurrentQuestchain().CanTurnIn(user))
                {
                    GetCurrentQuestchain().CollectChainRewards(currentQuester.MyQuests);
                    currentQuester.MyQuests.GetQuestLogRuntime().UpdateChain(GetCurrentQuestchain(), QuestStatusType.RewardsCollected);
                    questGiverEvents.OnQuestChainRewardsCollected.Invoke(GetCurrentQuestchain());
                }
            }

            OpenQuestDialogue(false, currentQuester);
           

        }

        void OpenQuestDialogue(bool isOpen, IActorHub forUser)
        {
            if (isOpen)
            {
                currentQuester = forUser;
                currentQuester.MyQuests.SetQuestGiver(this);
                questGiverEvents.OnQuestDialogueOpen.Invoke();

            }
            else
            {
                currentQuester.MyQuests.SetQuestGiver(null);
                currentQuester = null;
                questGiverEvents.OnQuestDialogueClose.Invoke();

            }
            canvas.EnableCanvas(isOpen);

        }
        public bool DoInteraction(GameObject interactor)
        {
            IActorHub questUser = CheckPreconditions(interactor);
            if (questUser == null) return false;

            //open dialogue
            OpenQuestDialogue(true, questUser);

            SetupQuest();
            return true;
        }
        public bool IsInRange(GameObject interactor)
        {
            Vector3 dir = interactor.transform.position - this.transform.position;
            float sqrdMag = dir.sqrMagnitude;
            return (sqrdMag <= (interactRange * interactRange));
        }


        public void IncrementQuests()
        {
            questChainIndex += 1;
            if (questChainIndex > questchains.Length - 1)
            {
                questChainIndex = 0;
            }
            SetupQuest();
        }
        public void DecrementQuests()
        {
            questChainIndex -= 1;
            if (questChainIndex < 0)
            {
                questChainIndex = questchains.Length - 1;
            }
            SetupQuest();
        }



        IActorHub CheckPreconditions(GameObject interactor)
        {
            if (interactor == null) return null;
            IActorHub questUser = interactor.GetComponent<IActorHub>();
            return questUser;

           
        }


        public void AbandonQuest()
        {
            currentQuester.MyQuests.GetQuestLogRuntime().UpdateChain(GetCurrentQuestchain(), QuestStatusType.UnAvailable);
            OpenQuestDialogue(false, currentQuester);
            questGiverEvents.OnQuestChainAbandoned.Invoke(GetCurrentQuestchain());
        }
      
     

        void SetupQuest()
        {
            //reset canvas
            canvas.EnableAcceptQuestButtons(false);
            canvas.EnableTurnInButton(false);
            canvas.EnableInProgressButton(false);
            canvas.EnableResetQuestchainPanel(false);
            string description = "Default Chatty Script";

            int completed = 0;
            for (int i = 0; i < questchains.Length; i++)
            {
                QuestState chainState = currentQuester.MyQuests.GetQuestLogRuntime().GetRuntimeQuestChain(questchains[i]);
                if (chainState.State == QuestStatusType.RewardsCollected)
                {
                    completed += 1;
                }
            }

            if (questchains == null || questchains.Length == 0 || completed == questchains.Length)
            {
                //go to just dialogue mode. 
                description = "I have no quests for you";
                InProgressState();
                canvas.SetQuestText(description);
                canvas.EnableCanvas(true);
                return;
            }
            Questchain chain = GetCurrentQuestchain();

            //check preconditions and set available/un
            bool canStart = chain.CanStartQuestChain(currentQuester, user); 
           
            if (canStart == false)
            {
                description = chain.GetRequirementDescription();
                InProgressState();
            }
            else
            {
                //makes the chain available
                QuestState chainState = currentQuester.MyQuests.GetQuestLogRuntime().GetRuntimeQuestChain(chain);
                if (chainState.State == QuestStatusType.UnAvailable)
                {
                    currentQuester.MyQuests.GetQuestLogRuntime().UpdateChain(chain, QuestStatusType.Available);
                    chainState = currentQuester.MyQuests.GetQuestLogRuntime().GetRuntimeQuestChain(chain);
                }
                //makes it available

                int questIndex = currentQuester.MyQuests.GetQuestLogRuntime().GetQuestIndexInChain(chain);
                Quest template = GetCurrentQuestchain().GetQuests()[questIndex];
                QuestState queststate = currentQuester.MyQuests.GetQuestLogRuntime().GetRuntimeQuestState(template);
                QuestStatusType statusQuest = queststate.State;
                QuestStatusType questChainStatus = chainState.State;
                description = chain.GetQuestGiverText(questChainStatus, user);
                switch (questChainStatus)
                {
                    case QuestStatusType.Available:
                        AcceptState();
                        break;
                    case QuestStatusType.InProgress:
                        SetUIState(queststate.QuestTemplate, statusQuest);//ui buttons state
                        description = template.GetQuestGiverText(queststate.State, user);

                        if (statusQuest == QuestStatusType.Completed)
                        {
                            if (queststate.QuestTemplate.CanTurnIn(user) == false)
                            {
                                description = "What do I want with that? (Default text for not being able to turn in)";
                                break;
                            }

                        }
                        break;
                    case QuestStatusType.Completed:
                        TurnInState();
                        break;
                    case QuestStatusType.RewardsCollected:
                        if (chain.GetIsRepeatable())
                        {
                            ReseetQuestChainState();
                        }
                        else
                        {
                            InProgressState();
                        }
                     
                        break;
                }

            }


            //need to know do the text before the chain, the text after. Also if I want chain rewards. 

            canvas.SetQuestText(description);
            canvas.EnableCanvas(true);
        }

        void ReseetQuestChainState()
        {
            canvas.EnableResetQuestchainPanel(true);
        }
        void TurnInState()
        {
            canvas.EnableTurnInButton(true);
        }
        void AcceptState()
        {
            canvas.EnableAcceptQuestButtons(true);
        }
        void InProgressState()
        {
            canvas.EnableInProgressButton(true);
        }
        private void SetUIState(Quest template, QuestStatusType statusQuest)
        {
            switch (statusQuest)
            {
                case QuestStatusType.Available:
                    //canvas.EnableAcceptQuestButtons(true);
                    AcceptState();
                    break;
                case QuestStatusType.InProgress:
                   // canvas.EnableInProgressButton(true);
                    InProgressState();
                    break;
                case QuestStatusType.Completed:
                    if (template.CanTurnIn(user))
                    {
                   //     canvas.EnableTurnInButton(true);
                        TurnInState();
                    }
                    else
                    {
                    //    canvas.EnableInProgressButton(true);
                        InProgressState();
                    }
                    break;
                case QuestStatusType.RewardsCollected:
                  //  canvas.EnableInProgressButton(true);
                    InProgressState();
                    break;
            }

        }


      
        public Questchain GetCurrentQuestchain()
        {
            return questchains[questChainIndex];

        }
        public int GetCurrentQuestchainIndex(IQuestUser questUser)
        {
            return questUser.GetQuestLogRuntime().GetQuestIndexInChain(GetCurrentQuestchain());
     

        }

        public void ResetQuestChain()
        {
            currentQuester.MyQuests.GetQuestLogRuntime().ResetQuestChain(GetCurrentQuestchain());

            OpenQuestDialogue(false, currentQuester);
        }

        public Transform GetInstance() => this.transform;
     
    }
}