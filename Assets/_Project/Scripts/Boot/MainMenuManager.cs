using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
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
        private RectTransform _soundToggleRect;
        private RectTransform _parametersRect;
        private RectTransform _creditsRect;
        private RectTransform _logoRect;

        private TMP_Text _soundStateLabel;
        private TMP_Text _versionLabel;

        private ParametersPanel _parametersPanel;
        private bool _isTransitioning;

        // Sound state
        private bool _soundEnabled = true;
        private const string PrefSound = "Param_SoundEnabled";

        private void Start()
        {
            // Find UI elements
            _playRect = FindRect("PlayAction");
            _soundToggleRect = FindRect("SoundToggleAction");
            _parametersRect = FindRect("ParametersAction");
            _creditsRect = FindRect("CreditsAction");

            _soundStateLabel = FindTMP("SoundStateLabel");
            _versionLabel = FindTMP("VersionLabel");
            _logoRect = FindRect("Logo");

            // Animate logo
            AnimateLogo();

            // Add juice to all buttons
            AddJuice(_playRect);
            AddJuice(_soundToggleRect);
            AddJuice(_parametersRect);
            AddJuice(_creditsRect);

            // Version label
            if (_versionLabel != null)
                _versionLabel.text = $"v{Application.version}";

            // Restore sound state
            _soundEnabled = PlayerPrefs.GetInt(PrefSound, 1) == 1;
            ApplySoundState();

            Core.LocalizedStrings.OnLanguageChanged += ApplySoundState;
        }

        private void OnDestroy()
        {
            Core.LocalizedStrings.OnLanguageChanged -= ApplySoundState;
            if (_logoRect != null) _logoRect.DOKill();
        }

        private void Update()
        {
            if (_isTransitioning) return;
            if (_parametersPanel != null && _parametersPanel.IsOpen) return;
            if (_parametersPanel == null)
                _parametersPanel = FindAnyObjectByType<ParametersPanel>();

            var pointer = Pointer.current;
            if (pointer == null || !pointer.press.wasPressedThisFrame) return;

            Vector2 screenPos = pointer.position.ReadValue();

            // Play
            if (_playRect != null &&
                RectTransformUtility.RectangleContainsScreenPoint(_playRect, screenPos, null))
            {
                OnPlayTapped();
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

        private void OnSoundToggleTapped()
        {
            _soundEnabled = !_soundEnabled;
            PlayerPrefs.SetInt(PrefSound, _soundEnabled ? 1 : 0);
            PlayerPrefs.Save();
            ApplySoundState();
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
            // Animation only for now — no credits screen yet
            if (_creditsRect != null)
            {
                _creditsRect.DOKill();
                _creditsRect.localScale = Vector3.one;
                _creditsRect.DOPunchScale(Vector3.one * 0.15f, 0.4f, 6, 0.5f);
            }
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
