using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;
using CatHotel.Audio;
using CatHotel.Services;

namespace CatHotel.UI
{
    /// <summary>
    /// Manages the "ParametersPanel" overlay.
    /// Music and Effects volumes are independent:
    ///   - Music slider controls MusicVolume (applied to any AudioSource tagged as music)
    ///   - Effects slider controls EffectsVolume (used by SFX PlayOneShot calls)
    ///   - AudioListener.volume stays at 1.0 always
    /// </summary>
    public class ParametersPanel : MonoBehaviour
    {
        private RectTransform _panel;
        private GameObject _panelObj;
        private RectTransform _closeRect;
        private float _panelWidth;
        private Tween _slideTween;
        private bool _isOpen;

        // Toggle rects
        private RectTransform _pushNotifRect;
        private GameObject _pushToggleOn;
        private GameObject _pushToggleOff;

        private RectTransform _batteryRect;
        private GameObject _batteryToggleOn;
        private GameObject _batteryToggleOff;

        // Language toggles
        private RectTransform _enActiveRect;
        private RectTransform _enInactiveRect;
        private RectTransform _frActiveRect;
        private RectTransform _frInactiveRect;
        private RectTransform _deActiveRect;
        private RectTransform _deInactiveRect;
        private RectTransform _esActiveRect;
        private RectTransform _esInactiveRect;
        private RectTransform _ptActiveRect;
        private RectTransform _ptInactiveRect;

        // Music slider
        private RectTransform _musicControl;
        private RectTransform _musicImageValue;
        private bool _isDraggingMusic;

        // Effects slider
        private RectTransform _effectsControl;
        private RectTransform _effectsImageValue;
        private bool _isDraggingEffects;

        private const float SliderMinX = -167f;
        private const float SliderMaxX = 167f;
        private const float BarRightMin = 2.7f;   // full volume
        private const float BarRightMax = 397f;    // muted

        // Prefs keys
        private const string PrefPush = "Param_PushNotif";
        private const string PrefBattery = "Param_BatterySaving";
        private const string PrefEffects = "Param_EffectsVolume";
        private const string PrefMusic = "Param_MusicVolume";

        public bool IsOpen => _isOpen;
        public System.Action OnClosed;

        // --- Static volumes ---
        private static float _musicVolume = 1f;
        private static float _effectsVolume = 1f;

        /// <summary>Current music volume (0-1). Apply to music AudioSources.</summary>
        public static float MusicVolume => _musicVolume;

        /// <summary>Current effects volume (0-1). Use this to scale SFX.</summary>
        public static float EffectsVolume => _effectsVolume;

        /// <summary>
        /// Event fired when music volume changes. Music players should subscribe.
        /// </summary>
        public static event System.Action<float> OnMusicVolumeChanged;

        // --- Battery saving ---
        private static bool _batterySaving;

        /// <summary>Current battery saving state. True = caps fps and slows passive anims.</summary>
        public static bool BatterySaving => _batterySaving;

        /// <summary>Fired when battery saving toggle changes. Subscribers apply their own effects.</summary>
        public static event System.Action<bool> OnBatterySavingChanged;

        // Saved volumes before mute (to restore on unmute)
        private float _preMuteMusic = 1f;
        private float _preMuteEffects = 1f;
        private bool _isMuted;

        /// <summary>
        /// Mute/unmute both sliders. Called by MainMenuManager sound toggle.
        /// </summary>
        public void SetGlobalMute(bool muted)
        {
            if (muted && !_isMuted)
            {
                _preMuteMusic = _musicVolume;
                _preMuteEffects = _effectsVolume;
                SetMusicVolume(0f);
                _effectsVolume = 0f;
                RestoreSlider(_musicControl, _musicImageValue, 0f);
                RestoreSlider(_effectsControl, _effectsImageValue, 0f);
                SaveVolumePref(PrefMusic, _musicControl);
                SaveVolumePref(PrefEffects, _effectsControl);
            }
            else if (!muted && _isMuted)
            {
                SetMusicVolume(_preMuteMusic);
                _effectsVolume = _preMuteEffects;
                RestoreSlider(_musicControl, _musicImageValue, _preMuteMusic);
                RestoreSlider(_effectsControl, _effectsImageValue, _preMuteEffects);
                SaveVolumePref(PrefMusic, _musicControl);
                SaveVolumePref(PrefEffects, _effectsControl);
            }
            _isMuted = muted;
        }

        private void Start()
        {
            _panelObj = FindInactiveByName("ParametersPanel");
            if (_panelObj == null) return;

            _panel = _panelObj.GetComponent<RectTransform>();

            _panelObj.SetActive(true);

            var panelImg = _panelObj.GetComponent<Image>();
            if (panelImg == null)
            {
                panelImg = _panelObj.AddComponent<Image>();
                panelImg.color = Color.clear;
            }
            panelImg.raycastTarget = true;

            Canvas.ForceUpdateCanvases();
            _panelWidth = _panel.rect.width;
            if (_panelWidth <= 0f) _panelWidth = 800f;

            var pos = _panel.anchoredPosition;
            pos.x = _panelWidth;
            _panel.anchoredPosition = pos;
            _panelObj.SetActive(false);

            // CloseImage
            var closeTransform = FindInChildren(_panelObj.transform, "CloseImage");
            if (closeTransform != null)
            {
                _closeRect = closeTransform.GetComponent<RectTransform>();
                if (closeTransform.GetComponent<ButtonJuice>() == null)
                    closeTransform.gameObject.AddComponent<ButtonJuice>();
            }

            // Push Notification toggle
            _pushNotifRect = FindRect("PushNotificationAction");
            _pushToggleOn = FindGO("PushToggleOn");
            _pushToggleOff = FindGO("PushToggleOff");

            // Battery Saving toggle — default OFF until LoadSavedValues overrides
            _batteryRect = FindRect("BatterySavingAction");
            _batteryToggleOn = FindGO("BatteryToggleOn");
            _batteryToggleOff = FindGO("BatteryToggleOff");
            if (_batteryToggleOn != null) _batteryToggleOn.SetActive(false);
            if (_batteryToggleOff != null) _batteryToggleOff.SetActive(true);

            // Language toggles
            _enActiveRect = FindRect("ENActiveAction");
            _enInactiveRect = FindRect("ENInactiveAction");
            _frActiveRect = FindRect("FRActiveAction");
            _frInactiveRect = FindRect("FRInactiveAction");
            _deActiveRect = FindRect("DEActiveAction");
            _deInactiveRect = FindRect("DEInactiveAction");
            _esActiveRect = FindRect("ESActiveAction");
            _esInactiveRect = FindRect("ESInactiveAction");
            _ptActiveRect = FindRect("PTActiveAction");
            _ptInactiveRect = FindRect("PTInactiveAction");
            AddJuice(_enActiveRect);
            AddJuice(_enInactiveRect);
            AddJuice(_frActiveRect);
            AddJuice(_frInactiveRect);
            AddJuice(_deActiveRect);
            AddJuice(_deInactiveRect);
            AddJuice(_esActiveRect);
            AddJuice(_esInactiveRect);
            AddJuice(_ptActiveRect);
            AddJuice(_ptInactiveRect);
            Debug.Log($"[Params] Lang buttons: EN={_enActiveRect != null}/{_enInactiveRect != null}, FR={_frActiveRect != null}/{_frInactiveRect != null}, DE={_deActiveRect != null}/{_deInactiveRect != null}, ES={_esActiveRect != null}/{_esInactiveRect != null}, PT={_ptActiveRect != null}/{_ptInactiveRect != null}");

            // Music slider
            var musicT = FindInChildren(_panelObj.transform, "MusicControl");
            if (musicT != null) _musicControl = musicT.GetComponent<RectTransform>();
            var musicBarT = FindInChildren(_panelObj.transform, "MusicImageValue");
            if (musicBarT != null) _musicImageValue = musicBarT.GetComponent<RectTransform>();

            // Effects slider
            var effectsT = FindInChildren(_panelObj.transform, "EffectsControl");
            if (effectsT != null) _effectsControl = effectsT.GetComponent<RectTransform>();
            var effectsBarT = FindInChildren(_panelObj.transform, "EffectsImageValue");
            if (effectsBarT != null) _effectsImageValue = effectsBarT.GetComponent<RectTransform>();

            // Ensure AudioListener.volume is always 1 (never touch it)
            AudioListener.volume = 1f;

            // Restore saved state
            RestorePrefs();
        }

        private void Update()
        {
            if (!_isOpen) return;

            var pointer = Pointer.current;
            if (pointer == null) return;

            Vector2 screenPos = pointer.position.ReadValue();

            // Music drag
            if (_musicControl != null)
            {
                if (pointer.press.wasPressedThisFrame &&
                    RectTransformUtility.RectangleContainsScreenPoint(_musicControl, screenPos, null))
                    _isDraggingMusic = true;

                if (_isDraggingMusic && pointer.press.isPressed)
                    UpdateSliderDrag(screenPos, _musicControl, _musicImageValue, SetMusicVolume);

                if (_isDraggingMusic && pointer.press.wasReleasedThisFrame)
                {
                    _isDraggingMusic = false;
                    SaveVolumePref(PrefMusic, _musicControl);
                }
            }

            // Effects drag
            if (_effectsControl != null)
            {
                if (pointer.press.wasPressedThisFrame &&
                    RectTransformUtility.RectangleContainsScreenPoint(_effectsControl, screenPos, null))
                    _isDraggingEffects = true;

                if (_isDraggingEffects && pointer.press.isPressed)
                    UpdateSliderDrag(screenPos, _effectsControl, _effectsImageValue, vol => _effectsVolume = vol);

                if (_isDraggingEffects && pointer.press.wasReleasedThisFrame)
                {
                    _isDraggingEffects = false;
                    SaveVolumePref(PrefEffects, _effectsControl);
                }
            }

            if (!pointer.press.wasPressedThisFrame) return;

            // Close
            if (_closeRect != null &&
                RectTransformUtility.RectangleContainsScreenPoint(_closeRect, screenPos, null))
            {
                Close();
                return;
            }

            // Push toggle
            if (_pushNotifRect != null &&
                RectTransformUtility.RectangleContainsScreenPoint(_pushNotifRect, screenPos, null))
            {
                TogglePush();
                return;
            }

            // Battery toggle
            if (_batteryRect != null &&
                RectTransformUtility.RectangleContainsScreenPoint(_batteryRect, screenPos, null))
            {
                ToggleBattery();
                return;
            }

            // Language: tap any language button that is currently active (visible) → no-op
            if (IsTapOn(_enActiveRect, screenPos)) return; // already EN
            if (IsTapOn(_frActiveRect, screenPos)) return; // already FR
            if (IsTapOn(_deActiveRect, screenPos)) return; // already DE
            if (IsTapOn(_esActiveRect, screenPos)) return; // already ES
            if (IsTapOn(_ptActiveRect, screenPos)) return; // already PT
            // Tap on an inactive language → switch to it
            if (IsTapOn(_enInactiveRect, screenPos)) { SetLanguage("en"); return; }
            if (IsTapOn(_frInactiveRect, screenPos)) { SetLanguage("fr"); return; }
            if (IsTapOn(_deInactiveRect, screenPos)) { SetLanguage("de"); return; }
            if (IsTapOn(_esInactiveRect, screenPos)) { SetLanguage("es"); return; }
            if (IsTapOn(_ptInactiveRect, screenPos)) { SetLanguage("pt"); return; }
        }

        private static bool IsTapOn(RectTransform rect, Vector2 screenPos)
        {
            return rect != null && rect.gameObject.activeSelf &&
                   RectTransformUtility.RectangleContainsScreenPoint(rect, screenPos, null);
        }

        private static void SetMusicVolume(float vol)
        {
            _musicVolume = vol;
            OnMusicVolumeChanged?.Invoke(vol);
        }

        private void SetLanguage(string langCode)
        {
            CatHotel.Core.LocalizedStrings.SetLanguage(langCode);
            UpdateLanguageToggles();
        }

        private void UpdateLanguageToggles()
        {
            string lang = CatHotel.Core.LocalizedStrings.CurrentLanguage;
            SetLangToggle(_enActiveRect, _enInactiveRect, lang == "en");
            SetLangToggle(_frActiveRect, _frInactiveRect, lang == "fr");
            SetLangToggle(_deActiveRect, _deInactiveRect, lang == "de");
            SetLangToggle(_esActiveRect, _esInactiveRect, lang == "es");
            SetLangToggle(_ptActiveRect, _ptInactiveRect, lang == "pt");
        }

        private static void SetLangToggle(RectTransform activeRect, RectTransform inactiveRect, bool isCurrent)
        {
            if (activeRect != null) activeRect.gameObject.SetActive(isCurrent);
            if (inactiveRect != null) inactiveRect.gameObject.SetActive(!isCurrent);
        }

        private void TogglePush()
        {
            if (_pushToggleOn == null || _pushToggleOff == null) return;
            bool wasOn = _pushToggleOn.activeSelf;
            _pushToggleOn.SetActive(!wasOn);
            _pushToggleOff.SetActive(wasOn);

            if (CloudSaveManager.Instance != null)
            {
                CloudSaveManager.Instance.Settings.pushNotifications = !wasOn;
                CloudSaveManager.Instance.SaveSettings();
            }
        }

        private void ToggleBattery()
        {
            if (_batteryToggleOn == null || _batteryToggleOff == null) return;
            bool wasOn = _batteryToggleOn.activeSelf;
            _batteryToggleOn.SetActive(!wasOn);
            _batteryToggleOff.SetActive(wasOn);

            _batterySaving = !wasOn;
            OnBatterySavingChanged?.Invoke(_batterySaving);

            if (CloudSaveManager.Instance != null)
            {
                CloudSaveManager.Instance.Settings.batterySaving = _batterySaving;
                CloudSaveManager.Instance.SaveSettings();
            }
        }

        private void UpdateSliderDrag(Vector2 screenPos, RectTransform control,
            RectTransform bar, System.Action<float> applyVolume)
        {
            var parent = control.parent as RectTransform;
            if (parent == null) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parent, screenPos, null, out var localPos);

            float clampedX = Mathf.Clamp(localPos.x, SliderMinX, SliderMaxX);
            var ap = control.anchoredPosition;
            ap.x = clampedX;
            control.anchoredPosition = ap;

            float t = Mathf.InverseLerp(SliderMinX, SliderMaxX, clampedX);

            if (bar != null)
            {
                var offset = bar.offsetMax;
                offset.x = -Mathf.Lerp(BarRightMax, BarRightMin, t);
                bar.offsetMax = offset;
            }

            applyVolume?.Invoke(t);
        }

        private static void RestoreSlider(RectTransform control, RectTransform bar, float vol)
        {
            if (control != null)
            {
                var ap = control.anchoredPosition;
                ap.x = Mathf.Lerp(SliderMinX, SliderMaxX, vol);
                control.anchoredPosition = ap;
            }
            if (bar != null)
            {
                var offset = bar.offsetMax;
                offset.x = -Mathf.Lerp(BarRightMax, BarRightMin, vol);
                bar.offsetMax = offset;
            }
        }

        private static void SaveVolumePref(string key, RectTransform control)
        {
            float t = 0f;
            if (control != null)
                t = Mathf.InverseLerp(SliderMinX, SliderMaxX, control.anchoredPosition.x);

            if (CloudSaveManager.Instance != null)
            {
                if (key == PrefMusic)
                    CloudSaveManager.Instance.Settings.musicVolume = t;
                else if (key == PrefEffects)
                    CloudSaveManager.Instance.Settings.effectsVolume = t;
                CloudSaveManager.Instance.SaveSettings();
            }
        }

        private void RestorePrefs()
        {
            var settings = CloudSaveManager.Instance?.Settings;

            if (settings != null && CloudSaveManager.Instance.IsLoaded)
            {
                // Push notifications
                bool pushOn = settings.pushNotifications;
                if (_pushToggleOn != null) _pushToggleOn.SetActive(pushOn);
                if (_pushToggleOff != null) _pushToggleOff.SetActive(!pushOn);

                // Battery saving
                bool batteryOn = settings.batterySaving;
                if (_batteryToggleOn != null) _batteryToggleOn.SetActive(batteryOn);
                if (_batteryToggleOff != null) _batteryToggleOff.SetActive(!batteryOn);
                _batterySaving = batteryOn;
                OnBatterySavingChanged?.Invoke(batteryOn);

                // Music volume
                SetMusicVolume(settings.musicVolume);
                RestoreSlider(_musicControl, _musicImageValue, settings.musicVolume);

                // Effects volume
                _effectsVolume = settings.effectsVolume;
                RestoreSlider(_effectsControl, _effectsImageValue, settings.effectsVolume);
            }
            else
            {
                // Fallback to PlayerPrefs for migration / first launch
                bool pushOn = PlayerPrefs.GetInt(PrefPush, 1) == 1;
                if (_pushToggleOn != null) _pushToggleOn.SetActive(pushOn);
                if (_pushToggleOff != null) _pushToggleOff.SetActive(!pushOn);

                bool batteryOn = PlayerPrefs.GetInt(PrefBattery, 0) == 1;
                if (_batteryToggleOn != null) _batteryToggleOn.SetActive(batteryOn);
                if (_batteryToggleOff != null) _batteryToggleOff.SetActive(!batteryOn);
                _batterySaving = batteryOn;
                OnBatterySavingChanged?.Invoke(batteryOn);

                float musicVol = PlayerPrefs.GetFloat(PrefMusic, 1f);
                SetMusicVolume(musicVol);
                RestoreSlider(_musicControl, _musicImageValue, musicVol);

                float effectsVol = PlayerPrefs.GetFloat(PrefEffects, 1f);
                _effectsVolume = effectsVol;
                RestoreSlider(_effectsControl, _effectsImageValue, effectsVol);

                // Migrate to cloud save
                if (CloudSaveManager.Instance != null)
                {
                    CloudSaveManager.Instance.Settings.pushNotifications = pushOn;
                    CloudSaveManager.Instance.Settings.batterySaving = batteryOn;
                    CloudSaveManager.Instance.Settings.musicVolume = musicVol;
                    CloudSaveManager.Instance.Settings.effectsVolume = effectsVol;
                    CloudSaveManager.Instance.SaveSettings();
                }
            }

            // Language toggles
            UpdateLanguageToggles();
        }

        public void Open()
        {
            if (_panel == null) return;
            _isOpen = true;
            UISoundManager.Instance?.PlayOpenSection();
            _panelObj.SetActive(true);
            var p = _panel.anchoredPosition;
            p.x = _panelWidth;
            _panel.anchoredPosition = p;
            _slideTween?.Kill();
            _slideTween = _panel.DOAnchorPosX(0f, 0.7f)
                .SetEase(Ease.OutCubic)
                .SetUpdate(true);
        }

        public void Close()
        {
            if (_panel == null) return;
            _isOpen = false;
            UISoundManager.Instance?.PlayCloseSection();
            _isDraggingMusic = false;
            _isDraggingEffects = false;
            _slideTween?.Kill();
            _slideTween = _panel.DOAnchorPosX(_panelWidth, 0.5f)
                .SetEase(Ease.InCubic)
                .SetUpdate(true)
                .OnComplete(() => _panelObj.SetActive(false));
            OnClosed?.Invoke();
        }

        private static void AddJuice(RectTransform rt)
        {
            if (rt == null) return;
            if (rt.GetComponent<ButtonJuice>() == null)
                rt.gameObject.AddComponent<ButtonJuice>();
        }

        private RectTransform FindRect(string name)
        {
            var t = FindInChildren(_panelObj.transform, name);
            return t != null ? t.GetComponent<RectTransform>() : null;
        }

        private GameObject FindGO(string name)
        {
            var t = FindInChildren(_panelObj.transform, name);
            return t != null ? t.gameObject : null;
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

        private static GameObject FindInactiveByName(string name)
        {
            var go = GameObject.Find(name);
            if (go != null) return go;

            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                if (!scene.isLoaded) continue;
                foreach (var root in scene.GetRootGameObjects())
                {
                    var found = FindInChildren(root.transform, name);
                    if (found != null) return found.gameObject;
                }
            }
            return null;
        }
    }
}
