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
    public class IncomeItemViewModel : IDocumentContent, ISupportParameter
    {
        public object Title => "Income";

        public IDocumentOwner DocumentOwner { get; set; }

        public object Parameter { get; set; }

        [SetterProperty]
        public PersistenceService PersistenceService { get; set; }

        public virtual BudgetItem IncomeItem { get; set; }

        public virtual bool IsNoEndDate { get; set; }

        [BindableProperty(false)]
        public bool IsOK { get; private set; }

        public virtual async void Ok()
        {
            if (this.IncomeItem.IsExpense)
            {
                this.IncomeItem.IsExpense = false;
            }

            if (this.IsNoEndDate)
            {
                this.IncomeItem.EndYear = 2300;
            }

            await Task.Run(() => this.PersistenceService.SaveBudgetItem(this.IncomeItem));
            this.IsOK = true;
            this.DocumentOwner?.Close(this);
        }

        public bool CanOk()
        {
            return !string.IsNullOrEmpty(this.IncomeItem?.Name) && this.IncomeItem.InitialValue > 0;
        }

        public void OnClose(CancelEventArgs e)
        {
        }

        public void OnDestroy()
        {
        }

        public void Loaded()
        {
            this.IncomeItem = this.Parameter as BudgetItem;
        }
    }
}