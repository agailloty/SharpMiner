using System;
using System.Linq;

using MathNet.Numerics.LinearAlgebra;

using ScottPlot;

using SkiaSharp;

namespace SharpMiner.Graphics
{
    public static class Visualisations
    {
        public static Plot ScreePlot(double[] components)
        {
            var graph = new Plot();
            graph.Add.Bars(components);

            var ticks = Enumerable.Range(0, components.Length).Select(x => new Tick(x, $"Comp {x+1}"))
                .ToArray();

            graph.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(ticks);

            graph.Title("PCA - Screeplot", 18);
            graph.XLabel("Components");

            return graph;
        }

        public static Plot CorrelationCircle(Matrix<double> corrMatrix)
        {
            var plt = new Plot();

            // Add unit circle
            plt.Add.Circle(0, 0, 1);

            // Add correlation vectors
            for (int i = 0; i < corrMatrix.RowCount; i++)
            {
                for (int j = 0; j < corrMatrix.ColumnCount; j++)
                {
                    if (i != j)
                    {

                        var coordX = new Coordinates(0, Math.Cos(j * Math.PI / 180) * corrMatrix[i, j]);
                        var coordY = new Coordinates(0, Math.Sin(j * Math.PI / 180) * corrMatrix[i, j]);

                        var coordinate = new CoordinateLine(coordX, coordY);
                        plt.Add.Arrow(coordinate);
                    }
                }
            }

            plt.Title("Correlation Circle");

            return plt;
        }
    }
}
