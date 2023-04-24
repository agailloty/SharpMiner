using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using SharpMiner.Test;
using MathNet.Numerics.Data.Text;
using System;

/*
string csvFilePath = "indicators-headless.csv";
var data = DelimitedReader.Read<double>(csvFilePath, false, ",");



var pca = new PCA();
var transformedData = pca.Transform(data, 10);

DelimitedWriter.Write("out.csv", transformedData, ",");
*/

var m1 = DenseMatrix.OfArray(new double[,] {
    { 1.0, 2.0, 3.0 },
    { 4.0, 5.0, 6.0 },
    { 7.0, 8.0, 9.0 }
});

var m2 = DenseMatrix.OfArray(new double[,] {
    { 1.0, 2.0, 3.0, 2.4 },
    { 4.0, 5.0, 6.0, 4.0 },
    { 7.0, 8.0, 9.0, 5.0 }
});

var res = Utils.CrossProd(m1, m2);

Console.WriteLine(res);