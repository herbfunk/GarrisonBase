using System;
using Styx;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace Herbfunk.GarrisonBase.Cache
{
    public abstract class C_WoWObject
    {
        public readonly WoWObject ref_WoWObject;
        public WoWPoint Location { get; set; }
        public WoWGuid Guid { get; set; }
        public uint Entry { get; set; }
        public string Name { get; set; }
        public WoWObjectType Type { get; set; }
        public BlacklistType BlacklistType { get; set; }
        public bool IgnoreRemoval { get; set; }
        public virtual float LootDistance
        {
            get { return _lootDistance; }
            set { _lootDistance = value; }
        }
        private float _lootDistance=7f;
        public virtual WoWObjectTypes SubType
        {
            get { return _subType; }
            set { _subType = value; }
        }
        private WoWObjectTypes _subType= WoWObjectTypes.Unknown;

        //
        public double CentreDistance
        {
            get { return StyxWoW.Me.Location.Distance(Location); }
        }
        public int LoopsUnseen { get; set; }
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

        protected C_WoWObject(WoWObject obj)
        {
            ref_WoWObject = obj;
            Location = obj.Location;
            Guid = obj.Guid;
            Entry = obj.Entry;
            Name = obj.Name;
            Type = obj.Type;
            BlacklistType= BlacklistType.None;
        }
        public virtual bool IsStillValid()
        {
            //Check DiaObject first
            if (ref_WoWObject == null || !ref_WoWObject.IsValid || ref_WoWObject.BaseAddress == IntPtr.Zero)
            {
                NeedsRemoved = true;
                //BlacklistType= BlacklistType.Guid;
                return false;
            }
            return true;
        }

        public virtual bool IsValid
        {
            get
            {
                //Check DiaObject first
                if (ref_WoWObject == null || !ref_WoWObject.IsValid || ref_WoWObject.BaseAddress == IntPtr.Zero)
                {
                    NeedsRemoved = true;
                    //BlacklistType = BlacklistType.Guid;
                    return false;
                }
                return true;
            }
        }
        public virtual bool Update()
        {
            return true;
        }

        private bool _requiresUpdate = true;
        public bool RequiresUpdate
        {
            get { return _requiresUpdate; }
            set { _requiresUpdate = value; }
        }
        public bool InLineOfSight
        {
            get
            {
                if (!IsStillValid()) return false;
                return ref_WoWObject.InLineOfSight;
            }
        }

        public bool WithinInteractRange
        {
            get
            {
                if (!IsStillValid()) return false;
                return ref_WoWObject.WithinInteractRange;
            }
        }




        public void Interact()
        {
            if (!IsStillValid()) return;
            ref_WoWObject.Interact();
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
            return String.Format("{0}, //{1}\r\n" +
                                 "[{2}] {3} [{4}]\r\n" +
                                 "{5} ({6})",
                                 Entry,Name,Guid, Type, SubType, Location, CentreDistance);
        }


    }
}
