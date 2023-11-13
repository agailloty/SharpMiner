using Microsoft.ML.Data;
using System.Globalization;

namespace SharpMiner.Test
{
    internal class OutputData
    {
        [ColumnName("PCAFeatures")]
        public float[] Features { get; set; }
        public string[] FeaturesString(char numberSeparator = '.')
        {
            if (Features is not null)
            {
                return Features.Select(feature => feature.ToString(new NumberFormatInfo { NumberDecimalSeparator = "." })).ToArray();
            }
            return Enumerable.Empty<string>().ToArray();
        }
    }
}
