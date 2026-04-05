using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using CatHotel.Audio;

namespace CatHotel.Editor
{
    public class SoundTesterWindow : EditorWindow
    {
        private static readonly string[] AudioExtensions = { ".wav", ".mp3", ".ogg", ".aif", ".aiff" };
        private static readonly string SearchRoot = "Assets/_Project";
        private static readonly string SettingsPath = "Assets/_Project/Audio/SoundTesterSettings.json";

        [Serializable]
        private class SavedEntryData
        {
            public string path;
            public float volume = 1f;
            public bool loop;
        }

        [Serializable]
        private class SavedSettings
        {
            public float masterVolume = 1f;
            public List<SavedEntryData> entries = new();
        }

        private bool _dirty;

        private List<AudioEntry> _entries = new();
        private Vector2 _scrollPos;
        private string _searchFilter = "";
        private float _masterVolume = 1f;

        // Runtime: root GO + child GOs (one per playing entry, each with AudioSource + AudioAmplifier)
        private GameObject _runtimeGO;
        private Dictionary<AudioEntry, GameObject> _activeNodes = new();

        // Editor playback (Edit mode) — single clip only via AudioUtil
        private static readonly Type AudioUtilType;
        private static readonly MethodInfo PlayClipMethod;
        private static readonly MethodInfo StopClipsMethod;
        private static readonly MethodInfo IsPlayingMethod;
        private AudioEntry _editorPlayingEntry;

        static SoundTesterWindow()
        {
            AudioUtilType = typeof(AudioImporter).Assembly.GetType("UnityEditor.AudioUtil");
            if (AudioUtilType == null) return;
            PlayClipMethod = AudioUtilType.GetMethod("PlayPreviewClip", BindingFlags.Static | BindingFlags.Public);
            StopClipsMethod = AudioUtilType.GetMethod("StopAllPreviewClips", BindingFlags.Static | BindingFlags.Public);
            IsPlayingMethod = AudioUtilType.GetMethod("IsPreviewClipPlaying", BindingFlags.Static | BindingFlags.Public);
        }

        private class AudioEntry
        {
            public string Path;
            public string FileName;
            public string Category;
            public AudioClip Clip;
            public float Volume = 1f;
            public bool Loop;
        }

        [MenuItem("Cat Hotel/Sound Tester", false, 20)]
        public static void ShowWindow()
        {
            var window = GetWindow<SoundTesterWindow>("Sound Tester");
            window.minSize = new Vector2(500, 300);
            window.RefreshAudioList();
        }

        private void OnEnable()
        {
            RefreshAudioList();
            EditorApplication.update += EditorUpdate;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        private void OnDisable()
        {
            EditorApplication.update -= EditorUpdate;
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            if (_dirty) SaveSettings();
            StopAll();
            DestroyRuntime();
        }

        private void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                _activeNodes.Clear();
                _runtimeGO = null;
                _editorPlayingEntry = null;
            }
        }

        private void EnsureRuntime()
        {
            if (_runtimeGO != null) return;

            _runtimeGO = new GameObject("[SoundTester]");

            if (UnityEngine.Object.FindFirstObjectByType<AudioListener>() == null)
                _runtimeGO.AddComponent<AudioListener>();
        }

        private void DestroyRuntime()
        {
            if (_runtimeGO != null)
                DestroyImmediate(_runtimeGO);
            _runtimeGO = null;
            _activeNodes.Clear();
        }

        private bool UseRuntime => Application.isPlaying;

        private void PlayEntry(AudioEntry entry)
        {
            if (UseRuntime)
            {
                if (_activeNodes.ContainsKey(entry)) return;

                EnsureRuntime();

                // Each entry gets its own child GO so OnAudioFilterRead works per-source
                var child = new GameObject($"[ST] {entry.FileName}");
                child.transform.SetParent(_runtimeGO.transform);

                var source = child.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.spatialBlend = 0f;
                source.clip = entry.Clip;
                source.loop = entry.Loop;
                // Keep AudioSource.volume at 1; amplification is done by AudioAmplifier
                source.volume = Mathf.Min(entry.Volume * _masterVolume, 1f);

                var amp = child.AddComponent<AudioAmplifier>();
                amp.gain = entry.Volume * _masterVolume;

                source.Play();
                _activeNodes[entry] = child;
            }
            else
            {
                // Edit mode: single clip only (AudioUtil limitation)
                StopClipsMethod?.Invoke(null, null);
                PlayClipMethod?.Invoke(null, new object[] { entry.Clip, 0, entry.Loop });
                _editorPlayingEntry = entry;
            }
        }

        private void StopEntry(AudioEntry entry)
        {
            if (UseRuntime)
            {
                if (_activeNodes.TryGetValue(entry, out var node))
                {
                    if (node != null) DestroyImmediate(node);
                    _activeNodes.Remove(entry);
                }
            }
            else
            {
                if (_editorPlayingEntry == entry)
                {
                    StopClipsMethod?.Invoke(null, null);
                    _editorPlayingEntry = null;
                }
            }
        }

        private void StopAll()
        {
            if (UseRuntime)
            {
                foreach (var kv in _activeNodes)
                {
                    if (kv.Value != null)
                        DestroyImmediate(kv.Value);
                }
                _activeNodes.Clear();
            }

            StopClipsMethod?.Invoke(null, null);
            _editorPlayingEntry = null;
        }

        private bool IsEntryPlaying(AudioEntry entry)
        {
            if (UseRuntime)
            {
                if (!_activeNodes.TryGetValue(entry, out var node) || node == null) return false;
                var src = node.GetComponent<AudioSource>();
                return src != null && src.isPlaying;
            }

            if (_editorPlayingEntry == entry && IsPlayingMethod != null)
                return (bool)IsPlayingMethod.Invoke(null, null);

            return false;
        }

        private bool HasAnyPlaying()
        {
            if (UseRuntime)
                return _activeNodes.Count > 0;

            return _editorPlayingEntry != null;
        }

        private void ApplyVolume(AudioEntry entry)
        {
            if (UseRuntime && _activeNodes.TryGetValue(entry, out var node) && node != null)
            {
                float combined = entry.Volume * _masterVolume;
                var src = node.GetComponent<AudioSource>();
                if (src != null) src.volume = Mathf.Min(combined, 1f);
                var amp = node.GetComponent<AudioAmplifier>();
                if (amp != null) amp.gain = combined;
            }
        }

        private void ApplyMasterVolume()
        {
            if (!UseRuntime) return;
            foreach (var kv in _activeNodes)
            {
                if (kv.Value == null) continue;
                float combined = kv.Key.Volume * _masterVolume;
                var src = kv.Value.GetComponent<AudioSource>();
                if (src != null) src.volume = Mathf.Min(combined, 1f);
                var amp = kv.Value.GetComponent<AudioAmplifier>();
                if (amp != null) amp.gain = combined;
            }
        }

        private void SaveSettings()
        {
            var data = new SavedSettings { masterVolume = _masterVolume };
            foreach (var entry in _entries)
            {
                if (Math.Abs(entry.Volume - 1f) > 0.001f || entry.Loop)
                {
                    data.entries.Add(new SavedEntryData
                    {
                        path = entry.Path,
                        volume = entry.Volume,
                        loop = entry.Loop,
                    });
                }
            }

            string dir = System.IO.Path.GetDirectoryName(SettingsPath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            File.WriteAllText(SettingsPath, JsonUtility.ToJson(data, true));
            _dirty = false;
        }

        private void LoadSettings()
        {
            if (!File.Exists(SettingsPath)) return;

            try
            {
                var data = JsonUtility.FromJson<SavedSettings>(File.ReadAllText(SettingsPath));
                _masterVolume = data.masterVolume;

                var lookup = new Dictionary<string, SavedEntryData>();
                foreach (var s in data.entries)
                    lookup[s.path] = s;

                foreach (var entry in _entries)
                {
                    if (lookup.TryGetValue(entry.Path, out var saved))
                    {
                        entry.Volume = saved.volume;
                        entry.Loop = saved.loop;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[SoundTester] Failed to load settings: {e.Message}");
            }
            _dirty = false;
        }

        private void ResetAllSettings()
        {
            _masterVolume = 1f;
            foreach (var entry in _entries)
            {
                entry.Volume = 1f;
                entry.Loop = false;
            }
            ApplyMasterVolume();
            _dirty = true;
        }

        private void MarkDirty()
        {
            _dirty = true;
        }

        private void RefreshAudioList()
        {
            StopAll();
            _entries.Clear();

            var guids = AssetDatabase.FindAssets("t:AudioClip", new[] { SearchRoot });

            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string lower = path.ToLowerInvariant();
                bool validExt = false;
                foreach (var ext in AudioExtensions)
                {
                    if (lower.EndsWith(ext)) { validExt = true; break; }
                }
                if (!validExt) continue;

                var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                if (clip == null) continue;

                string relativePath = path.Replace(SearchRoot + "/", "");
                string category = relativePath.Contains("/")
                    ? relativePath.Substring(0, relativePath.LastIndexOf('/'))
                    : "Root";

                _entries.Add(new AudioEntry
                {
                    Path = path,
                    FileName = System.IO.Path.GetFileName(path),
                    Category = category,
                    Clip = clip,
                });
            }

            _entries = _entries.OrderBy(e => e.Category).ThenBy(e => e.FileName).ToList();
            LoadSettings();
        }

        private void OnGUI()
        {
            DrawToolbar();

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox(
                    "Mode Play requis pour le multi-pistes et le controle de volume.\n" +
                    "En Edit mode : lecture mono-piste, volume fixe.",
                    MessageType.Info);
            }

            DrawMasterVolume();

            if (_entries.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    $"Aucun fichier audio trouve dans {SearchRoot}/.\n" +
                    "Formats supportes : WAV, MP3, OGG, AIF, AIFF.\n\n" +
                    "Placez vos sons dans Assets/_Project/Audio/ puis cliquez Refresh.",
                    MessageType.Info);
                return;
            }

            DrawAudioList();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(60)))
                RefreshAudioList();

            if (GUILayout.Button("Stop All", EditorStyles.toolbarButton, GUILayout.Width(60)))
                StopAll();

            GUILayout.Space(6);

            GUI.enabled = _dirty;
            if (GUILayout.Button("Save", EditorStyles.toolbarButton, GUILayout.Width(40)))
                SaveSettings();
            GUI.enabled = true;

            if (GUILayout.Button("Reset", EditorStyles.toolbarButton, GUILayout.Width(45)))
                ResetAllSettings();

            GUILayout.Space(10);
            EditorGUILayout.LabelField("Recherche", GUILayout.Width(65));
            _searchFilter = EditorGUILayout.TextField(_searchFilter, EditorStyles.toolbarSearchField);
            if (GUILayout.Button("X", EditorStyles.toolbarButton, GUILayout.Width(20)))
                _searchFilter = "";

            GUILayout.Space(10);

            int playingCount = UseRuntime ? _activeNodes.Count : (_editorPlayingEntry != null ? 1 : 0);
            string countLabel = playingCount > 0
                ? $"{_entries.Count} sons | {playingCount} actifs"
                : $"{_entries.Count} sons";
            EditorGUILayout.LabelField(countLabel, EditorStyles.miniLabel, GUILayout.Width(120));

            EditorGUILayout.EndHorizontal();
        }

        private void DrawMasterVolume()
        {
            bool volDisabled = !UseRuntime;
            EditorGUI.BeginDisabledGroup(volDisabled);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Master Volume", GUILayout.Width(95));
            float newMaster = EditorGUILayout.Slider(_masterVolume, 0f, 6f);
            if (Math.Abs(newMaster - _masterVolume) > 0.001f)
            {
                _masterVolume = newMaster;
                ApplyMasterVolume();
                MarkDirty();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space(2);
        }

        private void DrawAudioList()
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            string currentCategory = null;

            foreach (var entry in _entries)
            {
                if (!string.IsNullOrEmpty(_searchFilter))
                {
                    bool matchName = entry.FileName.IndexOf(_searchFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                    bool matchPath = entry.Path.IndexOf(_searchFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                    if (!matchName && !matchPath) continue;
                }

                if (entry.Category != currentCategory)
                {
                    currentCategory = entry.Category;
                    EditorGUILayout.Space(6);
                    EditorGUILayout.LabelField(currentCategory, EditorStyles.boldLabel);
                    DrawSeparator();
                }

                DrawAudioEntry(entry);
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawAudioEntry(AudioEntry entry)
        {
            bool isThisPlaying = IsEntryPlaying(entry);

            var style = new GUIStyle(EditorStyles.helpBox);
            if (isThisPlaying)
            {
                var tex = new Texture2D(1, 1);
                tex.SetPixel(0, 0, new Color(0.2f, 0.35f, 0.2f, 0.3f));
                tex.Apply();
                style.normal.background = tex;
            }

            EditorGUILayout.BeginHorizontal(style);

            // Play / Stop button
            string playLabel = isThisPlaying ? "\u25A0" : "\u25B6";
            GUI.backgroundColor = isThisPlaying ? new Color(1f, 0.4f, 0.4f) : Color.white;
            if (GUILayout.Button(playLabel, GUILayout.Width(28), GUILayout.Height(22)))
            {
                if (isThisPlaying)
                    StopEntry(entry);
                else
                    PlayEntry(entry);
            }
            GUI.backgroundColor = Color.white;

            // Loop toggle
            bool newLoop = GUILayout.Toggle(entry.Loop, "Loop", GUILayout.Width(42));
            if (newLoop != entry.Loop)
            {
                entry.Loop = newLoop;
                MarkDirty();
                if (isThisPlaying && UseRuntime && _activeNodes.TryGetValue(entry, out var node) && node != null)
                {
                    var src = node.GetComponent<AudioSource>();
                    if (src != null) src.loop = entry.Loop;
                }
            }

            // Clip name
            EditorGUILayout.LabelField(entry.FileName, GUILayout.MinWidth(120));

            // Volume slider
            bool volDisabled = !UseRuntime;
            EditorGUI.BeginDisabledGroup(volDisabled);
            EditorGUILayout.LabelField("Vol", GUILayout.Width(25));
            float newVol = EditorGUILayout.Slider(entry.Volume, 0f, 6f, GUILayout.MinWidth(100));
            if (Math.Abs(newVol - entry.Volume) > 0.001f)
            {
                entry.Volume = newVol;
                MarkDirty();
                if (isThisPlaying) ApplyVolume(entry);
            }
            EditorGUI.EndDisabledGroup();

            // Duration
            if (entry.Clip != null)
            {
                string dur = $"{entry.Clip.length:F1}s";
                EditorGUILayout.LabelField(dur, EditorStyles.miniLabel, GUILayout.Width(35));
            }

            // Ping in project
            if (GUILayout.Button("Loc", GUILayout.Width(32), GUILayout.Height(22)))
            {
                EditorGUIUtility.PingObject(entry.Clip);
                Selection.activeObject = entry.Clip;
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawSeparator()
        {
            var rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.5f));
        }

        private void EditorUpdate()
        {
            // Clean up finished non-looping sources
            if (UseRuntime && _activeNodes.Count > 0)
            {
                var finished = new List<AudioEntry>();
                foreach (var kv in _activeNodes)
                {
                    if (kv.Value == null)
                    {
                        finished.Add(kv.Key);
                        continue;
                    }
                    var src = kv.Value.GetComponent<AudioSource>();
                    if (src == null || !src.isPlaying)
                        finished.Add(kv.Key);
                }
                foreach (var entry in finished)
                {
                    if (_activeNodes.TryGetValue(entry, out var node) && node != null)
                        DestroyImmediate(node);
                    _activeNodes.Remove(entry);
                }
                if (finished.Count > 0) Repaint();
            }

            // Edit mode: clear when clip finishes
            if (!UseRuntime && _editorPlayingEntry != null)
            {
                if (IsPlayingMethod != null && !(bool)IsPlayingMethod.Invoke(null, null))
                {
                    _editorPlayingEntry = null;
                    Repaint();
                }
            }

            if (HasAnyPlaying())
                Repaint();
        }
    }
}
