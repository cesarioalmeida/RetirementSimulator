namespace RetirementSimulator.ViewModels
{
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    using DevExpress.Mvvm;
    using DevExpress.Mvvm.DataAnnotations;
    using DevExpress.Mvvm.POCO;
    using DevExpress.Xpf.Grid;
    using DevExpress.Xpf.WindowsUI;

    using RetirementSimulator.Core.Models;
    using RetirementSimulator.Core.Services;
    using RetirementSimulator.Models;
    using RetirementSimulator.Views;

    using StructureMap.Attributes;

    [POCOViewModel]
    public class HomeViewModel
    {
        [SetterProperty]
        public PersistenceService PersistenceService { get; set; }

        public virtual bool IsBusy { get; set; }

        public virtual Simulation Simulation { get; set; }

        public virtual AssetItem SelectedAsset { get; set; }

        public virtual BudgetItem SelectedIncome { get; set; }

        public virtual BudgetItem SelectedExpense { get; set; }

        public virtual List<Column> ResultColumns { get; set; }

        public virtual List<ExpandoObject> ResultRows { get; set; }

        public virtual List<ChartDataItem> ChartData { get; set; }

        public virtual string ChartArgument { get; set; } = "Year";

        protected IDocumentManagerService DocumentManagerService => this.GetService<IDocumentManagerService>();

        public async void Loaded()
        {
            this.IsBusy = true;

            try
            {
                this.Simulation = await Task.Run(() => this.PersistenceService.GetSimulation());
                if (this.Simulation == null)
                {
                    this.Simulation = new Simulation();

                    await Task.Run(() => this.PersistenceService.SaveSimulation(this.Simulation));
                }

                await Task.Run(() => this.Simulation.Run());

                await Task.Run(this.PrepareResults);

                await Task.Run(this.PrepareChartData);
            }
            finally
            {
                this.RaisePropertyChanged(x => x.Simulation);
                this.RaisePropertyChanged(x => x.ResultColumns);
                this.RaisePropertyChanged(x => x.ResultRows);
                this.RaisePropertyChanged(x => x.ChartData);
                this.IsBusy = false;
            }
        }

        public async void NewSimulation()
        {
            if (WinUIMessageBox.Show(
                    "Creating a new simulation will erase all the assets, income and expenses. Do you want to continue?",
                    "new simulation",
                    MessageBoxButton.OKCancel) != MessageBoxResult.OK)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                await Task.Run(() => this.PersistenceService.DeleteSimulation(this.Simulation));

                this.Simulation = new Simulation();
                await Task.Run(() => this.PersistenceService.SaveSimulation(this.Simulation));
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        public void ShowSettings()
        {
            var doc = this.DocumentManagerService.CreateDocument(nameof(SettingsView), null, this);
            doc.Show();

            this.Loaded();
        }

        public async void AddAsset()
        {
            var asset = new AssetItem();
            var doc = this.DocumentManagerService.CreateDocument(nameof(AssetView), asset, this);
            doc.Show();

            var vm = doc.Content as AssetViewModel;
            if (vm?.IsOK ?? false)
            {
                this.Simulation.Items.Add(vm.Asset);
                await Task.Run(() => this.PersistenceService.SaveSimulation(this.Simulation));
            }

            this.Loaded();
        }

        public void EditAsset()
        {
            var doc = this.DocumentManagerService.CreateDocument(nameof(AssetView), this.SelectedAsset, this);
            doc.Show();

            this.Loaded();
        }

        public bool CanEditAsset()
        {
            return this.SelectedAsset != null;
        }

        public async void DeleteAsset()
        {
            if (WinUIMessageBox.Show(
                    $"Are you sure you want to delete the asset {this.SelectedAsset.Name}?",
                    "delete asset",
                    MessageBoxButton.OKCancel) != MessageBoxResult.OK)
            {
                return;
            }

            this.Simulation.Items.Remove(this.SelectedAsset);
            await Task.Run(() => this.PersistenceService.DeleteAssetItem(this.SelectedAsset));

            this.Loaded();
        }

        public bool CanDeleteAsset()
        {
            return this.SelectedAsset != null;
        }

        public async void AddIncome()
        {
            var settings = await Task.Run(() => this.PersistenceService.GetSettings());

            var income = new BudgetItem(settings.InflationRate);
            var doc = this.DocumentManagerService.CreateDocument(nameof(IncomeItemView), income, this);
            doc.Show();

            var vm = doc.Content as IncomeItemViewModel;
            if (vm?.IsOK ?? false)
            {
                this.Simulation.Items.Add(vm.IncomeItem);
                await Task.Run(() => this.PersistenceService.SaveSimulation(this.Simulation));
            }

            this.Loaded();
        }

        public void EditIncome()
        {
            var doc = this.DocumentManagerService.CreateDocument(nameof(IncomeItemView), this.SelectedIncome, this);
            doc.Show();

            this.Loaded();
        }

        public bool CanEditIncome()
        {
            return this.SelectedIncome != null;
        }

        public async void DeleteIncome()
        {
            if (WinUIMessageBox.Show(
                    $"Are you sure you want to delete the income {this.SelectedIncome.Name}?",
                    "delete income",
                    MessageBoxButton.OKCancel) != MessageBoxResult.OK)
            {
                return;
            }

            this.Simulation.Items.Remove(this.SelectedIncome);
            await Task.Run(() => this.PersistenceService.DeleteBudgetItem(this.SelectedIncome));

            this.Loaded();
        }

        public bool CanDeleteIncome()
        {
            return this.SelectedIncome != null;
        }

        public async void AddExpense()
        {
            var settings = await Task.Run(() => this.PersistenceService.GetSettings());

            var expense = new BudgetItem(settings.InflationRate);
            var doc = this.DocumentManagerService.CreateDocument(nameof(ExpenseItemView), expense, this);
            doc.Show();

            var vm = doc.Content as ExpenseItemViewModel;
            if (vm?.IsOK ?? false)
            {
                this.Simulation.Items.Add(vm.ExpenseItem);
                await Task.Run(() => this.PersistenceService.SaveSimulation(this.Simulation));
            }

            this.Loaded();
        }

        public void EditExpense()
        {
            var doc = this.DocumentManagerService.CreateDocument(nameof(ExpenseItemView), this.SelectedExpense, this);
            doc.Show();

            this.Loaded();
        }

        public bool CanEditExpense()
        {
            return this.SelectedExpense != null;
        }

        public async void DeleteExpense()
        {
            if (WinUIMessageBox.Show(
                    $"Are you sure you want to delete the expense {this.SelectedExpense.Name}?",
                    "delete expense",
                    MessageBoxButton.OKCancel) != MessageBoxResult.OK)
            {
                return;
            }

            this.Simulation.Items.Remove(this.SelectedExpense);
            await Task.Run(() => this.PersistenceService.DeleteBudgetItem(this.SelectedExpense));

            this.Loaded();
        }

        public bool CanDeleteExpense()
        {
            return this.SelectedExpense != null;
        }

        public void SetXArgument(string key)
        {
            this.ChartArgument = key;
        }

        private void PrepareResults()
        {
            this.ResultColumns = new List<Column>
                                     {
                                         new Column("year", "year", ColumnFieldTypes.Int, "####", FixedStyle.Left),
                                         new Column("myage", "my age", ColumnFieldTypes.Int, "###", FixedStyle.Left),
                                         new Column("totalvalue", "total value", ColumnFieldTypes.Currency, "c0"),
                                         new Column("cash", "cash", ColumnFieldTypes.Currency, "c0"),
                                         new Column("assets", "assets", ColumnFieldTypes.Currency, "c0")
                                     };

            foreach (var item in this.Simulation.Items)
            {
                this.ResultColumns.Add(new Column(
                    item.Id.ToString(),
                    item.Name + " (" + ((item as BudgetItem)?.IsExpense ?? false ? "expense" : "income") + ")",
                    ColumnFieldTypes.Currency,
                    "c0"));
            }

            this.ResultRows = new List<ExpandoObject>();

            var age = this.PersistenceService.GetSettings().AgeAtStartDate;
            for (var year = this.Simulation.StartYear; year <= this.Simulation.EndYear; year++)
            {
                IDictionary<string, object> row = new ExpandoObject();
                row["year"] = year;
                row["myage"] = age++;
                row["totalvalue"] = this.Simulation.GetTotalValue(year);
                row["cash"] = this.Simulation.GetCash(year);
                row["assets"] = this.Simulation.GetAssets(year);

                foreach (var item in this.Simulation.Items)
                {
                    row[item.Id.ToString()] = item.GetAmount(year);
                }

                this.ResultRows.Add((ExpandoObject)row);

                if (age >= 120)
                {
                    break;
                }
            }
        }

        private void PrepareChartData()
        {
            this.ChartData = new List<ChartDataItem>();

            var age = this.PersistenceService.GetSettings().AgeAtStartDate;
            for (var year = this.Simulation.StartYear; year <= this.Simulation.EndYear; year++)
            {
                this.ChartData.Add(new ChartDataItem(
                    year,
                    age++,
                    this.Simulation.GetTotalValue(year),
                    this.Simulation.GetCash(year),
                    this.Simulation.GetAssets(year),
                    this.Simulation.IncomeItems.Sum(x => x.GetAmount(year)) + this.Simulation.Assets.Sum(x => x.GetAmount(year)),
                    this.Simulation.ExpenseItems.Sum(x => x.GetAmount(year))));

                if (age >= 120)
                {
                    break;
                }
            }
        }
    }
}