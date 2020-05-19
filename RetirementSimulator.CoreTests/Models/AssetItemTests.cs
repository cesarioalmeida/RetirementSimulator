namespace RetirementSimulator.CoreTests.Models
{
    using System;

    using NUnit.Framework;

    using RetirementSimulator.Core.Models;

    [TestFixture]
    public class AssetItemTests
    {
        private Func<int, double, double, double, AssetItem> _assetItemFactory => (year, value, changePerYear, income) => new AssetItem
        {
            StartYear = year,
            EndYear = 2030,
            InitialValue = value,
            PercentageChangePerYear = changePerYear,
            IncomePercentagePerYear = income
        };

        [Test]
        public void NoDividendNoChange()
        {
            var target = this._assetItemFactory(2020, 100, 0, 0);

            this.AssertAmount(target, 0, 0, 0);
            this.AssertValue(target, 100, 100, 100);
        }

        [Test]
        public void NoDividendWithChange()
        {
            var target = this._assetItemFactory(2020, 100, 0.1, 0);

            this.AssertAmount(target, 0, 0, 0);
            this.AssertValue(target, 100, 110, 133.1);
        }

        [Test]
        public void DividendNoChange()
        {
            var target = this._assetItemFactory(2020, 100, 0, 0.1);

            this.AssertAmount(target, 10, 10, 10);
            this.AssertValue(target, 100, 100, 100);
        }

        [Test]
        public void DividendNoChange_EndYear()
        {
            var target = this._assetItemFactory(2020, 100, 0, 0.1);
            target.EndYear = 2023;

            this.AssertAmount(target, 10, 10, 110);
            this.AssertValue(target, 100, 100, 100);
        }

        [Test]
        public void DividendWithChange()
        {
            var target = this._assetItemFactory(2020, 100, -0.2, 0.1);
            target.EndYear = 2023;

            this.AssertAmount(target, 10, 8, 56.32);
            this.AssertValue(target, 100, 80, 51.2);
        }

        [Test]
        public void DividendNoChange_SellPartial()
        {
            var target = this._assetItemFactory(2020, 100, 0, 0.1);
            target.Sell(2022, 70);

            this.AssertAmount(target, 10, 10, 3);
            this.AssertValue(target, 100, 100, 30);
        }

        [Test]
        public void DividendChange_SellAll()
        {
            var target = this._assetItemFactory(2020, 100, 0, 0.1);
            target.SellAll(2022);

            this.AssertAmount(target, 10, 10, 0);
            this.AssertValue(target, 100, 100, 0);
        }

        [Test]
        public void DividendChange_Buy()
        {
            var target = this._assetItemFactory(2020, 100, 0, 0.1);
            target.Buy(2022, 25);

            this.AssertAmount(target, 10, 10, 12.5);
            this.AssertValue(target, 100, 100, 125);
        }

        private void AssertAmount(AssetItem target, double valueYear0, double valueYear1, double valueYear3)
        {
            Assert.That(target.GetAmount(2020), Is.EqualTo(valueYear0).Within(0.1).Percent);
            Assert.That(target.GetAmount(2021), Is.EqualTo(valueYear1).Within(0.1).Percent);
            Assert.That(target.GetAmount(2023), Is.EqualTo(valueYear3).Within(0.1).Percent);
        }

        private void AssertValue(AssetItem target, double valueYear0, double valueYear1, double valueYear3)
        {
            Assert.That(target.GetValue(2020), Is.EqualTo(valueYear0).Within(0.1).Percent);
            Assert.That(target.GetValue(2021), Is.EqualTo(valueYear1).Within(0.1).Percent);
            Assert.That(target.GetValue(2023), Is.EqualTo(valueYear3).Within(0.1).Percent);
        }
    }
}