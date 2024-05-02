using System.Linq;

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Statistics;

namespace SharpMiner
{
    public class DatasetStatistics
    {
        public double[] Means { get; private set; }
        public double[] StandardDeviations { get; private set; }

        public DatasetStatistics(double[] means, double[] standardDeviations)
        {
            Means = means;
            StandardDeviations = standardDeviations;
        }

        public DatasetStatistics(Matrix<double> data) 
        { 
            ComputeStatistics(data);
        }

        private void ComputeStatistics(Matrix<double> data)
        {
            StandardDeviations =  data.ToColumnArrays()
                .Select(col => col.PopulationStandardDeviation())
                .ToArray();

            Means = data.ToColumnArrays()
                .Select(col => col.Mean())
                .ToArray();
        }
    }
}
