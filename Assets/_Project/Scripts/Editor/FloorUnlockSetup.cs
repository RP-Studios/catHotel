using System.IO;
using UnityEditor;
using UnityEngine;
using CatHotel.Hotel;

namespace CatHotel.Editor
{
    /// <summary>
    /// Stamps Assets/_Project/Data/FloorUnlockConfig.asset with the agreed
    /// progression: rep gates, costs, and capacities. Re-runnable; overwrites.
    /// </summary>
    public static class FloorUnlockSetup
    {
        private const string DataDir = "Assets/_Project/Data";
        private const string AssetPath = DataDir + "/FloorUnlockConfig.asset";

        [MenuItem("Cat Hotel/Build Floor Unlock Config")]
        public static void Build()
        {
            Directory.CreateDirectory(DataDir);

            var config = AssetDatabase.LoadAssetAtPath<FloorUnlockConfig>(AssetPath);
            if (config == null)
            {
                config = ScriptableObject.CreateInstance<FloorUnlockConfig>();
                AssetDatabase.CreateAsset(config, AssetPath);
            }

            var so = new SerializedObject(config);
            var arr = so.FindProperty("floors");
            arr.arraySize = 6;

            // (floor, rep, cost, capacity)
            var entries = new (int, int, int, int)[]
            {
                (0, 0, 0,       5),   // RDC — always unlocked
                (1, 1, 750,    10),
                (2, 3, 4000,   16),
                (3, 5, 18000,  24),
                (4, 7, 60000,  32),
                (5, 9, 120000, 40),
            };

            for (int i = 0; i < entries.Length; i++)
            {
                var (floor, rep, cost, cap) = entries[i];
                var e = arr.GetArrayElementAtIndex(i);
                e.FindPropertyRelative("floorIndex").intValue = floor;
                e.FindPropertyRelative("requiredReputationLevel").intValue = rep;
                e.FindPropertyRelative("catCoinCost").intValue = cost;
                e.FindPropertyRelative("totalCapacity").intValue = cap;
            }

            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[FloorUnlockSetup] Wrote {AssetPath}");
            EditorGUIUtility.PingObject(config);
            Selection.activeObject = config;
        }

        // Debug helper
        [MenuItem("Cat Hotel/Debug/Unlock Next Floor (in play mode)")]
        public static void DebugUnlockNext()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[FloorUnlockSetup] Enter Play mode first.");
                return;
            }
            var prog = FloorProgression.Instance;
            if (prog == null) { Debug.LogWarning("No FloorProgression in scene."); return; }
            int next = prog.NextLockedFloor;
            if (next < 0) { Debug.Log("All floors unlocked."); return; }
            // Force unlock for debug (skips rep + coin checks)
            var ctx = new SerializedObject(prog);
            // Use the built-in debug context-menu via reflection? simpler: just call TryUnlock (will fail if rep/coins missing)
            var res = prog.TryUnlock(next);
            Debug.Log($"[FloorUnlockSetup] TryUnlock floor {next} → {res}");
        }
    }
}
