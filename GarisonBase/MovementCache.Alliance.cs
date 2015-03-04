using System.Collections.Generic;
using Styx;

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

        internal static readonly WoWPoint AllianceMediumPlot22SafePoint = new WoWPoint(1867.064, 298.5665, 81.66045);
        internal static readonly WoWPoint AllianceMediumPlot22Entrance = new WoWPoint(1866.179, 307.4393, 81.98087);

        internal static readonly WoWPoint AllianceLargePlot23SafePoint = new WoWPoint(1892.103, 239.2194, 76.63974);
        internal static readonly WoWPoint AllianceLargePlot23Entrance = new WoWPoint(1899.569, 236.3036, 76.96661);

        internal static readonly WoWPoint AllianceMediumPlot25SafePoint = new WoWPoint(1877.974, 195.6795, 78.05623);
        internal static readonly WoWPoint AllianceMediumPlot25Entrance = new WoWPoint(1879.877, 193.9648, 78.60085);
        
        internal static readonly WoWPoint AllianceLargePlot24SafePoint = new WoWPoint(1833.884, 270.7426, 76.63985);
        internal static readonly WoWPoint AllianceLargePlot24Entrance = new WoWPoint(1830.601, 274.0393, 76.95255);

        internal static readonly WoWPoint AllianceHerbSafePoint = new WoWPoint(1854.805, 145.9212, 78.29183);
        internal static readonly WoWPoint AllianceMineSafePoint = new WoWPoint(1909.829, 92.21799, 83.52757);

        internal static readonly WoWPoint AllianceSellRepairNpc = new WoWPoint(1860.432, 260.8574, 76.63974);

        internal static readonly WoWPoint AllianceGarrisonEntrance = new WoWPoint(1924.372, 296.1701, 88.966);

        internal static List<WoWPoint> Alliance_Herb_LevelOne = new List<WoWPoint>
        {
            new WoWPoint(1829.674, 151.3166, 76.60562),
            new WoWPoint(1824.084, 158.0668, 76.60562),
            new WoWPoint(1815.551, 157.7762, 76.60562),
            new WoWPoint(1816.689, 144.1966, 76.60562),
            new WoWPoint(1825, 144.8783, 76.60562),
        };

        internal static List<WoWPoint> Alliance_Mine_LevelOne = new List<WoWPoint>
        {
            new WoWPoint(1889.203, 85.08511, 84.41315),
            new WoWPoint(1867.766, 70.31022, 73.20315),
            new WoWPoint(1868.375, 47.56387, 63.69995),
            new WoWPoint(1899.638, 50.47875, 49.03188),
            new WoWPoint(1923.146, 49.2985, 44.78643),
            new WoWPoint(1922.795, 68.88879, 38.1831),
            new WoWPoint(1916.361, 82.01344, 31.24682),
            new WoWPoint(1913.865, 109.0081, 26.28172),
            new WoWPoint(1885.752, 116.5619, 23.6226),
            new WoWPoint(1856.457, 123.4885, 21.93341),
            new WoWPoint(1842.731, 178.8193, 13.13167),
            new WoWPoint(1845.789, 153.685, 18.70906),
            new WoWPoint(1871.184, 175.439, 10.82855),
            new WoWPoint(1846.742, 153.9697, 18.64499),
            new WoWPoint(1862.108, 120.4472, 21.35746),
            new WoWPoint(1915.871, 109.2227, 26.26868),
            new WoWPoint(1922.651, 77.61671, 32.70341),
            new WoWPoint(1923.254, 65.63617, 38.14819),
            new WoWPoint(1922.807, 49.64631, 44.79056),
            new WoWPoint(1872.178, 46.4214, 62.56678),
            new WoWPoint(1864.854, 61.88112, 69.25108),
            new WoWPoint(1887.615, 84.1413, 84.41303),
        };

        internal static List<WoWPoint> Alliance_Mine_LevelTwo = new List<WoWPoint>
        {
            new WoWPoint(1889.203, 85.08511, 84.41315),
            new WoWPoint(1867.766, 70.31022, 73.20315),
            new WoWPoint(1868.375, 47.56387, 63.69995),
            new WoWPoint(1899.638, 50.47875, 49.03188),
            new WoWPoint(1923.146, 49.2985, 44.78643),
            new WoWPoint(1922.795, 68.88879, 38.1831),
            new WoWPoint(1916.361, 82.01344, 31.24682),
            new WoWPoint(1913.865, 109.0081, 26.28172),
            new WoWPoint(1885.752, 116.5619, 23.6226),
            new WoWPoint(1856.457, 123.4885, 21.93341),
            new WoWPoint(1842.731, 178.8193, 13.13167),
            new WoWPoint(1845.789, 153.685, 18.70906),
            new WoWPoint(1871.184, 175.439, 10.82855),
            new WoWPoint(1846.742, 153.9697, 18.64499),
            new WoWPoint(1862.108, 120.4472, 21.35746),
            new WoWPoint(1915.871, 109.2227, 26.26868),

            new WoWPoint(1969.197, 127.8173, 19.04734),
            new WoWPoint(1992.767, 104.8804, 19.61248),
            new WoWPoint(2001.758, 78.6369, 14.26775),
            new WoWPoint(1973.02, 75.89962, 14.19517),
            new WoWPoint(1969.463, 98.07942, 17.43518),
            new WoWPoint(1984.092, 120.2357, 18.91512),
            new WoWPoint(1950.845, 126.7416, 22.12422),
            new WoWPoint(1919.41, 110.0222, 26.36375),

            
            new WoWPoint(1915.871, 109.2227, 26.26868),
            new WoWPoint(1922.651, 77.61671, 32.70341),
            new WoWPoint(1923.254, 65.63617, 38.14819),
            new WoWPoint(1922.807, 49.64631, 44.79056),
            new WoWPoint(1872.178, 46.4214, 62.56678),
            new WoWPoint(1864.854, 61.88112, 69.25108),
            new WoWPoint(1887.615, 84.1413, 84.41303),
        };

        internal static List<WoWPoint> Alliance_Mine_LevelThree = new List<WoWPoint>
        {
            new WoWPoint(1889.203, 85.08511, 84.41315),
            new WoWPoint(1867.766, 70.31022, 73.20315),
            new WoWPoint(1868.375, 47.56387, 63.69995),
            new WoWPoint(1899.638, 50.47875, 49.03188),
            new WoWPoint(1923.146, 49.2985, 44.78643),
            new WoWPoint(1922.795, 68.88879, 38.1831),
            new WoWPoint(1916.361, 82.01344, 31.24682),
            new WoWPoint(1913.865, 109.0081, 26.28172),
            new WoWPoint(1885.752, 116.5619, 23.6226),
            new WoWPoint(1856.457, 123.4885, 21.93341),
            new WoWPoint(1842.731, 178.8193, 13.13167),
            new WoWPoint(1845.789, 153.685, 18.70906),
            new WoWPoint(1871.184, 175.439, 10.82855),
            new WoWPoint(1846.742, 153.9697, 18.64499),
            new WoWPoint(1862.108, 120.4472, 21.35746),

            //
            new WoWPoint(1894.704, 143.9975, 22.97749),
            new WoWPoint(1908.806, 187.163, 19.75881),
            new WoWPoint(1943.453, 225.6138, 20.54592),
            new WoWPoint(1901.836, 173.7192, 21.2011),
            new WoWPoint(1892.738, 118.4613, 23.72561),


            new WoWPoint(1915.871, 109.2227, 26.26868),
            new WoWPoint(1969.197, 127.8173, 19.04734),
            new WoWPoint(1992.767, 104.8804, 19.61248),
            new WoWPoint(2001.758, 78.6369, 14.26775),
            new WoWPoint(1973.02, 75.89962, 14.19517),
            new WoWPoint(1969.463, 98.07942, 17.43518),
            new WoWPoint(1984.092, 120.2357, 18.91512),
            new WoWPoint(1950.845, 126.7416, 22.12422),
            new WoWPoint(1919.41, 110.0222, 26.36375),

            
            new WoWPoint(1915.871, 109.2227, 26.26868),
            new WoWPoint(1922.651, 77.61671, 32.70341),
            new WoWPoint(1923.254, 65.63617, 38.14819),
            new WoWPoint(1922.807, 49.64631, 44.79056),
            new WoWPoint(1872.178, 46.4214, 62.56678),
            new WoWPoint(1864.854, 61.88112, 69.25108),
            new WoWPoint(1887.615, 84.1413, 84.41303),
        };

    }
}
