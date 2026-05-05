using System.Collections.Generic;
using UnityEngine;
using CatHotel.Core;

namespace CatHotel.Cats
{
    /// <summary>
    /// Pool of cat names from localization. Tracks used names to avoid duplicates.
    /// Supports per-breed extra names appended to the common pool.
    /// </summary>
    public static class CatNames
    {
        private static string[] _commonNames;
        private static readonly Dictionary<string, string[]> _breedExtras = new();
        private static readonly HashSet<string> UsedNames = new();
        private static int _duplicateCounter;

        private static string[] CommonNames
        {
            get
            {
                if (_commonNames == null)
                    _commonNames = ParsePool(LocalizedStrings.Get("names.pool"));
                return _commonNames;
            }
        }

        private static string[] GetBreedExtras(string breedAssetName)
        {
            if (string.IsNullOrEmpty(breedAssetName)) return System.Array.Empty<string>();
            if (_breedExtras.TryGetValue(breedAssetName, out var cached)) return cached;

            string raw = LocalizedStrings.Get($"names.pool.{breedAssetName}");
            // LocalizedStrings.Get returns "[key]" when missing → treat as empty
            if (string.IsNullOrEmpty(raw) || raw.StartsWith("[") && raw.EndsWith("]"))
            {
                _breedExtras[breedAssetName] = System.Array.Empty<string>();
                return _breedExtras[breedAssetName];
            }

            var arr = ParsePool(raw);
            _breedExtras[breedAssetName] = arr;
            return arr;
        }

        private static string[] ParsePool(string raw)
        {
            if (string.IsNullOrEmpty(raw)) return System.Array.Empty<string>();
            var parts = raw.Split(',');
            for (int i = 0; i < parts.Length; i++) parts[i] = parts[i].Trim();
            return parts;
        }

        /// <summary>Pick a random unused name. Optionally include per-breed extras.</summary>
        public static string GetRandomName(string breedAssetName = null)
        {
            var common = CommonNames;
            var extras = GetBreedExtras(breedAssetName);
            int total = common.Length + extras.Length;
            if (total == 0) return "Cat";

            for (int attempt = 0; attempt < total; attempt++)
            {
                int idx = Random.Range(0, total);
                string name = idx < common.Length ? common[idx] : extras[idx - common.Length];
                if (UsedNames.Add(name)) return name;
            }

            _duplicateCounter++;
            int fallbackIdx = Random.Range(0, total);
            string baseName = fallbackIdx < common.Length ? common[fallbackIdx] : extras[fallbackIdx - common.Length];
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
            _commonNames = null;
            _breedExtras.Clear();
        }
    }
}
