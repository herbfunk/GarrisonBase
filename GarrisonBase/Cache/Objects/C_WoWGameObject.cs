using System;
using Herbfunk.GarrisonBase.Cache.Enums;
using Herbfunk.GarrisonBase.Character;
using Herbfunk.GarrisonBase.Garrison.Objects;
using Herbfunk.GarrisonBase.TargetHandling;
using Styx;
using Styx.Common.Helpers;
using Styx.WoWInternals.WoWObjects;

namespace Herbfunk.GarrisonBase.Cache.Objects
{
    public class C_WoWGameObject : C_WoWObject
    {
        public WoWGameObject RefWoWGameObject;
        public override void UpdateReference(WoWObject obj)
        {
            base.UpdateReference(obj);
            RefWoWGameObject = obj.ToGameObject();
        }


        public C_WoWGameObject(WoWGameObject obj)
            : base(obj)
        {
            RefWoWGameObject = obj;

            switch (SubType)
            {
                case WoWObjectTypes.OreVein:
                    ShouldLoot = TargetManager.CheckLootFlag(TargetManager.LootFlags.Ore);
                    IgnoresRemoval = Player.InsideGarrison;
                    InteractRange = 6f;
                    LineofSightWaitTimer = new WaitTimer(new TimeSpan(0, 0, 0, 2, 500));
                    LineofSightWaitTimer.Stop();
                    break;
                case WoWObjectTypes.Herb:
                    ShouldLoot = TargetManager.CheckLootFlag(TargetManager.LootFlags.Herbs);
                    IgnoresRemoval = Player.InsideGarrison;
                    InteractRange = 5f;
                    break;
                case WoWObjectTypes.GarrisonCommandTable:
                case WoWObjectTypes.GarrisonCache:
                    IgnoresRemoval = true;
                    return;
                case WoWObjectTypes.GarrisonShipment:
                    InteractRange = 5.8f;
                    IgnoresRemoval = true;
                    return;
                case WoWObjectTypes.GarrisonFinalizePlot:
                    break;
                case WoWObjectTypes.Mailbox:
                    InteractRange = 6f;
                    IgnoresRemoval = true;
                    return;
                case WoWObjectTypes.Timber:
                    ShouldLoot = TargetManager.CheckLootFlag(TargetManager.LootFlags.Lumber);
                    InteractRange = 5f;
                    break;
                case WoWObjectTypes.Unknown:
                    RequiresUpdate = false;
                    break;
            }

            if (!ObjectCacheManager.LootIds.Contains(Entry)) return;
            ShouldLoot = true;
            RequiresUpdate = true;
        }



        public override bool Update()
        {
            if (!base.Update()) return false;

            return true;
        }

        public override bool ValidForLooting
        {
            get
            {
                if (!base.ValidForTargeting) return false;

                if (!ShouldLoot) return false;

                switch (SubType)
                {
                    case WoWObjectTypes.Herb:
                        if (!TargetManager.CheckLootFlag(TargetManager.LootFlags.Herbs)) return false;
                        break;
                    case WoWObjectTypes.OreVein:
                        if (!TargetManager.CheckLootFlag(TargetManager.LootFlags.Ore)) return false;
                        break;
                    case WoWObjectTypes.Timber:
                        if (!TargetManager.CheckLootFlag(TargetManager.LootFlags.Lumber)) return false;
                        break;
                }

                if (!ObjectCacheManager.IgnoreLineOfSightFailure && !LineOfSight) return false;
                
                return true;
            }
        }

        public WoWCursorType GetCursor
        {
            get
            {
                if (!IsValid) return WoWCursorType.None;
                return RefWoWGameObject.GetCursor();
            }
        }


        public override string ToString()
        {
            return String.Format("{0}\r\n" +
                                 "ShouldLoot {1}",
                                    base.ToString(),
                                    ShouldLoot);
        }

    }
}