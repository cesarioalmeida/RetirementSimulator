namespace RetirementSimulator.Core.DTOs
{
    using LiteDB;

    public class SettingsDTO
    {
        public ObjectId Id { get; set; }

        public double InflationRate { get; set; }

        public int AgeAtStartDate { get; set; }
    }
}