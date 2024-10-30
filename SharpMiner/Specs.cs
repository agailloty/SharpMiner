using System;

using MathNet.Numerics.LinearAlgebra;

using SharpMiner.Core;

namespace SharpMiner
{
    /// <summary>
    /// This class is used to parameterize a Computation
    /// </summary>
    public class Specs
    {
        private double[] _rowsWeights;
        private double[] _columnsWeights;
        private int _numberOfComponents;
        private DataSet _centeredAndScaledData;
        private DecompositionMethod? _decompositionMethod;

        public Specs(FactorMethod factorMethod, int numberOfComponents, DataSet dataSet, double[] rowsWeights = null, 
                        double[] columnWeights = null,
                        bool centeredAndScale = true, DecompositionMethod? decompositionMethod = null) 
        {
            FactorMethod = factorMethod;
            DataSet = dataSet;
            RowsWeights = rowsWeights;
            ColumnsWeights = columnWeights;
            NumberOfComponents = numberOfComponents;
            IsCenteredAndScaled = centeredAndScale;
            if (centeredAndScale)
            {
                var centeredScaledMatrix = MatrixHelper.CenterAndScale(DataSet.Data);
                _centeredAndScaledData = DataSet.LoadFromMatrix(centeredScaledMatrix);
                WeighedMatrix = DataSet.Data.MapIndexed((i, j, value) => value * Math.Sqrt(ColumnsWeights[j]) * Math.Sqrt(RowsWeights[i]));
            }
            _decompositionMethod = decompositionMethod;
            if (_decompositionMethod == null && factorMethod == FactorMethod.PCA)
            { 
                _decompositionMethod = Core.DecompositionMethod.Svd;
            }
        }

        /// <summary>
        /// Specify which factor analysis method to use
        /// </summary>
        public FactorMethod FactorMethod { get; set; }
        /// <summary>
        /// Specify the dataset on which the factor analysis method is to be computed
        /// </summary>
        public DataSet DataSet { get; set; }

        public Matrix<double> WeighedMatrix { get; }

        /// <summary>
        /// Specify the weights attributed to each row in the dataset
        /// </summary>
        public double[] RowsWeights
        {
            get => _rowsWeights;
            set
            {
                if (value == null)
                {
                    _rowsWeights = new double[DataSet.RowCount];
                    double defaultweight = 1.0 / DataSet.RowCount;
                    for (int i = 0; i < DataSet.RowCount; i++)
                    {
                        _rowsWeights[i] = defaultweight;
                    }
                }
                else if (value.Length != DataSet.RowCount)
                {
                    throw new ArgumentException("Rows weights should match dataset rows count");
                }
            }
        }
        /// <summary>
        /// Specify the weights attributed to each column in the dataset
        /// </summary>
        public double[] ColumnsWeights
        {
            get => _columnsWeights;

            set
            {
                if (value == null)
                {
                    _columnsWeights = new double[DataSet.ColumnCount];
                    for (int i = 0; i < DataSet.ColumnCount;i++)
                    {
                        _columnsWeights[i] = 1.0;
                    }
                }

                else if ( value.Length != DataSet.ColumnCount)
                {
                    throw new ArgumentException("Column weights should match dataset columns count");
                }
            }
        }

        /// <summary>
        /// Number of components to be kept after the analysis
        /// </summary>
        public int NumberOfComponents
        {
            get => _numberOfComponents;
            set
            {
                if (value > DataSet.ColumnCount)
                {
                    throw new ArgumentException("The number of components should not exceed the number of columns in the dataset.");
                }
                _numberOfComponents = value;
            }
        }
        /// <summary>
        /// Specify if the dataset must be centered and scaled before computation
        /// </summary>
        public bool IsCenteredAndScaled { get; set; }
        /// <summary>
        /// Compute centered and scaled data for computation
        /// </summary>
        public DataSet CenteredAndScaledData
        {
            get => _centeredAndScaledData;
        }

        public DecompositionMethod? DecompositionMethod => _decompositionMethod;
    }
}
