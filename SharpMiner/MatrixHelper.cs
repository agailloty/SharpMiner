using System;
using System.Collections.Generic;
using System.Text;

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Statistics;

namespace SharpMiner
{
    /// <summary>
    /// Helper class that contains various static methods
    /// </summary>
    public static class MatrixHelper
    {
        /// <summary>
        /// Centers and scales the columns of the input matrix by subtracting the mean and dividing by the 
        /// standard deviation of each column.
        /// </summary>
        /// <param name="data">The matrix containing the data to be centered and scaled, where each column represents a variable.</param>
        /// <returns>
        /// A new matrix where each column has been centered (mean subtracted) and scaled (divided by standard deviation).
        /// If a column's standard deviation is zero, the data is only centered, without scaling.
        /// </returns>
        /// <remarks>
        /// This method normalizes the input matrix by making each column have a mean of 0 and a standard deviation of 1, 
        /// which is often useful for machine learning algorithms or statistical analysis.
        /// </remarks>

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
