using System.Collections.Generic;
using Styx;
using Tripper.Tools.Math;

namespace Herbfunk.GarrisonBase
{
    public static partial class MovementCache
    {
        internal static readonly WoWPoint HordeSmallPlot18SafePoint = new WoWPoint(5644.31, 4518.49, 119.22);
        internal static readonly WoWPoint HordeSmallPlot18Entrance = new WoWPoint(5642.857, 4514.143, 119.9609);
        internal static readonly WoWPoint HordeSmallPlot18WorkOrderPoint = new WoWPoint(5650.948,4511.86,119.2701);
        
        internal static readonly WoWPoint HordeSmallPlot19SafePoint = new WoWPoint(5650.921, 4543.233, 119.1478);
        internal static readonly WoWPoint HordeSmallPlot19Entrance = new WoWPoint(5660.101, 4546.847, 120.1033);
        internal static readonly WoWPoint HordeSmallPlot19WorkOrderPoint = new WoWPoint(5654.194,4552.898,119.2681);
        
        internal static readonly WoWPoint HordeSmallPlot20SafePoint = new WoWPoint(5628.176, 4521.718, 119.26);
        internal static readonly WoWPoint HordeSmallPlot20Entrance = new WoWPoint(5622.267, 4514.931, 119.9431);
        internal static readonly WoWPoint HordeSmallPlot20WorkOrderPoint = new WoWPoint(5628.583, 4510.27, 119.2702);

        internal static readonly WoWPoint HordeMediumPlot22SafePoint = new WoWPoint(5554.022, 4521.992, 130.979);
        internal static readonly WoWPoint HordeMediumPlot22Entrance = new WoWPoint(5540.719, 4521.523, 132.2882);
        internal static readonly WoWPoint HordeMediumPlot22WorkOrderPoint = new WoWPoint(5544.464,4516.489,132.2224);
        
        //
        internal static readonly WoWPoint HordeMediumPlot25SafePoint = new WoWPoint(5675.528, 4491.359, 129.8331);
        internal static readonly WoWPoint HordeMediumPlot25Entrance = new WoWPoint(5683.659, 4484.063, 131.0837);
        internal static readonly WoWPoint HordeMediumPlot25WorkOrderPoint = new WoWPoint(5685.945,4488.408,131.0202);

        internal static readonly WoWPoint HordeLargePlot23SafePoint = new WoWPoint(5591.252, 4486.254, 130.2408);
        internal static readonly WoWPoint HordeLargePlot23Entrance = new WoWPoint(5585.418, 4476.083, 130.671);

        internal static readonly WoWPoint HordeLargePlot24SafePoint = new WoWPoint(5640.684, 4469.426, 130.5282);
        internal static readonly WoWPoint HordeLargePlot24Entrance = new WoWPoint(5646.658, 4455.48, 130.8512);
        
        internal static readonly WoWPoint HordeMineSafePoint = new WoWPoint(5474.757, 4443.908, 144.6317);
        internal static readonly WoWPoint HordeHerbSafePoint = new WoWPoint(5416.397, 4562.327, 138.6368);

        internal static readonly WoWPoint HordePrimalTraderLevel2 = new WoWPoint(5627.185,4463.037,130.2032);
        internal static readonly WoWPoint HordePrimalTraderLevel3 = new WoWPoint(5578.712,4389.038,136.4498);


        private static readonly Vector2 Horde_Garrison_North0 = new Vector2(5730.992f, 4517.743f);
        private static readonly Vector2 Horde_Garrison_North1 = new Vector2(5709.716f, 4447.305f);
        private static readonly Vector2 Horde_Garrison_North2 = new Vector2(5686.682f, 4410.421f);
        private static readonly Vector2 Horde_Garrison_North3 = new Vector2(5593.379f, 4401.482f);
        private static readonly Vector2 Horde_Garrison_North4 = new Vector2(5619.595f, 4484.42f);

        internal static readonly WoWPoint HordeSellRepairNpc = new WoWPoint(5618.266, 4619.81, 138.6086);

        internal static readonly WoWPoint HordeGarrisonEntrance = new WoWPoint(5584.52, 4576.55, 136.78);
        
        #region Mine Paths

        internal static List<WoWPoint> Horde_Mine_LevelOne = new List<WoWPoint>
        {
            new WoWPoint(5468.426, 4433.92, 145.5129),
            new WoWPoint(5457.243, 4415.33, 137.0531),
            new WoWPoint(5459.553, 4398.723, 128.9129),
            new WoWPoint(5468.657, 4390.295, 124.7158),
            new WoWPoint(5483.245, 4397.901, 115.8856),
            new WoWPoint(5497.315, 4409.374, 110.7253),
            new WoWPoint(5514.499, 4417.739, 105.8532),
            new WoWPoint(5506.601, 4432.289, 99.24547),
            new WoWPoint(5499.969, 4443.858, 93.44688),
            new WoWPoint(5496.31, 4433.191, 94.49226),
            new WoWPoint(5489.424, 4454.552, 88.95512),
            new WoWPoint(5477.264, 4467.936, 87.17371),
            new WoWPoint(5446.308, 4457.766, 84.45317),
            new WoWPoint(5419.16, 4451.607, 83.18919),
            new WoWPoint(5399.129, 4469.644, 80.91027),
            new WoWPoint(5381.586, 4496.472, 73.38876),
            new WoWPoint(5399.129, 4469.644, 80.91027),
            new WoWPoint(5405.637, 4500.569, 73.15517),
            //Back
            new WoWPoint(5399.129, 4469.644, 80.91027),
            new WoWPoint(5419.16, 4451.607, 83.18919),
            new WoWPoint(5446.308, 4457.766, 84.45317),
            new WoWPoint(5477.264, 4467.936, 87.17371),
            new WoWPoint(5489.424, 4454.552, 88.95512),
            new WoWPoint(5496.31, 4433.191, 94.49226),
            new WoWPoint(5499.969, 4443.858, 93.44688),
            new WoWPoint(5506.601, 4432.289, 99.24547),
            new WoWPoint(5514.499, 4417.739, 105.8532),
            new WoWPoint(5497.315, 4409.374, 110.7253),
            new WoWPoint(5483.245, 4397.901, 115.8856),
            new WoWPoint(5468.657, 4390.295, 124.7158),
            new WoWPoint(5459.553, 4398.723, 128.9129),
            new WoWPoint(5457.243, 4415.33, 137.0531),
            new WoWPoint(5468.426, 4433.92, 145.5129),
        };

        internal static List<WoWPoint> Horde_Mine_LevelTwo = new List<WoWPoint>
        {
            new WoWPoint(5468.114, 4433.229, 145.514),
            new WoWPoint(5457.143, 4414.951, 136.8505),
            new WoWPoint(5458.759, 4399.931, 129.7475),
            new WoWPoint(5467.899, 4390.434, 124.8342),
            new WoWPoint(5486.985, 4400.745, 113.3002),
            new WoWPoint(5503.659, 4411.06, 110.8346),
            new WoWPoint(5514.952, 4417.597, 105.8567),
            new WoWPoint(5507.028, 4431.992, 99.24064),
            new WoWPoint(5499.1, 4445.17, 93.0611),
            new WoWPoint(5496.071, 4464.227, 88.99474),
            new WoWPoint(5501.986, 4494.269, 84.59019),
            new WoWPoint(5523.809, 4511.294, 80.05484),
            new WoWPoint(5552.856, 4497.281, 80.43407),
            new WoWPoint(5571.034, 4483.193, 74.93996),
            new WoWPoint(5553.587, 4465.762, 75.57629),
            new WoWPoint(5539.181, 4466.372, 75.23327),
            new WoWPoint(5532.026, 4485.888, 79.3177),
            new WoWPoint(5540.152, 4505.699, 80.53173),
            new WoWPoint(5509.921, 4505.284, 81.24356),
            new WoWPoint(5486.276, 4483.75, 85.4512),
            new WoWPoint(5470.472, 4466.344, 86.39157),
            new WoWPoint(5424.468, 4451.729, 82.85767),
            new WoWPoint(5400.208, 4464.841, 81.54099),
            new WoWPoint(5382.654, 4493.569, 74.20657),
            new WoWPoint(5396.69, 4472.12, 79.98512),
            new WoWPoint(5407.695, 4505.514, 72.06953),
            new WoWPoint(5397.122, 4471.448, 80.14297),
            new WoWPoint(5411.791, 4455.492, 83.54063),
            new WoWPoint(5447.291, 4457.866, 84.51817),
            new WoWPoint(5478.451, 4467.92, 87.34101),
            new WoWPoint(5499.524, 4443.04, 93.60712),
            new WoWPoint(5506.756, 4433.775, 99.26666),
            new WoWPoint(5513.995, 4419.095, 105.9212),
            new WoWPoint(5502.188, 4410.501, 110.8506),
            new WoWPoint(5482.446, 4397.981, 116.346),
            new WoWPoint(5467.989, 4388.934, 124.7328),
            new WoWPoint(5456.43, 4404.632, 132.2067),
            new WoWPoint(5464.164, 4428.584, 144.3604),
            new WoWPoint(5472.642, 4441.37, 144.7324),
        };

        internal static List<WoWPoint> Horde_Mine_LevelThree = new List<WoWPoint>
        {
            new WoWPoint(5470.184, 4437.162, 145.453),
            new WoWPoint(5458.945, 4416.404, 138.0157),
            new WoWPoint(5459.259, 4399.147, 129.2153),
            new WoWPoint(5470.396, 4390.42, 124.2538),
            new WoWPoint(5500.641, 4410.375, 110.8533),
            new WoWPoint(5515.192, 4417.786, 105.8599),
            new WoWPoint(5505.839, 4434.039, 99.26975),
            new WoWPoint(5496.016, 4447.882, 92.11174),
            new WoWPoint(5479.829, 4467.641, 87.40823),
            new WoWPoint(5424.378, 4451.391, 82.71361),
            new WoWPoint(5395.939, 4467.646, 81.1519),
            new WoWPoint(5379.451, 4500.462, 72.22378),
            new WoWPoint(5394.149, 4477.419, 78.70185),
            new WoWPoint(5411.542, 4509.639, 70.82801),
            new WoWPoint(5396.192, 4474.488, 79.34724),
            new WoWPoint(5418.297, 4449.297, 83.47066),
            new WoWPoint(5457.951, 4463.707, 85.12363),
            new WoWPoint(5441.62, 4502.058, 84.39003),
            new WoWPoint(5435.632, 4531.614, 81.21237),
            new WoWPoint(5448.064, 4576.881, 81.71964),
            new WoWPoint(5451.561, 4551.217, 82.198),
            new WoWPoint(5435.033, 4529.121, 81.25176),
            new WoWPoint(5458.1, 4463.685, 85.14763),
            new WoWPoint(5522.53, 4516.111, 81.23506),
            new WoWPoint(5554.668, 4495.281, 80.17819),
            new WoWPoint(5579.444, 4480.768, 73.28632),
            new WoWPoint(5542.817, 4464.619, 75.2293),
            new WoWPoint(5530.515, 4483.024, 78.49503),
            new WoWPoint(5538.352, 4503.316, 80.54839),
            new WoWPoint(5513.248, 4507.157, 80.43447),
            new WoWPoint(5501.109, 4489.634, 85.26486),
            new WoWPoint(5495.56, 4449.341, 91.66743),
            new WoWPoint(5506.939, 4432.216, 99.24582),
            new WoWPoint(5515.266, 4417.694, 105.8614),
            new WoWPoint(5500.292, 4408.67, 110.854),
            new WoWPoint(5468.042, 4389.145, 124.7168),
            new WoWPoint(5457.929, 4402.586, 131.0235),
            new WoWPoint(5473.227, 4441.238, 144.7218)
        };

        #endregion

        internal static List<WoWPoint> Horde_Herb_LevelOne = new List<WoWPoint>
        {
            new WoWPoint(5411.076, 4546.272, 139.124),
            new WoWPoint(5410.355, 4538.49, 139.124),
            new WoWPoint(5415.397, 4534.234, 139.124),
            new WoWPoint(5420.36, 4535.431, 139.124),
            new WoWPoint(5421.288, 4538.967, 139.124),
            new WoWPoint(5420.495, 4544.562, 139.124),
            new WoWPoint(5417.282, 4549.279, 139.124),
        };


        internal static List<WoWPoint> Horde_Plot22_TradePost_Level1 = new List<WoWPoint>
        {
            new WoWPoint(5539.182, 4521.761, 132.3156),
        };
        internal static List<WoWPoint> Horde_Plot25_TradePost_Level1 = new List<WoWPoint>
        {
            new WoWPoint(5685.888, 4481.797, 131.1157),
        };

        internal static List<WoWPoint> Horde_Plot22_TradePost_Level2 = new List<WoWPoint>
        {
            new WoWPoint(5538.009, 4524.245, 132.1923),
            new WoWPoint(5530.804, 4522.976, 132.2856),
        };
        internal static List<WoWPoint> Horde_Plot25_TradePost_Level2 = new List<WoWPoint>
        {
            new WoWPoint(5686.813, 4477.623, 131.0837),
            new WoWPoint(5691.814, 4473.452, 130.8819),
        };

        internal static List<WoWPoint> Horde_Plot22_TradePost_Level3 = new List<WoWPoint>
        {
            new WoWPoint(5533.957, 4522.382, 132.267),
            new WoWPoint(5524.172, 4519.543, 132.376),
        };
        internal static List<WoWPoint> Horde_Plot25_TradePost_Level3 = new List<WoWPoint>
        {
            new WoWPoint(5685.489, 4480.07, 131.1701),
            new WoWPoint(5698.563, 4471.478, 131.1745),
        };

        internal static List<WoWPoint> Horde_Plot24_WarMill_Level2 = new List<WoWPoint>
        {
            new WoWPoint(5648.714, 4449.405, 132.6737),
            new WoWPoint(5649.092, 4440.927, 132.7159),
        };

        internal static List<WoWPoint> Horde_Plot23_WarMill_Level2 = new List<WoWPoint>
        {
            new WoWPoint(5580.145, 4466.241, 132.5222),
            new WoWPoint(5573.215, 4461.042, 132.5584),
        };

        internal static List<WoWPoint> Horde_Plot24_WarMill_Level3 = new List<WoWPoint>
        {
            new WoWPoint(5645.536, 4455.636, 130.5243),
            new WoWPoint(5651.836, 4439.904, 132.8827),
            new WoWPoint(5642.813, 4428.252, 132.8838),
            new WoWPoint(5639.995, 4428.511, 132.8838),
        };

        internal static List<WoWPoint> Horde_Plot23_WarMill_Level3 = new List<WoWPoint>
        {
            new WoWPoint(5584.343, 4472.387, 130.3712),
            new WoWPoint(5573.402, 4458.196, 132.7247),
            new WoWPoint(5559.412, 4459.72, 132.7274),
            new WoWPoint(5558.325, 4462.138, 132.7274),
        };
    }
}
