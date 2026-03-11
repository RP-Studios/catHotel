using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using CatHotel.Grid;

namespace CatHotel.Cats
{
    public class BedSpot
    {
        public Vector2Int GridPos;
        public bool Occupied;

        public BedSpot(Vector2Int gridPos) { GridPos = gridPos; }
    }

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

        [Header("Special Cats")]
        [SerializeField] private CatBreed _cleoBreed;
        [SerializeField] private CatBreed _aristoteBreed;

        [Header("Auto Spawn")]
        [SerializeField] private float _spawnInterval = 5f;
        [SerializeField] private int _maxCatsPerEntrance = 5;
        [SerializeField] private int _sortingOrder = 10;

        private readonly List<CatEntity> _cats = new();
        private readonly List<CatEntity> _pensionCats = new();
        private readonly List<CatEntity> _refugeCats = new();
        private readonly List<BedSpot> _bedSpots = new();

        private float _pensionTimer;
        private float _refugeTimer;
        private bool _autoSpawnEnabled = true;

        public RuntimeAnimatorController FightCloudController => _fightCloudController;
        public RuntimeAnimatorController HandPetController => _handPetController;
        public IReadOnlyList<CatEntity> AllCats => _cats;

        private bool _pettingMode;
        private Camera _mainCam;

        public bool PettingMode => _pettingMode;

        private IEnumerator Start()
        {
            // Wait one frame so GridRenderer.Start() fills the central room
            yield return null;

            // Auto-detect bed objects from Decorations hierarchy
            var decoRoot = GameObject.Find("Decorations");
            if (decoRoot != null)
            {
                for (int i = 0; i < decoRoot.transform.childCount; i++)
                {
                    var child = decoRoot.transform.GetChild(i);
                    string n = child.name.ToLowerInvariant();
                    if (n.Contains("bed") || n.Contains("coussin"))
                    {
                        var gp = new Vector2Int(
                            Mathf.FloorToInt(child.position.x),
                            Mathf.FloorToInt(child.position.y));
                        _bedSpots.Add(new BedSpot(gp));
                    }
                }
            }
            Debug.Log($"[CatSpawner] Registered {_bedSpots.Count} bed spots");

            // Spawn unique special cats
            SpawnSpecialCat(_cleoBreed);
            SpawnSpecialCat(_aristoteBreed);
        }

        public BedSpot ClaimNearestBed(Vector2Int from)
        {
            BedSpot best = null;
            float bestDist = float.MaxValue;
            foreach (var bed in _bedSpots)
            {
                if (bed.Occupied) continue;
                float dist = Vector2Int.Distance(from, bed.GridPos);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = bed;
                }
            }
            if (best != null) best.Occupied = true;
            return best;
        }

        public void ReleaseBed(BedSpot bed)
        {
            if (bed != null) bed.Occupied = false;
        }

        public void TogglePetting()
        {
            _pettingMode = !_pettingMode;
        }

        private void Update()
        {
            // Debug: S key still works for manual spawn
            var kb = Keyboard.current;
            if (kb != null && kb.sKey.wasPressedThisFrame)
                SpawnInitialCats();

            if (_pettingMode && Pointer.current != null
                && Pointer.current.press.wasPressedThisFrame)
            {
                TryPetAtPointer();
            }

            if (_autoSpawnEnabled)
                HandleAutoSpawn();
        }

        private void HandleAutoSpawn()
        {
            if (_gridRenderer.Entrances.Count < 2) return;

            _pensionTimer += Time.deltaTime;
            _refugeTimer += Time.deltaTime;

            if (_pensionTimer >= _spawnInterval && _pensionCats.Count < _maxCatsPerEntrance)
            {
                _pensionTimer = 0f;
                SpawnFromEntrance(0, _pensionCats);
            }

            if (_refugeTimer >= _spawnInterval && _refugeCats.Count < _maxCatsPerEntrance)
            {
                _refugeTimer = 0f;
                SpawnFromEntrance(1, _refugeCats);
            }
        }

        private void SpawnFromEntrance(int entranceIndex, List<CatEntity> trackingList)
        {
            var entrance = _gridRenderer.Entrances[entranceIndex];
            var cat = SpawnCat(entrance);
            if (cat == null) return;

            trackingList.Add(cat);

            // Pick a random target in the central room and pathfind there
            var centralCells = _gridRenderer.CentralRoomFloorCells;
            if (centralCells.Count > 0)
            {
                var target = centralCells[Random.Range(0, centralCells.Count)];
                cat.WalkToTarget(target);
            }
        }

        private void TryPetAtPointer()
        {
            if (_mainCam == null) _mainCam = Camera.main;
            if (_mainCam == null) return;

            Vector2 screenPos = Pointer.current.position.ReadValue();
            Vector3 worldPos = _mainCam.ScreenToWorldPoint(screenPos);

            CatEntity nearest = null;
            float bestDist = 1.5f;
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

            int count = Mathf.Min(3, floorCells.Count);
            ShuffleList(floorCells);

            for (int i = 0; i < count; i++)
                SpawnCat(floorCells[i]);

            Debug.Log($"[CatSpawner] Spawned {count} cats on floor cells");
        }

        private void SpawnSpecialCat(CatBreed breed)
        {
            if (breed == null || breed.frontSprite == null) return;

            var floorCells = _gridRenderer.Data.GetAllFloorCells();
            if (floorCells.Count == 0)
            {
                Debug.LogWarning($"[CatSpawner] No floor cells to spawn {breed.name}");
                return;
            }

            var cell = floorCells[Random.Range(0, floorCells.Count)];

            var go = new GameObject($"Cat_{breed.name}");
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

            float scale = Random.Range(0.5f, 0.8f);
            go.transform.localScale = new Vector3(scale, scale, 1f);

            var cat = go.AddComponent<CatEntity>();
            cat.SetSprites(breed.frontSprite, breed.rightSprite, breed.backSprite);
            cat.Init(_gridRenderer.Data, cell, this);

            _cats.Add(cat);
            Debug.Log($"[CatSpawner] Spawned special cat {breed.name} at {cell}");
        }

        public CatEntity SpawnCat(Vector2Int cell)
        {
            if (_breeds == null || _breeds.Length == 0) return null;

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

            // Random scale variation
            float scale = Random.Range(0.5f, 0.8f);
            go.transform.localScale = new Vector3(scale, scale, 1f);

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
