using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using SharpMiner.Test;
using MathNet.Numerics.Data.Text;
using System;
using static SharpMiner.Test.Utils;


string csvFilePath = "indicators-headless.csv";

var analysis = new PCA(csvFilePath);
analysis.Fit();

DelimitedWriter.Write("scaled.csv", analysis.ScaledData, ",");
DelimitedWriter.Write("cormat.csv", analysis.PearsonCorrelationMatrix, ",");
DelimitedWriter.Write("prcomp.csv", analysis.PrincipalComponents, ",");
DelimitedWriter.Write("evdmat.csv", analysis.EvdMatrix!.EigenVectors, ",");
DelimitedWriter.Write("explainedVariance.csv", analysis.ExplainedVariance!.ToColumnMatrix(), ",");
