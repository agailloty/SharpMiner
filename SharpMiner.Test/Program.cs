using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using SharpMiner.Test;
using MathNet.Numerics.Data.Text;
using System;
using static SharpMiner.Test.Utils;


string csvFilePath = "indicators-headless.csv";
var data = DelimitedReader.Read<double>(csvFilePath, false, ",");

var covmat = Utils.CovarianceMatrix(data);

DelimitedWriter.Write("covmat.csv", covmat, ",");

var cormat = Utils.CorrelationMatrix(data);

DelimitedWriter.Write("cormat.csv", cormat, ",");

