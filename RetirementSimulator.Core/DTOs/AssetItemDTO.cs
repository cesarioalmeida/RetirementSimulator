namespace RetirementSimulator.Core.DTOs
{
    using LiteDB;

    public class AssetItemDTO
    {
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public int StartYear { get; set; }

        public int EndYear { get; set; }

        public double InitialValue { get; set; }

        public double PercentageChangePerYear { get; set; }

        public double IncomePercentagePerYear { get; set; }

        public bool CanSellPartial { get; set; }
    }
}