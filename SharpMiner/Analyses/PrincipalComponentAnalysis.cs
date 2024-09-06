using System;
using System.Collections.Generic;
using System.Text;

using MathNet.Numerics.LinearAlgebra;

using SharpMiner.Core;

namespace SharpMiner.Analyses
{
    /// <summary>
    /// This class computes the core PCA algorithm and provides additional statistics data for in depth analysis
    /// </summary>
    public class PrincipalComponentAnalysis
    {
        private CorePCA _corePCA;
        private long _numberComponents;
        private Matrix<double> _projections;
        private DataSet _dataset;
        /// <summary>
        /// Initialize an instance of the class with a <see cref="Specs"/> object.
        /// </summary>
        /// <param name="specs"></param>
        public PrincipalComponentAnalysis(Specs specs)
        {
            if (specs.IsCenteredAndScaled)
            {
                _dataset = specs.CenteredAndScaledData;
            }
            else
            {
                _dataset = specs.DataSet;
            }

            _numberComponents = specs.NumberOfComponents;
            _corePCA = new CorePCA(specs);
        }
    }
}
