using MathNet.Numerics.LinearAlgebra;
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
    }
}
