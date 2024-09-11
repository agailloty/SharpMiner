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
            Assert.That(result, Is.EqualTo(expected).Within(1e-16));
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
            Assert.That(result, Is.EqualTo(expected).Within(1e-16));
        }
    }
}
