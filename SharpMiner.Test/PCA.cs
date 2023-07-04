using MathNet.Numerics.Data.Text;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;

namespace SharpMiner.Test
{
    public class PCA
    {
        private Matrix<double> _data;
        private Matrix<double>? _scaledData;
        private Evd<double>? _evdMatrix;
        private MathNet.Numerics.LinearAlgebra.Vector<double>? _explainedVariance;
        private Matrix<double>? _covarianceMatrix;
        private Matrix<double>? _pearsonCorrelationMatrix;
        private Matrix<double>? _principalComponents;
  

        public PCA(string filepath)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(filepath);
            _data = DelimitedReader.Read<double>(filePath: filepath, delimiter:",");
        }

        public PCA(Matrix<double> data) 
        {
            ArgumentNullException.ThrowIfNull(data);
            _data = data;
        }
        public Matrix<double> Data => _data;
        public Matrix<double>? ScaledData => _scaledData;
        public Evd<double>? EvdMatrix  => _evdMatrix;
        public MathNet.Numerics.LinearAlgebra.Vector<double>? ExplainedVariance 
                => _explainedVariance;
        public Matrix<double>? CovarianceMatrix => _covarianceMatrix;
        public Matrix<double>? PearsonCorrelationMatrix => _pearsonCorrelationMatrix;
        public Matrix<double>? PrincipalComponents => _principalComponents;
        public PCA Fit() 
        {
            _scaledData = Utils.ScaleAndReduce(_data);
            _covarianceMatrix = Utils.CovarianceMatrix(_scaledData);
            _pearsonCorrelationMatrix = Utils.CorrelationMatrix(_scaledData);
            _evdMatrix = _pearsonCorrelationMatrix.Evd();
            var eigSum = _evdMatrix.EigenValues.Sum().Real;
            _explainedVariance = ((_evdMatrix.EigenValues.Real() / eigSum ) * 100);
            _principalComponents = _scaledData.Multiply(_covarianceMatrix);
            return this;
        }
        
    }

}

