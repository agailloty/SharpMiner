using MathNet.Numerics.LinearAlgebra;

namespace SharpMiner
{
    /// <summary>
    /// This class represents a summary of the factor analysis.
    /// </summary>
    public class FactorResults
    {
        /// <summary>
        /// Initialise the factor results class 
        /// </summary>
        /// <param name="explainedVariance"></param>
        /// <param name="cumulativeExplainedVariance"></param>
        /// <param name="coordinates"></param>
        /// <param name="squaredCosinus"></param>
        /// <param name="contributions"></param>
        internal FactorResults(Vector<double> explainedVariance,
                                Vector<double> cumulativeExplainedVariance,
                                Matrix<double> coordinates,
                                Matrix<double> squaredCosinus,
                                Matrix<double> contributions)
        {
            Coordinates = coordinates;
            SquaredCosinus = squaredCosinus;
            Contributions = contributions;
            ExplainedVariance = explainedVariance;
            CumulativeExplainedVariance = cumulativeExplainedVariance;
        }
        /// <summary>
        /// Represents the coordinates of a Principal Component Analysis (PCA).
        /// This field can hold either the scores or loadings of the PCA, depending on the context of its use.
        /// </summary>
        public Matrix<double> Coordinates { get; }
        /// <summary>
        /// Represents the squared cosines (SquaredCosinus) of the observations or variables in a Principal Component Analysis (PCA).
        /// This field provides a measure of the quality of the representation of each observation or variable in the 
        /// reduced PCA space. Each value indicates the proportion of the total variance of an observation or variable
        /// that is explained by the selected principal components, effectively quantifying how well it is represented 
        /// in the PCA dimensions.
        /// </summary>
        public Matrix<double> SquaredCosinus { get; }
        /// <summary>
        /// Represents the contributions of observations or variables to the principal components in a Principal Component Analysis (PCA).
        /// This field quantifies how much each observation or variable contributes to the formation of each principal component,
        /// helping to identify which elements are most influential in defining the axes of the PCA space.
        /// </summary>
        /// <remarks>
        /// The contributions are often expressed as a percentage and organized in a matrix where rows correspond to 
        /// observations or variables, and columns represent the principal components. Key points to note:
        /// - A high contribution value for an observation or variable in a particular principal component suggests that it plays
        ///   a significant role in defining that component.
        /// - Low contribution values indicate that the observation or variable has a lesser influence on the component's definition.
        ///
        /// Contribution values can help in understanding the structure of the PCA and identifying which variables or observations
        /// drive the main patterns and directions in the data.
        /// </remarks>
        public Matrix<double> Contributions { get; }
        /// <summary>
        /// Get the percentage of explained variance by each components
        /// </summary>

        public Vector<double> ExplainedVariance { get; }
        /// <summary>
        /// Get the cumulative sum of explained variance
        /// </summary>
        public Vector<double> CumulativeExplainedVariance { get; }
    }
}
