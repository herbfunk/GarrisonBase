using System.Collections.Generic;
using Styx;
using Styx.CommonBot.Profiles;

namespace Herbfunk.GarrisonBase
{
    public static partial class MovementCache
    {
        internal static readonly WoWPoint AllianceSmallPlot18SafePoint = new WoWPoint(1822.733, 220.1028, 72.12727);
        internal static readonly WoWPoint AllianceSmallPlot18Entrance = new WoWPoint(1821.443, 226.7355, 72.9455);

        internal static readonly WoWPoint AllianceSmallPlot19SafePoint = new WoWPoint(1826.584, 207.0568, 71.9642);
        internal static readonly WoWPoint AllianceSmallPlot19Entrance = new WoWPoint(1828.715, 201.7769, 72.74796);

        internal static readonly WoWPoint AllianceSmallPlot20SafePoint = new WoWPoint(1800.6, 198.2247, 70.07494);
        internal static readonly WoWPoint AllianceSmallPlot20Entrance = new WoWPoint(1802.908, 193.6203, 70.83499);

        internal static readonly WoWPoint AllianceMediumPlot22SafePoint = new WoWPoint(1867.312, 293.0235, 81.63145);
        internal static readonly WoWPoint AllianceMediumPlot22Entrance = new WoWPoint(1866.78, 301.1098, 81.66068);

        internal static readonly WoWPoint AllianceLargePlot23SafePoint = new WoWPoint(1885.174, 241.8866, 76.63958);
        internal static readonly WoWPoint AllianceLargePlot23Entrance = new WoWPoint(1896.116, 237.473, 76.64002);

        internal static readonly WoWPoint AllianceMediumPlot25SafePoint = new WoWPoint(1869.486, 202.0029, 77.9813);
        internal static readonly WoWPoint AllianceMediumPlot25Entrance = new WoWPoint(1876.818, 196.8224, 78.05737);

        internal static readonly WoWPoint AllianceLargePlot24SafePoint = new WoWPoint(1843.467, 263.6939, 76.63972);
        internal static readonly WoWPoint AllianceLargePlot24Entrance = new WoWPoint(1833.216, 271.6732, 76.63962);

        internal static readonly WoWPoint AllianceHerbSafePoint = new WoWPoint(1854.805, 145.9212, 78.29183);
        internal static readonly WoWPoint AllianceMineSafePoint = new WoWPoint(1909.829, 92.21799, 83.52757);

        internal static readonly WoWPoint AlliancePrimalTraderLevel2 = new WoWPoint(1849.894,242.8204,76.63979);
        internal static readonly WoWPoint AlliancePrimalTraderLevel3 = new WoWPoint(1919.877,324.0956,88.96598);


        internal static readonly WoWPoint AllianceSellRepairNpc = new WoWPoint(1860.432, 260.8574, 76.63974);
        internal static readonly WoWPoint AllianceFlightPathNpc = new WoWPoint(1865.812, 220.7363, 76.69885);

        internal static readonly WoWPoint AllianceGarrisonEntrance = new WoWPoint(1924.372, 296.1701, 88.966);

        internal static List<WoWPoint> Alliance_Herb_LevelOne = new List<WoWPoint>
        {
            new WoWPoint(1829.674, 151.3166, 76.60562),
            new WoWPoint(1824.084, 158.0668, 76.60562),
            new WoWPoint(1815.551, 157.7762, 76.60562),
            new WoWPoint(1816.689, 144.1966, 76.60562),
            new WoWPoint(1825, 144.8783, 76.60562),
        };

        #region Mining Paths
        /* 
         * new WoWPoint(1913.269, 57.64397, 32.5749)
         * 
         * First Shaft
         * new WoWPoint(1911.077, 111.9462, 25.61837) -- 48f radius
         * new WoWPoint(1871.701, 118.334, 21.48756) -- 41f radius
         * new WoWPoint(1849.546, 164.6769, 17.30295) -- 41f radius
         * 
         * Second Shaft
         * new WoWPoint(1980.759, 123.6748, 18.69657) -- 25f radius
         * new WoWPoint(2007.479, 76.96797, 12.70772) -- 31f radius
         * new WoWPoint(1968.581, 74.60398, 13.89289) -- 27f radius
         * 
         * Third Shaft
         *  new WoWPoint(1898.615, 157.8819, 23.2644) -- 36f radius
         *  new WoWPoint(1933.587, 208.5867, 20.71699) -- 36f radius
         *  
         */

        internal static List<WoWPoint> Alliance_Mine_LevelOne = new List<WoWPoint>
        {
            new WoWPoint(1913.269, 57.64397, 32.5749) ,
            new WoWPoint(1911.077, 111.9462, 25.61837) ,
            new WoWPoint(1871.701, 118.334, 21.48756) ,
            new WoWPoint(1846.136, 156.506, 17.91364),


            //new WoWPoint(1889.203, 85.08511, 84.41315),
            //new WoWPoint(1867.766, 70.31022, 73.20315),
            //new WoWPoint(1868.375, 47.56387, 63.69995),
            //new WoWPoint(1899.638, 50.47875, 49.03188),
            //new WoWPoint(1923.146, 49.2985, 44.78643),
            //new WoWPoint(1922.795, 68.88879, 38.1831),

            //new WoWPoint(1913.671, 70.46033, 33.45458),

            //new WoWPoint(1916.361, 82.01344, 31.24682),
            //new WoWPoint(1913.865, 109.0081, 26.28172),
            //new WoWPoint(1885.752, 116.5619, 23.6226),
            //new WoWPoint(1856.457, 123.4885, 21.93341),
            //new WoWPoint(1842.731, 178.8193, 13.13167),
            //new WoWPoint(1845.789, 153.685, 18.70906),
            //new WoWPoint(1871.184, 175.439, 10.82855),
            //new WoWPoint(1846.742, 153.9697, 18.64499),
            //new WoWPoint(1862.108, 120.4472, 21.35746),
            //new WoWPoint(1915.871, 109.2227, 26.26868),
            //new WoWPoint(1922.651, 77.61671, 32.70341),
            //new WoWPoint(1923.254, 65.63617, 38.14819),
            //new WoWPoint(1922.807, 49.64631, 44.79056),
            //new WoWPoint(1872.178, 46.4214, 62.56678),
            //new WoWPoint(1864.854, 61.88112, 69.25108),
            //new WoWPoint(1887.615, 84.1413, 84.41303),
        };

        internal static List<WoWPoint> Alliance_Mine_LevelTwo = new List<WoWPoint>
        {
            new WoWPoint(1913.269, 57.64397, 32.5749) ,
            new WoWPoint(1911.077, 111.9462, 25.61837) ,
            new WoWPoint(1871.701, 118.334, 21.48756) ,
            new WoWPoint(1846.136, 156.506, 17.91364),

            new WoWPoint(1980.759, 123.6748, 18.69657),
            new WoWPoint(2007.479, 76.96797, 12.70772) ,
            new WoWPoint(1968.581, 74.60398, 13.89289) ,

            //new WoWPoint(1889.203, 85.08511, 84.41315),
            //new WoWPoint(1867.766, 70.31022, 73.20315),
            //new WoWPoint(1868.375, 47.56387, 63.69995),
            //new WoWPoint(1899.638, 50.47875, 49.03188),
            //new WoWPoint(1923.146, 49.2985, 44.78643),
            //new WoWPoint(1922.795, 68.88879, 38.1831),

            //new WoWPoint(1913.671, 70.46033, 33.45458),

            //new WoWPoint(1916.361, 82.01344, 31.24682),
            //new WoWPoint(1913.865, 109.0081, 26.28172),
            //new WoWPoint(1885.752, 116.5619, 23.6226),
            //new WoWPoint(1856.457, 123.4885, 21.93341),
            //new WoWPoint(1842.731, 178.8193, 13.13167),
            //new WoWPoint(1845.789, 153.685, 18.70906),
            //new WoWPoint(1871.184, 175.439, 10.82855),
            //new WoWPoint(1846.742, 153.9697, 18.64499),
            //new WoWPoint(1862.108, 120.4472, 21.35746),
            //new WoWPoint(1915.871, 109.2227, 26.26868),

            //new WoWPoint(1969.197, 127.8173, 19.04734),
            //new WoWPoint(1992.767, 104.8804, 19.61248),
            //new WoWPoint(2001.758, 78.6369, 14.26775),
            //new WoWPoint(1973.02, 75.89962, 14.19517),
            //new WoWPoint(1969.463, 98.07942, 17.43518),
            //new WoWPoint(1984.092, 120.2357, 18.91512),
            //new WoWPoint(1950.845, 126.7416, 22.12422),
            //new WoWPoint(1919.41, 110.0222, 26.36375),

            
            //new WoWPoint(1915.871, 109.2227, 26.26868),
            //new WoWPoint(1922.651, 77.61671, 32.70341),
            //new WoWPoint(1923.254, 65.63617, 38.14819),
            //new WoWPoint(1922.807, 49.64631, 44.79056),
            //new WoWPoint(1872.178, 46.4214, 62.56678),
            //new WoWPoint(1864.854, 61.88112, 69.25108),
            //new WoWPoint(1887.615, 84.1413, 84.41303),
        };

        internal static List<WoWPoint> Alliance_Mine_LevelThree = new List<WoWPoint>
        {
            new WoWPoint(1913.269, 57.64397, 32.5749) ,
            new WoWPoint(1911.077, 111.9462, 25.61837) ,
            new WoWPoint(1871.701, 118.334, 21.48756) ,
            new WoWPoint(1846.136, 156.506, 17.91364),

            new WoWPoint(1980.759, 123.6748, 18.69657),
            new WoWPoint(2007.479, 76.96797, 12.70772) ,
            new WoWPoint(1968.581, 74.60398, 13.89289) ,

            new WoWPoint(1898.615, 157.8819, 23.2644),
            new WoWPoint(1933.587, 208.5867, 20.71699),
            //new WoWPoint(1889.203, 85.08511, 84.41315),
            //new WoWPoint(1867.766, 70.31022, 73.20315),
            //new WoWPoint(1868.375, 47.56387, 63.69995),
            //new WoWPoint(1899.638, 50.47875, 49.03188),
            //new WoWPoint(1923.146, 49.2985, 44.78643),
            //new WoWPoint(1922.795, 68.88879, 38.1831),

            //new WoWPoint(1913.671, 70.46033, 33.45458),

            //new WoWPoint(1916.361, 82.01344, 31.24682),
            //new WoWPoint(1913.865, 109.0081, 26.28172),
            //new WoWPoint(1885.752, 116.5619, 23.6226),
            //new WoWPoint(1856.457, 123.4885, 21.93341),
            //new WoWPoint(1842.731, 178.8193, 13.13167),
            //new WoWPoint(1845.789, 153.685, 18.70906),
            //new WoWPoint(1871.184, 175.439, 10.82855),
            //new WoWPoint(1846.742, 153.9697, 18.64499),
            //new WoWPoint(1862.108, 120.4472, 21.35746),

            ////
            //new WoWPoint(1894.704, 143.9975, 22.97749),
            //new WoWPoint(1908.806, 187.163, 19.75881),
            //new WoWPoint(1943.453, 225.6138, 20.54592),
            //new WoWPoint(1901.836, 173.7192, 21.2011),
            //new WoWPoint(1892.738, 118.4613, 23.72561),


            //new WoWPoint(1915.871, 109.2227, 26.26868),
            //new WoWPoint(1969.197, 127.8173, 19.04734),
            //new WoWPoint(1992.767, 104.8804, 19.61248),
            //new WoWPoint(2001.758, 78.6369, 14.26775),
            //new WoWPoint(1973.02, 75.89962, 14.19517),
            //new WoWPoint(1969.463, 98.07942, 17.43518),
            //new WoWPoint(1984.092, 120.2357, 18.91512),
            //new WoWPoint(1950.845, 126.7416, 22.12422),
            //new WoWPoint(1919.41, 110.0222, 26.36375),

            
            //new WoWPoint(1915.871, 109.2227, 26.26868),
            //new WoWPoint(1922.651, 77.61671, 32.70341),
            //new WoWPoint(1923.254, 65.63617, 38.14819),
            //new WoWPoint(1922.807, 49.64631, 44.79056),
            //new WoWPoint(1872.178, 46.4214, 62.56678),
            //new WoWPoint(1864.854, 61.88112, 69.25108),
            //new WoWPoint(1887.615, 84.1413, 84.41303),
        };
        
        #endregion

        internal static readonly WoWPoint AllianceSalvageYardPlot18 = new WoWPoint(1819.787, 227.919, 72.93478);
        internal static readonly WoWPoint AllianceSalvageYardPlot19 = new WoWPoint(1830.51, 200.0502, 72.74524);
        internal static readonly WoWPoint AllianceSalvageYardPlot20=new WoWPoint(1803.424, 192.5173, 70.83544);


        #region Dwarven Bunkers

        internal static List<WoWPoint> Alliance_WarMill_Plot24_Level2 = new List<WoWPoint>
        {
            new WoWPoint(1824.36, 278.8286, 78.51082),
            new WoWPoint(1812.681, 287.7504, 76.96829),
            new WoWPoint(1815.465, 299.3411, 78.4222),
            new WoWPoint(1803.549, 297.9706, 79.85912),
        };

        internal static List<WoWPoint> Alliance_WarMill_Plot23_Level2 = new List<WoWPoint>
        {
            new WoWPoint(1906.482, 233.3928, 78.52262),
            new WoWPoint(1921.283, 227.5641, 76.97861),
            new WoWPoint(1921.863, 216.0392, 78.43543),
            new WoWPoint(1926.895, 216.7648, 79.86779),
        };
        internal static List<WoWPoint> Alliance_WarMill_Plot24_Level3 = new List<WoWPoint>
        {
            //TODO!!
        };

        internal static List<WoWPoint> Alliance_WarMill_Plot23_Level3 = new List<WoWPoint>
        {
            //TODO!!
        };
        
        #endregion

        #region Trade Post

        internal static List<WoWPoint> Alliance_Plot22_TradePost_Level1 = new List<WoWPoint>
        {
            new WoWPoint(1865.945, 310.084, 82.24733),
        };
        internal static List<WoWPoint> Alliance_Plot25_TradePost_Level1 = new List<WoWPoint>
        {
            new WoWPoint(1883.729, 191.4884, 78.64293),
        };
        internal static List<WoWPoint> Alliance_Plot22_TradePost_Level2 = new List<WoWPoint>
        {
            new WoWPoint(1865.944, 311.0232, 83.32607),
            new WoWPoint(1864.96, 315.6888, 83.32607),
        };
        internal static List<WoWPoint> Alliance_Plot25_TradePost_Level2 = new List<WoWPoint>
        {
            new WoWPoint(1883.594, 192.1426, 79.72157),
            new WoWPoint(1890.991, 187.2217, 79.72182),
        };
        internal static List<WoWPoint> Alliance_Plot22_TradePost_Level3 = new List<WoWPoint>
        {
            new WoWPoint(1866.712, 311.7191, 83.3259),
            new WoWPoint(1860.573, 315.4993, 83.32614),
        };
        internal static List<WoWPoint> Alliance_Plot25_TradePost_Level3 = new List<WoWPoint>
        {
            new WoWPoint(1885.674, 189.499, 79.72184),
            new WoWPoint(1892.04, 192.2097, 79.72184),
        };
        
        #endregion

        #region Barn

        internal static List<WoWPoint> Alliance_Plot22_Barn_Level2 = new List<WoWPoint>
        {
            new WoWPoint(1866.953, 297.2848, 81.66016),
            new WoWPoint(1865.661, 309.8015, 82.27261),
            new WoWPoint(1865.039, 318.2485, 82.27153),
        };
        internal static List<WoWPoint> Alliance_Plot25_Barn_Level2 = new List<WoWPoint>
        {
            new WoWPoint(1876.044, 197.2052, 78.05712),
            new WoWPoint(1884.703, 191.6729, 78.66826),
            new WoWPoint(1892.027, 186.6466, 78.66734),
        };
        internal static List<WoWPoint> Alliance_Plot22_Barn_Level3 = new List<WoWPoint>
        {
            new WoWPoint(1866.662, 298.6634, 81.66038),
            new WoWPoint(1865.716, 309.0012, 81.89859),
            new WoWPoint(1863.15, 323.2336, 81.89859),
        };
        internal static List<WoWPoint> Alliance_Plot25_Barn_Level3 = new List<WoWPoint>
        {
            new WoWPoint(1874.823, 197.8349, 78.05936),
            new WoWPoint(1884.341, 191.9854, 78.29198),
            new WoWPoint(1896.735, 184.7108, 78.29198),
        };

        #endregion

        #region The Forge

        #region Level 1

        internal static List<WoWPoint> Alliance_Plot18_Forge_Level1 = new List<WoWPoint>
        {
            new WoWPoint(1819.69, 227.1223, 72.96257),
        };
        internal static List<WoWPoint> Alliance_Plot19_Forge_Level1 = new List<WoWPoint>
        {
            new WoWPoint(1830.243, 201.0425, 72.77658),
        };
        internal static List<WoWPoint> Alliance_Plot20_Forge_Level1 = new List<WoWPoint>
        {
           new WoWPoint(1803.589, 193.1322, 70.86319),
        };

        #endregion

        #region Level 2

        internal static List<WoWPoint> Alliance_Plot18_Forge_Level2 = new List<WoWPoint>
        {
           //TODO
        };
        internal static List<WoWPoint> Alliance_Plot19_Forge_Level2 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot20_Forge_Level2 = new List<WoWPoint>
        {
            //TODO
        };

        #endregion

        #region Level 3

        internal static List<WoWPoint> Alliance_Plot18_Forge_Level3 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot19_Forge_Level3 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot20_Forge_Level3 = new List<WoWPoint>
        {
            //TODO
        };

        #endregion

        #endregion

        #region Tannery Special Movement

        #region Level 1
        internal static List<WoWPoint> Alliance_Plot18_Tannery_Level1 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot19_Tannery_Level1 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot20_Tannery_Level1 = new List<WoWPoint>
        {
            //TODO
        };
        #endregion

        #region Level 2
        internal static List<WoWPoint> Alliance_Plot18_Tannery_Level2 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot19_Tannery_Level2 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot20_Tannery_Level2 = new List<WoWPoint>
        {
            //TODO
        };
        #endregion

        #region Level 3
        internal static List<WoWPoint> Alliance_Plot18_Tannery_Level3 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot19_Tannery_Level3 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot20_Tannery_Level3 = new List<WoWPoint>
        {
            //TODO
        };
        #endregion

        #endregion

        #region Alchemy Lab

        #region Level 1
        internal static List<WoWPoint> Alliance_Plot18_AlchemyLab_Level1 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot19_AlchemyLab_Level1 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot20_AlchemyLab_Level1 = new List<WoWPoint>
        {
            //TODO
        };
        #endregion

        #region Level 2
        internal static List<WoWPoint> Alliance_Plot18_AlchemyLab_Level2 = new List<WoWPoint>
        {
            new WoWPoint(1822.562, 227.7075, 72.97662),
        };
        internal static List<WoWPoint> Alliance_Plot19_AlchemyLab_Level2 = new List<WoWPoint>
        {
            new WoWPoint(1828.326, 200.6566, 72.75695),
        };
        internal static List<WoWPoint> Alliance_Plot20_AlchemyLab_Level2 = new List<WoWPoint>
        {
            new WoWPoint(1802.076, 192.555, 70.87757),
        };
        #endregion

        #region Level 3
        internal static List<WoWPoint> Alliance_Plot18_AlchemyLab_Level3 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot19_AlchemyLab_Level3 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot20_AlchemyLab_Level3 = new List<WoWPoint>
        {
            //TODO
        };
        #endregion

        #endregion

        #region Enchanters Study

        #region Level 1
        internal static List<WoWPoint> Alliance_Plot18_EnchantersStudy_Level1 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot19_EnchantersStudy_Level1 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot20_EnchantersStudy_Level1 = new List<WoWPoint>
        {
            //TODO
        };
        #endregion

        #region Level 2
        internal static List<WoWPoint> Alliance_Plot18_EnchantersStudy_Level2 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot19_EnchantersStudy_Level2 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot20_EnchantersStudy_Level2 = new List<WoWPoint>
        {
            //TODO
        };
        #endregion

        #region Level 3
        internal static List<WoWPoint> Alliance_Plot18_EnchantersStudy_Level3 = new List<WoWPoint>
        {
            new WoWPoint(1819.439, 227.3108, 73.03439),
            new WoWPoint(1823.86, 232.2389, 72.94739),
        };
        internal static List<WoWPoint> Alliance_Plot19_EnchantersStudy_Level3 = new List<WoWPoint>
        {
            new WoWPoint(1830.014, 201.1206, 72.80533),
            new WoWPoint(1827.52, 196.2728, 72.75903),
        };
        internal static List<WoWPoint> Alliance_Plot20_EnchantersStudy_Level3 = new List<WoWPoint>
        {
            new WoWPoint(1803.312, 193.8891, 70.84586),
            new WoWPoint(1800.87, 187.9106, 70.84789),
        };
        #endregion

        #endregion

        #region Scribes Quarters

        #region Level 1
        internal static List<WoWPoint> Alliance_Plot18_ScribesQuarter_Level1 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot19_ScribesQuarter_Level1 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot20_ScribesQuarter_Level1 = new List<WoWPoint>
        {
            //TODO
        };
        #endregion

        #region Level 2
        internal static List<WoWPoint> Alliance_Plot18_ScribesQuarter_Level2 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot19_ScribesQuarter_Level2 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot20_ScribesQuarter_Level2 = new List<WoWPoint>
        {
            //TODO
        };
        #endregion

        #region Level 3
        internal static List<WoWPoint> Alliance_Plot18_ScribesQuarter_Level3 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot19_ScribesQuarter_Level3 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot20_ScribesQuarter_Level3 = new List<WoWPoint>
        {
            //TODO
        };
        #endregion

        #endregion

        #region Engineering Works

        #region Level 1
        internal static List<WoWPoint> Alliance_Plot18_EngineeringWorks_Level1 = new List<WoWPoint>
        {
            new WoWPoint(1818.893, 228.4791, 72.94559),
        };
        internal static List<WoWPoint> Alliance_Plot19_EngineeringWorks_Level1 = new List<WoWPoint>
        {
            new WoWPoint(1830.636, 198.3738, 72.75722),
        };
        internal static List<WoWPoint> Alliance_Plot20_EngineeringWorks_Level1 = new List<WoWPoint>
        {
            new WoWPoint(1804.518, 190.968, 70.84632),
        };
        #endregion

        #region Level 2
        internal static List<WoWPoint> Alliance_Plot18_EngineeringWorks_Level2 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot19_EngineeringWorks_Level2 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot20_EngineeringWorks_Level2 = new List<WoWPoint>
        {
            //TODO
        };
        #endregion

        #region Level 3
        internal static List<WoWPoint> Alliance_Plot18_EngineeringWorks_Level3 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot19_EngineeringWorks_Level3 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot20_EngineeringWorks_Level3 = new List<WoWPoint>
        {
            //TODO
        };
        #endregion

        #endregion

        #region Tailoring Emporium

        #region Level 1
        internal static List<WoWPoint> Alliance_Plot18_TailoringEmporium_Level1 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot19_TailoringEmporium_Level1 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot20_TailoringEmporium_Level1 = new List<WoWPoint>
        {
            //TODO
        };
        #endregion

        #region Level 2
        internal static List<WoWPoint> Alliance_Plot18_TailoringEmporium_Level2 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot19_TailoringEmporium_Level2 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot20_TailoringEmporium_Level2 = new List<WoWPoint>
        {
            //TODO
        };
        #endregion

        #region Level 3
        internal static List<WoWPoint> Alliance_Plot18_TailoringEmporium_Level3 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot19_TailoringEmporium_Level3 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot20_TailoringEmporium_Level3 = new List<WoWPoint>
        {
            //TODO
        };
        #endregion

        #endregion

        #region Store House

        #region Level 1

        internal static List<WoWPoint> Alliance_Plot18_StoreHouse_Level1 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot19_StoreHouse_Level1 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot20_StoreHouse_Level1 = new List<WoWPoint>
        {
            //TODO
        };

        #endregion

        #region Level 2
        internal static List<WoWPoint> Alliance_Plot18_StoreHouse_Level2 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot19_StoreHouse_Level2 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot20_StoreHouse_Level2 = new List<WoWPoint>
        {
            //TODO
        };
        #endregion

        #region Level 3
        internal static List<WoWPoint> Alliance_Plot18_StoreHouse_Level3 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot19_StoreHouse_Level3 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot20_StoreHouse_Level3 = new List<WoWPoint>
        {
            //TODO
        };
        #endregion

        #endregion

        #region Gem Boutique

        #region Level 1
        internal static List<WoWPoint> Alliance_Plot18_GemBoutique_Level1 = new List<WoWPoint>
        {
            new WoWPoint(1822.369, 228.9625, 72.9454),
        };
        internal static List<WoWPoint> Alliance_Plot19_GemBoutique_Level1 = new List<WoWPoint>
        {
            new WoWPoint(1827.3, 200.1062, 72.75676),
        };
        internal static List<WoWPoint> Alliance_Plot20_GemBoutique_Level1 = new List<WoWPoint>
        {
           new WoWPoint(1801.436, 192.0244, 70.84675),
        };
        #endregion

        #region Level 2
        internal static List<WoWPoint> Alliance_Plot18_GemBoutique_Level2 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot19_GemBoutique_Level2 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot20_GemBoutique_Level2 = new List<WoWPoint>
        {
            //TODO
        };
        #endregion

        #region Level 3
        internal static List<WoWPoint> Alliance_Plot18_GemBoutique_Level3 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot19_GemBoutique_Level3 = new List<WoWPoint>
        {
            //TODO
        };
        internal static List<WoWPoint> Alliance_Plot20_GemBoutique_Level3 = new List<WoWPoint>
        {
            //TODO
        };
        #endregion

        #endregion

        internal static readonly List<Blackspot> AllianceBlackSpots = new List<Blackspot>
        {
            new Blackspot(new WoWPoint(1830.085,197.1292,72.67456), 5f, 10f),
            new Blackspot(new WoWPoint(1804.306,189.4055,70.70669), 5f, 10f),
            new Blackspot(new WoWPoint(1819.719,231.1095,72.64378), 5f, 10f),
            new Blackspot(new WoWPoint(1893.243,187.0173,78.22331), 9f, 10f),
            new Blackspot(new WoWPoint(1822.411,280.115,77.7338), 9f, 10f),
        };
    }
}
