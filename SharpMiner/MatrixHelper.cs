using System;

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
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

        /// <summary>
        /// The <c>ComputeSignFlip</c> function adjusts the signs of the singular vectors 
        /// (both left and right) from the Singular Value Decomposition (SVD) of a matrix 
        /// in a consistent way. This is necessary because the SVD of a matrix is not 
        /// unique: the signs of the singular vectors can be arbitrarily flipped (positive 
        /// or negative) without changing the mathematical correctness of the decomposition.
        /// This function resolves these ambiguities by ensuring consistent signs across 
        /// both the left and right singular vectors.
        /// </summary>
        /// <param name="svd">
        /// The <see cref="Svd{Double}"/> object that represents the singular value 
        /// decomposition of a matrix. This object contains three components:
        /// <list type="bullet">
        /// <item>
        /// <description>U: Matrix of left singular vectors (orthonormal).</description>
        /// </item>
        /// <item>
        /// <description>V: Matrix of right singular vectors (orthonormal), typically 
        /// transposed as VT in MathNet.</description>
        /// </item>
        /// <item>
        /// <description>S: Diagonal matrix of singular values (non-negative real numbers).</description>
        /// </item>
        /// </list>
        /// </param>
        /// <param name="X">
        /// The original matrix for which the SVD was computed. This matrix is used in the 
        /// sign-flipping algorithm to help determine the appropriate signs of the singular 
        /// vectors.
        /// </param>
        /// <param name="U_prime">
        /// Output matrix of left singular vectors (<c>U_prime</c>) with adjusted signs. 
        /// This matrix is a copy of <c>U</c>, where the signs of its columns are flipped 
        /// as necessary to ensure consistency between the left and right singular vectors.
        /// </param>
        /// <param name="V_prime">
        /// Output matrix of right singular vectors (<c>V_prime</c>) with adjusted signs. 
        /// Similar to <c>U_prime</c>, this matrix is derived from <c>V</c>, but with 
        /// adjusted signs to maintain consistency with the left singular vectors.
        /// </param>
        /// <remarks>
        /// <para>
        /// In the context of the Singular Value Decomposition (SVD), given a matrix <c>X</c>, 
        /// it can be factored into:
        /// <c>X = U * S * V^T</c>, where:
        /// <list type="bullet">
        /// <item><description><c>U</c> is a matrix of left singular vectors (orthonormal columns).</description></item>
        /// <item><description><c>S</c> is a diagonal matrix of singular values (non-negative, real numbers).</description></item>
        /// <item><description><c>V^T</c> is the transpose of the matrix of right singular vectors (orthonormal rows).</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// However, the SVD is not unique because for any singular vector <c>u_k</c> and 
        /// <c>v_k</c> (the k-th left and right singular vectors), flipping the sign of both 
        /// (i.e., replacing <c>u_k</c> with <c>-u_k</c> and <c>v_k</c> with <c>-v_k</c>) 
        /// leaves the product <c>X = U * S * V^T</c> unchanged. This means that without 
        /// further constraints, there can be ambiguity in the signs of the singular vectors.
        /// </para>
        /// <para>
        /// The <c>ComputeSignFlip</c> function eliminates this ambiguity by ensuring that 
        /// the signs of the left (<c>U</c>) and right (<c>V</c>) singular vectors are 
        /// consistent. The function evaluates the sign of each singular vector by 
        /// analyzing its relationship with the original data matrix <c>X</c>. It adjusts 
        /// the signs of the singular vectors to minimize inconsistencies, leading to a 
        /// more interpretable and consistent SVD.
        /// </para>
        /// <para>
        /// This process is useful when using SVD for dimensionality reduction, principal 
        /// component analysis (PCA), and other applications where the interpretability and 
        /// consistency of the singular vectors is important.
        /// </para>
        /// </remarks>
        public static void ComputeSignFlip(Svd<double> svd, Matrix<double> X, out Matrix<double> U_prime, out Matrix<double> V_prime)
        {
            Matrix<double> U = svd.U;  // Left singular vectors
            Matrix<double> V = svd.VT.Transpose();  // Right singular vectors (from VT, need to transpose)
            Vector<double> S = svd.S;  // Singular values

            int K = S.Count;  // Number of singular vectors
            U_prime = U.Clone();
            V_prime = V.Clone();

            // Step 1: Adjust signs for left singular vectors
            for (int k = 0; k < K; k++)
            {
                double Sk_left = 0.0;

                for (int j = 0; j < K; j++)
                {
                    if (j == k) continue;
                    Vector<double> y_j = X.Column(j);
                    Sk_left += Math.Sign(U.Column(k).DotProduct(y_j)) * U.Column(k).DotProduct(y_j);
                }

                // Step 2: Adjust signs for right singular vectors
                double Sk_right = 0.0;
                for (int i = 0; i < K; i++)
                {
                    if (i == k) continue;
                    Vector<double> y_i = X.Row(i);
                    Sk_right += Math.Sign(V.Column(k).DotProduct(y_i)) * V.Column(k).DotProduct(y_i);
                }

                // Step 3: Sign flipping based on Sk_left and Sk_right
                if (Sk_left * Sk_right < 0)
                {
                    if (Sk_left < Sk_right)
                    {
                        Sk_left = -Sk_left;
                    }
                    else
                    {
                        Sk_right = -Sk_right;
                    }
                }

                // Update U_prime and V_prime based on computed Sk_left and Sk_right
                U_prime.SetColumn(k, U.Column(k).Multiply(Math.Sign(Sk_left)));
                V_prime.SetColumn(k, V.Column(k).Multiply(Math.Sign(Sk_right)));
            }
        }

        /// <summary>
        /// Computes the correlation matrix from the given data matrix.
        /// Each column of the data matrix represents a variable, and each row represents an observation.
        /// </summary>
        /// <param name="data">A matrix of size (n x m) where 'n' is the number of observations and 'm' is the number of variables.</param>
        /// <returns>A correlation matrix of size (m x m) where the element at (i, j) represents the correlation coefficient between variables i and j.</returns>
        public static Matrix<double> ComputeCorrelationMatrix(Matrix<double> data)
        {
            int n = data.RowCount;  // Number of observations
            int m = data.ColumnCount;  // Number of variables

            // Step 1: Compute the mean of each column (variable)
            var means = Vector<double>.Build.Dense(m);
            for (int j = 0; j < m; j++)
            {
                means[j] = data.Column(j).Mean();
            }

            // Step 2: Subtract the mean from each column (center the data)
            var centeredData = data.Clone();
            for (int j = 0; j < m; j++)
            {
                centeredData.SetColumn(j, data.Column(j) - means[j]);
            }

            // Step 3: Compute the covariance matrix
            var covarianceMatrix = centeredData.TransposeThisAndMultiply(centeredData).Divide(n - 1);

            // Step 4: Compute the standard deviations (sqrt of diagonal elements of covariance matrix)
            var stdDevs = covarianceMatrix.Diagonal().Map(Math.Sqrt);

            // Step 5: Normalize the covariance matrix to get the correlation matrix
            var correlationMatrix = covarianceMatrix.MapIndexed((i, j, cov) => cov / (stdDevs[i] * stdDevs[j]));

            return correlationMatrix;
        }
        /// <summary>
        /// Applies row and column weights to the input matrix by adjusting each element based on the 
        /// square roots of the corresponding row and column weights. This transformation is often used in 
        /// dimensionality reduction techniques such as Principal Component Analysis (PCA) to balance the 
        /// influence of each row and column while preserving the overall structure of the data.
        /// </summary>
        /// <param name="X">The input matrix where rows represent observations and columns represent features.</param>
        /// <param name="rowWeights">
        /// A vector of weights corresponding to each row in the matrix. If not provided, uniform weights
        /// are assumed, meaning each row has an equal contribution. The weights are normalized to sum to 1.
        /// </param>
        /// <param name="colWeights">
        /// A vector of weights corresponding to each column in the matrix. If not provided, each column
        /// is assumed to have an equal weight of 1, meaning each feature has an equal contribution.
        /// </param>
        /// <returns>
        /// A new matrix where each element is scaled by the square root of its respective row and column weights.
        /// </returns>
        /// <remarks>
        /// The square root of the weights is used to ensure that the overall weighting remains balanced. 
        /// Multiplying by the square root of both row and column weights means that the scaling for each 
        /// matrix element is proportional to the contribution of both its row and column without overly 
        /// inflating or diminishing the influence of any single weight. This allows the matrix to be transformed 
        /// in such a way that maintains relative distances between data points, which is crucial for algorithms 
        /// like PCA, where geometric relationships (such as variance) are important.
        /// </remarks>

        public static Matrix<double> GetWeighedMatrix(Matrix<double> X, Vector<double> rowWeights = null, Vector<double> colWeights = null)
        {
            int nrows = X.RowCount;
            int ncols = X.ColumnCount;

            // Handle default row weights
            if (rowWeights == null)
            {
                rowWeights = Vector<double>.Build.Dense(nrows, 1.0 / nrows);
            }

            // Handle default column weights
            if (colWeights == null)
            {
                colWeights = Vector<double>.Build.Dense(ncols, 1.0);
            }

            // Normalize the row and column weights
            rowWeights = rowWeights.Divide(rowWeights.Sum());

            // Apply row and column weights to X
            Matrix<double> weightedX = X.MapIndexed((i, j, value) => value * Math.Sqrt(colWeights[j]) * Math.Sqrt(rowWeights[i]));

            return weightedX;
        }

    }
}
