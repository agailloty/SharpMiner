using MathNet.Numerics.LinearAlgebra;

namespace SharpMiner
{
    public class FactorResults
    {
        public FactorResults(Vector<double> explainedVariance, Matrix<double> coordinates, Matrix<double> squaredCosinus, Matrix<double> contributions) 
        { 
            Coordinates = coordinates;
            SquaredCosinus = squaredCosinus;
            Contributions = contributions;
            ExplainedVariance = explainedVariance;
        }

        public Matrix<double> Coordinates { get; }
        public Matrix<double> SquaredCosinus { get; }

        public Matrix<double> Contributions { get; }
        /// <summary>
        /// Get the percentage of explained variance by each components
        /// </summary>

        public Vector<double> ExplainedVariance { get; }
        /// <summary>
        /// Get the cumulative sum of explained variance
        /// </summary>
        public Vector<double> CumulativeExplainedVariance
        {
            get
            {
                if (ExplainedVariance == null)
                    return null;
                return StatsHelper.CumulativeSum(ExplainedVariance);
            }
        }
    }
}
