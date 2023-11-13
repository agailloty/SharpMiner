using SharpMiner.Test;
using MathNet.Numerics.Data.Text;
using System.Globalization;

string csvFilePath = "indicators.csv";
var provider = new NumberFormatInfo { NumberDecimalSeparator = "." };

var data = DelimitedReader.Read<double>(filePath: csvFilePath, delimiter: ",", formatProvider: provider, hasHeaders: true);
var analysis = new PCA(data);

analysis.Fit();

DelimitedWriter.Write("scaled.csv", analysis.ScaledData, ",");
DelimitedWriter.Write("cormat.csv", analysis.PearsonCorrelationMatrix, ",");
DelimitedWriter.Write("prcomp.csv", analysis.PrincipalComponents, ",");
DelimitedWriter.Write("evdmat.csv", analysis.EvdMatrix!.EigenVectors, ",");
DelimitedWriter.Write("explainedVariance.csv", analysis.ExplainedVariance!.ToColumnMatrix(), ",");