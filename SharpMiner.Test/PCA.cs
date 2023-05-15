using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Statistics;

namespace SharpMiner.Test
{
    public class PCA
    {
        private Matrix<double> CovarianceMatrix(Matrix<double> data)
        {
            // Center the data
            var centeredData = data.Subtract(data.ColumnMeans().At(0));

            // Calculate the covariance matrix
            var covarianceMatrix = centeredData.Transpose().Multiply(centeredData).Divide(data.RowCount - 1);

            return covarianceMatrix;
        }

        public Matrix<double> Transform(Matrix<double> data, int numComponents)
        {
            // Calculate the covariance matrix
            var covarianceMatrix = CovarianceMatrix(data);

            // Calculate the eigenvectors and eigenvalues
            var evd = covarianceMatrix.Evd(Symmetricity.Symmetric);

            var eigenvectors = evd.EigenVectors.ToColumnArrays()
                .OrderByDescending(x => x[0])
                .Take(numComponents)
                .ToList();

            double[][] result = new double[covarianceMatrix.RowCount][];
            for (int i = 0; i < covarianceMatrix.RowCount; i++)
            {
                result[i] = new double[numComponents];
                for (int j = 0; j < numComponents; j++)
                {
                    result[i][j] = eigenvectors[j][i];
                }
            }

            // Transform the data into the new space

            var transformedData = data.Multiply(DenseMatrix.OfColumnArrays(result.Transpose()));

            return transformedData;
        }



    }

}

