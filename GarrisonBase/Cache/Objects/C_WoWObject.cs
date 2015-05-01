using System;
using System.Collections.Generic;
using Herbfunk.GarrisonBase.Cache.Enums;
using Herbfunk.GarrisonBase.Character;
using Styx;
using Styx.Common.Helpers;
using Styx.Helpers;
using Styx.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.World;
using Styx.WoWInternals.WoWObjects;

namespace Herbfunk.GarrisonBase.Cache.Objects
{
    public class C_WoWObject : EntryBase
    {
        protected C_WoWObject(WoWObject obj):base(obj)
        {
            RefWoWObject = obj;
            Location = obj.Location;
            Rotation = obj.Rotation;
            Guid = obj.Guid;
            BlacklistType = BlacklistType.None;
            LineOfSight = new CachedValue<bool>(() => TestLineOfSight(this));
            Collision = new CachedValue<bool>(() => TestCollision(this));
        }



        public WoWObject RefWoWObject;
        /// <summary>
        /// Used to update the object reference to the HB WoWObject. (Used for cached objects that ignore removal)
        /// </summary>
        /// <param name="obj"></param>
        public virtual void UpdateReference(WoWObject obj)
        {
            RefWoWObject = obj;
        }

        public WoWPoint Location { get; set; }
        public float Rotation { get; set; }
        public WoWGuid Guid { get; set; }
        public BlacklistType BlacklistType { get; set; }
        public bool ShouldLoot
        {
            get
            {
                if (BaseSettings.CurrentSettings.LootAnyMobs) return true;
                return _shouldLoot;
            }
            set { _shouldLoot = value; }
        }
        private bool _shouldLoot;

        public bool ShouldKill
        {
            get { return _shouldKill; }
            set { _shouldKill = value; }
        }
        private bool _shouldKill;

        public bool IsQuestNpc { get; set; }

        public WaitTimer IgnoredTimer
        {
            get { return _ignoredTimer; }
            set
            {
                _ignoredTimer = value;
                _ignoredTimer.Reset();
            }
        }
        private WaitTimer _ignoredTimer= new WaitTimer(new TimeSpan(0,0,0,0,0));
        public bool IgnoresRemoval { get; set; }
        public float InteractRange
        {
            get { return _interactRange; }
            set { _interactRange = value; }
        }
        private float _interactRange=4f;

        //
        public double Distance
        {
            get { return Character.Player.Location.Distance(Location); }
        }

        public double DistanceSqr
        {
            get
            {
                return Character.Player.Location.DistanceSqr(Location);
            }
        }

        public double ZDifference
        {
            get
            {
                if (Character.Player.Location.Z>Location.Z)
                    return Character.Player.Location.Z - Location.Z;

                return Location.Z - Character.Player.Location.Z;
            }
        }


        
        ///<summary>
        ///Flag that determines if the object should be removed from the collection.
        ///</summary>
        public bool NeedsRemoved
        {
            get
            {
                return removal_;
            }
            set
            {
                removal_ = value;
                //This helps reduce code by flagging this here instead of after everytime we flag removal of an object!
                if (value) ObjectCacheManager.RemovalCheck = true;
            }
        }
        internal bool removal_;

        public int LoopsUnseen { get; set; }


        public virtual bool IsValid
        {
            get
            {
                //Check Object
                ObjectValidCheck = RefWoWObject != null && 
                    RefWoWObject.IsValid && 
                    RefWoWObject.BaseAddress != IntPtr.Zero;

                return ObjectValidCheck;
            }
        }
        internal bool ObjectValidCheck=true;

        /// <summary>
        /// This is used to reset and update values that need to reference the actual WoWObject.
        /// </summary>
        /// <returns></returns>
        public virtual bool Update()
        {
            if (!IsValid) return false;
            LineOfSight.Reset();
            Collision.Reset();
            return true;
        }

        /// <summary>
        /// Determines if object should be considered for targeting.
        /// </summary>
        public virtual bool ValidForTargeting
        {
            get
            {
                if (!IsValid)
                {
                    if (!IgnoresRemoval)
                    {
                        NeedsRemoved = true;
                        IgnoredTimer = WaitTimer.ThirtySeconds;
                    }
                    return false;
                }

                if (!IgnoredTimer.IsFinished)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Determines if object should be considered for looting.
        /// </summary>
        public virtual bool ValidForLooting
        {
            get
            {
                if (!IsValid)
                {
                    if (!IgnoresRemoval)
                    {
                        NeedsRemoved = true;
                        IgnoredTimer = WaitTimer.ThirtySeconds;
                    }
                    return false;
                }

                if (!IgnoredTimer.IsFinished)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Determines if object should be considered for combat.
        /// </summary>
        public virtual bool ValidForCombat
        {
            get
            {
                if (!IsValid)
                {
                    if (!IgnoresRemoval)
                    {
                        NeedsRemoved = true;
                        IgnoredTimer = WaitTimer.ThirtySeconds;
                    }

                    return false;
                }

                if (!IgnoredTimer.IsFinished)
                {
                    return false;
                }

                return true;
            }
        }


        
        public bool RequiresUpdate
        {
            get { return _requiresUpdate; }
            set { _requiresUpdate = value; }
        }
        private bool _requiresUpdate = true;

        public bool WithinInteractRange
        {
            get
            {
                if (!IsValid) return false;
                return CheckDistance(InteractRange);
                //return ref_WoWObject.WithinInteractRange;
            }
        }

        public bool CheckDistance(float distance)
        {
            float num = distance - 0.1f;
            return DistanceSqr <= num * num;
        }


        public int InteractionAttempts = 0;
        public void Interact()
        {
            if (!IsValid) return;
            RefWoWObject.Interact();
            InteractionAttempts++;
        }
        public bool IsBehindPlayer
        {
            get
            {
                return WoWMathHelper.IsBehind(Location, Player.Location, Player.Rotation, 3.141593f);
            }
        }
        public bool IsBehindObject(C_WoWObject obj)
        {
            return WoWMathHelper.IsBehind(Location, obj.Location, obj.Rotation, 3.141593f);
        }

        public WoWPoint GetBehindPoint(float distance=5f)
        {
            float facing = RefWoWObject.Rotation;
            facing += WoWMathHelper.DegreesToRadians(180);
            facing = WoWMathHelper.NormalizeRadian(facing);
            return Location.RayCast(facing, distance);
        }
        public WoWPoint GetFrontPoint(float distance = 5f)
        {
            float facing = RefWoWObject.Rotation;
            facing = WoWMathHelper.NormalizeRadian(facing);
            return Location.RayCast(facing, distance);
        }
        public WoWPoint GetSidePoint(float degrees, float distance = 5f)
        {

            float facing = WoWMathHelper.DegreesToRadians(degrees);
            facing = WoWMathHelper.NormalizeRadian(facing);
            return Location.RayCast(facing, distance);
        }

        public CachedValue<bool> LineOfSight, Collision;
        internal bool LineofsightResult = false;
        internal bool CollisionResult = false;

        public WoWPoint LineofsightPoint
        {
            get
            {
                if (_lineofsightPoint == WoWPoint.Zero) 
                    return Location;
                return _lineofsightPoint;
            }
            set { _lineofsightPoint = value; }
        }
        private WoWPoint _lineofsightPoint= WoWPoint.Zero;

        public WaitTimer LineofSightWaitTimer
        {
            get
            {
                return _lineofSightWaitTimer;
            }
            set
            {
                _lineofSightWaitTimer = value;
                _lineofSightWaitTimer.Reset();
            }
        }
        private WaitTimer _lineofSightWaitTimer=WaitTimer.FiveSeconds;

        public WaitTimer CollisionWaitTimer
        {
            get
            {
                return _collisionWaitTimer;
            }
            set
            {
                _collisionWaitTimer = value;
                _collisionWaitTimer.Reset();
            }
        }




        private WaitTimer _collisionWaitTimer = WaitTimer.FiveSeconds;

        internal static bool TestLineOfSight(C_WoWObject obj)
        {
            if (obj.LineofsightResult && obj.Distance <= obj.InteractRange)
                return true;

            if (!obj._lineofSightWaitTimer.IsFinished && (obj.Distance>30f || obj.LineofSightWaitTimer.TimeLeft.TotalMilliseconds>750))
                return obj.LineofsightResult;
            

            var neededFacing=WoWMathHelper.CalculateNeededFacing(Character.Player.Location, obj.Location);

            var testPoints = new List<WoWPoint>
            {
                WoWMathHelper.GetPointAt(obj.Location, obj.InteractRange, neededFacing - 3.141593f, 0f),
                WoWMathHelper.GetPointAt(obj.Location, obj.InteractRange, neededFacing - 2.356194f, 0f),
                WoWMathHelper.GetPointAt(obj.Location, obj.InteractRange, neededFacing - 1.570796f, 0f),
                WoWMathHelper.GetPointAt(obj.Location, obj.InteractRange, neededFacing - 0.7853982f, 0f),
                WoWMathHelper.GetPointAt(obj.Location, obj.InteractRange, neededFacing, 0f),
                WoWMathHelper.GetPointAt(obj.Location, obj.InteractRange, neededFacing + 3.141593f, 0f),
                WoWMathHelper.GetPointAt(obj.Location, obj.InteractRange, neededFacing + 2.356194f, 0f),
                WoWMathHelper.GetPointAt(obj.Location, obj.InteractRange, neededFacing + 1.570796f, 0f),
                WoWMathHelper.GetPointAt(obj.Location, obj.InteractRange, neededFacing + 0.7853982f, 0f)
            };

            foreach (var testPoint in testPoints)
            {
                if (GameWorld.IsInLineOfSight(Character.Player.TraceLinePosition,testPoint.Add(0f,0f,1.24f)))
                {
                    obj.LineofsightResult = true;
                    obj._lineofsightPoint = testPoint;
                    break;
                }
            }


            obj.LineofSightWaitTimer.Reset();
            return obj.LineofsightResult;
        }

        internal static bool TestCollision(C_WoWObject obj)
        {
            if (obj.CollisionResult && obj.Distance <= obj.InteractRange)
                return true;

            if (!obj._collisionWaitTimer.IsFinished && (obj.Distance > 30f || obj.CollisionWaitTimer.TimeLeft.TotalMilliseconds > 750))
                return obj.CollisionResult;

            var result = GameWorld.TraceLine(Character.Player.TraceLinePosition, obj.Location, TraceLineHitFlags.DoodadCollision);
            obj.CollisionResult = result;

            obj.CollisionWaitTimer.Reset();
            return result;
        }
 

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types. 
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                C_WoWObject p = (C_WoWObject)obj;
                return Guid == p.Guid;
            }
        }


        public override string ToString()
        {
            return String.Format("{0}" +
                                 "Guid {1}\r\n" +
                                 "{2} ({3})\r\n" +
                                 "LOS {4} ( {5} ) Tested {6}ms\r\n" +
                                 "Collision {9} Tested {10}ms\r\n" +
                                 "Valid {7}\r\n" +
                                 "Update {8}\r\n" +
                                 "IgnoreTime {11}",
                                 base.ToString(),Guid,
                                 Location, Distance, 
                                 LineofsightResult, _lineofsightPoint.ToString(), LineofSightWaitTimer.TimeLeft.TotalMilliseconds,
                                 ObjectValidCheck,
                                 RequiresUpdate,
                                 CollisionResult, 
                                 CollisionWaitTimer.TimeLeft.TotalMilliseconds,
                                 IgnoredTimer.IsFinished?"0":IgnoredTimer.TimeLeft.TotalMilliseconds + "ms");
        }


    }
}
