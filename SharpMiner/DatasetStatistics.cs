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
        internal double[] SquaredRowWeights { get; private set; }
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

            if (cweights is null)
            {
                cweights = Generate.Repeat(data.ColumnCount, 1.0);
            }

            ScaledAndReducedData = new MatrixTransformer(data).ScaledAndReduced;

            RowWeights = rweights;
            SquaredRowWeights = rweights.ToVector().PointwisePower(0.5).ToArray();
            ColumnWeights = cweights;
            SquaredColumnWeights = cweights.ToVector().PointwisePower(0.5).ToArray();

            ComputeStatistics(data);

        }

        private void ComputeStatistics(Matrix<double> data)
        {
            ColumnStandardDeviations =  data.ToColumnArrays()
                .Select(col => col.PopulationStandardDeviation())
                .ToArray();

            ColumnMeans = data.ToColumnArrays()
                .Select(col => col.Mean())
                .ToArray();

            RowMeans = data.ToRowArrays()
                .Select(col => col.Mean())
                .ToArray();

            RowsEuclidianDistance = ScaledAndReducedData
                .PointwisePower(2.0)
                .MultiplyByColumnVector(ColumnWeights.ToVector())
                .ToRowArrays()
                .Select(r => r.Sum())
                .ToArray();

            RowInertia = RowsEuclidianDistance
                .Select((x, i) => x * RowWeights[i])
                .ToArray();
        }
    }
}
