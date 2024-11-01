using System;
using System.Linq;

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Factorization;

namespace SharpMiner.Core
{
    /// <summary>
    /// This class implements the Principal Component Analysis algortithm 
    /// </summary>
    public class PrincipalComponentAnalysis
    {
        private readonly Specs _spec;
        private FactorResults _rowResults;
        private FactorResults _columnResults;
        private double[] _explainedVariance;
        private Svd<double> _svd;

        /// <summary>
        /// Initialize and compute PCA using SVD
        /// </summary>
        /// <param name="specs"></param>
        public PrincipalComponentAnalysis(Specs specs)
        {
            _spec = specs;
            ComputePrincipalComponentsUsingSvd();
            
        }
        /// <summary>
        /// Get the list of computed components organised by rows and columns results
        /// </summary>
        public Component[] Components 
        {
            get
            {
                return Enumerable.Range(1, _spec.NumberOfComponents)
                        .Select(c => ComputeComponent(c)).ToArray();
            }
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
        /// <summary>
        /// Get the result of the computed singular value decomposition
        /// </summary>
        public Svd<double> Svd => _svd;

        /// <summary>
        /// Get the individuals results 
        /// </summary>
        public FactorResults RowsResults => _rowResults;
        /// <summary>
        /// Get the features results
        /// </summary>
        public FactorResults ColumnsResults => _columnResults;


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
            Vector<double> squaredRowWeights = DenseVector.Build.DenseOfArray(_spec.RowsWeights).PointwiseSqrt();
            Matrix<double> weighedScaled = _spec.CenteredAndScaledData.MultiplyByRowVector(squaredRowWeights);

            _svd = weighedScaled.Svd(true);

            // Take a submatrix from U such that U matches the dimensions of original data
            Matrix<double> U = _svd.U.SubMatrix(0, _spec.RowsWeights.Length, 0, _spec.NumberOfComponents);
            
            Matrix<double> V = _svd.VT
                .SubMatrix(0, _spec.ColumnsWeights.Length, 0, _spec.NumberOfComponents)
                .Transpose();
            Vector<double> S = _svd.S.SubVector(0, _spec.NumberOfComponents);

            U = U.MapIndexed((i, j, value) => value / Math.Sqrt(_spec.RowsWeights[i]));

            V = V.MapIndexed((i, j, value) => value / Math.Sqrt(_spec.ColumnsWeights[j]));

            Matrix<double> scores = U.MultiplyByColumnVector(S);
            Matrix<double> loadings = V.Transpose().MultiplyByColumnVector(S);

            Vector<double> rowWeightVector = DenseVector.Build.DenseOfArray(_spec.RowsWeights);

            Vector<double> columnWeightVector = DenseVector.Build.DenseOfEnumerable(_spec.ColumnsWeights.Take(_spec.NumberOfComponents));
            Vector<double> squaredS = S.PointwisePower(2);

            Matrix<double> X = _spec.CenteredAndScaledData.SubMatrix(0, _spec.RowsWeights.Length, 0, _spec.NumberOfComponents);

            Vector<double> rowDistance = X
                .PointwisePower(2)
                .MultiplyByColumnVector(columnWeightVector)
                .RowSums();

            Matrix<double> rowCos2 = scores
                .PointwisePower(2)
                .DivideByRowVector(rowDistance);

            Matrix<double> rowContribs = scores
                .PointwisePower(2)
                .MultiplyByRowVector(rowWeightVector.Divide(rowWeightVector.Sum()))
                .DivideByVector(squaredS.Multiply(100.0));

            // Columns

            Matrix<double> squaredWeighedData = _spec.CenteredAndScaledData.PointwisePower(2);

            Vector<double> columnDistance = squaredWeighedData
                .MultiplyRows(rowWeightVector)
                .ColumnSums().SubVector(0, _spec.NumberOfComponents);

            Vector<double> explainedVariance = squaredS.Divide(_svd.S.PointwisePower(2).Sum()) * 100;
            _explainedVariance = explainedVariance.ToArray();

            Matrix<double> columnCorrelation = loadings.DivideByVector(columnDistance);

            Matrix<double> columnCos2 = columnCorrelation.PointwisePower(2);

            Matrix<double> columnContribs = loadings
            .PointwisePower(2)
            .DivideByVector(squaredS.Multiply(100.0))
            .MultiplyByColumnVector(columnWeightVector)
            .Transpose();

            _rowResults = new FactorResults(explainedVariance, scores, rowCos2, rowContribs);
            _columnResults = new FactorResults(explainedVariance, loadings, columnCos2, columnContribs);

        }
        #endregion
    }
}
