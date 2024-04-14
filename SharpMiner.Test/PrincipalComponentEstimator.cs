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

        #region Public properties
        public Vector<double>? EigenValues { get; }
        public Matrix<double>? EigenVectors { get; }
        public Matrix<double>? Scores { get; private set; }
        public Matrix<double>? Projections { get; private set; }
        public Matrix<double>? Coefficients { get; private set; }
        public Matrix<double>? ScaledAndReducedData { get; }
        public DatasetStatistics DatasetStatistics { get; }
        #endregion


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

            DatasetStatistics = new DatasetStatistics(data);

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
            Coefficients = EigenVectors!.Transpose();
            Projections = Scores.Multiply(Coefficients);
            if (normalize)
            {
                Vector<double> rootEigenvalues = EigenValues!.PointwisePower(0.5);
                Scores = Scores.DivideByVector(rootEigenvalues);
                Coefficients = Coefficients.Transpose()
                    .MultiplyByVector(rootEigenvalues).Transpose();

                var std = DenseVector.Build.DenseOfArray(DatasetStatistics.StandardDeviations);

                Projections = Projections.MultiplyByVector(std);
            }
        }
    }
}
