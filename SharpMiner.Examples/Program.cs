using SharpMiner;
using MathNet.Numerics.Data.Text;
using System.Globalization;
using SharpMiner.Graphics;

string csvFilePath = "Datasets/indicators.csv";
var provider = new NumberFormatInfo { NumberDecimalSeparator = "." };

var data = DelimitedReader.Read<double>(filePath: csvFilePath, delimiter: ",", formatProvider: provider, hasHeaders: true);

if (data != null)
{
    var pca = new PrincipalComponentAnalysis(data, ncomponents: 5);

    DelimitedWriter.Write(filePath: "columnCoordinates.csv", pca.ColumnsResults.Coordinates);
    DelimitedWriter.Write(filePath: "columnSquaredCosinus.csv", pca.ColumnsResults.SquaredCosinus);
    DelimitedWriter.Write(filePath: "columnContributions.csv", pca.ColumnsResults.Contributions);

    DelimitedWriter.Write(filePath: "rowCoordinates.csv", pca.RowResults.Coordinates);
    DelimitedWriter.Write(filePath: "rowSquaredCosinus.csv", pca.RowResults.SquaredCosinus);
    DelimitedWriter.Write(filePath: "rowContributions.csv", pca.RowResults.Contributions);

    var plot = Visualisations.ScreePlot(new double[] { 35, 25, 17, 11, 7, 3 });
    plot.SavePng("scrreplot.png", 800, 600);

    var correlationCircle = Visualisations.CorrelationCircle(pca.DatasetStatistics.CorrelationMatrix);

    correlationCircle.SavePng("correlationCirlce.png", 800, 800);

}

