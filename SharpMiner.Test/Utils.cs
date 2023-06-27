using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Statistics;

namespace SharpMiner.Test
{
    public static class Utils
    {
        public static Matrix<double> CrossProd(Matrix<double> x, Matrix<double> y) => x.Transpose().Multiply(y);
        public static Vector<double> ScaleAndReduce(Vector<double> vector) 
            => (vector - vector.Mean()).Divide(vector.StandardDeviation());

        public static Matrix<double> ScaleAndReduce(Matrix<double> matrix) 
        {
            Matrix<double> mat = new DenseMatrix(matrix.RowCount, matrix.ColumnCount);
            for (int i = 0; i < matrix.ColumnCount; i++)
                mat.SetColumn(i, ScaleAndReduce(matrix.Column(i)));
            return mat;
        }

        public static Matrix<double> Covariate(Matrix<double> matrix, Func<IEnumerable<double>, IEnumerable<double>, double> covariateFunc) 
        {
            // Output matrix is a square matrix (n*p) where n=p and n = n columns
            int n = matrix.ColumnCount;
            Matrix covmat = new DenseMatrix(n, n);
            for (int row = 0; row < n; row++) {
                for (int col = 0; col < n; col++)
                    covmat[row, col] = covariateFunc(matrix.Column(row), matrix.Column(col));
            }
            return covmat;
        }

        public static Matrix<double> CovarianceMatrix(Matrix<double> matrix) 
                    => Covariate(matrix, Statistics.Covariance);

        public static Matrix<double> CorrelationMatrix(Matrix<double> matrix) 
                    => Covariate(matrix, Correlation.Pearson);

    }
}
