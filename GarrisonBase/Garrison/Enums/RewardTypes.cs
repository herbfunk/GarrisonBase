using System;

namespace Herbfunk.GarrisonBase.Garrison.Enums
{
    [Flags]
    public enum RewardTypes
    {
        None=0,
        Gold=1,
        Garrison=2,
        ApexisCrystal = 4,
        FollowerToken=8,
        Other=16,
        XP=32,
        CharacterToken=64,
        SealOfTemperedFate=128,
        HonorPoints=256,
        ArtifactFragment=512,
        RetrainingCertificate=1024,
        FollowerContract=2048,
        FollowerTrait=4096,
        RushOrder = 8192,
        Items = 16384,
    }
}