using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using CatHotel.Services;
using CatHotel.UI;

namespace CatHotel.Boot
{
    /// <summary>
    /// Handles main menu interactions in the Boot scene.
    /// Finds UI elements by name, adds ButtonJuice, handles taps.
    /// </summary>
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private string _gameSceneName = "Proto";

        private RectTransform _playRect;
        private RectTransform _newGameRect;
        private RectTransform _soundToggleRect;
        private RectTransform _parametersRect;
        private RectTransform _creditsRect;
        private RectTransform _logoRect;

        private TMP_Text _playLabel;
        private TMP_Text _newGameLabel;
        private TMP_Text _soundStateLabel;
        private TMP_Text _versionLabel;

        private bool _hasSave;

        private ParametersPanel _parametersPanel;
        private CreditsPanel _creditsPanel;
        private bool _isTransitioning;
        private CanvasGroup _menuGroup;

        // Sound state
        private bool _soundEnabled = true;
        private const string PrefSound = "Param_SoundEnabled";

        private void Awake()
        {
            // Hide menu BEFORE first render — no flash of default values
            _menuGroup = GetOrAddCanvasGroup();
            _menuGroup.alpha = 0f;
            _menuGroup.interactable = false;
        }

        private IEnumerator Start()
        {
            // Find UI elements
            _playRect = FindRect("PlayAction");
            _newGameRect = FindRect("NewGameAction");
            _soundToggleRect = FindRect("SoundToggleAction");
            _parametersRect = FindRect("ParametersAction");
            _creditsRect = FindRect("CreditsAction");

            _playLabel = FindTMP("PlayLabel");
            _newGameLabel = FindTMP("NewGameLabel");
            _soundStateLabel = FindTMP("SoundStateLabel");
            _versionLabel = FindTMP("VersionLabel");
            _logoRect = FindRect("Logo");

            // Animate logo (visible during loading)
            AnimateLogo();

            // Add juice to all buttons
            AddJuice(_playRect);
            AddJuice(_newGameRect);
            AddJuice(_soundToggleRect);
            AddJuice(_parametersRect);
            AddJuice(_creditsRect);

            // Wait for BootManager to finish loading cloud save + settings
            var boot = FindAnyObjectByType<BootManager>();
            while (boot != null && !boot.IsReady)
                yield return null;

            // --- Everything is loaded, set final values before revealing ---

            // Version label
            if (_versionLabel != null)
                _versionLabel.text = $"v{Application.version}";

            // Sound (from cloud save, fallback to PlayerPrefs for migration)
            if (CloudSaveManager.Instance != null && CloudSaveManager.Instance.IsLoaded
                && CloudSaveManager.Instance.HasPersistedSave)
            {
                _soundEnabled = CloudSaveManager.Instance.Settings.soundEnabled;
            }
            else
            {
                _soundEnabled = PlayerPrefs.GetInt(PrefSound, 1) == 1;
                // Migrate to cloud save
                if (CloudSaveManager.Instance != null)
                {
                    CloudSaveManager.Instance.Settings.soundEnabled = _soundEnabled;
                    CloudSaveManager.Instance.SaveSettings();
                }
            }
            ApplySoundState();

            Core.LocalizedStrings.OnLanguageChanged += ApplySoundState;
            Core.LocalizedStrings.OnLanguageChanged += UpdateMenuLabels;

            // Save state → button visibility + labels
            _hasSave = HasExistingSave();
            UpdateMenuVisibility();
            UpdateMenuLabels();

            // Small delay to let everything stabilize, then smooth reveal
            yield return new WaitForSecondsRealtime(0.6f);
            _menuGroup.interactable = true;
            _menuGroup.DOFade(1f, 0.5f).SetEase(Ease.OutQuad).SetUpdate(true);
        }

        /// <summary>Get or add a CanvasGroup on the root Canvas of this menu.</summary>
        private CanvasGroup GetOrAddCanvasGroup()
        {
            // Try to find the parent Canvas
            var canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                var cg = canvas.GetComponent<CanvasGroup>();
                if (cg == null) cg = canvas.gameObject.AddComponent<CanvasGroup>();
                return cg;
            }
            // Fallback: add on this GameObject
            var group = GetComponent<CanvasGroup>();
            if (group == null) group = gameObject.AddComponent<CanvasGroup>();
            return group;
        }

        private void OnDestroy()
        {
            Core.LocalizedStrings.OnLanguageChanged -= ApplySoundState;
            Core.LocalizedStrings.OnLanguageChanged -= UpdateMenuLabels;
            if (_logoRect != null) _logoRect.DOKill();
            if (_menuGroup != null) _menuGroup.DOKill();
        }

        private void Update()
        {
            if (_isTransitioning) return;
            if (_parametersPanel != null && _parametersPanel.IsOpen) return;
            if (_creditsPanel != null && _creditsPanel.IsOpen) return;
            if (_parametersPanel == null)
                _parametersPanel = FindAnyObjectByType<ParametersPanel>();
            if (_creditsPanel == null)
                _creditsPanel = FindAnyObjectByType<CreditsPanel>();

            var pointer = Pointer.current;
            if (pointer == null || !pointer.press.wasPressedThisFrame) return;

            Vector2 screenPos = pointer.position.ReadValue();

            // Play (continue) — only if save exists
            if (_playRect != null &&
                RectTransformUtility.RectangleContainsScreenPoint(_playRect, screenPos, null))
            {
                if (_hasSave)
                    OnPlayTapped();
                return;
            }

            // New Game
            if (_newGameRect != null && _newGameRect.gameObject.activeSelf &&
                RectTransformUtility.RectangleContainsScreenPoint(_newGameRect, screenPos, null))
            {
                OnNewGameTapped();
                return;
            }

            // Sound toggle
            if (_soundToggleRect != null &&
                RectTransformUtility.RectangleContainsScreenPoint(_soundToggleRect, screenPos, null))
            {
                OnSoundToggleTapped();
                return;
            }

            // Parameters
            if (_parametersRect != null &&
                RectTransformUtility.RectangleContainsScreenPoint(_parametersRect, screenPos, null))
            {
                OnParametersTapped();
                return;
            }

            // Credits
            if (_creditsRect != null &&
                RectTransformUtility.RectangleContainsScreenPoint(_creditsRect, screenPos, null))
            {
                OnCreditsTapped();
                return;
            }
        }

        private void OnPlayTapped()
        {
            _isTransitioning = true;

            // Scale up punch then fade out and load game scene
            if (_playRect != null)
            {
                _playRect.DOKill();
                _playRect.localScale = Vector3.one;
                _playRect.DOPunchScale(Vector3.one * 0.2f, 0.3f, 8, 0.5f)
                    .OnComplete(() => LoadGameScene());
            }
            else
            {
                LoadGameScene();
            }
        }

        private void LoadGameScene()
        {
            LoadingScreen.TransitionTo(_gameSceneName);
        }

        private void OnNewGameTapped()
        {
            _isTransitioning = true;

            // Reset save data
            if (CloudSaveManager.Instance != null)
                CloudSaveManager.Instance.ResetAllData();

            if (_newGameRect != null)
            {
                _newGameRect.DOKill();
                _newGameRect.localScale = Vector3.one;
                _newGameRect.DOPunchScale(Vector3.one * 0.2f, 0.3f, 8, 0.5f)
                    .OnComplete(() => LoadGameScene());
            }
            else
            {
                LoadGameScene();
            }
        }

        /// <summary>Check if a save exists (cloud or local).</summary>
        private static bool HasExistingSave()
        {
            return CloudSaveManager.Instance != null
                && CloudSaveManager.Instance.IsLoaded
                && CloudSaveManager.Instance.HasPersistedSave;
        }

        private static readonly Color DisabledColor = new(1f, 1f, 1f, 0.35f);

        /// <summary>Grey out Continue button if no save exists.</summary>
        private void UpdateMenuVisibility()
        {
            if (_playRect == null) return;

            // Tint all Image + TMP_Text children
            float alpha = _hasSave ? 1f : DisabledColor.a;

            foreach (var img in _playRect.GetComponentsInChildren<Image>(true))
                img.color = new Color(img.color.r, img.color.g, img.color.b, alpha);

            foreach (var tmp in _playRect.GetComponentsInChildren<TMP_Text>(true))
                tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, alpha);

            // Disable ButtonJuice animation when greyed out
            var juice = _playRect.GetComponent<ButtonJuice>();
            if (juice != null) juice.enabled = _hasSave;
        }

        /// <summary>Update button labels with localized text.</summary>
        private void UpdateMenuLabels()
        {
            if (_playLabel != null)
                _playLabel.text = Core.LocalizedStrings.Get("ui.continue");
            if (_newGameLabel != null)
                _newGameLabel.text = Core.LocalizedStrings.Get("ui.new_game");
        }

        private void OnSoundToggleTapped()
        {
            _soundEnabled = !_soundEnabled;
            ApplySoundState();

            if (CloudSaveManager.Instance != null)
            {
                CloudSaveManager.Instance.Settings.soundEnabled = _soundEnabled;
                CloudSaveManager.Instance.SaveSettings();
            }
        }

        private void ApplySoundState()
        {
            AudioListener.volume = _soundEnabled ? 1f : 0f;

            if (_soundStateLabel != null)
                _soundStateLabel.text = _soundEnabled
                    ? Core.LocalizedStrings.Get("sound.on")
                    : Core.LocalizedStrings.Get("sound.off");

            // Sync ParametersPanel sliders if open or cached
            if (_parametersPanel == null)
                _parametersPanel = FindAnyObjectByType<ParametersPanel>();
            if (_parametersPanel != null)
                _parametersPanel.SetGlobalMute(!_soundEnabled);
        }

        private void OnParametersTapped()
        {
            // Lazy find — ParametersPanel may not be available at Start
            if (_parametersPanel == null)
                _parametersPanel = FindAnyObjectByType<ParametersPanel>();
            if (_parametersPanel != null)
                _parametersPanel.Open();
        }

        private void AnimateLogo()
        {
            if (_logoRect == null) return;

            // Entry: scale from 0 with bounce
            _logoRect.localScale = Vector3.zero;
            _logoRect.DOScale(Vector3.one, 0.6f).SetEase(Ease.OutBack, 2f);

            // Slight rotation wobble on entry
            _logoRect.localRotation = Quaternion.Euler(0, 0, -3f);
            _logoRect.DORotate(Vector3.zero, 0.8f).SetEase(Ease.OutElastic, 1f, 0.3f)
                .SetDelay(0.3f);

            // Continuous gentle float: slow bob + tiny breathing scale
            var basePos = _logoRect.anchoredPosition;
            DOTween.Sequence()
                .SetDelay(1f)
                .Append(_logoRect.DOAnchorPosY(basePos.y + 8f, 2f).SetEase(Ease.InOutSine))
                .Append(_logoRect.DOAnchorPosY(basePos.y - 4f, 2f).SetEase(Ease.InOutSine))
                .SetLoops(-1, LoopType.Yoyo);

            DOTween.Sequence()
                .SetDelay(1f)
                .Append(_logoRect.DOScale(1.02f, 2.5f).SetEase(Ease.InOutSine))
                .Append(_logoRect.DOScale(0.98f, 2.5f).SetEase(Ease.InOutSine))
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void OnCreditsTapped()
        {
            // Lazy find — CreditsPanel may not be available at Start
            if (_creditsPanel == null)
                _creditsPanel = FindAnyObjectByType<CreditsPanel>();
            if (_creditsPanel != null)
                _creditsPanel.Open();
        }

        private static RectTransform FindRect(string name)
        {
            var go = FindByName(name);
            return go != null ? go.GetComponent<RectTransform>() : null;
        }

        private static TMP_Text FindTMP(string name)
        {
            var go = FindByName(name);
            return go != null ? go.GetComponent<TMP_Text>() : null;
        }

        private static void AddJuice(RectTransform rt)
        {
            if (rt == null) return;
            if (rt.GetComponent<ButtonJuice>() == null)
                rt.gameObject.AddComponent<ButtonJuice>();
        }

        private static GameObject FindByName(string name)
        {
            var go = GameObject.Find(name);
            if (go != null) return go;

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (!scene.isLoaded) continue;
                foreach (var root in scene.GetRootGameObjects())
                {
                    var found = FindInChildren(root.transform, name);
                    if (found != null) return found.gameObject;
                }
            }
            return null;
        }

        private static Transform FindInChildren(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name) return child;
                var found = FindInChildren(child, name);
                if (found != null) return found;
            }
            return null;
        }
    }
}
