using System.Globalization;

using MathNet.Numerics.Data.Text;
using MathNet.Numerics.LinearAlgebra;

namespace SharpMiner.Test
{
    internal class DataWranglingTest
    {
        private Matrix<double> _data;
        [SetUp]
        public void Setup()
        {
            string csvFilePath = "Data/indicators-headless.csv";
            var provider = new NumberFormatInfo { NumberDecimalSeparator = "." };
            _data = DelimitedReader.Read<double>(filePath: csvFilePath, 
                                                                delimiter: ",", 
                                                                formatProvider: provider);


        }

        [Test]
        public void CenterAndScale_ShouldMatchROutput()
        {

            // Expected centered and scaled matrix
            string csvFilePath = "Data/indicators_centered_scaled.csv";
            var provider = new NumberFormatInfo { NumberDecimalSeparator = "." };
            var expected = DelimitedReader.Read<double>(filePath: csvFilePath,
                                                                delimiter: ",",
                                                                formatProvider: provider);

            // Act
            var result = MatrixHelper.CenterAndScale(_data);

            // Assert
            double tolerance = 1e-10;

            Assert.IsTrue(AreMatricesEqual(expected, result, tolerance));
        }

        private bool AreMatricesEqual(Matrix<double> matrix1, Matrix<double> matrix2, double tolerance)
        {
            if (matrix1.RowCount != matrix2.RowCount || matrix1.ColumnCount != matrix2.ColumnCount)
            {
                return false;
            }

            for (int i = 0; i < matrix1.RowCount; i++)
            {
                for (int j = 0; j < matrix1.ColumnCount; j++)
                {
                    if (Math.Abs(matrix1[i, j] - matrix2[i, j]) > tolerance)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        [Test]
        public void CenterAndScale_ZeroStandardDeviation_ShouldOnlyCenterMatrix()
        {
            // Arrange
            var data = Matrix<double>.Build.DenseOfArray(new double[,]
            {
            { 2, 2, 2 },
            { 2, 2, 2 },
            { 2, 2, 2 }
            });

            // Expected matrix (centered but not scaled due to zero standard deviation)
            var expected = Matrix<double>.Build.DenseOfArray(new double[,]
            {
            { 0, 0, 0 },
            { 0, 0, 0 },
            { 0, 0, 0 }
            });

            // Act
            var result = MatrixHelper.CenterAndScale(data);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
