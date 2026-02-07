# Cat Hotel Tycoon - 2D Art
## Liste des assets graphiques

**Version:** 1.0
**Date:** Fevrier 2026
**Style:** Cartoon 2D, couleurs vives et chaleureuses
**Perspective:** Vue de dessus (top-down / legere isometrie)
**Orientation:** Paysage (prioritaire) + Portrait — les assets doivent etre lisibles dans les deux orientations
**Inspirations:** Neko Atsume, Animal Crossing, Two Point Hospital

---

## 1. CHARACTERS (Chats)

### 1.1 Races - Sprites de base

Chaque race necessite **3 vues** (face, cote, dos) servant de base aux animations.

| # | Race | Couleur dominante | Taille relative | Notes visuelles |
|---|------|-------------------|-----------------|-----------------|
| 1 | **Europeen** | Gris | 1.0 (reference) | Chat tigre classique, rayures discretes |
| 2 | **Ragdoll** | Creme | 1.1 (plus grand) | Poil mi-long, yeux bleus, masque facial |
| 3 | **Siamois** | Beige | 0.9 (plus petit) | Extremites foncees (pattes, museau, oreilles), yeux bleus |
| 4 | **British Shorthair** | Bleu (gris-bleu) | 1.05 | Rondouillard, grosse tete, fourrure dense |
| 5 | **Maine Coon** | Brun | 1.3 (le plus grand) | Poil long, touffe aux oreilles, imposant |
| 6 | **Bengal** | Dore | 0.95 | Robe tachetee/rosetee, allure feline athletique |
| 7 | **Chartreux** | Gris-bleu | 1.1 | Musculeux, yeux cuivre/or, sourire naturel |
| 8 | **Norvegien** | Sombre (brun fonce) | 1.25 | Poil tres long, collerette, queue touffue |

**Livrable par race :**
- Sprite face (idle frame de reference)
- Sprite cote gauche (idle frame de reference)
- Sprite dos (idle frame de reference)
- Le cote droit est obtenu par miroir horizontal du cote gauche

**Total : 8 races x 3 vues = 24 sprites de base**

---

### 1.2 Variantes "Chats Speciaux"

Chaque race possede un chat special rare avec un design unique reconnaissable. Il doit etre clairement identifiable comme membre de sa race, mais avec des elements distinctifs (accessoire, marquage, aura...).

| # | Nom | Race | Icone | Piste visuelle |
|---|-----|------|-------|----------------|
| 1 | **Aristote** | Europeen | Mortarboard | Petites lunettes rondes, air studieux |
| 2 | **Orion** | Ragdoll | Etoile | Marque en forme d'etoile sur le front, reflets lumineux |
| 3 | **Cleo** | Siamois | Couronne | Petit diademe/collier dore, allure royale |
| 4 | **Winston** | British | Haut-de-forme | Noeud papillon, air distingue |
| 5 | **Thor** | Maine Coon | Eclair | Marque eclair sur le flanc, criniere plus imposante |
| 6 | **Panthera** | Bengal | Flamme | Rosettes plus prononcees, regard intense, aura ardente |
| 7 | **Napoleon** | Chartreux | Trophee | Medaille autour du cou, posture fiere |
| 8 | **Odin** | Norvegien | Flocon | Un oeil ferme (borgne), touches de givre sur la fourrure |

**Livrable par chat special :**
- Memes 3 vues que la race de base mais avec les variantes visuelles
- L'icone distinctive en version sprite (flottante au-dessus du chat)

**Total : 8 chats speciaux x 3 vues = 24 sprites + 8 icones flottantes**

---

### 1.3 Emotes / Bulles d'etat

Petites icones qui apparaissent au-dessus des chats pour communiquer leur etat.

#### Bulles de besoins (quand le besoin est critique < 20%)
| # | Emote | Description |
|---|-------|-------------|
| 1 | **Bulle Faim** | Bulle de pensee avec une gamelle / os de poisson |
| 2 | **Bulle Sommeil** | Bulle de pensee avec des "Zzz" |
| 3 | **Bulle Jeu** | Bulle de pensee avec une balle / pelote |
| 4 | **Bulle Proprete** | Bulle de pensee avec une goutte d'eau / brosse |

#### Bulles d'emotion
| # | Emote | Description |
|---|-------|-------------|
| 5 | **Heureux** | Coeurs flottants (2-3 petits coeurs) |
| 6 | **Tres heureux** | Coeurs + etoiles |
| 7 | **Neutre** | Aucune bulle |
| 8 | **Mecontent** | Petit nuage sombre au-dessus de la tete |
| 9 | **En colere** | Symbole colere (veine manga) + nuage sombre |
| 10 | **Combat** | Nuage de poussiere avec etoiles et eclairs |

#### Bulles de statut (systeme pension/refuge)
| # | Emote | Description |
|---|-------|-------------|
| 11 | **En pension** | Petite icone horloge |
| 12 | **A adopter** | Petit coeur avec point d'interrogation |
| 13 | **Adoption en cours** | Coeur plein avec exclamation |

#### Bulles de caprice
| # | Emote | Description |
|---|-------|-------------|
| 14 | **Caprice service** | Bulle de pensee avec silhouette de l'objet voulu |
| 15 | **Caprice hauteur** | Bulle de pensee avec fleche vers le haut |
| 16 | **Caprice confort** | Bulle de pensee avec etoile de confort |

**Total : 16 emotes/bulles**

---

## 2. ENVIRONNEMENT

### 2.1 Tiles de construction

#### Sol
| # | Tile | Description |
|---|------|-------------|
| 1 | **Sol basique** | Carrelage/parquet standard (RDC) |
| 2 | **Sol etage 1** | Variante legere (premier etage) |
| 3 | **Sol etage 2** | Variante plus soignee |
| 4 | **Sol etage 3** | Variante haut de gamme |
| 5 | **Sol etage 4** | Variante luxueuse |
| 6 | **Sol penthouse** | Variante prestige (marbre, parquet massif) |

#### Murs
| # | Tile | Description |
|---|------|-------------|
| 7 | **Mur horizontal** | Segment de mur horizontal |
| 8 | **Mur vertical** | Segment de mur vertical |
| 9 | **Coin mur (4 rotations)** | Jonction de murs en angle |
| 10 | **Mur T (4 rotations)** | Jonction en T |
| 11 | **Mur croix** | Jonction en croix |

#### Exterieur
| # | Tile | Description |
|---|------|-------------|
| 12 | **Cellule vide** | Zone exterieure (herbe, gravier, ou vide) |
| 13 | **Zone de construction** | Preview lors de la creation de piece |

#### Indicateurs de placement
| # | Tile | Description |
|---|------|-------------|
| 14 | **Overlay valide** | Surbrillance verte semi-transparente |
| 15 | **Overlay invalide** | Surbrillance rouge semi-transparente |
| 16 | **Overlay selection** | Surbrillance bleue/blanche de l'objet selectionne |

**Total : ~16 tiles (+ rotations)**

---

### 2.2 Objets - Nourriture

| # | Objet | Taille grille | Description visuelle |
|---|-------|---------------|----------------------|
| 1 | **Gamelle** | 1x1 | Bol classique avec croquettes |
| 2 | **Gamelle d'eau** | 1x1 | Bol avec eau (reflet) |
| 3 | **Distributeur auto** | 1x1 | Machine/silo avec gamelle integree |
| 4 | **Fontaine a eau** | 1x1 | Fontaine circulaire avec eau en circulation |

### 2.3 Objets - Sommeil

| # | Objet | Taille grille | Description visuelle |
|---|-------|---------------|----------------------|
| 5 | **Coussin** | 1x1 | Coussin rond moelleux |
| 6 | **Lit pour chat** | 2x1 | Petit lit sureleve avec rebords |
| 7 | **Panier premium** | 2x2 | Grand panier tresse luxueux avec coussins |

### 2.4 Objets - Jeu

| # | Objet | Taille grille | Description visuelle |
|---|-------|---------------|----------------------|
| 8 | **Balle** | 1x1 | Balle coloree (avec grelot?) |
| 9 | **Arbre a chat** | 2x2 | Structure multi-niveaux avec plateformes et griffoir |

### 2.5 Objets - Proprete

| # | Objet | Taille grille | Description visuelle |
|---|-------|---------------|----------------------|
| 10 | **Litiere** | 1x1 | Bac ouvert avec litiere |
| 11 | **Litiere auto** | 2x1 | Bac ferme/dome avec mecanisme |

### 2.6 Objets - Decorations

| # | Objet | Taille grille | Placement | Description visuelle |
|---|-------|---------------|-----------|----------------------|
| 12 | **Plante** | 1x1 | Sol | Petit pot avec plante verte |
| 13 | **Grande plante** | 1x1 | Sol | Grand pot avec plante imposante |
| 14 | **Lampe** | 1x1 | Sol | Lampe sur pied, lumiere chaude |
| 15 | **Etagere** | 1x1 | Mur | Etagere murale avec bibelots |
| 16 | **Tableau** | 1x1 | Mur | Cadre avec peinture (chat?) |
| 17 | **Aquarium** | 1x1 | Sol | Aquarium rectangulaire avec poissons |
| 18 | **Griffoir** | 1x1 | Sol | Poteau recouvert de corde/sisal |

### 2.7 Objets - Supports (meubles)

| # | Objet | Taille grille | Description visuelle |
|---|-------|---------------|----------------------|
| 19 | **Table basse** | 1x1 | Petite table en bois, 2 emplacements |
| 20 | **Meuble haut** | 1x1 | Meuble type commode, 3 emplacements |
| 21 | **Etagere murale** | 1x1 | Planche fixee au mur, 2 emplacements |

### 2.8 Objets - Tapis

| # | Objet | Taille grille | Description visuelle |
|---|-------|---------------|----------------------|
| 22 | **Tapis confort** | 2x2 | Tapis moelleux, tons chauds |
| 23 | **Tapis jeu** | 2x2 | Tapis colore/motifs ludiques |
| 24 | **Tapis premium** | 3x3 | Grand tapis persan/luxueux |

**Total objets : 24**

---

### 2.9 Icones de boutique

Chaque objet necessite une **icone de boutique** (vignette carree simplifiee pour l'UI).

> **Contrainte double orientation :** les icones doivent rester lisibles a petite taille (mode portrait compact). Privilegier des silhouettes claires avec peu de details fins. Taille minimum effective : 48x48 dp.

**Total : 24 icones d'objets**

---

## 3. PARTICULES ET EFFETS

| # | Effet | Description |
|---|-------|-------------|
| 1 | **Coeurs** | Petits coeurs roses flottants (bonheur) |
| 2 | **Etoiles** | Etoiles dorees scintillantes |
| 3 | **Pieces** | Piece "$" dorée qui flotte vers le haut |
| 4 | **Nuage de poussiere** | Pour les combats |
| 5 | **Eclairs** | Petits eclairs dans le nuage de combat |
| 6 | **Zzz** | Lettres Z flottantes (chat qui dort) |
| 7 | **Notes de musique** | Pour chat qui ronronne (bonheur) |
| 8 | **Gouttes de sueur** | Stress/mecontentement |

### 3.2 Elements d'interaction (Tap-to-Collect & Caresses)

| # | Asset | Description |
|---|-------|-------------|
| 9 | **Piece flottante ($)** | Piece doree avec symbole "$", vue de dessus, taille ~32x32 dp minimum. Doit etre lisible dans les deux orientations (portrait et paysage) |
| 10 | **Piece flottante (lot)** | Variante "grosse piece" pour les gros gains (pension/adoption). Legere brillance doree |
| 11 | **Badge compteur pieces** | Pastille numerique (ex: "x12") qui s'affiche quand > 5 pieces s'accumulent sur un chat |
| 12 | **Icone main caresse** | Main stylisee cartoon apparaissant brievement lors d'une caresse. Semi-transparente |
| 13 | **Indicateur cooldown caresse** | Arc de cercle / anneau qui entoure le chat pendant le cooldown (30s). Couleur neutre, se remplit progressivement |
| 14 | **Icone main doree (boost)** | Variante doree de l'icone main, affichee dans le HUD pendant le boost gemmes caresses |
| 15 | **Icone bouton Ramasser tout** | Icone combinant une main et des pieces, pour le bouton HUD. Variantes : normal, cooldown (grise), boost (doree) |

**Total : 8 effets/particules + 7 assets d'interaction = 15 assets effets/interaction**

---

## RECAPITULATIF

| Categorie | Nombre d'assets |
|-----------|-----------------|
| Chats - Sprites de base (8 races x 3 vues) | 24 |
| Chats - Sprites speciaux (8 x 3 vues) | 24 |
| Chats - Icones speciaux flottantes | 8 |
| Emotes / Bulles | 16 |
| Tiles construction (sol, murs, overlays) | ~16 |
| Objets environnement | 24 |
| Icones boutique | 24 |
| Effets / Particules | 8 |
| Interaction (pieces, caresses, cooldown) | 7 |
| **TOTAL** | **~151 assets** |

---

*Document associe : [gdd.md](gdd.md) | [2d-animation.md](2d-animation.md) | [ui-ux.md](ui-ux.md) | [jalons.md](jalons.md)*
