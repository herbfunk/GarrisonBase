using System;
using System.Threading.Tasks;
using Herbfunk.GarrisonBase.Garrison.Objects;
using Herbfunk.GarrisonBase.Helpers;
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
            : base(building.SafeMovementPoint, building.WorkOrderNpcEntryId)
        {
            Building = building;
            QuestID = building.FirstQuestId;
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

            if (!QuestHelper.QuestContainedInQuestLog(QuestID) || QuestHelper.GetQuestFromQuestLog(QuestID).IsCompleted)
                return false;

            if (Building.WorkOrder == null || Building.WorkOrder.TotalWorkorderStartups() == 0)
                return false;

            if (await StartMovement.MoveTo())
                return true;

            if (GossipHelper.IsOpen)
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