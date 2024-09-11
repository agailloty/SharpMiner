﻿using System;
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
        private Specs _spec;

        /// <summary>
        /// Initialize and compute PCA using SVD
        /// </summary>
        /// <param name="specs"></param>
        public CorePCA(Specs specs)
        {
            _spec = specs;
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
        /// Projects the dataset onto a lower-dimensional subspace using the specified number of components.
        /// </summary>
        /// <param name="numberComponents">
        /// The number of principal components to use for the projection. If the value exceeds the available number 
        /// of components, it will be automatically reduced to the maximum available.
        /// </param>
        /// <returns>
        /// A matrix representing the projected data in the reduced dimensionality space.
        /// </returns>
        /// <remarks>
        /// This method multiplies the dataset matrix by a sub-matrix constructed from the first <paramref name="numberComponents"/>
        /// principal components to perform the projection. It ensures the number of components does not exceed the total number available.
        /// </remarks>

        public Matrix<double> Project(int numberComponents)
        {
            if (numberComponents > _spec.NumberOfComponents)
                numberComponents = (int)_spec.NumberOfComponents;

            int rowNumbers = (int)_spec.DataSet.RowCount;
            var firstColumns = Enumerable.Range(0, numberComponents);
            var subMatrix = Matrix<double>.Build
                .DenseOfColumnVectors(firstColumns.Select(i => _principalComponents.Column(i))
                .ToArray());

            return _dataset.Data.Multiply(subMatrix);
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
