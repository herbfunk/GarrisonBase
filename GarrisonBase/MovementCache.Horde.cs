using System.Collections.Generic;
using Styx;
using Styx.CommonBot.Profiles;

namespace Herbfunk.GarrisonBase
{
    public static partial class MovementCache
    {
        internal static readonly WoWPoint HordeSmallPlot18SafePoint = new WoWPoint(5644.31, 4518.49, 119.22);
        internal static readonly WoWPoint HordeSmallPlot18Entrance = new WoWPoint(5642.857, 4514.143, 119.9609);
        internal static readonly WoWPoint HordeSmallPlot18WorkOrderPoint = new WoWPoint(5650.948, 4511.86, 119.2701);

        internal static readonly WoWPoint HordeSmallPlot19SafePoint = new WoWPoint(5650.921, 4543.233, 119.1478);
        internal static readonly WoWPoint HordeSmallPlot19Entrance = new WoWPoint(5660.101, 4546.847, 120.1033);
        internal static readonly WoWPoint HordeSmallPlot19WorkOrderPoint = new WoWPoint(5654.194, 4552.898, 119.2681);

        internal static readonly WoWPoint HordeSmallPlot20SafePoint = new WoWPoint(5628.176, 4521.718, 119.26);
        internal static readonly WoWPoint HordeSmallPlot20Entrance = new WoWPoint(5622.267, 4514.931, 119.9431);
        internal static readonly WoWPoint HordeSmallPlot20WorkOrderPoint = new WoWPoint(5628.583, 4510.27, 119.2702);

        internal static readonly WoWPoint HordeMediumPlot22SafePoint = new WoWPoint(5554.022, 4521.992, 130.979);
        internal static readonly WoWPoint HordeMediumPlot22Entrance = new WoWPoint(5540.719, 4521.523, 132.2882);
        internal static readonly WoWPoint HordeMediumPlot22WorkOrderPoint = new WoWPoint(5544.464, 4516.489, 132.2224);

        //
        internal static readonly WoWPoint HordeMediumPlot25SafePoint = new WoWPoint(5675.528, 4491.359, 129.8331);
        internal static readonly WoWPoint HordeMediumPlot25Entrance = new WoWPoint(5683.659, 4484.063, 131.0837);
        internal static readonly WoWPoint HordeMediumPlot25WorkOrderPoint = new WoWPoint(5685.945, 4488.408, 131.0202);

        internal static readonly WoWPoint HordeLargePlot23SafePoint = new WoWPoint(5591.252, 4486.254, 130.2408);
        internal static readonly WoWPoint HordeLargePlot23Entrance = new WoWPoint(5585.418, 4476.083, 130.671);

        internal static readonly WoWPoint HordeLargePlot24SafePoint = new WoWPoint(5640.684, 4469.426, 130.5282);
        internal static readonly WoWPoint HordeLargePlot24Entrance = new WoWPoint(5646.658, 4455.48, 130.8512);

        internal static readonly WoWPoint HordeMineSafePoint = new WoWPoint(5474.757, 4443.908, 144.6317);
        internal static readonly WoWPoint HordeHerbSafePoint = new WoWPoint(5416.397, 4562.327, 138.6368);

        internal static readonly WoWPoint HordePrimalTraderLevel2 = new WoWPoint(5627.185, 4463.037, 130.2032);
        internal static readonly WoWPoint HordePrimalTraderLevel3 = new WoWPoint(5578.712, 4389.038, 136.4498);

        internal static readonly WoWPoint HordeSellRepairNpc = new WoWPoint(5618.266, 4619.81, 138.6086);

        internal static readonly WoWPoint HordeFlightPathNpc = new WoWPoint(5579.027, 4565.401, 136.2501);

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

        internal static readonly WoWPoint HordeSalvageYardPlot18 = new WoWPoint(5643.238, 4507.169, 120.1373);
        internal static readonly WoWPoint HordeSalvageYardPlot19 = new WoWPoint(5662.977, 4548.137, 120.1351);
        internal static readonly WoWPoint HordeSalvageYardPlot20 = new WoWPoint(5620.044, 4512.414, 120.1376);


        #region Trade Post Special Movement

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

        #endregion

        #region War Mill Special Movement

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

        #endregion


        #region The Forge

        #region Level 1

        internal static List<WoWPoint> Horde_Plot18_Forge_Level1 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Horde_Plot19_Forge_Level1 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Horde_Plot20_Forge_Level1 = new List<WoWPoint>
        {
            //TODO
        };

        #endregion

        #region Level 2

        internal static List<WoWPoint> Horde_Plot18_Forge_Level2 = new List<WoWPoint>
        {
            new WoWPoint(5644.724, 4510.004, 119.9598),
        };
        internal static List<WoWPoint> Horde_Plot19_Forge_Level2 = new List<WoWPoint>
        {
            new WoWPoint(5663.5, 4548.672, 119.959),
        };
        internal static List<WoWPoint> Horde_Plot20_Forge_Level2 = new List<WoWPoint>
        {
            new WoWPoint(5619.303, 4510.864, 119.9594),
        };
        
        #endregion

        #region Level 3

        internal static List<WoWPoint> Horde_Plot18_Forge_Level3 = new List<WoWPoint>
        {
            new WoWPoint(5643.425, 4504.817, 120.1371),
        };
        internal static List<WoWPoint> Horde_Plot19_Forge_Level3 = new List<WoWPoint>
        {
            new WoWPoint(5664.637, 4549.081, 120.135),
        };
        internal static List<WoWPoint> Horde_Plot20_Forge_Level3 = new List<WoWPoint>
        {
            new WoWPoint(5619.353, 4511.691, 120.1383),
        };
        
        #endregion

        #endregion

        #region Tannery Special Movement

        #region Level 1
        internal static List<WoWPoint> Horde_Plot18_Tannery_Level1 = new List<WoWPoint>
        {
            new WoWPoint(5643.922, 4510.145, 120.0833),
            new WoWPoint(5646.894, 4505.219, 120.0833),
            new WoWPoint(5640.625, 4503.011, 120.0843),
        };
        internal static List<WoWPoint> Horde_Plot19_Tannery_Level1 = new List<WoWPoint>
        {
           new WoWPoint(5659.193, 4547.115, 120.0805),
           new WoWPoint(5663.818, 4552.051, 120.0805),
           new WoWPoint(5666.795, 4546.408, 120.0841),
        };
        internal static List<WoWPoint> Horde_Plot20_Tannery_Level1 = new List<WoWPoint>
        {
            new WoWPoint(5622.619, 4514.744, 120.0839),
            new WoWPoint(5620.327, 4508.683, 120.0839),
            new WoWPoint(5615.176, 4512.928, 120.0848),
        };
        #endregion

        #region Level 2
        internal static List<WoWPoint> Horde_Plot18_Tannery_Level2 = new List<WoWPoint>
        {
            new WoWPoint(5642.981, 4511.13, 120.0836),
            new WoWPoint(5648.264, 4505.076, 120.0851),
            new WoWPoint(5639.276, 4503.719, 120.085),
        };
        internal static List<WoWPoint> Horde_Plot19_Tannery_Level2 = new List<WoWPoint>
        {
            new WoWPoint(5659.718, 4546.733, 120.0842),
            new WoWPoint(5661.997, 4553.868, 120.0826),
            new WoWPoint(5667.482, 4546.621, 120.083),
        };
        internal static List<WoWPoint> Horde_Plot20_Tannery_Level2 = new List<WoWPoint>
        {
            new WoWPoint(5622.567, 4514.994, 120.0852),
            new WoWPoint(5621.969, 4507.079, 120.0846),
            new WoWPoint(5615.2, 4512.108, 120.0855),
        };
        #endregion

        #region Level 3
        internal static List<WoWPoint> Horde_Plot18_Tannery_Level3 = new List<WoWPoint>
        {
           new WoWPoint(5642.971, 4511.538, 120.0849),
           new WoWPoint(5647.557, 4506.59, 120.0835),
           new WoWPoint(5639.714, 4503.459, 120.1659),
        };
        internal static List<WoWPoint> Horde_Plot19_Tannery_Level3 = new List<WoWPoint>
        {
            new WoWPoint(5659.867, 4546.536, 120.0818),
            new WoWPoint(5662.684, 4553.269, 120.0829),
            new WoWPoint(5667.736, 4545.67, 120.1642),
        };
        internal static List<WoWPoint> Horde_Plot20_Tannery_Level3 = new List<WoWPoint>
        {
            new WoWPoint(5623.112, 4515.688, 120.0853),
            new WoWPoint(5622.1, 4506.791, 120.0847),
            new WoWPoint(5615.039, 4512.487, 120.1662),
        };
        #endregion

        #endregion

        #region Alchemy Lab

        #region Level 1
        internal static List<WoWPoint> Horde_Plot18_AlchemyLab_Level1 = new List<WoWPoint>
        {
            new WoWPoint(5643.07, 4507.967, 119.958),
            new WoWPoint(5640.691, 4503.669, 119.958),
            new WoWPoint(5642.813, 4499.633, 119.958),
            new WoWPoint(5645.904, 4504.415, 119.9567),
        };
        internal static List<WoWPoint> Horde_Plot19_AlchemyLab_Level1 = new List<WoWPoint>
        {
            new WoWPoint(5661.299, 4547.595, 119.9548),
            new WoWPoint(5666.48, 4545.784, 119.9548),
            new WoWPoint(5669.536, 4549.629, 119.9548),
            new WoWPoint(5664.695, 4552.659, 119.9533),
        };
        internal static List<WoWPoint> Horde_Plot20_AlchemyLab_Level1 = new List<WoWPoint>
        {
            new WoWPoint(5622.161, 4515.262, 119.953),
            new WoWPoint(5614.694, 4512.322, 119.957),
            new WoWPoint(5614.147, 4507.57, 119.957),
            new WoWPoint(5620.31, 4507.949, 119.9575),
        };
        #endregion

        #region Level 2
        internal static List<WoWPoint> Horde_Plot18_AlchemyLab_Level2 = new List<WoWPoint>
        {
            new WoWPoint(5643.146, 4509.426, 120.0005),
            new WoWPoint(5640.879, 4504.467, 120.002),
            new WoWPoint(5643.131, 4501.704, 120.002),
            new WoWPoint(5646.818, 4503.573, 120.0013),
        };
        internal static List<WoWPoint> Horde_Plot19_AlchemyLab_Level2 = new List<WoWPoint>
        {
            new WoWPoint(5661.052, 4547.431, 120.0013),
            new WoWPoint(5666.495, 4546.308, 120.0013),
            new WoWPoint(5667.466, 4551.914, 119.9905),
            new WoWPoint(5664.221, 4552.516, 119.9996),
        };
        internal static List<WoWPoint> Horde_Plot20_AlchemyLab_Level2 = new List<WoWPoint>
        {
            new WoWPoint(5621.028, 4513.415, 120.0021),
            new WoWPoint(5615.941, 4512.358, 120.0021),
            new WoWPoint(5616.848, 4507.248, 119.9908),
            new WoWPoint(5620.248, 4507.834, 120.0017),
        };
        #endregion

        #region Level 3
        internal static List<WoWPoint> Horde_Plot18_AlchemyLab_Level3 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Horde_Plot19_AlchemyLab_Level3 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Horde_Plot20_AlchemyLab_Level3 = new List<WoWPoint>
        {
            //TODO
        };
        #endregion

        #endregion

        #region Enchanters Study

        #region Level 1
        internal static List<WoWPoint> Horde_Plot18_EnchantersStudy_Level1 = new List<WoWPoint>
        {
            new WoWPoint(5643.142, 4509.64, 120.1067),
            new WoWPoint(5639.574, 4503.922, 120.1098),
            new WoWPoint(5643.175, 4500.025, 120.1098),
            new WoWPoint(5647.299, 4504.517, 120.1071),
        };
        internal static List<WoWPoint> Horde_Plot19_EnchantersStudy_Level1 = new List<WoWPoint>
        {
            new WoWPoint(5660.983, 4547.448, 120.1055),
            new WoWPoint(5663.649, 4552.266, 120.1055),
            new WoWPoint(5668.636, 4551.175, 120.0987),
            new WoWPoint(5668.735, 4547.184, 120.1049),
        };
        internal static List<WoWPoint> Horde_Plot20_EnchantersStudy_Level1 = new List<WoWPoint>
        {
            new WoWPoint(5621.803, 4513.864, 120.1091),
            new WoWPoint(5615.729, 4512.985, 120.1091),
            new WoWPoint(5614.355, 4508.331, 120.1091),
            new WoWPoint(5619.001, 4507.144, 120.1022),
        };
        #endregion

        #region Level 2
        internal static List<WoWPoint> Horde_Plot18_EnchantersStudy_Level2 = new List<WoWPoint>
        {

        };
        internal static List<WoWPoint> Horde_Plot19_EnchantersStudy_Level2 = new List<WoWPoint>
        {

        };
        internal static List<WoWPoint> Horde_Plot20_EnchantersStudy_Level2 = new List<WoWPoint>
        {

        };
        #endregion

        #region Level 3
        internal static List<WoWPoint> Horde_Plot18_EnchantersStudy_Level3 = new List<WoWPoint>
        {
            new WoWPoint(5643.151, 4508.896, 120.0241),
            new WoWPoint(5640.957, 4503.771, 120.0241),
            new WoWPoint(5644.368, 4500.015, 120.02),
            new WoWPoint(5648.473, 4503.678, 120.023),
        };
        internal static List<WoWPoint> Horde_Plot19_EnchantersStudy_Level3 = new List<WoWPoint>
        {
            new WoWPoint(5660.997, 4547.609, 120.0205),
            new WoWPoint(5666.845, 4546.877, 120.0205),
            new WoWPoint(5668.91, 4551.284, 120.0185),
            new WoWPoint(5664.189, 4554.438, 120.0209),
        };
        internal static List<WoWPoint> Horde_Plot20_EnchantersStudy_Level3 = new List<WoWPoint>
        {
            new WoWPoint(5621.071, 4513.625, 120.0234),
            new WoWPoint(5615.636, 4511.868, 120.0234),
            new WoWPoint(5615.937, 4506.911, 120.0173),
            new WoWPoint(5621.751, 4506.618, 120.0242),
        };
        #endregion

        #endregion

        #region Scribes Quarters

        #region Level 1
        internal static List<WoWPoint> Horde_Plot18_ScribesQuarter_Level1 = new List<WoWPoint>
        {
            new WoWPoint(5642.551, 4510.19, 120.0849),
        };
        internal static List<WoWPoint> Horde_Plot19_ScribesQuarter_Level1 = new List<WoWPoint>
        {
            new WoWPoint(5660.298, 4546.919, 120.0818),
        };
        internal static List<WoWPoint> Horde_Plot20_ScribesQuarter_Level1 = new List<WoWPoint>
        {
            new WoWPoint(5622.744, 4515.659, 120.0844),
        };
        #endregion

        #region Level 2
        internal static List<WoWPoint> Horde_Plot18_ScribesQuarter_Level2 = new List<WoWPoint>
        {
            new WoWPoint(5643.289, 4510.766, 120.0842),
            new WoWPoint(5643.498, 4505.065, 120.0842),
        };
        internal static List<WoWPoint> Horde_Plot19_ScribesQuarter_Level2 = new List<WoWPoint>
        {
            new WoWPoint(5658.015, 4546.043, 120.0739),
            new WoWPoint(5664.303, 4548.788, 120.0823),
        };
        internal static List<WoWPoint> Horde_Plot20_ScribesQuarter_Level2 = new List<WoWPoint>
        {
            new WoWPoint(5623.19, 4515.393, 120.0839),
            new WoWPoint(5618.752, 4510.736, 120.0839),
        };
        #endregion

        #endregion

        #region Engineering Works

        #region Level 1
        internal static List<WoWPoint> Horde_Plot18_EngineeringWorks_Level1 = new List<WoWPoint>
        {
            new WoWPoint(5643.362, 4505.608, 120.1037),
        };
        internal static List<WoWPoint> Horde_Plot19_EngineeringWorks_Level1 = new List<WoWPoint>
        {
            new WoWPoint(5664.124, 4548.49, 120.1015),
        };
        internal static List<WoWPoint> Horde_Plot20_EngineeringWorks_Level1 = new List<WoWPoint>
        {
            new WoWPoint(5618.37, 4511.011, 120.1046),
        };
        #endregion

        #region Level 2
        internal static List<WoWPoint> Horde_Plot18_EngineeringWorks_Level2 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Horde_Plot19_EngineeringWorks_Level2 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Horde_Plot20_EngineeringWorks_Level2 = new List<WoWPoint>
        {
            //TODO
        };
        #endregion

        #region Level 3
        internal static List<WoWPoint> Horde_Plot18_EngineeringWorks_Level3 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Horde_Plot19_EngineeringWorks_Level3 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Horde_Plot20_EngineeringWorks_Level3 = new List<WoWPoint>
        {
            //TODO
        };
        #endregion

        #endregion

        #region Tailoring Emporium

        #region Level 1
        internal static List<WoWPoint> Horde_Plot18_TailoringEmporium_Level1 = new List<WoWPoint>
        {
            new WoWPoint(5643.272, 4504.816, 119.9495),
        };
        internal static List<WoWPoint> Horde_Plot19_TailoringEmporium_Level1 = new List<WoWPoint>
        {
            new WoWPoint(5664.21, 4548.489, 119.9463),
        };
        internal static List<WoWPoint> Horde_Plot20_TailoringEmporium_Level1 = new List<WoWPoint>
        {
            new WoWPoint(5618.079, 4510.942, 119.9479),
        };
        #endregion

        #region Level 2
        internal static List<WoWPoint> Horde_Plot18_TailoringEmporium_Level2 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Horde_Plot19_TailoringEmporium_Level2 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Horde_Plot20_TailoringEmporium_Level2 = new List<WoWPoint>
        {
            //TODO
        };
        #endregion

        #region Level 3
        internal static List<WoWPoint> Horde_Plot18_TailoringEmporium_Level3 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Horde_Plot19_TailoringEmporium_Level3 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Horde_Plot20_TailoringEmporium_Level3 = new List<WoWPoint>
        {
            //TODO
        };
        #endregion

        #endregion

        #region Store House

        #region Level 1

        internal static List<WoWPoint> Horde_Plot18_StoreHouse_Level1 = new List<WoWPoint>
        {
            new WoWPoint(5643.814, 4511.185, 120.0795),
        };
        internal static List<WoWPoint> Horde_Plot19_StoreHouse_Level1 = new List<WoWPoint>
        {
            new WoWPoint(5662.811, 4548.519, 120.0801),
        };
        internal static List<WoWPoint> Horde_Plot20_StoreHouse_Level1 = new List<WoWPoint>
        {
           new WoWPoint(5619.601, 4511.941, 120.0808),
        };
        
        #endregion

        #region Level 2
        internal static List<WoWPoint> Horde_Plot18_StoreHouse_Level2 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Horde_Plot19_StoreHouse_Level2 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Horde_Plot20_StoreHouse_Level2 = new List<WoWPoint>
        {
            //TODO
        };
        #endregion

        #region Level 3
        internal static List<WoWPoint> Horde_Plot18_StoreHouse_Level3 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Horde_Plot19_StoreHouse_Level3 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Horde_Plot20_StoreHouse_Level3 = new List<WoWPoint>
        {
            //TODO
        };
        #endregion

        #endregion

        #region Gem Boutique

        #region Level 1
        internal static List<WoWPoint> Horde_Plot18_GemBoutique_Level1 = new List<WoWPoint>
        {
           new WoWPoint(5643.241, 4506.282, 120.3154),
        };
        internal static List<WoWPoint> Horde_Plot19_GemBoutique_Level1 = new List<WoWPoint>
        {
            new WoWPoint(5663.585, 4548.461, 120.3224),
        };
        internal static List<WoWPoint> Horde_Plot20_GemBoutique_Level1 = new List<WoWPoint>
        {
            new WoWPoint(5619.759, 4511.728, 120.3176),
        };
        #endregion

        #region Level 2
        internal static List<WoWPoint> Horde_Plot18_GemBoutique_Level2 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Horde_Plot19_GemBoutique_Level2 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Horde_Plot20_GemBoutique_Level2 = new List<WoWPoint>
        {
            //TODO
        };
        #endregion

        #region Level 3
        internal static List<WoWPoint> Horde_Plot18_GemBoutique_Level3 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Horde_Plot19_GemBoutique_Level3 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Horde_Plot20_GemBoutique_Level3 = new List<WoWPoint>
        {
            //TODO
        };
        #endregion

        #endregion

        internal static readonly List<Blackspot> HordeBlackSpots = new List<Blackspot>
        {
            new Blackspot(new WoWPoint(5665.256,4549.886,120.41), 5f, 10f),
            new Blackspot(new WoWPoint(5643.812,4504.172,119.9041), 5f, 10f),
            new Blackspot(new WoWPoint(5618.075,4510.39,119.8379), 6f, 10f),
            new Blackspot(new WoWPoint(5524.729,4522.987,132.3107), 8f, 10f),
            new Blackspot(new WoWPoint(5649.103,4447.976,130.5355), 8f, 10f),
            new Blackspot(new WoWPoint(5576.156,4459.763,130.5877), 8f, 10f),
            new Blackspot(new WoWPoint(5691.774,4471.398,130.9299), 8f, 10f),
        };
    }
}
