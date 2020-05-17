namespace RetirementSimulator.Core.Models
{
    using System.Collections.Generic;
    using System.Linq;

    using DevExpress.Mvvm;

    using LiteDB;

    using RetirementSimulator.Core.DTOs;
    using RetirementSimulator.Core.Services;

    using StructureMap.Attributes;

    public class Simulation : BindableBase
    {
        private Dictionary<int, double> _valueDictionary = new Dictionary<int, double>();

        private Dictionary<int, double> _cashDictionary = new Dictionary<int, double>();

        public Simulation()
        {
            this.Id = ObjectId.NewObjectId();
            this.StartYear = 2020;
            this.EndYear = 2300;
        }

        public Simulation(SimulationDTO dto, IEnumerable<BudgetItem> budgetItems, IEnumerable<AssetItem> assetItems)
        {
            this.Id = dto.Id;
            this.StartYear = dto.StartYear;
            this.EndYear = dto.EndYear;

            if (budgetItems != null)
            {
                this.Items.AddRange(budgetItems);
            }

            if (assetItems != null)
            {
                this.Items.AddRange(assetItems);
            }
        }

        [SetterProperty]
        public PersistenceService PersistenceService { get; set; }

        public ObjectId Id { get; set; }

        public List<SimulationItem> Items { get; } = new List<SimulationItem>();

        public int StartYear { get; set; }

        public int EndYear { get; set; }

        public double FirstYearValue => this.GetTotalValue(this.StartYear);

        public void Run()
        {
            this._valueDictionary = new Dictionary<int, double>();
            this._cashDictionary = new Dictionary<int, double>();

            var income = this.Items.OfType<BudgetItem>().Where(x => !x.IsExpense).ToArray();
            var expenses = this.Items.OfType<BudgetItem>().Where(x => x.IsExpense).ToArray();

            var assets = this.Items.OfType<AssetItem>().ToList();

            var cash = 0d;
            for (var year = this.StartYear; year <= this.EndYear; year++)
            {
                if (!this._valueDictionary.ContainsKey(year))
                {
                    this._valueDictionary.Add(year, 0d);
                }

                var sumIncome = income.Sum(x => x.GetAmount(year));
                sumIncome += assets.Sum(x => x.GetAmount(year));

                var sumExpense = expenses.Sum(x => x.GetAmount(year));

                cash += sumIncome;
                cash -= sumExpense;

                // no more cash
                if (cash < 0)
                {
                    // sell assets
                    while (cash < 0 && assets.Any())
                    {
                        var assetToSell = assets.Last();
                        if (assetToSell.CanSellPartial && assetToSell.GetValue(year) > -cash)
                        {
                            cash += assetToSell.Sell(year, -cash).InitialValue;
                        }
                        else
                        {
                            cash += assetToSell.SellAll(year).InitialValue;
                            assets.Remove(assetToSell);
                        }
                    }

                    if (cash < 0)
                    {
                        cash = 0d;
                    }
                }

                this._cashDictionary[year] = cash;

                this._valueDictionary[year] = this._cashDictionary[year] + assets.Sum(x => x.GetValue(year));
            }
        }

        public double GetTotalValue(int year)
        {
            return this._valueDictionary.ContainsKey(year) ? this._valueDictionary[year] : 0d;
        }

        public double GetCash(int year)
        {
            return this._cashDictionary.ContainsKey(year) ? this._cashDictionary[year] : 0d;
        }

        public double GetAssets(int year)
        {
            return this._valueDictionary.ContainsKey(year) ? this._valueDictionary[year] - this._cashDictionary[year] : 0d;
        }

        public SimulationDTO GetDTO()
        {
            return new SimulationDTO
            {
                Id = this.Id,
                StartYear = this.StartYear,
                EndYear = this.EndYear,
                BudgetItemsIds = this.Items.OfType<BudgetItem>().Select(x => x.Id).ToList(),
                AssetItemsIds = this.Items.OfType<AssetItem>().Select(x => x.Id).ToList()
            };
        }
    }
}