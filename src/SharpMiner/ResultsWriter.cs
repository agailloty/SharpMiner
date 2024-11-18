using MathNet.Numerics.LinearAlgebra;

using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMiner
{
    public static class ResultsWriter
    {
        public static void PrintResults(PrincipalComponentAnalysis pca)
        {
            Console.WriteLine("Principal Component Analysis");
            Console.WriteLine("Number of components: " + pca.Components.Length);
            Console.WriteLine("Cumulative explained variance: " + string.Join(", ", pca.CumulativeExplainedVariance));
            
            Console.WriteLine("Coordinates:");
            DisplayMatrix(pca.RowsResults.Coordinates);

            Console.WriteLine("Squared cosinus:");
            DisplayMatrix(pca.RowsResults.SquaredCosinus);

        }

        public static void WriteResultsOnFile(PrincipalComponentAnalysis pca)
        {
        }

        // Display only 2 numbers after the comma
        private static void DisplayMatrix(Matrix<double> matrix)
        {
            for (int i = 0; i < matrix.RowCount; i++)
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    Console.Write($"{matrix[i, j]:F2} ");
                }
                Console.WriteLine();
            }
        }
    }
}
