namespace RetirementSimulator.Core.DTOs
{
    public class AssetItemDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int StartYear { get; set; }

        public int EndYear { get; set; }

        public double InitialValue { get; set; }

        public double PercentageChangePerYear { get; set; }

        public double IncomePercentagePerYear { get; set; }

        public bool CanSellPartial { get; set; }
    }
}