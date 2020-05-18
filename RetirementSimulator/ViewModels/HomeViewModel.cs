namespace RetirementSimulator.ViewModels
{
    using System.Threading.Tasks;
    using System.Windows;

    using DevExpress.Mvvm;
    using DevExpress.Mvvm.DataAnnotations;
    using DevExpress.Mvvm.POCO;
    using DevExpress.Xpf.WindowsUI;

    using RetirementSimulator.Core.Models;
    using RetirementSimulator.Core.Services;
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
            }
            finally
            {
                this.RaisePropertyChanged(x => x.Simulation);
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
                    $"Are you sure you want to delete the asset {this.SelectedAsset}?",
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
    }
}