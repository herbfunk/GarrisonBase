using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache.Objects;
using Herbfunk.GarrisonBase.Character;
using Herbfunk.GarrisonBase.Coroutines.Behaviors;
using Herbfunk.GarrisonBase.Helpers;
using Styx;
using Styx.Common;
using Styx.Common.Helpers;
using Styx.CommonBot;
using Styx.CommonBot.Coroutines;
using Styx.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace Herbfunk.GarrisonBase.Coroutines
{
    public class Movement
    {
        public static bool IgnoreTaxiCheck
        {
            get { return _ignoreTaxiCheck; }
            set
            {
                GarrisonBase.Debug("Movement Ignore Taxi Check set to {0}", value);
                _ignoreTaxiCheck = value;
            }
        }
        private static bool _ignoreTaxiCheck;

        
        public enum MovementTypes
        {
            Normal,
            ClickToMove,
        }

        internal Queue<WoWPoint> CurrentMovementQueue = new Queue<WoWPoint>();
        internal Queue<WoWPoint> DequeuedPoints = new Queue<WoWPoint>(); 
        internal List<WoWPoint> DequeuedFinalPlayerPositionPoints = new List<WoWPoint>();
        internal C_WoWObject WoWObject = null;
        public int CanNavigateFailures = 0;
        private bool _checkedShoulUseFlightPath;
        private bool _didResetStuckChecker = false;
        private bool _checkStuck;
        public float Distance { get; set; }

        

        public readonly string Name;

        public Movement(WoWPoint location, float distance, string name = "", bool checkStuck = false)
            : this(new[] { location }, distance, name: name, checkStuck: checkStuck)
        {
        }

        public Movement(WoWPoint[] locations, string name = "", bool checkStuck = false)
            : this(locations, 5f, name: name, checkStuck: checkStuck)
        {
        }

        public Movement(C_WoWObject obj, float distance, string name = "", bool checkStuck = false)
            : this(new[] {obj.Location}, distance, name, checkStuck)
        {
            WoWObject = obj;
        }

        public Movement(WoWPoint[] locations, float distance, string name = "", bool checkStuck = false)
        {
            Name = name;
            _checkStuck = checkStuck;
            Distance = distance;
            foreach (var p in locations)
            {
                CurrentMovementQueue.Enqueue(p);
            }
            
        }

        private WoWPoint CurrentLocation
        {
            get
            {
                if (WoWObject != null) return WoWObject.Location;
                return CurrentMovementQueue.Count > 0 ? CurrentMovementQueue.Peek() : WoWPoint.Zero;
            }
        }

        public async Task<bool> MoveTo(bool allowDequeue=true)
        {
            if (CurrentMovementQueue.Count == 0)
            {
                return false;
            }

            WoWPoint location = CurrentLocation;
            WoWPoint playerPos = Player.Location;
            float currentDistance = location.Distance(playerPos);
            if (currentDistance <= Distance)
            {
                if (allowDequeue)
                {
                    Log("MoveTo", String.Format("has dequeued location - {0}", location.ToString()));

                    DequeuedPoints.Enqueue(CurrentMovementQueue.Dequeue());
                    DequeuedFinalPlayerPositionPoints.Add(playerPos);
                }

                if (CurrentMovementQueue.Count == 0 || CurrentMovementQueue.Count==1 && !allowDequeue)
                {
                    Log("MoveTo", "is finished");
                    WoWMovement.MoveStop();
                    return false;
                }
                
                return true;
            }

            if (!IgnoreTaxiCheck && !_checkedShoulUseFlightPath)
            {
                _checkedShoulUseFlightPath = true;
                if (TaxiFlightHelper.ShouldTakeFlightPath(location))
                {
                    if (BehaviorManager.SwitchBehaviors.All(b => b.Type != BehaviorType.Taxi))
                    {
                        BehaviorManager.SwitchBehaviors.Add(new BehaviorUseFlightPath(location));
                    }
                    
                    return true;
                }
            }

            if (!_didResetStuckChecker)
            {
                StuckChecker.Reset();
                _didResetStuckChecker = true;
            }
            else if (_checkStuck)
            {
                if (StuckChecker.CheckStuck())
                {
                    Log("MoveTo", "Stuck Checker returned true!");
                    return false;
                }
            }


            if (!CheckCanNavigate())
            {
                Log("MoveTo", "Can Navigate Return False " + location.ToString());
                return false;
            }
           
            //if (StyxWoW.Me.IsMoving)
            //    return true;

            var moveresult = MoveResult.Moved;

            try
            {
                moveresult = await CommonCoroutines.MoveTo(location);
            }
            catch (Exception ex)
            {
                Navigator.Clear();
                Log("MoveTo", "Exception during movement attempt! " + location.ToString());
               
                try
                {
                    Navigator.MoveTo(location);
                }
                catch
                {
                    Log("MoveTo", "Double Exception during movement attempt!! " + location.ToString());
                    return false;
                }
            }
 
            
            //Navigator.GetRunStatusFromMoveResult(moveresult);
            switch (moveresult)
            {
                case MoveResult.UnstuckAttempt:
                    Log("MoveTo", "MoveResult: UnstuckAttempt " + location.ToString());
                    await Buddy.Coroutines.Coroutine.Sleep(500);
                    break;

                case MoveResult.Failed:
                    Log("MoveTo", "MoveResult: Failed " + location.ToString());
                    return false;

                case MoveResult.ReachedDestination:
                    Log("MoveTo", "MoveResult: ReachedDestination " + location.ToString());
                    return true;
            }

            if (MovementCache.ShouldRecord)
                MovementCache.AddPosition(playerPos, Distance);

            return true;
        }

        public async Task<MoveResult> MoveTo_Result(bool allowDequeue=true)
        {
            if (CurrentMovementQueue.Count == 0)
                return MoveResult.ReachedDestination;

            WoWPoint location = CurrentLocation;
            WoWPoint playerPos = Character.Player.Location;
            float currentDistance = location.Distance(playerPos);
            if (currentDistance <= Distance)
            {
                if (allowDequeue)
                {
                    Log("MoveToResult", String.Format("has dequeued location - {0}", location.ToString()));

                    DequeuedPoints.Enqueue(CurrentMovementQueue.Dequeue());
                    DequeuedFinalPlayerPositionPoints.Add(playerPos);
                }

                if (CurrentMovementQueue.Count == 0 || CurrentMovementQueue.Count == 1 && !allowDequeue)
                {
                    Log("MoveToResult", "is finished");
                    WoWMovement.MoveStop();
                    return MoveResult.ReachedDestination;
                }

                return MoveResult.Moved;
            }

            if (!_didResetStuckChecker)
            {
                StuckChecker.Reset();
                _didResetStuckChecker = true;
            }
            else if (_checkStuck)
            {
                if (StuckChecker.CheckStuck())
                {
                    Log("MoveToResult", "Stuck Checker returned true!");
                    return MoveResult.Failed;
                }
            }

            if (!CheckCanNavigate())
            {
                Log("MoveToResult", "Can Navigate Return False " + location.ToString());
                return MoveResult.Failed;
            }


            var moveresult = MoveResult.Moved;

            try
            {
                moveresult = await CommonCoroutines.MoveTo(location);
            }
            catch (Exception ex)
            {
                Navigator.Clear();
                Log("MoveToResult", "Exception during movement attempt!! " + location.ToString());
                try
                {
                    Navigator.MoveTo(location);
                }
                catch
                {
                    Log("MoveToResult", "Double Exception during movement attempt!! " + location.ToString());
                    return MoveResult.Failed;
                }
            }

            if (MovementCache.ShouldRecord)
                MovementCache.AddPosition(playerPos, Distance);

            //GarrisonBase.Log("[MoveTo] MoveResult: {0}", moveresult.ToString());
            return moveresult;
        }

        public async Task<MoveResult> ClickToMove_Result(bool allowDequeue = true)
        {
            if (CurrentMovementQueue.Count == 0)
                return MoveResult.ReachedDestination;

            WoWPoint location = CurrentLocation;
            WoWPoint playerPos = Character.Player.Location;
            float currentDistance = location.Distance(playerPos);
            if (currentDistance <= Distance)
            {
                if (allowDequeue)
                {
                    Log("ClickToMoveResult", String.Format("has dequeued location - {0}", location.ToString()));

                    DequeuedPoints.Enqueue(CurrentMovementQueue.Dequeue());
                    DequeuedFinalPlayerPositionPoints.Add(playerPos);
                }

                if (CurrentMovementQueue.Count == 0 || CurrentMovementQueue.Count == 1 && !allowDequeue)
                {
                    Log("ClickToMoveResult", "is finished");
                    WoWMovement.MoveStop();
                    return MoveResult.ReachedDestination;
                }

                return MoveResult.ReachedDestination;
            }

            if (!WoWMovement.ClickToMoveInfo.IsClickMoving)
            {
                Log("ClickToMoveResult", location.ToString());
                WoWMovement.ClickToMove(location);
                await Coroutine.Sleep(StyxWoW.Random.Next(525, 800));
            }

            if (!_didResetStuckChecker)
            {
                StuckChecker.Reset();
                _didResetStuckChecker = true;
            }
            else if (_checkStuck)
            {
                if (StuckChecker.CheckStuck())
                {
                    Log("ClickToMoveResult", "Stuck Checker returned true!");
                    return MoveResult.Failed;
                }
            }

            return MoveResult.Moved;
        }

        public async Task<bool> ClickToMove(bool allowDequeue = true)
        {
            if (CurrentMovementQueue.Count == 0)
                return false;

            WoWPoint location = CurrentLocation;
            WoWPoint playerPos = Character.Player.Location;
            float currentDistance = location.Distance(playerPos);
            if (currentDistance <= Distance)
            {
                if (allowDequeue)
                {
                    Log("ClickToMove", String.Format("has dequeued location - {0}", location.ToString()));

                    DequeuedPoints.Enqueue(CurrentMovementQueue.Dequeue());
                    DequeuedFinalPlayerPositionPoints.Add(playerPos);
                }

                if (CurrentMovementQueue.Count == 0 || CurrentMovementQueue.Count == 1 && !allowDequeue)
                {
                    Log("ClickToMove", "is finished");
                    WoWMovement.MoveStop();
                    return false;
                }

                return true;
            }

            if (!WoWMovement.ClickToMoveInfo.IsClickMoving)
            {
                Log("ClickToMove", location.ToString());
                WoWMovement.ClickToMove(location);
               // await Coroutine.Sleep(StyxWoW.Random.Next(525, 800));
            }

            if (!_didResetStuckChecker)
            {
                StuckChecker.Reset();
                _didResetStuckChecker = true;
            }
            else if (_checkStuck)
            {
                if (StuckChecker.CheckStuck())
                {
                    Log("ClickToMoveResult", "Stuck Checker returned true!");
                    return false;
                }
            }

            if (MovementCache.ShouldRecord)
                MovementCache.AddPosition(playerPos, Distance);

            return true;
        }

        private bool CheckCanNavigate()
        {
            bool canNavigate = true;
            try
            {
                canNavigate = Navigator.CanNavigateWithin(Player.Location, CurrentLocation, Distance);
            }
            catch (Exception ex)
            {

            }

            if (!canNavigate) CanNavigateFailures++;
            return canNavigate;
        }
        
        public void Reset()
        {
            Log("Movement", "Resetting the queue");

            while(CurrentMovementQueue.Count>0)
                DequeuedPoints.Enqueue(CurrentMovementQueue.Dequeue());

            while (DequeuedPoints.Count > 0)
                CurrentMovementQueue.Enqueue(DequeuedPoints.Dequeue());
        }

        public void DequeueAll(bool addToDequeue)
        {
            Log("Movement", "DequeueAll");
            while (CurrentMovementQueue.Count > 0)
            {
                WoWPoint dPoint = CurrentMovementQueue.Dequeue();
                if (addToDequeue) DequeuedPoints.Enqueue(dPoint);
            }
        }

        public void UseDeqeuedPoints(bool reversed)
        {
            Log("Movement", "UseDeqeuedPoints reversed " + reversed.ToString());

            CurrentMovementQueue.Clear();

            var newPoints=DequeuedPoints.ToList();
            if (reversed) newPoints.Reverse();

            foreach (var newPoint in newPoints)
            {
                CurrentMovementQueue.Enqueue(newPoint);
            }
        }

        public void ForceDequeue(bool addToDequeue)
        {
            Log("Movement", "ForceDequeue: add to dequeue " + addToDequeue.ToString());

            if (CurrentMovementQueue.Count > 0)
            {
                WoWPoint dPoint = CurrentMovementQueue.Dequeue();
                if (addToDequeue) DequeuedPoints.Enqueue(dPoint);
            }
        }

        public void Log(string methodName, string logtext)
        {
            GarrisonBase.Debug("{0} ({1}) - {2}", methodName, Name, logtext);
        }

        public static class StuckChecker
        {
            private static DateTime _lastMoved = DateTime.Now;
            private static WaitTimer _stuckWaitTimer = new WaitTimer(new TimeSpan(0,0,0,3));
            private static WoWPoint _lastPlayerPos = WoWPoint.Zero;
            public static bool CheckStuck()
            {
                if (Player.Location.Distance(_lastPlayerPos) > 2f)
                {
                    _lastPlayerPos = Player.Location;
                    _lastMoved = DateTime.Now;
                    _stuckWaitTimer.Reset();
                    return false;
                }

                if (_stuckWaitTimer.IsFinished)
                {
                    return true;
                }

                return false;
            }

            public static void Reset()
            {
                _lastMoved = DateTime.Now;
                _stuckWaitTimer.Reset();
                _lastPlayerPos = Player.Location;
            }
        }


        public static WoWPoint FindNearestPoint(WoWPoint position, List<WoWPoint> points)
        {
            float distance = position.Distance(points[0]);
            WoWPoint currentPoint = points[0];
            for (int i = 1; i < points.Count; i++)
            {
                var testDistance = position.Distance(points[i]);
                if (testDistance < distance)
                {
                    distance = testDistance;
                    currentPoint = points[i];
                }
            }

            return currentPoint;
        }
        public static Vector3 GetVector3(WoWPoint point)
        {
            return new Vector3(point.X, point.Y, point.Z);
        }
        public static WoWPoint GetWoWPoint(Vector3 point)
        {
            return new WoWPoint(point.X, point.Y, point.Z);
        }

        /// <summary>
        /// Records players position periodically!
        /// </summary>
        public static class MovementCache
        {
            public static bool ShouldRecord { get; set; }
            public static List<LocationEntry> CachedPositions=new List<LocationEntry>();

            private static readonly WaitTimer _cachedWaitTimer = new WaitTimer(new TimeSpan(0, 0, 0, 0, 500));
            public static void AddPosition(WoWPoint pos, float radius = 10f)
            {
                if (!_cachedWaitTimer.IsFinished) return;
                _cachedWaitTimer.Reset();

                if (!CachedPositions.Any(p => p.Location.Distance(pos) <= p.Radius))
                    CachedPositions.Add(new LocationEntry(pos, radius));
            }

            public static void ResetCache()
            {
                CachedPositions.Clear();
                ShouldRecord = false;
            }

            public class LocationEntry
            {
                public WoWPoint Location { get; set; }
                public float Radius { get; set; }

                public LocationEntry(WoWPoint loc, float radius)
                {
                    Location = loc;
                    Radius = radius;
                }
            }

        }

        // Originally contributed by Chinajade.
        //
        // LICENSE:
        // This work is licensed under the
        //     Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
        //
        //Modified to work in conjunction with my behavior objects.
        /// <summary>
		/// Uses the transport.
		/// </summary>
		/// <param name="transportId">The transport identifier.</param>
		/// <param name="startLocation">The start location.</param>
		/// <param name="endLocation">The end location.</param>
		/// <param name="waitAtLocation">The wait at location.</param>
		/// <param name="standAtLocation">The stand at location.</param>
		/// <param name="getOffLocation">The get off location.</param>
		/// <param name="movement">The movement.</param>
		/// <param name="destination">The destination.</param>
		/// <param name="navigationFailedAction">
		///     The action to take if <paramref name="waitAtLocation" /> cant be navigated to
		/// </param>
		/// <returns>returns <c>true</c> until done</returns>
		/// <exception cref="Exception">A delegate callback throws an exception. </exception>
		public static async Task<bool> UseTransport(
			int transportId,
			WoWPoint startLocation,
			WoWPoint endLocation,
			WoWPoint waitAtLocation,
			WoWPoint standAtLocation,
			WoWPoint getOffLocation,
			MovementTypes movementType,
			string destination = null)
		{
			if (getOffLocation != WoWPoint.Empty && StyxWoW.Me.Location.DistanceSqr(getOffLocation) < 2 * 2)
			{
				return false;
			}

			var transportLocation = GetTransportLocation(transportId);
			if (transportLocation != WoWPoint.Empty
				&& transportLocation.DistanceSqr(startLocation) < 1.5 * 1.5
                && waitAtLocation.DistanceSqr(Player.Location) < 2 * 2)
			{
				TreeRoot.StatusText = "Moving inside transport";
				Navigator.PlayerMover.MoveTowards(standAtLocation);
				await CommonCoroutines.SleepForLagDuration();
				// wait for bot to get on boat.
				await Coroutine.Wait(12000, () => !StyxWoW.Me.IsMoving || Navigator.AtLocation(standAtLocation));
			}

			// loop while on transport to prevent bot from doing anything else
			while (StyxWoW.Me.Transport != null && StyxWoW.Me.Transport.Entry == transportId)
			{
				if (transportLocation != WoWPoint.Empty && transportLocation.DistanceSqr(endLocation) < 1.5 * 1.5)
				{
					TreeRoot.StatusText = "Moving out of transport";
					Navigator.PlayerMover.MoveTowards(getOffLocation);
					await CommonCoroutines.SleepForLagDuration();
					// Sleep until we stop moving.
					await Coroutine.Wait(12000, () => !StyxWoW.Me.IsMoving || Navigator.AtLocation(getOffLocation));
					return true;
				}

				// Exit loop if in combat or dead.
				if (StyxWoW.Me.Combat || !StyxWoW.Me.IsAlive)
					return false;

				TreeRoot.StatusText = "Waiting for the end location";
				await Coroutine.Yield();
				// update transport location.
				transportLocation = GetTransportLocation(transportId);
			}

			if (waitAtLocation.DistanceSqr(Player.Location) > 2 * 2)
			{
			    var _movement = new Movement(waitAtLocation, 2f, name: "TransportWaitAtLocation", checkStuck: false);

                if (movementType == MovementTypes.ClickToMove) 
                    await _movement.ClickToMove();
                else
			        await _movement.MoveTo();

				return true;
			}
			await CommonCoroutines.LandAndDismount();
			TreeRoot.StatusText = "Waiting for transport";
			return true;
		}

		private static WoWPoint GetTransportLocation(int transportId)
		{
			var transport = ObjectManager.GetObjectsOfType<WoWGameObject>().FirstOrDefault(o => o.Entry == transportId);
			return transport != null ? transport.WorldLocation : WoWPoint.Zero;
		}
	}
    

}
