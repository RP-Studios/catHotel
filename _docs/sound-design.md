# Cat Hotel Tycoon - Sound Design
## Liste des assets audio

**Version:** 1.0
**Date:** Mars 2026
**Style:** Cartoon / mignon, sons satisfaisants et chaleureux
**Format:** WAV
**Inspirations:** Neko Atsume, Animal Crossing, Two Point Hospital

---

## Documents associes

| Document | Contenu |
|----------|---------|
| **[gdd.md](gdd.md)** | Design complet du jeu |
| **[ui-ux.md](ui-ux.md)** | Ecrans, flux de navigation |
| **[2d-animation.md](2d-animation.md)** | Animations (sync audio) |
| **[2d-art.md](2d-art.md)** | Assets graphiques |
| **[jalons.md](jalons.md)** | Roadmap par phases |

---

## 1. DIRECTION SONORE

### 1.1 Principes generaux

- **Ton :** Chaleureux, mignon, cartoon — jamais agressif, jamais realiste
- **Satisfaction :** Chaque interaction du joueur doit produire un retour audio satisfaisant (taps, collectes, caresses)
- **Lisibilite :** Les sons de feedback (succes, erreur, alerte) doivent etre immediatement identifiables sans regarder l'ecran
- **Non-intrusif :** Les sons ambiants et de chats doivent creer une atmosphere cosy sans saturer l'espace sonore
- **Variations :** Les sons frequemment joues (collecte de pieces, pas, miaulements) doivent avoir des variantes ou de la modulation de pitch pour eviter la fatigue auditive

### 1.2 Nomenclature des fichiers

```
{categorie}_{sous-categorie}_{action}_{variante}.wav
```

Exemples :
- `cat_europeen_meow_happy_01.wav`
- `cat_siamois_meow_happy_01.wav`
- `ui_button_click_positive.wav`
- `gameplay_coin_collect_01.wav`
- `event_adoption_fanfare.wav`
- `music_gameplay_loop.wav`

---

## 2. MUSIQUE

### 2.1 Theme principal (loop gameplay)

Le theme principal tourne en boucle pendant le gameplay dans l'hotel.

| # | Piste | Duree | Loop | Description |
|---|-------|-------|------|-------------|
| 1 | **Gameplay loop** | 2:00-3:00 | Oui, seamless | Melodie lofi / jazzy, detendue et enjouee. Piano doux, guitare acoustique legere, basse ronde. Tempo lent (~90 BPM). Doit rester agreable apres des heures d'ecoute |

### 2.2 Jingles (one-shots)

Courts jingles joues sur des evenements importants. Ils se superposent a la musique de fond (duck le volume de la loop pendant le jingle).

| # | Jingle | Duree | Description |
|---|--------|-------|-------------|
| 1 | **Bienvenue / lancement** | 3-4s | Joue apres le splash screen. Court, memorable, identifie le jeu. Quelques notes de piano + clochettes |
| 2 | **Arrivee de chat** | 2s | Court motif joyeux et curieux. Annonce un nouveau visiteur |
| 3 | **Depart pension (paiement)** | 2s | Motif satisfaisant de recompense. Accompagne le bilan avec les pieces |
| 4 | **Adoption reussie** | 3s | Motif emotionnel et celebratoire. Le moment le plus "feel-good" du jeu. Harpe + clochettes + montee |
| 5 | **Chat en danger** | 2s | Motif inquietant mais pas stressant. Quelques notes descendantes, legerement dissonantes |
| 6 | **Level-up reputation** | 3s | Fanfare triomphante. Montee en gamme + resolution satisfaisante |
| 7 | **Level-up confort chat** | 1.5s | Mini-fanfare doree. Plus discret que la reputation, mais satisfaisant |
| 8 | **Deblocage d'etage** | 3s | Fanfare de construction. Sons de marteau/clochettes + resolution |
| 9 | **Recompense journaliere** | 2s | Motif festif et invitant. Donne envie de collecter |
| 10 | **Rapport d'absence** | 2s | Motif de retrouvailles. Chaleureux et accueillant ("content de vous revoir") |
| 11 | **Combat** | 1.5s | Motif comique/cartoon de bagarre. Bref et drole, pas menaçant |
| 12 | **Tutoriel termine** | 3s | Motif de victoire + applaudissements doux |

**Total musique : 1 loop + 12 jingles = 13 pistes**

---

## 3. VOCALISATIONS DES CHATS

Chaque race possede **1 set de base** de vocalisations avec une tonalite propre. Les variations internes a une race se font par **modulation de pitch** (±10-20%) et **selection aleatoire** parmi 2-3 variantes.

### 3.1 Set de base par race

Chaque set contient les sons suivants :

| # | Son | Variantes | Description |
|---|-----|-----------|-------------|
| 1 | **Miaulement content** | 2-3 | Miaulement court, aigu et joyeux. Bonheur > 70% |
| 2 | **Miaulement neutre** | 2-3 | Miaulement standard, ni joyeux ni triste |
| 3 | **Miaulement triste** | 2 | Miaulement long et plaintif. Bonheur < 40% |
| 4 | **Miaulement urgent** | 1-2 | Miaulement frenetique et inquiet. Bonheur < 20%, depart imminent |
| 5 | **Ronronnement** | 1 (loop) | Loop de ronronnement doux. Joue pendant les caresses et en idle heureux |
| 6 | **Ronronnement fort** | 1 (loop) | Loop de ronronnement intense. Pendant les caresses actives |
| 7 | **Baillement** | 1 | Baillement de chat avant de dormir ou au reveil |
| 8 | **Miaulement joueur** | 1-2 | Petit cri excite pendant le jeu. Court, aigu, energique |

**Sons par race : ~12-15 fichiers**

### 3.2 Caractere sonore par race

| Race | Tonalite | Particularites |
|------|----------|----------------|
| **Europeen** | Moyenne, equilibree | Reference sonore. Miaulements classiques et polyvalents |
| **Ragdoll** | Douce, feutree | Miaulements plus longs et plus doux. Ronronnement tres prononce. Moins frequent |
| **Siamois** | Aigue, bavarde | Miaulements forts et frequents, grande variete de tons. Le plus "vocal" de toutes les races |
| **British Shorthair** | Grave, discrete | Miaulements courts et retenus. Ronronnement subtil. Peu vocal |
| **Maine Coon** | Grave, resonante | Miaulements profonds et imposants. Presence sonore forte malgre une frequence modere |
| **Bengal** | Aigue, sauvage | Trilles et gazouillis au lieu de miaulements classiques. Sons energiques et atypiques |
| **Chartreux** | Moyenne, affirmee | Miaulements assertifs et territoriaux. Ton autoritaire |
| **Norvegien** | Grave, profonde | Miaulements lents et bas. Impression de puissance calme |

### 3.3 Sons de combat (generiques)

Sons partages par toutes les races agressives. Utilises uniquement pendant la sequence de combat (3 sec).

| # | Son | Description |
|---|-----|-------------|
| 1 | **Feulements** | Echange de grognements/crachements avant le combat. Cartoon et comique |
| 2 | **Coups de pattes** | Impacts legers et rapides pendant la bagarre |
| 3 | **Nuage de poussiere** | Whoosh tourbillonnant comique pendant le nuage de combat |
| 4 | **Cri de surprise** | Petit couinement cartoon quand un chat prend un coup |
| 5 | **Separation** | Son d'arret net. Les chats se separent, grognement final |

### 3.4 Chats speciaux

Les 8 chats speciaux (Aristote, Orion, Cleo, etc.) utilisent le set de base de leur race. Aucun son supplementaire n'est requis — leur unicite est visuelle, pas sonore.

**Total vocalisations : 8 races x ~13 sons + 5 sons combat = ~109 fichiers**

---

## 4. SONS D'ACTIONS DES CHATS

Sons lies aux animations d'action (ref [2d-animation.md](2d-animation.md)). Joues quand un chat interagit avec un objet.

### 4.1 Manger (EATING)

| # | Son | Description |
|---|-----|-------------|
| 1 | **Croquettes** | Craquement de croquettes, mou et mignon. Loop courte (~1s) |
| 2 | **Laper** | Son de langue qui lappe l'eau. Loop courte (~1s) |
| 3 | **Gamelle cliquetis** | Petit choc ceramique/metal quand le chat touche la gamelle |

### 4.2 Dormir (SLEEPING)

| # | Son | Description |
|---|-----|-------------|
| 1 | **Respiration douce** | Loop de respiration lente et apaisante (~2s) |
| 2 | **Ronflement leger** | Ronflement cartoon, drole et mignon. Occasionnel |
| 3 | **Installation** | Froissement/plop quand le chat se couche sur le coussin/lit |

### 4.3 Jouer (PLAYING)

| # | Son | Description |
|---|-----|-------------|
| 1 | **Tape de patte (balle)** | Petit "boing" cartoon quand le chat tape la balle |
| 2 | **Bond/saut** | Whoosh leger + atterrissage doux |
| 3 | **Griffoir (arbre a chat)** | Grattement de griffes sur sisal/corde |

### 4.4 Litiere (CLEANING)

| # | Son | Description |
|---|-----|-------------|
| 1 | **Grattement litiere** | Son de sable/gravier gratte. Loop courte |
| 2 | **Recouvrement** | Son de sable pousse a la fin |

### 4.5 Deplacement

| # | Son | Description |
|---|-----|-------------|
| 1 | **Pas de marche** | Petit "pat pat" de pattes feutrees. 2 variantes pour alternance |
| 2 | **Pas de course** | Meme base, plus rapide et plus appuye |

**Total sons d'actions : ~15 fichiers**

---

## 5. SONS D'INTERACTION JOUEUR

### 5.1 Tap-to-Collect (pieces)

| # | Son | Description |
|---|-----|-------------|
| 1 | **Piece spawn** | Petit "pop" doux quand une piece apparait au-dessus d'un chat |
| 2 | **Piece collecte** | "Kling" de piece metallique satisfaisant. 3 variantes de pitch pour eviter la repetition |
| 3 | **Piece collecte combo** | Meme son mais avec pitch ascendant quand le joueur collecte rapidement. Effet "gamme montante" |
| 4 | **Piece fly-to** | Swoosh leger pendant le trajet de la piece vers le compteur HUD |
| 5 | **Compteur increment** | Son de compteur qui augmente. "Tik" rapide et satisfaisant |
| 6 | **Ramasser tout** | Cascade rapide de "klings" + swoosh collectif. Le son le plus satisfaisant du jeu |

### 5.2 Caresses (petting)

| # | Son | Description |
|---|-----|-------------|
| 1 | **Main apparait** | Son de contact doux, comme un toucher de tissu |
| 2 | **Caresse reussie** | Chime positif + coeurs. Declenchement du ronronnement (section 3.1) |
| 3 | **Cooldown refuse** | Petit "bonk" cartoon discret quand le joueur tape un chat en cooldown |
| 4 | **Cooldown pret** | Micro-chime doux indiquant que le chat est de nouveau caressable (optionnel) |

**Total sons d'interaction : ~10 fichiers**

---

## 6. SONS D'INTERFACE (UI)

### 6.1 Boutons et controles

| # | Son | Contexte | Description |
|---|-----|----------|-------------|
| 1 | **Tap positif** | Boutons primaires (Accueillir, Acheter, Confirmer, Ameliorer) | Click lumineux et satisfaisant |
| 2 | **Tap neutre** | Boutons secondaires (Annuler, Fermer, onglets, filtres) | Click neutre et leger |
| 3 | **Tap negatif** | Boutons danger (Vendre, Refuser, Supprimer) | Click mat et sourd |
| 4 | **Tap desactive** | Bouton grise (hotel plein, fonds insuffisants, hors connexion) | Son etouffé / bloque. Bref et non agaçant |
| 5 | **Toggle on** | Activation d'un parametre | Click positif avec resolution |
| 6 | **Toggle off** | Desactivation d'un parametre | Click neutre sans resolution |

### 6.2 Panneaux et navigation

| # | Son | Contexte | Description |
|---|-----|----------|-------------|
| 7 | **Panneau slide in** | Ouverture boutique, info chat, info objet | Whoosh doux montant |
| 8 | **Panneau slide out** | Fermeture panneau | Whoosh doux descendant |
| 9 | **Popup ouvre** | Ouverture de toute popup centree | "Pop" doux + leger chime |
| 10 | **Popup ferme** | Fermeture de popup | Son inverse discret |
| 11 | **Ecran plein transition** | Transition vers ecran plein ecran (Gestion, Boutique Premium, Stats) | Swoosh fluide |
| 12 | **Changement d'onglet** | Onglets boutique (Nourriture, Sommeil, etc.) | Petit clic leche, different du tap neutre |
| 13 | **Scroll** | Defilement de liste (optionnel, tres discret) | Leger froissement |
| 14 | **Changement d'etage** | Tap fleche haut/bas navigation etages | Son d'ascenseur cartoon. Petit "ding" a l'arrivee |

### 6.3 Notifications toast

| # | Son | Contexte | Description |
|---|-----|----------|-------------|
| 15 | **Toast succes** (vert) | Action reussie, chat heureux, achat valide | Chime ascendant lumineux |
| 16 | **Toast avertissement** (orange) | Besoin critique, ressources basses | Chime d'attention, ton moyen |
| 17 | **Toast erreur** (rouge) | Echec, depart de chat, probleme | Chime descendant, ton sombre |
| 18 | **Toast caprice** (violet) | Nouveau caprice genere | Son unique et fantasque, petit grelot |
| 19 | **Toast evenement** (bleu) | Arrivee, adoption, combat | Chime distinctif et neutre |

### 6.4 Compteurs et HUD

| # | Son | Contexte | Description |
|---|-----|----------|-------------|
| 20 | **Pieces ajoutees** | Compteur de pieces augmente (hors collecte individuelle) | Tintement rapide et satisfaisant |
| 21 | **Gemmes ajoutees** | Compteur de gemmes augmente | Son cristallin / magique, different des pieces |
| 22 | **Timer expire** | Un timer atteint zero (construction, prochain chat, cooldown objet) | Sonnette douce, "ding" clair |
| 23 | **Badge notification** | Badge rouge/orange apparait sur un element HUD | Micro "ping" d'alerte subtil |

**Total sons UI : ~23 fichiers**

---

## 7. SONS D'EVENEMENTS GAMEPLAY

### 7.1 Arrivee de chat

| # | Son | Description |
|---|-----|-------------|
| 1 | **Arrivee pension** | Son de caisse de transport qui s'ouvre. Plastique + grincement cartoon |
| 2 | **Arrivee refuge** | Son plus doux et emotionnel. Petite clochette timide |
| 3 | **Chat accueilli** | Son de validation joyeux quand le joueur accepte le chat |
| 4 | **Chat refuse** | Son neutre de depart. Pas triste, juste un "au revoir" |

### 7.2 Depart pension (bilan)

| # | Son | Description |
|---|-----|-------------|
| 1 | **Calcul de paiement** | Sons de pieces qui s'empilent, compteur montant. Tres satisfaisant |
| 2 | **Pourboire** | Son bonus supplementaire quand bonheur > 80%. Petit "cha-ching" dore |
| 3 | **Depart content** | Miaulement heureux + son de caisse qui se ferme |

### 7.3 Adoption (refuge)

| # | Son | Description |
|---|-----|-------------|
| 1 | **Adoptant arrive** | Pas doux + petite melodie d'espoir |
| 2 | **Confettis** | Burst de confettis cartoon. "Pop pop pop" festif |
| 3 | **Coeurs** | Sons de coeurs qui apparaissent. Petits scintillements doux |

### 7.4 Construction et placement

| # | Son | Description |
|---|-----|-------------|
| 1 | **Preview valide** | Ton positif bref quand la zone de placement est verte |
| 2 | **Preview invalide** | Ton negatif bref quand la zone est rouge |
| 3 | **Piece construite** | Son de construction satisfaisant. "Clac" de dernier element pose |
| 4 | **Objet place** | Son de placement : petit "thump" de meuble pose au sol |
| 5 | **Objet vendu** | Son de pieces recues + objet qui disparait |
| 6 | **Mur detruit** | Son de demolition cartoon. Crash leger et drole |

### 7.5 Caprices

| # | Son | Description |
|---|-----|-------------|
| 1 | **Nouveau caprice** | Son d'apparition de bulle de pensee. "Plop" fantasque |
| 2 | **Caprice satisfait** | Chime de resolution positif + miaulement content |
| 3 | **Caprice echoue** | Chime de deception + miaulement triste |

### 7.6 Level-up confort chat

| # | Son | Description |
|---|-----|-------------|
| 1 | **XP gagne** | Micro-son discret quand un service ajoute +1 XP. Petit "tik" positif |
| 2 | **Barre XP pleine** | Son de barre qui se remplit. Montee en ton |

> Le jingle de level-up confort (section 2.2, jingle #7) se joue ensuite.

### 7.7 Reputation

| # | Son | Description |
|---|-----|-------------|
| 1 | **Conditions remplies** | Son de notification quand toutes les conditions de montee sont remplies. Badge HUD |
| 2 | **Animation level-up** | Flash dore + particules. Son d'eclat lumineux |

> Le jingle de level-up reputation (section 2.2, jingle #6) se joue ensuite.

### 7.8 Recompenses journalieres

| # | Son | Description |
|---|-----|-------------|
| 1 | **Jour debloque** | Petit "clic" de case de calendrier qui s'ouvre |
| 2 | **Collecte pieces** | Cascade de pieces (reutilise les sons de collecte section 5.1) |
| 3 | **Collecte gemmes** | Sons cristallins de gemmes qui s'ajoutent |
| 4 | **Chat special garanti (J7)** | Son rare et magique. Clochettes + scintillement special |
| 5 | **Streak perdu** | Son de deception doux. Pas punitif, juste neutre-triste |

### 7.9 Rapport d'absence

| # | Son | Description |
|---|-----|-------------|
| 1 | **Decompte revenus** | Sons de pieces qui s'accumulent pendant le defilement du rapport. Crescendo satisfaisant |

### 7.10 Chat en danger

| # | Son | Description |
|---|-----|-------------|
| 1 | **Alerte depart** | Son d'urgence, montee en tension (pas stressant, mais "attention !") |
| 2 | **Compte a rebours** | Tick-tock cartoon, 8 secondes, accelere vers la fin |
| 3 | **Chat sauve** | Son de soulagement. Chime apaisant + miaulement soulagé |
| 4 | **Chat parti** | Son de depart triste. Porte qui se ferme + silence bref |

### 7.11 Achat et economie

| # | Son | Description |
|---|-----|-------------|
| 1 | **Achat reussi (pieces)** | "Cha-ching" de caisse enregistreuse cartoon |
| 2 | **Achat reussi (gemmes)** | Son magique cristallin |
| 3 | **Fonds insuffisants** | Son d'erreur doux. Tirelire vide / "boing" cartoon |

**Total sons d'evenements : ~33 fichiers**

---

## 8. SONS D'OBJETS AMBIANTS

Certains objets produisent un son ambiant continu ou ponctuel quand ils sont actifs.

| # | Objet | Type | Description |
|---|-------|------|-------------|
| 1 | **Fontaine a eau** | Loop ambiant | Clapotis d'eau doux et regulier. Discret, contribue a l'ambiance cosy |
| 2 | **Aquarium** | Loop ambiant | Bulles d'eau + legers clapotis. Apaisant |
| 3 | **Distributeur auto** | Ponctuel | Son mecanique cartoon quand il distribue. Petit "whirr" + croquettes qui tombent |
| 4 | **Litiere auto** | Ponctuel | Son mecanique de nettoyage. Petit "vrr" + sable qui bouge |
| 5 | **Lampe** | Ponctuel (optionnel) | Micro-clic quand elle s'allume. Pas de loop |

> Les objets sans mecanisme (coussin, lit, plante, tableau, etagere, tapis, etc.) ne produisent aucun son propre.

**Total sons d'objets : ~5 fichiers**

---

## 9. SONS DE NOTIFICATIONS PUSH (hors app)

Sons joues par le systeme quand l'app est fermee. Courts et identifiables.

| # | Notification | Description |
|---|-------------|-------------|
| 1 | **Construction terminee** | Petit chime de construction. 1s |
| 2 | **Chat malheureux** | Miaulement triste court. 1s |
| 3 | **Chats en attente** | Miaulements curieux superposes. 1s |
| 4 | **Recompense journaliere** | Chime festif de rappel. 1s |
| 5 | **Offre limitee** | Son d'opportunite. Scintillement. 1s |
| 6 | **Retour apres absence** | Miaulement accueillant. 1s |

**Total sons push : 6 fichiers**

---

## RECAPITULATIF

| Categorie | Nombre de fichiers |
|-----------|-------------------|
| Musique (1 loop + 12 jingles) | 13 |
| Vocalisations chats (8 races x ~13 sons) | ~104 |
| Combat (sons generiques) | 5 |
| Actions des chats (manger, dormir, jouer, litiere, deplacement) | 15 |
| Interaction joueur (tap-to-collect, caresses) | 10 |
| Interface UI (boutons, panneaux, toasts, HUD) | 23 |
| Evenements gameplay | 33 |
| Objets ambiants | 5 |
| Notifications push | 6 |
| **TOTAL** | **~214 fichiers audio** |

---

## PRIORITES DE PRODUCTION

### Phase 1 — J1 (core gameplay)

Sons indispensables pour le prototype jouable avec 1 race (Europeen) :

| Categorie | Sons | Quantite |
|-----------|------|----------|
| Europeen vocalisations | Set complet (13 sons) | 13 |
| Actions chats | Tous (manger, dormir, jouer, litiere, deplacement) | 15 |
| Combat | Tous (5 sons generiques) | 5 |
| Tap-to-Collect | Tous (6 sons) | 6 |
| Caresses | Tous (4 sons) | 4 |
| UI boutons et controles | 6 sons | 6 |
| UI panneaux et navigation | 8 sons (sans etages) | 7 |
| UI toasts | 5 types | 5 |
| UI compteurs/HUD | Timer, pieces, badge | 3 |
| Jingles | Bienvenue, arrivee, depart pension, chat en danger, combat | 5 |
| Gameplay loop | 1 piste | 1 |
| Arrivee/depart chat | 4 sons | 4 |
| Construction/placement | 6 sons | 6 |
| Caprices | 3 sons | 3 |
| Level-up confort (sans pub) | XP gagne, barre pleine | 2 |
| Chat en danger | 4 sons | 4 |
| Achat/economie | 3 sons | 3 |
| **Total J1** | | **~92 fichiers** |

### Phase 2 — J2 (contenu)

| Categorie | Sons | Quantite |
|-----------|------|----------|
| 7 races supplementaires (vocalisations) | 7 x ~13 sons | ~91 |
| Navigation etages | Son d'ascenseur | 1 |
| Jingles | Level-up reputation, deblocage etage | 2 |
| Reputation | Conditions remplies, animation level-up | 2 |
| Objets ambiants | Fontaine, aquarium, distributeur auto, litiere auto, lampe | 5 |
| **Total J2** | | **~101 fichiers** |

### Phase 3 — J3 (monetisation + polish)

| Categorie | Sons | Quantite |
|-----------|------|----------|
| Jingles | Recompense journaliere, rapport d'absence, tutoriel termine, level-up confort, adoption | 5 |
| Recompenses journalieres | 5 sons | 5 |
| Rapport d'absence | 1 son | 1 |
| UI gemmes | Gemmes ajoutees, achat reussi gemmes | 2 |
| Notifications push | 6 sons | 6 |
| Ecran plein transition | Stats, boutique premium | 1 |
| Adoption | Adoptant, confettis, coeurs | 3 |
| **Total J3** | | **~23 fichiers** |

### Verification des totaux

| Phase | Fichiers |
|-------|----------|
| J1 | ~92 |
| J2 | +101 |
| J3 | +23 |
| **Total** | **~216 fichiers** |

---

*Document associe : [gdd.md](gdd.md) | [2d-art.md](2d-art.md) | [2d-animation.md](2d-animation.md) | [ui-ux.md](ui-ux.md) | [jalons.md](jalons.md)*
