using System;
using System.Linq;

using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Statistics;

namespace SharpMiner
{
    public class DatasetStatistics
    {
        public double[] RowMeans { get; private set; }
        public double[] ColumnMeans { get; private set; }
        public double[] ColumnStandardDeviations { get; private set; }

        public double[] RowsEuclidianDistance { get; private set; }

        public double[] RowInertia { get; private set; }

        public double[] RowWeights {  get; private set; }

        public double[] ColumnWeights { get; private set; }

        public Matrix<double> ScaledAndReducedData { get; }


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
            ColumnWeights = cweights;

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
