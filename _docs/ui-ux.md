# Cat Hotel Tycoon - UI/UX
## Ecrans, cinematiques et flux de navigation

**Version:** 2.1
**Date:** Mars 2026
**Plateforme:** Mobile Android — Paysage (prioritaire) + Portrait
**Orientation:** L'interface s'adapte dynamiquement aux deux orientations. Voir section 14 pour les regles d'adaptation
**Police:** Balsamic Sans (Google Fonts) — utilisee pour tous les textes UI
**Palette:** voir [gdd.md](gdd.md) section 10.3

---

## 1. MODELE ECONOMIQUE - CURRENCIES

Le jeu repose sur un modele dual-currency standard F2P mobile.

### 1.1 Soft Currency - Pieces ($)

- Gagnee en jeu : chats heureux, pension, adoption, recompenses
- Depensee pour : construction, objets, amelioration de reputation, deblocage d'etages
- Abondante, flux constant, jamais achetable directement en IAP
- Affichee en permanence dans le HUD

### 1.2 Hard Currency - Gemmes

- Sources gratuites : recompenses journalieres, paliers de reputation, pubs rewarded (rare), succes
- Achetable en IAP (packs de gemmes)
- Depensee pour : skip timers, achats cosmetiques exclusifs, deblocage anticipe de races, boost premium
- Affichee dans le HUD a cote des pieces, tap dessus ouvre la boutique gemmes

### 1.3 Taux de conversion implicite

- 1 gemme ~= 10$ en pieces (le joueur ne peut pas convertir, c'est un ratio d'equilibrage interne)
- Les gemmes ne remplacent jamais les pieces : elles accelerent ou deverrouillent du contenu exclusif

---

## 2. FLUX DE LANCEMENT (Session Start)

Sequence d'ecrans au demarrage de l'app. Les popups se deroulent dans un ordre strict, **jamais d'empilement**.

### 2.1 Cinematique de lancement

1. **Splash Screen** - Logos studio/Unity (1-2s, non skippable)
2. **Ecran de chargement** - Barre de progression, tips de gameplay rotatifs
3. **Rapport d'absence** *(si retour apres 1h+ d'absence)* - Resume des revenus idle accumules, nombre de chats geres, evenements en attente. Bouton "Doubler les gains" (pub rewarded) ou "Collecter"
4. **Recompense journaliere** *(si nouveau jour)* - Calendrier 7 jours avec recompense du jour mise en avant. Collecte automatique puis fermeture
5. **Offre speciale** *(si applicable, max 1 par session)* - Starter pack (J1-J3), offre contextuelle (niveau up recent, retour apres 48h+), ou offre limitee dans le temps
6. **Hotel** - Le joueur atterrit sur son hotel, derniere vue avant fermeture

**Regle : chaque popup a un bouton de fermeture visible. Jamais plus de 3 popups avant le jeu.**

### 2.2 Premiere session (FTUE)

Pour un nouveau joueur, le flux est different :

1. **Splash Screen**
2. **Ecran titre** - Logo, illustration, bouton "Commencer l'aventure"
3. **Introduction narrative** - 2-3 ecrans de contexte : "Vous heritez d'un vieil hotel a chats..." avec mascotte guide
4. **Tutoriel guide** - Enchainement d'etapes directement en jeu (voir section 7)
5. **Fin de tutoriel** - Recompense de bienvenue, deblocage du jeu libre

---

## 3. ECRANS PRINCIPAUX

### 3.1 Ecran titre

- **Contexte :** Affiche uniquement en FTUE et si le joueur revient au menu depuis la pause
- **Contenu :** Logo du jeu, illustration de fond (hotel avec chats), bouton "Jouer", bouton "Parametres", bouton "Credits", toggle son, numero de version
- **Transition :** Tap "Jouer" → fondu vers le chargement puis l'hotel

### 3.2 Ecran Hotel (gameplay principal)

L'ecran central du jeu. Le joueur y passe 95% de son temps.

**Composition :**

- **HUD haut** : controles de vitesse (pause / x1 / x2), compteur pieces (avec animation +$ au tap), compteur gemmes (tap = boutique gemmes), compteur chats (actuels/max + nombre de contents), jauge confort moyen, niveau de reputation + nom du rang + bouton "Ameliorer" (avec badge notification si conditions remplies), timer prochain chat, icone calendrier (badge point orange si recompense non collectee aujourd'hui)
- **Zone de jeu** : grille interactive avec pieces, objets et chats animes. Pan/zoom tactile. **Pieces flottantes** au-dessus des chats generant du revenu (tap pour collecter). **Chats tappables** pour caresser (cooldown visible)
- **Navigation etages** : boutons fleche haut/bas + indicateur etage courant, positionnes a droite de l'ecran
- **Barre d'outils bas** : boutons Selection, Creer piece, Supprimer, Boutique, Gestion des chats, **Statistiques** (icone graphique). Bouton **"Ramasser tout"** (icone main + pieces, grise pendant cooldown 60s, badge gemme pour skip cooldown). Icone pub rewarded (bonus pieces) avec compteur restant
- **Zone de notifications toast** : bannieres temporaires en bas de l'ecran (succes vert, avertissement orange, erreur rouge, caprice violet, evenement bleu)

**Transitions depuis cet ecran :**
- Tap pause → Menu Pause (overlay)
- Tap Boutique → Panneau Boutique (slide depuis le bas)
- Tap Gestion → Ecran Gestion des chats (plein ecran)
- Tap sur un chat → Panneau Info Chat (slide lateral)
- Tap sur un objet → Panneau Info Objet (slide lateral)
- Tap compteur gemmes → Ecran Boutique Premium (plein ecran)
- Tap "Ameliorer" → Popup Amelioration Reputation
- Tap bouton etage verrouille → Popup Deblocage Etage
- Arrivee d'un chat → Popup Arrivee Chat
- Depart pension → Popup Bilan Pension
- Adoption reussie → Popup Adoption
- Tap sur une piece flottante → piece vole vers le compteur HUD, animation +$
- Tap "Ramasser tout" → toutes les pieces volent vers le HUD en cascade
- Tap sur un chat (hors piece) → animation caresse + coeurs, boost bonheur
- Tap sur un chat en cooldown caresse → indicateur cooldown clignote (refus)
- Tap icone calendrier (HUD) → Popup Calendrier Recompenses
- Tap Statistiques → Ecran Statistiques (plein ecran)
- Notification push in-game → toast animee

### 3.3 Menu Pause

- **Contexte :** Overlay semi-transparent par-dessus le jeu (le jeu est mis en pause)
- **Contenu :** Bouton Reprendre, Recompenses, Parametres, Sauvegarder, Menu principal
- **Transition :** Tap "Recompenses" → Popup Calendrier Recompenses (consultation seule si deja collecte). Tap "Menu principal" → popup de confirmation ("Votre progression est sauvegardee") → ecran titre. Tap "Reprendre" → fermeture de l'overlay, le jeu reprend

### 3.4 Ecran Parametres

- **Contexte :** Overlay accessible depuis Menu Pause ou Ecran Titre
- **Contenu :** Slider volume musique, slider volume effets, toggle notifications push, toggle economie batterie, selecteur langue (FR/EN), bouton restaurer achats, liens legaux (confidentialite, CGU)
- **Transition :** Bouton fermer (X) → retour a l'ecran precedent

---

## 3.5 INTERACTIONS GAMEPLAY (Tap-to-Collect & Caresses)

Le joueur interagit en permanence avec l'hotel via des taps directs sur les elements de jeu. Ces interactions constituent la boucle d'engagement principal du mode connecte.

### 3.5.1 Tap-to-Collect (Pieces)

Quand un chat genere du revenu (bonheur > 40%), une piece doree apparait et flotte au-dessus de lui. Le joueur doit taper dessus pour la collecter.

- **Apparition :** La piece spawn au-dessus du chat avec une legere animation de pop (scale 0 → 1). Elle flotte avec un leger mouvement sinusoidal vertical
- **Tap sur la piece :** La piece effectue un fly-to vers le compteur de pieces dans le HUD haut. Le compteur affiche "+X$" en vert avec animation de rebond
- **Accumulation :** Si le joueur ne collecte pas, les pieces s'empilent au-dessus du chat (max ~20 affichees, au-dela elles se superposent). Un badge numerique indique le nombre total de pieces non collectees si > 5
- **Bouton "Ramasser tout" :** Dans la barre d'outils bas. Tap → toutes les pieces de l'etage courant volent en cascade vers le compteur (effet visuel satisfaisant). Cooldown de 60 secondes apres utilisation (barre de rechargement visible sur le bouton). Le cooldown peut etre supprime pour 1 gemme (duree 5 min sans cooldown)
- **Feedback audio :** Son de piece a chaque collecte (pitch variable si collecte rapide pour effet de "combo"), son special pour Ramasser tout

### 3.5.2 Caresses (Pet)

Le joueur peut taper sur un chat pour le caresser et booster son bonheur.

- **Tap sur un chat :** Si aucune piece n'est sur la zone de tap et que le chat n'est pas en cooldown → animation de caresse (main + coeurs), +5 bonheur instantane, son de ronronnement
- **Cooldown :** Apres une caresse, un indicateur circulaire (arc de cercle) apparait autour du chat et se remplit en 30 secondes. Pendant le cooldown, taper sur le chat affiche l'indicateur qui clignote brievement (refus visuel, pas de son d'erreur intrusif)
- **Suppression cooldown :** Bouton dans le HUD ou popup contextuelle : 5 gemmes → supprime le cooldown de caresses sur tous les chats pendant 5 min. Icone "main doree" dans le HUD pendant la duree du boost
- **Priorite de tap :** Si une piece flottante est au-dessus du chat, le tap collecte la piece en priorite. La caresse ne se declenche que si le tap est sur le corps du chat sans piece

---

## 4. PANNEAUX IN-GAME (Overlays sur l'hotel)

Ces panneaux s'affichent par-dessus la vue hotel sans quitter le gameplay.

### 4.1 Panneau Boutique (objets)

- **Declencheur :** Tap bouton Boutique dans la barre d'outils
- **Apparition :** Slide depuis le bas, occupe ~40% de l'ecran
- **Contenu :** 7 onglets par categorie (Nourriture, Sommeil, Jeu, Proprete, Decoration, Supports, Tapis). Grille d'icones scrollable. Chaque carte affiche : icone, nom, prix en pieces. Les objets verrouilles (niveau insuffisant) sont grises avec indication du niveau requis. Tap sur un objet → description + stats dans une zone basse, puis mode placement sur la grille
- **Fermeture :** Bouton X ou swipe vers le bas

### 4.2 Panneau Info Chat

- **Declencheur :** Tap sur un chat dans la zone de jeu
- **Apparition :** Slide lateral (droite)
- **Contenu :** Portrait de la race, nom (modifiable par tap pour les chats du refuge — ouvre un champ de saisie inline), race, badge special (si applicable), badge pension/refuge avec timer ou statut, jauge de bonheur globale, 4 jauges de besoins (faim, sommeil, jeu, proprete), liste des caprices actifs avec bouton "Assigner" par caprice, revenu genere par seconde
- **Fermeture :** Tap hors du panneau ou bouton X

### 4.3 Panneau Info Objet

- **Declencheur :** Tap sur un objet place dans la zone de jeu
- **Apparition :** Slide lateral (droite)
- **Contenu :** Icone et nom de l'objet, categorie, efficacite, taille, liste des chats qui l'utilisent actuellement, assignation de caprice (a quel chat, ou aucun), bouton Vendre (avec prix), bouton Deplacer
- **Fermeture :** Tap hors du panneau ou bouton X

### 4.4 Popup Amelioration de Reputation

- **Declencheur :** Tap bouton "Ameliorer" dans le HUD
- **Apparition :** Popup centree
- **Contenu :** Niveau actuel et niveau suivant (avec noms), checklist des conditions (check vert si rempli, croix rouge sinon), cout en pieces, section deblocages (nouvelles races, nouveau plafond de chats). Si conditions non remplies, le bouton est grise
- **Transition :** Tap "Ameliorer" → animation de level-up (flash dore, particules) → notification toast de succes → retour hotel
- **Fermeture :** Bouton X si conditions non remplies

### 4.5 Popup Deblocage d'Etage

- **Declencheur :** Tap sur un etage verrouille dans la navigation
- **Apparition :** Popup centree
- **Contenu :** Nom et numero de l'etage, cout en pieces, stats (capacite, bonus confort, bonus revenus), apercu des nouvelles races accessibles avec portraits
- **Transition :** Tap "Debloquer" → animation de construction (timer si F2P, option pub ou gemmes pour skip) → notification succes → navigation vers le nouvel etage
- **Fermeture :** Bouton X

---

## 5. ECRANS PLEIN ECRAN (depuis l'hotel)

### 5.1 Ecran Gestion des Chats

- **Declencheur :** Tap bouton Gestion dans la barre d'outils
- **Apparition :** Plein ecran avec transition slide
- **Contenu :**
  - Barre de filtres : Tous, En pension, Au refuge, Heureux (>70%), En danger (<40%)
  - Barre de tri : Nom, Race, Bonheur, Etage
  - Liste scrollable de tous les chats : mini portrait rond, nom (+ badge special), race, etage actuel, mini jauge bonheur coloree, badge statut (horloge pension / coeur refuge / check adopte), bouton localiser (oeil = centre la camera sur ce chat et ferme l'ecran)
  - Barre de resume en bas : total chats, bonheur moyen, repartition heureux/neutres/mecontents
- **Transition :** Tap localiser → fermeture ecran + camera centre sur le chat. Bouton retour → retour hotel
- **Fermeture :** Bouton retour (fleche) en haut a gauche

### 5.2 Ecran Boutique Premium

- **Declencheur :** Tap compteur gemmes dans le HUD, ou lien "Obtenir des gemmes" depuis un contexte de manque
- **Apparition :** Plein ecran
- **Contenu :**
  - Solde de gemmes actuel
  - 4 packs de gemmes (Petit 100/0.99, Moyen 550/4.99, Grand 1200/9.99, Mega 2500/19.99) avec badges "Populaire" ou "Meilleur rapport"
  - Encart Abonnement Premium (4.99/mois) avec liste d'avantages
  - Section packs ponctuels : Starter Pack (limite au premier achat), Pack Race, Pack Deco
  - Bouton "Regarder une pub" pour gemmes gratuites (si dispo, 1/jour, 5 gemmes)
- **Transition :** Achat → confirmation store natif → animation de gemmes ajoutees → retour
- **Fermeture :** Bouton retour

### 5.3 Ecran Statistiques

- **Declencheur :** Tap bouton Statistiques (icone graphique) dans la barre d'outils
- **Apparition :** Plein ecran avec transition slide
- **Contenu :**

#### Zone haute — Resume de session

Bandeau recapitulatif de la session en cours :
- Duree de session, pieces collectees cette session, chats accueillis cette session, bonheur moyen actuel

#### Zone principale — Graphiques (7 derniers jours)

4 graphiques en courbes, scrollables horizontalement si l'ecran est trop etroit :

| Graphique | Axe Y | Detail |
|-----------|-------|--------|
| **Revenus quotidiens** | Pieces ($) | Barres empilees : revenus tap-to-collect + pension + adoption. Ligne de tendance |
| **Bonheur moyen** | % (0-100) | Courbe lissee avec zone coloree (vert > 70%, orange 40-70%, rouge < 40%) |
| **Population** | Nb chats | Courbe avec 2 lignes : chats en pension / chats au refuge. Aire remplie sous chaque courbe |
| **Adoptions** | Nb adoptions | Barres journalieres avec annotation des races adoptees |

Chaque graphique est tappable pour afficher les valeurs exactes du jour selectionne (tooltip).

#### Zone basse — Statistiques globales

Liste scrollable de chiffres cles depuis le debut du jeu :

| Categorie | Stats |
|-----------|-------|
| **Hotel** | Pieces totales gagnees, pieces depensees, pieces actuelles, gemmes totales gagnees, gemmes depensees, gemmes actuelles |
| **Chats** | Total chats accueillis, chats en pension total, chats au refuge total, adoptions reussies, chats enfuis, bonheur moyen historique, record bonheur (chat + valeur) |
| **Construction** | Pieces construites, objets places, objets vendus, etages debloques |
| **Progression** | Niveau de reputation, races debloquees (X/8), chats speciaux rencontres (X/8), temps de jeu total, nombre de sessions, plus longue session |
| **Interactions** | Pieces collectees (tap), caresses donnees, "Ramasser tout" utilises, combats survenus |

Chaque stat affiche un **mini indicateur de tendance** (fleche verte haut / rouge bas / gris stable) compare a la semaine precedente.

- **Fermeture :** Bouton retour (fleche) en haut a gauche

---

## 6. POPUPS EVENEMENTIELLES

Declenchees par des evenements de gameplay. Elles apparaissent une par une, jamais empilees. File d'attente si plusieurs arrivent en meme temps.

### 6.1 Popup Arrivee de Chat

- **Declencheur :** Timer de spawn ecoule, un chat se presente
- **Contenu :** Portrait de la race (grand format), nom, race, type (Pension avec duree estimee OU Refuge), indication du niveau de demande
- **Actions :** "Accueillir" (vert) ou "Refuser" (gris). Si l'hotel est plein, le bouton Accueillir est grise avec indication "Hotel plein"
- **Fermeture :** Un des deux boutons

### 6.2 Popup Bilan Pension (depart)

- **Declencheur :** Fin du sejour d'un chat en pension
- **Contenu :** Portrait du chat, nom, bonheur moyen pendant le sejour, detail du paiement (tarif de base, multiplicateur bonheur, pourboire si >80%), total en pieces
- **Monetisation :** Bouton "Doubler les gains" (pub rewarded) a cote du bouton "Collecter"
- **Transition :** "Collecter" → pieces s'ajoutent au compteur avec animation. "Doubler" → pub → x2 pieces → animation
- **Fermeture :** Bouton Collecter

### 6.3 Popup Adoption Reussie

- **Declencheur :** Un adoptant emmene un chat du refuge
- **Contenu :** Portrait du chat avec coeurs, nom, message emotionnel ("Felix a trouve sa famille!"), frais d'adoption en pieces, bonus reputation gagne
- **Monetisation :** Bouton "Doubler la recompense" (pub rewarded)
- **Transition :** Animation celebratoire (confettis, coeurs) → pieces et reputation ajoutees
- **Fermeture :** Bouton "Genial!" ou "Doubler"

### 6.4 Popup Timer de Construction

- **Declencheur :** Le joueur lance la construction d'une piece
- **Contenu :** Visuel de la piece en construction, temps restant, barre de progression
- **Monetisation :** "Terminer maintenant" avec cout en gemmes OU "Regarder une pub" (gratuit, illimite). Si abonne premium : construction instantanee
- **Fermeture :** Bouton "Attendre" → timer visible sur la piece en jeu

### 6.5 Popup Manque de Ressources

- **Declencheur :** Le joueur tente un achat sans assez de pieces ou gemmes
- **Contenu :** "Pas assez de pieces/gemmes!", montant manquant
- **Monetisation :** Si pieces : "Gagner des pieces" (pub rewarded, +50-100$) ou "Boutique". Si gemmes : lien direct vers Boutique Premium
- **Fermeture :** Bouton "OK" ou navigation vers boutique

### 6.6 Popup Confirmation

- **Declencheur :** Actions irreversibles (vendre objet, retour menu, depense importante)
- **Contenu :** Texte de confirmation clair, detail de l'action
- **Actions :** "Confirmer" / "Annuler"
- **Fermeture :** Un des deux boutons

### 6.7 Popup Chat en Danger

- **Declencheur :** Un chat atteint <20% de bonheur et va partir
- **Contenu :** Portrait du chat avec expression triste, alerte urgente, compte a rebours avant depart (8 sec)
- **Monetisation :** "Sauver ce chat" (pub rewarded) → reset bonheur a 50% (1 fois par chat par session)
- **Fermeture :** "Laisser partir" ou "Sauver"

---

## 7. RECOMPENSES JOURNALIERES

### 7.1 Calendrier 7 jours

- **Declencheur principal :** Automatique au premier lancement de chaque jour (apres le rapport d'absence)
- **Acces libre :** Le joueur peut consulter le calendrier a tout moment via l'**icone calendrier dans le HUD haut** (badge point orange si recompense non collectee) ou via le bouton **"Recompenses"** dans le Menu Pause. Si la recompense du jour a deja ete collectee, le calendrier s'affiche en mode consultation (pas de bouton "Collecter", le jour actuel est coche)
- **Contenu :** Grille de 7 jours. Le jour actuel est mis en avant. Les jours passes sont coches. Les jours futurs sont verrouilles mais visibles (anticipation)
- **Recompenses escaladantes :**

| Jour | Recompense |
|------|------------|
| J1 | 100$ |
| J2 | 150$ + 2 gemmes |
| J3 | 200$ |
| J4 | 300$ + 5 gemmes |
| J5 | 500$ |
| J6 | 750$ + 10 gemmes |
| J7 | 1000$ + 20 gemmes + chat special garanti a la prochaine arrivee |

- **Cycle :** Le calendrier se reinitialise apres J7
- **Streak :** Si le joueur manque un jour, le calendrier repart de J1 (pas de rattrapage gratuit). Option pub rewarded pour "rattraper" 1 jour manque
- **Transition :** Tap "Collecter" → animation des recompenses qui s'ajoutent → fermeture

### 7.2 Offre de Retour (Win-back)

- **Declencheur :** Le joueur revient apres 48h+ d'absence
- **Contenu :** "Vous nous avez manque!" + offre speciale limitee (pack de pieces + gemmes a prix reduit, ou bonus gratuit)
- **Duree :** Offre disponible pendant 24h, timer visible
- **Fermeture :** "Acheter" / "Non merci"

---

## 8. PUBS REWARDED - POINTS D'INSERTION

Toutes les pubs sont **toujours optionnelles**. Le joueur ne voit jamais de pub sans l'avoir choisie.

| # | Situation | Recompense | Frequence max | Icone/CTA |
|---|-----------|------------|---------------|-----------|
| 1 | **Bouton HUD** | +50-100$ | 5/jour | Icone camera dans la barre d'outils, badge avec compteur restant |
| 2 | **Fin de sejour pension** | Doubler le paiement | Illimitee (1/evenement) | Bouton dans la popup bilan |
| 3 | **Adoption reussie** | Doubler les frais d'adoption | Illimitee (1/evenement) | Bouton dans la popup adoption |
| 4 | **Construction en cours** | Terminer instantanement | Illimitee | Bouton sur la popup timer |
| 5 | **Cooldown objet premium** | Reset cooldown | Illimitee | Bouton sur l'objet en cooldown |
| 6 | **Boost revenus x2** | 30 min de revenus doubles | 3/jour | Bouton dedie dans le HUD ou popup proposee |
| 7 | **Chat en danger** | Sauver le chat (reset bonheur 50%) | 1/chat/session | Bouton dans la popup alerte |
| 8 | **Manque de pieces** | +50-100$ | 3/jour | Bouton dans la popup de manque |
| 9 | **Gemmes gratuites** | +5 gemmes | 1/jour | Bouton dans la Boutique Premium |
| 10 | **Streak manque** | Rattraper 1 jour de connexion | 1/cycle | Bouton dans le calendrier journalier |

---

## 9. TUTORIEL (FTUE)

Sequence guidee pour les nouveaux joueurs. Chaque etape est un overlay sur le jeu reel avec zone de focus (spotlight) et bulle de dialogue de la mascotte.

### 9.1 Etapes du tutoriel

| # | Etape | Focus | Action guidee | Recompense |
|---|-------|-------|---------------|------------|
| 1 | **Bienvenue** | Ecran complet | Lire l'introduction, rencontrer la mascotte guide | - |
| 2 | **Construire une piece** | Bouton "Creer piece" + grille | Le joueur trace sa premiere piece (3x3 min) | - |
| 3 | **Placer une gamelle** | Boutique > Nourriture | Le joueur achete et place une gamelle | Gamelle offerte |
| 4 | **Accueillir un chat** | Popup d'arrivee | Premier chat (Europeen, pension) arrive automatiquement | - |
| 5 | **Comprendre les besoins** | Panneau Info Chat | Le joueur tap sur le chat et decouvre les 4 jauges | - |
| 6 | **Placer un lit** | Boutique > Sommeil | Le joueur achete et place un coussin | Coussin offert |
| 7 | **Collecter des pieces** | Pieces flottantes | La mascotte explique le tap-to-collect : "Tape sur les pieces pour gagner de l'argent!". Le joueur doit taper 3 pieces. Decouverte du bouton "Ramasser tout" | Bonus 200$ |
| 8 | **Fin du tutoriel** | Ecran complet | Felicitations, recapitulatif, deblocage du jeu libre | 300$ + 10 gemmes |

### 9.2 Regles du tutoriel

- Le joueur ne peut pas acceder aux ecrans non encore presentes
- Le tutoriel est **non skippable** sur la premiere session (bonnes pratiques de retention J1)
- Un rappel contextuel subtil reapparait si le joueur ne fait rien pendant 10 sec
- A la fin du tutoriel, tous les boutons sont demasques progressivement

---

## 10. NOTIFICATIONS PUSH (hors app)

| # | Notification | Delai | Contenu |
|---|-------------|-------|---------|
| 1 | **Construction terminee** | Fin du timer | "Votre piece est prete!" |
| 2 | **Chat malheureux** | Bonheur <30% depuis 5 min | "Minou a besoin de vous..." |
| 3 | **Nouveaux chats en attente** | Apres 2h d'absence | "Des chats attendent devant votre hotel!" |
| 4 | **Recompense journaliere** | 20h apres derniere connexion | "Votre recompense du jour vous attend!" |
| 5 | **Offre limitee** | Declencheur specifique | "Offre speciale pendant 24h!" |
| 6 | **Retour apres absence** | 48h sans connexion | "Vos chats s'ennuient de vous..." |

**Regles :** Max 2 push/jour. Respecter les opt-out. Pas de push les 24 premieres heures.

---

## 11. RAPPORT D'ABSENCE (Idle Revenue)

- **Declencheur :** Le joueur ouvre l'app apres 1h+ d'absence
- **Contenu :** Duree d'absence, nombre de chats geres pendant l'absence, revenus accumules (plafonnes a 80% du revenu reel pour inciter a jouer), evenements en attente (chats partis, nouveaux chats en file)
- **Monetisation :** Bouton "x2 revenus" (pub rewarded). Si abonne premium : x1.5 automatique
- **Transition :** "Collecter" → animation pieces → enchainement avec recompense journaliere si applicable → hotel

---

## 12. LISTE DES ECRANS - RECAPITULATIF

| # | Ecran | Type | Acces |
|---|-------|------|-------|
| 1 | Splash Screen | Plein ecran | Auto au lancement |
| 2 | Ecran de chargement | Plein ecran | Auto au lancement |
| 3 | Ecran titre | Plein ecran | FTUE + retour menu |
| 4 | Introduction narrative (FTUE) | Plein ecran | Premiere session uniquement |
| 5 | **Hotel (gameplay)** | Plein ecran | Ecran principal |
| 6 | Menu Pause | Overlay | Bouton pause |
| 7 | Parametres | Overlay | Depuis pause ou titre |
| 8 | Credits | Plein ecran | Depuis titre |
| 9 | Panneau Boutique objets | Overlay bas | Bouton boutique |
| 10 | Panneau Info Chat | Overlay lateral | Tap sur chat |
| 11 | Panneau Info Objet | Overlay lateral | Tap sur objet |
| 12 | Ecran Gestion des Chats | Plein ecran | Bouton gestion |
| 13 | Ecran Boutique Premium | Plein ecran | Tap gemmes / liens internes |
| 14 | Ecran Statistiques | Plein ecran | Bouton stats dans barre d'outils |
| 15 | Popup Recompense Journaliere | Popup centree | Auto quotidien + icone calendrier HUD + menu Pause |
| 16 | Popup Rapport d'Absence | Popup centree | Auto si absence >1h |
| 17 | Popup Offre Speciale | Popup centree | Contextuel (session start) |
| 18 | Popup Offre de Retour | Popup centree | Retour apres 48h+ |
| 19 | Popup Arrivee Chat | Popup centree | Evenement spawn |
| 20 | Popup Bilan Pension | Popup centree | Evenement depart pension |
| 21 | Popup Adoption Reussie | Popup centree | Evenement adoption |
| 22 | Popup Timer Construction | Popup centree | Lancement construction |
| 23 | Popup Manque de Ressources | Popup centree | Achat impossible |
| 24 | Popup Chat en Danger | Popup centree | Bonheur <20% |
| 25 | Popup Amelioration Reputation | Popup centree | Bouton ameliorer |
| 26 | Popup Deblocage Etage | Popup centree | Tap etage verrouille |
| 27 | Popup Confirmation | Popup centree | Actions irreversibles |
| 28 | Popup Pub Rewarded | Popup centree | Tous contextes ad |
| 29 | Overlay Tutoriel | Overlay spotlight | FTUE (8 etapes) |
| **Total** | **29 ecrans/panneaux/popups** | | |

> Note : les interactions tap-to-collect et caresses ne sont pas des ecrans mais des mecaniques in-game decrites en section 3.5. Le calendrier des recompenses (popup #15) est accessible a la fois automatiquement et manuellement (section 7.1).

---

## 13. COMPOSANTS UI REUTILISABLES

| # | Composant | Variantes |
|---|-----------|-----------|
| 1 | **Bouton primaire** | Normal, desactive (grise), avec prix |
| 2 | **Bouton secondaire** | Normal, desactive |
| 3 | **Bouton danger** | Rouge (actions irreversibles) |
| 4 | **Bouton icone** | Rond, carre, avec badge notification |
| 5 | **Bouton pub rewarded** | Avec icone camera, timer/compteur |
| 6 | **Jauge de progression** | Coloree (vert > orange > rouge), mini (inline), grande |
| 7 | **Onglet** | Actif, inactif, avec badge |
| 8 | **Panneau/cadre** | Standard, premium (dore), alerte |
| 9 | **Carte d'objet boutique** | Normal, selectionne, verrouille |
| 10 | **Ligne de liste (chat)** | Avec portrait, jauges, badges |
| 11 | **Toast notification** | 5 couleurs (succes, warning, erreur, caprice, evenement) |
| 12 | **Slider** | Volume, progression |
| 13 | **Toggle ON/OFF** | Standard |
| 14 | **Badge compteur** | Pastille rouge avec nombre |
| 15 | **Compteur HUD** | Pieces (avec animation +), Gemmes (avec animation +) |
| 16 | **Timer** | Compte a rebours (construction, pension, prochain chat) |
| 17 | **Calendrier 7 jours** | Cases collecte, actif, futur, manque |
| 18 | **Portrait de race** | Rond mini (liste), carre moyen (boutique), grand (popup) |
| 19 | **Piece flottante** | Piece doree animee (idle flottant, fly-to collecte), avec badge nombre si > 5 |
| 20 | **Bouton Ramasser tout** | Icone main+pieces, normal, en cooldown (grise + barre), boost actif (dore) |
| 21 | **Indicateur cooldown caresse** | Arc de cercle autour du chat, se remplit en 30s, clignotement refus |
| 22 | **Badge boost caresse** | Icone "main doree" dans le HUD pendant la duree du boost gemmes |
| 23 | **Icone calendrier HUD** | Icone calendrier avec badge point orange (non collecte) / sans badge (collecte). Tap ouvre la popup calendrier |
| 24 | **Graphique courbe** | Courbe lissee avec zone coloree, tooltip au tap sur un jour, legende, axe Y auto-scale. 4 variantes (revenus, bonheur, population, adoptions) |
| 25 | **Indicateur de tendance** | Mini fleche directionnelle : verte haut (amelioration), rouge bas (degradation), gris stable. Accompagne chaque stat globale |

---

## 14. ADAPTATION PORTRAIT / PAYSAGE

Le jeu supporte les deux orientations avec **priorite au paysage**. Le gameplay (grille, chats, objets) est identique dans les deux modes ; seul le layout de l'UI se reorganise dynamiquement.

### 14.1 Principes generaux

| Regle | Detail |
|-------|--------|
| **Orientation par defaut** | Paysage (le jeu demarre en paysage) |
| **Rotation libre** | Le joueur peut tourner son telephone a tout moment, l'UI s'adapte en < 0.3s |
| **Grille inchangee** | La grille 24x16 reste la meme, la camera ajuste le zoom pour que la zone visible soit coherente |
| **Pas de contenu exclusif** | Toutes les fonctionnalites sont accessibles dans les deux orientations |
| **Zone de jeu maximisee** | L'UI prend le moins de place possible pour laisser la grille visible |

### 14.2 Layout Paysage (prioritaire)

C'est le layout de reference, decrit dans les sections precedentes :

- **HUD haut** : barre horizontale pleine largeur (controles vitesse, pieces, gemmes, chats, confort, reputation, timer)
- **Barre d'outils bas** : barre horizontale pleine largeur (Selection, Creer piece, Supprimer, Boutique, Gestion, Ramasser tout, pub)
- **Navigation etages** : boutons fleche a droite de l'ecran
- **Panneaux lateraux** (Info Chat, Info Objet) : slide depuis la droite, ~30% de la largeur
- **Panneau Boutique** : slide depuis le bas, ~40% de la hauteur
- **Popups** : centrees, taille fixe (~60% de la largeur ecran)
- **Toasts** : bannieres en bas, centrees horizontalement

### 14.3 Layout Portrait

En portrait, les elements se reorganisent pour s'adapter a l'ecran plus etroit et plus haut :

| Element | Comportement en portrait |
|---------|--------------------------|
| **HUD haut** | Se scinde en 2 lignes : ligne 1 = controles vitesse + pieces + gemmes ; ligne 2 = chats + confort + reputation + timer |
| **Barre d'outils bas** | Compactee : icones plus petites, le label texte est masque (icones seules). Si > 6 boutons, les boutons secondaires passent dans un menu "..." |
| **Navigation etages** | Boutons fleche deplaces en bas a droite (au-dessus de la barre d'outils) |
| **Zone de jeu** | La camera zoom pour que la largeur de la grille tienne dans l'ecran. La grille est plus visible verticalement (on voit plus de lignes) |
| **Panneaux lateraux** (Info Chat, Info Objet) | Deviennent des panneaux slide depuis le bas (~50% de la hauteur) au lieu de la droite |
| **Panneau Boutique** | Slide depuis le bas, occupe ~50% de la hauteur (plus grand qu'en paysage) |
| **Popups** | Centrees, taille adaptee (~80% de la largeur ecran, plus haute si necessaire) |
| **Toasts** | Bannieres en bas, pleine largeur |
| **Pieces flottantes** | Meme taille et comportement, la zone de tap reste identique |
| **Ecrans plein ecran** (Gestion, Boutique Premium) | S'adaptent naturellement avec des listes plus longues verticalement |

### 14.4 Regles de transition

- La rotation de l'ecran est detectee automatiquement (pas de bouton de verrouillage dans le jeu, le joueur utilise le verrouillage systeme)
- Pendant une transition, le jeu se met en pause tres brievement (~0.2s) pour recalculer le layout
- Les popups ouvertes restent ouvertes apres rotation, elles se re-dimensionnent en place
- Le panneau boutique et les panneaux lateraux restent ouverts mais changent de direction de slide
- La camera re-cadre la vue pour garder le centre de l'ecran au meme endroit sur la grille

### 14.5 Safe Areas

- Le layout respecte les **safe areas** iOS/Android (notch, barre de navigation, poincon camera)
- Le HUD haut s'insere sous le notch/poincon
- La barre d'outils bas s'insere au-dessus de la barre de navigation systeme
- En portrait, les zones tactiles des boutons restent >= 44x44 dp minimum (accessibilite)

---

*Document associe : [gdd.md](gdd.md) | [2d-art.md](2d-art.md) | [2d-animation.md](2d-animation.md) | [jalons.md](jalons.md)*
