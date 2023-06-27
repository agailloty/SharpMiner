using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using SharpMiner.Test;
using MathNet.Numerics.Data.Text;
using System;
using static SharpMiner.Test.Utils;


string csvFilePath = "indicators-headless.csv";

var analysis = new PCA(csvFilePath);
analysis.Fit();

DelimitedWriter.Write("prcomp.csv", analysis.PrincipalComponents, ",");