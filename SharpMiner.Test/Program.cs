using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using System;

    Func<Vector<double>, Vector<double>, Vector<double>> moyPtab = (V, poids) 
    => (poids.PointwiseDivide(poids.Sum()) * Matrix<double>.Build.DenseOfColumnVectors(V)).ColumnSums();

    var notes = new DenseVector<double> {10, 12, 14, 8, 10, 8};
    var effectif = new DenseVector<double> {3, 10, 2, 5, 3, 1};

    var res = moyPtab(notes, effectif); 

    Console.WriteLine(res);