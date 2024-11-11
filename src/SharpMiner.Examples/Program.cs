using SharpMiner;
using System.Globalization;
using SharpMiner.Graphics;
using MathNet.Numerics.Data.Text;
using System.Data;

string csvFilePath = "Datasets/indicators.csv";
string indicator_mixed = "Datasets/indicators_mixed.csv";
string indicator_headless = "Datasets/indicators-headless.csv";
var provider = new NumberFormatInfo { NumberDecimalSeparator = "." };

var data = DataSetLoader.LoadFromCsvFile(csvFilePath, provider);
string urlPath = "https://raw.githubusercontent.com/agailloty/Outils-Analyses-R/refs/heads/main/dataset/indicators_numerics.csv";
string indicator_mixed_url = "https://raw.githubusercontent.com/agailloty/Outils-Analyses-R/refs/heads/main/dataset/indicators.csv";
var remoteData = DataSetLoader.LoadCsvFromRemoteFile(urlPath, provider);

string[] oneHotColumns = { "REGION", "INCOME_GROUP" };
string[] labelEncodedCol = { "COUNTRY" };

DataTable indicators_mixed_online = DataSetLoader.LoadDataTableCsvFromRemoteFile(indicator_mixed_url, oneHotEncodedColumns : oneHotColumns, labelEncodedColumns : labelEncodedCol);
DataTable indicators_mixed_local = DataSetLoader.LoadDataTableFromCsvFile(indicator_mixed, oneHotEncodedColumns : oneHotColumns, labelEncodedColumns : labelEncodedCol);
DataTable indicators_headless_local = DataSetLoader.LoadDataTableFromCsvFile(indicator_headless, oneHotEncodedColumns : oneHotColumns, labelEncodedColumns : labelEncodedCol, hasHeaders: false);


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

