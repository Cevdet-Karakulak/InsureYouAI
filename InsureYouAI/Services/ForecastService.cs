using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;

namespace InsureYouAI.Services
{
    public class PolicySalesData
    {
        public DateTime Date { get; set; }
        public float SaleCount { get; set; }
    }
    public class PolicySalesForecast
    {
        public float[] ForecastedValues { get; set; }
        public float[] LowerBoundValues { get; set; }
        public float[] UpperBoundValues { get; set; }
    }
    public class ForecastService
    {
        private readonly MLContext _mlContext;
        public ForecastService()
        {
            _mlContext = new MLContext();
        }

        public PolicySalesForecast GetForecast(List<PolicySalesData> salesData, int horizon = 3)
        {

            int count = salesData.Count;

            var dataView = _mlContext.Data.LoadFromEnumerable(salesData);
            var forecastingPipeline = _mlContext.Forecasting.ForecastBySsa(
                outputColumnName: "ForecastedValues",
                inputColumnName: "SaleCount",
                windowSize: Math.Max(2, count / 4),          // toplamın 1/4'ü
                seriesLength: Math.Max(4, count / 2),        // toplamın yarısı
                trainSize: count - horizon,
                horizon: horizon,
                confidenceLevel: 0.95f,
                confidenceLowerBoundColumn: "LowerBoundValues",
                confidenceUpperBoundColumn: "UpperBoundValues"
                );

            var model = forecastingPipeline.Fit(dataView);
            var forecastingEngine = model.CreateTimeSeriesEngine<PolicySalesData, PolicySalesForecast>(_mlContext);

            return forecastingEngine.Predict();
        }

    }
}
