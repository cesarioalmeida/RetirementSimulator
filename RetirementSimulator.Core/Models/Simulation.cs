namespace RetirementSimulator.Core.Models
{
    using System.Collections.Generic;
    using System.Linq;

    public class Simulation
    {
        private Dictionary<int, double> _value = new Dictionary<int, double>();

        private Dictionary<int, double> _cash = new Dictionary<int, double>();

        public int Id { get; set; }

        public List<SimulationItem> Items { get; private set; } = new List<SimulationItem>();

        public int StartYear { get; set; }

        public int EndYear { get; set; }

        public void Run()
        {
            this._value = new Dictionary<int, double>();
            this._cash = new Dictionary<int, double>();

            var income = this.Items.OfType<BudgetItem>().Where(x => !x.IsExpense).ToArray();
            var expenses = this.Items.OfType<BudgetItem>().Where(x => x.IsExpense).ToArray();

            var cash = 0d;
            for (var year = this.StartYear; year <= this.EndYear; year++)
            {
                if (!this._value.ContainsKey(year))
                {
                    this._value.Add(year, 0d);
                }

                var sumIncome = income.Sum(x => x.GetAmount(year));
                var sumExpense = expenses.Sum(x => x.GetAmount(year));

                cash += sumIncome;
                cash -= sumExpense;
                
                this._cash[year] = cash;

                this._value[year] = this._cash[year];
            }
        }

        public double GetTotalValue(int year)
        {
            return this._value.ContainsKey(year) ? this._value[year] : 0d;
        }

        public double GetCash(int year)
        {
            return this._cash.ContainsKey(year) ? this._cash[year] : 0d;
        }
    }
}