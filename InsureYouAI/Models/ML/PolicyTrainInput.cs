using Microsoft.ML.Data;

namespace InsureYouAI.Models.ML
{
    public class PolicyTrainInput
    {
        [LoadColumn(0)]
        public float Ay { get; set; }

        [LoadColumn(1)]
        public float PolicyTypeId { get; set; }

        [ColumnName("Label")]
        [LoadColumn(2)]
        public float Satis { get; set; }
    }
}
