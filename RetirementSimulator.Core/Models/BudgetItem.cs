namespace RetirementSimulator.Core.Models
{
    using System;

    using RetirementSimulator.Core.DTOs;

    public class BudgetItem
    {
        private readonly double _inflationRate;

        public BudgetItem(double inflationRate)
        {
            this._inflationRate = inflationRate;
        }

        public BudgetItem(double inflationRate, BudgetItemDTO dto)
        {
            this.Id = dto.Id;
            this.IsExpense = dto.IsExpense;
            this.Name = dto.Name;
            this.StartDate = dto.StartDate;
            this.EndDate = dto.EndDate;
            this.InitialValue = dto.InitialValue;
            this.PercentageChangePerYear = dto.PercentageChangePerYear;
            this._inflationRate = inflationRate;
            this.IsAffectedByInflationRate = dto.IsAffectedByInflationRate;
        }

        public int Id { get; set; }

        public bool IsExpense { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public double InitialValue { get; set; }

        public double PercentageChangePerYear { get; set; }

        public bool IsAffectedByInflationRate { get; set; }

        public double GetValue(DateTime date)
        {
            if (date.Date > this.EndDate.Date || date.Date < this.StartDate.Date)
            {
                return 0d;
            }

            var numberOfYears = date.Date - this.StartDate.Date;

            return this.InitialValue
                   * Math.Pow(1d + this.PercentageChangePerYear, numberOfYears.TotalDays / 365d)
                   * (this.IsAffectedByInflationRate ? Math.Pow(1d + this._inflationRate, numberOfYears.TotalDays / 365d) : 1d);
        }

        public BudgetItemDTO GetDTO()
        {
            return new BudgetItemDTO
            {
                Id = this.Id,
                IsExpense = this.IsExpense,
                Name = this.Name,
                StartDate = this.StartDate,
                EndDate = this.EndDate,
                InitialValue = this.InitialValue,
                PercentageChangePerYear = this.PercentageChangePerYear,
                IsAffectedByInflationRate = this.IsAffectedByInflationRate
            };
        }
    }
}