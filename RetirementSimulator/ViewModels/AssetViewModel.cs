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
    public class AssetViewModel : IDocumentContent, ISupportParameter
    {
        public object Title => "Asset";

        public IDocumentOwner DocumentOwner { get; set; }

        public object Parameter { get; set; }

        [SetterProperty]
        public PersistenceService PersistenceService { get; set; }

        public virtual AssetItem Asset { get; set; }

        public virtual bool IsNoEndDate { get; set; }

        [BindableProperty(false)]
        public bool IsOK { get; private set; }

        public virtual async void Ok()
        {
            if (this.IsNoEndDate)
            {
                this.Asset.EndYear = 2300;
            }

            await Task.Run(() => this.PersistenceService.SaveAssetItem(this.Asset));
            this.IsOK = true;
            this.DocumentOwner?.Close(this);
        }

        public bool CanOk()
        {
            return !string.IsNullOrEmpty(this.Asset?.Name) && this.Asset.InitialValue > 0;
        }

        public void OnClose(CancelEventArgs e)
        {
        }

        public void OnDestroy()
        {
        }

        public void Loaded()
        {
            this.Asset = this.Parameter as AssetItem;
        }
    }
}