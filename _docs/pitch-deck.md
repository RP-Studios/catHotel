---
marp: true
theme: uncover
paginate: true
backgroundColor: #0D1117
color: #E6EDF3
style: |
  @import url('https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700;800;900&display=swap');

  :root {
    --accent: #F97316;
    --accent-light: #FB923C;
    --accent-glow: #FDBA74;
    --bg-dark: #0D1117;
    --bg-card: #161B22;
    --bg-card-light: #1C2333;
    --text: #E6EDF3;
    --text-muted: #8B949E;
    --green: #3FB950;
    --blue: #58A6FF;
    --purple: #BC8CFF;
    --pink: #F778BA;
    --yellow: #D29922;
  }

  section {
    font-family: 'Inter', 'Segoe UI', sans-serif;
    padding: 50px 70px;
    display: flex;
    flex-direction: column;
    justify-content: center;
  }

  h1 {
    color: var(--accent);
    font-weight: 800;
    font-size: 2.4em;
    letter-spacing: -1px;
    line-height: 1.1;
    margin-bottom: 0.3em;
  }

  h2 {
    color: var(--text);
    font-weight: 700;
    font-size: 1.7em;
    letter-spacing: -0.5px;
    margin-bottom: 0.4em;
  }

  h3 {
    color: var(--accent-light);
    font-weight: 600;
    font-size: 1.1em;
    text-transform: uppercase;
    letter-spacing: 2px;
    margin-bottom: 0.5em;
  }

  p, li {
    font-size: 0.78em;
    line-height: 1.6;
    color: var(--text-muted);
  }

  strong {
    color: var(--text);
    font-weight: 600;
  }

  em {
    color: var(--accent-glow);
    font-style: normal;
    font-weight: 600;
  }

  blockquote {
    border-left: 4px solid var(--accent);
    background: var(--bg-card);
    padding: 16px 24px;
    border-radius: 0 12px 12px 0;
    margin: 16px 0;
    font-size: 0.82em;
  }

  blockquote p {
    color: var(--text);
    margin: 0;
  }

  table {
    font-size: 0.72em;
    width: 100%;
    border-collapse: separate;
    border-spacing: 0;
    border-radius: 12px;
    overflow: hidden;
    margin: 12px 0;
  }

  thead th {
    background: var(--accent);
    color: white;
    font-weight: 700;
    padding: 10px 14px;
    text-align: left;
    letter-spacing: 0.3px;
  }

  tbody td {
    background: var(--bg-card);
    padding: 8px 14px;
    border-bottom: 1px solid #21262D;
    color: var(--text-muted);
  }

  tbody td:first-child {
    color: var(--text);
    font-weight: 600;
  }

  tbody tr:last-child td {
    border-bottom: none;
  }

  code {
    background: var(--bg-card);
    padding: 2px 8px;
    border-radius: 6px;
    font-size: 0.9em;
    color: var(--accent-glow);
  }

  ul {
    list-style: none;
    padding-left: 0;
  }

  ul li::before {
    content: "→ ";
    color: var(--accent);
    font-weight: 700;
  }

  a {
    color: var(--blue);
    text-decoration: none;
  }

  section.lead {
    text-align: center;
    justify-content: center;
  }

  section.lead h1 {
    font-size: 3.2em;
    margin-bottom: 0.1em;
  }

  footer {
    color: #30363D !important;
    font-size: 0.55em !important;
  }

  /* Utility */
  .cols { display: flex; gap: 40px; align-items: flex-start; }
  .col { flex: 1; }
  .small { font-size: 0.65em; color: var(--text-muted); }
  .tag { display: inline-block; background: var(--bg-card); border: 1px solid #30363D; padding: 2px 10px; border-radius: 20px; font-size: 0.7em; margin: 2px; color: var(--text-muted); }
  .highlight { color: var(--accent); font-weight: 700; }
  .big-number { font-size: 2.8em; font-weight: 800; color: var(--accent); line-height: 1; }
  .metric-label { font-size: 0.7em; color: var(--text-muted); text-transform: uppercase; letter-spacing: 1px; }
---

<!-- _class: lead -->
<!-- _paginate: false -->

# 🐱 CAT HOTEL

### Royal Pourceau Studios

**Construisez et gerez un hotel de luxe pour chats**

<span class="small" style="margin-top: 40px; display:block;">Mobile Android · Free-to-Play · Release 1er Avril 2026</span>

---

<!-- _class: lead -->

### Le Concept

# Pension + Refuge = *Cat Hotel*

Des proprietaires deposent leur chat le temps des vacances.
D'autres abandonnent leur compagnon en quete d'un foyer.

**Accueillez. Cajolez. Trouvez-leur une famille.**

> Plus votre reputation grandit, plus des chats prestigieux
> et des adoptants genereux viendront frapper a votre porte.

---

### Opportunite de marche

# Le casual pet care explose

- Les jeux **pet simulation** generent *+2.5 Mds$* de revenus annuels sur mobile
- **Neko Atsume** : +50M de telechargements, monetisation minimaliste, enorme potentiel inexploite
- Les jeux **tycoon/idle** sont le genre #1 en retention sur les stores
- Creneau vide : **aucun tycoon de pension pour chats** sur le marche

<div style="margin-top: 20px;">

| Concurrent | Genre | Ce qui manque |
|-----------|-------|---------------|
| **Neko Atsume** | Collection passive | Pas de gestion, pas de tycoon |
| **Idle Cat Tycoon** | Idle pur | Pas d'attachement, pas de personnalite |
| **Two Point Hospital** | Tycoon PC | Pas mobile, pas de chats |

</div>

---

### Gameplay

# La boucle qui rend accro

<div style="text-align: center; margin: 20px 0;">

`Construire` → `Accueillir` → `Satisfaire` → `Collecter` → `Agrandir`

</div>

- **Construisez** des pieces et placez des services (gamelles, lits, jouets, litieres)
- **Accueillez** des chats en *pension* (revenus reguliers) ou en *refuge* (bonus adoption)
- **Satisfaites** 4 besoins (faim, sommeil, jeu, proprete) + les *caprices* uniques
- **Collectez** les pieces en tapant activement — engagement constant
- **Agrandissez** : 6 etages, 8 races, reputation croissante

> Le joueur ne regarde pas passivement : il **tape**, **caresse**, **assigne**, **optimise**.

---

### Engagement actif

# Tap-to-Collect + Caresses

**Quand le joueur est connecte, rien ne tombe tout seul.**

<div class="cols">
<div class="col">

**Pieces flottantes**
- Chaque chat heureux genere des pieces
- Le joueur *tape* pour collecter
- Bouton "Ramasser tout" (cooldown 60s)
- Accumulation infinie = toujours une raison de revenir

</div>
<div class="col">

**Caresser les chats**
- Tap sur un chat = *+5 bonheur*
- Cooldown 30s par chat
- Animation coeurs + ronronnement
- Suppression cooldown = 5 gemmes

</div>
</div>

> Resultat : sessions actives de 5-15 min, pas un idle qu'on oublie.

---

### Les chats

# 8 races · 8 speciaux · 72+ noms

<div class="cols">
<div class="col">

| Race | Personnalite | Taille |
|------|-------------|--------|
| **Europeen** | Equilibre | 1.0x |
| **Ragdoll** | Docile, dort +30% | 1.1x |
| **Siamois** | Bavard, joue +50% | 0.9x |
| **British** | Paresseux, dort +60% | 1.05x |
| **Maine Coon** | Gourmand, faim +60% | 1.3x |
| **Bengal** | Energique, joue +80% | 0.95x |
| **Chartreux** | Dominant, *agressif* | 1.1x |
| **Norvegien** | Territorial, *agressif* | 1.25x |

</div>
<div class="col">

**Chats speciaux (rares)**

Aristote 🎓 · Orion ⭐ · Cleo 👑
Winston 🎩 · Thor ⚡ · Panthera 🔥
Napoleon 🏆 · Odin ❄️

- Revenus *x2 a x3.5*
- Caprices doubles
- Design unique + icone flottante
- Un seul par race a la fois

</div>
</div>

---

### Profondeur strategique

# Caprices · Combats · Reputation

<div class="cols">
<div class="col">

**Systeme de caprices**
- 3 types : service, hauteur, confort
- Chats speciaux = 2 caprices simultanes
- Non satisfait = *-20% bonheur*
- Force le joueur a reorganiser en permanence

**Combats**
- Races agressives (Chartreux, Norvegien)
- Declenchement si bonheur < 50%
- *-25 bonheur* aux deux chats
- Chaos = urgence = engagement

</div>
<div class="col">

**10 niveaux de reputation**
- De "Debutant" a "Maitre des Chats"
- Debloque races, etages, capacite
- Conditions : nb chats + bonheur min
- Penalite si reputation insuffisante

**6 etages**
- RDC gratuit → Penthouse a 25 000$
- Bonus confort et revenus par etage
- Races prestigieuses exigent les etages hauts
- Progression verticale satisfaisante

</div>
</div>

---

### Monetisation

# F2P ethique · Dual currency

<div class="cols">
<div class="col">

**Pieces ($)** — Soft currency
- Gagnee activement en jeu
- Construction, objets, reputation
- Jamais achetable en IAP
- Flux constant et satisfaisant

**Gemmes** — Hard currency
- Sources gratuites (daily, reputation, pubs)
- Achetable en IAP (0.99 a 19.99 EUR)
- Skip timers, cosmetiques, boosts
- Pas de pay-to-win

</div>
<div class="col">

**10 points de pub rewarded**
- Toutes *optionnelles*
- Doubler gains pension/adoption
- Sauver un chat en danger
- Boost revenus x2 (30 min)
- Construction instantanee
- Gemmes gratuites (1/jour)

**Abonnement Premium**
- 4.99 EUR/mois
- Construction 2x rapide
- +50% revenus hors-ligne
- 1 chat special garanti/semaine

</div>
</div>

---

<!-- _class: lead -->

### Projections de revenus

<div style="display: flex; justify-content: space-around; margin: 30px 0; text-align: center;">
<div>
<div class="big-number">10</div>
<div class="metric-label">Points de pub rewarded</div>
</div>
<div>
<div class="big-number">4</div>
<div class="metric-label">Packs IAP gemmes</div>
</div>
<div>
<div class="big-number">3</div>
<div class="metric-label">Packs ponctuels</div>
</div>
<div>
<div class="big-number">1</div>
<div class="metric-label">Abonnement premium</div>
</div>
</div>

> **100% du contenu gameplay est accessible gratuitement.**
> La monetisation accelere, elle ne bloque jamais.

---

### Direction artistique

# Cartoon 2D · Cosy · Chaleureux

- **Style :** Cartoon 2D, couleurs vives et chaleureuses
- **Perspective :** Top-down / legere isometrie
- **Inspirations :** Neko Atsume, Animal Crossing, Two Point Hospital
- **Ambiance :** Cosy, mignon sans etre enfantin, adapte au 12+

<div style="margin-top:16px;">

| Categorie | Volume |
|-----------|--------|
| Sprites de chats (8 races + 8 speciaux, 3 vues) | 48 sprites + 8 icones |
| Emotes / Bulles d'etat | 16 assets |
| Objets + icones boutique | 48 assets |
| Tiles environnement | ~16 assets |
| Effets, particules, interaction | 15 assets |
| **Total assets graphiques** | **~151 assets** |

</div>

---

### Orientation

# Portrait + Paysage

- **Paysage prioritaire** : layout de reference
- **Portrait supporte** : l'UI se reorganise dynamiquement
- Grille identique, seul le layout change
- Rotation libre, transition < 0.3s
- Safe areas respectees (notch, barre de nav)
- Zones tactiles >= 44x44 dp (accessibilite)

> Le joueur joue comme il veut, ou il veut.
> Pas de contrainte d'orientation = plus de temps de jeu.

---

### Production

# Roadmap en 3 jalons

| Jalon | Contenu | Assets | Animations |
|-------|---------|--------|------------|
| **J1 — Core** | 1 race, 9 objets, pension/refuge, caprices, combats, tap-to-collect, double orientation | ~63 | ~364 frames |
| **J2 — Contenu** | +7 races, +8 speciaux, reputation, 2 etages, 15 objets | +88 | +1910 frames |
| **J3 — Economie** | Gemmes, IAP, pubs, daily rewards, idle, FTUE, notifications | +10 | — |
| **Total** | **8 races, 24 objets, 6 etages, F2P complet** | **~161** | **~2274** |

<div style="margin-top: 16px;">

**17 ecrans UI en J1** → **28 ecrans UI au total**

</div>

---

### Calendrier

# Release : *1er Avril 2026*

<div class="cols">
<div class="col">

**Fevrier 2026 — Alpha**
- Migration Unity 6 + URP
- Assets definitifs (2-3 races)
- Animations chats
- UI mobile + double orientation
- Systeme pension/refuge
- Tap-to-collect & caresses

**Mars 2026 — Beta**
- Integration pubs rewarded
- Systeme IAP (gemmes, packs)
- Effets sonores + musique
- Equilibrage economie F2P
- Tests utilisateurs (soft launch)

</div>
<div class="col">

**1er Avril 2026 — Release**
- Build Android (Google Play)
- ASO (App Store Optimization)
- Marketing de lancement

**Q2-Q3 2026 — Live ops**
- Contenu regulier (voir slide suivante)
- Evenements saisonniers
- Analyse metriques + optimisations
- Fonctionnalites sociales

</div>
</div>

---

### Post-launch

# Contenu regulier pour la retention

<div class="cols">
<div class="col">

**Cycles mensuels**
- *Nouvelle race* ou *chat special* chaque mois
- Objets cosmetiques saisonniers
- Evenements thematiques limites (Halloween, Noel, Ete...)
- Classements temporaires

**Trimestre 2 (Q2 2026)**
- Etages 3 a 5 (Penthouse)
- Nouveaux types d'objets
- Succes / trophees
- Localisation complete (FR/EN)

</div>
<div class="col">

**Trimestre 3 (Q3 2026)**
- Systeme social (visiter les hotels d'amis)
- Evenements cooperatifs
- Races post-launch exclusives
- Battle pass saisonnier (optionnel)

**Long terme**
- Themes d'hotel (medieval, futuriste, tropical...)
- Mode histoire / campagne
- Extension iOS
- Cross-save cloud

</div>
</div>

> Objectif : *1 mise a jour majeure par mois* pour maintenir la retention J30+.

---

### KPIs cibles

# Metriques de succes

<div style="display: flex; justify-content: space-around; margin: 30px 0; text-align: center;">
<div>
<div class="big-number">40%</div>
<div class="metric-label">Retention J1</div>
</div>
<div>
<div class="big-number">15%</div>
<div class="metric-label">Retention J7</div>
</div>
<div>
<div class="big-number">5%</div>
<div class="metric-label">Retention J30</div>
</div>
<div>
<div class="big-number">3%</div>
<div class="metric-label">Taux de conversion</div>
</div>
</div>

<div style="display: flex; justify-content: space-around; margin: 10px 0; text-align: center;">
<div>
<div class="big-number" style="font-size:2em;">12 min</div>
<div class="metric-label">Session moyenne</div>
</div>
<div>
<div class="big-number" style="font-size:2em;">2.3x</div>
<div class="metric-label">Sessions / jour</div>
</div>
<div>
<div class="big-number" style="font-size:2em;">< 150 MB</div>
<div class="metric-label">Taille app</div>
</div>
</div>

---

### Stack technique

# Unity 6 · Android · URP

- **Moteur :** Unity 6 avec Universal Render Pipeline
- **Plateforme :** Android uniquement (API 24+, Android 7.0 min)
- **Resolution :** Adaptative 16:9 / 18:9 (paysage) + 9:16 / 9:18 (portrait)
- **Cible taille :** < 150 MB (telechargement initial)
- **Sauvegarde :** Locale + cloud
- **Animations :** Sprite sheet / frame-by-frame, 8-12 FPS
- **Pathfinding :** BFS + A* optimise avec cache

> Architecture pensee pour le contenu additionnel : races, objets et etages
> ajoutables via ScriptableObjects sans rebuild.

---

### USP

# Pourquoi Cat Hotel va fonctionner

<div style="font-size: 0.85em; line-height: 1.8;">

**1.** Chaque race a une *personnalite et des besoins distincts* — pas des reskins

**2.** Le systeme de *caprices* cree des defis uniques et imprevisibles

**3.** L'equilibre entre *gestion optimisee et chaos felin* = fun emergent

**4.** Les chats speciaux rares avec *noms et personnalites uniques* = collection

**5.** La double dimension *pension + refuge* = deux boucles emotionnelles

**6.** Le *tap-to-collect + caresses* = engagement actif, pas un idle oubliable

**7.** Un *contenu post-launch regulier* = retention long terme

</div>

---

<!-- _class: lead -->
<!-- _paginate: false -->

# 🐱 CAT HOTEL

### Royal Pourceau Studios

<div style="margin-top: 40px; font-size: 0.8em; color: var(--text-muted);">

**Release :** 1er Avril 2026 · Android

**Contact :** Royal Pourceau Studios

</div>

<div style="margin-top: 30px; font-size: 0.7em; color: #30363D;">

Construisez. Accueillez. Cajolez. Adoptez.

</div>

---

<!-- _class: lead -->
<!-- _paginate: false -->

<div style="font-size: 0.7em; color: var(--text-muted);">

### Annexe — Documents de reference

| Document | Contenu |
|----------|---------|
| **gdd.md** | Design complet du jeu |
| **ui-ux.md** | Ecrans, flux de navigation, economie F2P |
| **2d-art.md** | Liste des assets graphiques (~151) |
| **2d-animation.md** | Liste des animations 2D (~2274 frames) |
| **jalons.md** | Roadmap par jalons (J1/J2/J3) |

*Tous les documents sont disponibles en PDF dans le dossier `_docs/pdf/`*

</div>
