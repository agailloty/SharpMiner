using System;
using System.Collections.Generic;
using System.Text;

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Statistics;

namespace SharpMiner
{
    public static class MatrixHelper
    {
        public static Matrix<double> CenterAndScale(Matrix<double> data)
        {
            int rows = data.RowCount;
            int cols = data.ColumnCount;

            // Create vectors to store the means and standard deviations of each column
            var means = Vector<double>.Build.Dense(cols);
            var stdDevs = Vector<double>.Build.Dense(cols);

            // Calculate means and standard deviations for each column
            for (int j = 0; j < cols; j++)
            {
                var column = data.Column(j);
                means[j] = column.Mean();
                stdDevs[j] = column.StandardDeviation();
            }

            // Create a new matrix to store the centered and scaled data
            var centeredAndScaledData = Matrix<double>.Build.Dense(rows, cols);

            // Centering and scaling
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (stdDevs[j] != 0) // Avoid division by zero
                    {
                        centeredAndScaledData[i, j] = (data[i, j] - means[j]) / stdDevs[j];
                    }
                    else
                    {
                        centeredAndScaledData[i, j] = data[i, j] - means[j]; // Only center if stdDev is zero
                    }
                }
            }

            return centeredAndScaledData;
        }
    }
}
