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
        
        internal static readonly WoWPoint HordeMediumPlot22SafePoint = new WoWPoint(5551.187, 4521.895, 131.1648);
        internal static readonly WoWPoint HordeMediumPlot22Entrance = new WoWPoint(5540.719, 4521.523, 132.2882);
        internal static readonly WoWPoint HordeMediumPlot22WorkOrderPoint = new WoWPoint(5544.464,4516.489,132.2224);
        
        //
        internal static readonly WoWPoint HordeMediumPlot25SafePoint = new WoWPoint(5678.497, 4484.875, 130.1029);
        internal static readonly WoWPoint HordeMediumPlot25Entrance = new WoWPoint(5683.659, 4484.063, 131.0837);
        internal static readonly WoWPoint HordeMediumPlot25WorkOrderPoint = new WoWPoint(5685.945,4488.408,131.0202);
        
        internal static readonly WoWPoint HordeLargePlot23SafePoint = new WoWPoint(5588.029, 4483.632, 130.3161);
        internal static readonly WoWPoint HordeLargePlot23Entrance = new WoWPoint(5585.418, 4476.083, 130.671);
        
        internal static readonly WoWPoint HordeLargePlot24SafePoint = new WoWPoint(5641.472, 4468.675, 130.5273);
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

        internal static List<WoWPoint> Horde_Herb_LevelOne = new List<WoWPoint>
        {
            HordeHerbSafePoint,
            new WoWPoint(5411.076, 4546.272, 139.124),
            new WoWPoint(5410.355, 4538.49, 139.124),
            new WoWPoint(5415.397, 4534.234, 139.124),
            new WoWPoint(5420.36, 4535.431, 139.124),
            new WoWPoint(5421.288, 4538.967, 139.124),
            new WoWPoint(5420.495, 4544.562, 139.124),
            new WoWPoint(5417.282, 4549.279, 139.124),
        };

        private static readonly Vector2 Horde_Mine3_Blacklist1 = new Vector2(5448.65f, 4592.78f);
        private static readonly Vector2 Horde_Mine3_Blacklist2 = new Vector2(5468.288f, 4596.651f);
        private static readonly Vector2 Horde_Mine3_Blacklist3 = new Vector2(5479.805f, 4579.167f);
        private static readonly Vector2 Horde_Mine3_Blacklist4 = new Vector2(5467.495f, 4540.507f);

        internal static Vector2[] Horde_Mine3_BlackListPoly = { Horde_Mine3_Blacklist1, Horde_Mine3_Blacklist2, Horde_Mine3_Blacklist3, Horde_Mine3_Blacklist4 };



        internal static readonly Vector2[] Horde_Garrison_North_Poly2 = new[]
        {
            Horde_Garrison_North0,
            Horde_Garrison_North1,
            Horde_Garrison_North2,
            Horde_Garrison_North3,
            Horde_Garrison_North4
        };
        internal static readonly WoWPoint Horde_North_Exit = new WoWPoint(5627.083, 4468.313, 130.1959);
        internal static readonly WoWPoint Horde_North_Entrance = new WoWPoint(5693.042, 4499.6, 129.2168);
        internal static Polygon Horde_North = new Polygon
        {
            Name = "Horde_North",
            PlotIds= new List<int>{24,25},
            Vector2Array = Horde_Garrison_North_Poly2,
            Entrance=Horde_North_Entrance,
            Exit=Horde_North_Exit,
        };


        private static readonly Vector2 Horde_Garrison_Center0 = new Vector2(5607.157f, 4484.917f);
        private static readonly Vector2 Horde_Garrison_Center1 = new Vector2(5585.771f, 4502.96f);
        private static readonly Vector2 Horde_Garrison_Center2 = new Vector2(5632.274f, 4593.749f);
        private static readonly Vector2 Horde_Garrison_Center3 = new Vector2(5682.127f, 4561.89f);
        internal static readonly Vector2[] Horde_Garrison_Center_Poly2 = new[]
        {
            Horde_Garrison_North4,
            Horde_Garrison_Center0,
            Horde_Garrison_Center1,
            Horde_Garrison_Center2,
            Horde_Garrison_Center3,
             Horde_Garrison_North0,
        };
        internal static readonly WoWPoint Horde_Center_Exit = new WoWPoint(5698.433, 4511.054, 127.6866);
        internal static readonly WoWPoint Horde_Center_Entrance = new WoWPoint(5634.558, 4543.17, 119.1519);
        internal static Polygon Horde_Center = new Polygon
        {
            Name = "Horde_Center",
            PlotIds = new List<int> { 18, 19, 20 },
            Vector2Array = Horde_Garrison_Center_Poly2,
            Entrance = Horde_Center_Entrance,
            Exit = Horde_Center_Exit,
        };


        private static readonly Vector2 Horde_Garrison_CenterLarge0 = new Vector2(5510.888f, 471.715f);
        private static readonly Vector2 Horde_Garrison_CenterLarge1 = new Vector2(5504.897f, 4482.069f);
        internal static readonly Vector2[] Horde_Garrison_CenterLarge_Poly2 = new[]
        {
            Horde_Garrison_North3,
            Horde_Garrison_North4,
            Horde_Garrison_Center0,
            Horde_Garrison_Center1,
            Horde_Garrison_CenterLarge0,
            Horde_Garrison_CenterLarge1
        };
        internal static readonly WoWPoint Horde_LargeCenter_Exit = new WoWPoint(5522.989, 4484.512, 134.7289);
        internal static readonly WoWPoint Horde_LargeCenter_Entrance = new WoWPoint(5610.379, 4475.491, 130.127);
        internal static Polygon Horde_LargeCenter = new Polygon
        {
            Name = "Horde_LargeCenter",
            PlotIds = new List<int> { 23 },
            Vector2Array = Horde_Garrison_CenterLarge_Poly2,
            Entrance = Horde_LargeCenter_Entrance,
            Exit = Horde_LargeCenter_Exit,
        };

        private static readonly Vector2 Horde_Garrison_CenterMedium0 = new Vector2(5472.625f, 4531.584f);
        private static readonly Vector2 Horde_Garrison_CenterMedium1 = new Vector2(5528.418f, 4578.603f);
        internal static readonly Vector2[] Horde_Garrison_CenterMedium_Poly2 = new[]
        {
            Horde_Garrison_CenterMedium0,
            Horde_Garrison_CenterMedium1,
            Horde_Garrison_Center1,
            Horde_Garrison_CenterLarge1
        };
        internal static readonly WoWPoint Horde_CenterMedium_Exit = new WoWPoint(5512.66, 4562.018, 133.9657);
        internal static readonly WoWPoint Horde_CenterMedium_Entrance = new WoWPoint(5560.439, 4517.674, 130.6848);
        internal static Polygon Horde_CenterMedium = new Polygon
        {
            Name = "Horde_CenterMedium",
            PlotIds = new List<int> { 22 },
            Vector2Array = Horde_Garrison_CenterMedium_Poly2,
            Entrance = Horde_CenterMedium_Entrance,
            Exit = Horde_CenterMedium_Exit,
        };

        private static readonly Vector2 Horde_Garrison_FishShack0 = new Vector2(5486.694f, 4700.894f);
        private static readonly Vector2 Horde_Garrison_FishShack1 = new Vector2(5408.967f, 4647.949f);
        internal static readonly Vector2[] Horde_Garrison_FishShack_Poly2 = new[]
        {
            Horde_Garrison_FishShack0,
            Horde_Garrison_FishShack1,
            Horde_Garrison_CenterMedium0,
            Horde_Garrison_CenterMedium1
        };
        internal static readonly WoWPoint Horde_FishShack_Exit = new WoWPoint(5462.186, 4606.305, 135.0662);
        internal static readonly WoWPoint Horde_FishShack_Entrance = new WoWPoint(5507.324, 4570.137, 134.7262);
        internal static Polygon Horde_FishShack = new Polygon
        {
            Name = "Horde_FishShack",
            PlotIds = new List<int> { 67 },
            Vector2Array = Horde_Garrison_FishShack_Poly2,
            Entrance = Horde_FishShack_Entrance,
            Exit = Horde_FishShack_Exit,
        };

        private static readonly Vector2 Horde_Garrison_Garden0 = new Vector2(5361.443f, 4591.392f);
        private static readonly Vector2 Horde_Garrison_Garden1 = new Vector2(5421.232f, 4492.331f);
        internal static readonly Vector2[] Horde_Garrison_Garden_Poly2 = new[]
        {
            Horde_Garrison_Garden0,
            Horde_Garrison_Garden1,
            Horde_Garrison_CenterMedium0,
            Horde_Garrison_FishShack1
        };
        internal static readonly WoWPoint Horde_Garden_Exit = new WoWPoint(5452.741, 4519.041, 138.5737);
        internal static readonly WoWPoint Horde_Garden_Entrance = new WoWPoint(5443.799, 4576.659, 135.285);
        internal static Polygon Horde_Garden = new Polygon
        {
            Name = "Horde_Garden",
            PlotIds = new List<int> { 63 },
            Vector2Array = Horde_Garrison_Garden_Poly2,
            Entrance = Horde_Garden_Entrance,
            Exit = Horde_Garden_Exit,
        };

        private static readonly Vector2 Horde_Garrison_Mine0 = new Vector2(5500.182f, 4414.262f);
        internal static readonly Vector2[] Horde_Garrison_Mine_Poly2 = new[]
        {
            Horde_Garrison_Mine0,
            Horde_Garrison_Garden1,
            Horde_Garrison_CenterMedium0,
            Horde_Garrison_CenterLarge0,
            Horde_Garrison_CenterLarge1
        };
        internal static readonly WoWPoint Horde_Mine_Exit = new WoWPoint(5501.796, 4470.081, 139.482);
        internal static readonly WoWPoint Horde_Mine_Entrance = new WoWPoint(5461.799, 4508.809, 138.9827);
        internal static Polygon Horde_Mine = new Polygon
        {
            Name = "Horde_Mine",
            PlotIds = new List<int> { 59 },
            Vector2Array = Horde_Garrison_Mine_Poly2,
            Entrance = Horde_Mine_Entrance,
            Exit = Horde_Mine_Exit,
        };

        private static readonly Vector2 Horde_Garrison_West0 = new Vector2(5566.979f, 4640.736f);
        private static readonly Vector2 Horde_Garrison_West1 = new Vector2(5529.119f, 4721.634f);
        private static readonly Vector2 Horde_Garrison_West2 = new Vector2(5633.26f, 4668.528f);
        internal static readonly Vector2[] Horde_Garrison_West_Poly2 = new[]
        {
            Horde_Garrison_West0,
            Horde_Garrison_West1,
            Horde_Garrison_West2,
            Horde_Garrison_Center2,
            Horde_Garrison_Center3
        };
        internal static readonly WoWPoint Horde_West_Exit = new WoWPoint();
        internal static readonly WoWPoint Horde_West_Entrance = new WoWPoint();

        internal static readonly Vector2[] Horde_Garrison_Poly2 = new[]
        {
            Horde_Garrison_West0,
            Horde_Garrison_West1,
            Horde_Garrison_CenterMedium1,
            Horde_Garrison_Center1,
            Horde_Garrison_Center2
        };
        internal static readonly WoWPoint Horde_Garrison_Exit = new WoWPoint(5594.325, 4532.786, 126.3277);
        internal static readonly WoWPoint Horde_Garrison_Entrance = new WoWPoint(5603.863, 4599.905, 136.591);
        internal static Polygon Horde_Garrison = new Polygon
        {
            Name = "Horde_Garrison",
            PlotIds = new List<int>(),
            Vector2Array = Horde_Garrison_Poly2,
            Entrance = HordeGarrisonEntrance,
            Exit = Horde_Garrison_Exit,
        };


    }
}
