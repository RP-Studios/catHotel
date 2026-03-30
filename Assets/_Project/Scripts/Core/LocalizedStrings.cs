namespace CatHotel.Core
{
    /// <summary>
    /// Centralized UI strings for easy localization.
    /// All user-visible text should reference this class.
    /// </summary>
    public static class LocalizedStrings
    {
        // Cat Info Panel
        public const string Kitten = "Chaton";
        public const string AdultCat = "Chat adulte";
        public const string StayPension = "En pension";
        public const string StayRefuge = "Arrivé au refuge";

        // Affinities
        public const string LikesFormat = "Aime les {0}";
        public const string DislikesFormat = "Déteste les {0}";

        // End Pension Panel
        public const string ByeFormat = "Au revoir {0} !";

        // Need bubble (for future localization)
        public const string NeedHungry = "A faim";
        public const string NeedThirsty = "A soif";
        public const string NeedTired = "Fatigué";
        public const string NeedBored = "S'ennuie";
        public const string NeedDirty = "Sale";

        // Mood
        public const string MoodHappy = "Content";
        public const string MoodEcstatic = "Enthousiaste";
        public const string MoodDepressed = "Déprimé";
        public const string MoodAggressive = "Bagarreur";
        public const string MoodAngry = "En colère";
    }
}
