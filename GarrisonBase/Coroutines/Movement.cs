using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Character;
using Herbfunk.GarrisonBase.Coroutines.Behaviors;
using Herbfunk.GarrisonBase.Helpers;
using Styx;
using Styx.Common;
using Styx.Common.Helpers;
using Styx.CommonBot.Coroutines;
using Styx.Pathing;
using Styx.WoWInternals;

namespace Herbfunk.GarrisonBase.Coroutines
{
    public class Movement
    {
        public enum MovementTypes
        {
            Normal,
            ClickToMove
        }

        internal Queue<WoWPoint> CurrentMovementQueue = new Queue<WoWPoint>();
        internal Queue<WoWPoint> DequeuedPoints = new Queue<WoWPoint>(); 
        internal List<WoWPoint> DequeuedFinalPlayerPositionPoints = new List<WoWPoint>();

        private bool _checkedShoulUseFlightPath;
        private bool _didResetStuckChecker = false;
        private bool _checkStuck;
        public float Distance { get; set; }
        public readonly string Name;

        public Movement(WoWPoint location, float distance, bool ignoreTaxiCheck = false, string name = "", bool checkStuck=false)
            : this(new[] { location }, distance, ignoreTaxiCheck, name, checkStuck)
        {
        }

        public Movement(WoWPoint[] locations, bool ignoreTaxiCheck = false, string name = "", bool checkStuck = false)
            : this(locations, 5f, ignoreTaxiCheck, name, checkStuck)
        {
        }

        public Movement(WoWPoint[] locations, float distance, bool ignoreTaxiCheck = false, string name = "", bool checkStuck = false)
        {
            Name = name;
            _checkedShoulUseFlightPath = ignoreTaxiCheck;
            _checkStuck = checkStuck;
            Distance = distance;
            foreach (var p in locations)
            {
                CurrentMovementQueue.Enqueue(p);
            }
        }

        public async Task<bool> MoveTo(bool allowDequeue=true)
        {
            if (CurrentMovementQueue.Count == 0)
                return false;

            WoWPoint location = CurrentMovementQueue.Peek();
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

            if (!_checkedShoulUseFlightPath)
            {
                _checkedShoulUseFlightPath = true;
                if (TaxiFlightHelper.ShouldTakeFlightPath(location))
                {
                    BehaviorManager.SwitchBehaviors.Add(new BehaviorUseFlightPath(location));
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

            bool canNavigate = true;
            try
            {
                canNavigate = Navigator.CanNavigateWithin(playerPos, location, Distance);
            }
            catch (Exception ex)
            {

            }
            

            if (!canNavigate)
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

            WoWPoint location = CurrentMovementQueue.Peek();
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

                return MoveResult.ReachedDestination;
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

            bool canNavigate = true;
            try
            {
                canNavigate = Navigator.CanNavigateWithin(playerPos, location, Distance);
            }
            catch (Exception ex)
            {

            }
            if (!canNavigate)
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

            WoWPoint location = CurrentMovementQueue.Peek();
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

            WoWPoint location = CurrentMovementQueue.Peek();
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
    }

}
