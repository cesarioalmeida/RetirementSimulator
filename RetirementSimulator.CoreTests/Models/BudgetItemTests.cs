namespace RetirementSimulator.CoreTests.Models
{
    using NUnit.Framework;

    using RetirementSimulator.Core.Models;

    [TestFixture]
    public class BudgetItemTests
    {
        [Test]
        public void GetValue_Income_NoInflationNoChange()
        {
            var income = new BudgetItem(0.2d)
            {
                StartYear = 2020,
                InitialValue = 10d,
                IsExpense = false,
                PercentageChangePerYear = 0d,
                IsAffectedByInflationRate = false
            };

            Assert.That(income.GetAmount(2021), Is.EqualTo(10d).Within(0.1).Percent);
            Assert.That(income.GetAmount(2023), Is.EqualTo(10d).Within(0.1).Percent);
        }

        [Test]
        public void GetValue_Income_NoInflationWithChangePerYear()
        {
            var income = new BudgetItem(0.2d)
            {
                StartYear = 2020,
                InitialValue = 10d,
                IsExpense = false,
                PercentageChangePerYear = 0.01d,
                IsAffectedByInflationRate = false
            };

            Assert.That(income.GetAmount(2021), Is.EqualTo(10.1).Within(0.1).Percent);
            Assert.That(income.GetAmount(2023), Is.EqualTo(10.303).Within(0.1).Percent);
        }

        [Test]
        public void GetValue_Income_WithInflationNoChangePerYear()
        {
            var income = new BudgetItem(0.02d)
            {
                StartYear = 2020,
                InitialValue = 10d,
                IsExpense = false,
                PercentageChangePerYear = 0d,
                IsAffectedByInflationRate = true
            };

            Assert.That(income.GetAmount(2021), Is.EqualTo(10.2).Within(0.1).Percent);
            Assert.That(income.GetAmount(2023), Is.EqualTo(10.612).Within(0.1).Percent);
        }

        [Test]
        public void GetValue_Income_WithInflationAndChangePerYear()
        {
            var income = new BudgetItem(0.02d)
            {
                StartYear = 2020,
                InitialValue = 10d,
                IsExpense = false,
                PercentageChangePerYear = 0.05d,
                IsAffectedByInflationRate = true
            };

            Assert.That(income.GetAmount(2021), Is.EqualTo(10.71).Within(0.1).Percent);
            Assert.That(income.GetAmount(2023), Is.EqualTo(12.2848).Within(0.1).Percent);
        }
    }
}