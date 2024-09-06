using System;
using System.Linq;

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;

namespace SharpMiner.Core
{
    /// <summary>
    /// This class implements the Principal Component Analysis algortithm 
    /// </summary>
    public class CorePCA
    {
        private Svd<double> _svd { get; set; }
        private DataSet _dataset;
        private Matrix<double> _principalComponents;
        private Matrix<double> _singularValues;

        /// <summary>
        /// Initialize and compute PCA using SVD
        /// </summary>
        /// <param name="specs"></param>
        public CorePCA(Specs specs)
        {
            if (specs.IsCenteredAndScaled)
            {
                _dataset = specs.CenteredAndScaledData;
            }
            else
            {
                _dataset = specs.DataSet;
            }

            ComputePrincipalComponentsUsingSVD();
        }
        /// <summary>
        /// Get the normalized singular values computed from the SVD
        /// </summary>
        public Matrix<double> SingularValues => _singularValues;

        /// <summary>
        /// Get the singular value decomposition on which the principal components are computed.
        /// </summary>
        public Svd<double> Svd => _svd;

        /// <summary>
        /// Returns an array containing the cumulative sums of explained variance up to the specified number of components
        /// </summary>
        /// <param name="numberComponents"></param>
        /// <returns></returns>
        public double[] GetExplainedVariance(int numberComponents)
        {
            double[] singularValues = _svd.S.ToArray();

            double[] squaredSingularValues = singularValues.Select(s => s * s).ToArray();
            double totalVariance = squaredSingularValues.Sum();

            double[] explainedVarianceRatio = squaredSingularValues.Select(s => s / totalVariance).ToArray();

            double[] cumulativeSums = new double[numberComponents];

            double cumSum = 0;
            for (int i = 0; i < numberComponents; i++)
            {
                cumSum += explainedVarianceRatio[i];
                cumulativeSums[i] = cumSum;
            }

            return cumulativeSums;
        }
        /// <summary>
        /// Project the original data onto the principal components
        /// </summary>
        /// <param name="numberComponents"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Matrix<double> Project(int numberComponents)
        {
            return _dataset.Data.Multiply(_principalComponents);
        }
        /// <summary>
        /// Returns the principal components as n*n matrix where n is the number of columns
        /// </summary>
        public Matrix<double> PrincipalComponents => _principalComponents;

        #region Private methods
        private void ComputePrincipalComponentsUsingSVD()
        {
            _svd = _dataset.Data.Svd();
            _principalComponents = _svd.VT.Transpose();
        }

        #endregion
    }
}
