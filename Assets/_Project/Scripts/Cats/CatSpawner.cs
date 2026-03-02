using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using CatHotel.Grid;

namespace CatHotel.Cats
{
    [System.Serializable]
    public class CatBreed
    {
        public string name;
        public Sprite frontSprite;
        public Sprite rightSprite;
        public Sprite backSprite;
        public RuntimeAnimatorController controller;
    }

    public class CatSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GridRenderer _gridRenderer;

        [Header("Breeds")]
        [SerializeField] private CatBreed[] _breeds;

        [Header("Combat")]
        [SerializeField] private RuntimeAnimatorController _fightCloudController;

        [Header("Petting")]
        [SerializeField] private RuntimeAnimatorController _handPetController;

        [Header("Spawning")]
        [SerializeField] private int _initialCatCount = 3;
        [SerializeField] private int _sortingOrder = 10;

        private readonly List<CatEntity> _cats = new();

        public RuntimeAnimatorController FightCloudController => _fightCloudController;
        public RuntimeAnimatorController HandPetController => _handPetController;

        private bool _pettingMode;
        private Camera _mainCam;

        public bool PettingMode => _pettingMode;

        public void TogglePetting()
        {
            _pettingMode = !_pettingMode;
        }

        private void Update()
        {
            var kb = Keyboard.current;
            if (kb != null && kb.sKey.wasPressedThisFrame)
                SpawnInitialCats();

            if (_pettingMode && UnityEngine.InputSystem.Pointer.current != null
                && UnityEngine.InputSystem.Pointer.current.press.wasPressedThisFrame)
            {
                TryPetAtPointer();
            }
        }

        private void TryPetAtPointer()
        {
            if (_mainCam == null) _mainCam = Camera.main;
            if (_mainCam == null) return;

            Vector2 screenPos = UnityEngine.InputSystem.Pointer.current.position.ReadValue();
            Vector3 worldPos = _mainCam.ScreenToWorldPoint(screenPos);

            CatEntity nearest = null;
            float bestDist = 1.5f; // max tap radius in world units
            foreach (var cat in _cats)
            {
                float dist = Vector2.Distance(worldPos, cat.transform.position);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    nearest = cat;
                }
            }

            if (nearest != null)
                nearest.PlayPetting(_handPetController);
        }

        public void SpawnInitialCats()
        {
            var floorCells = _gridRenderer.Data.GetAllFloorCells();
            if (floorCells.Count == 0)
            {
                Debug.LogWarning("[CatSpawner] No floor cells. Build a room first!");
                return;
            }

            int count = Mathf.Min(_initialCatCount, floorCells.Count);
            ShuffleList(floorCells);

            for (int i = 0; i < count; i++)
                SpawnCat(floorCells[i]);

            Debug.Log($"[CatSpawner] Spawned {count} cats on floor cells");
        }

        public CatEntity SpawnCat(Vector2Int cell)
        {
            var breed = _breeds[Random.Range(0, _breeds.Length)];

            var go = new GameObject($"Cat_{_cats.Count + 1}_{breed.name}");
            go.transform.SetParent(transform);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = breed.frontSprite;
            sr.sortingOrder = _sortingOrder;

            if (breed.controller != null)
            {
                var animator = go.AddComponent<Animator>();
                animator.runtimeAnimatorController = breed.controller;
                animator.enabled = false;
            }

            var cat = go.AddComponent<CatEntity>();
            cat.SetSprites(breed.frontSprite, breed.rightSprite, breed.backSprite);
            cat.Init(_gridRenderer.Data, cell, this);

            _cats.Add(cat);
            return cat;
        }

        public CatEntity GetRandomCat()
        {
            if (_cats.Count == 0) return null;
            return _cats[Random.Range(0, _cats.Count)];
        }

        public CatEntity GetCatAt(Vector2Int pos, CatEntity exclude = null)
        {
            foreach (var cat in _cats)
                if (cat != exclude && cat.GridPos == pos)
                    return cat;
            return null;
        }

        private static void ShuffleList<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
