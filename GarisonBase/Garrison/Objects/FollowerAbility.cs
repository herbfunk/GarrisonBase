using Herbfunk.GarrisonBase.Garrison.Enums;

namespace Herbfunk.GarrisonBase.Garrison.Objects
{
    public class FollowerAbility
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public CombatAbilities Counters { get; set; }
        public int CounterID { get; set; }

        public FollowerAbility(int id, string name, CombatAbilities counter, int counterid=-1)
        {
            ID = id;
            Name = name;
            Counters = counter;
            CounterID = counterid;
        }
    }
}