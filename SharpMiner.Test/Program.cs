using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using SharpMiner.Test;
using System;


var data = DenseMatrix.OfArray(new double[,]
{
    {1.0, 2.0, 3.0},
    {4.0, 5.0, 6.0},
    {7.0, 8.0, 9.0},
    {10.0, 11.0, 12.0}
});

var pca = new PCA();
var transformedData = pca.Transform(data, 2);

Console.WriteLine(transformedData);