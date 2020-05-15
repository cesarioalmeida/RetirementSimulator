namespace RetirementSimulator.Core.Models
{
    using RetirementSimulator.Core.DTOs;

    public class Settings
    {
        public Settings()
        {
            this.InflationRate = 0.02d;
        }

        public Settings(SettingsDTO dto)
        {
            this.Id = dto.Id;
            this.InflationRate = dto.InflationRate;
        }

        public int Id { get; set; }

        public double InflationRate { get; set; }

        public SettingsDTO GetDTO()
        {
            return new SettingsDTO
            {
                Id = this.Id,
                InflationRate = this.InflationRate
            };
        }
    }
}