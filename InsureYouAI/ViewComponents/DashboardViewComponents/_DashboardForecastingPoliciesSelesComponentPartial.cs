using InsureYouAI.Helpers;
using InsureYouAI.Models.ML;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.ML;

namespace InsureYouAI.ViewComponents.DashboardViewComponents
{
    public class _DashboardForecastingPoliciesSelesComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var ml = new MLContext();

            // 1) SQL'den veri çek
            string connStr = "Server=localhost\\MSSQLSERVER2022;Database=InsureDb;Integrated Security=True;TrustServerCertificate=True;";
            var dataList = new List<PolicyTrainInput>();

            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT 
                        MONTH(CreatedDate) AS Ay,
                        PolicyType,
                        COUNT(*) AS Satis
                    FROM Policies
                    GROUP BY MONTH(CreatedDate), PolicyType
                    ORDER BY Ay;
                ", conn);

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string type = reader["PolicyType"].ToString();

                    int typeId = type switch
                    {
                        "Trafik" => 1,
                        "Kasko" => 2,
                        "Sağlık" => 3,
                        "Konut" => 4,
                        "Egitim" => 5,
                        _ => 0
                    };

                    dataList.Add(new PolicyTrainInput
                    {
                        Ay = float.Parse(reader["Ay"].ToString()),
                        PolicyTypeId = typeId,
                        Satis = float.Parse(reader["Satis"].ToString())
                    });
                }
            }

            if (!dataList.Any())
            {
                return View("/Views/Shared/Components/_DashboardForecastingPoliciesSelesComponentPartial/Default.cshtml",
                    new List<(string Type, float Forecast)>());
            }

            // 2) Veri yükle
            var data = ml.Data.LoadFromEnumerable(dataList);

            // 3) Pipeline
            var pipeline = ml.Transforms
                .Concatenate("Features", nameof(PolicyTrainInput.Ay), nameof(PolicyTrainInput.PolicyTypeId))
                .Append(ml.Regression.Trainers.FastTree());

            // 4) Modeli eğit
            var model = pipeline.Fit(data);

            // 5) PredictionEngine
            var engine = ml.Model.CreatePredictionEngine<PolicyForecastInput, PolicyForecastOutput>(model);

            // 6) Aralık tahmini
            int targetMonth = 12;
            var types = new[] { "Trafik", "Kasko", "Sağlık", "Konut", "Egitim" };

            var results = new List<(string Type, float Forecast)>();

            foreach (var type in types)
            {
                var input = new PolicyForecastInput
                {
                    Ay = targetMonth, // DÜZELTİLMİŞ
                    PolicyTypeId = PolicyTypeMapper.Map[type]
                };

                var prediction = engine.Predict(input);

                results.Add((type, prediction.Score));
            }

            return View("/Views/Shared/Components/_DashboardForecastingPoliciesSelesComponentPartial/Default.cshtml", results);
        }
    }
}
