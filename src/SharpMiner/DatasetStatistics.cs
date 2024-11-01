using System;
using System.Linq;

using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Statistics;

namespace SharpMiner
{
    /// <summary>
    /// Represents statistics computed for a dataset, including various metrics and properties.
    /// </summary>
    public class DatasetStatistics
    {
        /// <summary>
        /// Gets the mean value for each row in the dataset.
        /// </summary>
        public double[] RowMeans { get; private set; }
        /// <summary>
        /// Gets the inertia for each row in the dataset.
        /// </summary>
        public double[] RowInertia { get; private set; }
        /// <summary>
        /// Gets the weights assigned to each row in the dataset.
        /// </summary>
        public double[] RowWeights { get; private set; }
        /// <summary>
        /// Gets the Euclidean distance for each row in the dataset.
        /// </summary>
        public double[] RowsEuclidianDistance { get; private set; }
        /// <summary>
        /// Gets the squared weights assigned to each row in the dataset.
        /// </summary>
        internal double[] RootSquaredRowWeights { get; private set; }
        /// <summary>
        /// Gets the mean value for each column in the dataset.
        /// </summary>
        public double[] ColumnMeans { get; private set; }
        /// <summary>
        /// Gets the standard deviation for each column in the dataset.
        /// </summary>
        public double[] ColumnStandardDeviations { get; private set; }
        /// <summary>
        /// Gets the weights assigned to each column in the dataset.
        /// By default, each column has a weight equal to 1.
        /// </summary>
        public double[] ColumnWeights { get; private set; }
        internal double[] SquaredColumnWeights { get; private set; }
        /// <summary>
        /// Gets the scaled and reduced data matrix.
        /// </summary>
        public Matrix<double> ScaledAndReducedData { get; }

        public Matrix<double> CorrelationMatrix { get; }

        public Matrix<double> WeighedDataset { get; }

        protected Matrix<double> DemeanedData { get; }


        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetStatistics"/> class with the specified data, row weights, and column weights.
        /// </summary>
        /// <param name="data">The matrix of data to compute statistics on.</param>
        /// <param name="rweights">Optional. The weights assigned to each row in the dataset. If null, row weights will be set to 1 divided by the number of rows.</param>
        /// <param name="cweights">Optional. The weights assigned to each column in the dataset. If null, column weights will be set to 1.</param>
        public DatasetStatistics(Matrix<double> data, double[] rweights = null, double[] cweights = null) 
        {
            if (rweights != null && rweights.Length != data.RowCount)
                throw new ArgumentException("Row weights should be of same length as row count");

            if (cweights != null && cweights.Length != data.ColumnCount)
                throw new ArgumentException("Column weights should be of same length as row count");

            if (rweights is null)
            {
                double rweight = 1.0 / data.RowCount;
                rweights = Generate.Repeat(data.RowCount, rweight);
            }

            RowWeights = rweights;

            if (cweights is null)
            {
                cweights = Generate.Repeat(data.ColumnCount, 1.0);
            }

            ColumnWeights = cweights;

            (ColumnMeans, ColumnStandardDeviations) = ComputeStatistics(data);

            CorrelationMatrix = Correlation.PearsonMatrix(data.ToColumnArrays());

            ScaledAndReducedData = ComputeReducedAndScaledDataset(data, rweights);

            RowsEuclidianDistance = ScaledAndReducedData
            .PointwisePower(2.0)
            .MultiplyByColumnVector(ColumnWeights.ToVector())
            .ToRowArrays()
            .Select(r => r.Sum())
            .ToArray();

            RowInertia = RowsEuclidianDistance
                        .Select((x, i) => x * RowWeights[i])
                        .ToArray();

            RootSquaredRowWeights = rweights.ToVector().PointwisePower(0.5).ToArray();

            WeighedDataset = ScaledAndReducedData.MultiplyByRowVector(RootSquaredRowWeights.ToVector());
            
            SquaredColumnWeights = cweights.ToVector().PointwisePower(0.5).ToArray();

        }

        private static (double[] colStd, double[] colMeans) ComputeStatistics(Matrix<double> data)
        {
            double[] colStd =  data.ToColumnArrays()
                .Select(col => col.PopulationStandardDeviation())
                .ToArray();

            double[] colMeans = data.ToColumnArrays()
                .Select(col => col.Mean())
                .ToArray();

            return (colStd, colMeans);
        }

        private static Matrix<double> ComputeReducedAndScaledDataset(Matrix<double> data, double[] rweights)
        {

            // Multiply each row in the dataset by its weights

            Matrix<double> rowWeights = Matrix<double>.Build.DenseOfRowArrays(rweights)
                .Divide(rweights.Sum());

            Matrix<double> centers = rowWeights
                .Multiply(data);

            Matrix<double> deviations = rowWeights
                .Multiply(data.PointwisePower(2.0))
                .PointwisePower(0.5);

            // Center and reduce the dataset : for each column, substract the value by the mean
            // Divide each column by its deviation

            Matrix<double> result = data
                .AddColumnVector(centers.Row(0), AddOperation.Subtraction)
                .DivideByVector(deviations.Row(0));

            return result;
        }
    }
}
