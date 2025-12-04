namespace InsureYouAI.Helpers
{
    public static class PolicyTypeMapper
    {
        public static readonly Dictionary<string, int> Map = new()
    {
        { "Trafik", 1 },
        { "Kasko", 2 },
        { "Sağlık", 3 },
        { "Konut", 4 },
        { "Egitim", 5 }
    };
    }

}
