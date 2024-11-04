namespace SharpMiner.Core
{
    /// <summary>
    /// Represent a principal component. A PC is a linear combination of the original variables that maximally explain the variance of all the variables
    /// </summary>
    public class Component
    {
        /// <summary>
        /// Initialise a component object
        /// </summary>
        /// <param name="rank"></param>
        /// <param name="explainedVariance"></param>
        /// <param name="featureResults"></param>
        /// <param name="rowResults"></param>
        public Component(int rank, double explainedVariance, FactorResults featureResults, FactorResults rowResults)
        {
            Rank = rank;
            PercentExplainedVariance = explainedVariance;
            FeaturesResults = featureResults;
            RowsResults = rowResults;
        }
        /// <summary>
        /// The rank of the component by the amount of information explained.
        /// </summary>
        public int Rank { get; }
        /// <summary>
        /// The percentage of the total information of the dataset that this single component explains
        /// </summary>
        public double PercentExplainedVariance { get; }
        /// <summary>
        /// Represents statistics computed for the features of the dataset
        /// </summary>
        public FactorResults FeaturesResults { get; }
        /// <summary>
        /// Represents statistics computed for the rows of the dataset
        /// </summary>
        public FactorResults RowsResults { get; }
    }
}
