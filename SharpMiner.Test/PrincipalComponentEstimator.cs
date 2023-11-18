﻿using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics.Statistics;

namespace SharpMiner.Test
{
    internal class PrincipalComponentEstimator
    {
        private Matrix<double> _data;
        private Vector<double> _weights;
        private readonly int _nrows;
        private readonly int _ncols;
        private double _mu, _sigma, _rsquare;
        private Vector<double> _loadings, _eigenvalues;
        private Svd<double> _svd;
        private Matrix<double> _projections, _eigenvectors, _scores, _coefficients;

        public PrincipalComponentEstimator(DenseMatrix data, IEnumerable<double>? weights = null)
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(data));
            _data = data;
            _nrows = _data.RowCount;
            _ncols = _data.ColumnCount;
            if (weights == null)
            {
                _weights = DenseVector.Create(_nrows, 1.0);
            } 
            else
            {
                if (weights.Count() != _nrows)
                {
                    throw new ArgumentException($"Weights should have {_data.RowCount} elements.");
                }

                _weights = new DenseVector(weights.ToArray());
                double mean = ArrayStatistics.Mean(_weights.PointwisePower(2).ToArray());
                _weights.MapInplace(x => x / mean);
            }

            _svd = data.Svd();

            _eigenvalues = _svd.S.PointwisePower(2.0);
            _eigenvectors = _svd.VT;
        }

        public Matrix<double> Project(int? ncomponents = null)
        {
            if (ncomponents <= 0)
                throw new ArgumentException("ncomponents must be greater than 0.");

            if (ncomponents > _ncols)
                throw new ArgumentException("ncomp must be smaller than the number of components computed.");

            ncomponents ??= _ncols;

            _projections = _data.Multiply(_eigenvectors);
            return _projections;
        }

        private void ComputePCA(int ncomponents)
        {
            _scores = _data.Multiply(_eigenvectors);
            _coefficients = _eigenvectors.Transpose();

            _coefficients.SubMatrix(0, _nrows, 0, ncomponents);
        }

        /// <summary>
        /// Reorder matrix based on the ordering computed eignenvalues
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="ncomponents"></param>
        /// <returns>A subset of the matrix where columns are reordered following the order by which the eigenvalues are sorted</returns>
        private Matrix<double> ReoderMatrix(Matrix<double> matrix, int ncomponents)
        {
            // Set matrix columns order based on ordered eigenvalues 
            int[] indices = _eigenvalues.EnumerateIndexed()
                .OrderByDescending(x => x.Item2).Select(p => p.Item1)
                .Take(ncomponents)
                .ToArray();

            // If initial matrix is (n * p) and ncomp = 5 then we return a matrix (n * 5) where
            // each column in ordered by the values of the eigenvalues
            Matrix<double> sortedMatrix = DenseMatrix.Create(matrix.RowCount, ncomponents, 
                (i, j) => matrix[i, indices[j]]);

            return sortedMatrix;
        }
    }
}