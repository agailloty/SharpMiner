using System.Data;
using System;
using System.Globalization;
using System.IO;
using System.Net.Http;

using MathNet.Numerics.Data.Text;

using MathNet.Numerics.LinearAlgebra;
using System.Text.RegularExpressions;

namespace SharpMiner
{
    /// <summary>
    /// This class contains helper method to read a dataset
    /// </summary>
    public static class DataSetLoader
    {
        public static DataTable LoadDataTableFromCsvFile(string fileName,
                                                bool hasHeaders = true,
                                                string delimiter = ",")
        {
            var dataTable = new DataTable();
            using (var reader = new StreamReader(fileName))
            {
                string[] headers = null;
                if (hasHeaders)
                {
                    headers = ParseCsvLine(reader.ReadLine(), delimiter);
                    foreach (var header in headers)
                    {
                        dataTable.Columns.Add(header);
                    }
                }

                while (!reader.EndOfStream)
                {
                    var values = ParseCsvLine(reader.ReadLine(), delimiter);
                    var row = dataTable.NewRow();
                    for (int i = 0; i < values.Length; i++)
                    {
                        row[i] = values[i];
                    }
                    dataTable.Rows.Add(row);
                }
            }
            return dataTable;
        }

        public static DataTable LoadDataTableCsvFromRemoteFile(string url,
                                                        bool hasHeaders = true,
                                                        string delimiter = ",")
        {
            var dataTable = new DataTable();
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();
                string csvContent = response.Content.ReadAsStringAsync().Result;

                using (var reader = new StringReader(csvContent))
                {
                    string[] headers = null;
                    if (hasHeaders)
                    {
                        headers = ParseCsvLine(reader.ReadLine(), delimiter);
                        foreach (var header in headers)
                        {
                            dataTable.Columns.Add(header);
                        }
                    }

                    while (reader.Peek() > -1)
                    {
                        var values = ParseCsvLine(reader.ReadLine(), delimiter);
                        var row = dataTable.NewRow();
                        for (int i = 0; i < values.Length; i++)
                        {
                            row[i] = values[i];
                        }
                        dataTable.Rows.Add(row);
                    }
                }
            }
            return dataTable;
        }

        private static string[] ParseCsvLine(string line, string delimiter)
        {
            var pattern = string.Format("(?<=^|{0})(\"(?:[^\"]|\"\")*\"|[^{0}]*)", delimiter);
            var matches = Regex.Matches(line, pattern);
            var values = new string[matches.Count];
            for (int i = 0; i < matches.Count; i++)
            {
                values[i] = matches[i].Value.Trim('"').Replace("\"\"", "\"");
            }
            return values;
        }



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
