using MathNet.Numerics.LinearAlgebra;

namespace SharpMiner
{
    public class FactorResults
    {
        public FactorResults(Matrix<double> coordinates, Matrix<double> squaredCosinus, Matrix<double> contributions) 
        { 
            Coordinates = coordinates;
            SquaredCosinus = squaredCosinus;
            Contributions = contributions;
        }

        public Matrix<double> Coordinates { get; }
        public Matrix<double> SquaredCosinus { get; }
        public Matrix<double> Contributions { get; }
    }
}
