namespace InsureYouAI.Models
{
    public class AIInsuranceRecommendationViewModel
    {  // Kullanıcıdan alınacak alanlar
        public int? Age { get; set; }
        public string? Occupation { get; set; }
        public string? City { get; set; }
        public string? MaritalStatus { get; set; }
        public int? ChildrenCount { get; set; }
        public string? TravelFrequency { get; set; }   
        public decimal? MonthlyBudget { get; set; }      
        public bool HasChronicDisease { get; set; }
        public string? ChronicDiseaseDetails { get; set; }
        public string? CoveragePriority { get; set; }  
        public string? RecommendedPackage { get; set; }
        public string? SecondBestPackage { get; set; }
        public string? AnalysisText { get; set; }

        public string? onerilenPaket { get; set; }
        public string? ikinciSecenek { get; set; }
        public string? neden { get; set; }
    }
}