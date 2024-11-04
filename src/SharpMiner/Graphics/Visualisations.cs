using System;
using System.Linq;

using ScottPlot;

namespace SharpMiner.Graphics
{
    /// <summary>
    /// This class contains methods to draw the results of the factor analysis on various types of charts
    /// </summary>
    public static class Visualisations
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="explainedVariances"></param>
        /// <returns></returns>
        public static Plot ScreePlot(double[] explainedVariances)
        {
            explainedVariances = explainedVariances.OrderByDescending(x => x).ToArray();

            double maxVariance = explainedVariances[0];
            double minVariance = explainedVariances[explainedVariances.Length - 1];

            var graph = new Plot();

            var screeplot = graph.Add.Bars(explainedVariances);

            Tick[] xTicks = Enumerable.Range(0, explainedVariances.Length)
                .Select(x => new Tick(x, $"Comp {x+1}")).ToArray();

            Tick[] yTicks = Generate.Consecutive(5, maxVariance / 5, minVariance)
                .Select(x => new Tick(x, Math.Round(x).ToString())).ToArray();

            graph.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(xTicks);
            graph.Axes.Left.TickGenerator = new ScottPlot.TickGenerators.NumericManual(yTicks);

            double[] explainedVarianceCumSum = CumulativeSum(explainedVariances);

            //screeplot.Bars.ToList().Zip(explainedVarianceCumSum, (bar, value) => bar.Label = value.ToString());

            explainedVarianceCumSum.Zip(screeplot.Bars, (value, bar) => bar.Label = (value/100).ToString("P")).ToArray();

            graph.Title("PCA - Screeplot", 30);
            graph.XLabel("Components");
            graph.YLabel("% of explained variance");

            screeplot.ValueLabelStyle.Bold = true;
            screeplot.ValueLabelStyle.FontSize = 18;

            double[] CumulativeSum(double[] variances)
            {
                var cumulativeSum = variances
                    .Select((value, index) => new { Value = value, Index = index })
                                .Aggregate(
                        new { Sum = 0.0, Results = new double[variances.Length] },
                        (acc, current) => {
                            var newSum = acc.Sum + current.Value;
                            acc.Results[current.Index] = newSum;
                            return new { Sum = newSum, Results = acc.Results };
                        },
                        acc => acc.Results
                    );

                return cumulativeSum;
            }

            return graph;
        }
        /// <summary>
        /// Represents the correlation circle in a Principal Component Analysis (PCA), which visualizes the correlations 
        /// between the original variables and the principal components.
        /// This field contains data that helps illustrate how each variable is associated with the components, revealing 
        /// patterns and relationships within the data in the context of the reduced PCA dimensions.
        /// </summary>
        /// <remarks>
        /// The correlation circle is typically a graphical representation where:
        /// - Each variable is displayed as a vector, with the coordinates on each axis representing its correlation 
        ///   with the respective principal component.
        /// - The length and direction of each vector indicate both the strength and nature of the correlation, where 
        ///   longer vectors imply stronger correlations.
        /// - Variables with vectors pointing in similar directions are positively correlated, while those in opposite 
        ///   directions are negatively correlated. Vectors closer to the origin suggest weaker correlations.
        ///
        /// The data in this field is usually represented in matrix form, where rows correspond to variables and columns 
        /// to the principal components. This visualization is valuable for interpreting the roles of variables in the 
        /// principal component space and understanding underlying data structures.
        /// </remarks>
        public static Plot CorrelationCircle((double, double)[] correlations, (sbyte, sbyte)? dimensions = null)
        {
            var plt = new Plot();

            var ticks = new double[] {-1, -0.5, 0, 0.5, 1}.Select(x => new Tick(x, x.ToString()))
                .ToArray();

            plt.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(ticks);
            plt.Axes.Left.TickGenerator = new ScottPlot.TickGenerators.NumericManual(ticks);

            // Add unit circle
            plt.Add.Circle(0, 0, 1);

            // Add correlation vectors
            foreach(var (lower, upper) in correlations)
            {
                var coordX = new Coordinates(0, 0);
                var coordY = new Coordinates(lower, upper);

                var coordinate = new CoordinateLine(coordX, coordY);
                var arrow = plt.Add.Arrow(coordinate);
                arrow.ArrowShape = ArrowShape.SingleLine.GetShape();
                //arrow.ArrowheadLength = 50;
                arrow.ArrowheadWidth = 7f;
                arrow.ArrowheadAxisLength = 0.25f;
                arrow.ArrowheadLength = 7;
            }
            sbyte xAxis, yAxis;
            if (dimensions is null)
            {
                (xAxis, yAxis) = (1, 2);
            } else 
                (xAxis, yAxis) = dimensions.Value;


            plt.XLabel($"Component {xAxis}", size: 20);
            plt.YLabel($"Component {yAxis}", size: 20);

            plt.Title("Correlation Circle", size: 30);

            return plt;
        }
    }
}
