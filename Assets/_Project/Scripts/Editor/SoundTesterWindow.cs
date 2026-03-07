using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace CatHotel.Editor
{
    public class SoundTesterWindow : EditorWindow
    {
        private static readonly string[] AudioExtensions = { ".wav", ".mp3", ".ogg", ".aif", ".aiff" };
        private static readonly string SearchRoot = "Assets/_Project";

        private List<AudioEntry> _entries = new();
        private Vector2 _scrollPos;
        private string _searchFilter = "";
        private float _masterVolume = 1f;
        private AudioEntry _playingEntry;

        // Runtime playback (Play mode)
        private GameObject _runtimeGO;
        private AudioSource _runtimeSource;

        // Editor playback (Edit mode) via AudioUtil reflection
        private static readonly Type AudioUtilType;
        private static readonly MethodInfo PlayClipMethod;
        private static readonly MethodInfo StopClipsMethod;
        private static readonly MethodInfo IsPlayingMethod;

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
            Stop();
            DestroyRuntime();
        }

        private void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                _playingEntry = null;
                _runtimeSource = null;
                _runtimeGO = null;
            }
        }

        private void EnsureRuntime()
        {
            if (_runtimeGO != null && _runtimeSource != null) return;

            _runtimeGO = new GameObject("[SoundTester]");
            _runtimeSource = _runtimeGO.AddComponent<AudioSource>();
            _runtimeSource.playOnAwake = false;
            _runtimeSource.spatialBlend = 0f; // 2D

            // Ensure AudioListener exists
            if (UnityEngine.Object.FindFirstObjectByType<AudioListener>() == null)
                _runtimeGO.AddComponent<AudioListener>();

            Debug.Log("[SoundTester] AudioSource cree OK");
        }

        private void DestroyRuntime()
        {
            if (_runtimeGO != null)
                DestroyImmediate(_runtimeGO);
            _runtimeGO = null;
            _runtimeSource = null;
        }

        private bool UseRuntime => Application.isPlaying;

        private void Play(AudioEntry entry)
        {
            Stop();
            _playingEntry = entry;

            if (UseRuntime)
            {
                // Stop any AudioUtil preview that might be running
                StopClipsMethod?.Invoke(null, null);

                EnsureRuntime();
                _runtimeSource.clip = entry.Clip;
                _runtimeSource.loop = entry.Loop;
                _runtimeSource.volume = entry.Volume * _masterVolume;
                _runtimeSource.Play();

                Debug.Log($"[SoundTester] Play: {entry.FileName} | clip null? {entry.Clip == null} " +
                          $"| vol={_runtimeSource.volume:F2} | loop={entry.Loop} " +
                          $"| isPlaying={_runtimeSource.isPlaying} | listener={UnityEngine.Object.FindFirstObjectByType<AudioListener>() != null}");
            }
            else
            {
                PlayClipMethod?.Invoke(null, new object[] { entry.Clip, 0, entry.Loop });
            }
        }

        private void Stop()
        {
            _playingEntry = null;

            if (_runtimeSource != null && _runtimeSource.isPlaying)
                _runtimeSource.Stop();

            StopClipsMethod?.Invoke(null, null);
        }

        private bool IsPlaying()
        {
            if (UseRuntime)
                return _runtimeSource != null && _runtimeSource.isPlaying;

            if (IsPlayingMethod != null)
                return (bool)IsPlayingMethod.Invoke(null, null);

            return false;
        }

        private bool IsPlayingEntry(AudioEntry entry)
        {
            return _playingEntry == entry && IsPlaying();
        }

        private void ApplyVolume()
        {
            if (UseRuntime && _runtimeSource != null && _playingEntry != null)
                _runtimeSource.volume = _playingEntry.Volume * _masterVolume;
        }

        private void RefreshAudioList()
        {
            Stop();
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
        }

        private void OnGUI()
        {
            DrawToolbar();

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox(
                    "Mode Play requis pour le controle de volume.\n" +
                    "En Edit mode, la lecture fonctionne mais le volume est fixe.",
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
                Stop();

            GUILayout.Space(10);
            EditorGUILayout.LabelField("Recherche", GUILayout.Width(65));
            _searchFilter = EditorGUILayout.TextField(_searchFilter, EditorStyles.toolbarSearchField);
            if (GUILayout.Button("X", EditorStyles.toolbarButton, GUILayout.Width(20)))
                _searchFilter = "";

            GUILayout.Space(10);
            EditorGUILayout.LabelField($"{_entries.Count} sons", EditorStyles.miniLabel, GUILayout.Width(60));

            EditorGUILayout.EndHorizontal();
        }

        private void DrawMasterVolume()
        {
            bool volDisabled = !UseRuntime;
            EditorGUI.BeginDisabledGroup(volDisabled);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Master Volume", GUILayout.Width(95));
            float newMaster = EditorGUILayout.Slider(_masterVolume, 0f, 1f);
            if (Math.Abs(newMaster - _masterVolume) > 0.001f)
            {
                _masterVolume = newMaster;
                ApplyVolume();
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
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

            bool isThisPlaying = IsPlayingEntry(entry);

            // Play / Stop
            string playLabel = isThisPlaying ? "■" : "▶";
            GUI.backgroundColor = isThisPlaying ? new Color(1f, 0.4f, 0.4f) : Color.white;
            if (GUILayout.Button(playLabel, GUILayout.Width(28), GUILayout.Height(22)))
            {
                if (isThisPlaying)
                    Stop();
                else
                    Play(entry);
            }
            GUI.backgroundColor = Color.white;

            // Loop toggle
            bool newLoop = GUILayout.Toggle(entry.Loop, "Loop", GUILayout.Width(42));
            if (newLoop != entry.Loop)
            {
                entry.Loop = newLoop;
                if (isThisPlaying)
                {
                    if (UseRuntime && _runtimeSource != null)
                        _runtimeSource.loop = entry.Loop;
                    else
                        Play(entry);
                }
            }

            // Clip name
            EditorGUILayout.LabelField(entry.FileName, GUILayout.MinWidth(120));

            // Volume slider
            bool volDisabled = !UseRuntime;
            EditorGUI.BeginDisabledGroup(volDisabled);
            EditorGUILayout.LabelField("Vol", GUILayout.Width(25));
            float newVol = EditorGUILayout.Slider(entry.Volume, 0f, 1f, GUILayout.MinWidth(100));
            if (Math.Abs(newVol - entry.Volume) > 0.001f)
            {
                entry.Volume = newVol;
                if (isThisPlaying) ApplyVolume();
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
            if (_playingEntry != null && !IsPlaying())
            {
                _playingEntry = null;
                Repaint();
            }

            if (_playingEntry != null)
                Repaint();
        }
    }
}
