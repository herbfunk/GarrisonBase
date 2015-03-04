using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
       

        
        public async Task<MoveResult> MoveTowards()
        {
            //

            if (CurrentMovementQueue.Count == 0)
                return MoveResult.ReachedDestination;

            
            WoWPoint location = CurrentMovementQueue.Peek();

            float currentDistance = location.Distance(StyxWoW.Me.Location);
            if (currentDistance <= Distance)
            {
                GarrisonBase.Log("MoveTo has dequeued location - {0}", location.ToString());
                CurrentMovementQueue.Dequeue();

                if (CurrentMovementQueue.Count == 0)
                {
                    GarrisonBase.Log("MoveTo has finished!");
                    WoWMovement.MoveStop();
                    return MoveResult.ReachedDestination;
                }

                return MoveResult.Moved;
            }
           
            //if (StyxWoW.Me.IsMoving)
            //    return true;

            var newloc = MathEx.CalculatePointFrom(StyxWoW.Me.Location, location, Math.Max(currentDistance - Distance, 0));
            WoWPoint newpoint = new WoWPoint(newloc.X, newloc.Y, newloc.Z);
           
            var moveresult = MoveResult.Moved;

            try
            {
                moveresult = await CommonCoroutines.MoveTo(newpoint);
            }
            catch (Exception ex)
            {
                Navigator.Clear();
                GarrisonBase.Log("[Movement] Exception during movement attempt!");
                try
                {
                    Navigator.MoveTo(newpoint);
                }
                catch
                {
                    GarrisonBase.Log("[Movement] Double Exception during movement attempt!!");
                    return MoveResult.Failed;
                }
            }


            //Navigator.GetRunStatusFromMoveResult(moveresult);
            switch (moveresult)
            {
                case MoveResult.UnstuckAttempt:
                    GarrisonBase.Log("[Movement] MoveResult: UnstuckAttempt.");
                    await Buddy.Coroutines.Coroutine.Sleep(500);
                    break;

                case MoveResult.Failed:
                    GarrisonBase.Log("[Movement] MoveResult: Failed for {0}", newpoint.ToString());
                    break;

                case MoveResult.ReachedDestination:
                    GarrisonBase.Log("[Movement] MoveResult: ReachedDestination.");
                    break;
            }

            return moveresult;
        }

        public async Task<bool> MoveTo()
        {
            if (CurrentMovementQueue.Count == 0)
                return false;

            //WoWPoint _curLocation = CurrentMovementQueue.Peek();
            //float z;
            //Navigator.FindHeight(_curLocation.X, _curLocation.Y, out z);
            //WoWPoint location = new WoWPoint(_curLocation.X, _curLocation.Y, z);

            WoWPoint location = CurrentMovementQueue.Peek();

            float currentDistance = location.Distance(StyxWoW.Me.Location);
            if (currentDistance <= Distance)
            {
                GarrisonBase.Log("MoveTo has dequeued location - {0}", location.ToString());
                CurrentMovementQueue.Dequeue();

                if (CurrentMovementQueue.Count == 0)
                {
                    GarrisonBase.Log("MoveTo has finished!");
                    WoWMovement.MoveStop();
                    return false;
                }
                
                return true;
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
                GarrisonBase.Log("[Movement] Exception during movement attempt!");
                try
                {
                    Navigator.MoveTo(location);
                }
                catch
                {
                    GarrisonBase.Log("[Movement] Double Exception during movement attempt!!");
                    return false;
                }
            }
 
            
            //Navigator.GetRunStatusFromMoveResult(moveresult);
            switch (moveresult)
            {
                case MoveResult.UnstuckAttempt:
                    GarrisonBase.Log("[Movement] MoveResult: UnstuckAttempt.");
                    await Buddy.Coroutines.Coroutine.Sleep(500);
                    break;

                case MoveResult.Failed:
                    GarrisonBase.Log("[Movement] MoveResult: Failed for {0}", location.ToString());
                    return false;

                case MoveResult.ReachedDestination:
                    GarrisonBase.Log("[Movement] MoveResult: ReachedDestination.");
                    return false;
            }

            return true;
        }

        internal static void Test()
        {
            
          //  GarrisonPolygons.Clear();
          //  //, , , 
          //  //, , , , 
          //  WoWPoint playerpos = StyxWoW.Me.Location;
           
          ////  GarrisonPolygons.Add(new Polygon("Horde_Garrison_North_Poly2", Horde_Garrison_North_Poly2));
          //  if (WoWMathHelper.IsPointInPoly(playerpos, Horde_Garrison_North_Poly2))
          //      Logging.Write("Player Position inside Horde_Garrison_North_Poly2");

          // // GarrisonPolygons.Add(new Polygon("Horde_Garrison_Center_Poly2", Horde_Garrison_Center_Poly2));
          //  if (WoWMathHelper.IsPointInPoly(playerpos, Horde_Garrison_Center_Poly2))
          //      Logging.Write("Player Position inside Horde_Garrison_Center_Poly2");

          // // GarrisonPolygons.Add(new Polygon("Horde_Garrison_CenterLarge_Poly2", Horde_Garrison_CenterLarge_Poly2));
          //  if (WoWMathHelper.IsPointInPoly(playerpos, Horde_Garrison_CenterLarge_Poly2))
          //      Logging.Write("Player Position inside Horde_Garrison_CenterLarge_Poly2");

          // // GarrisonPolygons.Add(new Polygon("Horde_Garrison_CenterMedium_Poly2", Horde_Garrison_CenterMedium_Poly2));
          //  if (WoWMathHelper.IsPointInPoly(playerpos, Horde_Garrison_CenterMedium_Poly2))
          //      Logging.Write("Player Position inside Horde_Garrison_CenterMedium_Poly2");

          // // GarrisonPolygons.Add(new Polygon("Horde_Garrison_FishShack_Poly2", Horde_Garrison_FishShack_Poly2));
          //  if (WoWMathHelper.IsPointInPoly(playerpos, Horde_Garrison_FishShack_Poly2))
          //      Logging.Write("Player Position inside Horde_Garrison_FishShack_Poly2");

          //  //GarrisonPolygons.Add(new Polygon("Horde_Garrison_Garden_Poly2", Horde_Garrison_Garden_Poly2));
          //  if (WoWMathHelper.IsPointInPoly(playerpos, Horde_Garrison_Garden_Poly2))
          //      Logging.Write("Player Position inside Horde_Garrison_Garden_Poly2");

          //  //GarrisonPolygons.Add(new Polygon("Horde_Garrison_Mine_Poly2", Horde_Garrison_Mine_Poly2));
          //  if (WoWMathHelper.IsPointInPoly(playerpos, Horde_Garrison_Mine_Poly2))
          //      Logging.Write("Player Position inside Horde_Garrison_Mine_Poly2");

          //  //GarrisonPolygons.Add(new Polygon("Horde_Garrison_West_Poly2", Horde_Garrison_West_Poly2));
          //  if (WoWMathHelper.IsPointInPoly(playerpos, Horde_Garrison_West_Poly2))
          //      Logging.Write("Player Position inside Horde_Garrison_West_Poly2");

          // // GarrisonPolygons.Add(new Polygon("Horde_Garrison_Poly2", Horde_Garrison_Poly2));
          //  if (WoWMathHelper.IsPointInPoly(playerpos, Horde_Garrison_Poly2))
          //      Logging.Write("Player Position inside Horde_Garrison_Poly2");

          //  //foreach (var garrisonPolygon in GarrisonPolygons)
          //  //{
          //  //    garrisonPolygon.UpdateConnectingPolygons(GarrisonPolygons);
          //  //    Logging.Write("{0}", garrisonPolygon.ToString());
          //  //}
        }

    }
    public class Polygon
    {
        public Vector2[] Vector2Array { get; set; }
        public string Name { get; set; }
        public WoWPoint Entrance { get; set; }
        public WoWPoint Exit { get; set; }
        public List<int> PlotIds { get; set; }
        internal readonly Guid _guid;

        public Polygon(string name, Vector2[] array)
        {
            Name = name;
            Vector2Array = array;
            _guid = Guid.NewGuid();
        }

        public Polygon()
        {
            _guid = Guid.NewGuid();
        }

        public List<Polygon> ConnectingPolygons = new List<Polygon>();

        public void UpdateConnectingPolygons(List<Polygon> polygons)
        {
            ConnectingPolygons.Clear();

            foreach (var polygon in polygons)
            {
                if (polygon.Equals(this)) continue;

                var connectingCount = 0;
                foreach (var vector2 in polygon.Vector2Array)
                {
                    if (Vector2Array.Contains(vector2))
                        connectingCount++;

                    if (connectingCount > 1)
                        break;
                }

                if (connectingCount > 1) ConnectingPolygons.Add(polygon);
            }
        }

        public bool LocationInsidePolygon(WoWPoint loc)
        {
            return WoWMathHelper.IsPointInPoly(loc, Vector2Array);
        }
        public override string ToString()
        {
            string connectingPolygons = ConnectingPolygons.Aggregate("", (current, connectingPolygon) => current + connectingPolygon.Name + "\r\n");
            return String.Format("{0} ({1}) {2} Vector2's\r\nConnecting Polygons\r\n{3}",
                Name, _guid.ToString(), Vector2Array.Length, connectingPolygons);
        }

        public override int GetHashCode()
        {
            return _guid.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types. 
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var p = (Polygon)obj;
            return _guid.Equals(p._guid);
        }
    }
}
