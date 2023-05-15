using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpMiner.Test
{
    public static class Utils
    {
        public static Matrix<double> CrossProd(Matrix<double> x, Matrix<double> y)
        {
            return x.Transpose().Multiply(y);
        }
        public static Vector<double> ScaleAndReduce(Vector<double> vector) {
            return (vector - vector.Mean()).Divide(vector.StandardDeviation());
        }

        public static Matrix<double> ScaleAndReduce(Matrix matrix) 
        {
            Matrix<double> mat = new DenseMatrix(matrix.RowCount, matrix.ColumnCount);
            for (int i = 0; i < matrix.ColumnCount; i++)
                mat.SetColumn(i, ScaleAndReduce(matrix.Column(i)));
            return mat;
        }
    }
}
