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
    

    var specs = new Specs(factorMethod: FactorMethod.PCA, numberOfComponents : 5,
                                dataSet: DataSet.LoadFromMatrix(data), 
                                decompositionMethod: DecompositionMethod.Svd);

    var corepca = new CorePCA(specs);
    var pcaRes = new PrincipalComponentAnalysis(specs);


    var projections = corepca.GetPrincipalComponents(5);


    //DelimitedWriter.Write(filePath: "projections.csv", projections);

    //DelimitedWriter.Write(filePath: "singularValues.csv", singularValues);

    var pca = new PrincipalComponentAnalysis(specs);

    /*

    DelimitedWriter.Write(filePath: "columnCoordinates.csv", pca.ColumnsResults.Coordinates);
    DelimitedWriter.Write(filePath: "columnSquaredCosinus.csv", pca.ColumnsResults.SquaredCosinus);
    DelimitedWriter.Write(filePath: "columnContributions.csv", pca.ColumnsResults.Contributions);

    DelimitedWriter.Write(filePath: "rowCoordinates.csv", pca.RowResults.Coordinates);
    DelimitedWriter.Write(filePath: "rowSquaredCosinus.csv", pca.RowResults.SquaredCosinus);
    DelimitedWriter.Write(filePath: "rowContributions.csv", pca.RowResults.Contributions);

    DelimitedWriter.Write(filePath: "Eigenvectors.csv", pca.EigenVectors);

    DelimitedWriter.Write(filePath: "ScaledReducedData.csv", pca.DatasetStatistics.ScaledAndReducedData);
    */

    var plot = Visualisations.ScreePlot(corepca.CumulativeExplainedVariance);
    plot.SavePng("scrreplot.png", 800, 600);


}

