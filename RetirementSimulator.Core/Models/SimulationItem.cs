namespace RetirementSimulator.Core.Models
{
    public class SimulationItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int StartYear { get; set; }

        public int EndYear { get; set; } = 3000;

        public double InitialValue { get; set; }

        public virtual double GetAmount(int year)
        {
            return this.InitialValue;
        }
    }
}