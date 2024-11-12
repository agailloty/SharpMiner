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
using MathNet.Numerics.LinearAlgebra.Double;

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
        /// <param name="oneHotEncodedColumns">Columns that need to be one hot encoded.</param>
        /// <param name="labelEncodedColumns">Columns that need to be label encoded.</param>
        /// <returns>A DataTable containing the data from the remote CSV file.</returns>
        public static DataTable LoadDataTableFromCsvFile(string fileName,
                                                 bool hasHeaders = true,
                                                 string delimiter = ",",
                                                 NumberFormatInfo numberProvider = null,
                                                 string[] oneHotEncodedColumns = null,
                                                 string[] labelEncodedColumns = null)
        {
            if (numberProvider == null)
            {
                numberProvider = new NumberFormatInfo { NumberDecimalSeparator = "." };
            }

            var dataTable = new DataTable();
            using (var reader = new StreamReader(fileName))
            {
                string[] headers = null;
                string[] firstLine = ParseCsvLine(reader.ReadLine(), delimiter);

                var sampleRows = new List<string[]>();
                var allRows = new List<string[]>();

                if (hasHeaders)
                {
                    headers = firstLine;
                } 
                else
                {
                    headers = Enumerable.Range(1, firstLine.Length).Select(i => "X" + i).ToArray();
                    allRows.Add(firstLine);
                }

                foreach (var header in headers)
                {
                    dataTable.Columns.Add(header);
                }

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
            if (oneHotEncodedColumns != null)
            {
                foreach (var column in oneHotEncodedColumns)
                {
                    DataColumn col = dataTable.Columns[column];
                    if (col != null && col.DataType == typeof(string))
                    {
                        CreateOneHotEncodedColumns(dataTable, col);
                        dataTable.Columns.Remove(col);
                    }
                }
            }

            if (labelEncodedColumns != null)
            {
                foreach (var column in labelEncodedColumns)
                {
                    DataColumn col = dataTable.Columns[column];
                    if (col != null && col.DataType == typeof(string))
                    {
                        CreateLabelEncodedColumn(dataTable, col);
                    }
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
        /// <param name="oneHotEncodedColumns">Columns that need to be one hot encoded.</param>
        /// <param name="labelEncodedColumns">Columns that need to be label encoded.</param>
        /// <returns>A DataTable containing the data from the remote CSV file.</returns>
        public static DataTable LoadDataTableCsvFromRemoteFile(string url,
                                                               bool hasHeaders = true,
                                                               string delimiter = ",",
                                                               NumberFormatInfo numberProvider = null, 
                                                               string[] oneHotEncodedColumns = null, string[] labelEncodedColumns = null)
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
                    string[] firstLine = ParseCsvLine(reader.ReadLine(), delimiter);

                    var sampleRows = new List<string[]>();
                    var allRows = new List<string[]>();

                    if (hasHeaders)
                    {
                        headers = firstLine;
                    }
                    else
                    {
                        headers = Enumerable.Range(1, firstLine.Length).Select(i => "X" + i).ToArray();
                        allRows.Add(firstLine);
                    }

                    foreach (var header in headers)
                    {
                        dataTable.Columns.Add(header);
                    }
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
                if (oneHotEncodedColumns != null)
                {
                    foreach (var column in oneHotEncodedColumns)
                    {
                        DataColumn col = dataTable.Columns[column];
                        if (col != null && col.DataType == typeof(string))
                        {
                            CreateOneHotEncodedColumns(dataTable, col);
                            dataTable.Columns.Remove(col);
                        }
                    }
                }

                if (labelEncodedColumns != null)
                {
                    foreach (var column in labelEncodedColumns)
                    {
                        DataColumn col = dataTable.Columns[column];
                        if (col != null && col.DataType == typeof(string))
                        {
                            CreateLabelEncodedColumn(dataTable, col);
                        }
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

        private static void CreateOneHotEncodedColumns(DataTable dataTable, DataColumn column)
        {
            string[] values = new string[dataTable.Rows.Count];

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                values[i] = dataTable.Rows[i][column].ToString();
            }

            string[] uniqueValues = values.Distinct().ToArray();

            List<int[]> encodedColumns = new List<int[]>();

            foreach (string modality in uniqueValues)
            {
                string modalityName = column.ColumnName + "_" + modality;
                dataTable.Columns.Add(modalityName, typeof(int));

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    dataTable.Rows[i][modalityName] = values[i] == modality ? 1 : 0;
                }
            }
        }

        private static void CreateLabelEncodedColumn(DataTable dataTable, DataColumn column)
        {
            string[] values = new string[dataTable.Rows.Count];

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                values[i] = dataTable.Rows[i][column].ToString();
            }

            string[] uniqueValues = values.Distinct().ToArray();

            int[] encodedColumns = Enumerable.Range(1, uniqueValues.Length).ToArray();

            Dictionary<string, int> encoding = new Dictionary<string, int>();

            for (int i = 0; i < uniqueValues.Length; i++)
            {
                encoding.Add(uniqueValues[i], encodedColumns[i]);
            }

            string columnName = column.ColumnName;
            dataTable.Columns.Remove(column);
            dataTable.Columns.Add(columnName, typeof(int));

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                dataTable.Rows[i][columnName] = encoding[values[i]];
            }
        }
        /// <summary>
        /// Converts a DataTable to a Matrix of doubles.
        /// </summary>
        /// <param name="dataTable">The DataTable to convert.</param>
        /// <returns>A Matrix of doubles representing the data in the DataTable.</returns>
        public static Matrix<double> ConvertToMatrix(this DataTable dataTable) 
        {
            // Remove non-numeric columns
            var numericColumns = dataTable.Columns.Cast<DataColumn>()
            .Where(col => col.DataType == typeof(double) || col.DataType == typeof(int))
            .ToList();

            var rows = dataTable.Rows.Count;
            var cols = numericColumns.Count;
            var matrix = new DenseMatrix(rows, cols);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    matrix[i, j] = Convert.ToDouble(dataTable.Rows[i][numericColumns[j]]);
                }
            }

            return matrix;
        }
    }
}
