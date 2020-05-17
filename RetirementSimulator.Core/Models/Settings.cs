namespace RetirementSimulator.Core.Models
{
    using LiteDB;

    using RetirementSimulator.Core.DTOs;

    public class Settings
    {
        public Settings()
        {
            this.Id = ObjectId.NewObjectId();
            this.InflationRate = 0.02d;
            this.AgeAtStartDate = 40;
        }

        public Settings(SettingsDTO dto)
        {
            this.Id = dto.Id;
            this.InflationRate = dto.InflationRate;
            this.AgeAtStartDate = dto.AgeAtStartDate;
        }

        public ObjectId Id { get; set; }

        public double InflationRate { get; set; }

        public int AgeAtStartDate { get; set; }

        public SettingsDTO GetDTO()
        {
            return new SettingsDTO
            {
                Id = this.Id,
                InflationRate = this.InflationRate,
                AgeAtStartDate = this.AgeAtStartDate
            };
        }
    }
}