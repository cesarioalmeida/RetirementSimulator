namespace RetirementSimulator.Models
{
    public class ChartDataItem
    {
        public ChartDataItem(int year, int age, double totalValue, double cash, double assets, double income, double expenses)
        {
            this.Year = year;
            this.Age = age;
            this.TotalValue = totalValue;
            this.Cash = cash;
            this.Assets = assets;
            this.Income = income;
            this.Expenses = expenses;
        }

        public int Year { get; set; }

        public int Age { get; set; }

        public double TotalValue { get; set; }

        public double Cash { get; set; }

        public double Assets { get; set; }

        public double Income { get; set; }

        public double Expenses { get; set; }
    }
}