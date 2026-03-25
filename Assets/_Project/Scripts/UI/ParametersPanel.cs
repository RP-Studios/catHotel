using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;

namespace CatHotel.UI
{
    /// <summary>
    /// Manages the "ParametersPanel" overlay.
    /// Opens from OptionsPanel > ParamsOption, slides in from the right.
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

            // Battery Saving toggle
            _batteryRect = FindRect("BatterySavingAction");
            _batteryToggleOn = FindGO("BatteryToggleOn");
            _batteryToggleOff = FindGO("BatteryToggleOff");

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
                    UpdateSliderDrag(screenPos, _musicControl, _musicImageValue, vol => AudioListener.volume = vol);

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
        }

        private void TogglePush()
        {
            if (_pushToggleOn == null || _pushToggleOff == null) return;
            bool wasOn = _pushToggleOn.activeSelf;
            _pushToggleOn.SetActive(!wasOn);
            _pushToggleOff.SetActive(wasOn);
            PlayerPrefs.SetInt(PrefPush, wasOn ? 0 : 1);
            PlayerPrefs.Save();
        }

        private void ToggleBattery()
        {
            if (_batteryToggleOn == null || _batteryToggleOff == null) return;
            bool wasOn = _batteryToggleOn.activeSelf;
            _batteryToggleOn.SetActive(!wasOn);
            _batteryToggleOff.SetActive(wasOn);
            PlayerPrefs.SetInt(PrefBattery, wasOn ? 0 : 1);
            PlayerPrefs.Save();
        }

        private static float _effectsVolume = 1f;

        /// <summary>Current effects volume (0-1). Use this to scale SFX.</summary>
        public static float EffectsVolume => _effectsVolume;

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
            PlayerPrefs.SetFloat(key, t);
            PlayerPrefs.Save();
        }

        private void RestorePrefs()
        {
            // Push notifications (default: on)
            bool pushOn = PlayerPrefs.GetInt(PrefPush, 1) == 1;
            if (_pushToggleOn != null) _pushToggleOn.SetActive(pushOn);
            if (_pushToggleOff != null) _pushToggleOff.SetActive(!pushOn);

            // Battery saving (default: off)
            bool batteryOn = PlayerPrefs.GetInt(PrefBattery, 0) == 1;
            if (_batteryToggleOn != null) _batteryToggleOn.SetActive(batteryOn);
            if (_batteryToggleOff != null) _batteryToggleOff.SetActive(!batteryOn);

            // Music volume (default: 1.0)
            float musicVol = PlayerPrefs.GetFloat(PrefMusic, 1f);
            AudioListener.volume = musicVol;
            RestoreSlider(_musicControl, _musicImageValue, musicVol);

            // Effects volume (default: 1.0)
            float effectsVol = PlayerPrefs.GetFloat(PrefEffects, 1f);
            _effectsVolume = effectsVol;
            RestoreSlider(_effectsControl, _effectsImageValue, effectsVol);
        }

        public void Open()
        {
            if (_panel == null) return;
            _isOpen = true;
            _panelObj.SetActive(true);
            var p = _panel.anchoredPosition;
            p.x = _panelWidth;
            _panel.anchoredPosition = p;
            _slideTween?.Kill();
            _slideTween = _panel.DOAnchorPosX(0f, 0.35f)
                .SetEase(Ease.OutBack)
                .SetUpdate(true);
        }

        public void Close()
        {
            if (_panel == null) return;
            _isOpen = false;
            _isDraggingMusic = false;
            _isDraggingEffects = false;
            _slideTween?.Kill();
            _slideTween = _panel.DOAnchorPosX(_panelWidth, 0.25f)
                .SetEase(Ease.InCubic)
                .SetUpdate(true)
                .OnComplete(() => _panelObj.SetActive(false));
            OnClosed?.Invoke();
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
