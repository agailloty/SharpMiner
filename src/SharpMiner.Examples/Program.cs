using SharpMiner;
using System.Globalization;
using SharpMiner.Graphics;
using MathNet.Numerics.Data.Text;
using System.Data;
using static SharpMiner.ResultsWriter;

string csvFilePath = "Datasets/indicators.csv";
string indicator_mixed = "Datasets/indicators_mixed.csv";
string indicator_headless = "Datasets/indicators-headless.csv";
var provider = new NumberFormatInfo { NumberDecimalSeparator = "." };

string urlPath = "https://raw.githubusercontent.com/agailloty/Outils-Analyses-R/refs/heads/main/dataset/indicators_numerics.csv";
string indicator_mixed_url = "https://raw.githubusercontent.com/agailloty/Outils-Analyses-R/refs/heads/main/dataset/indicators.csv";

string[] oneHotColumns = { "REGION", "INCOME_GROUP" };
string[] labelEncodedCol = { "COUNTRY" };

var data = DataSetLoader.ReadDataTable(indicator_mixed_url, oneHotEncodedColumns: oneHotColumns, labelEncodedColumns: labelEncodedCol);


if (data != null)
{
    var specs = new PCASpecs(numberOfComponents : 5, dataSet: data);

    var corepca = new PrincipalComponentAnalysis(specs);


    var projections = corepca.Components;


    var pca = new PrincipalComponentAnalysis(specs);


    corepca.ScreePlot().SavePng("screeplot.png", 250, 250); 
    corepca.CorrelationCircle().SavePng("correlation.png", 800, 800);


}

