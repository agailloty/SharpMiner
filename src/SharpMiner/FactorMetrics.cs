using System;
using System.Linq;

namespace SharpMiner
{
    public static class FactorMetrics
    {
        public static double SquaredCosinus(double coord, double[] coordArray)
        {
            return Math.Pow(coord, 2) / coordArray.Sum();
        }

        public static double ContributionFromSquaredCosinus(double cos2, double[] cos2Array) 
        { 
            return (Math.Pow(cos2, 2) / cos2Array.Sum()) * 100;
        }

        public static double ContributionFromCoordinates(double cos2, double[] cos2Array)
        {
            throw new NotImplementedException();
        }
    }
}
