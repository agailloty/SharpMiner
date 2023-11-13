using Microsoft.Data.Analysis;
using Microsoft.ML;
using System.Globalization;
using Microsoft.ML.AutoML;

namespace SharpMiner.Test
{
    internal class MLNETExplore
    {
        public void Compute()
        {
            var mlContext = new MLContext();

            var customCulture = new CultureInfo("fr-FR");
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            CultureInfo.CurrentCulture = customCulture;
            var data = DataFrame.LoadCsv("indicators.csv", ',');


            var inference = mlContext.Auto().InferColumns("indicators.csv", labelColumnName: "GDP_CAPITA", separatorChar: ',');

            var loader = mlContext.Data.CreateTextLoader(inference.TextLoaderOptions);

            var indicators = loader.Load("indicators.csv");


            //var sdcaEstimator = mlContext.Regression.Trainers.Sdca("GDP_CAPITA");

            var pipeline = mlContext.Transforms.NormalizeMinMax("Features")
                .Append(mlContext.Transforms
                .ProjectToPrincipalComponents(outputColumnName: "PCAFeatures", inputColumnName: "Features"));

            var model = pipeline.Fit(indicators);
            var legitTransformedData = model.Transform(indicators);

            var predictions = mlContext.Data.CreateEnumerable<OutputData>(legitTransformedData, reuseRowObject: false);

            var outputs = predictions.Select(prediction => string.Join(',', prediction.Features)).ToArray();

            var res = new float[] { 1.5f, 3.6f };

            // Display first 5 results
            var predictionsArray = predictions.ToArray();
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("PCA Features: " + string.Join(", ", predictionsArray[i].Features));
            }

            // Build machine learning model
            //var trainedModel = sdcaEstimator.Fit(indicators);

            Console.WriteLine(data);

            Console.ReadLine();
        }
    }
}
