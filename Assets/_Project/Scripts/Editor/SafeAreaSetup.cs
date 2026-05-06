using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using CatHotel.UI;

namespace CatHotel.Editor
{
    /// <summary>
    /// Creates a SafeArea container under the main scene Canvas and reparents
    /// HUD elements into it. Background, fade and loading layers stay siblings
    /// of SafeArea so they continue to cover the full screen.
    /// </summary>
    public static class SafeAreaSetup
    {
        // Direct Canvas children that must STAY full-screen (NOT moved into SafeArea).
        // Match by case-insensitive substring on the GameObject name.
        private static readonly string[] _excludeSubstrings =
        {
            "Fade",
            "Loading",
            "Background",
            "Backdrop",
            "Splash",
        };

        [MenuItem("Cat Hotel/Setup Safe Area")]
        public static void Setup()
        {
            var canvas = FindMainCanvas();
            if (canvas == null)
            {
                EditorUtility.DisplayDialog("Safe Area",
                    "No main ScreenSpaceOverlay Canvas found in the active scene. " +
                    "Open the Hotel / Proto scene and try again.", "OK");
                return;
            }

            var canvasTr = canvas.transform;
            var safeAreaTr = canvasTr.Find("SafeArea") as RectTransform;

            if (safeAreaTr == null)
            {
                var go = new GameObject("SafeArea", typeof(RectTransform));
                int uiLayer = LayerMask.NameToLayer("UI");
                if (uiLayer >= 0) go.layer = uiLayer;
                safeAreaTr = (RectTransform)go.transform;
                safeAreaTr.SetParent(canvasTr, false);
                safeAreaTr.anchorMin = Vector2.zero;
                safeAreaTr.anchorMax = Vector2.one;
                safeAreaTr.offsetMin = Vector2.zero;
                safeAreaTr.offsetMax = Vector2.zero;
                Undo.RegisterCreatedObjectUndo(go, "Create SafeArea");
            }

            var safeAreaGo = safeAreaTr.gameObject;
            if (safeAreaGo.GetComponent<SafeAreaFitter>() == null)
                Undo.AddComponent<SafeAreaFitter>(safeAreaGo);

            // Snapshot current direct children of Canvas (iteration is invalidated by reparent).
            var currentChildren = new List<Transform>(canvasTr.childCount);
            foreach (Transform c in canvasTr) currentChildren.Add(c);

            int moved = 0, skipped = 0;
            foreach (var child in currentChildren)
            {
                if (child == safeAreaTr) continue;
                if (IsExcluded(child.name)) { skipped++; continue; }

                Undo.SetTransformParent(child, safeAreaTr, "Reparent under SafeArea");
                // SetParent appends as last sibling — order is preserved across the loop.
                moved++;
            }

            // SafeArea on top of any background siblings so HUD draws above them.
            // Excluded fade overlays usually call SetAsLastSibling() at runtime, so they
            // still draw above HUD — no need to enforce order here.
            EditorSceneManager.MarkSceneDirty(canvas.gameObject.scene);
            Selection.activeGameObject = safeAreaGo;

            Debug.Log($"[SafeArea] Setup done on '{canvas.name}'. " +
                      $"Reparented {moved} child(ren) under 'SafeArea', skipped {skipped} full-screen layer(s).");
        }

        private static bool IsExcluded(string name)
        {
            foreach (var ex in _excludeSubstrings)
                if (name.IndexOf(ex, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            return false;
        }

        /// <summary>
        /// Heuristic: pick the screen-space-overlay Canvas with the most descendants.
        /// Skips canvases with overrideSorting (NarrationUI bubble, modal popups inside prefabs)
        /// and canvases that are not at scene root (so persistent prefabs don't win).
        /// </summary>
        private static Canvas FindMainCanvas()
        {
            var canvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            Canvas best = null;
            int bestCount = -1;
            foreach (var c in canvases)
            {
                if (c == null) continue;
                if (c.renderMode != RenderMode.ScreenSpaceOverlay) continue;
                if (c.overrideSorting) continue;
                if (c.transform.parent != null) continue; // skip nested canvases

                int count = c.GetComponentsInChildren<Transform>(true).Length;
                if (count > bestCount)
                {
                    bestCount = count;
                    best = c;
                }
            }
            return best;
        }
    }
}
