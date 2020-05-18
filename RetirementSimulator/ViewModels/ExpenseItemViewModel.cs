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
    public class ExpenseItemViewModel : IDocumentContent, ISupportParameter
    {
        public object Title => "Expense";

        public IDocumentOwner DocumentOwner { get; set; }

        public object Parameter { get; set; }

        [SetterProperty]
        public PersistenceService PersistenceService { get; set; }

        public virtual BudgetItem ExpenseItem { get; set; }

        public virtual bool IsNoEndDate { get; set; }

        [BindableProperty(false)]
        public bool IsOK { get; private set; }

        public virtual async void Ok()
        {
            if (!this.ExpenseItem.IsExpense)
            {
                this.ExpenseItem.IsExpense = true;
            }

            if (this.IsNoEndDate)
            {
                this.ExpenseItem.EndYear = 2300;
            }

            await Task.Run(() => this.PersistenceService.SaveBudgetItem(this.ExpenseItem));
            this.IsOK = true;
            this.DocumentOwner?.Close(this);
        }

        public bool CanOk()
        {
            return !string.IsNullOrEmpty(this.ExpenseItem?.Name) && this.ExpenseItem.InitialValue > 0;
        }

        public void OnClose(CancelEventArgs e)
        {
        }

        public void OnDestroy()
        {
        }

        public void Loaded()
        {
            this.ExpenseItem = this.Parameter as BudgetItem;
        }
    }
}