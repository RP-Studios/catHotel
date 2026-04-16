using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatHotel.Cats;
using CatHotel.Hotel;
using CatHotel.Input;
using CatHotel.Services;

namespace CatHotel.Tutorial
{
    /// <summary>
    /// Orchestrates the tutorial sequence. Drives NarrationUI, listens for game-event
    /// triggers, disables irrelevant UI, and persists the current step index.
    /// </summary>
    public class TutorialManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TutorialSequenceData _sequence;
        [SerializeField] private NarrationUI _narrationUI;
        [SerializeField] private HotelManager _hotel;
        [SerializeField] private CameraFocus _cameraFocus;
        [SerializeField] private CatHotel.Grid.GridRenderer _gridRenderer;

        private int _currentStep;
        private bool _running;
        private bool _waitingForAction;
        private CatEntity _lastSpawnedCat;

        // UI elements disabled during the tutorial (restored when done)
        private readonly List<GameObject> _disabledUI = new();

        public static TutorialManager Instance { get; private set; }

        /// <summary>True while the tutorial hasn't finished all steps.</summary>
        public bool IsActive => _running;

        /// <summary>Current step index (for save persistence).</summary>
        public int CurrentStepIndex => _currentStep;

        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// Called by HotelManager after the scene is ready (grid built, breeds loaded).
        /// Resumes from the saved step index or starts at 0.
        /// </summary>
        public void Begin(int resumeFromStep = 0)
        {
            // Auto-find missing refs
            if (_sequence == null)
            {
                _sequence = Resources.Load<TutorialSequenceData>("TutorialFirstTime");
                if (_sequence == null)
                {
                    var all = Resources.FindObjectsOfTypeAll<TutorialSequenceData>();
                    if (all.Length > 0) _sequence = all[0];
                }
            }
            if (_narrationUI == null)
                _narrationUI = FindAnyObjectByType<NarrationUI>(FindObjectsInactive.Include);
            if (_hotel == null)
                _hotel = FindAnyObjectByType<HotelManager>();
            if (_cameraFocus == null)
                _cameraFocus = FindAnyObjectByType<CameraFocus>();
            if (_gridRenderer == null)
                _gridRenderer = FindAnyObjectByType<CatHotel.Grid.GridRenderer>();

            if (_sequence == null || _sequence.StepCount == 0)
            {
                Debug.LogWarning("[Tutorial] No sequence found. Run Cat Hotel > Build Tutorial Assets, then Setup Proto Scene.");
                return;
            }
            if (_narrationUI == null)
            {
                Debug.LogWarning("[Tutorial] NarrationUI not found in scene. Run Cat Hotel > Build Tutorial Assets, then Setup Proto Scene.");
                return;
            }

            _currentStep = Mathf.Clamp(resumeFromStep, 0, _sequence.StepCount);
            if (_currentStep >= _sequence.StepCount) return; // already completed

            _running = true;
            Debug.Log($"[Tutorial] Starting from step {_currentStep}/{_sequence.StepCount}");
            DisableGameUI();
            SubscribeEvents();
            StartCoroutine(RunStep());
        }

        /// <summary>Signal the tutorial that a game event happened (cat selected, object placed, etc.).</summary>
        public void NotifyEvent(TutorialTrigger trigger, object context = null)
        {
            if (!_running || !_waitingForAction) return;
            if (_currentStep >= _sequence.StepCount) return;

            var step = _sequence.steps[_currentStep];
            if (step.trigger != trigger) return;

            // For parameterized triggers, validate context
            if (trigger == TutorialTrigger.WaitForObjectPlaced && context is Core.ObjectCategory cat)
            {
                if (cat != step.requiredCategory) return;
            }

            _waitingForAction = false;
        }

        private IEnumerator RunStep()
        {
            while (_currentStep < _sequence.StepCount)
            {
                var step = _sequence.steps[_currentStep];

                // Execute start action (may spawn cat, lock camera, etc.)
                ExecuteAction(step.actionOnStart);

                // --- Pure Action step (no dialogue, no wait) ---
                if (!step.HasDialogue && step.trigger == TutorialTrigger.Action)
                {
                    ExecuteAction(step.actionOnComplete);
                    AdvanceStep();
                    yield return null;
                    continue;
                }

                // --- Silent delay step ---
                if (!step.HasDialogue && step.trigger == TutorialTrigger.WaitForDelay)
                {
                    yield return new WaitForSeconds(step.delaySeconds);
                    ExecuteAction(step.actionOnComplete);
                    AdvanceStep();
                    continue;
                }

                // --- Dialogue step ---
                if (step.HasDialogue)
                    _narrationUI.Show(step.speakerLabelKey, step.speakerPortrait, step.textKey);

                // --- Wait for trigger ---
                if (step.trigger == TutorialTrigger.WaitForTap)
                {
                    bool tapped = false;
                    void OnTap() => tapped = true;
                    _narrationUI.OnTapAdvance += OnTap;
                    yield return new WaitUntil(() => tapped);
                    _narrationUI.OnTapAdvance -= OnTap;
                }
                else if (step.trigger == TutorialTrigger.WaitForDelay)
                {
                    // Dialogue + delay: show text, wait delay, auto-advance
                    yield return new WaitForSeconds(step.delaySeconds);
                }
                else if (step.trigger != TutorialTrigger.Action)
                {
                    // Game-event trigger (selection, placement, service used, etc.)
                    _waitingForAction = true;
                    EnableUIForStep(step);
                    yield return new WaitUntil(() => !_waitingForAction);
                    DisableGameUI();
                }

                if (step.HasDialogue)
                    _narrationUI.Hide();

                // Execute complete action
                ExecuteAction(step.actionOnComplete);

                AdvanceStep();

                // Small pause between steps for visual breathing
                yield return new WaitForSecondsRealtime(0.3f);
            }

            // Tutorial complete
            _running = false;
            RestoreGameUI();
            ClearShopFilter();
            UnsubscribeEvents();
            Debug.Log("[Tutorial] Complete!");
        }

        private void AdvanceStep()
        {
            _currentStep++;
            SaveStepIndex();
        }

        private void SaveStepIndex()
        {
            if (CloudSaveManager.Instance == null) return;
            CloudSaveManager.Instance.Progression.tutorialStepIndex = _currentStep;
            CloudSaveManager.Instance.SaveProgression();
        }

        // ---- Actions ----

        private CatEntity _refugeCat; // second cat spawned for the tutorial

        private void ExecuteAction(TutorialAction action)
        {
            switch (action)
            {
                case TutorialAction.SpawnFirstCat:
                    _lastSpawnedCat = _hotel != null ? _hotel.SpawnTutorialCat() : null;
                    break;

                case TutorialAction.SpawnRefugeCatHungry:
                    _refugeCat = _hotel != null ? _hotel.SpawnTutorialRefugeCat() : null;
                    _lastSpawnedCat = _refugeCat;
                    // Force hunger critical, everything else comfortable
                    if (_refugeCat != null)
                    {
                        var needs = _refugeCat.GetComponent<CatHotel.Cats.CatNeeds>();
                        needs?.FromArray(new float[] { 10f, 90f, 90f, 90f, 90f });
                    }
                    break;

                case TutorialAction.SetRefugeCatThirsty:
                    SetRefugeCatNeeds(90f, 10f, 90f, 90f, 90f);
                    break;
                case TutorialAction.SetRefugeCatSleepy:
                    SetRefugeCatNeeds(90f, 90f, 10f, 90f, 90f);
                    break;
                case TutorialAction.SetRefugeCatDirty:
                    SetRefugeCatNeeds(90f, 90f, 90f, 90f, 10f);
                    break;
                case TutorialAction.SetRefugeCatBored:
                    SetRefugeCatNeeds(90f, 90f, 90f, 10f, 90f);
                    break;

                case TutorialAction.FocusCameraOnLastSpawnedCat:
                    if (_cameraFocus != null && _lastSpawnedCat != null)
                        _cameraFocus.Focus(_lastSpawnedCat.transform);
                    break;
                case TutorialAction.FocusCameraOnPensionEntrance:
                    FocusOnGridPosition(_gridRenderer?.PensionEntrance ?? default);
                    break;
                case TutorialAction.FocusCameraOnRefugeEntrance:
                    FocusOnGridPosition(_gridRenderer?.RefugeEntrance ?? default);
                    break;
                case TutorialAction.FocusCameraOnUnhappyExit:
                    FocusOnGridPosition(_gridRenderer?.UnhappyExit ?? default);
                    break;
                case TutorialAction.ReleaseCameraFocus:
                    if (_cameraFocus != null) _cameraFocus.Release();
                    break;

                case TutorialAction.EnableShopFood:
                    ApplyShopFilter(CatHotel.UI.ShopCategory.Croquettes);
                    break;
                case TutorialAction.EnableShopWater:
                    AddShopFilter(CatHotel.UI.ShopCategory.Water);
                    break;
                case TutorialAction.EnableShopSleep:
                    AddShopFilter(CatHotel.UI.ShopCategory.Beds, CatHotel.UI.ShopCategory.Pillows);
                    break;
                case TutorialAction.EnableShopClean:
                    AddShopFilter(CatHotel.UI.ShopCategory.Litters);
                    break;
                case TutorialAction.EnableShopBalls:
                    AddShopFilter(CatHotel.UI.ShopCategory.Balls);
                    break;
                case TutorialAction.EnableFullShop:
                    ClearShopFilter();
                    break;
            }
        }

        private void SetRefugeCatNeeds(float hunger, float thirst, float sleep, float play, float clean)
        {
            if (_refugeCat == null) return;
            var needs = _refugeCat.GetComponent<CatHotel.Cats.CatNeeds>();
            needs?.FromArray(new[] { hunger, thirst, sleep, play, clean });
        }

        private void FocusOnGridPosition(Vector2Int gridPos)
        {
            if (_cameraFocus == null) return;
            _cameraFocus.FocusOnPosition(new Vector3(gridPos.x + 0.5f, gridPos.y + 0.5f, 0f));
        }

        // ---- Shop filter ----

        private CatHotel.UI.ShopPanel _shopPanel;
        private readonly HashSet<CatHotel.UI.ShopCategory> _shopFilter = new();

        private CatHotel.UI.ShopPanel GetShop()
        {
            if (_shopPanel == null)
                _shopPanel = FindAnyObjectByType<CatHotel.UI.ShopPanel>();
            return _shopPanel;
        }

        private void ApplyShopFilter(params CatHotel.UI.ShopCategory[] cats)
        {
            _shopFilter.Clear();
            foreach (var c in cats) _shopFilter.Add(c);
            GetShop()?.SetTutorialFilter(_shopFilter);
        }

        private void AddShopFilter(params CatHotel.UI.ShopCategory[] cats)
        {
            foreach (var c in cats) _shopFilter.Add(c);
            GetShop()?.SetTutorialFilter(_shopFilter);
        }

        private void ClearShopFilter()
        {
            _shopFilter.Clear();
            GetShop()?.ClearTutorialFilter();
        }

        // ---- Events from game systems ----

        private void SubscribeEvents()
        {
            // We use a polling approach via NotifyEvent (called by game systems)
            // rather than wiring every event here. Game systems call
            // TutorialManager.Instance?.NotifyEvent(...) at the right moments.
        }

        private void UnsubscribeEvents() { }

        // ---- UI gating ----

        private static readonly string[] ManagedUINames =
        {
            "ShopPanel", "ParametersPanel", "FloorUpAction", "FloorDownAction",
            "ShopAction", "ParameterAction",
        };

        private void DisableGameUI()
        {
            foreach (var name in ManagedUINames)
            {
                var go = FindByName(name);
                if (go != null && go.activeSelf)
                {
                    go.SetActive(false);
                    if (!_disabledUI.Contains(go))
                        _disabledUI.Add(go);
                }
            }
        }

        private void RestoreGameUI()
        {
            foreach (var go in _disabledUI)
                if (go != null) go.SetActive(true);
            _disabledUI.Clear();
        }

        /// <summary>Selectively re-enable UI relevant to the current step.</summary>
        private void EnableUIForStep(TutorialStep step)
        {
            switch (step.trigger)
            {
                case TutorialTrigger.WaitForShopOpened:
                case TutorialTrigger.WaitForObjectPlaced:
                    EnableByName("ShopAction");
                    break;
                case TutorialTrigger.WaitForFloorChanged:
                    EnableByName("FloorUpAction");
                    EnableByName("FloorDownAction");
                    break;
                // Cat selection/petting/coin/service don't need UI buttons — direct in-world.
            }
        }

        private void EnableByName(string name)
        {
            var go = FindByName(name);
            if (go != null) go.SetActive(true);
        }

        // ---- Helpers ----

        private static GameObject FindByName(string name)
        {
            var go = GameObject.Find(name);
            if (go != null) return go;
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                if (!scene.isLoaded) continue;
                foreach (var root in scene.GetRootGameObjects())
                {
                    var found = FindDeep(root.transform, name);
                    if (found != null) return found.gameObject;
                }
            }
            return null;
        }

        private static Transform FindDeep(Transform parent, string name)
        {
            if (parent.name == name) return parent;
            for (int i = 0; i < parent.childCount; i++)
            {
                var found = FindDeep(parent.GetChild(i), name);
                if (found != null) return found;
            }
            return null;
        }
    }
}
