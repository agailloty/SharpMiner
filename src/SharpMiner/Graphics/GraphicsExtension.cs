using ScottPlot;

using System.Linq;

namespace SharpMiner.Graphics
{
    /// <summary>
    /// This class contains methods to draw the results of the factor analysis on various types of charts
    /// </summary>
    public static class GraphicsExtension
    {
        /// <summary>
        /// Represent the scree plot of a PCA explained variance
        /// </summary>
        /// <param name="prcomp"></param>
        /// <returns></returns>
        public static Plot ScreePlot(this PrincipalComponentAnalysis prcomp)
        {
            return Visualisations.ScreePlot(prcomp.ExplainedVariance);
        }

        public static Plot CorrelationCircle(this PrincipalComponentAnalysis prcomp, (sbyte, sbyte)? dimensions = null)
        {
            var coord1 = prcomp.ColumnsResults.Coordinates.Column(dimensions?.Item1 ?? 1).AsArray();
            var coord2 = prcomp.ColumnsResults.Coordinates.Column(dimensions?.Item2 ?? 2).AsArray();
            var coords = coord1.Zip(coord2, (x, y) => (x, y)).ToArray();
            return Visualisations.CorrelationCircle(coords, dimensions);
        }
    }
}
