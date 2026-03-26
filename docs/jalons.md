# Cat Hotel Tycoon - Jalons de Production
## Roadmap par phases

**Version:** 1.1
**Date:** Fevrier 2026
**Scope:** Features de game design par jalon (pas de taches techniques)
**Orientation:** Paysage (prioritaire) + Portrait — l'UI s'adapte dynamiquement (ref [ui-ux.md](ui-ux.md) section 14)

---

## Documents associes

| Document | Contenu |
|----------|---------|
| **[gdd.md](gdd.md)** | Design complet du jeu |
| **[2d-art.md](2d-art.md)** | Liste des assets graphiques |
| **[2d-animation.md](2d-animation.md)** | Liste des animations 2D |
| **[sound-design.md](sound-design.md)** | Liste des assets audio |
| **[ui-ux.md](ui-ux.md)** | Ecrans, flux de navigation, economie F2P |

---

## Vue d'ensemble

| Jalon | Objectif | Contenu cle |
|-------|----------|-------------|
| **J1** | Gameplay core fonctionnel | 1 race, services de base, construction de pieces (RDC uniquement), pension/refuge, caprices, combats, tap-to-collect basique, caresses |
| **J2** | Extension du contenu | Races supplementaires, systeme de reputation, 1-2 etages, objets avances |
| **J3** | Economie F2P complete | Gemmes, IAP, pubs rewarded, daily rewards, idle revenue, FTUE, timers |

---

## JALON 1 - Gameplay Core

### Objectif

Le jeu est jouable de bout en bout avec une seule race de chat. Tous les systemes fondamentaux fonctionnent. Le joueur construit des pieces, accueille des chats en pension ou refuge, satisfait leurs besoins, gere caprices et combats, et collecte des pieces par tap. **Pas de gemmes, pas de monetisation, pas d'etages.** L'UI supporte les deux orientations (paysage prioritaire + portrait) des le J1.

---

### 1.1 Systemes actifs

#### Grille et Construction — GDD 3.1, 3.2

- Grille 24x16 cellules
- Creation de pieces par glisser-deposer (taille min 3x3)
- Cout en pieces ($) : 10$/cellule
- Extension par destruction de murs mitoyens
- **Construction instantanee** (pas de timer — les timers sont une mecanique de monetisation reportee au J3)
- **RDC uniquement** : pas de navigation entre etages

#### Besoins — GDD 3.4

- 4 besoins fonctionnels : Faim, Sommeil, Jeu, Proprete
- Decroissance en temps reel selon la formule du GDD
- Seuil critique < 20% : bulle d'alerte au-dessus du chat
- Les chats cherchent automatiquement un objet disponible quand un besoin < 60%

#### Bonheur — GDD 3.5

- Formule complete : moyenne des besoins - caprices non satisfaits - penalite combat + confort + bonus caresse
- Seuils fonctionnels :
  - **> 70%** : chat heureux, genere 0.5$/sec (piece flottante)
  - **40-70%** : chat neutre, genere 0.2$/sec
  - **< 40%** : chat mecontent, pas de revenu
  - **< 20%** : le chat quitte l'hotel (depart apres 8 sec)

#### Confort — GDD 3.6

- Les decorations placees dans une piece augmentent le confort
- Penalite de surpopulation (> 80% occupation) : -20 confort max
- Bonus espace personnel (< 20% occupation) : +5

#### Caprices — GDD 3.7

- 3 types : service specifique, objet en hauteur, decoration dans la piece
- Generation aleatoire (toutes les 30 sec, probabilites selon GDD)
- Le joueur assigne un objet pour satisfaire le caprice
- Penalite : -20% bonheur par caprice non satisfait

#### Combats — GDD 3.9

- Declenchement : chat agressif avec bonheur < 50% et autre chat a proximite (< 3 cellules)
- Destruction d'objet : bonheur < 35% et objet a proximite (< 2 cellules)
- Duree : 3 sec, penalite : -25 bonheur, cooldown : 15 sec
- **Note :** L'Europeen n'est pas agressif. Le systeme est implemente et testable mais ne se declenchera naturellement qu'avec les races agressives du J2 (Chartreux, Norvegien). Pour le test en J1, prevoir un mode debug/cheat pour forcer un combat

#### Pension / Refuge — GDD 3.10

- **Pension** : le proprietaire depose un chat pour une duree (2-10 min). Paiement en pieces a la fin du sejour base sur bonheur moyen. Pourboire si bonheur > 80%
- **Refuge** : chat abandonne, cherche un foyer. Le joueur peut renommer les chats du refuge (tap sur le nom dans le Panneau Info Chat). Adoptant apparait si bonheur > 70% pendant 30+ sec. Bonus pieces + reputation a l'adoption. Fuite si bonheur < 30% trop longtemps
- Probabilites : 70% pension, 30% refuge
- Popups d'arrivee, de bilan et d'adoption fonctionnelles (sans boutons de pub rewarded)

#### Tap-to-Collect — GDD 2.4

- Pieces dorées flottantes au-dessus des chats qui generent du revenu
- Tap sur la piece → fly-to vers le compteur HUD
- Accumulation infinie (cap visuel ~20 pieces par chat)
- Bouton "Ramasser tout" : cooldown 60 sec
- **Pas de suppression de cooldown par gemmes** (J3)

#### Caresses — GDD 2.4

- Tap sur un chat → +5 bonheur, animation coeurs, son de ronronnement
- Cooldown 30 sec par chat (indicateur circulaire visible)
- **Pas de suppression de cooldown par gemmes** (J3)
- Priorite de tap : piece > caresse

#### Pathfinding — GDD 3.8

- BFS pour chemins simples en zone construite
- A* pour traversee de zones vides

---

### 1.2 Race disponible

| Race | Variante speciale | Taille | Agressif | Ref GDD |
|------|-------------------|--------|----------|---------|
| **Europeen** | Aristote (lunettes, mortarboard) | 1.0x | Non | 4.1, 4.3 |

- Systeme de noms fonctionnel (72+ noms, ref GDD 4.5)
- Machine a etats complete : IDLE → SEEKING → ACTION → retour IDLE, + UNHAPPY → LEAVING, + PICKUP/ADOPTED (ref GDD 4.4)
- Pas de limite de reputation (niveau 0 = 5 chats max)

---

### 1.3 Objets disponibles

Minimum fonctionnel : 1 objet de base par categorie de besoin + quelques decorations.

| # | Objet | Categorie | Taille | Cout | Ref GDD |
|---|-------|-----------|--------|------|---------|
| 1 | **Gamelle** | Nourriture | 1x1 | 30$ | 5.1 |
| 2 | **Gamelle d'eau** | Nourriture | 1x1 | 25$ | 5.1 |
| 3 | **Coussin** | Sommeil | 1x1 | 40$ | 5.2 |
| 4 | **Balle** | Jeu | 1x1 | 20$ | 5.3 |
| 5 | **Litiere** | Proprete | 1x1 | 35$ | 5.4 |
| 6 | **Plante** | Decoration | 1x1 | 25$ | 5.5 |
| 7 | **Lampe** | Decoration | 1x1 | 30$ | 5.5 |
| 8 | **Table basse** | Support | 1x1 | 40$ | 5.6 |
| 9 | **Tapis confort** | Tapis | 2x2 | 50$ | 5.7 |

**Total J1 : 9 objets**

---

### 1.4 Ecrans et UI — ref ui-ux.md

#### Inclus dans le J1

| # | Ecran / Panneau | Type | Notes J1 |
|---|-----------------|------|----------|
| 1 | Splash Screen | Plein ecran | Logo simple |
| 2 | Ecran de chargement | Plein ecran | Barre de progression |
| 3 | Ecran titre | Plein ecran | Logo + "Jouer" + "Parametres" |
| 4 | **Hotel (gameplay)** | Plein ecran | HUD sans gemmes, sans bouton pub. Avec tap-to-collect et caresses. Layout adaptatif portrait/paysage (ref ui-ux.md section 14) |
| 5 | Menu Pause | Overlay | Reprendre, Parametres, Sauvegarder |
| 6 | Parametres | Overlay | Volume, langue |
| 7 | Panneau Boutique | Overlay bas | Objets du J1 uniquement (9 objets) |
| 8 | Panneau Info Chat | Overlay lateral | Portrait, besoins, caprices, revenu |
| 9 | Panneau Info Objet | Overlay lateral | Stats, vendre, deplacer |
| 10 | Ecran Gestion des Chats | Plein ecran | Liste, filtres, tri, localiser |
| 11 | Popup Arrivee Chat | Popup | Accueillir / Refuser |
| 12 | Popup Bilan Pension | Popup | Paiement, pourboire — **sans bouton "Doubler" (pub)** |
| 13 | Popup Adoption Reussie | Popup | Frais, bonus reputation — **sans bouton "Doubler" (pub)** |
| 14 | Popup Chat en Danger | Popup | Alerte depart — **sans bouton "Sauver" (pub)** |
| 15 | Popup Confirmation | Popup | Actions irreversibles |
| 16 | Popup Manque Ressources | Popup | "Pas assez de pieces" — **sans option pub/gemmes** |
| 17 | Toast Notifications | Banniere | Succes, avertissement, erreur, caprice |

**Total J1 : 17 ecrans/panneaux/popups**

#### Reporte aux jalons suivants

| Ecran | Jalon | Raison |
|-------|-------|--------|
| Navigation etages | J2 | Pas d'etages en J1 |
| Popup Deblocage Etage | J2 | Pas d'etages en J1 |
| Popup Amelioration Reputation | J2 | Pas de systeme de reputation en J1 |
| Boutique Premium (gemmes) | J3 | Pas de gemmes en J1 |
| Popup Recompense Journaliere | J3 | Mecanique de monetisation |
| Popup Rapport d'Absence | J3 | Idle revenue = monetisation |
| Popup Offre Speciale | J3 | Monetisation |
| Popup Timer Construction | J3 | Construction instantanee en J1 |
| Popup Pub Rewarded | J3 | Pas de pubs en J1 |
| Overlay Tutoriel (FTUE) | J3 | Le FTUE guide l'experience F2P complete |

---

### 1.5 Assets graphiques necessaires — ref 2d-art.md

| Categorie | Assets | Quantite |
|-----------|--------|----------|
| Europeen base (face, cote, dos) | Sprites de base | 3 |
| Aristote special (face, cote, dos) + icone | Sprites speciaux | 3 + 1 |
| Emotes / Bulles | Besoins (4) + emotions (5) + statuts (3) + caprices (3) | 15 |
| Tiles construction | Sol RDC + murs + exterieur + overlays | ~10 |
| Objets (9 objets du J1) | Sprites + icones boutique | 9 + 9 |
| Effets / Particules | Coeurs, etoiles, pieces, zzz, nuage, exclamation | 6 |
| Interaction | Piece flottante, badge, main caresse, cooldown, bouton Ramasser tout | 7 |
| **Total J1** | | **~63 assets** |

---

### 1.6 Animations necessaires — ref 2d-animation.md

| Categorie | Animations | Frames estimees |
|-----------|------------|-----------------|
| Europeen base : idle (3x3dir) + marche (3dir) + actions (5x2dir) + emotions (5dir) + caresse (3dir) | 30 anims | ~135 frames |
| Aristote special : idem | 30 anims | ~135 frames |
| Animations speciales generiques (combat, arrivee, depart) | 7 anims | ~34 frames |
| Animations objets (lampe) | 1 anim | ~2 frames |
| Animations effets/particules (coeur, etoile, piece, zzz, nuage, exclamation) | 6 anims | ~21 frames |
| Animations interaction (piece spawn/idle/collect, main caresse, cooldown) | 8 anims | ~37 frames |
| **Total J1** | | **~364 frames** |

---

### 1.7 Audio — ref sound-design.md

| Categorie | Sons | Quantite |
|-----------|------|----------|
| Musique | Gameplay loop (1 piste) | 1 |
| Jingles | Bienvenue, arrivee, depart pension, chat en danger, combat | 5 |
| Vocalisations Europeen | Set complet (miaulements, ronronnement, baillement) | ~13 |
| Actions chats | Manger, boire, dormir, jouer, litiere, marche, course | 15 |
| Tap-to-Collect | Spawn, collecte (3 var.), combo, fly-to, compteur, ramasser tout | 6 |
| Caresses | Main, reussite, cooldown refuse, cooldown pret | 4 |
| Interface | Boutons (4), panneaux (7), toasts (5), HUD (3) | 19 |
| Evenements | Arrivee, depart, construction, caprices, confort, danger, achat | 28 |
| **Total J1** | | **~91 fichiers** |

---

### 1.8 Ce qui n'est PAS dans le J1

- Gemmes (hard currency)
- IAP et Boutique Premium
- Publicites rewarded
- Recompenses journalieres (daily rewards)
- Rapport d'absence (idle revenue)
- Timers de construction / cooldowns d'objets
- Etages (au-dela du RDC)
- Systeme de reputation
- Races autres que l'Europeen
- Tutoriel guide (FTUE)
- Notifications push
- Sauvegarde cloud (sauvegarde locale suffit)
- Objets avances (Distributeur auto, Fontaine, Lit, Panier premium, Arbre a chat, Litiere auto)
- Decorations avancees (Grande plante, Etagere, Tableau, Aquarium, Griffoir)
- Tapis supplementaires

---
---

## JALON 2 - Extension du Contenu

### Objectif

Enrichir le jeu avec de nouvelles races, le systeme de reputation, des etages supplementaires et des objets avances. Le joueur decouvre la progression verticale (etages) et horizontale (races). **Toujours pas de monetisation F2P.**

---

### 2.1 Systemes ajoutes

#### Systeme d'etages — GDD 3.3

- Deblocage de l'etage 1 et de l'etage 2
- Cout en pieces (500$ et 1500$) avec multiplicateur de construction
- Navigation entre etages : boutons fleche haut/bas + indicateur
- Bonus confort et revenus par etage
- Comportement des chats par etage (preference, tolerance, malus)

| Etage | Cout | Bonus confort | Bonus revenus | Capacite | Races ajoutees |
|-------|------|---------------|---------------|----------|----------------|
| RDC | Deja actif | +0 | x1.0 | 10 | Europeen |
| 1er | 500$ | +5 | x1.2 | 12 | Ragdoll, Siamois |
| 2eme | 1 500$ | +10 | x1.5 | 15 | British, Maine Coon |

#### Systeme de reputation — GDD 8 (7.1, 7.2, 7.3)

- 10 niveaux de reputation (Debutant → Maitre des Chats)
- Conditions de montee : nombre de chats requis + bonheur minimum
- Cout en pieces pour ameliorer
- Deblocage progressif des races
- Penalite de reputation (decroissance acceleree si reputation insuffisante pour une race)
- Popup Amelioration de Reputation fonctionnelle

#### Races supplementaires

| Race | Reputation min | Taille | Agressif | Notes |
|------|----------------|--------|----------|-------|
| **Ragdoll** | 1 | 1.1x | Non | Docile, dort +30% |
| **Siamois** | 2 | 0.9x | Non | Bavard, joue +50% |
| **British** | 3 | 1.05x | Non | Paresseux, dort +60% |
| **Maine Coon** | 5 | 1.3x | Non | Gourmand, faim +60% |
| **Bengal** | 6 | 0.95x | Non | Energique, joue +80% |
| **Chartreux** | 8 | 1.1x | **Oui** | Dominant, equilibre |
| **Norvegien** | 9 | 1.25x | **Oui** | Territorial, faim +30% |

Chaque race inclut sa variante speciale (Orion, Cleo, Winston, Thor, Panthera, Napoleon, Odin).

**Note :** L'ajout du Chartreux et du Norvegien active naturellement le systeme de combats implemente en J1.

---

### 2.2 Objets ajoutes

Objets avances et decorations supplementaires pour diversifier le gameplay.

| # | Objet | Categorie | Taille | Cout | Ref GDD |
|---|-------|-----------|--------|------|---------|
| 1 | **Distributeur auto** | Nourriture | 1x1 | 80$ | 5.1 |
| 2 | **Fontaine a eau** | Nourriture | 1x1 | 100$ | 5.1 |
| 3 | **Lit pour chat** | Sommeil | 2x1 | 80$ | 5.2 |
| 4 | **Panier premium** | Sommeil | 2x2 | 120$ | 5.2 |
| 5 | **Arbre a chat** | Jeu | 2x2 | 150$ | 5.3 |
| 6 | **Litiere auto** | Proprete | 2x1 | 120$ | 5.4 |
| 7 | **Grande plante** | Decoration | 1x1 | 45$ | 5.5 |
| 8 | **Etagere** | Decoration | 1x1 | 40$ | 5.5 |
| 9 | **Tableau** | Decoration | 1x1 | 60$ | 5.5 |
| 10 | **Aquarium** | Decoration | 1x1 | 150$ | 5.5 |
| 11 | **Griffoir** | Decoration | 1x1 | 50$ | 5.5 |
| 12 | **Meuble haut** | Support | 1x1 | 80$ | 5.6 |
| 13 | **Etagere murale** | Support | 1x1 | 60$ | 5.6 |
| 14 | **Tapis jeu** | Tapis | 2x2 | 45$ | 5.7 |
| 15 | **Tapis premium** | Tapis | 3x3 | 100$ | 5.7 |

**Total objets ajoutes J2 : 15 objets** (total cumule : 24 objets = catalogue complet)

---

### 2.3 Ecrans et UI ajoutes — ref ui-ux.md

| # | Ecran / Panneau | Type | Notes J2 |
|---|-----------------|------|----------|
| 1 | Navigation etages | Boutons HUD | Fleche haut/bas + indicateur etage |
| 2 | Popup Deblocage Etage | Popup | Cout, stats, races accessibles |
| 3 | Popup Amelioration Reputation | Popup | Niveau, conditions, cout, deblocages |
| 4 | Bouton "Ameliorer" dans le HUD | HUD | Badge notification si conditions remplies |
| 5 | Ecran Statistiques | Plein ecran | Graphiques 7 jours (revenus, bonheur, population, adoptions) + stats globales cumulees. Ref ui-ux.md section 5.3 |

**Total cumule J2 : 22 ecrans/panneaux/popups**

#### Mise a jour des ecrans existants

- **HUD haut** : ajout du niveau de reputation + nom du rang + bouton "Ameliorer"
- **Barre d'outils bas** : ajout du bouton Statistiques (icone graphique)
- **Panneau Boutique** : tous les objets disponibles (24), les objets verrouilles par niveau sont grises
- **Ecran Gestion des Chats** : filtres par race, tri par etage
- **Popup Arrivee Chat** : affichage de la race et du type (pension/refuge) avec portrait specifique

---

### 2.4 Assets graphiques ajoutes — ref 2d-art.md

| Categorie | Assets | Quantite |
|-----------|--------|----------|
| 7 races base (face, cote, dos) | Sprites | 21 |
| 7 chats speciaux (face, cote, dos) + icones | Sprites + icones | 21 + 7 |
| Tiles sol etage 1 et 2 | Variantes sol | 2 |
| Objets ajoutes (15 objets) | Sprites + icones boutique | 15 + 15 |
| Animations objets supplementaires (fontaine, distributeur, litiere auto, aquarium, balle) | Sprites anim | 5 |
| Effets supplementaires (notes de musique, gouttes de sueur) | Sprites | 2 |
| Icones UI stats (icone graphique, 3 fleches tendance) | Icones | 4 |
| **Total J2** | | **~92 assets** |

**Total cumule J1+J2 : ~155 assets**

---

### 2.5 Animations ajoutees — ref 2d-animation.md

| Categorie | Frames estimees |
|-----------|-----------------|
| 7 races base x ~135 frames (idle, marche, actions, emotions, caresse) | ~945 frames |
| 7 chats speciaux x ~135 frames | ~945 frames |
| Animations objets supplementaires (fontaine, distributeur, litiere auto, aquarium, balle) | ~20 frames |
| **Total J2** | **~1910 frames** |

**Total cumule J1+J2 : ~2274 frames** (= catalogue complet ref 2d-animation.md)

---

### 2.6 Audio ajoute — ref sound-design.md

| Categorie | Sons | Quantite |
|-----------|------|----------|
| Vocalisations 7 races | 7 sets complets (~13 sons chacun) | ~91 |
| Jingles | Level-up reputation, deblocage etage | 2 |
| Reputation | Conditions remplies, animation level-up | 2 |
| Objets ambiants | Fontaine, aquarium, distributeur auto, litiere auto, lampe | 5 |
| Navigation etages | Son d'ascenseur cartoon | 1 |
| **Total J2** | | **~101 fichiers** |

**Total cumule J1+J2 : ~192 fichiers audio**

---

### 2.7 Ce qui n'est PAS dans le J2

- Gemmes (hard currency)
- IAP et Boutique Premium
- Publicites rewarded
- Recompenses journalieres (daily rewards)
- Rapport d'absence (idle revenue)
- Timers de construction / cooldowns d'objets
- Tutoriel guide (FTUE)
- Notifications push
- Etages 3, 4 et Penthouse (reportes au J3 ou post-launch)
- Suppression de cooldown par gemmes (Ramasser tout, caresses)
- Offres speciales et win-back

---
---

## JALON 3 - Economie F2P Complete

### Objectif

Integrer toute la couche de monetisation Free-to-Play : gemmes, IAP, publicites rewarded, recompenses journalieres, idle revenue, timers, FTUE, et notifications push. Le jeu est pret pour un soft launch. Optionnellement, debloquer les etages restants (3, 4, Penthouse).

---

### 3.1 Systemes ajoutes

#### Dual Currency — GDD 6.0

- Introduction des **Gemmes** (hard currency)
- Compteur gemmes dans le HUD (tap → Boutique Premium)
- Sources gratuites : daily rewards, paliers de reputation, pub rewarded (rare), succes
- Taux implicite : 1 gemme ~= 10$

#### Suppression de cooldowns par gemmes — GDD 2.4, 6.2

- **Ramasser tout** : 1 gemme → 5 min sans cooldown
- **Caresses** : 5 gemmes → 5 min sans cooldown sur tous les chats
- Icone "main doree" dans le HUD pendant le boost caresses

#### Timers de construction — GDD 7.3

- Les pieces ne sont plus construites instantanement
- Timer selon la taille (5 min petit, 15 min moyen, 30 min grand)
- Options : attendre, pub rewarded (gratuit), gemmes (instantane), premium (instantane)
- Timer visible sur la piece en construction dans le jeu

#### Cooldowns d'objets premium — GDD 7.3

- Certains objets premium ont un cooldown de 2h
- Skip par pub rewarded ou gemmes

#### Recompenses journalieres — GDD 7.4, ui-ux.md section 7

- Calendrier 7 jours avec recompenses escaladantes
- J1: 100$ → J7: 1000$ + 20 gemmes + chat special garanti
- Reset si jour manque (option pub pour rattraper 1 jour)
- Popup automatique a chaque nouveau jour
- **Acces libre** : icone calendrier dans le HUD haut (badge point orange si non collecte) + bouton "Recompenses" dans le Menu Pause. En mode consultation si deja collecte

#### Rapport d'absence (Idle Revenue) — GDD 7.5, ui-ux.md section 11

- Revenus accumules pendant l'absence (plafonne a 80% du reel)
- Popup au retour apres 1h+
- Bouton "x2 revenus" (pub rewarded)
- Bonus x1.5 automatique pour les abonnes premium

#### Publicites Rewarded — GDD 7.6, ui-ux.md section 8

10 points d'insertion contextuels, tous optionnels :

| # | Situation | Recompense | Frequence max |
|---|-----------|------------|---------------|
| 1 | Bouton HUD | +50-100$ | 5/jour |
| 2 | Fin de sejour pension | Doubler paiement | 1/evenement |
| 3 | Adoption reussie | Doubler frais | 1/evenement |
| 4 | Construction en cours | Terminer instantanement | Illimitee |
| 5 | Cooldown objet premium | Reset cooldown | Illimitee |
| 6 | Boost revenus x2 | 30 min de boost | 3/jour |
| 7 | Chat en danger | Sauver (reset bonheur 50%) | 1/chat/session |
| 8 | Manque de pieces | +50-100$ | 3/jour |
| 9 | Gemmes gratuites | +5 gemmes | 1/jour |
| 10 | Streak manque | Rattraper 1 jour | 1/cycle |

#### IAP — GDD 7.7

- 4 packs de gemmes (0.99 a 19.99 EUR)
- Abonnement Premium optionnel (4.99 EUR/mois)
- Packs ponctuels : Starter Pack, Pack Race, Pack Deco

#### FTUE (Tutoriel) — ui-ux.md section 9

- 8 etapes guidees pour les nouveaux joueurs
- Overlay spotlight + bulle mascotte
- Non skippable en premiere session
- Inclut la decouverte du tap-to-collect (etape 7)
- Recompenses de bienvenue : gamelle + coussin offerts, 300$ + 10 gemmes

#### Notifications Push — ui-ux.md section 10

- 6 types de notifications (construction terminee, chat malheureux, chats en attente, recompense journaliere, offre limitee, retour apres absence)
- Max 2 push/jour, respect des opt-out
- Pas de push les 24 premieres heures

#### Offre de retour (Win-back) — ui-ux.md section 7.2

- Popup apres 48h+ d'absence
- Offre speciale limitee 24h

#### Etages supplementaires (optionnel J3)

Si le temps le permet, debloquer les etages restants :

| Etage | Cout | Bonus confort | Bonus revenus | Capacite | Race ajoutee |
|-------|------|---------------|---------------|----------|--------------|
| 3eme | 4 000$ | +15 | x1.8 | 18 | Bengal (deja dispo en J2 via reputation) |
| 4eme | 10 000$ | +20 | x2.2 | 20 | Chartreux (deja dispo en J2) |
| 5eme (Penthouse) | 25 000$ | +30 | x3.0 | 25 | Norvegien (deja dispo en J2) |

Les tiles de sol supplementaires (etage 3, 4, penthouse) sont necessaires.

---

### 3.2 Ecrans et UI ajoutes — ref ui-ux.md

| # | Ecran / Panneau | Type | Notes J3 |
|---|-----------------|------|----------|
| 1 | Ecran Boutique Premium | Plein ecran | Packs gemmes, abonnement, packs ponctuels |
| 2 | Popup Recompense Journaliere | Popup | Calendrier 7 jours |
| 3 | Popup Rapport d'Absence | Popup | Revenus idle, bouton x2 |
| 4 | Popup Offre Speciale | Popup | Starter pack, offres contextuelles |
| 5 | Popup Offre de Retour | Popup | Win-back apres 48h |
| 6 | Popup Timer Construction | Popup | Timer, gemmes, pub |
| 7 | Popup Pub Rewarded | Popup | Tous contextes ad |
| 8 | Overlay Tutoriel (FTUE) | Overlay | 8 etapes guidees |
| 9 | Introduction narrative (FTUE) | Plein ecran | 2-3 ecrans contexte |
| 10 | Credits | Plein ecran | Depuis ecran titre |

**Total cumule J3 : 29 ecrans/panneaux/popups** (= catalogue complet ref ui-ux.md + 3 ecrans etages optionnels)

#### Mise a jour des ecrans existants

- **HUD haut** : ajout du compteur gemmes (tap → boutique), icone calendrier (badge point orange si recompense non collectee), bouton pub rewarded dans la barre d'outils
- **Menu Pause** : ajout bouton "Recompenses" (ouvre le calendrier en consultation si deja collecte)
- **Popup Bilan Pension** : ajout bouton "Doubler les gains" (pub)
- **Popup Adoption** : ajout bouton "Doubler la recompense" (pub)
- **Popup Chat en Danger** : ajout bouton "Sauver ce chat" (pub)
- **Popup Manque Ressources** : ajout option pub (+50-100$) et lien vers Boutique Premium
- **Ecran titre** : ajout bouton "Credits"

---

### 3.3 Assets graphiques ajoutes — ref 2d-art.md

| Categorie | Assets | Quantite |
|-----------|--------|----------|
| Tiles sol etage 3, 4, Penthouse (si optionnel actif) | Variantes sol | 3 |
| Icones UI monetisation (gemmes, pub, premium) | Icones | ~5 |
| Icone calendrier HUD (avec badge point orange) | Icone | 1 |
| Mascotte tutoriel (si applicable) | Sprite | 1 |
| **Total J3** | | **~10 assets** |

La majorite des assets graphiques sont deja produits en J1+J2. Le J3 ajoute principalement des elements d'UI.

---

### 3.4 Audio ajoute — ref sound-design.md

| Categorie | Sons | Quantite |
|-----------|------|----------|
| Jingles | Recompense journaliere, rapport d'absence, tutoriel termine, level-up confort, adoption | 5 |
| Recompenses journalieres | Jour debloque, collecte pieces/gemmes, chat special J7, streak perdu | 5 |
| Rapport d'absence | Decompte revenus | 1 |
| UI gemmes | Gemmes ajoutees, achat reussi gemmes | 2 |
| Adoption | Adoptant arrive, confettis, coeurs | 3 |
| Ecran plein transition | Stats, boutique premium | 1 |
| Notifications push | 6 sons de notification hors app | 6 |
| **Total J3** | | **~23 fichiers** |

**Total cumule J1+J2+J3 : ~216 fichiers audio** (= catalogue complet ref sound-design.md)

---

### 3.5 Ce qui n'est PAS dans le J3 (post-launch)

- Evenements saisonniers
- Objets cosmetiques supplementaires
- Fonctionnalites sociales
- Nouvelles races de chats post-launch
- Localisation complete (FR/EN)
- ASO et marketing

---
---

## RECAPITULATIF CROISE

### Systemes par jalon

| Systeme | J1 | J2 | J3 |
|---------|----|----|-----|
| Double orientation (portrait + paysage) | x | | |
| Grille / Construction | x | | |
| Besoins / Bonheur / Confort | x | | |
| Caprices | x | | |
| Combats | x (impl.) | x (actif) | |
| Pension / Refuge | x | | |
| Tap-to-Collect | x (basique) | | x (gemmes) |
| Caresses | x (basique) | | x (gemmes) |
| Pathfinding | x | | |
| Etages | | x (1-2) | x (3-5 opt.) |
| Reputation | | x | |
| Statistiques (graphiques + stats globales) | | x | |
| Races multiples | | x (8 races) | |
| Gemmes (hard currency) | | | x |
| IAP | | | x |
| Pubs Rewarded | | | x |
| Daily Rewards (+ calendrier accessible HUD/Pause) | | | x |
| Idle Revenue | | | x |
| Timers construction | | | x |
| FTUE | | | x |
| Notifications push | | | x |
| **Audio (musique + SFX)** | x (~91) | x (+101) | x (+23) |

### Volume d'assets par jalon

| Jalon | Assets art | Frames anim | Ecrans UI | Fichiers audio |
|-------|------------|-------------|-----------|----------------|
| J1 | ~66 | ~408 | 18 | ~91 |
| J2 | +92 | +2162 | +5 | +101 |
| J3 | +12 | - | +7 | +23 |
| **Total** | **~170** | **~2570** | **30** | **~215** |

### Charge de travail estimee (art)

| Jalon | Races a dessiner | Objets a dessiner | Effort relatif |
|-------|------------------|-------------------|----------------|
| J1 | 1 (Europeen + Aristote) | 9 | Faible |
| J2 | 7 (+ 7 speciaux) | 15 | **Tres eleve** |
| J3 | 0 | 0 (UI seulement) | Faible |

> Le goulot d'etranglement artistique est clairement le **Jalon 2**. Envisager de le decouper en sous-jalons (J2a: 3-4 races, J2b: races restantes) si necessaire.

---

*Document cree pour Cat Hotel Tycoon - Fevrier 2026*

*Documents associes : [gdd.md](gdd.md) | [2d-art.md](2d-art.md) | [2d-animation.md](2d-animation.md) | [sound-design.md](sound-design.md) | [ui-ux.md](ui-ux.md)*
