using System;
using System.Threading.Tasks;
using Herbfunk.GarrisonBase.Garrison.Objects;
using Herbfunk.GarrisonBase.Quest;
using Styx.CommonBot;
using Styx.CommonBot.Coroutines;
using Styx.CommonBot.Frames;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorQuestWorkOrder : Behavior
    {
            
        public readonly uint QuestID;
        public readonly Building Building;
        public BehaviorQuestWorkOrder(Building building)
            : base(building.SafeMovementPoint, building.WorkOrderNPCEntryId)
        {
            Building = building;
            QuestID = building.FirstQuestID;
        }

        public override void Initalize()
        {
            _npcMovement = null;
            base.Initalize();
        }

        private Movement _npcMovement;
        public override async Task<bool> BehaviorRoutine()
        {
            if (await base.BehaviorRoutine()) return true;
            if (IsDone) return false;

            if (!QuestManager.QuestContainedInQuestLog(QuestID) || QuestManager.GetQuestFromQuestLog(QuestID).IsCompleted)
                return false;

            if (Building.WorkOrder == null || Building.WorkOrder.TotalWorkorderStartups() == 0)
                return false;

            if (await StartMovement.MoveTo())
                return true;
                
            if (LuaEvents.GossipFrameOpen)
            {
                GossipFrame.Instance.SelectGossipOption(0);
                await CommonCoroutines.SleepForRandomUiInteractionTime();
                return true;
            }

            if (LuaCommands.IsGarrisonCapacitiveDisplayFrame())
            {
                if (!LuaCommands.ClickStartOrderButtonEnabled())
                {
                    Building.CheckedWorkOrderStartUp = true;
                    GarrisonBase.Log("Order Button Disabled!");
                    return false;
                }

                   
                LuaCommands.ClickStartOrderButton();
                await CommonCoroutines.SleepForRandomUiInteractionTime();
                await CommonCoroutines.SleepForLagDuration();
                return true;
            }

            if (InteractionObject != null && InteractionObject.IsValid)
            {
                if (InteractionObject.WithinInteractRange)
                {
                    TreeRoot.StatusText = String.Format("Behavior {0} Quest NPC Interact", Type.ToString());
                    InteractionObject.Interact();
                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                    return true;
                }

                if (_npcMovement == null)
                    _npcMovement = new Movement(InteractionObject.Location, InteractionObject.InteractRange - 0.25f);

                await _npcMovement.ClickToMove(false);
                return true;
            }

            return false;
        }

    }
}