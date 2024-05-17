using System;
using System.Linq;

using ScottPlot;
using ScottPlot.Plottables;

namespace SharpMiner.Graphics
{
    public static class Visualisations
    {
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
                .Select(x => new Tick(x, x.ToString())).ToArray();

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
