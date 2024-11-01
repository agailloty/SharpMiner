using SharpMiner;
using MathNet.Numerics.Data.Text;
using System.Globalization;
using SharpMiner.Graphics;
using SharpMiner.Core;

string csvFilePath = "Datasets/indicators.csv";
var provider = new NumberFormatInfo { NumberDecimalSeparator = "." };

var data = DelimitedReader.Read<double>(filePath: csvFilePath, delimiter: ",", formatProvider: provider, hasHeaders: true);

if (data != null)
{
    

    var specs = new Specs(factorMethod: FactorMethod.PCA, 
                                numberOfComponents : 5,
                                dataSet: data);

    var corepca = new PrincipalComponentAnalysis(specs);


    var projections = corepca.Components;


    //DelimitedWriter.Write(filePath: "projections.csv", projections);

    //DelimitedWriter.Write(filePath: "singularValues.csv", singularValues);

    var pca = new PrincipalComponentAnalysis(specs);


    DelimitedWriter.Write(filePath: "columnCoordinates.csv", corepca.ColumnsResults.Coordinates);
    DelimitedWriter.Write(filePath: "columnSquaredCosinus.csv", corepca.ColumnsResults.SquaredCosinus);
    DelimitedWriter.Write(filePath: "columnContributions.csv", corepca.ColumnsResults.Contributions);

    DelimitedWriter.Write(filePath: "rowCoordinates.csv", corepca.RowsResults.Coordinates);
    DelimitedWriter.Write(filePath: "rowSquaredCosinus.csv", corepca.RowsResults.SquaredCosinus);
    DelimitedWriter.Write(filePath: "rowContributions.csv", corepca.RowsResults.Contributions);


    var plot = Visualisations.ScreePlot(corepca.ExplainedVariance);
    plot.SavePng("scrreplot.png", 800, 600);


}

