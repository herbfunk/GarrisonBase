using System.Threading.Tasks;
using Styx;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorTransport : Behavior
    {
        private readonly int TransportId;
        private readonly WoWPoint StartPoint, EndPoint, WaitPoint, StandPoint, GetOffPoint;
        private readonly Movement.MovementTypes MoveType;
        private readonly string DestinationName;

        public BehaviorTransport(int transportId,
			WoWPoint startLocation,
			WoWPoint endLocation,
			WoWPoint waitAtLocation,
			WoWPoint standAtLocation,
			WoWPoint getOffLocation,
			Movement.MovementTypes movementType,
			string destination = null)
        {
            TransportId = transportId;
            StartPoint = startLocation;
            EndPoint = endLocation;
            WaitPoint = waitAtLocation;
            StandPoint = standAtLocation;
            GetOffPoint = getOffLocation;
            MoveType = movementType;
            DestinationName = destination;
        }

        public override async Task<bool> BehaviorRoutine()
        {
            return
                await
                    Movement.UseTransport(TransportId, StartPoint, EndPoint, WaitPoint, StandPoint, GetOffPoint,
                        MoveType, DestinationName);

        }
    }
}
