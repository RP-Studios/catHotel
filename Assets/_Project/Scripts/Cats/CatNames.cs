using System.Collections.Generic;
using UnityEngine;

namespace CatHotel.Cats
{
    /// <summary>
    /// Pool of French cat names from the GDD.
    /// Tracks used names to avoid duplicates.
    /// </summary>
    public static class CatNames
    {
        private static readonly string[] Names =
        {
            "Minou", "Felix", "Caramel", "Luna", "Tigrou", "Noisette",
            "Moustache", "Pacha", "Cannelle", "Gribouille", "Minette", "Câlin",
            "Filou", "Chipie", "Réglisse", "Perle", "Simba", "Plume",
            "Biscuit", "Cookie", "Praline", "Nougat", "Macaron", "Brioche",
            "Muffin", "Crumble", "Tiramisu", "Meringue", "Brownie", "Fudge",
            "Cosmos", "Lune", "Étoile", "Comète", "Nova", "Nebula",
            "Soleil", "Aurore", "Eclipse", "Galaxie", "Astro", "Pluton",
            "Rubis", "Saphir", "Émeraude", "Jade", "Opale", "Ambre",
            "Topaze", "Diamant", "Cristal", "Onyx", "Ivoire", "Corail",
            "Ninja", "Pixel", "Wifi", "Sushi", "Tofu", "Wasabi",
            "Mozart", "Picasso", "Darwin", "Merlin", "Zorro", "Gatsby",
            "Chouette", "Papillon", "Colibri", "Hibou", "Renard", "Loutre"
        };

        private static readonly HashSet<string> UsedNames = new();
        private static int _duplicateCounter;

        public static string GetRandomName()
        {
            // Try to find an unused name
            for (int attempt = 0; attempt < Names.Length; attempt++)
            {
                string name = Names[Random.Range(0, Names.Length)];
                if (UsedNames.Add(name))
                    return name;
            }

            // All names used — generate numbered variant
            _duplicateCounter++;
            string baseName = Names[Random.Range(0, Names.Length)];
            string numbered = $"{baseName} {_duplicateCounter + 1}";
            UsedNames.Add(numbered);
            return numbered;
        }

        public static void ReleaseName(string name)
        {
            UsedNames.Remove(name);
        }

        public static void Reset()
        {
            UsedNames.Clear();
            _duplicateCounter = 0;
        }
    }
}
