namespace CatHotel.Core
{
    /// <summary>Need categories for cats.</summary>
    public enum NeedType
    {
        Hunger,
        Thirst,
        Sleep,
        Play,
        Clean
    }

    /// <summary>Behavioral state of a cat.</summary>
    public enum CatState
    {
        Idle,
        Walking,
        Seeking,     // looking for an object to satisfy a need
        UsingObject, // eating, sleeping, playing, cleaning
        Happy,
        Unhappy,
        Fighting,
        Leaving,     // walking to exit
        Arriving,    // walking from entrance to hotel
        Pickup,      // owner picking up (pension end)
        Adopted      // adoptant taking cat (refuge)
    }

    /// <summary>How the cat arrived at the hotel.</summary>
    public enum CatMode
    {
        Pension,  // owner deposited, will pick up later
        Refuge    // abandoned, waiting for adoption
    }

    /// <summary>Object need category (what need it satisfies).</summary>
    public enum ObjectCategory
    {
        Food,
        Water,
        Sleep,
        Play,
        Clean,
        Decoration,
        Support,
        Carpet
    }
}
