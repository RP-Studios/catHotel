using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CatHotel.Editor
{
    /// <summary>
    /// Builds a stamped LoadingScreen prefab at Assets/_Project/Resources/UI/LoadingScreen.prefab
    /// with the hierarchy the runtime expects (Background / TitleLabel / WoolBall / TipLabel).
    /// Ouvre-le ensuite dans Unity pour peaufiner les positions, sprites et styles.
    /// </summary>
    public static class LoadingScreenPrefabBuilder
    {
        private const string PrefabDir = "Assets/_Project/Resources/UI";
        private const string PrefabPath = PrefabDir + "/LoadingScreen.prefab";

        [MenuItem("Cat Hotel/Build Loading Screen Prefab")]
        public static void Build()
        {
            Directory.CreateDirectory(PrefabDir);

            // --- Root ---
            var root = new GameObject("LoadingScreen",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster),
                typeof(CanvasGroup));

            var canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999;

            var scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.matchWidthOrHeight = 0.5f;

            var cg = root.GetComponent<CanvasGroup>();
            cg.alpha = 1f;
            cg.blocksRaycasts = true;
            cg.interactable = false;

            // --- Background ---
            var bg = CreateChild(root.transform, "Background");
            var bgImg = bg.AddComponent<Image>();
            bgImg.color = new Color(0.12f, 0.10f, 0.08f, 1f);
            Stretch(bg);

            // --- TitleLabel ---
            var title = CreateChild(root.transform, "TitleLabel");
            var titleTmp = title.AddComponent<TextMeshProUGUI>();
            titleTmp.text = "Chargement";
            titleTmp.fontSize = 96f;
            titleTmp.fontStyle = FontStyles.Bold;
            titleTmp.alignment = TextAlignmentOptions.Center;
            titleTmp.color = new Color(1f, 1f, 1f, 0.95f);
            var titleRt = title.GetComponent<RectTransform>();
            titleRt.anchorMin = new Vector2(0f, 0.78f);
            titleRt.anchorMax = new Vector2(1f, 0.92f);
            titleRt.offsetMin = Vector2.zero;
            titleRt.offsetMax = Vector2.zero;

            // --- WoolBall (placeholder Image centered) ---
            var wool = CreateChild(root.transform, "WoolBall");
            var woolImg = wool.AddComponent<Image>();
            woolImg.preserveAspect = true;
            woolImg.color = new Color(1f, 1f, 1f, 1f);
            // Try to auto-wire the existing wool placeholder if present in Resources
            var woolSprite = AssetDatabase.LoadAssetAtPath<Sprite>(
                "Assets/_Project/Resources/UI/LoadingWoolBall.png");
            if (woolSprite != null) woolImg.sprite = woolSprite;
            var woolRt = wool.GetComponent<RectTransform>();
            woolRt.anchorMin = new Vector2(0.5f, 0.5f);
            woolRt.anchorMax = new Vector2(0.5f, 0.5f);
            woolRt.anchoredPosition = Vector2.zero;
            woolRt.sizeDelta = new Vector2(320f, 320f);

            // --- TipLabel ---
            var tip = CreateChild(root.transform, "TipLabel");
            var tipTmp = tip.AddComponent<TextMeshProUGUI>();
            tipTmp.text = "Astuce : caresse tes chats pour les rendre heureux.";
            tipTmp.fontSize = 40f;
            tipTmp.fontStyle = FontStyles.Italic;
            tipTmp.alignment = TextAlignmentOptions.Center;
            tipTmp.color = new Color(1f, 1f, 1f, 0.7f);
            tipTmp.enableWordWrapping = true;
            var tipRt = tip.GetComponent<RectTransform>();
            tipRt.anchorMin = new Vector2(0.1f, 0.12f);
            tipRt.anchorMax = new Vector2(0.9f, 0.22f);
            tipRt.offsetMin = Vector2.zero;
            tipRt.offsetMax = Vector2.zero;

            // --- Save as prefab ---
            PrefabUtility.SaveAsPrefabAsset(root, PrefabPath, out bool success);
            Object.DestroyImmediate(root);

            if (success)
            {
                Debug.Log($"[LoadingScreenPrefabBuilder] Prefab written to {PrefabPath}");
                var asset = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
                EditorGUIUtility.PingObject(asset);
                Selection.activeObject = asset;
            }
            else
            {
                Debug.LogError("[LoadingScreenPrefabBuilder] Failed to save prefab.");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static GameObject CreateChild(Transform parent, string name)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go;
        }

        private static void Stretch(GameObject go)
        {
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
    }
}
