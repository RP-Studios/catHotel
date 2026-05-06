using UnityEngine;
using UnityEditor;

namespace CatHotel.Editor
{
    public class CatHotelToolsWindow : EditorWindow
    {
        private Vector2 _scroll;

        [MenuItem("Cat Hotel/Help", false, 100)]
        public static void ShowWindow()
        {
            var win = GetWindow<CatHotelToolsWindow>("Cat Hotel Tools");
            win.minSize = new Vector2(480, 400);
        }

        private void OnGUI()
        {
            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            Header("Cat Hotel — Editor Tools");
            EditorGUILayout.Space(4);

            // ── Scene ──
            Section("Scene");

            Tool("Setup Proto Scene",
                "Reconstruit la scene Proto de zero : imports sprites, " +
                "decoupage spritesheets, creation des tiles et AnimatorControllers, " +
                "hierarchie complete (camera, grille, managers, UI), " +
                "ScriptableObjects et cablage de toutes les references.",
                "Apres un changement de structure de scene, ou pour repartir d'un etat propre.");

            Tool("Setup Safe Area",
                "Cree un container 'SafeArea' sous le Canvas principal et y reparente " +
                "automatiquement tous les enfants HUD. Le container ajuste ses anchors " +
                "sur Screen.safeArea (horizontal uniquement par defaut) pour tenir " +
                "compte des cameras / encoches / gestes des bords. Les calques full-screen " +
                "(Fade, Loading, Background) restent siblings et continuent de couvrir tout l'ecran.",
                "Une fois apres setup de scene, ou apres ajout de nouveaux groupes HUD direct enfants du Canvas.");

            // ── Build ──
            Section("Build");

            Tool("Setup Addressables",
                "Cree le groupe 'Breeds' dans Addressables et marque les 9 CatBreedData " +
                "comme addressable (adresse = nom du fichier). Les sprites et " +
                "AnimatorControllers references sont automatiquement inclus dans le bundle. " +
                "Configure le groupe en local, LZ4.",
                "Une seule fois au setup initial, ou apres ajout d'une nouvelle race de chat.");

            Tool("Check Addressables Duplicates",
                "Verifie qu'aucun sprite ou AnimatorController individuel n'est " +
                "marque comme addressable, ce qui doublerait la taille du build. " +
                "Ne modifie rien, diagnostic seulement.",
                "Avant un build, si la taille semble anormale.");

            Tool("Optimize All Textures",
                "Pipeline complet d'optimisation des textures en 2 passes :\n" +
                "  Pass 1 — Compression ASTC 6x6, crunched (quality 50), max 4096.\n" +
                "  Pass 2 — Desactive les mipmaps sur tous les sprites 2D " +
                "(evite le flou), cap les textures UI a 2048.",
                "Apres import de nouveaux sprites ou assets graphiques.");

            // ── Audio ──
            Section("Audio");

            Tool("Sound Tester",
                "Ouvre une fenetre pour ecouter et tester tous les clips audio du projet. " +
                "Controles play/stop/loop/volume par clip. Fonctionne en Edit et Play mode. " +
                "Sauvegarde les preferences de volume dans un JSON.",
                "Pour tester les sons sans lancer le jeu.");

            Tool("Create UI Sound Bank",
                "Cree ou met a jour le ScriptableObject Resources/UISoundBank.asset. " +
                "Assigne automatiquement les 10 clips SFX UI depuis Audio/SFX/UI/ " +
                "(6 tap positive, 1 neutral, 1 negative, 1 open, 1 close).",
                "Apres ajout ou modification de sons UI.");

            EditorGUILayout.EndScrollView();
        }

        private void Header(string text)
        {
            EditorGUILayout.LabelField(text, EditorStyles.boldLabel);
            DrawLine();
        }

        private void Section(string name)
        {
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField(name, EditorStyles.boldLabel);
            DrawLine();
        }

        private void Tool(string name, string description, string whenToUse)
        {
            EditorGUILayout.Space(4);

            var nameStyle = new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 12
            };
            EditorGUILayout.LabelField(name, nameStyle);

            var descStyle = new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                richText = true,
                padding = new RectOffset(12, 4, 0, 0)
            };
            EditorGUILayout.LabelField(description, descStyle);

            var whenStyle = new GUIStyle(EditorStyles.wordWrappedMiniLabel)
            {
                richText = true,
                padding = new RectOffset(12, 4, 2, 0),
                fontStyle = FontStyle.Italic
            };
            EditorGUILayout.LabelField("Quand : " + whenToUse, whenStyle);

            EditorGUILayout.Space(2);
        }

        private void DrawLine()
        {
            var rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.5f));
        }
    }
}
