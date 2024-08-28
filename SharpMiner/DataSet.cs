using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using MathNet.Numerics.Data.Text;

using MathNet.Numerics.LinearAlgebra;

namespace SharpMiner
{
    /// <summary>
    /// This class represents a tabular dataset with rows and columns
    /// </summary>
    public class DataSet
    {
        /// <summary>
        /// The matrix data whic is to be exposed
        /// </summary>
        public Matrix<double> Data { get; }
        private DataSet(Matrix<double> matrix) 
        {
            Data = matrix;
            RowCount = matrix.RowCount;
            ColumnCount = matrix.ColumnCount;
        }
        /// <summary>
        /// Create a dataset instance from CSV file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="numberProvider"></param>
        /// <param name="hasHeaders"></param>
        /// <returns></returns>
        public static DataSet LoadFromCsvFile(string fileName, NumberFormatInfo numberProvider = null, bool hasHeaders = true) 
        {
            if (numberProvider == null)
            {
                numberProvider = new NumberFormatInfo { NumberDecimalSeparator = ".",};
            }

            Matrix<double> data = DelimitedReader.Read<double>(filePath: fileName, delimiter: ",", formatProvider: numberProvider, hasHeaders: hasHeaders);

            return new DataSet(data);
        }

        /// <summary>
        /// Load CSV file from an http remote location
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static DataSet LoadCsvFromRemoteFile(string url)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Number of rows in the dataset
        /// </summary>
        public long RowCount { get; }
        /// <summary>
        /// Number of column
        /// </summary>
        public long ColumnCount { get; }

        /// <summary>
        /// Load dataset from an existing matrix
        /// </summary>
        /// <param name="matrix"></param>
        public static DataSet LoadFromMatrix(Matrix<double> matrix)
        {
            return new DataSet(matrix);
        }
    }
}
