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
            // Hide buttons while placing/moving objects
            if (_floorManager == null) return;
            bool busy = (_placement != null && _placement.IsPlacing)
                     || (_mover != null && _mover.IsMoving)
                     || _floorManager.IsTransitioning;
            if (_upObj != null && _upObj.activeSelf == busy && _upObj.activeInHierarchy)
            {
                // keep call cheap — only toggle if state mismatches
            }
            RefreshVisibility();
        }

        private void RefreshVisibility()
        {
            if (_floorManager == null) return;

            bool busy = (_placement != null && _placement.IsPlacing)
                     || (_mover != null && _mover.IsMoving)
                     || _floorManager.IsTransitioning;

            if (_upObj != null)
                _upObj.SetActive(!busy && _floorManager.CanGoUp);
            if (_downObj != null)
                _downObj.SetActive(!busy && _floorManager.CanGoDown);
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
            // Ensure there's a raycast target for the Button to register clicks
            var img = go.GetComponent<Image>();
            if (img == null)
            {
                var anyChildImg = go.GetComponentInChildren<Image>();
                if (anyChildImg != null) btn.targetGraphic = anyChildImg;
            }
            else
            {
                btn.targetGraphic = img;
            }
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
