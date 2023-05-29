using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using SharpMiner.Test;
using MathNet.Numerics.Data.Text;
using System;
using static SharpMiner.Test.Utils;


string csvFilePath = "indicators-headless.csv";
var data = DelimitedReader.Read<double>(csvFilePath, false, ",");
data = Utils.ScaleAndReduce(data);

DelimitedWriter.Write("scaled.csv", data, ",");

var covmat = Utils.CovarianceMatrix(data);

DelimitedWriter.Write("covmat.csv", covmat, ",");

var cormat = Utils.CorrelationMatrix(data);

DelimitedWriter.Write("cormat.csv", cormat, ",");

var evdmat = Utils.CorrelationMatrix(data).Evd();

var eigSUm = evdmat.EigenValues.Sum();
var explainedVar = (evdmat.EigenValues / eigSUm) * 100;

var projected = data.Multiply(covmat);


DelimitedWriter.Write("eigenvalues.csv", evdmat.EigenValues.ToColumnMatrix(), ",");
DelimitedWriter.Write("explainedVar.csv", explainedVar.ToRowMatrix(), ",");
DelimitedWriter.Write("eigenvectors.csv", evdmat.EigenVectors, ",");

DelimitedWriter.Write("projected.csv", projected, ",");