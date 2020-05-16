namespace RetirementSimulator.Core.Models
{
    using DevExpress.Mvvm;

    public class SimulationItem : BindableBase
    {
        public int Id { get; set; }

        public string Name
        {
            get => this.GetValue<string>();
            set => this.SetValue(value);
        }

        public int StartYear
        {
            get => this.GetValue<int>();
            set => this.SetValue(value, this.OnStartYearChanged);
        }

        public int EndYear
        {
            get => this.GetValue<int>();
            set => this.SetValue(value, this.OnEndYearChanged);
        }

        public double InitialValue
        {
            get => this.GetValue<double>();
            set => this.SetValue(value, this.OnInitialValueChanged);
        }

        public virtual double GetAmount(int year)
        {
            return this.InitialValue;
        }

        protected virtual void OnStartYearChanged()
        {
        }

        protected virtual void OnEndYearChanged()
        {
        }

        protected virtual void OnInitialValueChanged()
        {
        }
    }
}