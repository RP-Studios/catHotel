using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CatHotel.Tutorial;
using CatHotel.Core;

namespace CatHotel.Editor
{
    /// <summary>
    /// Stamps the tutorial ScriptableObject + NarrationUI prefab.
    /// Run via Cat Hotel > Build Tutorial Assets.
    /// </summary>
    public static class TutorialSetup
    {
        private const string DataDir   = "Assets/_Project/Data";
        private const string SeqPath   = DataDir + "/TutorialFirstTime.asset";
        private const string NarrRoot  = "Assets/_Project/Animations/Narration";
        private const string PrefabDir = "Assets/_Project/Prefabs/UI";
        private const string PrefabPath = PrefabDir + "/NarrationUI.prefab";

        [MenuItem("Cat Hotel/Build Tutorial Assets")]
        public static void Build()
        {
            Directory.CreateDirectory(DataDir);
            Directory.CreateDirectory(PrefabDir);

            var jasper01 = Load<Sprite>($"{NarrRoot}/JASPER_Exp_01.png");
            var jasper02 = Load<Sprite>($"{NarrRoot}/JASPER_Exp_02.png");
            var bubbleLg = Load<Sprite>($"{NarrRoot}/dialogue_container.png");

            // ---- Sequence SO ----
            var seq = CreateOrLoad<TutorialSequenceData>(SeqPath);
            var so = new SerializedObject(seq);
            var steps = so.FindProperty("steps");
            int idx = 0;

            void Step(string spk, Sprite spr, string txt, TutorialTrigger trig,
                TutorialAction onS = TutorialAction.None, TutorialAction onC = TutorialAction.None,
                float delay = 3f, ObjectCategory reqCat = default)
            {
                steps.arraySize = idx + 1;
                var e = steps.GetArrayElementAtIndex(idx);
                e.FindPropertyRelative("speakerLabelKey").stringValue = spk;
                e.FindPropertyRelative("speakerPortrait").objectReferenceValue = spr;
                e.FindPropertyRelative("textKey").stringValue = txt;
                e.FindPropertyRelative("trigger").enumValueIndex = (int)trig;
                e.FindPropertyRelative("actionOnStart").enumValueIndex = (int)onS;
                e.FindPropertyRelative("actionOnComplete").enumValueIndex = (int)onC;
                e.FindPropertyRelative("delaySeconds").floatValue = delay;
                e.FindPropertyRelative("requiredCategory").enumValueIndex = (int)reqCat;
                idx++;
            }

            string J = "tuto.speaker.jasper";

            // --- Intro ---
            Step(J, jasper01, "tuto.welcome",  TutorialTrigger.WaitForTap);
            Step(J, jasper02, "tuto.explain",  TutorialTrigger.WaitForTap);
            Step(J, jasper01, "tuto.beta",     TutorialTrigger.WaitForTap);
            Step(J, jasper02, "tuto.floors",   TutorialTrigger.WaitForTap);
            Step(J, jasper01, "tuto.coins",    TutorialTrigger.WaitForTap);

            // --- Tour: pension / refuge / exit ---
            Step(J, jasper02, "tuto.pension",  TutorialTrigger.WaitForTap,
                onS: TutorialAction.FocusCameraOnPensionEntrance, onC: TutorialAction.ReleaseCameraFocus);
            Step(J, jasper01, "tuto.refuge",   TutorialTrigger.WaitForTap,
                onS: TutorialAction.FocusCameraOnRefugeEntrance, onC: TutorialAction.ReleaseCameraFocus);
            Step(J, jasper02, "tuto.unhappy",  TutorialTrigger.WaitForTap,
                onS: TutorialAction.FocusCameraOnUnhappyExit, onC: TutorialAction.ReleaseCameraFocus);

            // --- Spawn pension cat ---
            Step("", null, "", TutorialTrigger.Action,
                onS: TutorialAction.SpawnFirstCat, onC: TutorialAction.FocusCameraOnLastSpawnedCat);
            Step(J, jasper01, "tuto.firstcat",     TutorialTrigger.WaitForTap);
            Step(J, jasper02, "tuto.selectcat",    TutorialTrigger.WaitForCatSelected);
            Step(J, jasper01, "tuto.catinfo",      TutorialTrigger.WaitForTap, onC: TutorialAction.ReleaseCameraFocus);
            Step(J, jasper02, "tuto.pensiontime",  TutorialTrigger.WaitForTap);

            // --- Let the game breathe 5s ---
            Step("", null, "", TutorialTrigger.WaitForDelay, delay: 5f);

            // --- Spawn refuge cat (hungry) ---
            Step("", null, "", TutorialTrigger.Action,
                onS: TutorialAction.SpawnRefugeCatHungry, onC: TutorialAction.FocusCameraOnLastSpawnedCat);
            Step(J, jasper01, "tuto.refugecat", TutorialTrigger.WaitForTap);

            // --- Food ---
            Step(J, jasper02, "tuto.buyfood", TutorialTrigger.WaitForObjectPlaced,
                onS: TutorialAction.EnableShopFood, reqCat: ObjectCategory.Food);
            Step(J, jasper01, "tuto.waiteat", TutorialTrigger.WaitForCatServiceUsed);
            Step(J, jasper02, "tuto.collectcoin", TutorialTrigger.WaitForCoinCollected);

            // --- Water (delay 3s + set thirsty) ---
            Step("", null, "", TutorialTrigger.WaitForDelay,
                onC: TutorialAction.SetRefugeCatThirsty, delay: 3f);
            Step(J, jasper02, "tuto.thirsty", TutorialTrigger.WaitForObjectPlaced,
                onS: TutorialAction.EnableShopWater, reqCat: ObjectCategory.Water);
            Step(J, jasper01, "tuto.waitdrink", TutorialTrigger.WaitForCatServiceUsed);

            // --- Sleep ---
            Step("", null, "", TutorialTrigger.WaitForDelay,
                onC: TutorialAction.SetRefugeCatSleepy, delay: 3f);
            Step(J, jasper02, "tuto.sleepy", TutorialTrigger.WaitForObjectPlaced,
                onS: TutorialAction.EnableShopSleep, reqCat: ObjectCategory.Sleep);
            Step(J, jasper01, "tuto.waitsleep", TutorialTrigger.WaitForCatServiceUsed);

            // --- Clean ---
            Step("", null, "", TutorialTrigger.WaitForDelay,
                onC: TutorialAction.SetRefugeCatDirty, delay: 3f);
            Step(J, jasper02, "tuto.dirty", TutorialTrigger.WaitForObjectPlaced,
                onS: TutorialAction.EnableShopClean, reqCat: ObjectCategory.Clean);
            Step(J, jasper01, "tuto.waitclean", TutorialTrigger.WaitForCatServiceUsed);

            // --- Play ---
            Step("", null, "", TutorialTrigger.WaitForDelay,
                onC: TutorialAction.SetRefugeCatBored, delay: 3f);
            Step(J, jasper02, "tuto.bored", TutorialTrigger.WaitForObjectPlaced,
                onS: TutorialAction.EnableShopBalls, reqCat: ObjectCategory.Play);
            Step(J, jasper01, "tuto.waitplay", TutorialTrigger.WaitForCatServiceUsed);

            // --- Fin ---
            Step(J, jasper02, "tuto.complete", TutorialTrigger.WaitForTap,
                onC: TutorialAction.EnableFullShop);

            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(seq);

            // ---- NarrationUI prefab (only created if missing — never overwrites your layout) ----
            if (!System.IO.File.Exists(PrefabPath))
                BuildNarrationPrefab(bubbleLg);
            else
                Debug.Log($"[TutorialSetup] Prefab already exists at {PrefabPath} — skipped (edit it in Unity).");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[TutorialSetup] Tutorial assets built.");
        }

        private static void BuildNarrationPrefab(Sprite bubbleLg)
        {
            // Root: Canvas overlay anchored to bottom 35% of screen
            var root = new GameObject("NarrationUI");
            var canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 800;

            var scaler = root.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.matchWidthOrHeight = 0.5f;
            root.AddComponent<GraphicRaycaster>();

            var cg = root.AddComponent<CanvasGroup>();
            cg.alpha = 1f;

            // Container: anchored at screen bottom
            var container = CreateUI("Container", root.transform);
            var contRt = container.GetComponent<RectTransform>();
            contRt.anchorMin = new Vector2(0f, 0f);
            contRt.anchorMax = new Vector2(1f, 0.30f);
            contRt.offsetMin = Vector2.zero;
            contRt.offsetMax = Vector2.zero;
            container.AddComponent<CanvasGroup>();

            // Dark tint behind dialogue (subtle, not full block)
            var tint = CreateUI("Tint", container.transform);
            var tintImg = tint.AddComponent<Image>();
            tintImg.color = new Color(0f, 0f, 0f, 0.3f);
            tintImg.raycastTarget = false;
            Stretch(tint);

            // Portrait (left, ~25% width)
            var portrait = CreateUI("Portrait", container.transform);
            var portraitImg = portrait.AddComponent<Image>();
            portraitImg.preserveAspect = true;
            portraitImg.raycastTarget = false;
            var pRt = portrait.GetComponent<RectTransform>();
            pRt.anchorMin = new Vector2(0.02f, 0.10f);
            pRt.anchorMax = new Vector2(0.25f, 0.95f);
            pRt.offsetMin = Vector2.zero;
            pRt.offsetMax = Vector2.zero;

            // Speaker label under portrait
            var label = CreateUI("SpeakerLabel", container.transform);
            var labelTmp = label.AddComponent<TextMeshProUGUI>();
            labelTmp.text = "Jasper";
            labelTmp.fontSize = 32f;
            labelTmp.fontStyle = FontStyles.Bold;
            labelTmp.alignment = TextAlignmentOptions.Center;
            labelTmp.color = new Color(1f, 1f, 1f, 0.9f);
            labelTmp.raycastTarget = false;
            var lRt = label.GetComponent<RectTransform>();
            lRt.anchorMin = new Vector2(0.02f, 0.0f);
            lRt.anchorMax = new Vector2(0.25f, 0.12f);
            lRt.offsetMin = Vector2.zero;
            lRt.offsetMax = Vector2.zero;

            // Bubble Large (right side, covers ~70% width)
            var bubbleLgObj = CreateUI("BubbleLarge", container.transform);
            var bubbleLgImg = bubbleLgObj.AddComponent<Image>();
            bubbleLgImg.sprite = bubbleLg;
            bubbleLgImg.type = Image.Type.Sliced;
            bubbleLgImg.preserveAspect = false;
            var blRt = bubbleLgObj.GetComponent<RectTransform>();
            blRt.anchorMin = new Vector2(0.26f, 0.05f);
            blRt.anchorMax = new Vector2(0.98f, 0.95f);
            blRt.offsetMin = Vector2.zero;
            blRt.offsetMax = Vector2.zero;
            // Tap handler on the bubble
            var bubbleBtn = bubbleLgObj.AddComponent<Button>();
            bubbleBtn.transition = Selectable.Transition.None;

            // Dialogue text (inside bubble area, with padding)
            var text = CreateUI("DialogueText", container.transform);
            var textTmp = text.AddComponent<TextMeshProUGUI>();
            textTmp.text = "";
            textTmp.fontSize = 36f;
            textTmp.color = new Color(0.2f, 0.15f, 0.1f, 1f);
            textTmp.alignment = TextAlignmentOptions.TopLeft;
            textTmp.enableWordWrapping = true;
            textTmp.raycastTarget = false;
            textTmp.richText = true;
            var tRt = text.GetComponent<RectTransform>();
            tRt.anchorMin = new Vector2(0.30f, 0.10f);
            tRt.anchorMax = new Vector2(0.95f, 0.90f);
            tRt.offsetMin = Vector2.zero;
            tRt.offsetMax = Vector2.zero;

            // Add NarrationUI component and wire refs via SerializedObject
            var narration = root.AddComponent<NarrationUI>();
            var soN = new SerializedObject(narration);
            soN.FindProperty("_portraitImage").objectReferenceValue   = portraitImg;
            soN.FindProperty("_speakerLabel").objectReferenceValue    = labelTmp;
            soN.FindProperty("_bubbleImage").objectReferenceValue     = bubbleLgImg;
            soN.FindProperty("_dialogueText").objectReferenceValue    = textTmp;
            soN.FindProperty("_containerRect").objectReferenceValue   = contRt;
            soN.ApplyModifiedProperties();

            // Wire Button.onClick → NarrationUI.OnBubbleTapped
            // (Can't use UnityEvent wiring in editor cleanly — NarrationUI will self-wire at runtime.)

            PrefabUtility.SaveAsPrefabAsset(root, PrefabPath, out bool ok);
            Object.DestroyImmediate(root);
            if (ok) Debug.Log($"[TutorialSetup] NarrationUI prefab → {PrefabPath}");
        }

        // ---- Helpers ----

        private static T Load<T>(string path) where T : Object
            => AssetDatabase.LoadAssetAtPath<T>(path);

        private static T CreateOrLoad<T>(string path) where T : ScriptableObject
        {
            var existing = AssetDatabase.LoadAssetAtPath<T>(path);
            if (existing != null) return existing;
            var asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static GameObject CreateUI(string name, Transform parent)
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
