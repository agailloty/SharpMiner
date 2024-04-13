using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Statistics;

namespace SharpMiner.Test
{
    internal class PrincipalComponentEstimator
    {
        private readonly Matrix<double> _data;
        private readonly Vector<double> _weights;
        private readonly int _defaultComponents;

        public Vector<double>? EigenValues { get; }
        public Matrix<double>? EigenVectors { get; }
        public Matrix<double>? Scores { get; private set; }
        public Matrix<double>? Projections { get; private set; }

        public Matrix<double>? ScaledAndReducedData { get; }

        /// <summary>
        /// The steps to compute a PCA are the following : compute eignenvalues, compute principal components from the eigenvalues
        /// and project
        /// </summary>
        /// <param name="data"></param>
        /// <param name="weights"></param>
        /// <param name="ncomponents"></param>
        /// <exception cref="ArgumentException"></exception>
        public PrincipalComponentEstimator(Matrix<double> data, IEnumerable<double>? weights = null, int? ncomponents = null)
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(data));
            _data = data;
            if (weights == null)
            {
                _weights = DenseVector.Create(data.RowCount, 1.0);
            } 
            else
            {
                if (weights.Count() != data.RowCount)
                {
                    throw new ArgumentException($"Weights should have {_data.RowCount} elements.");
                }

                _weights = new DenseVector(weights.ToArray());
                double mean = ArrayStatistics.Mean(_weights.PointwisePower(2).ToArray());
                _weights.MapInplace(x => x / mean);
            }

            ScaledAndReducedData = new MatrixTransformer(data).ScaledAndReduced;

            (EigenVectors, EigenValues) =  ComputeEigen(ScaledAndReducedData);

            ncomponents ??= data.ColumnCount;
            _defaultComponents = ncomponents.Value;

            ComputePCAFromEigen();
        }

        public Matrix<double> Project(int? ncomponents = null)
        {
            if (ncomponents <= 0)
                throw new ArgumentException("ncomponents must be greater than 0.");

            if (ncomponents > _data.ColumnCount)
                throw new ArgumentException("ncomp must be smaller than the number of components computed.");

            Projections = ScaledAndReducedData.Multiply(EigenVectors);
            
            return Projections;
        }

        private void ComputePCA(int ncomponents)
        {
            if (EigenValues != null && EigenVectors != null)
            {
                var orderedComponents = EigenValues.EnumerateIndexed()
                .OrderByDescending(x => x.Item2).Select(p => p.Item1).ToArray();

                Matrix<double> eigenvectors = ReorderMatrix(EigenVectors, orderedComponents, ncomponents);
                Scores = ScaledAndReducedData!.Multiply(eigenvectors);
            }

        }

        /// <summary>
        /// Reorder matrix based on the ordering computed eignenvalues
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="ncomponents"></param>
        /// <returns>A subset of the matrix where columns are reordered following the order by which the eigenvalues are sorted</returns>
        private Matrix<double> ReorderMatrix(Matrix<double> matrix, int[] indices, int ncomponents)
        {
            // Set matrix columns order based on ordered eigenvalues 

            indices = indices.Take(ncomponents).ToArray();
            // If initial matrix is (n * p) and ncomp = 5 then we return a matrix (n * 5) where
            // each column in ordered by the values of the eigenvalues
            Matrix<double> sortedMatrix = DenseMatrix.Create(matrix.RowCount, ncomponents, 
                (i, j) => matrix[i, indices[j]]);

            return sortedMatrix;
        }

        /// <summary>
        /// Computes both Eigenvalues & Eigenvectors
        /// </summary>
        /// <returns></returns>
        private static (Matrix<double>? eigenvectors, Vector<double>? eigenvalues) ComputeEigen(Matrix<double> data)
        {
            Matrix<double> eigenvectors = null;
            Vector<double> eigenvalues = null;
            var svd = data.Svd();
            if (svd != null)
            {
                eigenvalues = svd.S.PointwisePower(2.0);
                eigenvectors = svd.VT;
            }

            return (eigenvectors, eigenvalues);
        }

        private void ComputePCAFromEigen(bool normalize = true)
        {
            Scores = ScaledAndReducedData!.Multiply(EigenVectors);
            if (normalize)
            {
                Vector<double> rootEigenvalues = EigenValues!.PointwisePower(0.5);
                Scores = Scores.Divide(rootEigenvalues);
            }
        }


    }
}
