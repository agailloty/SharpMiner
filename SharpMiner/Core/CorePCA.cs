using System;
using System.Linq;

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace SharpMiner.Core
{
    /// <summary>
    /// This class implements the Principal Component Analysis algortithm 
    /// </summary>
    public class CorePCA
    {
        private DataSet _dataset;
        private Vector<double> _eigenValues;
        private Matrix<double> _eigenVectors;
        private Specs _spec;
        private Matrix<double> _projectedData;
        private DatasetStatistics _statistics;
        private FactorResults _rowResults;
        private FactorResults _columnResults;
        private double[] _explainedVariance;

        /// <summary>
        /// Initialize and compute PCA using SVD
        /// </summary>
        /// <param name="specs"></param>
        public CorePCA(Specs specs)
        {
            _spec = specs;
            _dataset = specs.IsCenteredAndScaled ? specs.CenteredAndScaledData : specs.DataSet;
            if (specs.DecompositionMethod == DecompositionMethod.Svd) 
            { 
                ComputePrincipalComponentsUsingSvd();
            } 
            
        }
        /// <summary>
        /// Get the normalized singular values computed from the SVD
        /// </summary>

        /// <summary>
        /// Returns the principal components as n*n matrix where n is the number of columns
        /// </summary>

        /// <summary>
        /// Compute the principal components
        /// </summary>
        /// <param name="numberComponents"></param>
        /// <returns></returns>
        public Component[] GetPrincipalComponents(int numberComponents)
        {
            return Enumerable.Range(1, numberComponents)
                .Select(c => ComputeComponent(c)).ToArray();
        }
        /// <summary>
        /// Get the percentage of inertia explained by each component
        /// </summary>
        /// <returns></returns>
        public double[] ExplainedVariance
            => _explainedVariance.Take(_spec.NumberOfComponents).ToArray();
        /// <summary>
        /// Get the cumulative sum of explained variance
        /// </summary>
        public double[] CumulativeExplainedVariance
        {
            get
            {
                if (_explainedVariance == null)
                    return null;
                return StatsHelper.CumulativeSum(ExplainedVariance);
            }
        }

        private Component ComputeComponent(int rank)
        {
            var index = rank - 1;
            int N = _spec.RowsWeights.Length;
            var explainedVariance = DenseVector.Build.DenseOfArray( new double[] { _rowResults.ExplainedVariance[index] });
            var rowCoordinates = _rowResults.Coordinates.SubMatrix(0, N, index, 1);
            var rowSquaredCos = _rowResults.SquaredCosinus.SubMatrix(0, N, index, 1);
            var rowContributions = _rowResults.Contributions.SubMatrix(0, N, index, 1);

            var rowResults = new FactorResults(explainedVariance, rowCoordinates, rowSquaredCos, rowContributions);

            var component = new Component(rank, explainedVariance.First(), rowResults, rowResults);

            return component;
        }

        #region Private methods
        private void ComputePrincipalComponentsUsingSvd()
        {
            var squaredRowWeights = DenseVector.Build.DenseOfArray(_spec.RowsWeights).PointwiseSqrt();
            var weighedScaled = _spec.CenteredAndScaledData.Data.MultiplyByRowVector(squaredRowWeights);

            var svd = weighedScaled.Svd(true);

            // Take a submatrix from U such that U matches the dimensions of original data
            Matrix<double> U = svd.U.SubMatrix(0, _spec.RowsWeights.Length, 0, _spec.ColumnsWeights.Length);
            
            Matrix<double> V = svd.VT.Transpose();
            Vector<double> S = svd.S;

            U = U.MapIndexed((i, j, value) => value / Math.Sqrt(_spec.RowsWeights[i]));

            V = V.MapIndexed((i, j, value) => value / Math.Sqrt(_spec.ColumnsWeights[j]));

            var scores = U.MultiplyByColumnVector(S);
            var loadings = V.Transpose().MultiplyByColumnVector(S);

            var rowWeightVector = DenseVector.Build.DenseOfArray(_spec.RowsWeights);

            Matrix<double> columnDist = weighedScaled
                .MultiplyByRowVector(rowWeightVector);

            var columnWeightVector = DenseVector.Build.DenseOfArray(_spec.ColumnsWeights);
            var squaredS = S.PointwisePower(2);

            var rowDistance = _spec.CenteredAndScaledData.Data
                .PointwisePower(2)
                .MultiplyByColumnVector(columnWeightVector)
                .RowSums();

            var rowCos2 = scores
                .PointwisePower(2)
                .DivideByRowVector(rowDistance);

            var rowContribs = scores
                .PointwisePower(2)
                .MultiplyByRowVector(rowWeightVector.Divide(rowWeightVector.Sum()))
                .DivideByVector(squaredS);



            var columnContribs = loadings
                .PointwisePower(2)
                .DivideByVector(squaredS)
                .MultiplyByColumnVector(columnWeightVector)
                .Transpose();

            var explainedVariance = squaredS.Divide(squaredS.Sum()) * 100;
            _explainedVariance = explainedVariance.ToArray();

            _rowResults = new FactorResults(explainedVariance, scores, rowCos2, rowContribs);
            _columnResults = new FactorResults(explainedVariance, loadings, null, null);

        }
        #endregion
    }
}
