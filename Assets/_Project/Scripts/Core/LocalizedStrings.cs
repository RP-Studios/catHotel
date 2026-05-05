using System.Collections.Generic;
using CatHotel.Services;

namespace CatHotel.Core
{
    /// <summary>
    /// Key-based localization system. All user-visible text is referenced by key.
    /// To add a new language: create a new dictionary and swap it via SetLanguage().
    /// Format strings use {0}, {1} etc. for parameters.
    /// </summary>
    public static class LocalizedStrings
    {
        private static Dictionary<string, string> _current;

        static LocalizedStrings()
        {
            _current = French;
        }

        /// <summary>Get a localized string by key.</summary>
        public static string Get(string key)
        {
            return _current.TryGetValue(key, out var value) ? value : $"[{key}]";
        }

        /// <summary>Get a localized string with format parameters.</summary>
        public static string Get(string key, params object[] args)
        {
            if (!_current.TryGetValue(key, out var value)) return $"[{key}]";
            return string.Format(value, args);
        }

        /// <summary>Fired when language changes. UI should subscribe to refresh labels.</summary>
        public static event System.Action OnLanguageChanged;

        /// <summary>Current language key ("fr", "en" or "de").</summary>
        public static string CurrentLanguage { get; private set; } = "fr";

        private const string PrefKey = "Param_Language";

        private static Dictionary<string, string> ResolveDictionary(string langCode)
        {
            return langCode switch
            {
                "en" => English,
                "de" => German,
                _ => French,
            };
        }

        /// <summary>Switch language at runtime and notify all listeners.</summary>
        public static void SetLanguage(string langCode)
        {
            CurrentLanguage = langCode;
            _current = ResolveDictionary(langCode);

            if (CloudSaveManager.Instance != null)
            {
                CloudSaveManager.Instance.Settings.language = langCode;
                CloudSaveManager.Instance.SaveSettings();
            }

            OnLanguageChanged?.Invoke();
        }

        /// <summary>Initialize language from saved preference or system language.</summary>
        public static void InitFromSystem()
        {
            // Try cloud save first
            if (CloudSaveManager.Instance != null && CloudSaveManager.Instance.IsLoaded
                && !string.IsNullOrEmpty(CloudSaveManager.Instance.Settings.language))
            {
                string saved = CloudSaveManager.Instance.Settings.language;
                CurrentLanguage = saved;
                _current = ResolveDictionary(saved);
                return;
            }

            // Fallback: check PlayerPrefs (migration from old system)
            if (UnityEngine.PlayerPrefs.HasKey(PrefKey))
            {
                string saved = UnityEngine.PlayerPrefs.GetString(PrefKey);
                CurrentLanguage = saved;
                _current = ResolveDictionary(saved);

                // Migrate to cloud save
                if (CloudSaveManager.Instance != null)
                {
                    CloudSaveManager.Instance.Settings.language = saved;
                    CloudSaveManager.Instance.SaveSettings();
                }
                return;
            }

            // Auto-detect from system
            var sysLang = UnityEngine.Application.systemLanguage;
            if (sysLang == UnityEngine.SystemLanguage.French)
            {
                CurrentLanguage = "fr";
                _current = French;
            }
            else if (sysLang == UnityEngine.SystemLanguage.German)
            {
                CurrentLanguage = "de";
                _current = German;
            }
            else
            {
                CurrentLanguage = "en";
                _current = English;
            }

            if (CloudSaveManager.Instance != null)
            {
                CloudSaveManager.Instance.Settings.language = CurrentLanguage;
                CloudSaveManager.Instance.SaveSettings();
            }
        }

        // ==================== FRENCH ====================
        public static readonly Dictionary<string, string> French = new()
        {
            // --- Cat Info Panel ---
            { "cat.kitten", "Chaton" },
            { "cat.adult", "Chat adulte" },
            { "cat.stay.pension", "En pension" },
            { "cat.stay.refuge", "Arrivé au refuge" },
            { "cat.likes", "Aime les {0}" },
            { "cat.dislikes", "Déteste les {0}" },
            { "cat.time.none", "--:--:--" },

            // --- End Pension Panel ---
            { "pension.bye", "Au revoir {0} !" },

            // --- Loading ---
            { "ui.loading", "Chargement" },
            { "loading.tip.0", "Caresse tes chats pour les rendre heureux." },
            { "loading.tip.1", "Un chat malheureux risque de partir sans te payer." },
            { "loading.tip.2", "Tape deux fois sur un objet pour le déplacer." },
            { "loading.tip.3", "Les tapis offrent un bonus de confort à la zone." },
            { "loading.tip.4", "Certaines races ne s'entendent pas — garde un œil dessus." },
            { "loading.tip.5", "Plus ton hôtel est réputé, plus les chats sont exigeants." },
            { "loading.tip.6", "Les escaliers mènent aux étages supérieurs." },
            { "loading.tip.7", "Collecte tes pièces rapidement avant qu'elles disparaissent." },

            // --- Tutorial (Narration) ---
            { "tuto.speaker.jasper", "Jasper" },
            { "tuto.speaker.winston", "Winston" },
            { "tuto.skip.link", "Passer le tutoriel" },
            { "tuto.skip.ask", "Passer le tutoriel ?" },
            { "tuto.skip.yes", "Oui" },
            { "tuto.skip.no", "Non" },
            { "tuto.welcome", "Bienvenue dans <b>Meowtel</b> !\nJe suis Jasper, le maître d'hôtel." },
            { "tuto.explain", "Ici, vous accueillez des chats en pension et au refuge. Prenez soin d'eux et ils vous récompenseront !" },
            { "tuto.floors", "L'hôtel compte <b>10 étages</b> à débloquer au-dessus du RDC. Plus l'hôtel grandit, plus vous accueillez de chats !" },
            { "tuto.levelup.intro", "Pour débloquer un nouvel étage, il faut <b>monter de niveau de réputation</b>." },
            { "tuto.levelup.xp", "Gagnez de l'<b>XP</b> en rendant les chats heureux. Chaque pension réussie et chaque adoption en rapportent." },
            { "tuto.levelup.cond", "Pour passer un palier, il faut : assez d'<b>XP</b>, assez de <b>chats à plus de 75% de bonheur</b>, et payer en <b>Cat Coins</b>." },
            { "tuto.levelup.tap", "Tapez sur la <b>barre de niveau</b> en haut pour voir vos conditions actuelles." },
            { "tuto.levelup.done", "Parfait ! Vous y reviendrez régulièrement pour faire grandir l'hôtel. Refermez le panneau pour continuer." },
            { "tuto.coins", "La <b>monnaie</b> ici est le Cat Coin. Achetez des objets pour vos chats... et payez les passages de niveau !" },
            // --- HUD elements walkthrough ---
            { "tuto.hud.coins", "Voici votre <b>réserve de Cat Coins</b>. Elle se remplit quand vous récupérez les pièces laissées par les chats heureux. Sert à acheter des objets et à monter de niveau." },
            { "tuto.hud.purrls", "Les <b>Puurls</b> sont la monnaie premium. Plus tard, ils permettront d'acheter des objets spéciaux à gros bonus, ou d'activer des boosts de revenus." },
            { "tuto.hud.capacity", "La <b>capacité</b> de l'hôtel : pourcentage des places occupées par rapport au maximum autorisé par votre niveau." },
            { "tuto.hud.comfort", "Le <b>confort moyen</b> de l'hôtel. Base 50%, +5% si l'hôtel est peu rempli (<20%), augmente avec les objets de décoration, et chute si l'hôtel est surchargé (>80%)." },
            { "tuto.hud.floors", "L'<b>étage</b> actuellement affiché. Utilisez les flèches pour monter et descendre quand vous en aurez débloqué." },
            { "tuto.hud.system", "Le <b>menu pause</b>. Réglages, langue, son, et options du jeu." },
            { "tuto.hud.collectall", "<b>Récupérer toutes les pièces</b> d'un coup. Coût : 5 Cat Coins. Pratique quand l'hôtel se remplit !" },
            { "tuto.hud.addboost", "Regarder une pub pour <b>doubler vos revenus pendant 60s</b>. Cooldown : 120s entre deux activations." },
            { "tuto.hud.nextcat", "Le <b>compte à rebours</b> avant l'arrivée du prochain chat. Plus votre hôtel grandit, plus le rythme s'accélère." },
            { "tuto.hud.shop", "La <b>boutique d'objets</b>. Achetez ici tout ce qu'il faut pour satisfaire les besoins de vos chats." },
            { "tuto.pension", "Des chats arrivent en <b>pension</b> pour un temps déterminé. Leur bonheur doit être le plus élevé possible pour gagner un maximum de Cat Coins !" },
            { "tuto.refuge", "Des chats tristes arriveront par le <b>refuge</b>. Rendez-les heureux pour qu'ils puissent être adoptés !" },
            { "tuto.unhappy", "Attention : un chat dont le bonheur tombe sous <b>25%</b> voudra partir. Ne le laissez pas filer !" },
            { "tuto.firstcat", "Oh ! Votre premier client arrive ! Observez-le..." },
            { "tuto.selectcat", "Tapez sur le chat pour le sélectionner." },
            { "tuto.catinfo", "Voici ses informations : son nom, sa race, son humeur et ses besoins. Gardez un œil dessus !" },
            { "tuto.pensiontime", "Regardez le temps restant en pension. Quand il s'écoulera, le chat repartira. Plus il est heureux, plus vous gagnez !" },
            { "tuto.refugecat", "Un chat du refuge arrive ! Il a faim... Regardez ses jauges : il a besoin de croquettes." },
            { "tuto.buyfood", "Ouvrez la boutique et achetez une <b>gamelle</b>, puis placez-la dans la pièce." },
            { "tuto.waiteat", "Bien ! Attendez que le chat aille manger..." },
            { "tuto.collectcoin", "Une pièce est apparue ! Tapez dessus pour collecter vos <b>Cat Coins</b>." },
            { "tuto.thirsty", "Il a soif maintenant ! Achetez de l'<b>eau</b> et placez-la." },
            { "tuto.waitdrink", "Laissez-le boire tranquillement..." },
            { "tuto.sleepy", "Le chat est fatigué. Offrez-lui un <b>lit</b> ou un <b>coussin</b> !" },
            { "tuto.waitsleep", "Il se repose... chut !" },
            { "tuto.dirty", "Il a besoin d'une <b>litière</b> ! Achetez-en une." },
            { "tuto.waitclean", "Il fait sa toilette..." },
            { "tuto.bored", "Il veut jouer ! Achetez une <b>balle de laine</b>." },
            { "tuto.waitplay", "Regardez-le s'amuser !" },
            { "tuto.complete", "Bravo ! Vous connaissez les bases. L'hôtel est à vous, bonne chance !" },

            // --- Sound ---
            { "sound.on", "Son activé" },
            { "sound.off", "Son désactivé" },

            // --- Mood ---
            { "mood.happy", "Content" },
            { "mood.ecstatic", "Enthousiaste" },
            { "mood.depressed", "Déprimé" },
            { "mood.aggressive", "Bagarreur" },
            { "mood.angry", "En colère" },

            // --- Needs ---
            { "need.hungry", "A faim" },
            { "need.thirsty", "A soif" },
            { "need.tired", "Fatigué" },
            { "need.bored", "S'ennuie" },
            { "need.dirty", "Sale" },

            // --- HUD ---
            { "hud.floor.ground", "RDC" },
            { "hud.floor", "Étage {0}" },
            { "hud.level", "Niveau {0}" },
            { "levelup.title.current", "Niveau {0}" },
            { "levelup.title.next", "Niveau {0}" },
            { "levelup.newstuff", "Nouveautés" },
            { "levelup.condition.xp", "{0} / {1} XP" },
            { "levelup.condition.happycats", "{0} chats > 75% de bonheur sur {1}" },
            { "levelup.newfloor", "Accès niveau {0}" },
            { "levelup.capacity.title", "Capacité de l'hôtel" },
            { "levelup.go.next", "Passer au niveau suivant" },
            { "hud.level.max", "MAX" },
            { "hud.level.max.reached", "Niveau maximum atteint !" },
            { "hud.level.objective", "{0}/{1} chats à +{2}% de bonheur" },
            { "hud.boost.active", "Boost x2 actif ! {0}s" },

            // --- Reputation Level Names ---
            { "rep.0", "Débutant" },
            { "rep.1", "Amateur" },
            { "rep.2", "Compétent" },
            { "rep.3", "Professionnel" },
            { "rep.4", "Expert" },
            { "rep.5", "Renommé" },
            { "rep.6", "Célèbre" },
            { "rep.7", "Prestigieux" },
            { "rep.8", "Élite" },
            { "rep.9", "Légendaire" },
            { "rep.10", "Maître des Chats" },

            // --- Shop Categories ---
            { "shop.beds", "Lits" },
            { "shop.pillows", "Coussins" },
            { "shop.croquettes", "Croquettes" },
            { "shop.water", "Eau" },
            { "shop.balls", "Balles" },
            { "shop.scratchers", "Griffoirs" },
            { "shop.litters", "Litières" },
            { "shop.frames", "Cadres" },
            { "shop.lamps", "Lampes" },
            { "shop.tables", "Tables" },
            { "shop.plants", "Plantes" },
            { "shop.shelves", "Étagères" },
            { "shop.aquariums", "Aquariums" },
            { "shop.carpets", "Tapis" },
            { "shop.trees", "Arbres à chat" },

            // --- Breed Names ---
            { "breed.europeen", "Européen" },
            { "breed.siamois", "Siamois" },
            { "breed.ragdoll", "Ragdoll" },
            { "breed.siberien", "Sibérien" },
            { "breed.chartreux", "Chartreux" },

            // --- Personality: Race Traits ---
            { "trait.hunger", "Gourmand" },
            { "trait.thirst", "Assoiffé" },
            { "trait.sleep", "Dormeur" },
            { "trait.play", "Joueur" },
            { "trait.clean", "Maniaque" },
            { "trait.aggressive", "Bagarreur" },
            { "trait.big", "Imposant" },
            { "trait.small", "Petit gabarit" },
            { "trait.fast", "Rapide" },
            { "trait.slow", "Nonchalant" },

            // --- Personality: Personality Pool ---
            { "personality.cuddly", "Câlin" },
            { "personality.independent", "Indépendant" },
            { "personality.curious", "Curieux" },
            { "personality.fearful", "Craintif" },
            { "personality.affectionate", "Affectueux" },
            { "personality.observer", "Observateur" },
            { "personality.discreet", "Discret" },
            { "personality.clingy", "Pot de colle" },
            { "personality.adventurer", "Aventurier" },
            { "personality.territorial", "Territorial" },
            { "personality.sociable", "Sociable" },
            { "personality.solitary", "Solitaire" },
            { "personality.clever", "Malin" },
            { "personality.lazy", "Paresseux" },
            { "personality.loyal", "Fidèle" },
            { "personality.capricious", "Capricieux" },

            // --- Personality: Quirky Pool ---
            { "quirk.grudge", "Rancunier" },
            { "quirk.touchy", "Susceptible" },
            { "quirk.dumb", "Idiot" },
            { "quirk.big_eater", "Gros mangeur" },
            { "quirk.drinker", "Soiffard" },
            { "quirk.food_thief", "Voleur de croquettes" },
            { "quirk.snorer", "Ronfleur" },
            { "quirk.litter_fear", "Peureux des litières" },
            { "quirk.jealous", "Jaloux" },
            { "quirk.show_off", "Frimeur" },
            { "quirk.drama_queen", "Drama queen" },
            { "quirk.hair_collector", "Collectionneur de poils" },
            { "quirk.grumpy", "Grognon" },
            { "quirk.clumsy", "Maladroit" },
            { "quirk.hypochondriac", "Hypocondriaque" },
            { "quirk.couch_king", "Roi du canapé" },
            { "quirk.snob", "Snob" },
            { "quirk.pilferer", "Chapardeur" },

            // --- Scene Labels (static text in scene hierarchy) ---
            // Boot scene
            { "ui.play", "Jouer" },
            { "ui.continue", "Continuer partie" },
            { "ui.new_game", "Nouvelle partie" },

            // Credits panel
            { "credits.title", "Crédits" },
            { "credits.gameby", "un jeu de" },
            { "credits.direction", "Direction / développement" },
            { "credits.disclaimer", "Ce jeu a été créé à l'aide du moteur Unity, propriété de Unity Technologies. Unity et le logo Unity sont des marques déposées de Unity Technologies aux États-Unis d'Amérique et dans d'autres pays" },
            { "ui.credits", "Crédits" },
            { "ui.parameters", "Paramètres" },

            // Options panel
            { "ui.resume", "Reprendre" },
            { "ui.main_menu", "Menu principal" },
            { "ui.meowdex", "Meowdex" },

            // Parameters panel
            { "param.languages", "Langues" },
            { "param.notifications", "Notifications push" },
            { "param.battery", "Économie de batterie" },
            { "param.music_volume", "Volume musique" },
            { "param.effects_volume", "Volume des effets" },

            // Shop
            { "ui.shop", "Boutique" },
            { "shop.category.food", "Nourriture" },
            { "shop.category.drink", "Boisson" },
            { "shop.category.sleep", "Sommeil" },
            { "shop.category.play", "Amusement" },
            { "shop.category.clean", "Propreté" },
            { "shop.category.comfort", "Confort" },
            { "shop.category.deco", "Décoration" },
            { "shop.double_gains", "Doubler les gains" },

            // Cat Info Panel
            { "cat.name_label", "Nom du chat" },
            { "cat.race_label", "Race" },
            { "cat.age_label", "Âge" },
            { "cat.character_label", "Caractère" },
            { "cat.affinity_label", "Affinités" },
            { "cat.needs_label", "Besoins" },
            { "cat.need.hunger", "Faim" },
            { "cat.need.thirst", "Soif" },
            { "cat.need.sleep", "Sommeil" },
            { "cat.need.play", "Jeu" },
            { "cat.need.clean", "Propreté" },
            { "cat.need.happiness", "Bonheur" },

            // End Pension Panel
            { "pension.title", "Fin de séjour en pension !" },
            { "pension.happiness", "Bonheur moyen lors du séjour :" },
            { "pension.details", "Détails du paiement" },
            { "pension.base", "Base" },
            { "pension.tip", "Pourboire" },
            { "pension.total", "Total" },
            { "pension.time_remaining", "Temps restant en pension" },

            // HUD
            { "hud.boost.label", "Boost x2 collecte de cat coins actif !" },

            // Object display names (shop items)
            { "obj.bed", "Lit" },
            { "obj.luxury_bed", "Lit de luxe" },
            { "obj.pillow", "Coussin" },
            { "obj.food_bowl", "Gamelle" },
            { "obj.food_bowl_v2", "Gamelle var. 2" },
            { "obj.food_bowl_v3", "Gamelle var. 3" },
            { "obj.food_bowl_v4", "Gamelle var. 4" },
            { "obj.water_bowl", "Bol d'eau" },
            { "obj.water_bowl_04", "Bol d'eau moderne" },
            { "obj.water_bowl_v2", "Bol d'eau var. 2" },
            { "obj.water_bowl_v3", "Bol d'eau var. 3" },
            { "obj.wool_ball", "Balle de laine" },
            { "obj.cat_tree", "Arbre à chat" },
            { "obj.scratcher", "Griffoir" },
            { "obj.litter", "Litière" },
            { "obj.litter_v1", "Litière var." },
            { "obj.frame_1", "Cadre 1" },
            { "obj.frame_2", "Cadre 2" },
            { "obj.frame_3", "Cadre 3" },
            { "obj.painting", "Peinture" },
            { "obj.lamp", "Grande lampe" },
            { "obj.coffee_table", "Table basse" },
            { "obj.drawer", "Commode" },
            { "obj.plant_big", "Grande plante" },
            { "obj.plant_small", "Petite plante" },
            { "obj.shelf", "Étagère" },
            { "obj.shelf_v1", "Étagère var." },
            { "obj.aquarium", "Aquarium" },
            { "obj.carpet_confort", "Tapis Confort" },
            { "obj.carpet_play", "Tapis Jeu" },
            { "obj.carpet_cosmic", "Tapis Cosmique" },

            // --- Cat Names ---
            { "names.pool", "Minou,Felix,Caramel,Luna,Tigrou,Noisette,Moustache,Pacha,Cannelle,Gribouille,Minette,Câlin,Filou,Chipie,Réglisse,Perle,Simba,Plume,Biscuit,Cookie,Praline,Nougat,Macaron,Brioche,Muffin,Crumble,Tiramisu,Meringue,Brownie,Fudge,Cosmos,Lune,Étoile,Comète,Nova,Nebula,Soleil,Aurore,Eclipse,Galaxie,Astro,Pluton,Rubis,Saphir,Émeraude,Jade,Opale,Ambre,Topaze,Diamant,Cristal,Onyx,Ivoire,Corail,Ninja,Pixel,Wifi,Sushi,Tofu,Wasabi,Mozart,Picasso,Darwin,Merlin,Zorro,Gatsby,Chouette,Papillon,Colibri,Hibou,Renard,Loutre,Belle,Roméo,Vanille,Sucre,Cacao,Latte,Madeleine,Vega,Sirius,Andromède,Apollo,Atlas,Mars,Indigo,Améthyste,Turquoise,Quartz,Voltaire,Hugo,Tesla,Curie,Athéna,Hermès,Iris,Pandora,Chouquette,Lapin,Belette,Castor,Vortex,Matrix" },
            // Per-breed extra names: appended to the common pool when a cat of this breed spawns.
            { "names.pool.Breed_Europeen2", "Odyssée" },
        };

        // ==================== ENGLISH ====================
        public static readonly Dictionary<string, string> English = new()
        {
            // --- Cat Info Panel ---
            { "cat.kitten", "Kitten" },
            { "cat.adult", "Adult cat" },
            { "cat.stay.pension", "Boarding" },
            { "cat.stay.refuge", "Shelter arrival" },
            { "cat.likes", "Likes {0}" },
            { "cat.dislikes", "Dislikes {0}" },
            { "cat.time.none", "--:--:--" },

            // --- End Pension Panel ---
            { "pension.bye", "Goodbye {0}!" },

            // --- Loading ---
            { "ui.loading", "Loading" },
            { "loading.tip.0", "Pet your cats to make them happy." },
            { "loading.tip.1", "An unhappy cat may leave without paying." },
            { "loading.tip.2", "Long-press an object to move it." },
            { "loading.tip.3", "Carpets boost comfort in their area." },
            { "loading.tip.4", "Some breeds don't get along — keep an eye on them." },
            { "loading.tip.5", "The better your rep, the pickier the cats." },
            { "loading.tip.6", "Stairs lead to the upper floors." },
            { "loading.tip.7", "Grab your coins quickly before they fade away." },

            // --- Tutorial (Narration) ---
            { "tuto.speaker.jasper", "Jasper" },
            { "tuto.speaker.winston", "Winston" },
            { "tuto.skip.link", "Skip tutorial" },
            { "tuto.skip.ask", "Skip tutorial?" },
            { "tuto.skip.yes", "Yes" },
            { "tuto.skip.no", "No" },
            { "tuto.welcome", "Welcome to <b>Meowtel</b>!\nI'm Jasper, the concierge." },
            { "tuto.explain", "Here, you welcome cats for boarding and shelter. Take good care of them and they'll reward you!" },
            { "tuto.floors", "The hotel has <b>10 floors</b> to unlock above the ground floor. The bigger your hotel, the more cats you can host!" },
            { "tuto.levelup.intro", "To unlock a new floor, you need to <b>level up your reputation</b>." },
            { "tuto.levelup.xp", "Earn <b>XP</b> by keeping cats happy. Every successful boarding and adoption rewards XP." },
            { "tuto.levelup.cond", "To level up, you need: enough <b>XP</b>, enough <b>cats above 75% happiness</b>, and to pay in <b>Cat Coins</b>." },
            { "tuto.levelup.tap", "Tap the <b>level bar</b> at the top to see your current conditions." },
            { "tuto.levelup.done", "Great! Come back here regularly to grow the hotel. Close the panel to continue." },
            { "tuto.coins", "The <b>currency</b> here is the Cat Coin. Buy items for your cats... and pay for level-ups!" },
            // --- HUD elements walkthrough ---
            { "tuto.hud.coins", "This is your <b>Cat Coin reserve</b>. It fills up when you collect coins left by happy cats. Used to buy items and level up." },
            { "tuto.hud.purrls", "<b>Puurls</b> are the premium currency. Later, they'll let you buy special items with big bonuses, or activate revenue boosts." },
            { "tuto.hud.capacity", "The hotel's <b>capacity</b>: percentage of slots occupied compared to the max allowed by your level." },
            { "tuto.hud.comfort", "The hotel's <b>average comfort</b>. Base 50%, +5% if the hotel is sparsely populated (<20%), rises with decoration items, and drops when overcrowded (>80%)." },
            { "tuto.hud.floors", "The currently displayed <b>floor</b>. Use the arrows to go up and down once you've unlocked some." },
            { "tuto.hud.system", "The <b>pause menu</b>. Settings, language, sound, and game options." },
            { "tuto.hud.collectall", "<b>Collect all coins</b> at once. Cost: 5 Cat Coins. Handy when the hotel fills up!" },
            { "tuto.hud.addboost", "Watch an ad to <b>double your revenue for 60s</b>. Cooldown: 120s between two activations." },
            { "tuto.hud.nextcat", "The <b>countdown</b> before the next cat arrives. The more your hotel grows, the faster the rhythm." },
            { "tuto.hud.shop", "The <b>item shop</b>. Buy everything you need here to fulfill your cats' needs." },
            { "tuto.pension", "Cats arrive for <b>boarding</b> for a set time. Keep their happiness as high as possible to earn the most Cat Coins!" },
            { "tuto.refuge", "Sad cats will arrive through the <b>shelter</b>. Make them happy so they can be adopted!" },
            { "tuto.unhappy", "Warning: a cat whose happiness drops below <b>25%</b> will try to leave. Don't let that happen!" },
            { "tuto.firstcat", "Oh! Your first guest is arriving! Watch..." },
            { "tuto.selectcat", "Tap on the cat to select it." },
            { "tuto.catinfo", "Here's their info: name, breed, mood and needs. Keep an eye on it!" },
            { "tuto.pensiontime", "Check the remaining boarding time. When it runs out, the cat leaves. The happier they are, the more you earn!" },
            { "tuto.refugecat", "A shelter cat has arrived! It's hungry... Look at its gauges: it needs food." },
            { "tuto.buyfood", "Open the shop and buy a <b>food bowl</b>, then place it in the room." },
            { "tuto.waiteat", "Great! Wait for the cat to eat..." },
            { "tuto.collectcoin", "A coin appeared! Tap on it to collect your <b>Cat Coins</b>." },
            { "tuto.thirsty", "It's thirsty now! Buy some <b>water</b> and place it." },
            { "tuto.waitdrink", "Let it drink in peace..." },
            { "tuto.sleepy", "The cat is tired. Get it a <b>bed</b> or a <b>pillow</b>!" },
            { "tuto.waitsleep", "It's resting... shh!" },
            { "tuto.dirty", "It needs a <b>litter box</b>! Buy one." },
            { "tuto.waitclean", "Grooming time..." },
            { "tuto.bored", "It wants to play! Buy a <b>wool ball</b>." },
            { "tuto.waitplay", "Watch it have fun!" },
            { "tuto.complete", "Well done! You know the basics. The hotel is yours, good luck!" },

            // --- Sound ---
            { "sound.on", "Sound on" },
            { "sound.off", "Sound off" },

            // --- Mood ---
            { "mood.happy", "Happy" },
            { "mood.ecstatic", "Ecstatic" },
            { "mood.depressed", "Depressed" },
            { "mood.aggressive", "Aggressive" },
            { "mood.angry", "Angry" },

            // --- Needs ---
            { "need.hungry", "Hungry" },
            { "need.thirsty", "Thirsty" },
            { "need.tired", "Tired" },
            { "need.bored", "Bored" },
            { "need.dirty", "Dirty" },

            // --- HUD ---
            { "hud.floor.ground", "Ground floor" },
            { "hud.floor", "Floor {0}" },
            { "hud.level", "Level {0}" },
            { "levelup.title.current", "Level {0}" },
            { "levelup.title.next", "Level {0}" },
            { "levelup.newstuff", "What's new" },
            { "levelup.condition.xp", "{0} / {1} XP" },
            { "levelup.condition.happycats", "{0} cats > 75% happy out of {1}" },
            { "levelup.newfloor", "Floor {0} unlocked" },
            { "levelup.capacity.title", "Hotel capacity" },
            { "levelup.go.next", "Go to next level" },
            { "hud.level.max", "MAX" },
            { "hud.level.max.reached", "Maximum level reached!" },
            { "hud.level.objective", "{0}/{1} cats at +{2}% happiness" },
            { "hud.boost.active", "x2 Boost active! {0}s" },

            // --- Reputation Level Names ---
            { "rep.0", "Beginner" },
            { "rep.1", "Novice" },
            { "rep.2", "Skilled" },
            { "rep.3", "Professional" },
            { "rep.4", "Expert" },
            { "rep.5", "Renowned" },
            { "rep.6", "Famous" },
            { "rep.7", "Prestigious" },
            { "rep.8", "Elite" },
            { "rep.9", "Legendary" },
            { "rep.10", "Cat Master" },

            // --- Shop Categories ---
            { "shop.beds", "Beds" },
            { "shop.pillows", "Pillows" },
            { "shop.croquettes", "Food bowls" },
            { "shop.water", "Water" },
            { "shop.balls", "Balls" },
            { "shop.scratchers", "Scratchers" },
            { "shop.litters", "Litter boxes" },
            { "shop.frames", "Frames" },
            { "shop.lamps", "Lamps" },
            { "shop.tables", "Tables" },
            { "shop.plants", "Plants" },
            { "shop.shelves", "Shelves" },
            { "shop.aquariums", "Aquariums" },
            { "shop.carpets", "Carpets" },
            { "shop.trees", "Cat trees" },

            // --- Breed Names ---
            { "breed.europeen", "European" },
            { "breed.siamois", "Siamese" },
            { "breed.ragdoll", "Ragdoll" },
            { "breed.siberien", "Siberian" },
            { "breed.chartreux", "Chartreux" },

            // --- Personality: Race Traits ---
            { "trait.hunger", "Glutton" },
            { "trait.thirst", "Thirsty" },
            { "trait.sleep", "Sleepyhead" },
            { "trait.play", "Playful" },
            { "trait.clean", "Neat freak" },
            { "trait.aggressive", "Fighter" },
            { "trait.big", "Large" },
            { "trait.small", "Small" },
            { "trait.fast", "Quick" },
            { "trait.slow", "Laid-back" },

            // --- Personality: Personality Pool ---
            { "personality.cuddly", "Cuddly" },
            { "personality.independent", "Independent" },
            { "personality.curious", "Curious" },
            { "personality.fearful", "Fearful" },
            { "personality.affectionate", "Affectionate" },
            { "personality.observer", "Observer" },
            { "personality.discreet", "Discreet" },
            { "personality.clingy", "Clingy" },
            { "personality.adventurer", "Adventurous" },
            { "personality.territorial", "Territorial" },
            { "personality.sociable", "Sociable" },
            { "personality.solitary", "Loner" },
            { "personality.clever", "Clever" },
            { "personality.lazy", "Lazy" },
            { "personality.loyal", "Loyal" },
            { "personality.capricious", "Fickle" },

            // --- Personality: Quirky Pool ---
            { "quirk.grudge", "Holds grudges" },
            { "quirk.touchy", "Touchy" },
            { "quirk.dumb", "Clueless" },
            { "quirk.big_eater", "Big eater" },
            { "quirk.drinker", "Water hog" },
            { "quirk.food_thief", "Food thief" },
            { "quirk.snorer", "Snorer" },
            { "quirk.litter_fear", "Litter-shy" },
            { "quirk.jealous", "Jealous" },
            { "quirk.show_off", "Show-off" },
            { "quirk.drama_queen", "Drama queen" },
            { "quirk.hair_collector", "Fur collector" },
            { "quirk.grumpy", "Grumpy" },
            { "quirk.clumsy", "Clumsy" },
            { "quirk.hypochondriac", "Hypochondriac" },
            { "quirk.couch_king", "Couch potato" },
            { "quirk.snob", "Snob" },
            { "quirk.pilferer", "Pilferer" },

            // --- Scene Labels ---
            // Boot scene
            { "ui.play", "Play" },
            { "ui.continue", "Continue" },
            { "ui.new_game", "New game" },

            // Credits panel
            { "credits.title", "Credits" },
            { "credits.gameby", "a game by" },
            { "credits.direction", "Direction / development" },
            { "credits.disclaimer", "This game was created using the Unity engine, owned by Unity Technologies. Unity and the Unity logo are trademarks of Unity Technologies in the United States of America and in other countries" },
            { "ui.credits", "Credits" },
            { "ui.parameters", "Settings" },

            // Options panel
            { "ui.resume", "Resume" },
            { "ui.main_menu", "Main menu" },
            { "ui.meowdex", "Meowdex" },

            // Parameters panel
            { "param.languages", "Languages" },
            { "param.notifications", "Push notifications" },
            { "param.battery", "Battery saver" },
            { "param.music_volume", "Music volume" },
            { "param.effects_volume", "Effects volume" },

            // Shop
            { "ui.shop", "Shop" },
            { "shop.category.food", "Food" },
            { "shop.category.drink", "Drink" },
            { "shop.category.sleep", "Sleep" },
            { "shop.category.play", "Play" },
            { "shop.category.clean", "Hygiene" },
            { "shop.category.comfort", "Comfort" },
            { "shop.category.deco", "Decoration" },
            { "shop.double_gains", "Double earnings" },

            // Cat Info Panel
            { "cat.name_label", "Cat name" },
            { "cat.race_label", "Breed" },
            { "cat.age_label", "Age" },
            { "cat.character_label", "Personality" },
            { "cat.affinity_label", "Affinities" },
            { "cat.needs_label", "Needs" },
            { "cat.need.hunger", "Hunger" },
            { "cat.need.thirst", "Thirst" },
            { "cat.need.sleep", "Sleep" },
            { "cat.need.play", "Play" },
            { "cat.need.clean", "Hygiene" },
            { "cat.need.happiness", "Happiness" },

            // End Pension Panel
            { "pension.title", "Boarding stay complete!" },
            { "pension.happiness", "Average happiness during stay:" },
            { "pension.details", "Payment details" },
            { "pension.base", "Base" },
            { "pension.tip", "Tip" },
            { "pension.total", "Total" },
            { "pension.time_remaining", "Time remaining" },

            // HUD
            { "hud.boost.label", "x2 coin collection boost active!" },

            // Object display names
            { "obj.bed", "Bed" },
            { "obj.luxury_bed", "Luxury bed" },
            { "obj.pillow", "Pillow" },
            { "obj.food_bowl", "Food bowl" },
            { "obj.food_bowl_v2", "Food bowl v.2" },
            { "obj.food_bowl_v3", "Food bowl v.3" },
            { "obj.food_bowl_v4", "Food bowl v.4" },
            { "obj.water_bowl", "Water bowl" },
            { "obj.water_bowl_04", "Modern water bowl" },
            { "obj.water_bowl_v2", "Water bowl v.2" },
            { "obj.water_bowl_v3", "Water bowl v.3" },
            { "obj.wool_ball", "Wool ball" },
            { "obj.cat_tree", "Cat tree" },
            { "obj.scratcher", "Scratcher" },
            { "obj.litter", "Litter box" },
            { "obj.litter_v1", "Litter box v." },
            { "obj.frame_1", "Frame 1" },
            { "obj.frame_2", "Frame 2" },
            { "obj.frame_3", "Frame 3" },
            { "obj.painting", "Painting" },
            { "obj.lamp", "Floor lamp" },
            { "obj.coffee_table", "Coffee table" },
            { "obj.drawer", "Dresser" },
            { "obj.plant_big", "Large plant" },
            { "obj.plant_small", "Small plant" },
            { "obj.shelf", "Shelf" },
            { "obj.shelf_v1", "Shelf v." },
            { "obj.aquarium", "Aquarium" },
            { "obj.carpet_confort", "Comfort carpet" },
            { "obj.carpet_play", "Play carpet" },
            { "obj.carpet_cosmic", "Cosmic carpet" },

            // --- Cat Names ---
            { "names.pool", "Whiskers,Felix,Caramel,Luna,Tigger,Hazel,Mittens,Paws,Cinnamon,Scribbles,Kitty,Cuddles,Rascal,Sassy,Licorice,Pearl,Simba,Feather,Biscuit,Cookie,Praline,Nougat,Macaron,Muffin,Crumble,Tiramisu,Brownie,Fudge,Marshmallow,Toffee,Cosmos,Moon,Star,Comet,Nova,Nebula,Sunny,Aurora,Eclipse,Galaxy,Astro,Pluto,Ruby,Sapphire,Emerald,Jade,Opal,Amber,Topaz,Diamond,Crystal,Onyx,Ivory,Coral,Ninja,Pixel,Wifi,Sushi,Tofu,Wasabi,Mozart,Picasso,Darwin,Merlin,Zorro,Gatsby,Owl,Butterfly,Hummingbird,Shadow,Fox,Otter,Belle,Romeo,Vanilla,Sugar,Cocoa,Latte,Madeleine,Vega,Sirius,Andromeda,Apollo,Atlas,Mars,Indigo,Amethyst,Turquoise,Quartz,Voltaire,Hugo,Tesla,Curie,Athena,Hermes,Iris,Pandora,Cookieface,Bunny,Weasel,Beaver,Vortex,Matrix" },
            // Per-breed extra names: appended to the common pool when a cat of this breed spawns.
            { "names.pool.Breed_Europeen2", "Odyssey" },
        };

        // ==================== GERMAN ====================
        public static readonly Dictionary<string, string> German = new()
        {
            // --- Cat Info Panel ---
            { "cat.kitten", "Kätzchen" },
            { "cat.adult", "Erwachsene Katze" },
            { "cat.stay.pension", "In Pension" },
            { "cat.stay.refuge", "Im Tierheim" },
            { "cat.likes", "Mag {0}" },
            { "cat.dislikes", "Mag keine {0}" },
            { "cat.time.none", "--:--:--" },

            // --- End Pension Panel ---
            { "pension.bye", "Auf Wiedersehen {0}!" },

            // --- Loading ---
            { "ui.loading", "Lädt" },
            { "loading.tip.0", "Streichle deine Katzen, um sie glücklich zu machen." },
            { "loading.tip.1", "Eine unzufriedene Katze geht ohne zu zahlen." },
            { "loading.tip.2", "Tippe doppelt auf ein Objekt, um es zu verschieben." },
            { "loading.tip.3", "Teppiche bieten einen Komfortbonus für die Zone." },
            { "loading.tip.4", "Manche Rassen vertragen sich nicht — pass auf sie auf." },
            { "loading.tip.5", "Je angesehener dein Hotel, desto anspruchsvoller die Katzen." },
            { "loading.tip.6", "Treppen führen zu den oberen Stockwerken." },
            { "loading.tip.7", "Sammle deine Münzen schnell, bevor sie verschwinden." },

            // --- Tutorial (Narration) ---
            { "tuto.speaker.jasper", "Jasper" },
            { "tuto.speaker.winston", "Winston" },
            { "tuto.skip.link", "Tutorial überspringen" },
            { "tuto.skip.ask", "Tutorial überspringen?" },
            { "tuto.skip.yes", "Ja" },
            { "tuto.skip.no", "Nein" },
            { "tuto.welcome", "Willkommen bei <b>Meowtel</b>!\nIch bin Jasper, der Concierge." },
            { "tuto.explain", "Hier nimmst du Katzen in Pension und ins Tierheim auf. Kümmere dich um sie und sie werden dich belohnen!" },
            { "tuto.floors", "Das Hotel hat <b>10 Etagen</b> über dem Erdgeschoss freizuschalten. Je größer dein Hotel, desto mehr Katzen kannst du beherbergen!" },
            { "tuto.levelup.intro", "Um eine neue Etage freizuschalten, musst du <b>im Ruf aufsteigen</b>." },
            { "tuto.levelup.xp", "Verdiene <b>XP</b>, indem du Katzen glücklich machst. Jede erfolgreiche Pension und jede Adoption bringen XP." },
            { "tuto.levelup.cond", "Zum Aufsteigen brauchst du: genug <b>XP</b>, genug <b>Katzen über 75% Glück</b>, und du musst in <b>Cat Coins</b> bezahlen." },
            { "tuto.levelup.tap", "Tippe oben auf die <b>Levelleiste</b>, um deine aktuellen Bedingungen zu sehen." },
            { "tuto.levelup.done", "Perfekt! Komm regelmäßig hierher zurück, um das Hotel wachsen zu lassen. Schließe das Panel zum Fortfahren." },
            { "tuto.coins", "Die <b>Währung</b> hier ist der Cat Coin. Kaufe Objekte für deine Katzen... und bezahle Levelaufstiege!" },
            // --- HUD elements walkthrough ---
            { "tuto.hud.coins", "Das ist deine <b>Cat-Coin-Reserve</b>. Sie füllt sich, wenn du die Münzen glücklicher Katzen einsammelst. Wird zum Kaufen und Aufsteigen verwendet." },
            { "tuto.hud.purrls", "<b>Puurls</b> sind die Premium-Währung. Später kannst du damit besondere Objekte mit großen Boni kaufen oder Einnahmen-Boosts aktivieren." },
            { "tuto.hud.capacity", "Die <b>Auslastung</b> des Hotels: Prozent der belegten Plätze im Verhältnis zum Maximum deiner Stufe." },
            { "tuto.hud.comfort", "Der <b>durchschnittliche Komfort</b> des Hotels. Basis 50%, +5% bei wenig Belegung (<20%), steigt mit Dekoration und sinkt bei Überfüllung (>80%)." },
            { "tuto.hud.floors", "Die aktuell angezeigte <b>Etage</b>. Mit den Pfeilen wechselst du zwischen freigeschalteten Etagen." },
            { "tuto.hud.system", "Das <b>Pause-Menü</b>. Einstellungen, Sprache, Ton und Spieloptionen." },
            { "tuto.hud.collectall", "<b>Sammle alle Münzen</b> auf einmal. Kosten: 5 Cat Coins. Praktisch wenn das Hotel voll ist!" },
            { "tuto.hud.addboost", "Sieh dir eine Werbung an, um <b>deine Einnahmen 60s lang zu verdoppeln</b>. Cooldown: 120s zwischen Aktivierungen." },
            { "tuto.hud.nextcat", "Der <b>Countdown</b> bis zur nächsten Katze. Je größer dein Hotel, desto schneller der Rhythmus." },
            { "tuto.hud.shop", "Der <b>Objekt-Shop</b>. Hier kaufst du alles, was deine Katzen brauchen." },
            { "tuto.pension", "Katzen kommen für eine festgelegte Zeit in <b>Pension</b>. Halte ihr Glück möglichst hoch, um maximal Cat Coins zu verdienen!" },
            { "tuto.refuge", "Traurige Katzen kommen über das <b>Tierheim</b>. Mach sie glücklich, damit sie adoptiert werden können!" },
            { "tuto.unhappy", "Achtung: Eine Katze, deren Glück unter <b>25%</b> fällt, will gehen. Lass das nicht zu!" },
            { "tuto.firstcat", "Oh! Dein erster Gast kommt! Beobachte ihn..." },
            { "tuto.selectcat", "Tippe auf die Katze, um sie auszuwählen." },
            { "tuto.catinfo", "Hier sind ihre Infos: Name, Rasse, Stimmung und Bedürfnisse. Behalte sie im Auge!" },
            { "tuto.pensiontime", "Schau auf die verbleibende Pensionszeit. Wenn sie abläuft, geht die Katze. Je glücklicher sie ist, desto mehr verdienst du!" },
            { "tuto.refugecat", "Eine Tierheim-Katze ist da! Sie hat Hunger... Schau dir ihre Anzeigen an: Sie braucht Futter." },
            { "tuto.buyfood", "Öffne den Shop und kaufe einen <b>Futternapf</b>, dann stell ihn ins Zimmer." },
            { "tuto.waiteat", "Gut! Warte, bis die Katze frisst..." },
            { "tuto.collectcoin", "Eine Münze ist erschienen! Tippe darauf, um deine <b>Cat Coins</b> zu sammeln." },
            { "tuto.thirsty", "Sie hat jetzt Durst! Kauf <b>Wasser</b> und stell es hin." },
            { "tuto.waitdrink", "Lass sie in Ruhe trinken..." },
            { "tuto.sleepy", "Die Katze ist müde. Bring ihr ein <b>Bett</b> oder ein <b>Kissen</b>!" },
            { "tuto.waitsleep", "Sie ruht sich aus... pssst!" },
            { "tuto.dirty", "Sie braucht eine <b>Katzentoilette</b>! Kauf eine." },
            { "tuto.waitclean", "Sie putzt sich..." },
            { "tuto.bored", "Sie will spielen! Kauf einen <b>Wollknäuel</b>." },
            { "tuto.waitplay", "Sieh ihr beim Spielen zu!" },
            { "tuto.complete", "Bravo! Du kennst die Grundlagen. Das Hotel gehört dir, viel Glück!" },

            // --- Sound ---
            { "sound.on", "Ton an" },
            { "sound.off", "Ton aus" },

            // --- Mood ---
            { "mood.happy", "Zufrieden" },
            { "mood.ecstatic", "Begeistert" },
            { "mood.depressed", "Niedergeschlagen" },
            { "mood.aggressive", "Aggressiv" },
            { "mood.angry", "Wütend" },

            // --- Needs ---
            { "need.hungry", "Hungrig" },
            { "need.thirsty", "Durstig" },
            { "need.tired", "Müde" },
            { "need.bored", "Gelangweilt" },
            { "need.dirty", "Schmutzig" },

            // --- HUD ---
            { "hud.floor.ground", "EG" },
            { "hud.floor", "Etage {0}" },
            { "hud.level", "Stufe {0}" },
            { "levelup.title.current", "Stufe {0}" },
            { "levelup.title.next", "Stufe {0}" },
            { "levelup.newstuff", "Neuheiten" },
            { "levelup.condition.xp", "{0} / {1} XP" },
            { "levelup.condition.happycats", "{0} von {1} Katzen > 75% Glück" },
            { "levelup.newfloor", "Etage {0} freigeschaltet" },
            { "levelup.capacity.title", "Hotelkapazität" },
            { "levelup.go.next", "Zur nächsten Stufe" },
            { "hud.level.max", "MAX" },
            { "hud.level.max.reached", "Maximalstufe erreicht!" },
            { "hud.level.objective", "{0}/{1} Katzen mit +{2}% Glück" },
            { "hud.boost.active", "x2 Boost aktiv! {0}s" },

            // --- Reputation Level Names ---
            { "rep.0", "Anfänger" },
            { "rep.1", "Amateur" },
            { "rep.2", "Geübt" },
            { "rep.3", "Profi" },
            { "rep.4", "Experte" },
            { "rep.5", "Bekannt" },
            { "rep.6", "Berühmt" },
            { "rep.7", "Renommiert" },
            { "rep.8", "Elite" },
            { "rep.9", "Legendär" },
            { "rep.10", "Katzenmeister" },

            // --- Shop Categories ---
            { "shop.beds", "Betten" },
            { "shop.pillows", "Kissen" },
            { "shop.croquettes", "Futternäpfe" },
            { "shop.water", "Wasser" },
            { "shop.balls", "Bälle" },
            { "shop.scratchers", "Kratzpfosten" },
            { "shop.litters", "Katzentoiletten" },
            { "shop.frames", "Bilderrahmen" },
            { "shop.lamps", "Lampen" },
            { "shop.tables", "Tische" },
            { "shop.plants", "Pflanzen" },
            { "shop.shelves", "Regale" },
            { "shop.aquariums", "Aquarien" },
            { "shop.carpets", "Teppiche" },
            { "shop.trees", "Kratzbäume" },

            // --- Breed Names ---
            { "breed.europeen", "Europäer" },
            { "breed.siamois", "Siamkatze" },
            { "breed.ragdoll", "Ragdoll" },
            { "breed.siberien", "Sibirische Katze" },
            { "breed.chartreux", "Kartäuser" },

            // --- Personality: Race Traits ---
            { "trait.hunger", "Vielfraß" },
            { "trait.thirst", "Durstig" },
            { "trait.sleep", "Schlafmütze" },
            { "trait.play", "Verspielt" },
            { "trait.clean", "Reinlich" },
            { "trait.aggressive", "Kämpfer" },
            { "trait.big", "Groß" },
            { "trait.small", "Klein" },
            { "trait.fast", "Flink" },
            { "trait.slow", "Gemütlich" },

            // --- Personality: Personality Pool ---
            { "personality.cuddly", "Verschmust" },
            { "personality.independent", "Unabhängig" },
            { "personality.curious", "Neugierig" },
            { "personality.fearful", "Ängstlich" },
            { "personality.affectionate", "Anhänglich" },
            { "personality.observer", "Beobachter" },
            { "personality.discreet", "Diskret" },
            { "personality.clingy", "Klette" },
            { "personality.adventurer", "Abenteurer" },
            { "personality.territorial", "Territorial" },
            { "personality.sociable", "Gesellig" },
            { "personality.solitary", "Einzelgänger" },
            { "personality.clever", "Schlau" },
            { "personality.lazy", "Faul" },
            { "personality.loyal", "Treu" },
            { "personality.capricious", "Launisch" },

            // --- Personality: Quirky Pool ---
            { "quirk.grudge", "Nachtragend" },
            { "quirk.touchy", "Empfindlich" },
            { "quirk.dumb", "Tollpatschig" },
            { "quirk.big_eater", "Vielfraß" },
            { "quirk.drinker", "Säufer" },
            { "quirk.food_thief", "Futterdieb" },
            { "quirk.snorer", "Schnarcher" },
            { "quirk.litter_fear", "Toilettenscheu" },
            { "quirk.jealous", "Eifersüchtig" },
            { "quirk.show_off", "Angeber" },
            { "quirk.drama_queen", "Drama-Queen" },
            { "quirk.hair_collector", "Haarsammler" },
            { "quirk.grumpy", "Mürrisch" },
            { "quirk.clumsy", "Ungeschickt" },
            { "quirk.hypochondriac", "Hypochonder" },
            { "quirk.couch_king", "Couch-König" },
            { "quirk.snob", "Snob" },
            { "quirk.pilferer", "Langfinger" },

            // --- Scene Labels ---
            // Boot scene
            { "ui.play", "Spielen" },
            { "ui.continue", "Spiel fortsetzen" },
            { "ui.new_game", "Neues Spiel" },

            // Credits panel
            { "credits.title", "Mitwirkende" },
            { "credits.gameby", "ein Spiel von" },
            { "credits.direction", "Leitung / Entwicklung" },
            { "credits.disclaimer", "Dieses Spiel wurde mit der Unity-Engine erstellt, die Eigentum von Unity Technologies ist. Unity und das Unity-Logo sind eingetragene Marken von Unity Technologies in den Vereinigten Staaten von Amerika und in anderen Ländern" },
            { "ui.credits", "Mitwirkende" },
            { "ui.parameters", "Einstellungen" },

            // Options panel
            { "ui.resume", "Weiter" },
            { "ui.main_menu", "Hauptmenü" },
            { "ui.meowdex", "Meowdex" },

            // Parameters panel
            { "param.languages", "Sprachen" },
            { "param.notifications", "Push-Benachrichtigungen" },
            { "param.battery", "Energiesparmodus" },
            { "param.music_volume", "Musiklautstärke" },
            { "param.effects_volume", "Effektlautstärke" },

            // Shop
            { "ui.shop", "Shop" },
            { "shop.category.food", "Futter" },
            { "shop.category.drink", "Getränk" },
            { "shop.category.sleep", "Schlaf" },
            { "shop.category.play", "Spiel" },
            { "shop.category.clean", "Hygiene" },
            { "shop.category.comfort", "Komfort" },
            { "shop.category.deco", "Dekoration" },
            { "shop.double_gains", "Einnahmen verdoppeln" },

            // Cat Info Panel
            { "cat.name_label", "Katzenname" },
            { "cat.race_label", "Rasse" },
            { "cat.age_label", "Alter" },
            { "cat.character_label", "Charakter" },
            { "cat.affinity_label", "Vorlieben" },
            { "cat.needs_label", "Bedürfnisse" },
            { "cat.need.hunger", "Hunger" },
            { "cat.need.thirst", "Durst" },
            { "cat.need.sleep", "Schlaf" },
            { "cat.need.play", "Spiel" },
            { "cat.need.clean", "Hygiene" },
            { "cat.need.happiness", "Glück" },

            // End Pension Panel
            { "pension.title", "Pensionsaufenthalt beendet!" },
            { "pension.happiness", "Durchschnittliches Glück während des Aufenthalts:" },
            { "pension.details", "Zahlungsdetails" },
            { "pension.base", "Basis" },
            { "pension.tip", "Trinkgeld" },
            { "pension.total", "Gesamt" },
            { "pension.time_remaining", "Verbleibende Zeit" },

            // HUD
            { "hud.boost.label", "x2 Münzensammel-Boost aktiv!" },

            // Object display names (shop items)
            { "obj.bed", "Bett" },
            { "obj.luxury_bed", "Luxusbett" },
            { "obj.pillow", "Kissen" },
            { "obj.food_bowl", "Futternapf" },
            { "obj.food_bowl_v2", "Futternapf V.2" },
            { "obj.food_bowl_v3", "Futternapf V.3" },
            { "obj.food_bowl_v4", "Futternapf V.4" },
            { "obj.water_bowl", "Wassernapf" },
            { "obj.water_bowl_04", "Moderner Wassernapf" },
            { "obj.water_bowl_v2", "Wassernapf V.2" },
            { "obj.water_bowl_v3", "Wassernapf V.3" },
            { "obj.wool_ball", "Wollknäuel" },
            { "obj.cat_tree", "Kratzbaum" },
            { "obj.scratcher", "Kratzpfosten" },
            { "obj.litter", "Katzentoilette" },
            { "obj.litter_v1", "Katzentoilette V." },
            { "obj.frame_1", "Bilderrahmen 1" },
            { "obj.frame_2", "Bilderrahmen 2" },
            { "obj.frame_3", "Bilderrahmen 3" },
            { "obj.painting", "Gemälde" },
            { "obj.lamp", "Stehlampe" },
            { "obj.coffee_table", "Couchtisch" },
            { "obj.drawer", "Kommode" },
            { "obj.plant_big", "Große Pflanze" },
            { "obj.plant_small", "Kleine Pflanze" },
            { "obj.shelf", "Regal" },
            { "obj.shelf_v1", "Regal V." },
            { "obj.aquarium", "Aquarium" },
            { "obj.carpet_confort", "Komfort-Teppich" },
            { "obj.carpet_play", "Spiel-Teppich" },
            { "obj.carpet_cosmic", "Kosmischer Teppich" },

            // --- Cat Names ---
            { "names.pool", "Mieze,Felix,Karamell,Luna,Tiger,Haselnuss,Schnurri,Pascha,Zimt,Krickel,Kätzchen,Kuschel,Schlingel,Frechdachs,Lakritze,Perle,Simba,Feder,Keks,Cookie,Praline,Nougat,Macaron,Brioche,Muffin,Crumble,Tiramisu,Baiser,Brownie,Fudge,Kosmos,Mond,Stern,Komet,Nova,Nebula,Sonne,Aurora,Eclipse,Galaxie,Astro,Pluto,Rubin,Saphir,Smaragd,Jade,Opal,Bernstein,Topas,Diamant,Kristall,Onyx,Elfenbein,Koralle,Ninja,Pixel,Wifi,Sushi,Tofu,Wasabi,Mozart,Picasso,Darwin,Merlin,Zorro,Gatsby,Eule,Schmetterling,Kolibri,Schatten,Fuchs,Otter,Odyssee,Belle,Romeo,Vanille,Zucker,Kakao,Latte,Madeleine,Vega,Sirius,Andromeda,Apollo,Atlas,Mars,Indigo,Amethyst,Türkis,Quarz,Voltaire,Hugo,Tesla,Curie,Athena,Hermes,Iris,Pandora,Schmunzel,Hase,Wiesel,Biber,Vortex,Matrix" },
        };
    }
}
