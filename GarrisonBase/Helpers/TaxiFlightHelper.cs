using System.Collections.Generic;
using Styx.CommonBot.Frames;

namespace Herbfunk.GarrisonBase.Helpers
{
    public static class TaxiFlightHelper
    {
        public static List<TaxiNodeInfo> TaxiNodes = new List<TaxiNodeInfo>();

        public static void PopulateTaxiNodes()
        {
            GarrisonBase.Debug("Populating Taxi Nodes!");
            
            if (!TaxiFrame.Instance.IsVisible) return;

            TaxiNodes.Clear();
            foreach (var node in TaxiFrame.Instance.Nodes)
            {
                TaxiNodes.Add(new TaxiNodeInfo(node));
            }
        }

        public class TaxiNodeInfo
        {
            public string Name { get; set; }
            public bool Known { get; set; }

            public TaxiNodeInfo(TaxiFrame.TaxiFrameNode node)
            {
                Name = node.Name;
                Known = node.Reachable;
            }
        }

    }
}
