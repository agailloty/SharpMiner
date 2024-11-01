using System;
using System.Globalization;

using MathNet.Numerics.Data.Text;

using MathNet.Numerics.LinearAlgebra;

namespace SharpMiner
{
    /// <summary>
    /// This class contains helper method to read a dataset
    /// </summary>
    public static class DataSetLoader
    {
        /// <summary>
        /// Create a dataset instance from CSV file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="numberProvider"></param>
        /// <param name="hasHeaders"></param>
        /// <returns></returns>
        public static Matrix<double> LoadFromCsvFile(string fileName, NumberFormatInfo numberProvider = null, bool hasHeaders = true) 
        {
            if (numberProvider == null)
            {
                numberProvider = new NumberFormatInfo { NumberDecimalSeparator = ".",};
            }

            Matrix<double> data = DelimitedReader.Read<double>(filePath: fileName, delimiter: ",", formatProvider: numberProvider, hasHeaders: hasHeaders);

            return data;
        }

        /// <summary>
        /// Load CSV file from an http remote location
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static Matrix<double> LoadCsvFromRemoteFile(string url)
        {
            throw new NotImplementedException();
        }
    }
}
