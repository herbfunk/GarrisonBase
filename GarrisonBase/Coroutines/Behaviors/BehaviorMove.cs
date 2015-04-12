using System.Threading.Tasks;
using Styx;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorMove : Behavior
    {
        public override BehaviorType Type { get { return BehaviorType.MoveTo; } }
        public Movement.MovementTypes MovementType;

        public BehaviorMove(WoWPoint loc, float distance = 10f, Movement.MovementTypes type = Movement.MovementTypes.Normal, bool stuckCheck=false)
            : this(new[] { loc }, distance, type, stuckCheck)
        {

        }
        public BehaviorMove(WoWPoint[] locs, float distance = 10f, Movement.MovementTypes type=Movement.MovementTypes.Normal, bool stuckCheck=false)
        {
            _stuckCheck = stuckCheck;
            _movementPoints = locs;
            _distance = distance;
            MovementType = type;
        }

        public override void Initalize()
        {
            _movement = new Movement(_movementPoints, _distance, false, "BehaviorMove", _stuckCheck);
            base.Initalize();
        }

        private bool _stuckCheck;
        private readonly WoWPoint[] _movementPoints;
        private readonly float _distance;
        private Movement _movement;

        public override async Task<bool> BehaviorRoutine()
        {
            if (await base.BehaviorRoutine()) return true;
            if (IsDone) return false;
            
            if (MovementType== Movement.MovementTypes.Normal)
            {
                if (await _movement.MoveTo())
                {
                    return true;
                }
            }
            else if (MovementType == Movement.MovementTypes.ClickToMove)
            {
                if (await _movement.ClickToMove())
                {
                    return true;
                }
            }

            IsDone = true;
            return false;
        }
    }
}