using System;
using System.Linq;

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Factorization;

using ScottPlot;

namespace SharpMiner
{
    /// <summary>
    /// Principal component analysis (PCA) is a linear dimensionality reduction technique with applications in exploratory data analysis, visualization and data preprocessing.
    ///The data is linearly transformed onto a new coordinate system such that the directions(principal components) capturing the largest variation in the data can be easily identified.
    /// </summary>
    public class PrincipalComponentAnalysis
    {

        #region Public properties
        public Vector<double> EigenValues { get; }
        public Matrix<double> EigenVectors { get; }
        public Matrix<double> Scores { get; private set; }
        public Matrix<double> Projections { get; private set; }
        public Matrix<double> Coefficients { get; private set; }
        
        public DatasetStatistics DatasetStatistics { get; }

        public Matrix<double> Dataset {  get; }
        public Svd<double> Svd { get; }
        public FactorResults ColumnsResults { get; private set; }
        public FactorResults RowResults { get; private set; }

        #endregion

        /// <summary>
        /// The steps to compute a PCA are the following : compute eignenvalues, compute principal components from the eigenvalues
        /// and project
        /// </summary>
        public PrincipalComponentAnalysis(Specs specifications)
        {
            Dataset = specifications.DataSet.Data;
            DatasetStatistics = new DatasetStatistics(Dataset, specifications.RowsWeights);
            
            (Svd, EigenValues, EigenVectors) = ComputeEigen(DatasetStatistics);

            ComputePCAFromEigen(normalize: false);
        }

        #region Public methods
        #endregion

        #region Private methods

        /// <summary>
        /// Computes both Eigenvalues & Eigenvectors
        /// </summary>
        /// <returns></returns>
        private static (Svd<double> svd, Vector<double> eigenvalues, Matrix<double> eigenvectors) ComputeEigen(DatasetStatistics dataset)
        {
            Svd<double> svd = ComputeAjustedSvd(dataset.ScaledAndReducedData, dataset.RowWeights, dataset.ColumnWeights);
            Vector<double> eigenvalues = svd.S.PointwisePower(2.0);
            Matrix<double> eigenvectors = svd.VT.Transpose();

            return (svd, eigenvalues, eigenvectors);
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
                .MultiplyByColumnVector(DatasetStatistics.RootSquaredRowWeights.ToVector())
                .RowSums();

            weighedEigenvalues = Svd.S
                .PointwiseDivide(weighedEigenvalues);

            var diagonalWeighedEigenvalues = Matrix<double>.Build.Diagonal(weighedEigenvalues.ToArray());

            var rightSingularSigns = EigenVectors.ColumnSums()
                .Select(x => Convert.ToDouble(Math.Sign(x)))
                .ToArray();

            Matrix<double> columnCoordinates = EigenVectors.MultiplyByRowVector(rightSingularSigns.ToVector())
                .Multiply(diagonalWeighedEigenvalues);

            Matrix<double> columnsSquaredCosinus = columnCoordinates.PointwisePower(2.0);

            //TODO : Utiliser la fonction Math.Sign pour trouver le signe (-1, 0, 1) de la somme des lignes de Svd.VT
            // Puis mutliplier chaque colonne de la matrice des coordonnées par le signe. 

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

        private static Svd<double> ComputeAjustedSvd(Matrix<double> reducedAndScaledDataset, double[] rowWeights, double[] columnWeights)
        {
            Matrix<double> m_rowWeights = Matrix<double>.Build.DenseOfRowArrays(rowWeights);
            Matrix<double> m_columnWeights = Matrix<double>.Build.DenseOfColumnArrays(columnWeights);

            reducedAndScaledDataset =
                reducedAndScaledDataset
                .MultiplyByColumnVector(columnWeights.ToVector().PointwisePower(0.5))
                .Transpose()
                .MultiplyByColumnVector(rowWeights.ToVector().PointwisePower(0.5))
                .Transpose();

            return reducedAndScaledDataset.Svd();
        }

        #endregion
    }
}
