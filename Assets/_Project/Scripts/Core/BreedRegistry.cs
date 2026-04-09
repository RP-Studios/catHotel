using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace CatHotel.Core
{
    /// <summary>
    /// Lazy-loads CatBreedData via Addressables on demand.
    /// Each breed is an addressable asset (address = asset filename, e.g. "Breed_Europeen").
    /// Sprites and AnimatorControllers referenced by the breed are pulled into
    /// the same bundle automatically — no duplication.
    /// </summary>
    public class BreedRegistry
    {
        [System.Serializable]
        public struct BreedEntry
        {
            public string assetName;   // Addressable address, e.g. "Breed_Europeen"
            public int minReputation;
        }

        private readonly BreedEntry[] _entries;
        private readonly Dictionary<string, CatBreedData> _cache = new();
        private readonly Dictionary<string, AsyncOperationHandle<CatBreedData>> _handles = new();

        public int Count => _entries.Length;

        public BreedRegistry(BreedEntry[] entries)
        {
            _entries = entries;
        }

        /// <summary>Get a breed by address (synchronous from cache). Returns null if not preloaded.</summary>
        public CatBreedData Get(string assetName)
        {
            if (_cache.TryGetValue(assetName, out var cached))
                return cached;

            // Synchronous fallback — blocks the main thread, avoid in production
            Debug.LogWarning($"[BreedRegistry] Sync-loading '{assetName}' — should have been preloaded");
            var handle = Addressables.LoadAssetAsync<CatBreedData>(assetName);
            handle.WaitForCompletion();
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _cache[assetName] = handle.Result;
                _handles[assetName] = handle;
                return handle.Result;
            }
            Debug.LogError($"[BreedRegistry] Failed to load '{assetName}': {handle.OperationException}");
            return null;
        }

        public BreedEntry[] Entries => _entries;

        /// <summary>Get all unlocked breeds for a given reputation level.</summary>
        public void GetUnlocked(int currentReputation, List<CatBreedData> results)
        {
            results.Clear();
            foreach (var entry in _entries)
            {
                if (currentReputation >= entry.minReputation)
                {
                    var breed = Get(entry.assetName);
                    if (breed != null)
                        results.Add(breed);
                }
            }
        }

        /// <summary>Find a breed by its breedName field.</summary>
        public CatBreedData FindByName(string breedName)
        {
            foreach (var kvp in _cache)
                if (kvp.Value.breedName == breedName)
                    return kvp.Value;

            // Load each until found
            foreach (var entry in _entries)
            {
                var breed = Get(entry.assetName);
                if (breed != null && breed.breedName == breedName)
                    return breed;
            }
            return null;
        }

        /// <summary>Coroutine to preload only the breeds the player has unlocked.</summary>
        public IEnumerator PreloadUnlockedAsync(int currentReputation)
        {
            foreach (var entry in _entries)
            {
                if (currentReputation >= entry.minReputation && !_cache.ContainsKey(entry.assetName))
                {
                    var handle = Addressables.LoadAssetAsync<CatBreedData>(entry.assetName);
                    yield return handle;
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        _cache[entry.assetName] = handle.Result;
                        _handles[entry.assetName] = handle;
                    }
                    else
                    {
                        Debug.LogError($"[BreedRegistry] Failed to preload '{entry.assetName}': {handle.OperationException}");
                    }
                }
            }
        }

        /// <summary>Release all loaded breed assets.</summary>
        public void UnloadAll()
        {
            foreach (var handle in _handles.Values)
            {
                if (handle.IsValid())
                    Addressables.Release(handle);
            }
            _handles.Clear();
            _cache.Clear();
        }

        public IEnumerable<CatBreedData> LoadedBreeds => _cache.Values;
    }
}
