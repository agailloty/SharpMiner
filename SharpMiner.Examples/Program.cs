using SharpMiner;
using MathNet.Numerics.Data.Text;
using System.Globalization;

string csvFilePath = "Datasets/indicators.csv";
var provider = new NumberFormatInfo { NumberDecimalSeparator = "." };

var data = DelimitedReader.Read<double>(filePath: csvFilePath, delimiter: ",", formatProvider: provider, hasHeaders: true);

if (data != null)
{
    var pca = new PrincipalComponentAnalysis(data, ncomponents: 5);

    DelimitedWriter.Write(filePath: "columnCoordinates.csv", pca.ColumnsResults.Coordinates);
    DelimitedWriter.Write(filePath: "columnSquaredCosinus.csv", pca.ColumnsResults.SquaredCosinus);

    DelimitedWriter.Write(filePath: "rowCoordinates.csv", pca.RowResults.Coordinates);
    DelimitedWriter.Write(filePath: "rowSquaredCosinus.csv", pca.RowResults.SquaredCosinus);
}

