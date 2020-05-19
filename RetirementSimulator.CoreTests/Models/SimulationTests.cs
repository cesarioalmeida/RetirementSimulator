namespace RetirementSimulator.CoreTests.Models
{
    using System;

    using NUnit.Framework;

    using RetirementSimulator.Core.Models;

    [TestFixture]
    public class SimulationTests
    {
        private readonly Func<double, double, bool, BudgetItem> _incomeItemFactory =
            (initial, percentChange, inflation) => new BudgetItem(0.02d)
            {
                StartYear = 2020,
                EndYear = 2300,
                InitialValue = initial,
                IsExpense = false,
                PercentageChangePerYear = percentChange,
                IsAffectedByInflationRate = inflation
            };

        private readonly Func<int, double, double, bool, BudgetItem> _expenseItemFactory =
            (year, initial, percentChange, inflation) => new BudgetItem(0.02d)
            {
                StartYear = year,
                EndYear = 2300,
                InitialValue = initial,
                IsExpense = true,
                PercentageChangePerYear = percentChange,
                IsAffectedByInflationRate = inflation
            };

        private readonly Func<int, double, BudgetItem> _cashItemFactory =
            (year, initial) => new BudgetItem(0.02d)
            {
                StartYear = year,
                EndYear = year,
                InitialValue = initial,
                IsExpense = false,
                PercentageChangePerYear = 0d,
                IsAffectedByInflationRate = false
            };

        private readonly Func<double, double, double, AssetItem> _assetItemFactory =
            (initial, percentChange, income) => new AssetItem
            {
                StartYear = 2020,
                EndYear = 2300,
                InitialValue = initial,
                PercentageChangePerYear = percentChange,
                IncomePercentagePerYear = income
            };

        private Simulation _simulation;

        [SetUp]
        public void Initialize()
        {
            this._simulation = new Simulation
            {
                StartYear = 2020,
                EndYear = 2030
            };
        }

        [Test]
        public void GetValue_NoItems_ReturnsZero()
        {
            this._simulation.Run();

            this.AssertTotalValue(0, 0, 0);
            this.AssertCash(0, 0, 0);
        }

        [Test]
        public void GetValue_OneIncome_ConstValue()
        {
            this._simulation.Items.Add(this._incomeItemFactory(10d, 0d, false));
            this._simulation.Run();

            this.AssertTotalValue(10, 20, 40);
            this.AssertCash(10, 20, 40);
        }

        [Test]
        public void GetValue_TwoIncome_ConstValue()
        {
            this._simulation.Items.Add(this._incomeItemFactory(10d, 0d, false));
            this._simulation.Items.Add(this._incomeItemFactory(40d, 0d, false));

            this._simulation.Run();

            this.AssertTotalValue(50, 100, 200);
            this.AssertCash(50, 100, 200);
        }

        [Test]
        public void GetValue_TwoIncome_ConstValue_OneExpiresEarlier()
        {
            var earlyExpiry = this._incomeItemFactory(10d, 0d, false);
            earlyExpiry.EndYear = 2022;

            this._simulation.Items.Add(earlyExpiry);
            this._simulation.Items.Add(this._incomeItemFactory(40d, 0d, false));

            this._simulation.Run();

            this.AssertTotalValue(50, 100, 190);
            this.AssertCash(50, 100, 190);
        }

        [Test]
        public void GetValue_OneIncome_WithPercentageChange()
        {
            this._simulation.Items.Add(this._incomeItemFactory(10d, 0.05d, false));
            this._simulation.Run();

            this.AssertTotalValue(10, 20.5, 43.125);
            this.AssertCash(10, 20.5, 43.125);
        }

        [Test]
        public void GetValue_TwoIncome_InflationAndPercentageChange()
        {
            this._simulation.Items.Add(this._incomeItemFactory(10d, 0.01d, true));
            this._simulation.Items.Add(this._incomeItemFactory(40d, 0.04d, true));

            this._simulation.Run();

            this.AssertTotalValue(50, 102.734, 217.041);
            this.AssertCash(50, 102.734, 217.041);
        }

        [Test]
        public void GetValue_OneCashItem_ConstValue()
        {
            this._simulation.Items.Add(this._cashItemFactory(2020, 10d));
            this._simulation.Run();

            this.AssertTotalValue(10, 10, 10);
            this.AssertCash(10, 10, 10);
        }

        [Test]
        public void GetValue_TwoCashItem_ConstValue()
        {
            this._simulation.Items.Add(this._cashItemFactory(2020, 10d));
            this._simulation.Items.Add(this._cashItemFactory(2022, 30d));
            this._simulation.Run();

            this.AssertTotalValue(10, 10, 40);
            this.AssertCash(10, 10, 40);
        }

        [Test]
        public void GetValue_OneCashOneIncome_WithInflation()
        {
            this._simulation.Items.Add(this._cashItemFactory(2022, 10d));
            this._simulation.Items.Add(this._incomeItemFactory(10d, 0.01d, true));

            this._simulation.Run();

            this.AssertTotalValue(10, 20.302, 51.848);
            this.AssertCash(10, 20.302, 51.848);
        }

        [Test]
        public void GetValue_OneCashOneExpense_ConstExpense()
        {
            this._simulation.Items.Add(this._cashItemFactory(2020, 100d));
            this._simulation.Items.Add(this._expenseItemFactory(2020, 10d, 0d, false));

            this._simulation.Run();

            this.AssertTotalValue(90, 80, 60);
            this.AssertCash(90, 80, 60);
        }

        [Test]
        public void GetValue_OneCashOneExpense_ExpenseLargerThanCash()
        {
            this._simulation.Items.Add(this._cashItemFactory(2020, 100d));
            this._simulation.Items.Add(this._expenseItemFactory(2020, 40d, 0d, false));

            this._simulation.Run();

            this.AssertTotalValue(60, 20, 0);
            this.AssertCash(60, 20, 0);
        }

        [Test]
        public void OneAsset_WithDividend()
        {
            this._simulation.Items.Add(this._assetItemFactory(10d, 0d, 0.1d));
            this._simulation.Run();

            this.AssertTotalValue(11, 12, 14);
            this.AssertCash(1, 2, 4);
        }

        [Test]
        public void RunOutOfCash_SellAsset()
        {
            this._simulation.Items.Add(this._cashItemFactory(2020, 100d));
            this._simulation.Items.Add(this._expenseItemFactory(2021, 80d, 0d, false));
            this._simulation.Items.Add(this._assetItemFactory(100d, 0d, 0.1d));
            this._simulation.Run();

            this.AssertTotalValue(210, 140, 0);
            this.AssertCash(110, 40, 0);
        }

        [Test]
        public void TwoAssets_RunOutOfCash_SoldAll()
        {
            this._simulation.Items.Add(this._cashItemFactory(2020, 10d));
            this._simulation.Items.Add(this._expenseItemFactory(2021, 80d, 0d, false));
            this._simulation.Items.Add(this._assetItemFactory(100d, 0d, 0.1d));
            this._simulation.Items.Add(this._assetItemFactory(50d, 0d, 0.1d));
            this._simulation.Run();

            this.AssertTotalValue(175, 110, 0);
            this.AssertCash(25, 0, 0);
        }

        [Test]
        public void AssetCannotSellPartial_RunOutOfCash_SellAsset()
        {
            this._simulation.Items.Add(this._cashItemFactory(2020, 100d));
            this._simulation.Items.Add(this._expenseItemFactory(2020, 80d, 0d, false));
            var assetItem = this._assetItemFactory(100d, 0d, 0.1d);
            assetItem.CanSellPartial = false;
            this._simulation.Items.Add(assetItem);
            this._simulation.Run();

            this.AssertTotalValue(130, 60, 0);
            this.AssertCash(30, 60, 0);
        }

        private void AssertTotalValue(double valueYear0, double valueYear1, double valueYear3)
        {
            Assert.That(this._simulation.GetTotalValue(2020), Is.EqualTo(valueYear0).Within(0.1).Percent);
            Assert.That(this._simulation.GetTotalValue(2021), Is.EqualTo(valueYear1).Within(0.1).Percent);
            Assert.That(this._simulation.GetTotalValue(2023), Is.EqualTo(valueYear3).Within(0.1).Percent);
        }

        private void AssertCash(double cashYear0, double cashYear1, double cashYear3)
        {
            Assert.That(this._simulation.GetCash(2020), Is.EqualTo(cashYear0).Within(0.1).Percent);
            Assert.That(this._simulation.GetCash(2021), Is.EqualTo(cashYear1).Within(0.1).Percent);
            Assert.That(this._simulation.GetCash(2023), Is.EqualTo(cashYear3).Within(0.1).Percent);
        }
    }
}