using System.Linq;

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Statistics;

namespace SharpMiner
{
    public class MatrixTransformer
    {
        public double[] ColumnMeans { get; }
        public double[] ColumnStandardDeviations { get; }
        private readonly Matrix<double> _matrix;
        public MatrixTransformer(Matrix<double> matrix)
        {
            _matrix = matrix;
            ColumnMeans = ComputeMeans(matrix).AsArray();
            ScaledAndReduced = ScaleAndReduce(matrix);
        }

        public Matrix<double> ScaledAndReduced { get; }

        private static Matrix<double> ScaleAndReduce(Matrix<double> matrix)
        {
            var reduced = matrix.ToColumnArrays()
                .AsParallel()
                .Select(col => ScaleAndReduce(col))
                .SelectMany(s => s).ToArray();

            var reducedMatrix = DenseMatrix.Build.Dense(matrix.RowCount, matrix.ColumnCount, reduced);
            return reducedMatrix;
        }

        public static double[] ScaleAndReduce(double[] array)
        {
            var mean = array.Mean();
            var std = array.PopulationStandardDeviation();
            return array.Select(x => x - mean).Select(x => x / std).ToArray();
        }

        private static Vector<double> ComputeMeans(Matrix<double> matrix)
        {
            return matrix.ColumnMeans();
        }
    }
}

