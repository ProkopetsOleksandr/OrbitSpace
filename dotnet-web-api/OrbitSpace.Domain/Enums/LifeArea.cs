using System.ComponentModel;

namespace OrbitSpace.Domain.Enums
{
    public enum LifeArea : byte
    {
        [Description("Career & Business")]
        Career = 1,

        [Description("Finance & Wealth")]
        Finance = 2,

        [Description("Health & Fitness")]
        Health = 3,

        [Description("Family & Friends")]
        FamilyAndFriends = 4,

        [Description("Romance & Relationships")]
        Relationships = 5,

        [Description("Personal Growth")]
        PersonalGrowth = 6,

        [Description("Fun & Recreation")]
        FunAndRecreation = 7,

        [Description("Physical Environment")]
        PhysicalEnvironment = 8
    }
}
