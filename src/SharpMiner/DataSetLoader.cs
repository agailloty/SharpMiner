using System;
using System.Globalization;
using System.IO;
using System.Net.Http;

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
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static Matrix<double> LoadFromCsvFile(string fileName, 
                                                     NumberFormatInfo numberProvider = null, 
                                                     bool hasHeaders = true, 
                                                     string delimiter = ",") 
        {
            if (numberProvider == null)
            {
                numberProvider = new NumberFormatInfo { NumberDecimalSeparator = "."};
            }

            Matrix<double> data = DelimitedReader.Read<double>(filePath: fileName, delimiter: delimiter, formatProvider: numberProvider, hasHeaders: hasHeaders);

            return data;
        }

        /// <summary>
        /// Create a dataset instance from a remote CSV file
        /// </summary>
        /// <param name="url"></param>
        /// <param name="numberProvider"></param>
        /// <param name="hasHeaders"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static Matrix<double> LoadCsvFromRemoteFile(string url,
                                                     NumberFormatInfo numberProvider = null,
                                                     bool hasHeaders = true,
                                                     string delimiter = ",")
        {
            if (numberProvider == null)
            {
                numberProvider = new NumberFormatInfo { NumberDecimalSeparator = "." };
            }
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();
                string csvContent = response.Content.ReadAsStringAsync().Result;

                using (var reader = new StringReader(csvContent))
                {
                    var data = DelimitedReader.Read<double>(reader, delimiter: delimiter, hasHeaders: hasHeaders, formatProvider: numberProvider);
                    return data;
                }
            }
        }
    }
}
