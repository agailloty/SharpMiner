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
        private Matrix<double> _centeredAndScaledData;

        /// <summary>
        /// Initialize a specification for computation
        /// </summary>
        /// <param name="factorMethod"></param>
        /// <param name="numberOfComponents"></param>
        /// <param name="dataSet"></param>
        /// <param name="rowsWeights"></param>
        /// <param name="columnWeights"></param>
        /// <param name="centeredAndScale"></param>
        public Specs(FactorMethod factorMethod, int numberOfComponents, Matrix<double> dataSet, double[] rowsWeights = null, 
                        double[] columnWeights = null,
                        bool centeredAndScale = true) 
        {
            FactorMethod = factorMethod;
            DataSet = dataSet;
            RowsWeights = rowsWeights;
            ColumnsWeights = columnWeights;
            NumberOfComponents = numberOfComponents;
            IsCenteredAndScaled = centeredAndScale;
            if (centeredAndScale)
            {
                var centeredScaledMatrix = MatrixHelper.CenterAndScale(dataSet);
                _centeredAndScaledData = centeredScaledMatrix;
                WeighedMatrix = dataSet.MapIndexed((i, j, value) => value * Math.Sqrt(ColumnsWeights[j]) * Math.Sqrt(RowsWeights[i]));
            }
        }

        /// <summary>
        /// Specify which factor analysis method to use
        /// </summary>
        public FactorMethod FactorMethod { get; set; }
        /// <summary>
        /// Specify the dataset on which the factor analysis method is to be computed
        /// </summary>
        public Matrix<double> DataSet { get; set; }

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
        public Matrix<double> CenteredAndScaledData
        {
            get => _centeredAndScaledData;
        }
    }
}
