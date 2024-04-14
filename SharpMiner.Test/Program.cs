using SharpMiner.Test;
using MathNet.Numerics.Data.Text;
using System.Globalization;

string csvFilePath = "indicators.csv";
var provider = new NumberFormatInfo { NumberDecimalSeparator = "." };

var data = DelimitedReader.Read<double>(filePath: csvFilePath, delimiter: ",", formatProvider: provider, hasHeaders: true);

if (data != null)
{
    var pca = new PrincipalComponentEstimator(data, ncomponents: 5);

    DelimitedWriter.Write("projections.csv", pca.Projections, ";");

    var secondPCA = new PCA(data);
    secondPCA.Fit();
    DelimitedWriter.Write("projections2.csv", secondPCA.PrincipalComponents, ";");
}

