using System;
using System.Linq;

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace SharpMiner
{

    public static class MatrixExtensions
    {
        public static Vector<double> ColumnMeans(this Matrix<double> matrix)
        {
            var means = matrix.ColumnSums() / matrix.RowCount;
            return means;
        }


        public static Vector<double> ToVector(this double[] array)
        {
            return new DenseVector(array);
        }


        public static Vector<double> MapToVector(this Matrix<double> matrix, Func<double[], double> func)
        {
            var res = matrix.ToColumnArrays().Select(vec => func(vec)).AsParallel();
            return res.ToArray().ToVector();
        }

        public static Matrix<double> DivideByVector(this Matrix<double> matrix, Vector<double> divisor)
        {
            // Make sure matrix columns count is equal to vector length
            if (matrix.ColumnCount != divisor.Count)
                throw new ArgumentException("Matrix column count should be equal to the divisor count");

            // Create a matrix of same size as the current matrix based on the vector

            Matrix<double> divisorMatrix = Matrix<double>.Build.Dense(matrix.RowCount, matrix.ColumnCount, (i, j) => divisor[j]);

            var res = matrix.PointwiseDivide(divisorMatrix);
            return res;
        }

        public static Matrix<double> MultiplyByColumnVector(this Matrix<double> matrix, Vector<double> vector)
        {
            // Make sure matrix columns count is equal to vector length
            if (matrix.ColumnCount != vector.Count)
                throw new ArgumentException("Matrix columns count should be equal to the length of multiplier vector");

            // Create a matrix of same size as the current matrix based on the vector

            Matrix<double> divisorMatrix = Matrix<double>.Build.Dense(matrix.RowCount, matrix.ColumnCount, (i, j) => vector[j]);

            var res = matrix.PointwiseMultiply(divisorMatrix);
            return res;

        }

        public static Matrix<double> MultiplyByRowVector(this Matrix<double> matrix, Vector<double> vector)
        {
            // Make sure matrix columns count is equal to vector length
            if (matrix.RowCount != vector.Count)
                throw new ArgumentException("Matrix rows count should be equal to the length of multiplier vector");

            // Create a matrix of same size as the current matrix based on the vector

            Matrix<double> divisorMatrix = Matrix<double>.Build.Dense(matrix.RowCount, matrix.ColumnCount, (i, j) => vector[j]);

            var res = matrix.PointwiseMultiply(divisorMatrix);
            return res;

        }
    }
}
