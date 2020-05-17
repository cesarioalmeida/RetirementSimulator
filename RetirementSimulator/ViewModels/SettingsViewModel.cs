namespace RetirementSimulator.ViewModels
{
    using System.ComponentModel;
    using System.Threading.Tasks;

    using DevExpress.Mvvm;
    using DevExpress.Mvvm.DataAnnotations;

    using RetirementSimulator.Core.Models;
    using RetirementSimulator.Core.Services;

    using StructureMap.Attributes;

    [POCOViewModel]
    public class SettingsViewModel : IDocumentContent
    {
        public object Title => "Settings";

        public IDocumentOwner DocumentOwner { get; set; }

        [SetterProperty]
        public PersistenceService PersistenceService { get; set; }

        public virtual Settings Settings { get; set; }

        public virtual async void Ok()
        {
            await Task.Run(() => this.PersistenceService.SaveSettings(this.Settings));
            this.DocumentOwner?.Close(this);
        }

        public void OnClose(CancelEventArgs e)
        {
        }

        public void OnDestroy()
        {
        }

        public async void Loaded()
        {
            this.Settings = await Task.Run(() => this.PersistenceService.GetSettings());

            if (this.Settings == null)
            {
                this.Settings = new Settings();
                await Task.Run(() => this.PersistenceService.SaveSettings(this.Settings));
            }
        }
    }
}