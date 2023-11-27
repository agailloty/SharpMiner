using SharpMiner.Test;
using MathNet.Numerics.Data.Text;
using System.Globalization;

string csvFilePath = "indicators.csv";
var provider = new NumberFormatInfo { NumberDecimalSeparator = "." };

var data = DelimitedReader.Read<double>(filePath: csvFilePath, delimiter: ",", formatProvider: provider, hasHeaders: true);

PrincipalComponentEstimator pca;

if (data != null)
{
    pca = new PrincipalComponentEstimator(data, ncomponents: 5);

    var projections = pca.Project();
}

