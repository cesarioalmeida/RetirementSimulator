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
            }
            finally
            {
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
    }
}