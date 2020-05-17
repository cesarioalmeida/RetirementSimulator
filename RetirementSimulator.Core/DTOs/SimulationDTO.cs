namespace RetirementSimulator.Core.DTOs
{
    using System.Collections.Generic;

    using LiteDB;

    public class SimulationDTO
    {
        public ObjectId Id { get; set; }

        public int StartYear { get; set; }

        public int EndYear { get; set; }

        public List<ObjectId> BudgetItemsIds { get; set; }

        public List<ObjectId> AssetItemsIds { get; set; }
    }
}