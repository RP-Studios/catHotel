using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CatHotel.Audio;
using CatHotel.Hotel;

namespace CatHotel.UI
{
    /// <summary>
    /// Wires the FloorUpAction / FloorDownAction Canvas GameObjects to FloorManager.
    /// Shows only the relevant button depending on current floor / unlocked floors.
    /// </summary>
    public class FloorNavigationUI : MonoBehaviour
    {
        [SerializeField] private FloorManager _floorManager;
        [SerializeField] private ObjectPlacement _placement;
        [SerializeField] private ObjectMover _mover;

        private GameObject _upObj;
        private GameObject _downObj;
        private Button _upBtn;
        private Button _downBtn;

        // Cache last applied visibility — avoids SetActive every frame which triggers
        // Canvas rebuilds and can interfere with click detection.
        private bool _lastUpVisible;
        private bool _lastDownVisible;
        private bool _initialized;

        private void Start()
        {
            _upObj = FindInactiveByName("FloorUpAction");
            _downObj = FindInactiveByName("FloorDownAction");

            if (_upObj != null)
            {
                _upBtn = EnsureButton(_upObj);
                _upBtn.onClick.AddListener(OnUpTapped);
            }
            if (_downObj != null)
            {
                _downBtn = EnsureButton(_downObj);
                _downBtn.onClick.AddListener(OnDownTapped);
            }

            if (_floorManager != null)
                _floorManager.OnFloorChanged += _ => RefreshVisibility();

            RefreshVisibility();
        }

        private void Update()
        {
            if (_floorManager == null) return;
            RefreshVisibility();
        }

        private void RefreshVisibility()
        {
            if (_floorManager == null) return;

            bool busy = (_placement != null && _placement.IsPlacing)
                     || (_mover != null && _mover.IsMoving)
                     || _floorManager.IsTransitioning;

            bool upVisible = !busy && _floorManager.CanGoUp;
            bool downVisible = !busy && _floorManager.CanGoDown;

            // Only toggle SetActive when state actually changes — protects click events
            // and avoids per-frame Canvas rebuild churn.
            if (!_initialized || upVisible != _lastUpVisible)
            {
                if (_upObj != null) _upObj.SetActive(upVisible);
                _lastUpVisible = upVisible;
            }
            if (!_initialized || downVisible != _lastDownVisible)
            {
                if (_downObj != null) _downObj.SetActive(downVisible);
                _lastDownVisible = downVisible;
            }
            _initialized = true;
        }

        private void OnUpTapped()
        {
            UISoundManager.Instance?.PlayTapPositive();
            _floorManager?.GoUp();
        }

        private void OnDownTapped()
        {
            UISoundManager.Instance?.PlayTapPositive();
            _floorManager?.GoDown();
        }

        private static Button EnsureButton(GameObject go)
        {
            var btn = go.GetComponent<Button>();
            if (btn == null) btn = go.AddComponent<Button>();
            btn.transition = Selectable.Transition.None;

            // Ensure there's a raycast target covering the WHOLE rect so the user
            // doesn't have to hit the small visible image.
            var img = go.GetComponent<Image>();
            if (img == null)
            {
                img = go.AddComponent<Image>();
                img.color = new Color(0f, 0f, 0f, 0f); // transparent but raycast-receiving
            }
            img.raycastTarget = true;
            btn.targetGraphic = img;

            // Children with images: keep the visual ones non-raycast so taps go to the parent.
            foreach (var childImg in go.GetComponentsInChildren<Image>(true))
                if (childImg != img) childImg.raycastTarget = false;

            return btn;
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

        private static Transform FindInChildren(Transform parent, string name)
        {
            if (parent.name == name) return parent;
            for (int i = 0; i < parent.childCount; i++)
            {
                var found = FindInChildren(parent.GetChild(i), name);
                if (found != null) return found;
            }
            return null;
        }
    }
}
