# Cat Hotel Tycoon - 2D Animation
## Liste des animations a realiser

**Version:** 1.0
**Date:** Fevrier 2026
**Technique:** Sprite sheet / Frame-by-frame
**Framerate cible:** 8-12 FPS (style cartoon fluide)

---

## 1. CONVENTIONS

### 1.1 Directions

Chaque animation est declinee en **3 directions** :
- **Face** (le chat regarde vers la camera / vers le bas de l'ecran)
- **Cote** (le chat regarde vers la gauche - le cote droit est obtenu par flip horizontal)
- **Dos** (le chat regarde vers le haut de l'ecran / dos a la camera)

### 1.2 Nomenclature des fichiers

```
{race}_{variante}_{animation}_{direction}
```

Exemples :
- `europeen_base_idle1_face`
- `ragdoll_special_walk_side`
- `bengal_base_eat_face`

### 1.3 Tailles

La taille de reference est celle de l'Europeen (1.0x). Les autres races sont mises a l'echelle selon leur facteur de taille defini dans le GDD (de 0.9x a 1.3x). L'artiste produit tous les sprites a la taille de reference ; le scaling est gere par le moteur.

---

## 2. ANIMATIONS IDLE (Repos)

Animations jouees quand le chat n'a aucune action en cours. **3 variantes** pour eviter la repetition, selectionnees aleatoirement.

### 2.1 Idle 1 - Respiration

Le chat est assis, respiration subtile (leger mouvement du ventre/thorax).

| Direction | Frames | Loop | Description |
|-----------|--------|------|-------------|
| Face | 4 | Oui | Assis face camera, ventre se gonfle/degonfle |
| Cote | 4 | Oui | Assis de profil, poitrine se gonfle/degonfle |
| Dos | 4 | Oui | Assis de dos, leger mouvement des epaules |

### 2.2 Idle 2 - Clignement des yeux

Le chat est assis, cligne des yeux occasionnellement.

| Direction | Frames | Loop | Description |
|-----------|--------|------|-------------|
| Face | 6 | Oui | Assis, yeux ouverts > mi-clos > fermes > ouverts |
| Cote | 6 | Oui | Profil, meme sequence de clignement |
| Dos | - | - | Non applicable (yeux non visibles) - remplace par queue qui bouge |

### 2.3 Idle 3 - Queue qui bouge

Le chat est assis, sa queue balance doucement.

| Direction | Frames | Loop | Description |
|-----------|--------|------|-------------|
| Face | 6 | Oui | Queue visible sur le cote, balancement lent |
| Cote | 6 | Oui | Queue derriere, balancement de gauche a droite |
| Dos | 6 | Oui | Queue bien visible, balancement ample |

**Total idle par race : 3 anims x 3 directions x ~5 frames = ~45 frames**
**Total idle toutes races (base) : 8 races x 45 = ~360 frames**
**Total idle chats speciaux : 8 speciaux x 45 = ~360 frames**

---

## 3. ANIMATIONS DE DEPLACEMENT

### 3.1 Marche

Cycle de marche standard.

| Direction | Frames | Loop | Description |
|-----------|--------|------|-------------|
| Face | 4 | Oui | Marche vers le bas, pattes alternees |
| Cote | 4 | Oui | Marche laterale, cycle classique 4 pattes |
| Dos | 4 | Oui | Marche vers le haut, vue de dos |

**Total marche par race : 3 directions x 4 frames = 12 frames**

---

## 4. ANIMATIONS D'ACTION

Jouees lorsque le chat interagit avec un objet. Direction principale : **face** et **cote** (le chat se tourne vers l'objet).

### 4.1 Manger (EATING)

Le chat mange a la gamelle / distributeur.

| Direction | Frames | Loop | Description |
|-----------|--------|------|-------------|
| Face | 4 | Oui | Tete baissee, mouvement de mastication |
| Cote | 4 | Oui | Profil, tete baissee vers gamelle |

### 4.2 Boire (DRINKING)

Le chat boit a la gamelle d'eau / fontaine.

| Direction | Frames | Loop | Description |
|-----------|--------|------|-------------|
| Face | 4 | Oui | Tete baissee, langue qui lappe |
| Cote | 4 | Oui | Profil, langue vers l'eau |

### 4.3 Dormir (SLEEPING)

Le chat dort sur un coussin / lit / panier.

| Direction | Frames | Loop | Description |
|-----------|--------|------|-------------|
| Face | 4 | Oui | Couche en boule, yeux fermes, leger mouvement respiration |
| Cote | 4 | Oui | Couche sur le flanc, respiration visible |

### 4.4 Jouer (PLAYING)

Le chat joue avec une balle / arbre a chat.

| Direction | Frames | Loop | Description |
|-----------|--------|------|-------------|
| Face | 6 | Oui | Tapote la balle avec la patte, mouvements vifs |
| Cote | 6 | Oui | Bondit / tapote, queue dressee |

### 4.5 Litiere (CLEANING)

Le chat utilise la litiere.

| Direction | Frames | Loop | Description |
|-----------|--------|------|-------------|
| Face | 4 | Oui | Accroupi, grattage |
| Cote | 4 | Oui | Profil, accroupi dans le bac |

**Total actions par race : 5 actions x 2 directions x ~4.4 frames = ~44 frames**

---

## 5. ANIMATIONS D'EMOTION

### 5.1 Heureux (HAPPY)

| Direction | Frames | Loop | Description |
|-----------|--------|------|-------------|
| Face | 4 | Oui | Yeux plisses (sourire felin), queue dressee, coeurs spawn |
| Cote | 4 | Oui | Meme expression, leger rebond de joie |

### 5.2 Mecontent (UNHAPPY)

| Direction | Frames | Loop | Description |
|-----------|--------|------|-------------|
| Face | 4 | Oui | Oreilles baissees/aplaties, yeux mi-clos, nuage sombre |
| Cote | 4 | Oui | Posture recroquevillee, queue basse |

### 5.3 Depart (LEAVING)

Le chat quitte l'hotel (bonheur trop bas).

| Direction | Frames | Loop | Non | Description |
|-----------|--------|------|-----|-------------|
| Dos | 6 | Non | - | Le chat marche vers la sortie, tete basse, queue entre les pattes |

**Total emotions par race : 2 anims x 2 dir + 1 anim x 1 dir = ~5 directions x ~4.4 frames = ~22 frames**

---

## 6. ANIMATIONS SPECIALES

### 6.1 Combat (FIGHTING)

Animation generique (non specifique a une race) - deux chats s'affrontent.

| Element | Frames | Loop | Description |
|---------|--------|------|-------------|
| **Nuage de combat** | 6 | Oui (3 sec) | Nuage de poussiere tourbillonnant, eclairs et etoiles |
| **Entree combat** | 3 | Non | Le chat se herisse, arque le dos |
| **Sortie combat** | 3 | Non | Le chat se secoue, reprend sa posture |

### 6.2 Arrivee d'un chat (SPAWN)

| Element | Frames | Loop | Description |
|---------|--------|------|-------------|
| **Arrivee pension** | 4 | Non | Chat sort d'une caisse de transport, regarde autour |
| **Arrivee refuge** | 4 | Non | Chat apparait timidement, oreilles en arriere |

### 6.3 Depart d'un chat

| Element | Frames | Loop | Description |
|---------|--------|------|-------------|
| **Depart pension (PICKUP)** | 4 | Non | Chat content rejoint son proprietaire |
| **Depart adoption (ADOPTED)** | 6 | Non | Chat rejoint son nouvel adoptant, coeurs |
| **Depart fuite (LEAVING)** | 4 | Non | Chat mecontent s'enfuit |

**Total animations speciales : ~34 frames (generiques, non par race)**

---

## 7. ANIMATIONS D'OBJETS

Certains objets ont des animations propres.

| # | Objet | Frames | Loop | Description |
|---|-------|--------|------|-------------|
| 1 | **Fontaine a eau** | 4 | Oui | Eau qui coule en boucle |
| 2 | **Distributeur auto** | 4 | Non | Croquettes qui tombent (trigger) |
| 3 | **Litiere auto** | 4 | Non | Mecanisme de nettoyage (trigger) |
| 4 | **Aquarium** | 4 | Oui | Poissons qui nagent, bulles |
| 5 | **Balle** | 4 | Non | Balle qui roule quand un chat joue |
| 6 | **Lampe** | 2 | Oui | Leger scintillement de lumiere (subtil) |

**Total animations objets : ~22 frames**

---

## 8. ANIMATIONS DE PARTICULES/EFFETS

| # | Effet | Frames | Loop | Description |
|---|-------|--------|------|-------------|
| 1 | **Coeur** | 4 | Non | Coeur qui monte et s'estompe |
| 2 | **Etoile** | 4 | Non | Etoile qui brille et disparait |
| 3 | **Piece $** | 4 | Non | Piece qui monte et s'estompe |
| 4 | **Zzz** | 3 | Oui | Z qui flottent et disparaissent en boucle |
| 5 | **Nuage sombre** | 3 | Oui | Nuage qui ondule au-dessus de la tete |
| 6 | **Exclamation** | 3 | Non | "!" qui apparait et disparait |

**Total animations effets : ~21 frames**

---

## 9. ANIMATIONS D'INTERACTION (Tap-to-Collect & Caresses)

### 9.1 Piece flottante (COIN)

Animations de la piece doree qui apparait au-dessus des chats generant du revenu.

| # | Animation | Frames | Loop | Description |
|---|-----------|--------|------|-------------|
| 1 | **Coin spawn** | 3 | Non | La piece apparait (scale 0 → 1 avec leger rebond / pop) |
| 2 | **Coin idle** | 4 | Oui | La piece flotte avec leger mouvement sinusoidal vertical + rotation lente |
| 3 | **Coin collect** | 4 | Non | La piece brille, retrecit et s'envole vers le HUD (fly-to, trainee de particules dorees) |
| 4 | **Coin collect all** | 3 | Non | Variante plus rapide pour le "Ramasser tout" : les pieces partent en cascade decalee |

### 9.2 Caresse (PETTING)

Animation de reaction quand le joueur caresse un chat. **Specifique par race** (meme sprite base mais reaction differente possible).

| Direction | Frames | Loop | Description |
|-----------|--------|------|-------------|
| Face | 4 | Non | Le chat ferme les yeux, leger sourire, oreilles en arriere (plaisir). Coeurs spawn |
| Cote | 4 | Non | Le chat penche la tete, yeux mi-clos, queue dressee. Coeurs spawn |
| Dos | 4 | Non | Le chat arque le dos (plaisir), queue dressee. Coeurs spawn |

### 9.3 Main de caresse (HAND)

| # | Animation | Frames | Loop | Description |
|---|-----------|--------|------|-------------|
| 1 | **Hand appear** | 2 | Non | Main stylisee apparait brievement au-dessus du chat (feedback visuel du tap) |
| 2 | **Hand pet** | 3 | Non | La main effectue un mouvement de caresse puis disparait |

### 9.4 Indicateur cooldown (COOLDOWN)

| # | Animation | Frames | Loop | Description |
|---|-----------|--------|------|-------------|
| 1 | **Cooldown fill** | - | Non | Arc de cercle qui se remplit progressivement (gere par code, pas frame-by-frame) |
| 2 | **Cooldown refuse** | 3 | Non | L'indicateur clignote/secoue brievement quand le joueur tape pendant le cooldown |
| 3 | **Cooldown ready** | 3 | Non | Flash lumineux quand le cooldown est termine (le chat est de nouveau caressable) |

**Total animations interaction : ~14 frames pieces + ~12 frames caresse/race + ~5 frames main + ~6 frames cooldown = ~37 frames + 12/race**

---

## RECAPITULATIF PAR RACE

Chaque race (base + special) necessite :

| Categorie | Nombre d'animations | Frames estimees |
|-----------|---------------------|-----------------|
| Idle (3 variantes x 3 directions) | 9 (dont 1 remplacee) | ~45 |
| Marche (3 directions) | 3 | ~12 |
| Actions (5 actions x 2 directions) | 10 | ~44 |
| Emotions (2 + depart) | 5 | ~22 |
| Caresse (3 directions) | 3 | ~12 |
| **Total par race** | **~30 animations** | **~135 frames** |

### Total general

| Categorie | Assets |
|-----------|--------|
| 8 races de base x ~135 frames | ~1080 frames |
| 8 chats speciaux x ~135 frames | ~1080 frames |
| Animations speciales (generiques) | ~34 frames |
| Animations objets | ~22 frames |
| Animations effets/particules | ~21 frames |
| Animations interaction (pieces, main, cooldown) | ~37 frames |
| **TOTAL ESTIME** | **~2274 frames** |

---

## PRIORITES DE PRODUCTION

### Phase 1 - Alpha (Fevrier 2026) : 2-3 races
1. **Europeen** (race de base, sert de reference)
2. **Ragdoll** ou **Siamois** (2eme race)
3. **Animations d'interaction** (pieces spawn/idle/collect, main caresse, cooldown) - essentielles pour la boucle de gameplay
4. Animations speciales generiques (combat, arrivee, depart)
5. Animations objets essentiels (fontaine, aquarium)
6. Effets/particules

### Phase 2 - Beta (Mars 2026) : races restantes
6. Races 3 a 8
7. Chats speciaux
8. Animations secondaires et polish

---

*Document associe : [gdd.md](gdd.md) | [2d-art.md](2d-art.md) | [ui-ux.md](ui-ux.md) | [jalons.md](jalons.md)*
