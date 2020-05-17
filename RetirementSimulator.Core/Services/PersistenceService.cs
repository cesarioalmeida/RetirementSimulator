namespace RetirementSimulator.Core.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using LiteDB;

    using RetirementSimulator.Core.DTOs;
    using RetirementSimulator.Core.Helpers;
    using RetirementSimulator.Core.Models;

    public class PersistenceService
    {
        private const string BudgetItemsColletion = "BudgetItems";
        private const string AssetItemsColletion = "AssetItems";
        private const string SimulationColletion = "Simulation";
        private const string SettingsColletion = "Settings";

        private static readonly string DbLocation = Path.Combine(ApplicationHelper.GetAppDataFolder(), "DB.db");

        private static readonly string DbConnectionString = $"filename={DbLocation}; journal=false";

        private readonly LiteDatabase _db = new LiteDatabase(DbConnectionString);

        public IEnumerable<BudgetItem> GetAllBudgetItems()
        {
            if (this._db.CollectionExists(BudgetItemsColletion))
            {
                var settings = this.GetSettings();
                var budgetDtos = this._db.GetCollection<BudgetItemDTO>(BudgetItemsColletion).FindAll();

                return budgetDtos?.Select(x => new BudgetItem(settings.InflationRate, x));
            }

            return Enumerable.Empty<BudgetItem>();
        }

        public IEnumerable<BudgetItem> GetBudgetItems(IReadOnlyCollection<ObjectId> ids)
        {
            if (this._db.CollectionExists(BudgetItemsColletion))
            {
                var settings = this.GetSettings();
                var budgetDtos = this._db.GetCollection<BudgetItemDTO>(BudgetItemsColletion).Find(x => ids.Contains(x.Id));

                return budgetDtos?.Select(x => new BudgetItem(settings.InflationRate, x));
            }

            return Enumerable.Empty<BudgetItem>();
        }

        public IEnumerable<BudgetItem> GetAllExpenses()
        {
            if (this._db.CollectionExists(BudgetItemsColletion))
            {
                var settings = this.GetSettings();
                var budgetDtos = this._db.GetCollection<BudgetItemDTO>(BudgetItemsColletion).Find(x => x.IsExpense);

                return budgetDtos?.Select(x => new BudgetItem(settings.InflationRate, x));
            }

            return Enumerable.Empty<BudgetItem>();
        }

        public IEnumerable<BudgetItem> GetAllIncome()
        {
            if (this._db.CollectionExists(BudgetItemsColletion))
            {
                var settings = this.GetSettings();
                var budgetDtos = this._db.GetCollection<BudgetItemDTO>(BudgetItemsColletion).Find(x => !x.IsExpense);

                return budgetDtos?.Select(x => new BudgetItem(settings.InflationRate, x));
            }

            return Enumerable.Empty<BudgetItem>();
        }

        public void SaveBudgetItem(BudgetItem item)
        {
            var dto = item.GetDTO();
            var budgetItemsTable = this._db.GetCollection<BudgetItemDTO>(BudgetItemsColletion);
            budgetItemsTable.Upsert(dto);
        }

        public void DeleteBudgetItem(BudgetItem item)
        {
            var budgetItemsTable = this._db.GetCollection<BudgetItemDTO>(BudgetItemsColletion);
            budgetItemsTable.Delete(item.Id);
        }

        public IEnumerable<AssetItem> GetAllAssetItems()
        {
            if (this._db.CollectionExists(AssetItemsColletion))
            {
                var dtos = this._db.GetCollection<AssetItemDTO>(AssetItemsColletion).FindAll();

                return dtos?.Select(x => new AssetItem(x));
            }

            return Enumerable.Empty<AssetItem>();
        }

        public IEnumerable<AssetItem> GetAssetItems(IReadOnlyCollection<ObjectId> ids)
        {
            if (this._db.CollectionExists(AssetItemsColletion))
            {
                var dtos = this._db.GetCollection<AssetItemDTO>(AssetItemsColletion).Find(x => ids.Contains(x.Id));

                return dtos?.Select(x => new AssetItem(x));
            }

            return Enumerable.Empty<AssetItem>();
        }

        public void SaveAssetItem(AssetItem item)
        {
            var dto = item.GetDTO();
            var table = this._db.GetCollection<AssetItemDTO>(AssetItemsColletion);
            table.Upsert(dto);
        }

        public void DeleteAssetItem(AssetItem item)
        {
            var table = this._db.GetCollection<AssetItemDTO>(AssetItemsColletion);
            table.Delete(item.Id);
        }

        public Simulation GetSimulation()
        {
            if (this._db.CollectionExists(SimulationColletion))
            {
                var dto = this._db.GetCollection<SimulationDTO>(SimulationColletion).FindAll().LastOrDefault();
                return dto != null ? new Simulation(dto, this.GetBudgetItems(dto.BudgetItemsIds), this.GetAssetItems(dto.AssetItemsIds)) : null;
            }

            return null;
        }

        public void SaveSimulation(Simulation item)
        {
            var dto = item.GetDTO();
            var table = this._db.GetCollection<SimulationDTO>(SimulationColletion);
            table.Upsert(dto);
        }

        public void DeleteSimulation(Simulation item)
        {
            var table = this._db.GetCollection<SimulationDTO>(SimulationColletion);
            table.Delete(item.Id);
        }

        public Settings GetSettings()
        {
            if (this._db.CollectionExists(SettingsColletion))
            {
                var dto = this._db.GetCollection<SettingsDTO>(SettingsColletion).FindAll().LastOrDefault();

                return dto != null ? new Settings(dto) : null;
            }

            return null;
        }

        public void SaveSettings(Settings settings)
        {
            var dto = settings.GetDTO();
            var table = this._db.GetCollection<SettingsDTO>(SettingsColletion);
            table.Upsert(dto);
        }

        public void DeleteSettings(Settings settings)
        {
            var table = this._db.GetCollection<SettingsDTO>(SettingsColletion);
            table.Delete(settings.Id);
        }
    }
}