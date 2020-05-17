namespace RetirementSimulator.Core.DTOs
{
    using LiteDB;

    public class BudgetItemDTO
    {
        public ObjectId Id { get; set; }

        public bool IsExpense { get; set; }

        public string Name { get; set; }

        public int StartYear { get; set; }

        public int EndYear { get; set; }

        public double InitialValue { get; set; }

        public double PercentageChangePerYear { get; set; }

        public bool IsAffectedByInflationRate { get; set; }
    }
}