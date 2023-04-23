using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace SharpMiner.Test
{

    public static class MatrixExtensions
    {
        public static Vector<double> ColumnMeans(this Matrix<double> matrix)
        {
            var means = matrix.ColumnSums() / matrix.RowCount;
            return means;
        }

        public static double[][] ToArray2D(this List<DenseMatrix> matrixList)
        {
            int numRows = matrixList[0].RowCount;
            int numCols = matrixList[0].ColumnCount;

            double[][] values = new double[numRows][];

            for (int i = 0; i < numRows; i++)
            {
                values[i] = new double[numCols * matrixList.Count];

                for (int j = 0; j < matrixList.Count; j++)
                {
                    double[] rowValues = matrixList[j].Storage.AsColumnMajorArray().ToArray();
                    Array.Copy(rowValues, j * numCols, values[i], i * numCols, numCols);
                }
            }

            return values;
        }

        public static double[][] GetPrincipalComponents(this DenseMatrix matrix, int numComponents)
        {
            var evd = matrix.Evd();
            var eigenvectors = evd.EigenVectors.ToColumnArrays()
                .OrderByDescending(x => x[0])
                .Take(numComponents)
                .ToList();

            double[][] result = new double[matrix.RowCount][];
            for (int i = 0; i < matrix.RowCount; i++)
            {
                result[i] = new double[numComponents];
                for (int j = 0; j < numComponents; j++)
                {
                    result[i][j] = eigenvectors[j][i];
                }
            }

            return result;
        }

        public static T[,] Transpose<T>(this T[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);

            T[,] result = new T[columns, rows];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    result[j, i] = matrix[i, j];
                }
            }

            return result;
        }

        public static double[][] Transpose(this double[][] matrix)
        {
            int rows = matrix.Length;
            int columns = matrix[0].Length;

            double[][] result = new double[columns][];

            for (int j = 0; j < columns; j++)
            {
                result[j] = new double[rows];

                for (int i = 0; i < rows; i++)
                {
                    result[j][i] = matrix[i][j];
                }
            }

            return result;
        }
    }

}
