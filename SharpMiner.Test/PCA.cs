﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Data.Text;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics.Statistics;

namespace SharpMiner.Test
{
    public class PCA
    {
        private Matrix<double> _data;
        private Evd<double>? _evdMatrix;
        private MathNet.Numerics.LinearAlgebra.Vector<double>? _explainedVariance;
        private Matrix<double>? _principalComponents;
        private Matrix<double>? _covarianceMatrix;

        public PCA(string filepath)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(filepath);
            _data = DelimitedReader.Read<double>(filePath: filepath, delimiter:",");
        }

        public PCA(Matrix<double> data)
            => _data = data;

        public Evd<double>? EvdMatrix  => _evdMatrix;
        public MathNet.Numerics.LinearAlgebra.Vector<double>? ExplainedVariance 
                => _explainedVariance;
        public Matrix<double>? CovarianceMatrix => _covarianceMatrix;
        public Matrix<double>? PrincipalComponents => _principalComponents;
        public PCA Fit() {
            _evdMatrix = Utils.CorrelationMatrix(_data).Evd();
            var eigSum = _evdMatrix.EigenValues.Sum().Real;
            _explainedVariance = ((_evdMatrix.EigenValues.Real() / eigSum ) * 100);
            _covarianceMatrix = Utils.CovarianceMatrix(_data);
            _principalComponents = _data.Multiply(_covarianceMatrix);
            return this;
        }
        
    }

}

