# Cat Hotel Tycoon
## Document de Conception de Jeu (GDD)

**Version:** 1.1
**Date:** Fevrier 2026
**Statut:** Prototype fonctionnel
**Sortie visée:** 1er Avril 2026

---

## Documents associes

| Document | Contenu |
|----------|---------|
| **[2d-art.md](2d-art.md)** | Liste des assets graphiques (characters, environnement, emotes, particules) |
| **[2d-animation.md](2d-animation.md)** | Liste des animations 2D (idles, marche, actions, emotions, objets) |
| **[sound-design.md](sound-design.md)** | Liste des assets audio (musique, vocalisations, UI, evenements) |
| **[ui-ux.md](ui-ux.md)** | Ecrans, cinematiques de navigation, economie F2P, pubs rewarded, daily rewards |
| **[jalons.md](jalons.md)** | Roadmap par jalons (J1: core, J2: contenu, J3: economie F2P) |

## Table des matieres

1. [Vision du jeu](#1-vision-du-jeu)
2. [Gameplay fondamental](#2-gameplay-fondamental)
3. [Systemes de jeu](#3-systèmes-de-jeu)
4. [Les chats](#4-les-chats)
5. [Les objets et services](#5-les-objets-et-services)
6. [Economie et currencies](#6-économie-et-currencies)
7. [Monetisation](#7-monétisation)
8. [Progression et reputation](#8-progression-et-réputation)
9. [Plateforme mobile](#9-plateforme-mobile)
10. [Direction artistique](#10-direction-artistique)
11. [Audio](#11-audio)
12. [References et inspirations](#12-références-et-inspirations)

---

## 1. Vision du jeu

### 1.1 Pitch

> **Construisez et gérez un hôtel de luxe pour chats, à mi-chemin entre pension et refuge.** Des propriétaires déposent leur félin le temps des vacances, d'autres abandonnent leur compagnon en quête d'un nouveau foyer. Accueillez différentes races, veillez à leur bonheur, répondez à leurs caprices... et trouvez-leur une famille aimante. Plus votre réputation grandit, plus des chats de race prestigieux - et des adoptants généreux - viendront frapper à votre porte.

### 1.2 Genre

- **Genre principal:** Simulation / Gestion (Tycoon)
- **Sous-genre:** Casual management, Pet care
- **Perspective:** Vue de dessus 2D isométrique/orthogonale

### 1.3 Pilliers de conception

1. **Accessibilité** - Gameplay simple à comprendre, difficile à maîtriser
2. **Attachement** - Créer un lien émotionnel avec les chats et leur personnalité
3. **Satisfaction** - Boucle de récompense gratifiante (chats heureux = argent = expansion)
4. **Progression** - Sentiment constant d'avancement et de nouveaux défis

### 1.4 Public cible

- **Âge:** 12+
- **Profil:** Joueurs casual, amateurs de jeux de gestion, amoureux des chats
- **Plateforme:** Mobile Android uniquement
- **Sessions:** 5-15 minutes (sessions courtes optimisées mobile)

### 1.5 Unique Selling Points (USP)

1. Chaque race de chat a une personnalité et des besoins distincts
2. Système de "caprices" créant des défis uniques et personnalisés
3. Équilibre entre gestion optimisée et chaos félin imprévisible
4. Chats spéciaux rares avec des noms et personnalités uniques
5. Double dimension pension/refuge : gardez des chats en séjour ou trouvez-leur un foyer

---

## 2. Gameplay fondamental

### 2.1 Boucle de jeu principale

```
┌──────────────────────────────────────────────────────────────────────────┐
│                                                                          │
│  ┌──────────┐    ┌──────────┐    ┌──────────┐    ┌──────────┐           │
│  │ Construire│───▶│ Accueillir│───▶│ Satisfaire│───▶│  Gagner  │          │
│  │  l'hôtel  │    │ des chats │    │  besoins  │    │  argent  │          │
│  └──────────┘    └──────────┘    └──────────┘    └─────┬────┘           │
│       ▲                │                               │                 │
│       │                ▼                               │                 │
│       │         ┌──────────┐    ┌──────────┐          │                 │
│       │         │  Dépôt   │    │ Adoption │──────────┘                 │
│       │         │(pension) │    │ (refuge) │                            │
│       │         └──────────┘    └──────────┘                            │
│       │                                │                                 │
│       └────────────────────────────────┘                                │
│                                                                          │
└──────────────────────────────────────────────────────────────────────────┘
```

**Deux flux de chats :**

- **Dépôt (pension)** - Des propriétaires confient temporairement leur chat. Revenus réguliers pendant le séjour, le chat repart à la fin.
- **Abandon/Adoption (refuge)** - Des chats sont abandonnés et attendent un nouveau foyer. Bonus à l'adoption réussie.

### 2.2 Actions du joueur

| Action | Description | Coût |
|--------|-------------|------|
| **Créer une pièce** | Dessiner un rectangle pour créer une nouvelle pièce | 10$/cellule |
| **Placer un objet** | Installer un service (gamelle, lit, etc.) dans une pièce | Variable |
| **Vendre un objet** | Récupérer 50% du prix d'achat | - |
| **Détruire un mur** | Agrandir une pièce en supprimant un mur intérieur | Gratuit |
| **Assigner un caprice** | Désigner un objet spécifique pour un chat capricieux | Gratuit |
| **Améliorer réputation** | Payer pour passer au niveau supérieur | Variable |
| **Collecter des pièces** | Taper sur les pièces flottantes au-dessus des chats | Gratuit |
| **Ramasser tout** | Collecter toutes les pièces visibles d'un coup (cooldown) | Gratuit |
| **Caresser un chat** | Taper sur un chat pour booster son bonheur | Cooldown par chat |

### 2.3 Conditions de victoire/défaite

- **Pas de défaite définitive** - Le jeu est sandbox avec objectifs progressifs
- **Objectifs de niveau** - Atteindre X chats heureux pour débloquer le niveau suivant
- **Objectif final** - Atteindre le niveau "Maître des Chats" (niveau 10)

### 2.4 Interactions directes (mode connecté)

Quand le joueur est connecté, les revenus ne s'accumulent **pas** automatiquement dans le compteur : le joueur doit **taper sur les pièces** qui apparaissent au-dessus des chats pour les collecter. Cela crée une boucle d'engagement actif.

#### Collecte de pièces (Tap-to-Collect)

| Aspect | Détail |
|--------|--------|
| **Apparition** | Une pièce spawn au-dessus du chat qui génère du revenu (bonheur > 40%) |
| **Fréquence** | 1 pièce par tick de revenu (chaque seconde si bonheur > 70%, chaque 2.5s si 40-70%) |
| **Valeur** | La pièce contient le montant du tick (0.5$ ou 0.2$ × multiplicateurs race/spécial/étage) |
| **Collection** | Tap sur la pièce → elle vole vers le compteur HUD avec animation et son |
| **Accumulation** | Les pièces non collectées s'accumulent sans limite. Cap visuel : max ~20 pièces affichées par chat ; au-delà elles se superposent mais restent collectables |
| **Ramasser tout** | Bouton HUD qui collecte toutes les pièces visibles en une fois. Cooldown de 60 secondes. Le cooldown peut être supprimé avec 1 gemme (achat ponctuel, durée 5 min) |
| **Hors-ligne** | Les revenus s'accumulent automatiquement dans le rapport d'absence (plafonné à 80%) |

#### Caresses (Pet)

Le joueur peut taper directement sur un chat pour le caresser.

| Aspect | Détail |
|--------|--------|
| **Action** | Tap sur un chat (hors zone de pièce flottante) |
| **Effet** | +5 bonheur instantané, animation de caresse (coeurs), son de ronronnement |
| **Cooldown** | 30 secondes par chat (indicateur circulaire visible autour du portrait du chat) |
| **Suppression cooldown** | 5 gemmes → supprime le cooldown de caresses sur tous les chats pendant 5 minutes |
| **Limite** | Un seul boost par chat par période de cooldown |

> Voir [ui-ux.md](ui-ux.md) pour les détails visuels et [2d-animation.md](2d-animation.md) pour les animations de caresse.

---

## 3. Systèmes de jeu

### 3.1 Système de grille

- **Taille:** 24 × 16 cellules
- **Types de cellules:**
  - `Empty` - Extérieur, non utilisable
  - `Floor` - Sol d'une pièce, traversable
  - `Wall` - Mur d'une pièce, bloquant

### 3.2 Système de pièces

- **Taille minimum:** 3×3 cellules (incluant les murs)
- **Création:** Glisser-déposer pour définir un rectangle
- **Extension:** Créer une pièce adjacente détruit automatiquement le mur commun
- **Coût:** 10$ par cellule (multiplié par le niveau de l'étage)

### 3.3 Système d'étages

L'hôtel se développe verticalement. Chaque étage doit être débloqué et permet d'accueillir des chats plus exigeants.

#### Étages disponibles

| Étage | Nom | Coût déblocage | Coût construction | Races accessibles |
|-------|-----|----------------|-------------------|-------------------|
| **RDC** | Rez-de-chaussée | Gratuit (départ) | ×1 | Européen |
| **1er** | Premier étage | 500$ | ×1.5 | + Ragdoll, Siamois |
| **2ème** | Deuxième étage | 1 500$ | ×2 | + British, Maine Coon |
| **3ème** | Troisième étage | 4 000$ | ×3 | + Bengal |
| **4ème** | Quatrième étage | 10 000$ | ×4 | + Chartreux |
| **5ème** | Penthouse | 25 000$ | ×5 | + Norvégien |

#### Mécanique des étages

- **Progression verticale** - Les étages supérieurs sont plus prestigieux
- **Chats exigeants** - Les races haut de gamme préfèrent les étages élevés
- **Coût croissant** - Construction et objets plus chers en hauteur
- **Bonus de standing** - Les étages élevés donnent un bonus de confort naturel
- **Navigation** - Le joueur switch entre étages via des boutons ou swipe vertical

#### Bonus par étage

| Étage | Bonus confort | Bonus revenus | Capacité max |
|-------|---------------|---------------|--------------|
| RDC | +0 | ×1.0 | 10 chats |
| 1er | +5 | ×1.2 | 12 chats |
| 2ème | +10 | ×1.5 | 15 chats |
| 3ème | +15 | ×1.8 | 18 chats |
| 4ème | +20 | ×2.2 | 20 chats |
| 5ème (Penthouse) | +30 | ×3.0 | 25 chats |

#### Comportement des chats par étage

- **Races basiques** (Européen) - Acceptent tous les étages
- **Races intermédiaires** - Préfèrent étage 1-3, tolèrent RDC avec malus bonheur
- **Races prestigieuses** - Exigent étage 3+ minimum, refusent les étages bas
- **Chats spéciaux** - Bonus supplémentaire si placés à l'étage approprié

### 3.4 Système de besoins

Chaque chat possède 4 besoins qui décroissent avec le temps :

| Besoin | Icône | Décroissance/sec | Seuil critique | Objets associés |
|--------|-------|------------------|----------------|-----------------|
| **Faim** (Hunger) | 🍽️ | 0.7 | < 20% | Gamelle, Fontaine, Distributeur |
| **Sommeil** (Sleep) | 😴 | 0.5 | < 20% | Coussin, Lit, Panier |
| **Jeu** (Play) | 🎾 | 0.6 | < 20% | Balle, Arbre à chat |
| **Propreté** (Clean) | 🧹 | 0.45 | < 20% | Litière, Litière auto |

**Formule de décroissance:**
```
décroissance = taux_base × trait_race × multiplicateur_demande × pénalité_réputation
```

### 3.5 Système de bonheur

Le bonheur d'un chat est calculé ainsi :

```
bonheur_base = moyenne(faim, sommeil, jeu, propreté)
bonheur = bonheur_base
        - (20% × caprices_non_satisfaits)
        - pénalité_combat
        + effet_confort
        + bonus_caresse (si caressé récemment : +5, décroît sur 30s)
```

**Seuils de bonheur:**
- **> 70%** - Chat heureux, génère 0.5$/sec
- **40-70%** - Chat neutre, génère 0.2$/sec
- **< 40%** - Chat mécontent
- **< 20%** - Chat part de l'hôtel

### 3.6 Système de confort

Le confort est influencé par :

1. **Décorations dans la pièce** - Chaque décoration ajoute un bonus
2. **Taux d'occupation** - > 80% = surpeuplé (-20 confort max)
3. **Espace personnel** - < 20% occupation = bonus +5

**Impact:** Confort 50 = neutre, 0 = -10 bonheur, 100 = +10 bonheur

### 3.7 Système de caprices

Les caprices sont des demandes spéciales des chats :

| Type | Description | Probabilité | Effet si non satisfait |
|------|-------------|-------------|------------------------|
| **Service** | Veut un objet spécifique exclusif | 8% (normal), 25% (spécial) | -20% bonheur |
| **Élevé** | Veut un service en hauteur | 20% (normal), 35% (spécial) | -20% bonheur |
| **Confort** | Veut une décoration dans sa pièce | 25% (normal), 35% (spécial) | -20% bonheur |

- **Maximum caprices:** 1 (normal) ou 2 (chat spécial)
- **Génération:** Vérifié toutes les 30 secondes
- **Assignation:** Le joueur doit désigner un objet pour le caprice

### 3.8 Système de pathfinding

- **Algorithme:** BFS (Breadth-First Search) pour chemins simples
- **A*** pour chemins à travers zones vides (construction)
- **Optimisation:** Cache des cellules traversables, Maps pour lookups O(1)

### 3.9 Système de combats

Les chats agressifs peuvent :

1. **Déclencher un combat** si bonheur < 50% et autre chat à proximité (< 3 cellules)
2. **Détruire un objet** si bonheur < 35% et objet à proximité (< 2 cellules)

**Conséquences d'un combat:**
- Durée: 3 secondes
- Pénalité: -25 bonheur pour les deux chats
- Cooldown: 15 secondes avant prochain combat

### 3.10 Système de dépôt et adoption

L'hôtel fonctionne comme une pension ET un refuge, avec deux types d'arrivées de chats.

#### Chats en pension (Dépôt)

Des propriétaires confient leur chat pour une durée déterminée.

| Aspect | Détail |
|--------|--------|
| **Arrivée** | Propriétaire dépose le chat avec durée de séjour |
| **Durée** | 2-10 minutes (temps réel), variable selon race |
| **Revenus** | Paiement au départ basé sur bonheur moyen |
| **Fin de séjour** | Le propriétaire revient chercher son chat |
| **Bonus** | Pourboire si bonheur > 80% au départ |
| **Malus** | Pénalité réputation si bonheur < 50% |

**Formule de paiement (pension) :**

```
paiement = tarif_base × durée_séjour × (bonheur_moyen / 100) × multiplicateur_race
pourboire = 20% du paiement si bonheur > 80%
```

#### Chats à adopter (Refuge)

Des chats abandonnés arrivent et cherchent un nouveau foyer.

| Aspect | Détail |
|--------|--------|
| **Arrivée** | Chat abandonné, pas de propriétaire |
| **Marquage** | Icône spéciale "À adopter" |
| **Renommage** | Le joueur peut renommer les chats du refuge (tap sur le nom dans le Panneau Info Chat) |
| **Objectif** | Maintenir bonheur élevé pour attirer adoptants |
| **Adoptants** | Apparaissent si bonheur > 70% pendant 30+ sec |
| **Adoption** | Bonus unique à l'adoption réussie |
| **Échec** | Si bonheur < 30% trop longtemps, le chat s'enfuit |

**Formule de paiement (adoption) :**

```
frais_adoption = tarif_base_race × 2 × (bonheur_actuel / 100)
bonus_réputation = +5 à +15 selon race
```

#### Probabilités d'arrivée

| Type | Probabilité | Condition |
|------|-------------|-----------|
| **Pension** | 70% | Par défaut |
| **Refuge** | 30% | Par défaut |
| **Refuge+** | 40% | Si réputation > 5 (refuge réputé) |

#### Interface visuelle

- **Chat en pension** - Icône horloge + temps restant
- **Chat à adopter** - Icône cœur + "Cherche famille"
- **Adoptant en approche** - Notification "Un adoptant s'intéresse à [Nom]!"

---

## 4. Les chats

### 4.1 Races disponibles

| Race | Couleur | Tempérament | Trait dominant | Réputation min | Agressif |
|------|---------|-------------|----------------|----------------|----------|
| **Européen** | Gris | Équilibré | Aucun | 0 | Non |
| **Ragdoll** | Crème | Docile | Dort +30% | 1 | Non |
| **Siamois** | Beige | Bavard | Joue +50% | 2 | Non |
| **British** | Bleu | Paresseux | Dort +60% | 3 | Non |
| **Maine Coon** | Brun | Gourmand | Faim +60% | 5 | Non |
| **Bengal** | Doré | Énergique | Joue +80% | 6 | Non |
| **Chartreux** | Gris-bleu | Dominant | Équilibré | 8 | **Oui** |
| **Norvégien** | Sombre | Territorial | Faim +30% | 9 | **Oui** |

### 4.2 Statistiques des races

| Race | Multiplicateur demande | Taille | Vitesse |
|------|------------------------|--------|---------|
| Européen | 1.0× | 1.0 | 1.0 |
| Ragdoll | 1.1× | 1.1 | 0.8 |
| Siamois | 1.2× | 0.9 | 1.2 |
| British | 1.3× | 1.05 | 0.7 |
| Maine Coon | 1.4× | 1.3 | 0.9 |
| Bengal | 1.5× | 0.95 | 1.4 |
| Chartreux | 1.6× | 1.1 | 1.0 |
| Norvégien | 1.8× | 1.25 | 1.1 |

### 4.3 Chats spéciaux

Chaque race peut avoir un chat spécial rare avec nom unique :

| Race | Nom | Icône | Chance | Revenu × | Demande × |
|------|-----|-------|--------|----------|-----------|
| Européen | Aristote | 🎓 | 8% | 2.0× | 1.5× |
| Ragdoll | Orion | ⭐ | 10% | 2.5× | 1.8× |
| Siamois | Cleo | 👑 | 7% | 2.2× | 1.6× |
| British | Winston | 🎩 | 6% | 2.3× | 1.7× |
| Maine Coon | Thor | ⚡ | 5% | 2.8× | 2.0× |
| Bengal | Panthera | 🔥 | 5% | 3.0× | 2.2× |
| Chartreux | Napoleon | 🏆 | 4% | 3.2× | 2.3× |
| Norvégien | Odin | ❄️ | 3% | 3.5× | 2.5× |

**Règle d'unicité:** Un seul chat spécial de chaque race peut être présent à la fois.

### 4.4 États des chats

```
┌─────────┐
│  IDLE   │◀────────────────────────────────┐
└────┬────┘                                 │
     │ besoin < 60%                         │
     ▼                                      │
┌─────────┐    objet trouvé    ┌─────────┐  │ besoin > 95%
│ SEEKING │───────────────────▶│ EATING  │──┤
└────┬────┘                    │SLEEPING │  │
     │                         │ PLAYING │  │
     │ pas d'objet             │CLEANING │  │
     │ besoin < 20%            └─────────┘  │
     ▼                                      │
┌─────────┐                                 │
│ UNHAPPY │                                 │
└────┬────┘                                 │
     │ 8 secondes                           │
     ▼                                      │
┌─────────┐    arrivé sortie                │
│ LEAVING │────────────────────────────────▶X (supprimé)
└─────────┘

┌──────────┐
│ FIGHTING │ (chats agressifs uniquement)
└──────────┘

États spéciaux (système pension/refuge):
┌──────────┐
│ PICKUP   │ ──▶ Propriétaire revient chercher (pension)
└──────────┘
┌──────────┐
│ ADOPTED  │ ──▶ Adoptant emmène le chat (refuge)
└──────────┘
```

### 4.5 Noms des chats

Liste de 72+ noms français uniques :
- Classiques: Minou, Felix, Caramel, Luna, Tigrou, Noisette...
- Gourmands: Biscuit, Cookie, Praline, Nougat, Macaron...
- Cosmiques: Cosmos, Lune, Étoile, Comète, Nova...
- Précieux: Rubis, Saphir, Émeraude, Jade, Opale...

Si tous les noms sont utilisés, génération de noms numérotés (ex: "Minou 2").

---

## 5. Les objets et services

### 5.1 Catégorie NOURRITURE (Food)

| Objet | Coût | Vente | Efficacité | Taille | Description |
|-------|------|-------|------------|--------|-------------|
| **Gamelle** | 30$ | 15$ | 1.0× | 1×1 | Service de base |
| **Gamelle d'eau** | 25$ | 12$ | 1.0× | 1×1 | Eau fraîche |
| **Distributeur auto** | 80$ | 40$ | 1.5× | 1×1 | Recharge automatique |
| **Fontaine à eau** | 100$ | 50$ | 1.8× | 1×1 | Eau en circulation |

### 5.2 Catégorie SOMMEIL (Sleep)

| Objet | Coût | Vente | Efficacité | Taille | Description |
|-------|------|-------|------------|--------|-------------|
| **Coussin** | 40$ | 20$ | 1.0× | 1×1 | Couchage basique |
| **Lit pour chat** | 80$ | 40$ | 1.3× | 2×1 | Plus confortable |
| **Panier premium** | 120$ | 60$ | 1.6× | 2×2 | Luxueux |

### 5.3 Catégorie JEU (Play)

| Objet | Coût | Vente | Efficacité | Taille | Description |
|-------|------|-------|------------|--------|-------------|
| **Balle** | 20$ | 10$ | 1.0× | 1×1 | Jouet simple |
| **Arbre à chat** | 150$ | 75$ | 1.8× | 2×2 | Multi-niveaux |

### 5.4 Catégorie PROPRETÉ (Clean)

| Objet | Coût | Vente | Efficacité | Taille | Description |
|-------|------|-------|------------|--------|-------------|
| **Litière** | 35$ | 17$ | 1.0× | 1×1 | Bac standard |
| **Litière auto** | 120$ | 60$ | 1.7× | 2×1 | Auto-nettoyante |

### 5.5 Catégorie DÉCORATION

| Objet | Coût | Confort | Placement | Description |
|-------|------|---------|-----------|-------------|
| **Plante** | 25$ | +2 | Sol | Verdure apaisante |
| **Grande plante** | 45$ | +3 | Sol | Imposante |
| **Lampe** | 30$ | +2 | Sol | Éclairage doux |
| **Étagère** | 40$ | +2 | Mur | Rangement déco |
| **Tableau** | 60$ | +3 | Mur | Art mural |
| **Aquarium** | 150$ | +5 | Sol | Divertissant |
| **Griffoir** | 50$ | +3 | Sol | Entretien griffes |

### 5.6 Supports (meubles pour objets élevés)

| Support | Coût | Capacité | Description |
|---------|------|----------|-------------|
| **Table basse** | 40$ | 2 objets | Support basique |
| **Meuble haut** | 80$ | 3 objets | Plus de place |
| **Étagère murale** | 60$ | 2 objets | Fixé au mur |

### 5.7 Tapis (bonus de zone)

| Tapis | Coût | Bonus | Zone |
|-------|------|-------|------|
| **Tapis confort** | 50$ | Sommeil +20% | 2×2 |
| **Tapis jeu** | 45$ | Jeu +20% | 2×2 |
| **Tapis premium** | 100$ | Tous +10% | 3×3 |

---

## 6. Economie et currencies

### 6.0 Modele dual-currency

Le jeu utilise un systeme standard F2P a deux monnaies :

- **Soft Currency - Pieces ($)** : gagnee en jeu (chats heureux, pension, adoption, recompenses journalieres). Depensee pour construction, objets, amelioration de reputation. Jamais achetable en IAP
- **Hard Currency - Gemmes** : sources gratuites limitees (daily rewards, paliers de reputation, pubs rewarded rares, succes). Achetable en IAP. Depensee pour skip timers, cosmetiques exclusifs, deblocage anticipe de races

> Voir [ui-ux.md](ui-ux.md) pour le detail des currencies, recompenses journalieres et points d'insertion des pubs rewarded.

### 6.1 Sources de revenus

| Source | Montant | Condition |
|--------|---------|-----------|
| **Revenus actifs (tap-to-collect)** | | |
| Chat heureux | 0.5$/tick (1 pièce/sec) | Bonheur > 70%, tap requis en ligne |
| Chat neutre | 0.2$/tick (1 pièce/2.5s) | Bonheur 40-70%, tap requis en ligne |
| Chat spécial | ×2 à ×3.5 | Selon la race |
| **Pension (dépôt)** | | |
| Paiement fin de séjour | 50-200$ | Basé sur durée × bonheur |
| Pourboire | +20% | Si bonheur > 80% au départ |
| **Refuge (adoption)** | | |
| Frais d'adoption | 100-500$ | Basé sur race × bonheur |
| Bonus réputation | +5 à +15 | Selon prestige de la race |
| **Autre** | | |
| Vente d'objets | 50% du prix | - |
| **Recompenses journalieres** | | |
| Calendrier J1-J7 | 100-1000$ + gemmes | Connexion quotidienne |
| Bonus streak J7 | Chat special garanti | Cycle complet |
| **Pubs rewarded** | | |
| Bouton HUD | +50-100$ | 5/jour |
| Doubler gains pension/adoption | x2 paiement | 1/evenement |
| Boost revenus x2 | 30 min | 3/jour |

### 6.2 Depenses

| Dépense | Coût |
|---------|------|
| Construction de pièce | 10$/cellule |
| Objets | Variable (20-150$) |
| Amelioration reputation | 100-5000$ |
| Skip timer (gemmes) | 1-10 gemmes |
| Deblocage anticipe race (gemmes) | 50-200 gemmes |
| Supprimer cooldown "Ramasser tout" (gemmes) | 1 gemme (5 min sans cooldown) |
| Supprimer cooldown caresses (gemmes) | 5 gemmes (5 min sans cooldown sur tous les chats) |

### 6.3 Équilibre économique

**Début de partie:**
- Capital initial: 500$
- Objectif: Créer 1 pièce + services de base
- Revenu attendu: ~2-3$/sec avec 3 chats heureux

**Mi-partie (niveau 5):**
- Capital accumulé: ~2000-3000$
- Objectif: 12+ chats, diversifier les races
- Revenu attendu: ~8-10$/sec

**Fin de partie (niveau 10):**
- Capital accumulé: ~10000$+
- Objectif: 25 chats avec 88% bonheur moyen
- Revenu attendu: ~20$/sec

---

## 7. Monetisation

> Voir [ui-ux.md](ui-ux.md) pour les cinematiques de chaque point de monetisation, les ecrans de boutique, et les flux de pubs rewarded contextuelles.

### 7.1 Philosophie

Le jeu est **Free-to-Play** avec monetisation ethique :

- Pas de pay-to-win
- Progression possible sans payer
- Les pubs rewarded sont toujours optionnelles mais avantageuses
- Respect du temps du joueur
- Jamais plus de 3 popups avant le jeu a chaque session

### 7.2 Deux modes de jeu

#### Mode Connecté (Temps réel)
Quand le joueur est actif dans le jeu :
- Les chats vivent leur vie en temps réel
- Interaction directe avec tous les systèmes
- Besoins décroissent normalement
- **Revenus par tap-to-collect** : les pièces apparaissent au-dessus des chats et le joueur doit les taper pour les collecter (voir section 2.4)
- **Caresses** : le joueur peut taper sur les chats pour booster leur bonheur (cooldown 30s par chat)

#### Mode Hors-ligne (Idle)
Quand le joueur quitte le jeu :
- **Construction différée** - Les nouvelles pièces prennent du temps réel à construire
- **Cooldowns d'objets** - Certains objets premium ont des temps de recharge
- **Revenus passifs limités** - Les chats génèrent moins pendant l'absence
- **Événements en attente** - Nouveaux chats arrivent mais attendent validation

### 7.3 Timers et temps réel

| Action | Temps (gratuit) | Avec pub | Avec premium |
|--------|-----------------|----------|--------------|
| Construction pièce (petite) | 5 min | Instantané | Instantané |
| Construction pièce (moyenne) | 15 min | Instantané | Instantané |
| Construction pièce (grande) | 30 min | Instantané | Instantané |
| Cooldown objet premium | 2h | Reset | Pas de cooldown |
| Attente nouveau chat | 30 min | Instantané | 15 min |

### 7.4 Recompenses journalieres (Daily Rewards)

Calendrier de 7 jours avec recompenses escaladantes. Se reinitialise apres J7. Si le joueur manque un jour, le cycle repart de J1 (option pub pour rattraper 1 jour). Le calendrier est consultable a tout moment via l'icone calendrier dans le HUD ou depuis le Menu Pause (en mode consultation si deja collecte).

| Jour | Recompense |
|------|------------|
| J1 | 100$ |
| J2 | 150$ + 2 gemmes |
| J3 | 200$ |
| J4 | 300$ + 5 gemmes |
| J5 | 500$ |
| J6 | 750$ + 10 gemmes |
| J7 | 1000$ + 20 gemmes + chat special garanti |

### 7.5 Rapport d'absence (Idle Revenue)

Quand le joueur revient apres 1h+ d'absence :

- Les revenus accumules sont plafonnes a 80% du revenu reel (incitation a jouer activement)
- Le joueur peut doubler les gains avec une pub rewarded
- Les abonnes premium ont un bonus automatique de x1.5

### 7.6 Publicites Rewarded

Les pubs sont **toujours optionnelles** et offrent des recompenses. 10 points d'insertion contextuels :

| Recompense | Frequence max | Contexte |
|------------|---------------|----------|
| **+50-100 pieces** | 5/jour | Bouton dedie dans le HUD |
| **Doubler gains pension** | 1/evenement | Popup bilan depart |
| **Doubler gains adoption** | 1/evenement | Popup adoption reussie |
| **Construction instantanee** | Illimitee | Popup timer de construction |
| **Skip cooldown objet** | Illimitee | Sur objet en cooldown |
| **Bonus de revenus x2** | 3/jour | Boost de 30 min |
| **Sauver un chat** | 1/chat/session | Popup chat en danger (<20% bonheur) |
| **+50-100$ urgence** | 3/jour | Popup manque de ressources |
| **+5 gemmes gratuites** | 1/jour | Bouton dans la Boutique Premium |
| **Rattraper 1 jour streak** | 1/cycle | Calendrier journalier |

> Detail des flux et cinematiques : voir [ui-ux.md](ui-ux.md) section 8.

### 7.7 Achats In-App (IAP)

#### Monnaie premium (Gemmes)
- Utilisées pour skip les timers
- Achat d'objets cosmétiques exclusifs
- Déblocage de races en avance

| Pack | Gemmes | Prix | Bonus |
|------|--------|------|-------|
| Petit | 100 | 0.99€ | - |
| Moyen | 550 | 4.99€ | +10% |
| Grand | 1200 | 9.99€ | +20% |
| Mega | 2500 | 19.99€ | +25% |

#### Abonnement Premium (optionnel)
- **Prix:** 4.99€/mois
- **Avantages:**
  - Pas de pubs forcées
  - Construction 2× plus rapide
  - +50% revenus passifs hors-ligne
  - 1 chat spécial garanti/semaine
  - Objets cosmétiques exclusifs

#### Achats ponctuels
- **Starter Pack** (2.99€) - 500 pièces + 50 gemmes + objet rare
- **Pack Race** (1.99€) - Débloquer une race en avance
- **Pack Déco** (0.99€) - Set de décorations thématiques

### 7.8 Economie F2P vs Payant

| Aspect | Joueur F2P | Joueur Payant |
|--------|------------|---------------|
| Progression | Normale | Accélérée |
| Toutes les races | Oui (via réputation) | Oui (plus tôt) |
| Contenu endgame | 100% accessible | 100% accessible |
| Temps pour niveau max | ~4 semaines | ~1-2 semaines |
| Cosmétiques exclusifs | Non | Oui |

---

## 8. Progression et réputation

### 7.1 Niveaux de réputation

| Niveau | Nom | Chats requis | Bonheur min | Coût upgrade | Chats max |
|--------|-----|--------------|-------------|--------------|-----------|
| 0 | Débutant | - | - | - | 5 |
| 1 | Amateur | 3 | 60% | 100$ | 10 |
| 2 | Compétent | 5 | 65% | 200$ | 15 |
| 3 | Professionnel | 7 | 70% | 350$ | 20 |
| 4 | Expert | 10 | 72% | 500$ | 25 |
| 5 | Renommé | 12 | 75% | 750$ | 30 |
| 6 | Célèbre | 14 | 77% | 1000$ | 35 |
| 7 | Prestigieux | 16 | 80% | 1500$ | 40 |
| 8 | Élite | 18 | 82% | 2000$ | 45 |
| 9 | Légendaire | 20 | 85% | 3000$ | 50 |
| 10 | Maître des Chats | 25 | 88% | 5000$ | 55 |

### 7.2 Déblocage de races

La réputation minimale débloque l'arrivée de nouvelles races :
- **Niveau 0:** Européen
- **Niveau 1:** Ragdoll
- **Niveau 2:** Siamois
- **Niveau 3:** British
- **Niveau 5:** Maine Coon
- **Niveau 6:** Bengal
- **Niveau 8:** Chartreux (agressif)
- **Niveau 9:** Norvégien (agressif)

### 7.3 Pénalité de réputation

Si la réputation tombe sous le minimum requis d'une race :
- Les chats de cette race ont des besoins qui décroissent +30% plus vite par niveau de déficit
- Exemple: Un Bengal (min 6) au niveau 4 = +60% de décroissance

---

## 9. Plateforme mobile

### 10.1 Spécifications techniques

- **Plateforme:** Android uniquement (API 24+, Android 7.0 minimum)
- **Résolution:** Adaptatif, optimisé 16:9 et 18:9 (paysage), 9:16 et 9:18 (portrait)
- **Orientation:** Paysage (prioritaire) + Portrait. L'interface s'adapte dynamiquement aux deux orientations. Le gameplay et la grille restent identiques, seul le layout UI se reorganise
- **Moteur:** Unity 6 avec URP
- **Taille app:** < 150 MB (téléchargement initial)

### 10.2 Contrôles tactiles

| Geste | Action |
|-------|--------|
| **Tap** | Sélectionner / Placer / Confirmer |
| **Tap long** | Menu contextuel / Infos détaillées |
| **Drag (1 doigt)** | Pan caméra |
| **Drag (sur objet)** | Déplacer objet |
| **Pinch** | Zoom in/out |
| **Double-tap** | Centrer sur élément |
| **Swipe haut/bas** | Ouvrir/fermer panneau |

### 10.3 Optimisations mobile

- **Batterie:** Mode économie d'énergie disponible
- **Hors-ligne:** Jouable sans connexion (synchro au retour)
- **Notifications:** Push pour événements importants
- **Reprise:** Sauvegarde automatique fréquente

---

## 10. Direction artistique

> Voir [2d-art.md](2d-art.md) pour la liste complete des assets graphiques et [2d-animation.md](2d-animation.md) pour les animations.

### 10.1 Style visuel

- **Style:** Cartoon 2D, palette pastel chaleureuse
- **Inspiration:** Animal Crossing, Neko Atsume, Two Point Hospital
- **Ambiance:** Cosy, chaleureux, mignon sans etre enfantin

### 10.2 Typographie

- **Police principale:** Balsamic Sans (Google Fonts)
- Utilisee pour tous les textes du jeu (HUD, boutons, popups, titres, descriptions)

### 10.3 Palette de couleurs

| Role | Nom | Hex | HSL |
| ------ | ----- | ----- | ----- |
| **Primary Regular** | Rouge pastel | `#D34E4E` | hsl(0, 60%, 57%) |
| **Primary Dark** | Rouge sombre | `#BC3E39` | hsl(2, 53%, 48%) |
| **Neutral Light** | Creme clair | `#F9E7B2` | hsl(45, 86%, 84%) |
| **Neutral Regular** | Sable | `#E9D595` | hsl(45, 66%, 75%) |
| **Neutral Medium** | Sable moyen | `#DDC57A` | hsl(45, 59%, 67%) |
| **Neutral Darker** | Or mat | `#C5A748` | hsl(46, 52%, 53%) |
| **Brown Reference** | Marron | `#56381E` | hsl(28, 48%, 23%) |
| **Brown Darker** | Marron fonce | `#33281F` | hsl(28, 24%, 16%) |
| **Green Lighter** | Vert clair | `#B5B946` | hsl(62, 45%, 50%) |
| **Green Darker** | Vert fonce | `#949735` | hsl(62, 48%, 40%) |

**Usage :**

- **Primary** : boutons d'action, accents, CTA, alertes
- **Neutral** : fonds, panneaux, cadres, HUD
- **Brown** : textes, contours, ombres, elements d'ancrage

### 10.4 Animation des chats

> Detail complet dans [2d-animation.md](2d-animation.md) : 3 idles x 3 directions, marche, 5 actions, emotions, combats.

- **Idle:** 3 variantes (respiration, clignement, queue) x 3 directions (face, cote, dos)
- **Marche:** 4 frames de cycle x 3 directions
- **Action:** Animation specifique par besoin (manger, boire, dormir, jouer, litiere) x 2 directions
- **Heureux:** Petits coeurs, ronronnement visuel
- **Mecontent:** Nuage sombre, oreilles baissees
- **Combat:** Nuage de poussiere avec etoiles

### 10.5 Effets visuels

> Detail complet dans [2d-art.md](2d-art.md) section Particules et Effets.

- Particules de bonheur (coeurs, etoiles)
- Bulles de pensee pour les besoins
- Indicateurs flottants (+$, besoins critiques)
- Surbrillance des objets selectionnes
- Preview de placement (vert = valide, rouge = invalide)

---

## 11. Audio

### 11.1 Musique

- **Style:** Lofi, jazzy, relaxant
- **Pistes:**
  - Menu principal: Douce et accueillante
  - Gameplay jour: Légère et enjouée
  - Gameplay nuit: Calme et apaisante
  - Événement spécial: Plus dynamique

### 11.2 Effets sonores

| Catégorie | Sons |
|-----------|------|
| **Interface** | Clic, validation, erreur, notification |
| **Construction** | Placement, destruction, achat |
| **Chats** | Miaulement (variations), ronronnement, grognement |
| **Actions** | Manger, boire, jouer, dormir |
| **Événements** | Arrivée chat, départ, combat, caprice |

### 11.3 Feedback audio

- Sons positifs pour récompenses (argent, chat heureux)
- Sons d'alerte pour problèmes (chat mécontent, caprice)
- Volume adaptatif selon le nombre de chats

---

## 12. References et inspirations

### 12.1 Jeux similaires

| Jeu | Éléments inspirants |
|-----|---------------------|
| **Neko Atsume** | Collection de chats, simplicité |
| **Two Point Hospital** | Gestion de pièces, humour |
| **Kairosoft games** | Progression, pixel art |
| **Stardew Valley** | Boucle de jeu satisfaisante |
| **The Sims** | Gestion de besoins |

### 12.2 Mecaniques empruntees

- **Neko Atsume:** Chats qui viennent et partent, personnalités
- **Tycoon games:** Économie, expansion, objectifs progressifs
- **Tamagotchi:** Besoins qui décroissent, conséquences si ignorés
- **City builders:** Placement sur grille, construction de zones

---

## Annexe A: Formules mathématiques

### Décroissance des besoins
```
décroissance = taux_base × trait_race × demande_race × (demande_spécial ou 1) × (1 + 0.3 × déficit_réputation)
```

### Calcul du bonheur
```
bonheur = moyenne(besoins) × (1 - 0.2 × caprices_non_satisfaits) - pénalité_combat + (confort - 50) × 0.2 + bonus_caresse
```

### Revenu par seconde
```
revenu = Σ pour chaque chat:
  si bonheur > 70: 0.5 × multiplicateur_spécial
  si bonheur > 40: 0.2 × multiplicateur_spécial
  sinon: 0
```

---

## Annexe B: Feuille de route

### 🎯 Objectif de sortie : 1er Avril 2026

**Scope de la v1.0 :**
- 2-3 races de chats (Européen + 1-2 autres)
- Systèmes core fonctionnels
- Monétisation intégrée
- Tutorial complet

---

### Phase 1: Prototype (✅ Complété - Janvier 2026)

- [x] Système de grille et pièces
- [x] Placement d'objets
- [x] Système de besoins
- [x] IA des chats basique
- [x] Économie de base
- [x] Système de caprices
- [x] Système de réputation

### Phase 2: Alpha (Février 2026)

- [ ] Migration Unity 6 + URP
- [ ] Assets graphiques définitifs (2-3 races)
- [ ] Animations des chats
- [ ] UI mobile épurée
- [ ] Système d'étages (6 niveaux)
- [ ] Système pension/refuge (dépôt et adoption)
- [ ] Système de timers (construction, cooldowns)
- [ ] Sauvegarde cloud
- [ ] Tutoriel interactif

### Phase 3: Beta (Mars 2026)

- [ ] Intégration pubs rewarded
- [ ] Système IAP (gemmes, packs)
- [ ] Effets sonores et musique
- [ ] Notifications push
- [ ] Équilibrage économie F2P
- [ ] Tests utilisateurs (soft launch)
- [ ] Localisation (FR/EN)

### Phase 4: Release (1er Avril 2026)

- [ ] Build Android (Google Play)
- [ ] ASO (App Store Optimization)
- [ ] Marketing de lancement
- [ ] Support communauté

### Phase 5: Post-launch (Q2-Q3 2026)

- [ ] Nouvelles races de chats (contenu régulier)
- [ ] Événements saisonniers
- [ ] Objets cosmétiques
- [ ] Fonctionnalités sociales
- [ ] Analyse des métriques et optimisations

---

*Document cree pour Cat Hotel Tycoon - Janvier 2026, mis a jour Fevrier 2026*

*Documents associes : [2d-art.md](2d-art.md) | [2d-animation.md](2d-animation.md) | [sound-design.md](sound-design.md) | [ui-ux.md](ui-ux.md) | [jalons.md](jalons.md)*
