namespace RetirementSimulator.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using RetirementSimulator.Core.DTOs;

    public class AssetItem : SimulationItem
    {
        private readonly Func<int, double, BudgetItem> _incomeFactory =
            (year, initial) => new BudgetItem(0d)
            {
                StartYear = year,
                EndYear = year,
                InitialValue = initial,
                IsExpense = false,
                PercentageChangePerYear = 0d,
                IsAffectedByInflationRate = false
            };

        private readonly Func<int, double, BudgetItem> _expenseFactory =
            (year, initial) => new BudgetItem(0d)
            {
                StartYear = year,
                EndYear = year,
                InitialValue = initial,
                IsExpense = true,
                PercentageChangePerYear = 0d,
                IsAffectedByInflationRate = false
            };

        private readonly Dictionary<int, double> _valueDictionary = new Dictionary<int, double>();

        public AssetItem()
        {
            this.CanSellPartial = true;
            this.EndYear = 2300;
        }

        public AssetItem(AssetItemDTO dto)
        {
            this.Id = dto.Id;
            this.Name = dto.Name;
            this.StartYear = dto.StartYear;
            this.EndYear = dto.EndYear;
            this.InitialValue = dto.InitialValue;
            this.PercentageChangePerYear = dto.PercentageChangePerYear;
            this.IncomePercentagePerYear = dto.IncomePercentagePerYear;
            this.CanSellPartial = dto.CanSellPartial;
        }

        public double PercentageChangePerYear
        {
            get => this.GetValue<double>();
            set => this.SetValue(value, this.OnPercentageChangePerYearChanged);
        }

        public double IncomePercentagePerYear
        {
            get => this.GetValue<double>();
            set => this.SetValue(value);
        }

        public bool CanSellPartial
        {
            get => this.GetValue<bool>();
            set => this.SetValue(value);
        }

        public BudgetItem Buy(int year, double value)
        {
            this.CalculateValue(year, this._valueDictionary[year] + value);
            return this._expenseFactory(year, value);
        }

        public BudgetItem Sell(int year, double value)
        {
            if (this._valueDictionary[year] < value)
            {
                return this._incomeFactory(year, 0d);
            }

            this.CalculateValue(year, this._valueDictionary[year] - value);
            return this._incomeFactory(year, value);
        }

        public BudgetItem SellAll(int year)
        {
            this.EndYear = year;
            var result = this._valueDictionary[year];

            return this._incomeFactory(year, result);
        }

        public override double GetAmount(int year)
        {
            var result = 0d;

            if (year > this.EndYear || year < this.StartYear)
            {
                result = 0d;
                return result;
            }

            if (year == this.EndYear)
            {
                result = this._valueDictionary[year];
            }

            result += this._valueDictionary[year] * this.IncomePercentagePerYear;
            return result;
        }

        public double GetValue(int year)
        {
            if (year > this.EndYear || year < this.StartYear)
            {
                return 0d;
            }

            return this._valueDictionary[year];
        }

        public AssetItemDTO GetDTO()
        {
            return new AssetItemDTO
            {
                Id = this.Id,
                Name = this.Name,
                StartYear = this.StartYear,
                EndYear = this.EndYear,
                InitialValue = this.InitialValue,
                PercentageChangePerYear = this.PercentageChangePerYear,
                IncomePercentagePerYear = this.IncomePercentagePerYear,
                CanSellPartial = this.CanSellPartial
            };
        }

        protected override void OnStartYearChanged()
        {
            this.InitializeValueDictionary();
        }

        protected override void OnEndYearChanged()
        {
            this.InitializeValueDictionary();
        }

        protected override void OnInitialValueChanged()
        {
            this.InitializeValueDictionary();

            this.CalculateValue(this.StartYear, this.InitialValue);
        }

        private void OnPercentageChangePerYearChanged()
        {
            this.InitializeValueDictionary();

            this.CalculateValue(this.StartYear, this.InitialValue);
        }

        private void CalculateValue(int fromYear, double fromValue)
        {
            for (var year = fromYear; year <= this.EndYear; year++)
            {
                var numberOfYears = year - fromYear;
                this._valueDictionary[year] = fromValue * Math.Pow(1d + this.PercentageChangePerYear, numberOfYears);
            }
        }

        private void InitializeValueDictionary()
        {
            if (!this._valueDictionary.Any() && this.StartYear != 0 && this.EndYear > this.StartYear)
            {
                for (var year = this.StartYear; year <= this.EndYear; year++)
                {
                    this._valueDictionary.Add(year, 0d);
                }
            }
        }
    }
}