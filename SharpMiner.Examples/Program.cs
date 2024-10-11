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
    

    var specs = new Specs(factorMethod: FactorMethod.PCA, dataSet: DataSet.LoadFromMatrix(data), 
        decompositionMethod: DecompositionMethod.Svd);
    var corepca = new CorePCA(specs);

    var projections = corepca.Project(5);

    //var singularValues = corepca.Svd.VT.Transpose();

    var exp = corepca.GetExplainedVariance(5);

    DelimitedWriter.Write(filePath: "projections.csv", projections);

    //DelimitedWriter.Write(filePath: "singularValues.csv", singularValues);

    var pca = new PrincipalComponentAnalysis(specs);

    

    DelimitedWriter.Write(filePath: "columnCoordinates.csv", pca.ColumnsResults.Coordinates);
    DelimitedWriter.Write(filePath: "columnSquaredCosinus.csv", pca.ColumnsResults.SquaredCosinus);
    DelimitedWriter.Write(filePath: "columnContributions.csv", pca.ColumnsResults.Contributions);

    DelimitedWriter.Write(filePath: "rowCoordinates.csv", pca.RowResults.Coordinates);
    DelimitedWriter.Write(filePath: "rowSquaredCosinus.csv", pca.RowResults.SquaredCosinus);
    DelimitedWriter.Write(filePath: "rowContributions.csv", pca.RowResults.Contributions);

    DelimitedWriter.Write(filePath: "Eigenvectors.csv", pca.EigenVectors);

    DelimitedWriter.Write(filePath: "ScaledReducedData.csv", pca.DatasetStatistics.ScaledAndReducedData);

    var plot = Visualisations.ScreePlot([29.96, 23.16, 17, 11.6, 7, 3]);
    plot.SavePng("scrreplot.png", 800, 600);
    
    var correlations = new (double, double)[] {(0.890168764861295, 0.3608298881130253),
        (-0.46014270644790806, 0.8827162691623838),
        (0.9915551834193608, 0.023415188379166344),
        (0.9649789606692489, 0.06399984704374741)
    };

    var correlationCircle = Visualisations.CorrelationCircle(correlations);
    correlationCircle.SavePng("correlationCirlce.png", 800, 800);

    var firstAxis =  pca.ColumnsResults.Coordinates.ToRowArrays()
        .Select(col => (col[0], col[1])).ToArray();

}

