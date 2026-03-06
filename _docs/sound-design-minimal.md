# Cat Hotel Tycoon - Sound Design Minimal
## Kit de demarrage — strict minimum

**Version:** 1.0
**Date:** Mars 2026
**Objectif:** Avoir un jeu qui "sonne" avec le minimum absolu de fichiers. 3 races, pas d'economie, pas de monetisation
**Scope:** Europeen, Ragdoll, Siamois — RDC uniquement, 9 objets de base
**Format:** WAV
**Document complet :** [sound-design.md](sound-design.md)

---

## PRINCIPES DU KIT MINIMAL

- Chaque son doit couvrir le maximum de situations (reutilisation)
- Un seul fichier par son, les variations se font par **pitch modulation runtime** (±15%)
- Pas de jingles d'evenements economiques (pas de gemmes, pas de daily, pas d'IAP)
- Pas d'objets ambiants (fontaine, aquarium = J2)
- Pas de notifications push
- Pas de sons d'etages

---

## 1. MUSIQUE

| # | Piste | Duree | Description |
|---|-------|-------|-------------|
| 1 | **Gameplay loop** | 2:00-3:00 | Unique loop lofi/jazzy. Seamless. Doit tenir sur la duree |

**Total : 1 fichier**

---

## 2. JINGLES

Seulement les evenements core qui existent sans economie.

| # | Jingle | Duree | Description |
|---|--------|-------|-------------|
| 1 | **Arrivee de chat** | 2s | Motif joyeux et curieux |
| 2 | **Depart pension** | 2s | Motif satisfaisant de recompense |
| 3 | **Chat en danger** | 2s | Motif inquietant, notes descendantes |
| 4 | **Combat** | 1.5s | Motif comique/cartoon |

**Total : 4 fichiers**

---

## 3. VOCALISATIONS CHATS (3 races)

Set reduit : **8 fichiers par race** au lieu de ~13. On fusionne les variantes et on laisse le pitch runtime gerer la diversite.

### 3.1 Set minimal par race

| # | Son | Description |
|---|-----|-------------|
| 1 | **Miaulement content** | 1 fichier. Pitch ±15% runtime pour variantes |
| 2 | **Miaulement neutre** | 1 fichier |
| 3 | **Miaulement triste** | 1 fichier |
| 4 | **Miaulement urgent** | 1 fichier. Bonheur < 20% |
| 5 | **Ronronnement** | 1 loop. Caresses + idle heureux |
| 6 | **Baillement** | 1 fichier |
| 7 | **Miaulement joueur** | 1 fichier. Pendant le jeu |
| 8 | **Miaulement excite** | 1 fichier. Arrivee, decouverte |

### 3.2 Races incluses

| Race | Tonalite | Note |
|------|----------|------|
| **Europeen** | Moyenne, equilibree | Reference |
| **Ragdoll** | Douce, feutree | Plus doux et lent |
| **Siamois** | Aigue, bavarde | Plus aigu et frequent |

### 3.3 Combat (generique)

| # | Son | Description |
|---|-----|-------------|
| 1 | **Feulement** | Grognement/crachement cartoon |
| 2 | **Nuage de combat** | Whoosh + impacts rapides (combine coups de pattes + nuage) |
| 3 | **Separation** | Son d'arret net |

**Total vocalisations : 3 races x 8 sons + 3 combat = 27 fichiers**

---

## 4. ACTIONS DES CHATS

Set reduit : on garde 1 son par action, les variations se font par pitch.

| # | Action | Son | Description |
|---|--------|-----|-------------|
| 1 | Manger | **Croquettes** | Craquement loop (~1s) |
| 2 | Boire | **Laper** | Langue qui lappe loop (~1s) |
| 3 | Dormir | **Respiration** | Loop respiration douce (~2s) |
| 4 | Jouer | **Tape de patte** | "Boing" cartoon |
| 5 | Litiere | **Grattement** | Son de sable loop |
| 6 | Marcher | **Pas** | "Pat pat" feutre. 1 fichier, pitch ±10% |
| 7 | Courir | **Pas rapides** | Meme base, plus rapide |

**Total : 7 fichiers**

---

## 5. INTERACTION JOUEUR

### 5.1 Tap-to-Collect

| # | Son | Description |
|---|-----|-------------|
| 1 | **Piece collecte** | "Kling" metallique. 1 fichier, pitch ±20% runtime pour variantes et combos |
| 2 | **Ramasser tout** | Cascade rapide de "klings" |

> Le spawn et le fly-to sont silencieux dans le kit minimal.

### 5.2 Caresses

| # | Son | Description |
|---|-----|-------------|
| 3 | **Caresse reussie** | Chime positif. Le ronronnement (section 3) se declenche en parallele |
| 4 | **Cooldown refuse** | "Bonk" cartoon discret |

**Total : 4 fichiers**

---

## 6. INTERFACE (UI)

Set reduit : on mutualise au maximum.

| # | Son | Utilisation | Description |
|---|-----|-------------|-------------|
| 1 | **Tap positif** | Tous boutons primaires | Click lumineux |
| 2 | **Tap neutre** | Boutons secondaires, onglets, fermeture | Click leger |
| 3 | **Tap erreur** | Boutons danger + desactives + fonds insuffisants | Click sourd |
| 4 | **Panneau ouvre** | Slide in boutique, info chat, info objet, popups | Whoosh montant |
| 5 | **Panneau ferme** | Slide out, fermeture popup | Whoosh descendant |
| 6 | **Toast notification** | Tous types de toast (pas de distinction vert/orange/rouge) | Chime neutre d'attention |
| 7 | **Timer expire** | Prochain chat, construction | "Ding" clair |

**Total : 7 fichiers**

---

## 7. EVENEMENTS GAMEPLAY

Set reduit : seulement les evenements qui existent sans economie F2P.

| # | Son | Contexte | Description |
|---|-----|----------|-------------|
| 1 | **Arrivee chat** | Popup arrivee (pension + refuge unifie) | Caisse de transport qui s'ouvre |
| 2 | **Chat accueilli** | Joueur accepte | Son de validation joyeux |
| 3 | **Chat refuse** | Joueur refuse | Son neutre de depart |
| 4 | **Paiement pension** | Bilan depart pension | Pieces qui s'empilent |
| 5 | **Preview valide** | Placement piece/objet, zone verte | Ton positif bref |
| 6 | **Preview invalide** | Zone rouge | Ton negatif bref |
| 7 | **Objet place** | Objet ou piece construit | "Thump" satisfaisant |
| 8 | **Objet detruit** | Vente ou demolition | Crash cartoon leger |
| 9 | **Nouveau caprice** | Bulle de caprice apparait | "Plop" fantasque |
| 10 | **Caprice resolu** | Caprice satisfait | Chime positif |
| 11 | **Alerte danger** | Chat < 20% bonheur | Son d'urgence |
| 12 | **Chat parti** | Chat quitte l'hotel | Porte qui se ferme |
| 13 | **XP confort** | +1 XP service | Micro "tik" positif |

**Total : 13 fichiers**

---

## RECAPITULATIF

| Categorie | Fichiers |
|-----------|----------|
| Musique (1 loop) | 1 |
| Jingles (4 evenements core) | 4 |
| Vocalisations (3 races x 8) | 24 |
| Combat (generique) | 3 |
| Actions chats | 7 |
| Interaction joueur (collect + caresses) | 4 |
| Interface UI | 7 |
| Evenements gameplay | 13 |
| **TOTAL** | **63 fichiers** |

---

## CE QUI N'EST PAS DANS LE KIT MINIMAL

Tout ce qui suit est dans le [document complet](sound-design.md) et sera ajoute progressivement :

- 5 races supplementaires (British, Maine Coon, Bengal, Chartreux, Norvegien)
- Variantes de miaulements (2e et 3e variantes par emotion)
- Ronronnement fort (distinct du ronronnement normal)
- Jingles : bienvenue, adoption, level-up reputation, deblocage etage, daily reward, rapport d'absence, tutoriel
- Sons de pieces : spawn, fly-to, compteur increment, combo pitch
- Sons de caresses : main apparait, cooldown pret
- UI : toggle on/off, scroll, changement d'etage, ecran plein transition
- Toasts : distinction par couleur (5 types → 1 seul dans le kit minimal)
- Compteurs : pieces ajoutees, gemmes ajoutees, badge notification
- Depart pension : pourboire, depart content
- Adoption : adoptant, confettis, coeurs
- Recompenses journalieres : 5 sons
- Rapport d'absence : decompte revenus
- Chat en danger : compte a rebours, chat sauve
- Economie : achat reussi (pieces/gemmes), fonds insuffisants
- Objets ambiants : fontaine, aquarium, distributeur auto, litiere auto, lampe
- Notifications push : 6 sons
- Sons supplementaires d'actions : gamelle cliquetis, ronflement, installation, bond/saut, griffoir, recouvrement litiere

---

## CHECKLIST PRODUCTION

Liste a cocher pour le sound designer :

**Musique**
- [ ] Gameplay loop (2-3 min, seamless)

**Jingles**
- [ ] Arrivee de chat
- [ ] Depart pension
- [ ] Chat en danger
- [ ] Combat

**Europeen (8 sons)**
- [ ] Miaulement content
- [ ] Miaulement neutre
- [ ] Miaulement triste
- [ ] Miaulement urgent
- [ ] Ronronnement (loop)
- [ ] Baillement
- [ ] Miaulement joueur
- [ ] Miaulement excite

**Ragdoll (8 sons)** — meme liste, tonalite douce/feutree

**Siamois (8 sons)** — meme liste, tonalite aigue/bavarde

**Combat**
- [ ] Feulement
- [ ] Nuage de combat
- [ ] Separation

**Actions**
- [ ] Croquettes (loop)
- [ ] Laper (loop)
- [ ] Respiration (loop)
- [ ] Tape de patte
- [ ] Grattement litiere (loop)
- [ ] Pas de marche
- [ ] Pas de course

**Interaction**
- [ ] Piece collecte
- [ ] Ramasser tout
- [ ] Caresse reussie
- [ ] Cooldown refuse

**UI**
- [ ] Tap positif
- [ ] Tap neutre
- [ ] Tap erreur
- [ ] Panneau ouvre
- [ ] Panneau ferme
- [ ] Toast notification
- [ ] Timer expire

**Evenements**
- [ ] Arrivee chat
- [ ] Chat accueilli
- [ ] Chat refuse
- [ ] Paiement pension
- [ ] Preview valide
- [ ] Preview invalide
- [ ] Objet place
- [ ] Objet detruit
- [ ] Nouveau caprice
- [ ] Caprice resolu
- [ ] Alerte danger
- [ ] Chat parti
- [ ] XP confort

---

*Kit minimal de [sound-design.md](sound-design.md) — 63 fichiers / ~215 dans le document complet*

*Document associe : [gdd.md](gdd.md) | [sound-design.md](sound-design.md) | [jalons.md](jalons.md)*
