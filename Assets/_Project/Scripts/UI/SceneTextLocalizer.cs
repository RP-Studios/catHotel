using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CatHotel.Core;

namespace CatHotel.UI
{
    /// <summary>
    /// Localizes static text labels in the scene.
    /// Matches labels by their default French text and replaces with the localized version.
    /// Supports both TextMeshProUGUI and legacy Text components.
    /// Attach to any root object in both Boot and Proto scenes.
    /// </summary>
    public class SceneTextLocalizer : MonoBehaviour
    {
        // Map: French text (default in scene) → localization key
        private static readonly Dictionary<string, string> TextToKey = new()
        {
            // Boot
            { "Jouer", "ui.play" },
            { "Crédits", "ui.credits" },

            // Shared
            { "Paramètres", "ui.parameters" },

            // Options
            { "Reprendre", "ui.resume" },
            { "Menu principal", "ui.main_menu" },
            { "Meowdex", "ui.meowdex" },

            // Parameters
            { "Langues", "param.languages" },
            { "Notifications push", "param.notifications" },
            { "Économie de batterie", "param.battery" },
            { "Economie de batterie", "param.battery" }, // alt encoding
            { "Volume musique", "param.music_volume" },
            { "Volume des effets", "param.effects_volume" },

            // Shop categories
            { "Boutique", "ui.shop" },
            { "Nourriture", "shop.category.food" },
            { "Boisson", "shop.category.drink" },
            { "Amusement", "shop.category.play" },
            { "Confort", "shop.category.comfort" },
            { "Décoration", "shop.category.deco" },
            { "Doubler les gains", "shop.double_gains" },

            // Shop sub-categories (also used as standalone labels)
            { "Lits", "shop.beds" },
            { "Coussins", "shop.pillows" },
            { "Croquettes", "shop.croquettes" },
            { "Eau", "shop.water" },
            { "Balles", "shop.balls" },
            { "Griffoirs", "shop.scratchers" },
            { "Gratoirs", "shop.scratchers" }, // alt spelling in scene
            { "Litières", "shop.litters" },
            { "Cadres", "shop.frames" },
            { "Lampes", "shop.lamps" },
            { "Tables", "shop.tables" },
            { "Plantes", "shop.plants" },
            { "Étagères", "shop.shelves" },
            { "Etag\u00e8res", "shop.shelves" }, // alt encoding
            { "Aquariums", "shop.aquariums" },
            { "Tapis", "shop.carpets" },

            // Cat info panel - titles
            { "Nom du chat", "cat.name_label" },
            { "Race", "cat.race_label" },
            { "Âge", "cat.age_label" },
            { "Adulte", "cat.adult" },
            { "Caractère", "cat.character_label" },
            { "Affinités", "cat.affinity_label" },
            { "Besoins", "cat.needs_label" },
            { "Faim", "cat.need.hunger" },
            { "Soif", "cat.need.thirst" },
            { "Sommeil", "cat.need.sleep" },
            { "Jeu", "cat.need.play" },
            { "Propreté", "cat.need.clean" },
            { "Bonheur", "cat.need.happiness" },

            // End pension panel
            { "Fin de séjour en pension !", "pension.title" },
            { "Bonheur moyen lors du séjour :", "pension.happiness" },
            { "Détails du paiement", "pension.details" },
            { "Base", "pension.base" },
            { "Pourboire", "pension.tip" },
            { "Total", "pension.total" },
            { "Temps restant en pension", "pension.time_remaining" },

            // Object names (displayed in shop item cards)
            { "Lit", "obj.bed" },
            { "Lit de luxe", "obj.luxury_bed" },
            { "Coussin", "obj.pillow" },
            { "Gamelle", "obj.food_bowl" },
            { "Gamelle var. 2", "obj.food_bowl_v2" },
            { "Gamelle var. 3", "obj.food_bowl_v3" },
            { "Gamelle var. 4", "obj.food_bowl_v4" },
            { "Bol d'eau", "obj.water_bowl" },
            { "Bol d'eau moderne", "obj.water_bowl_04" },
            { "Bol d'eau var. 2", "obj.water_bowl_v2" },
            { "Bol d'eau var. 3", "obj.water_bowl_v3" },
            { "Balle de laine", "obj.wool_ball" },
            { "Arbre à chat", "obj.cat_tree" },
            { "Griffoir", "obj.scratcher" },
            { "Litière", "obj.litter" },
            { "Litière var.", "obj.litter_v1" },
            { "Cadre 1", "obj.frame_1" },
            { "Cadre 2", "obj.frame_2" },
            { "Cadre 3", "obj.frame_3" },
            { "Peinture", "obj.painting" },
            { "Grande lampe", "obj.lamp" },
            { "Table basse", "obj.coffee_table" },
            { "Commode", "obj.drawer" },
            { "Grande plante", "obj.plant_big" },
            { "Petite plante", "obj.plant_small" },
            { "Étagère", "obj.shelf" },
            { "Étagère var.", "obj.shelf_v1" },
            { "Aquarium", "obj.aquarium" },
            { "Tapis Confort", "obj.carpet_confort" },
            { "Tapis Jeu", "obj.carpet_play" },
            { "Tapis Cosmique", "obj.carpet_cosmic" },
        };

        // Map: GameObject name → localization key (takes priority over text matching)
        private static readonly Dictionary<string, string> NameToKey = new()
        {
            { "CarspeciesLabel", "cat.race_label" },
            { "CatAgeLabel", "cat.age_label" },
            { "CatDescLabel", "cat.character_label" },
            { "CatSpeciesSpecLabel", "cat.affinity_label" },
        };

        private readonly List<(Component text, string key, bool isTMP)> _tracked = new();

        private void Start()
        {
            ScanAndTrack();
            RefreshAll();
            LocalizedStrings.OnLanguageChanged += RefreshAll;
        }

        private void OnDestroy()
        {
            LocalizedStrings.OnLanguageChanged -= RefreshAll;
        }

        private void ScanAndTrack()
        {
            // Scan TMP_Text
            var tmpTexts = FindObjectsByType<TMP_Text>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var tmp in tmpTexts)
            {
                if (tmp == null) continue;
                // Priority: match by GO name first, then by text content
                string key = FindKeyByName(tmp.gameObject.name) ?? FindKey(tmp.text);
                if (key != null)
                    _tracked.Add((tmp, key, true));
            }

            // Scan legacy Text
            var legacyTexts = FindObjectsByType<Text>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var txt in legacyTexts)
            {
                if (txt == null) continue;
                string key = FindKeyByName(txt.gameObject.name) ?? FindKey(txt.text);
                if (key != null)
                    _tracked.Add((txt, key, false));
            }
        }

        private static string FindKeyByName(string goName)
        {
            return NameToKey.TryGetValue(goName, out var key) ? key : null;
        }

        private static string FindKey(string currentText)
        {
            if (string.IsNullOrEmpty(currentText)) return null;
            string trimmed = currentText.Trim();

            // Match against French values
            if (TextToKey.TryGetValue(trimmed, out var key))
                return key;

            // Match against English values (in case already localized)
            foreach (var kvp in TextToKey)
            {
                if (LocalizedStrings.English.TryGetValue(kvp.Value, out var enVal) && trimmed == enVal)
                    return kvp.Value;
            }

            return null;
        }

        private void RefreshAll()
        {
            foreach (var (text, key, isTMP) in _tracked)
            {
                if (text == null) continue;
                string localized = LocalizedStrings.Get(key);
                if (isTMP)
                    ((TMP_Text)text).text = localized;
                else
                    ((Text)text).text = localized;
            }
        }
    }
}
