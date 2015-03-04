using Styx.WoWInternals;

namespace Herbfunk.GarrisonBase.Quest.Objects
{
    public class CPlayerQuest
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public bool IsCompleted { get; set; }

        public CPlayerQuest(PlayerQuest quest)
        {
            Id = quest.Id;
            Name = quest.Name;
            IsCompleted = quest.IsCompleted;
        }

        public void Update(PlayerQuest quest)
        {
            if (quest.Id != Id) return;

            IsCompleted = quest.IsCompleted;
        }
    }
}