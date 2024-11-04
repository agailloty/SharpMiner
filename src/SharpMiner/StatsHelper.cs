using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace SharpMiner
{
    internal static class StatsHelper
    {
        public static double[] CumulativeSum(double[] data)
        {
            var vec = DenseVector.Build.DenseOfArray(data);
            return CumulativeSum(vec).AsArray();
        }

        public static Vector<double> CumulativeSum(Vector<double> vector)
        {
            // Initialize the cumulative sum vector
            Vector<double> cumulativeSum = Vector<double>.Build.Dense(vector.Count);

            // Compute the cumulative sum
            double sum = 0;
            for (int i = 0; i < vector.Count; i++)
            {
                sum += vector[i];
                cumulativeSum[i] = sum;
            }

            return cumulativeSum;
        }
    }
}
