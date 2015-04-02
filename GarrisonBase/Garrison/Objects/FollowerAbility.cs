using Herbfunk.GarrisonBase.Garrison.Enums;

namespace Herbfunk.GarrisonBase.Garrison.Objects
{
    public class FollowerAbility
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CombatAbilities Counters { get; set; }
        public int CounterId { get; set; }

        public FollowerAbility(int id, string name, CombatAbilities counter, int counterid=-1)
        {
            Id = id;
            Name = name;
            Counters = counter;
            CounterId = counterid;
        }
    }
}