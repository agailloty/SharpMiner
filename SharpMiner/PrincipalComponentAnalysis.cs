using System;
using System.Collections.Generic;
using System.Linq;

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics.Statistics;

namespace SharpMiner
{
    public class PrincipalComponentAnalysis
    {
        private readonly int _defaultComponents;

        #region Public properties
        public Vector<double> EigenValues { get; private set; }
        public Matrix<double> EigenVectors { get; private set; }
        public Matrix<double> Scores { get; private set; }
        public Matrix<double> Projections { get; private set; }
        public Matrix<double> Coefficients { get; private set; }
        
        public DatasetStatistics DatasetStatistics { get; }

        public Matrix<double> Dataset {  get; }
        public Svd<double> Svd { get; private set; }
        public FactorResults ColumnsResults { get; private set; }
        public FactorResults RowResults { get; private set; }

        #endregion

        /// <summary>
        /// The steps to compute a PCA are the following : compute eignenvalues, compute principal components from the eigenvalues
        /// and project
        /// </summary>
        /// <param name="data"></param>
        /// <param name="weights"></param>
        /// <param name="ncomponents"></param>
        /// <exception cref="ArgumentException"></exception>
        public PrincipalComponentAnalysis(Matrix<double> data, IEnumerable<double> weights = null, int? ncomponents = null)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            Dataset = data;

            DatasetStatistics = new DatasetStatistics(data);
            
            ComputeEigen(DatasetStatistics.ScaledAndReducedData);

            if (ncomponents ==null)
                ncomponents = data.ColumnCount;

            _defaultComponents = ncomponents.Value;

            ComputePCAFromEigen(normalize: false);
        }

        /// <summary>
        /// Computes both Eigenvalues & Eigenvectors
        /// </summary>
        /// <returns></returns>
        private void ComputeEigen(Matrix<double> data)
        {
            var svd = data.Svd();
            if (svd != null)
            {
                Svd = svd;
                EigenValues = svd.S.PointwisePower(2.0);
                EigenVectors = svd.VT.Transpose();
            }
        }

        private void ComputePCAFromEigen(bool normalize = true)
        {
            Scores = DatasetStatistics.ScaledAndReducedData.Multiply(EigenVectors);
            Coefficients = EigenVectors;
            Projections = Scores.Multiply(Coefficients);
            if (normalize)
            {
                Vector<double> rootEigenvalues = EigenValues.PointwisePower(0.5);
                Scores = Scores.DivideByVector(rootEigenvalues);
                Coefficients = Coefficients.Transpose()
                    .MultiplyByColumnVector(rootEigenvalues).Transpose();

                var std = DenseVector.Build.DenseOfArray(DatasetStatistics.ColumnStandardDeviations);

                Projections = Projections.MultiplyByColumnVector(std);
            }
            SetRowsAndColumnsStatistics(Scores);
        }

        private void SetRowsAndColumnsStatistics(Matrix<double> scores)
        {
            if (scores != null)
            {
                var stats = ComputeStatistics(scores);
                RowResults = stats.Rows;
                ColumnsResults = stats.Columns;
            }
        }

        private (FactorResults Rows, FactorResults Columns) ComputeStatistics(Matrix<double> scores)
        {
            Vector<double> weighedEigenvalues = DenseMatrix.Build.Dense(Dataset.RowCount, Dataset.ColumnCount, 1.0)
                .MultiplyByColumnVector(DatasetStatistics.SquaredColumnWeights.ToVector())
                .Transpose()
                .MultiplyByColumnVector(DatasetStatistics.SquaredRowWeights.ToVector())
                .RowSums();

            weighedEigenvalues = Svd.S
                .PointwiseDivide(weighedEigenvalues);

            var columnCoordinates = DenseMatrix.Build.Dense(scores.ColumnCount, scores.ColumnCount);
            var columnsSquaredCosinus = DenseMatrix.Build.Dense(scores.ColumnCount, scores.ColumnCount);

            for (int scoreIndex = 0; scoreIndex < scores.ColumnCount; scoreIndex++)
            {
                for (int datasetIndex = 0; datasetIndex < scores.ColumnCount; datasetIndex++)
                {
                    double corcoeff = Correlation.Pearson(scores.Column(scoreIndex), Dataset.Column(datasetIndex));
                    columnCoordinates[scoreIndex, datasetIndex] = corcoeff;
                    columnsSquaredCosinus[scoreIndex, datasetIndex] = Math.Pow(corcoeff, 2);
                }
            }

            Vector<double> weighedEigenvaluesSquared = weighedEigenvalues.PointwisePower(2);

            Matrix<double> columnsContributions = columnsSquaredCosinus
                .Multiply(100)
                .MultiplyByColumnVector(DatasetStatistics.ColumnWeights.ToVector())
                .DivideByVector(weighedEigenvaluesSquared);


            Matrix<double> rowCoordinates = scores;
            Matrix<double> rowCoordinatesSquared = scores.PointwisePower(2);

            Matrix<double> rowSquaredCosinus =  rowCoordinatesSquared
                .DivideByRowVector(DatasetStatistics.RowsEuclidianDistance.ToVector());


            Matrix<double> rowContribution = rowCoordinatesSquared
                .Multiply(100)
                .MultiplyByRowVector(DatasetStatistics.RowWeights.ToVector())
                .DivideByVector(weighedEigenvaluesSquared);

            FactorResults rows = new FactorResults(rowCoordinates, rowSquaredCosinus, rowContribution);

            FactorResults columns = new FactorResults(columnCoordinates, columnsSquaredCosinus, columnsContributions);

            return (rows, columns);
        }
    }
}
