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
            item.Id = dto.Id;
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

        public void SaveAssetItem(AssetItem item)
        {
            var dto = item.GetDTO();
            var table = this._db.GetCollection<AssetItemDTO>(AssetItemsColletion);
            table.Upsert(dto);
            item.Id = dto.Id;
        }

        public void DeleteAssetItem(AssetItem item)
        {
            var table = this._db.GetCollection<AssetItemDTO>(AssetItemsColletion);
            table.Delete(item.Id);
        }

        public Settings GetSettings()
        {
            if (this._db.CollectionExists(SettingsColletion))
            {
                var dto = this._db.GetCollection<SettingsDTO>(SettingsColletion).FindAll().First();

                return new Settings(dto);
            }

            return new Settings();
        }

        public void SaveSettings(Settings settings)
        {
            var dto = settings.GetDTO();
            var table = this._db.GetCollection<SettingsDTO>(SettingsColletion);
            table.Upsert(dto);
            settings.Id = dto.Id;
        }

        public void DeleteSettings(Settings settings)
        {
            var table = this._db.GetCollection<SettingsDTO>(SettingsColletion);
            table.Delete(settings.Id);
        }
    }
}