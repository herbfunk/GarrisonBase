using System;
using System.Collections.Generic;
using System.Linq;
using Herbfunk.GarrisonBase.Character;
using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.CommonBot.Frames;

namespace Herbfunk.GarrisonBase.Helpers
{
    public static class TaxiFlightHelper
    {
        public static List<TaxiNodeInfo> TaxiNodes = new List<TaxiNodeInfo>();
        public static readonly List<FlightPathInfo> FlightPaths = new List<FlightPathInfo>();

        public static bool IsOpen { get; set; }

        static TaxiFlightHelper()
        {
            LuaEvents.OnTaxiMapOpened += TaxiFrameOpened;
            LuaEvents.OnTaxiMapClosed += TaxiFrameClosed;
            IsOpen = TaxiFrame.Instance.IsVisible;
            PopulateFlightPaths();

        }

        private static void TaxiFrameOpened()
        {
            IsOpen = true;
            if (TaxiNodes.Count == 0) PopulateTaxiNodes();
        }
        private static void TaxiFrameClosed()
        {
            IsOpen = false;
        }
       

        public static void PopulateTaxiNodes()
        {
            GarrisonBase.Debug("Populating Taxi Nodes!");

            if (!IsOpen) return;

            TaxiNodes.Clear();
            foreach (var node in TaxiFrame.Instance.Nodes)
            {
                TaxiNodes.Add(new TaxiNodeInfo(node));
            }

            
            GarrisonBase.Debug("Added a total of {0} Taxi Nodes!", TaxiNodes.Count);
        }

        private static void PopulateFlightPaths()
        {
            GarrisonBase.Debug("Populating Flight Paths from XML..");
            //Populate the known flight paths incase we need it!
            FlightPaths.Clear();
            foreach (var node in Styx.CommonBot.FlightPaths.XmlNodes)
            {
                FlightPaths.Add(new FlightPathInfo(node));
            }
            GarrisonBase.Debug("Added a total of {0} Flight Paths!", FlightPaths.Count);
        }

        public static TaxiNodeInfo NearestTaxiNode
        {
            get
            {
                var playerMapId = !Player.MapIsContinent
                    ? Player.ParentMapId
                    : (int)Player.MapId.Value;

                return NearestTaxiNodeFromLocation(Player.Location, playerMapId);
            }
        }
        public static TaxiNodeInfo NearestTaxiNodeFromLocation(WoWPoint location, int continentId = -1)
        {
            //Get flight paths (optionally filtering with continent id)
            List<TaxiNodeInfo> flightpaths = continentId > -1
                ? TaxiNodes.Where(fp => fp.Info != null && fp.Info.Continent == continentId)
                    .OrderBy(fp => fp.Info.Location.Distance(location))
                    .ToList()
                : TaxiNodes.Where(fp => fp.Info != null).OrderBy(fp => fp.Info.Location.Distance(location)).ToList();

            return flightpaths.Count > 0 ? flightpaths[0] : null;
        }


        public static FlightPathInfo NearestFlightPath
        {
            get
            {
                var playerMapId = !Player.MapIsContinent
                    ? Player.ParentMapId
                    : (int)Player.MapId.Value;

                return NearestFlightPathFromLocation(Player.Location, playerMapId);
            }
        }
        public static FlightPathInfo NearestFlightPathFromLocation(WoWPoint location, int continentId=-1)
        {
            //Get flight paths (optionally filtering with continent id)
            List<FlightPathInfo> flightpaths = continentId > -1 
                ? FlightPaths.Where(fp => fp.Continent == continentId)
                    .OrderBy(fp => fp.Location.Distance(location))
                    .ToList()
                : FlightPaths.OrderBy(fp => fp.Location.Distance(location)).ToList();

            return flightpaths.Count > 0 ? flightpaths[0] : null;
        }


        public static bool ShouldTakeFlightPath(WoWPoint destination)
        {
            
            var runTravelTime = Styx.CommonBot.FlightPaths.GetRunPathTime(new[] {Player.Location, destination}, 14.7f);
            GarrisonBase.Debug("Checking ShouldTakeFlightpath for {0} with mounted time {1} seconds", destination, runTravelTime.TotalSeconds);
            if (runTravelTime.TotalSeconds < 90)
            {
                return Styx.CommonBot.FlightPaths.ShouldTakeFlightpath(Player.Location, destination, 14.7f);
            }
            else
            {
                var playerMapId = !Player.MapIsContinent
                    ? Player.ParentMapId
                    : (int)Player.MapId.Value;

                //Lets verify that our possible destination
                var destinationTaxi=NearestFlightPathFromLocation(destination);
                var currentTaxi = NearestFlightPath;
                if (destinationTaxi.Name == currentTaxi.Name)
                {
                    GarrisonBase.Debug("ShouldTakeFlight path return false due to matching names taxi and current taxi");
                    return false;
                }
            }
            return true;
        }

        public static int GetMapId(WoWPoint location)
        {
            return StyxWoW.WorldScene.WorldMap.GetMapIdAt(location);
        }

        public class TaxiNodeInfo
        {
            public string Name { get; set; }
            public bool Known { get; set; }
            public readonly FlightPathInfo Info;

            public TaxiNodeInfo(TaxiFrame.TaxiFrameNode node)
            {
                Name = node.Name.ToLower();
                Known = node.Reachable;
                var flightpaths = FlightPaths.Where(fp => fp.Name == Name).ToList();
                if (flightpaths.Count > 0)
                    Info = flightpaths[0].ShallowCopy();
            }
        }

        public class FlightPathInfo
        {
            public static readonly FlightPathInfo InvalidInfo = new FlightPathInfo("Invalid", WoWPoint.Empty, 9999);
            public const uint LunarfallMasterEntry = 81103;
            public const uint FrostwallMasterEntry = 79407;

            public WoWPoint Location { get; set; }
            public string Name { get; set; }
            public uint Continent { get; set; }
            public uint MasterId { get; set; }
            public bool Known { get; set; }

            public FlightPathInfo(string name, WoWPoint loc, uint continent)
            {
                Name = name.ToLower();
                Location = loc;
                Continent = continent;
                MasterId = 9999;
                Known = true;
            }
            public FlightPathInfo(XmlFlightNode node)
            {
                Name = node.Name.ToLower();
                Location = node.Location;
                Continent = node.Continent;
                MasterId = node.MasterEntry;
                Known = true;
            }

            public override string ToString()
            {
                return String.Format("FlightPathInfo {0} Continent {1} [{2}]", Name, Continent, Location);
            }

            public FlightPathInfo ShallowCopy()
            {
                return (FlightPathInfo)this.MemberwiseClone();
            }
        }
    }
}
