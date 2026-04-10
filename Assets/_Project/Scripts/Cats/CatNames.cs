using System.Collections.Generic;
using UnityEngine;
using CatHotel.Core;

namespace CatHotel.Cats
{
    /// <summary>
    /// Pool of cat names from localization.
    /// Tracks used names to avoid duplicates.
    /// </summary>
    public static class CatNames
    {
        private static string[] _names;
        private static readonly HashSet<string> UsedNames = new();
        private static int _duplicateCounter;

        private static string[] Names
        {
            get
            {
                if (_names == null)
                {
                    string pool = LocalizedStrings.Get("names.pool");
                    _names = pool.Split(',');
                    for (int i = 0; i < _names.Length; i++)
                        _names[i] = _names[i].Trim();
                }
                return _names;
            }
        }

        public static string GetRandomName()
        {
            var names = Names;
            for (int attempt = 0; attempt < names.Length; attempt++)
            {
                string name = names[Random.Range(0, names.Length)];
                if (UsedNames.Add(name))
                    return name;
            }

            _duplicateCounter++;
            string baseName = names[Random.Range(0, names.Length)];
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
            _names = null; // re-read from localization on next access
        }
    }
}
