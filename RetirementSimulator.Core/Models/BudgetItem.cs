namespace RetirementSimulator.Core.Models
{
    using System;

    using DevExpress.Mvvm.DataAnnotations;

    using RetirementSimulator.Core.DTOs;

    [POCOViewModel]
    public class BudgetItem : SimulationItem
    {
        private readonly double _inflationRate;

        public BudgetItem(double inflationRate)
        {
            this._inflationRate = inflationRate;
            this.EndYear = 2300;
        }

        public BudgetItem(double inflationRate, BudgetItemDTO dto)
        {
            this.Id = dto.Id;
            this.IsExpense = dto.IsExpense;
            this.Name = dto.Name;
            this.StartYear = dto.StartYear;
            this.EndYear = dto.EndYear;
            this.InitialValue = dto.InitialValue;
            this.PercentageChangePerYear = dto.PercentageChangePerYear;
            this._inflationRate = inflationRate;
            this.IsAffectedByInflationRate = dto.IsAffectedByInflationRate;
        }

        public bool IsExpense
        {
            get => this.GetValue<bool>();
            set => this.SetValue(value);
        }

        public double PercentageChangePerYear
        {
            get => this.GetValue<double>();
            set => this.SetValue(value);
        }

        public bool IsAffectedByInflationRate
        {
            get => this.GetValue<bool>();
            set => this.SetValue(value);
        }

        public override double GetAmount(int year)
        {
            if (year > this.EndYear || year < this.StartYear)
            {
                return 0d;
            }

            var numberOfYears = year - this.StartYear;

            return this.InitialValue
                   * Math.Pow(1d + this.PercentageChangePerYear, numberOfYears)
                   * (this.IsAffectedByInflationRate ? Math.Pow(1d + this._inflationRate, numberOfYears) : 1d);
        }

        public BudgetItemDTO GetDTO()
        {
            return new BudgetItemDTO
            {
                Id = this.Id,
                IsExpense = this.IsExpense,
                Name = this.Name,
                StartYear = this.StartYear,
                EndYear = this.EndYear,
                InitialValue = this.InitialValue,
                PercentageChangePerYear = this.PercentageChangePerYear,
                IsAffectedByInflationRate = this.IsAffectedByInflationRate
            };
        }
    }
}