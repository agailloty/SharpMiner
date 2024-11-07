using System.Data;
using System;
using System.Globalization;
using System.IO;
using System.Net.Http;

using MathNet.Numerics.Data.Text;

using MathNet.Numerics.LinearAlgebra;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace SharpMiner
{
    /// <summary>
    /// This class contains helper methods to read a dataset.
    /// </summary>
    public static class DataSetLoader
    {
        /// <summary>
        /// Loads a DataTable from a CSV file.
        /// </summary>
        /// <param name="fileName">The path to the CSV file.</param>
        /// <param name="hasHeaders">Indicates if the CSV file has headers.</param>
        /// <param name="delimiter">The delimiter used in the CSV file.</param>
        /// <param name="numberProvider">The number format provider.</param>
        /// <returns>A DataTable containing the data from the CSV file.</returns>
        public static DataTable LoadDataTableFromCsvFile(string fileName,
                                                       bool hasHeaders = true,
                                                       string delimiter = ",",
                                                       NumberFormatInfo numberProvider = null)
        {
            if (numberProvider == null)
            {
                numberProvider = new NumberFormatInfo { NumberDecimalSeparator = "." };
            }

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

                var sampleRows = new List<string[]>();
                var allRows = new List<string[]>();
                while (!reader.EndOfStream)
                {
                    var row = ParseCsvLine(reader.ReadLine(), delimiter);
                    allRows.Add(row);
                }

                int typeInferenceRows = (int)(allRows.Count * 0.2);
                sampleRows.AddRange(allRows.Take(typeInferenceRows));

                InferColumnTypes(dataTable, sampleRows, numberProvider);

                foreach (var values in allRows)
                {
                    var row = dataTable.NewRow();
                    for (int i = 0; i < values.Length; i++)
                    {
                        row[i] = Convert.ChangeType(values[i], dataTable.Columns[i].DataType, numberProvider);
                    }
                    dataTable.Rows.Add(row);
                }
            }
            return dataTable;
        }

        /// <summary>
        /// Loads a DataTable from a remote CSV file.
        /// </summary>
        /// <param name="url">The URL of the remote CSV file.</param>
        /// <param name="hasHeaders">Indicates if the CSV file has headers.</param>
        /// <param name="delimiter">The delimiter used in the CSV file.</param>
        /// <param name="numberProvider">The number format provider.</param>
        /// <returns>A DataTable containing the data from the remote CSV file.</returns>
        public static DataTable LoadDataTableCsvFromRemoteFile(string url,
                                                               bool hasHeaders = true,
                                                               string delimiter = ",",
                                                               NumberFormatInfo numberProvider = null)
        {
            if (numberProvider == null)
            {
                numberProvider = new NumberFormatInfo { NumberDecimalSeparator = "." };
            }

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

                    var sampleRows = new List<string[]>();
                    var allRows = new List<string[]>();
                    while (reader.Peek() > -1)
                    {
                        var row = ParseCsvLine(reader.ReadLine(), delimiter);
                        allRows.Add(row);
                    }

                    int typeInferenceRows = (int)(allRows.Count * 0.2);
                    sampleRows.AddRange(allRows.Take(typeInferenceRows));

                    InferColumnTypes(dataTable, sampleRows, numberProvider);

                    foreach (var values in allRows)
                    {
                        var row = dataTable.NewRow();

                        for (int i = 0; i < values.Length; i++)
                        {
                            if (dataTable.Columns[i].DataType == typeof(double))
                            {
                                row[i] = double.Parse(values[i], numberProvider);
                            }
                            else
                            {
                                row[i] = values[i];
                            }
                        }
                        dataTable.Rows.Add(row);
                    }
                }
            }
            return dataTable;
        }

        /// <summary>
        /// Infers the column types of a DataTable based on sample rows.
        /// </summary>
        /// <param name="dataTable">The DataTable to infer column types for.</param>
        /// <param name="sampleRows">The sample rows to use for type inference.</param>
        /// <param name="numberProvider">The number format provider.</param>
        private static void InferColumnTypes(DataTable dataTable, List<string[]> sampleRows, NumberFormatInfo numberProvider)
        {
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                int doubleCount = 0;
                int stringCount = 0;

                foreach (var row in sampleRows)
                {
                    if (double.TryParse(row[i], NumberStyles.Float | NumberStyles.AllowExponent, numberProvider, out _))
                    {
                        doubleCount++;
                    }
                    else
                    {
                        stringCount++;
                    }
                }

                if (doubleCount > stringCount)
                {
                    dataTable.Columns[i].DataType = typeof(double);
                }
                else
                {
                    dataTable.Columns[i].DataType = typeof(string);
                }
            }
        }

        /// <summary>
        /// Parses a line of CSV into an array of strings.
        /// </summary>
        /// <param name="line">The CSV line to parse.</param>
        /// <param name="delimiter">The delimiter used in the CSV file.</param>
        /// <returns>An array of strings representing the values in the CSV line.</returns>
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
        /// Creates a dataset instance from a CSV file.
        /// </summary>
        /// <param name="fileName">The path to the CSV file.</param>
        /// <param name="numberProvider">The number format provider.</param>
        /// <param name="hasHeaders">Indicates if the CSV file has headers.</param>
        /// <param name="delimiter">The delimiter used in the CSV file.</param>
        /// <returns>A Matrix containing the data from the CSV file.</returns>
        public static Matrix<double> LoadFromCsvFile(string fileName,
                                                     NumberFormatInfo numberProvider = null,
                                                     bool hasHeaders = true,
                                                     string delimiter = ",")
        {
            if (numberProvider == null)
            {
                numberProvider = new NumberFormatInfo { NumberDecimalSeparator = "." };
            }

            Matrix<double> data = DelimitedReader.Read<double>(filePath: fileName, delimiter: delimiter, formatProvider: numberProvider, hasHeaders: hasHeaders);

            return data;
        }

        /// <summary>
        /// Creates a dataset instance from a remote CSV file.
        /// </summary>
        /// <param name="url">The URL of the remote CSV file.</param>
        /// <param name="numberProvider">The number format provider.</param>
        /// <param name="hasHeaders">Indicates if the CSV file has headers.</param>
        /// <param name="delimiter">The delimiter used in the CSV file.</param>
        /// <returns>A Matrix containing the data from the remote CSV file.</returns>
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
