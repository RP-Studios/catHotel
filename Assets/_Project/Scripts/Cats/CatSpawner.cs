using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using CatHotel.Grid;

namespace CatHotel.Cats
{
    public class CatSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GridRenderer _gridRenderer;

        [Header("Cat Sprites")]
        [SerializeField] private Sprite _frontSprite;
        [SerializeField] private Sprite _rightSprite;
        [SerializeField] private Sprite _backSprite;

        [Header("Animation")]
        [SerializeField] private RuntimeAnimatorController _catAnimController;

        [Header("Spawning")]
        [SerializeField] private int _initialCatCount = 3;
        [SerializeField] private int _sortingOrder = 10;

        private readonly List<CatEntity> _cats = new();

        private void Update()
        {
            var kb = Keyboard.current;
            if (kb == null) return;

            // Press S to spawn cats
            if (kb.sKey.wasPressedThisFrame)
                SpawnInitialCats();
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
            var go = new GameObject($"Cat_{_cats.Count + 1}");
            go.transform.SetParent(transform);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = _frontSprite;
            sr.sortingOrder = _sortingOrder;

            if (_catAnimController != null)
            {
                var animator = go.AddComponent<Animator>();
                animator.runtimeAnimatorController = _catAnimController;
                animator.enabled = false;
            }

            var cat = go.AddComponent<CatEntity>();
            cat.SetSprites(_frontSprite, _rightSprite, _backSprite);
            cat.Init(_gridRenderer.Data, cell);

            _cats.Add(cat);
            return cat;
        }

        public CatEntity GetRandomCat()
        {
            if (_cats.Count == 0) return null;
            return _cats[Random.Range(0, _cats.Count)];
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
