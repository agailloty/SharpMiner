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

    var plot = Visualisations.ScreePlot([35, 25, 17, 11, 7, 3]);
    plot.SavePng("scrreplot.png", 800, 600);

    var correlations = new (double, double)[] {(0.890168764861295, 0.3608298881130253),
        (-0.46014270644790806, 0.8827162691623838),
        (0.9915551834193608, 0.023415188379166344),
        (0.9649789606692489, 0.06399984704374741)
    };

    var correlationCircle = Visualisations.CorrelationCircle(correlations);

    correlationCircle.SavePng("correlationCirlce.png", 800, 800);

}

