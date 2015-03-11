using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
using Styx;
using Styx.Common;
using Styx.CommonBot.Coroutines;
using Styx.Helpers;
using Styx.Pathing;
using Styx.WoWInternals;
using Vector2= Tripper.Tools.Math.Vector2;

namespace Herbfunk.GarrisonBase
{
    public class Movement
    {
        internal Queue<WoWPoint> CurrentMovementQueue = new Queue<WoWPoint>();
        internal Queue<WoWPoint> DequeuedPoints = new Queue<WoWPoint>(); 
        internal List<WoWPoint> DequeuedFinalPlayerPositionPoints = new List<WoWPoint>(); 

        public float Distance { get; set; }

        public Movement(WoWPoint location) : this(location, 5f)
        {
        }

        public Movement(WoWPoint location, float distance)
        {
            Distance = distance;
            CurrentMovementQueue.Enqueue(location);
        }

        public Movement(WoWPoint[] locations) : this(locations, 5f)
        {
        }

        public Movement(WoWPoint[] locations, float distance)
        {
            Distance = distance;
            foreach (var p in locations)
            {
                CurrentMovementQueue.Enqueue(p);
            }
        }

        public async Task<MoveResult> MoveTowards(bool allowDequeue = true)
        {
            //

            if (CurrentMovementQueue.Count == 0)
                return MoveResult.ReachedDestination;

            
            WoWPoint location = CurrentMovementQueue.Peek();
            WoWPoint playerPos = Player.Location;

            float currentDistance = location.Distance(playerPos);
            if (currentDistance <= Distance)
            {
                if (allowDequeue)
                {
                    GarrisonBase.Debug("MoveTowards has dequeued location - {0}", location.ToString());
                    DequeuedPoints.Enqueue(CurrentMovementQueue.Dequeue());
                    DequeuedFinalPlayerPositionPoints.Add(playerPos);
                }

                

                if (CurrentMovementQueue.Count == 0)
                {
                    GarrisonBase.Debug("MoveTowards has finished!");
                    WoWMovement.MoveStop();
                    return MoveResult.ReachedDestination;
                }

                return MoveResult.Moved;
            }
           
            //if (StyxWoW.Me.IsMoving)
            //    return true;

            var newloc = MathEx.CalculatePointFrom(StyxWoW.Me.Location, location, Math.Max(currentDistance - Distance, 0));
            WoWPoint newpoint = new WoWPoint(newloc.X, newloc.Y, newloc.Z);

            bool canNavigate = Navigator.CanNavigateWithin(playerPos, newpoint, Distance);

            if (!canNavigate)
            {
                GarrisonBase.Debug("[MoveTowards] Can Navigate Return False! {0}", location.ToString());
                return MoveResult.Failed;
            }
           
            var moveresult = MoveResult.Moved;

            try
            {
                moveresult = await CommonCoroutines.MoveTo(newpoint);
            }
            catch (Exception ex)
            {
                Navigator.Clear();
                GarrisonBase.Debug("[MoveTowards] Exception during movement attempt!");
                try
                {
                    Navigator.MoveTo(newpoint);
                }
                catch
                {
                    GarrisonBase.Debug("[MoveTowards] Double Exception during movement attempt!!");
                    return MoveResult.Failed;
                }
            }


            //Navigator.GetRunStatusFromMoveResult(moveresult);
            switch (moveresult)
            {
                case MoveResult.UnstuckAttempt:
                    GarrisonBase.Debug("[MoveTowards] MoveResult: UnstuckAttempt.");
                    await Buddy.Coroutines.Coroutine.Sleep(500);
                    break;

                case MoveResult.Failed:
                    GarrisonBase.Debug("[MoveTowards] MoveResult: Failed for {0}", newpoint.ToString());
                    break;

                case MoveResult.ReachedDestination:
                    GarrisonBase.Debug("[MoveTowards] MoveResult: ReachedDestination.");
                    break;
            }

            return moveresult;
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
                    GarrisonBase.Debug("MoveTo has dequeued location - {0}", location.ToString());
                    DequeuedPoints.Enqueue(CurrentMovementQueue.Dequeue());
                    DequeuedFinalPlayerPositionPoints.Add(playerPos);
                }

                if (CurrentMovementQueue.Count == 0)
                {
                    GarrisonBase.Debug("MoveTo has finished!");
                    WoWMovement.MoveStop();
                    return false;
                }
                
                return true;
            }
            bool canNavigate = Navigator.CanNavigateWithin(playerPos, location, Distance);

            if (!canNavigate)
            {
                GarrisonBase.Debug("MoveTo Can Navigate Return False! {0}", location.ToString());
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
                GarrisonBase.Debug("[MoveTo] Exception during movement attempt!");
                try
                {
                    Navigator.MoveTo(location);
                }
                catch
                {
                    GarrisonBase.Debug("[MoveTo] Double Exception during movement attempt!!");
                    return false;
                }
            }
 
            
            //Navigator.GetRunStatusFromMoveResult(moveresult);
            switch (moveresult)
            {
                case MoveResult.UnstuckAttempt:
                    GarrisonBase.Debug("[MoveTo] MoveResult: UnstuckAttempt.");
                    await Buddy.Coroutines.Coroutine.Sleep(500);
                    break;

                case MoveResult.Failed:
                    GarrisonBase.Debug("[MoveTo] MoveResult: Failed for {0}", location.ToString());
                    return false;

                case MoveResult.ReachedDestination:
                    GarrisonBase.Debug("[MoveTo] MoveResult: ReachedDestination.");
                    return false;
            }

            return true;
        }

        public async Task<MoveResult> MoveTo_Result(bool allowDequeue=true)
        {
            if (CurrentMovementQueue.Count == 0)
                return MoveResult.ReachedDestination;

            WoWPoint location = CurrentMovementQueue.Peek();
            WoWPoint playerPos = Player.Location;
            float currentDistance = location.Distance(playerPos);
            if (currentDistance <= Distance)
            {
                if (allowDequeue)
                {
                    GarrisonBase.Debug("MoveTo has dequeued location - {0}", location.ToString());
                    DequeuedPoints.Enqueue(CurrentMovementQueue.Dequeue());
                    DequeuedFinalPlayerPositionPoints.Add(playerPos);
                }

                if (CurrentMovementQueue.Count == 0)
                {
                    GarrisonBase.Debug("MoveTo has finished!");
                    WoWMovement.MoveStop();
                    return MoveResult.ReachedDestination;
                }

                return MoveResult.ReachedDestination;
            }
            bool canNavigate = Navigator.CanNavigateWithin(playerPos, location, Distance);

            if (!canNavigate)
            {
                GarrisonBase.Debug("[MoveTo] Can Navigate Return False! {0}", location.ToString());
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
                GarrisonBase.Debug("[MoveTo] Exception during movement attempt!");
                try
                {
                    Navigator.MoveTo(location);
                }
                catch
                {
                    GarrisonBase.Debug("[MoveTo] Double Exception during movement attempt!!");
                    return MoveResult.Failed;
                }
            }


            //GarrisonBase.Log("[MoveTo] MoveResult: {0}", moveresult.ToString());
            return moveresult;
        }

        public async Task<MoveResult> ClickToMove_Result(bool allowDequeue = true)
        {
            if (CurrentMovementQueue.Count == 0)
                return MoveResult.ReachedDestination;

            WoWPoint location = CurrentMovementQueue.Peek();
            WoWPoint playerPos = Player.Location;
            float currentDistance = location.Distance(playerPos);
            if (currentDistance <= Distance)
            {
                if (allowDequeue)
                {
                    GarrisonBase.Debug("ClickToMove has dequeued location - {0}", location.ToString());
                    DequeuedPoints.Enqueue(CurrentMovementQueue.Dequeue());
                    DequeuedFinalPlayerPositionPoints.Add(playerPos);
                }

                if (CurrentMovementQueue.Count == 0)
                {
                    GarrisonBase.Debug("ClickToMove has finished!");
                    WoWMovement.MoveStop();
                    return MoveResult.ReachedDestination;
                }

                return MoveResult.ReachedDestination;
            }

            if (!WoWMovement.ClickToMoveInfo.IsClickMoving)
            {
                GarrisonBase.Debug("ClickToMove {0}", location);
                WoWMovement.ClickToMove(location);
                await Coroutine.Sleep(StyxWoW.Random.Next(525, 800));
            }
            

            return MoveResult.Moved;
        }
        public async Task<bool> ClickToMove(bool allowDequeue = true)
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
                    GarrisonBase.Debug("ClickToMove has dequeued location - {0}", location.ToString());
                    DequeuedPoints.Enqueue(CurrentMovementQueue.Dequeue());
                    DequeuedFinalPlayerPositionPoints.Add(playerPos);
                }

                if (CurrentMovementQueue.Count == 0)
                {
                    GarrisonBase.Debug("ClickToMove has finished!");
                    WoWMovement.MoveStop();
                    return false;
                }

                return true;
            }

            if (!WoWMovement.ClickToMoveInfo.IsClickMoving)
            {
                GarrisonBase.Debug("ClickToMove {0}", location);
                WoWMovement.ClickToMove(location);
               // await Coroutine.Sleep(StyxWoW.Random.Next(525, 800));
            }


            return true;
        }
        
        public void Reset()
        {
            GarrisonBase.Log("MoveTo Reset!");

            while(CurrentMovementQueue.Count>0)
                DequeuedPoints.Enqueue(CurrentMovementQueue.Dequeue());

            while (DequeuedPoints.Count > 0)
                CurrentMovementQueue.Enqueue(DequeuedPoints.Dequeue());
        }

        public void DequeueAll(bool addToDequeue)
        {
            GarrisonBase.Log("MoveTo DequeueAll!");

            while (CurrentMovementQueue.Count > 0)
            {
                WoWPoint dPoint = CurrentMovementQueue.Dequeue();
                if (addToDequeue) DequeuedPoints.Enqueue(dPoint);
            }
        }

        public void UseDeqeuedPoints(bool reversed)
        {
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
            if (CurrentMovementQueue.Count > 0)
            {
                WoWPoint dPoint = CurrentMovementQueue.Dequeue();
                if (addToDequeue) DequeuedPoints.Enqueue(dPoint);
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
    }

}
