namespace Herbfunk.GarrisonBase.Garrison.Objects
{
    public class ReagentInfo
    {
        public string Name { get; set; }
        public int ItemEntryId { get; set; }
        public int ItemAmountRequired { get; set; }
        public int ItemCountInBags { get; set; }


        public ReagentInfo(string name, int entryID, int amount, int count)
        {
            Name = name;
            ItemEntryId = entryID;
            ItemAmountRequired = amount;
            ItemCountInBags = count;
        }
    }
}