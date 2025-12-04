using Microsoft.ML.Data;

namespace InsureYouAI.Models.ML
{
    public class PolicyForecastInput
    {
        [ColumnName("Ay")]
        public float Ay { get; set; }

        public float PolicyTypeId { get; set; }
    }
}
