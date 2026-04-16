namespace CatHotel.Core
{
    /// <summary>
    /// Bump RequiredSaveVersion when pushing a new store build that must
    /// force ALL players to start a fresh game (new tutorial, data reset, etc.).
    /// Any existing save with saveVersion &lt; RequiredSaveVersion is wiped on boot.
    /// </summary>
    public static class GameVersion
    {
        /// <summary>
        /// Minimum save version accepted. Saves below this are wiped.
        /// Bump this number before each store push that needs a forced fresh start.
        /// </summary>
        public const int RequiredSaveVersion = 2; // v0.21 — forced fresh start for tutorial
    }
}
