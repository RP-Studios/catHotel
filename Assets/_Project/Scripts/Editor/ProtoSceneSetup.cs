using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.U2D.Sprites;
using CatHotel.Grid;
using CatHotel.Input;
using CatHotel.Cats;
using CatHotel.UI;
using CatHotel.Core;
using CatHotel.Economy;
using CatHotel.Hotel;
using CatHotel.Services;

namespace CatHotel.Editor
{
    public static class ProtoSceneSetup
    {
        private const string SpritesRoot = "Assets/_Project/Art/Environment";
        private const string ObjectsRoot = "Assets/_Project/Art/Objects";
        private const string CatSpritesRoot = "Assets/_Project/Art/Cats/Europeen";
        private const string AnimRoot = CatSpritesRoot + "/Animations";
        private const string CatControllerPath = AnimRoot + "/CatEuropeen.controller";

        private const string Eur2SpritesRoot = "Assets/_Project/Art/Cats/Europeen2";
        private const string Eur2AnimRoot = Eur2SpritesRoot + "/Animations";
        private const string Eur2ControllerPath = Eur2AnimRoot + "/CatEuropeen2.controller";

        private const string Eur3SpritesRoot = "Assets/_Project/Art/Cats/Europeen3";
        private const string Eur3AnimRoot = Eur3SpritesRoot + "/Animations";
        private const string Eur3ControllerPath = Eur3AnimRoot + "/CatEuropeen3.controller";

        private const string SiamoisSpritesRoot = "Assets/_Project/Art/Cats/Siamois";
        private const string SiamoisAnimRoot = SiamoisSpritesRoot + "/Animations";
        private const string SiamoisControllerPath = SiamoisAnimRoot + "/CatSiamois.controller";

        private const string RagdollSpritesRoot = "Assets/_Project/Art/Cats/Ragdoll";
        private const string RagdollAnimRoot = RagdollSpritesRoot + "/Animations";
        private const string RagdollControllerPath = RagdollAnimRoot + "/CatRagdoll.controller";

        private const string CleoSpritesRoot = "Assets/_Project/Art/SpecialCats/Cleo";
        private const string CleoAnimRoot = CleoSpritesRoot + "/Animations";
        private const string CleoControllerPath = CleoSpritesRoot + "/CatCleo.controller";

        private const string AristoteSpritesRoot = "Assets/_Project/Art/SpecialCats/Aristote";
        private const string AristoteAnimRoot = AristoteSpritesRoot + "/Animations";
        private const string AristoteControllerPath = AristoteSpritesRoot + "/CatAristote.controller";

        private const string SibBlackSpritesRoot = "Assets/_Project/Art/Cats/Siberien/Black";
        private const string SibBlackAnimRoot = SibBlackSpritesRoot;
        private const string SibBlackControllerPath = SibBlackSpritesRoot + "/CatSiberienBlack.controller";

        private const string SibWhiteSpritesRoot = "Assets/_Project/Art/Cats/Siberien/White";
        private const string SibWhiteAnimRoot = SibWhiteSpritesRoot;
        private const string SibWhiteControllerPath = SibWhiteSpritesRoot + "/CatSiberienWhite.controller";

        private const string Ragdoll2SpritesRoot = "Assets/_Project/Art/Cats/Ragdoll2";
        private const string Ragdoll2AnimRoot = Ragdoll2SpritesRoot;
        private const string Ragdoll2ControllerPath = Ragdoll2SpritesRoot + "/CatRagdoll2.controller";

        private const string ChartrSpritesRoot = "Assets/_Project/Art/Cats/Chartreux";
        private const string ChartrAnimRoot = ChartrSpritesRoot + "/Animations";
        private const string ChartrControllerPath = ChartrAnimRoot + "/CatChartreux.controller";

        private const string NapoleonSpritesRoot = "Assets/_Project/Art/SpecialCats/Napoleon";
        private const string NapoleonAnimRoot = NapoleonSpritesRoot;
        private const string NapoleonControllerPath = NapoleonSpritesRoot + "/CatNapoleon.controller";

        private const string OrionSpritesRoot = "Assets/_Project/Art/SpecialCats/Orion";
        private const string OrionAnimRoot = OrionSpritesRoot + "/Animations";
        private const string OrionControllerPath = OrionSpritesRoot + "/CatOrion.controller";

        private const string CloudRoot = "Assets/_Project/Art/Effects/Combat";
        private const string CloudControllerPath = CloudRoot + "/FightCloud.controller";

        private static readonly (string file, string prefix, string state, int frames, float fps)[] CloudAnimConfigs =
        {
            ("fighting_cloud.png", "fighting_cloud", "FightCloud", 6, 6f),
        };

        private const string CoinSpinRoot = "Assets/_Project/Animations/UI";
        private const string CoinSpinControllerPath = CoinSpinRoot + "/CoinSpin.controller";

        private static readonly (string file, string prefix, string state, int frames, float fps)[] CoinSpinAnimConfigs =
        {
            ("Coin_Spin_Idle.png", "coin_spin", "CoinSpin", 12, 12f),
            ("Coin_Spawn.png", "coin_spawn", "CoinSpawn", 24, 24f),
            ("Coin_Collect.png", "coin_collect", "CoinCollect", 24, 24f),
            ("Coin_Collect_all.png", "coin_collect_all", "CoinCollectAll", 24, 24f),
        };

        private const string CarpetCosmicAnimRoot = "Assets/_Project/Art/Objects/Carpets";
        private const string CarpetCosmicControllerPath = CarpetCosmicAnimRoot + "/CarpetCosmic.controller";

        private static readonly (string file, string prefix, string state, int frames, float fps)[] CarpetCosmicAnimConfigs =
        {
            ("Anim_Carpet_Cosmic.png", "carpet_cosmic", "Idle", 4, 6f),
        };

        private const string PettingRoot = "Assets/_Project/Animations/Petting";
        private const string HandPetControllerPath = PettingRoot + "/HandPet.controller";

        private static readonly (string file, string prefix, string state, int frames, float fps)[] HandPetAnimConfigs =
        {
            ("hand_pet.png", "hand_pet", "HandPet", 13, 6.5f),
        };

        // ==================== EUROPEEN 1 ====================
        // Subdirectory-based paths (reimported spritesheets)
        private static readonly (string file, string prefix, string state, int frames, float fps)[] AnimConfigs =
        {
            // Walk (8f @256, 12 FPS)
            ("Walk/europeen_base_walk_face.png",  "eur1_walk_face",  "Walk_Front", 8, 12f),
            ("Walk/europeen_base_walk_back.png",  "eur1_walk_back",  "Walk_Back",  8, 12f),
            ("Walk/europeen_base_walk_left.png",  "eur1_walk_left",  "Walk_Left",  8, 12f),
            ("Walk/europeen_base_walk_right.png", "eur1_walk_right", "Walk_Right", 8, 12f),

            // Idle1 (6f @256, 6 FPS)
            ("Idle 1/europeen_base_idle1_face.png",  "eur1_idle1_face",  "Idle1_Front", 6, 6f),
            ("Idle 1/europeen_base_idle1_back.png",  "eur1_idle1_back",  "Idle1_Back",  6, 6f),
            ("Idle 1/europeen_base_idle1_left.png",  "eur1_idle1_left",  "Idle1_Left",  6, 6f),
            ("Idle 1/europeen_base_idle1_right.png", "eur1_idle1_right", "Idle1_Right", 6, 6f),

            // Idle2 (6f @256, 6 FPS)
            ("Idle 2/europeen_base_idle2_face.png",  "eur1_idle2_face",  "Idle2_Front", 6, 6f),
            ("Idle 2/europeen_base_idle2_left.png",  "eur1_idle2_left",  "Idle2_Left",  6, 6f),
            ("Idle 2/europeen_base_idle2_right.png", "eur1_idle2_right", "Idle2_Right", 6, 6f),

            // Idle3 (8f @256, 8 FPS)
            ("Idle 3/europeen_base_idle3_face.png",  "eur1_idle3_face",  "Idle3_Front", 8, 8f),
            ("Idle 3/europeen_base_idle3_back.png",  "eur1_idle3_back",  "Idle3_Back",  8, 8f),
            ("Idle 3/europeen_base_idle3_left.png",  "eur1_idle3_left",  "Idle3_Left",  8, 8f),
            ("Idle 3/europeen_base_idle3_right.png", "eur1_idle3_right", "Idle3_Right", 8, 8f),

            // Sleep (9f @256, 4.5 FPS)
            ("Sleep/europeen_base_sleeping_face.png",  "eur1_sleeping_face",  "Sleep_Front", 9, 4.5f),
            ("Sleep/europeen_base_sleeping_left.png",  "eur1_sleeping_left",  "Sleep_Left",  9, 4.5f),
            ("Sleep/europeen_base_sleeping_right.png", "eur1_sleeping_right", "Sleep_Right", 9, 4.5f),

            // Eat (face 12f @256 → 6 FPS, left/right 10f @512 → 5 FPS)
            ("Eating/europeen_base_eating_face.png",  "eur1_eating_face",  "Eat_Front", 12, 6f),
            ("Eating/europeen_base_eating_left.png",  "eur1_eating_left",  "Eat_Left",  10, 5f),
            ("Eating/europeen_base_eating_right.png", "eur1_eating_right", "Eat_Right", 10, 5f),

            // Drink (face 8f @256 → 4 FPS, left/right 8f @512 → 4 FPS)
            ("Drunking/europeen_base_drunking_face.png",  "eur1_drunking_face",  "Drink_Front", 8, 4f),
            ("Drunking/europeen_base_drunking_left.png",  "eur1_drunking_left",  "Drink_Left",  8, 4f),
            ("Drunking/europeen_base_drunking_right.png", "eur1_drunking_right", "Drink_Right", 8, 4f),

            // Cleaning (11f @256, 5.5 FPS)
            ("Cleaning/europeen_base_cleaning_face.png",  "eur1_cleaning_face",  "Clean_Front", 11, 5.5f),
            ("Cleaning/europeen_base_cleaning_left.png",  "eur1_cleaning_left",  "Clean_Left",  11, 5.5f),
            ("Cleaning/europeen_base_cleaning_right.png", "eur1_cleaning_right", "Clean_Right", 11, 5.5f),

            // Playing main (23f @512, 11.5 FPS) — has right
            ("Playing/europeen_base_playing_face.png",  "eur1_playing_face",  "Play_Front", 23, 11.5f),
            ("Playing/europeen_base_playing_left.png",  "eur1_playing_left",  "Play_Left",  23, 11.5f),
            ("Playing/europeen_base_playing_right.png", "eur1_playing_right", "Play_Right", 23, 11.5f),

            // Playing In (3f @512, 6 FPS)
            ("Playing/europeen_base_playing_in_face.png",  "eur1_playing_in_face",  "Play_In_Front", 3, 6f),
            ("Playing/europeen_base_playing_in_left.png",  "eur1_playing_in_left",  "Play_In_Left",  3, 6f),
            ("Playing/europeen_base_playing_in_right.png", "eur1_playing_in_right", "Play_In_Right", 3, 6f),

            // Playing Boucle (face 16f, left/right 17f @512, ~8 FPS)
            ("Playing/europeen_base_playing_boucle_face.png",  "eur1_playing_boucle_face",  "Play_Boucle_Front", 16, 8f),
            ("Playing/europeen_base_playing_boucle_left.png",  "eur1_playing_boucle_left",  "Play_Boucle_Left",  17, 8.5f),
            ("Playing/europeen_base_playing_boucle_right.png", "eur1_playing_boucle_right", "Play_Boucle_Right", 17, 8.5f),

            // Playing Out (face 4f, left/right 3f @512, 6 FPS)
            ("Playing/europeen_base_playing_out_face.png",  "eur1_playing_out_face",  "Play_Out_Front", 4, 6f),
            ("Playing/europeen_base_playing_out_left.png",  "eur1_playing_out_left",  "Play_Out_Left",  3, 6f),
            ("Playing/europeen_base_playing_out_right.png", "eur1_playing_out_right", "Play_Out_Right", 3, 6f),

            // Happy (20f @512, 10 FPS)
            ("Happy/europeen_base_happy_face.png",  "eur1_happy_face",  "Happy_Front", 20, 10f),
            ("Happy/europeen_base_happy_left.png",  "eur1_happy_left",  "Happy_Left",  20, 10f),
            ("Happy/europeen_base_happy_right.png", "eur1_happy_right", "Happy_Right", 20, 10f),

            // Unhappy (6f @512, 6 FPS)
            ("Unhappy/europeen_base_unhappy_face.png",  "eur1_unhappy_face",  "Unhappy_Front", 6, 6f),
            ("Unhappy/europeen_base_unhappy_left.png",  "eur1_unhappy_left",  "Unhappy_Left",  6, 6f),
            ("Unhappy/europeen_base_unhappy_right.png", "eur1_unhappy_right", "Unhappy_Right", 6, 6f),

            // Petting (13f @256, 6.5 FPS)
            ("Petting/europeen_base_petting_face.png", "eur1_petting_face", "Pet_Front", 13, 6.5f),

            // Fighting In (15f @512, 7.5 FPS)
            ("Fighting/europeen_fighting_in_left.png",  "eur1_fighting_in_left",  "Fight_In_Left",  15, 7.5f),
            ("Fighting/europeen_fighting_in_right.png", "eur1_fighting_in_right", "Fight_In_Right", 15, 7.5f),

            // Fighting Out (8f @512, 8 FPS)
            ("Fighting/europeen_fighting_out_left.png",  "eur1_fighting_out_left",  "Fight_Out_Left",  8, 8f),
            ("Fighting/europeen_fighting_out_right.png", "eur1_fighting_out_right", "Fight_Out_Right", 8, 8f),

            // SadWalk (8f @256, 12 FPS) — only Right direction
            ("SadWalk/europeen1_base_walk_sad_right.png", "eur1_sadwalk_right", "SadWalk_Right", 8, 12f),
        };

        // ==================== EUROPEEN 2 ====================
        // Subdirectory-based paths. Eur2 drunking_left/right are 2480x310 (odd) → 8f at 310px cells
        // Eur2 eating_left/right are 3100x310 (odd) → 10f at 310px cells
        private static readonly (string file, string prefix, string state, int frames, float fps)[] Eur2AnimConfigs =
        {
            // Walk (8f @256, 12 FPS)
            ("Walk/europeen2_base_walk_face.png",  "eur2_walk_face",  "Walk_Front", 8, 12f),
            ("Walk/europeen2_base_walk_back.png",  "eur2_walk_back",  "Walk_Back",  8, 12f),
            ("Walk/europeen2_base_walk_left.png",  "eur2_walk_left",  "Walk_Left",  8, 12f),
            ("Walk/europeen2_base_walk_right.png", "eur2_walk_right", "Walk_Right", 8, 12f),

            // Idle1 (6f @256, 6 FPS)
            ("Idle 1/europeen2_base_idle1_face.png",  "eur2_idle1_face",  "Idle1_Front", 6, 6f),
            ("Idle 1/europeen2_base_idle1_back.png",  "eur2_idle1_back",  "Idle1_Back",  6, 6f),
            ("Idle 1/europeen2_base_idle1_left.png",  "eur2_idle1_left",  "Idle1_Left",  6, 6f),
            ("Idle 1/europeen2_base_idle1_right.png", "eur2_idle1_right", "Idle1_Right", 6, 6f),

            // Idle2 (6f @256, 6 FPS)
            ("Idle 2/europeen2_base_idle2_face.png",  "eur2_idle2_face",  "Idle2_Front", 6, 6f),
            ("Idle 2/europeen2_base_idle2_left.png",  "eur2_idle2_left",  "Idle2_Left",  6, 6f),
            ("Idle 2/europeen2_base_idle2_right.png", "eur2_idle2_right", "Idle2_Right", 6, 6f),

            // Idle3 (8f @256, 8 FPS)
            ("Idle 3/europeen2_base_idle3_face.png",  "eur2_idle3_face",  "Idle3_Front", 8, 8f),
            ("Idle 3/europeen2_base_idle3_back.png",  "eur2_idle3_back",  "Idle3_Back",  8, 8f),
            ("Idle 3/europeen2_base_idle3_left.png",  "eur2_idle3_left",  "Idle3_Left",  8, 8f),
            ("Idle 3/europeen2_base_idle3_right.png", "eur2_idle3_right", "Idle3_Right", 8, 8f),

            // Sleep (9f @256, 4.5 FPS)
            ("Sleep/europeen2_base_sleeping_face.png",  "eur2_sleeping_face",  "Sleep_Front", 9, 4.5f),
            ("Sleep/europeen2_base_sleeping_left.png",  "eur2_sleeping_left",  "Sleep_Left",  9, 4.5f),
            ("Sleep/europeen2_base_sleeping_right.png", "eur2_sleeping_right", "Sleep_Right", 9, 4.5f),

            // Eat (face 12f @256, 6 FPS | left/right 10f @310, 5 FPS)
            ("Eating/europeen2_base_eating_face.png",  "eur2_eating_face",  "Eat_Front", 12, 6f),
            ("Eating/europeen2_base_eating_left.png",  "eur2_eating_left",  "Eat_Left",  10, 5f),
            ("Eating/europeen2_base_eating_right.png", "eur2_eating_right", "Eat_Right", 10, 5f),

            // Drink (face 8f @256, 4 FPS | left/right 8f @310, 4 FPS)
            ("Drunking/europeen2_base_drunking_face.png",  "eur2_drunking_face",  "Drink_Front", 8, 4f),
            ("Drunking/europeen2_base_drunking_left.png",  "eur2_drunking_left",  "Drink_Left",  8, 4f),
            ("Drunking/europeen2_base_drunking_right.png", "eur2_drunking_right", "Drink_Right", 8, 4f),

            // Cleaning (11f @256, 5.5 FPS)
            ("Cleaning/europeen2_base_cleaning_face.png",  "eur2_cleaning_face",  "Clean_Front", 11, 5.5f),
            ("Cleaning/europeen2_base_cleaning_left.png",  "eur2_cleaning_left",  "Clean_Left",  11, 5.5f),
            ("Cleaning/europeen2_base_cleaning_right.png", "eur2_cleaning_right", "Clean_Right", 11, 5.5f),

            // Playing main (23f @512, 11.5 FPS)
            ("Playing/europeen2_base_playing_face.png",  "eur2_playing_face",  "Play_Front", 23, 11.5f),
            ("Playing/europeen2_base_playing_left.png",  "eur2_playing_left",  "Play_Left",  23, 11.5f),

            // Playing In (3f @512, 6 FPS → 0.5s)
            ("Playing/europeen2_base_playing_in_face.png",  "eur2_playing_in_face",  "Play_In_Front", 3, 6f),
            ("Playing/europeen2_base_playing_in_left.png",  "eur2_playing_in_left",  "Play_In_Left",  3, 6f),
            ("Playing/europeen2_base_playing_in_right.png", "eur2_playing_in_right", "Play_In_Right", 3, 6f),

            // Playing Boucle (face 16f, left/right 17f @512, 8 FPS → 2s loop)
            ("Playing/europeen2_base_playing_boucle_face.png",  "eur2_playing_boucle_face",  "Play_Boucle_Front", 16, 8f),
            ("Playing/europeen2_base_playing_boucle_left.png",  "eur2_playing_boucle_left",  "Play_Boucle_Left",  17, 8.5f),
            ("Playing/europeen2_base_playing_boucle_right.png", "eur2_playing_boucle_right", "Play_Boucle_Right", 17, 8.5f),

            // Playing Out (face 4f, left/right 3f @512, 6 FPS)
            ("Playing/europeen2_base_playing_out_face.png",  "eur2_playing_out_face",  "Play_Out_Front", 4, 6f),
            ("Playing/europeen2_base_playing_out_left.png",  "eur2_playing_out_left",  "Play_Out_Left",  3, 6f),
            ("Playing/europeen2_base_playing_out_right.png", "eur2_playing_out_right", "Play_Out_Right", 3, 6f),

            // Happy (20f @512, 10 FPS)
            ("Happy/europeen2_base_happy_face.png",  "eur2_happy_face",  "Happy_Front", 20, 10f),
            ("Happy/europeen2_base_happy_left.png",  "eur2_happy_left",  "Happy_Left",  20, 10f),
            ("Happy/europeen2_base_happy_right.png", "eur2_happy_right", "Happy_Right", 20, 10f),

            // Unhappy (6f @512, 6 FPS)
            ("Unhappy/europeen2_base_unhappy_face.png",  "eur2_unhappy_face",  "Unhappy_Front", 6, 6f),
            ("Unhappy/europeen2_base_unhappy_left.png",  "eur2_unhappy_left",  "Unhappy_Left",  6, 6f),
            ("Unhappy/europeen2_base_unhappy_right.png", "eur2_unhappy_right", "Unhappy_Right", 6, 6f),

            // Petting (13f @256, 6.5 FPS)
            ("Petting/europeen2_base_petting_face.png", "eur2_petting_face", "Pet_Front", 13, 6.5f),

            // Fighting In (15f @512, 7.5 FPS)
            ("Fighting/europeen2_fighting_in_left.png",  "eur2_fighting_in_left",  "Fight_In_Left",  15, 7.5f),
            ("Fighting/europeen2_fighting_in_right.png", "eur2_fighting_in_right", "Fight_In_Right", 15, 7.5f),

            // Fighting Out (8f @512, 8 FPS)
            ("Fighting/europeen2_fighting_out_left.png",  "eur2_fighting_out_left",  "Fight_Out_Left",  8, 8f),
            ("Fighting/europeen2_fighting_out_right.png", "eur2_fighting_out_right", "Fight_Out_Right", 8, 8f),

            // SadWalk (8f @256, 12 FPS)
            ("SadWalk/europeen2_base_walk_sad_right.png", "eur2_sadwalk_right", "SadWalk_Right", 8, 12f),
        };

        // ==================== EUROPEEN 3 ====================
        // Note: Idle1 files are named "europeen_base_idle1_*" (no "3" suffix) — naming anomaly
        // Eur3 has playing_right (23f) unlike Eur2
        private static readonly (string file, string prefix, string state, int frames, float fps)[] Eur3AnimConfigs =
        {
            // Walk (8f @256, 12 FPS)
            ("Walk/europeen3_base_walk_face.png",  "eur3_walk_face",  "Walk_Front", 8, 12f),
            ("Walk/europeen3_base_walk_back.png",  "eur3_walk_back",  "Walk_Back",  8, 12f),
            ("Walk/europeen3_base_walk_left.png",  "eur3_walk_left",  "Walk_Left",  8, 12f),
            ("Walk/europeen3_base_walk_right.png", "eur3_walk_right", "Walk_Right", 8, 12f),

            // Idle1 (6f @256, 6 FPS) — note: files named "europeen_base_idle1_*"
            ("Idle 1/europeen_base_idle1_face.png",  "eur3_idle1_face",  "Idle1_Front", 6, 6f),
            ("Idle 1/europeen_base_idle1_back.png",  "eur3_idle1_back",  "Idle1_Back",  6, 6f),
            ("Idle 1/europeen_base_idle1_left.png",  "eur3_idle1_left",  "Idle1_Left",  6, 6f),
            ("Idle 1/europeen_base_idle1_right.png", "eur3_idle1_right", "Idle1_Right", 6, 6f),

            // Idle2 (6f @256, 6 FPS)
            ("Idle 2/europeen3_base_idle2_face.png",  "eur3_idle2_face",  "Idle2_Front", 6, 6f),
            ("Idle 2/europeen3_base_idle2_left.png",  "eur3_idle2_left",  "Idle2_Left",  6, 6f),
            ("Idle 2/europeen3_base_idle2_right.png", "eur3_idle2_right", "Idle2_Right", 6, 6f),

            // Idle3 (8f @256, 8 FPS)
            ("Idle 3/europeen3_base_idle3_face.png",  "eur3_idle3_face",  "Idle3_Front", 8, 8f),
            ("Idle 3/europeen3_base_idle3_back.png",  "eur3_idle3_back",  "Idle3_Back",  8, 8f),
            ("Idle 3/europeen3_base_idle3_left.png",  "eur3_idle3_left",  "Idle3_Left",  8, 8f),
            ("Idle 3/europeen3_base_idle3_right.png", "eur3_idle3_right", "Idle3_Right", 8, 8f),

            // Sleep (9f @256, 4.5 FPS)
            ("Sleep/europeen3_base_sleeping_face.png",  "eur3_sleeping_face",  "Sleep_Front", 9, 4.5f),
            ("Sleep/europeen3_base_sleeping_left.png",  "eur3_sleeping_left",  "Sleep_Left",  9, 4.5f),
            ("Sleep/europeen3_base_sleeping_right.png", "eur3_sleeping_right", "Sleep_Right", 9, 4.5f),

            // Eat (face 12f @256, 6 FPS | left/right 10f @512, 5 FPS)
            ("Eating/europeen3_base_eating_face.png",  "eur3_eating_face",  "Eat_Front", 12, 6f),
            ("Eating/europeen3_base_eating_left.png",  "eur3_eating_left",  "Eat_Left",  10, 5f),
            ("Eating/europeen3_base_eating_right.png", "eur3_eating_right", "Eat_Right", 10, 5f),

            // Drink (face 8f @256, 4 FPS | left/right 8f @512, 4 FPS)
            ("Drunking/europeen3_base_drunking_face.png",  "eur3_drunking_face",  "Drink_Front", 8, 4f),
            ("Drunking/europeen3_base_drunking_left.png",  "eur3_drunking_left",  "Drink_Left",  8, 4f),
            ("Drunking/europeen3_base_drunking_right.png", "eur3_drunking_right", "Drink_Right", 8, 4f),

            // Cleaning (11f @256, 5.5 FPS)
            ("Cleaning/europeen3_base_cleaning_face.png",  "eur3_cleaning_face",  "Clean_Front", 11, 5.5f),
            ("Cleaning/europeen3_base_cleaning_left.png",  "eur3_cleaning_left",  "Clean_Left",  11, 5.5f),
            ("Cleaning/europeen3_base_cleaning_right.png", "eur3_cleaning_right", "Clean_Right", 11, 5.5f),

            // Playing main (23f @512, 11.5 FPS) — Eur3 has right too
            ("Playing/europeen3_base_playing_face.png",  "eur3_playing_face",  "Play_Front", 23, 11.5f),
            ("Playing/europeen3_base_playing_left.png",  "eur3_playing_left",  "Play_Left",  23, 11.5f),
            ("Playing/europeen3_base_playing_right.png", "eur3_playing_right", "Play_Right", 23, 11.5f),

            // Playing In (3f @512, 6 FPS)
            ("Playing/europeen3_base_playing_in_face.png",  "eur3_playing_in_face",  "Play_In_Front", 3, 6f),
            ("Playing/europeen3_base_playing_in_left.png",  "eur3_playing_in_left",  "Play_In_Left",  3, 6f),
            ("Playing/europeen3_base_playing_in_right.png", "eur3_playing_in_right", "Play_In_Right", 3, 6f),

            // Playing Boucle (face 16f, left/right 17f @512, ~8 FPS)
            ("Playing/europeen3_base_playing_boucle_face.png",  "eur3_playing_boucle_face",  "Play_Boucle_Front", 16, 8f),
            ("Playing/europeen3_base_playing_boucle_left.png",  "eur3_playing_boucle_left",  "Play_Boucle_Left",  17, 8.5f),
            ("Playing/europeen3_base_playing_boucle_right.png", "eur3_playing_boucle_right", "Play_Boucle_Right", 17, 8.5f),

            // Playing Out (face 4f, left/right 3f @512, 6 FPS)
            ("Playing/europeen3_base_playing_out_face.png",  "eur3_playing_out_face",  "Play_Out_Front", 4, 6f),
            ("Playing/europeen3_base_playing_out_left.png",  "eur3_playing_out_left",  "Play_Out_Left",  3, 6f),
            ("Playing/europeen3_base_playing_out_right.png", "eur3_playing_out_right", "Play_Out_Right", 3, 6f),

            // Happy (20f @512, 10 FPS)
            ("Happy/europeen3_base_happy_face.png",  "eur3_happy_face",  "Happy_Front", 20, 10f),
            ("Happy/europeen3_base_happy_left.png",  "eur3_happy_left",  "Happy_Left",  20, 10f),
            ("Happy/europeen3_base_happy_right.png", "eur3_happy_right", "Happy_Right", 20, 10f),

            // Unhappy (6f @512, 6 FPS)
            ("Unhappy/europeen3_base_unhappy_face.png",  "eur3_unhappy_face",  "Unhappy_Front", 6, 6f),
            ("Unhappy/europeen3_base_unhappy_left.png",  "eur3_unhappy_left",  "Unhappy_Left",  6, 6f),
            ("Unhappy/europeen3_base_unhappy_right.png", "eur3_unhappy_right", "Unhappy_Right", 6, 6f),

            // Petting (13f @256, 6.5 FPS)
            ("Petting/europeen3_base_petting_face.png", "eur3_petting_face", "Pet_Front", 13, 6.5f),

            // Fighting In (15f @512, 7.5 FPS)
            ("Fighting/europeen3_fighting_in_left.png",  "eur3_fighting_in_left",  "Fight_In_Left",  15, 7.5f),
            ("Fighting/europeen3_fighting_in_right.png", "eur3_fighting_in_right", "Fight_In_Right", 15, 7.5f),

            // Fighting Out (8f @512, 8 FPS)
            ("Fighting/europeen3_fighting_out_left.png",  "eur3_fighting_out_left",  "Fight_Out_Left",  8, 8f),
            ("Fighting/europeen3_fighting_out_right.png", "eur3_fighting_out_right", "Fight_Out_Right", 8, 8f),

            // SadWalk (8f @256, 12 FPS)
            ("SadWalk/europeen3_base_walk_sad_right.png", "eur3_sadwalk_right", "SadWalk_Right", 8, 12f),
        };

        // ==================== SIAMOIS ====================
        // Same structure as Eur3, all clean 256/512 grids
        // playing_boucle all 16f, playing_out all 4f (unlike Eur2/3 variations)
        private static readonly (string file, string prefix, string state, int frames, float fps)[] SiamoisAnimConfigs =
        {
            // Walk (8f @256, 12 FPS)
            ("Walk/siamois_base_walk_face.png",  "siam_walk_face",  "Walk_Front", 8, 12f),
            ("Walk/siamois_base_walk_back.png",  "siam_walk_back",  "Walk_Back",  8, 12f),
            ("Walk/siamois_base_walk_left.png",  "siam_walk_left",  "Walk_Left",  8, 12f),
            ("Walk/siamois_base_walk_right.png", "siam_walk_right", "Walk_Right", 8, 12f),

            // Idle1 (6f @256, 6 FPS)
            ("Idle 1/siamois_base_idle1_face.png",  "siam_idle1_face",  "Idle1_Front", 6, 6f),
            ("Idle 1/siamois_base_idle1_back.png",  "siam_idle1_back",  "Idle1_Back",  6, 6f),
            ("Idle 1/siamois_base_idle1_left.png",  "siam_idle1_left",  "Idle1_Left",  6, 6f),
            ("Idle 1/siamois_base_idle1_right.png", "siam_idle1_right", "Idle1_Right", 6, 6f),

            // Idle2 (6f @256, 6 FPS)
            ("Idle 2/siamois_base_idle2_face.png",  "siam_idle2_face",  "Idle2_Front", 6, 6f),
            ("Idle 2/siamois_base_idle2_left.png",  "siam_idle2_left",  "Idle2_Left",  6, 6f),
            ("Idle 2/siamois_base_idle2_right.png", "siam_idle2_right", "Idle2_Right", 6, 6f),

            // Idle3 (8f @256, 8 FPS)
            ("Idle 3/siamois_base_idle3_face.png",  "siam_idle3_face",  "Idle3_Front", 8, 8f),
            ("Idle 3/siamois_base_idle3_back.png",  "siam_idle3_back",  "Idle3_Back",  8, 8f),
            ("Idle 3/siamois_base_idle3_left.png",  "siam_idle3_left",  "Idle3_Left",  8, 8f),
            ("Idle 3/siamois_base_idle3_right.png", "siam_idle3_right", "Idle3_Right", 8, 8f),

            // Sleep (9f @256, 4.5 FPS)
            ("Sleep/siamois_base_sleeping_face.png",  "siam_sleeping_face",  "Sleep_Front", 9, 4.5f),
            ("Sleep/siamois_base_sleeping_left.png",  "siam_sleeping_left",  "Sleep_Left",  9, 4.5f),
            ("Sleep/siamois_base_sleeping_right.png", "siam_sleeping_right", "Sleep_Right", 9, 4.5f),

            // Eat (face 12f @256, 6 FPS | left/right 10f @512, 5 FPS)
            ("Eating/siamois_base_eating_face.png",  "siam_eating_face",  "Eat_Front", 12, 6f),
            ("Eating/siamois_base_eating_left.png",  "siam_eating_left",  "Eat_Left",  10, 5f),
            ("Eating/siamois_base_eating_right.png", "siam_eating_right", "Eat_Right", 10, 5f),

            // Drink (face 8f @256, 4 FPS | left/right 8f @512, 4 FPS)
            ("Drunking/siamois_base_drunking_face.png",  "siam_drunking_face",  "Drink_Front", 8, 4f),
            ("Drunking/siamois_base_drunking_left.png",  "siam_drunking_left",  "Drink_Left",  8, 4f),
            ("Drunking/siamois_base_drunking_right.png", "siam_drunking_right", "Drink_Right", 8, 4f),

            // Cleaning (11f @256, 5.5 FPS)
            ("Cleaning/siamois_base_cleaning_face.png",  "siam_cleaning_face",  "Clean_Front", 11, 5.5f),
            ("Cleaning/siamois_base_cleaning_left.png",  "siam_cleaning_left",  "Clean_Left",  11, 5.5f),
            ("Cleaning/siamois_base_cleaning_right.png", "siam_cleaning_right", "Clean_Right", 11, 5.5f),

            // Playing main (23f @512, 11.5 FPS) — Siamois has right too
            ("Playing/siamois_base_playing_face.png",  "siam_playing_face",  "Play_Front", 23, 11.5f),
            ("Playing/siamois_base_playing_left.png",  "siam_playing_left",  "Play_Left",  23, 11.5f),
            ("Playing/siamois_base_playing_right.png", "siam_playing_right", "Play_Right", 23, 11.5f),

            // Playing In (3f @512, 6 FPS)
            ("Playing/siamois_base_playing_in_face.png",  "siam_playing_in_face",  "Play_In_Front", 3, 6f),
            ("Playing/siamois_base_playing_in_left.png",  "siam_playing_in_left",  "Play_In_Left",  3, 6f),
            ("Playing/siamois_base_playing_in_right.png", "siam_playing_in_right", "Play_In_Right", 3, 6f),

            // Playing Boucle (all 16f @512, 8 FPS)
            ("Playing/siamois_base_playing_boucle_face.png",  "siam_playing_boucle_face",  "Play_Boucle_Front", 16, 8f),
            ("Playing/siamois_base_playing_boucle_left.png",  "siam_playing_boucle_left",  "Play_Boucle_Left",  16, 8f),
            ("Playing/siamois_base_playing_boucle_right.png", "siam_playing_boucle_right", "Play_Boucle_Right", 16, 8f),

            // Playing Out (all 4f @512, 6 FPS)
            ("Playing/siamois_base_playing_out_face.png",  "siam_playing_out_face",  "Play_Out_Front", 4, 6f),
            ("Playing/siamois_base_playing_out_left.png",  "siam_playing_out_left",  "Play_Out_Left",  4, 6f),
            ("Playing/siamois_base_playing_out_right.png", "siam_playing_out_right", "Play_Out_Right", 4, 6f),

            // Happy (20f @512, 10 FPS)
            ("Happy/siamois_base_happy_face.png",  "siam_happy_face",  "Happy_Front", 20, 10f),
            ("Happy/siamois_base_happy_left.png",  "siam_happy_left",  "Happy_Left",  20, 10f),
            ("Happy/siamois_base_happy_right.png", "siam_happy_right", "Happy_Right", 20, 10f),

            // Unhappy (6f @512, 6 FPS)
            ("Unhappy/siamois_base_unhappy_face.png",  "siam_unhappy_face",  "Unhappy_Front", 6, 6f),
            ("Unhappy/siamois_base_unhappy_left.png",  "siam_unhappy_left",  "Unhappy_Left",  6, 6f),
            ("Unhappy/siamois_base_unhappy_right.png", "siam_unhappy_right", "Unhappy_Right", 6, 6f),

            // Petting (13f @256, 6.5 FPS)
            ("Petting/siamois_base_petting_face.png", "siam_petting_face", "Pet_Front", 13, 6.5f),

            // Fighting In (15f @512, 7.5 FPS)
            ("Fighting/siamois_fighting_in_left.png",  "siam_fighting_in_left",  "Fight_In_Left",  15, 7.5f),
            ("Fighting/siamois_fighting_in_right.png", "siam_fighting_in_right", "Fight_In_Right", 15, 7.5f),

            // Fighting Out (8f @512, 8 FPS)
            ("Fighting/siamois_fighting_out_left.png",  "siam_fighting_out_left",  "Fight_Out_Left",  8, 8f),
            ("Fighting/siamois_fighting_out_right.png", "siam_fighting_out_right", "Fight_Out_Right", 8, 8f),

            // SadWalk (8f @256, 12 FPS)
            ("SadWalk/siamois_base_walk_sad_right.png", "siam_sadwalk_right", "SadWalk_Right", 8, 12f),
        };

        // ==================== RAGDOLL ====================
        private static readonly (string file, string prefix, string state, int frames, float fps)[] RagdollAnimConfigs =
        {
            // Walk (8f, 12 FPS)
            ("Walk/ragdoll_base_walk_face.png",  "rd_walk_face",  "Walk_Front", 8, 12f),
            ("Walk/ragdoll_base_walk_back.png",  "rd_walk_back",  "Walk_Back",  8, 12f),
            ("Walk/ragdoll_base_walk_left.png",  "rd_walk_left",  "Walk_Left",  8, 12f),
            ("Walk/ragdoll_base_walk_right.png", "rd_walk_right", "Walk_Right", 8, 12f),

            // Idle1 (6f, 6 FPS)
            ("Idle 1/ragdoll_base_idle1_face.png",  "rd_idle1_face",  "Idle1_Front", 6, 6f),
            ("Idle 1/ragdoll_base_idle1_back.png",  "rd_idle1_back",  "Idle1_Back",  6, 6f),
            ("Idle 1/ragdoll_base_idle1_left.png",  "rd_idle1_left",  "Idle1_Left",  6, 6f),
            ("Idle 1/ragdoll_base_idle1_right.png", "rd_idle1_right", "Idle1_Right", 6, 6f),

            // Idle2 (6f, 6 FPS)
            ("Idle 2/ragdoll_base_idle2_face.png",  "rd_idle2_face",  "Idle2_Front", 6, 6f),
            ("Idle 2/ragdoll_base_idle2_left.png",  "rd_idle2_left",  "Idle2_Left",  6, 6f),
            ("Idle 2/ragdoll_base_idle2_right.png", "rd_idle2_right", "Idle2_Right", 6, 6f),

            // Idle3 (8f, 8 FPS)
            ("Idle 3/ragdoll_base_idle3_face.png",  "rd_idle3_face",  "Idle3_Front", 8, 8f),
            ("Idle 3/ragdoll_base_idle3_back.png",  "rd_idle3_back",  "Idle3_Back",  8, 8f),
            ("Idle 3/ragdoll_base_idle3_left.png",  "rd_idle3_left",  "Idle3_Left",  8, 8f),
            ("Idle 3/ragdoll_base_idle3_right.png", "rd_idle3_right", "Idle3_Right", 8, 8f),

            // Sleep (9f, 4.5 FPS)
            ("Sleep/ragdoll_base_sleeping_face.png",  "rd_sleeping_face",  "Sleep_Front", 9, 4.5f),
            ("Sleep/ragdoll_base_sleeping_left.png",  "rd_sleeping_left",  "Sleep_Left",  9, 4.5f),
            ("Sleep/ragdoll_base_sleeping_right.png", "rd_sleeping_right", "Sleep_Right", 9, 4.5f),

            // Eat (face 12f, 6 FPS | left/right 12f, 6 FPS — typo in filenames: ragodoll)
            ("Eating/ragdoll_base_eating_face.png",   "rd_eating_face",  "Eat_Front", 12, 6f),
            ("Eating/ragodoll_base_eating_left.png",  "rd_eating_left",  "Eat_Left",  12, 6f),
            ("Eating/ragodoll_base_eating_right.png", "rd_eating_right", "Eat_Right", 12, 6f),

            // Drink (8f, 4 FPS — typo in filenames: ragodoll)
            ("Drinking/ragdoll_base_drinking_face.png",   "rd_drinking_face",  "Drink_Front", 8, 4f),
            ("Drinking/ragodoll_base_drinking_left.png",  "rd_drinking_left",  "Drink_Left",  8, 4f),
            ("Drinking/ragodoll_base_drinking_right.png", "rd_drinking_right", "Drink_Right", 8, 4f),

            // Cleaning (face 13f @6.5 FPS, left/right 12f @6 FPS)
            ("Cleaning/ragdoll_base_cleaning_face.png",  "rd_cleaning_face",  "Clean_Front", 13, 6.5f),
            ("Cleaning/ragdoll_base_cleaning_left.png",  "rd_cleaning_left",  "Clean_Left",  12, 6f),
            ("Cleaning/ragdoll_base_cleaning_right.png", "rd_cleaning_right", "Clean_Right", 12, 6f),

            // Playing main (23f, 11.5 FPS)
            ("Playing/ragdoll_base_playing_face.png",  "rd_playing_face",  "Play_Front", 23, 11.5f),
            ("Playing/ragdoll_base_playing_left.png",  "rd_playing_left",  "Play_Left",  23, 11.5f),
            ("Playing/ragdoll_base_playing_right.png", "rd_playing_right", "Play_Right", 23, 11.5f),

            // Happy (20f, 10 FPS)
            ("Happy/ragdoll_base_happy_face.png",  "rd_happy_face",  "Happy_Front", 20, 10f),
            ("Happy/ragdoll_base_happy_left.png",  "rd_happy_left",  "Happy_Left",  20, 10f),
            ("Happy/ragdoll_base_happy_right.png", "rd_happy_right", "Happy_Right", 20, 10f),

            // Unhappy (6f, 6 FPS)
            ("Unhappy/ragdoll_base_unhappy_face.png",  "rd_unhappy_face",  "Unhappy_Front", 6, 6f),
            ("Unhappy/ragdoll_base_unhappy_left.png",  "rd_unhappy_left",  "Unhappy_Left",  6, 6f),
            ("Unhappy/ragdoll_base_unhappy_right.png", "rd_unhappy_right", "Unhappy_Right", 6, 6f),

            // Petting (12f @256, 6 FPS)
            ("Petting/ragdoll_base_petting_face.png", "rd_petting_face", "Pet_Front", 12, 6f),

            // Fighting In (15f, 7.5 FPS)
            ("Fighting/ragdoll_base_fighting_in_left.png",  "rd_fighting_in_left",  "Fight_In_Left",  15, 7.5f),
            ("Fighting/ragdoll_base_fighting_in_right.png", "rd_fighting_in_right", "Fight_In_Right", 15, 7.5f),

            // Fighting Out (8f, 8 FPS)
            ("Fighting/ragdoll_base_fighting_out_left.png",  "rd_fighting_out_left",  "Fight_Out_Left",  8, 8f),
            ("Fighting/ragdoll_base_fighting_out_right.png", "rd_fighting_out_right", "Fight_Out_Right", 8, 8f),

            // SadWalk (8f, 12 FPS)
            ("Walk Sad/ragdoll_base_walk_sad_right.png", "rd_sadwalk_right", "SadWalk_Right", 8, 12f),
        };

        // ==================== RAGDOLL 2 ====================
        // Same frame pattern as Ragdoll 1 (cleaning 13/12/12, eating 12, petting 12) + Scratch
        // Note typos: ragodoll_2 for drinking/eating left/right
        private static readonly (string file, string prefix, string state, int frames, float fps)[] Ragdoll2AnimConfigs =
        {
            ("Walk/ragdoll_2_walk_face.png",  "rd2_walk_face",  "Walk_Front", 8, 12f),
            ("Walk/ragdoll_2_walk_back.png",  "rd2_walk_back",  "Walk_Back",  8, 12f),
            ("Walk/ragdoll_2_walk_left.png",  "rd2_walk_left",  "Walk_Left",  8, 12f),
            ("Walk/ragdoll_2_walk_right.png", "rd2_walk_right", "Walk_Right", 8, 12f),
            ("Idle 1/ragdoll_2_idle1_face.png",  "rd2_idle1_face",  "Idle1_Front", 6, 6f),
            ("Idle 1/ragdoll_2_idle1_back.png",  "rd2_idle1_back",  "Idle1_Back",  6, 6f),
            ("Idle 1/ragdoll_2_idle1_left.png",  "rd2_idle1_left",  "Idle1_Left",  6, 6f),
            ("Idle 1/ragdoll_2_idle1_right.png", "rd2_idle1_right", "Idle1_Right", 6, 6f),
            ("Idle 2/ragdoll_2_idle2_face.png",  "rd2_idle2_face",  "Idle2_Front", 6, 6f),
            ("Idle 2/ragdoll_2_idle2_left.png",  "rd2_idle2_left",  "Idle2_Left",  6, 6f),
            ("Idle 2/ragdoll_2_idle2_right.png", "rd2_idle2_right", "Idle2_Right", 6, 6f),
            ("Idle 3/ragdoll_2_idle3_face.png",  "rd2_idle3_face",  "Idle3_Front", 8, 8f),
            ("Idle 3/ragdoll_2_idle3_back.png",  "rd2_idle3_back",  "Idle3_Back",  8, 8f),
            ("Idle 3/ragdoll_2_idle3_left.png",  "rd2_idle3_left",  "Idle3_Left",  8, 8f),
            ("Idle 3/ragdoll_2_idle3_right.png", "rd2_idle3_right", "Idle3_Right", 8, 8f),
            ("Sleep/ragdoll_2_sleeping_face.png",  "rd2_sleeping_face",  "Sleep_Front", 9, 4.5f),
            ("Sleep/ragdoll_2_sleeping_left.png",  "rd2_sleeping_left",  "Sleep_Left",  9, 4.5f),
            ("Sleep/ragdoll_2_sleeping_right.png", "rd2_sleeping_right", "Sleep_Right", 9, 4.5f),
            ("Eating/ragdoll_2_eating_face.png",   "rd2_eating_face",  "Eat_Front", 12, 6f),
            ("Eating/ragodoll_2_eating_left.png",  "rd2_eating_left",  "Eat_Left",  12, 6f),
            ("Eating/ragodoll_2_eating_right.png", "rd2_eating_right", "Eat_Right", 12, 6f),
            ("Drinking/ragdoll_2_drinking_face.png",   "rd2_drinking_face",  "Drink_Front", 8, 4f),
            ("Drinking/ragodoll_2_drinking_left.png",  "rd2_drinking_left",  "Drink_Left",  8, 4f),
            ("Drinking/ragodoll_2_drinking_right.png", "rd2_drinking_right", "Drink_Right", 8, 4f),
            ("Cleaning/ragdoll_2_cleaning_face.png",  "rd2_cleaning_face",  "Clean_Front", 13, 6.5f),
            ("Cleaning/ragdoll_2_cleaning_left.png",  "rd2_cleaning_left",  "Clean_Left",  12, 6f),
            ("Cleaning/ragdoll_2_cleaning_right.png", "rd2_cleaning_right", "Clean_Right", 12, 6f),
            ("Playing/ragdoll_2_playing_face.png",  "rd2_playing_face",  "Play_Front", 23, 11.5f),
            ("Playing/ragdoll_2_playing_left.png",  "rd2_playing_left",  "Play_Left",  23, 11.5f),
            ("Playing/ragdoll_2_playing_right.png", "rd2_playing_right", "Play_Right", 23, 11.5f),
            ("Happy/ragdoll_2_happy_face.png",  "rd2_happy_face",  "Happy_Front", 20, 10f),
            ("Happy/ragdoll_2_happy_left.png",  "rd2_happy_left",  "Happy_Left",  20, 10f),
            ("Happy/ragdoll_2_happy_right.png", "rd2_happy_right", "Happy_Right", 20, 10f),
            ("Unhappy/ragdoll_2_unhappy_face.png",  "rd2_unhappy_face",  "Unhappy_Front", 6, 6f),
            ("Unhappy/ragdoll_2_unhappy_left.png",  "rd2_unhappy_left",  "Unhappy_Left",  6, 6f),
            ("Unhappy/ragdoll_2_unhappy_right.png", "rd2_unhappy_right", "Unhappy_Right", 6, 6f),
            ("Petting/ragdoll_2_petting_face.png", "rd2_petting_face", "Pet_Front", 12, 6f),
            ("Fighting/ragdoll_2_fighting_in_left.png",  "rd2_fighting_in_left",  "Fight_In_Left",  15, 7.5f),
            ("Fighting/ragdoll_2_fighting_in_right.png", "rd2_fighting_in_right", "Fight_In_Right", 15, 7.5f),
            ("Fighting/ragdoll_2_fighting_out_left.png",  "rd2_fighting_out_left",  "Fight_Out_Left",  8, 8f),
            ("Fighting/ragdoll_2_fighting_out_right.png", "rd2_fighting_out_right", "Fight_Out_Right", 8, 8f),
            ("Walk Sad/ragdoll_2_walk_sad_right.png", "rd2_sadwalk_right", "SadWalk_Right", 8, 12f),
            ("Scratch/In/Ragdoll_2_Scratch_In_Left.png",  "rd2_scratch_in_left",  "Scratch_In_Left",  7, 14f),
            ("Scratch/In/Ragdoll_2_Scratch_In_Right.png", "rd2_scratch_in_right", "Scratch_In_Right", 7, 14f),
            ("Scratch/Boucle/Ragdoll_2_Scratch_Left.png",  "rd2_scratch_boucle_left",  "Scratch_Boucle_Left",  11, 5.5f),
            ("Scratch/Boucle/Ragdoll_2_Scratch_Right.png", "rd2_scratch_boucle_right", "Scratch_Boucle_Right", 11, 5.5f),
            ("Scratch/Out/Ragdoll_2_Scratch_Out_Left.png",  "rd2_scratch_out_left",  "Scratch_Out_Left",  7, 14f),
            ("Scratch/Out/Ragdoll_2_Scratch_Out_Right.png", "rd2_scratch_out_right", "Scratch_Out_Right", 7, 14f),
        };

        // ==================== SIBERIAN BLACK ====================
        // Note: Siberian has richer animations (more frames) than other breeds
        // Also has Scratch animation (unique to this breed)
        // Walk filenames have typos: "sibeiren" (right), "siberienl" (left) — must match actual files
        private static readonly (string file, string prefix, string state, int frames, float fps)[] SibBlackAnimConfigs =
        {
            // Walk (8f, 12 FPS) — note filename typos in originals
            ("Walk/siberien_black_walk_face.png",   "sibb_walk_face",  "Walk_Front", 8, 12f),
            ("Walk/siberien_black_walk_back.png",   "sibb_walk_back",  "Walk_Back",  8, 12f),
            ("Walk/siberienl_black_walk_left.png",  "sibb_walk_left",  "Walk_Left",  8, 12f),
            ("Walk/sibeiren_black_walk_right.png",  "sibb_walk_right", "Walk_Right", 8, 12f),

            // Idle1 (6f, 6 FPS)
            ("Idle 1/siberien_black_idle1_face.png",  "sibb_idle1_face",  "Idle1_Front", 6, 6f),
            ("Idle 1/siberien_black_idle1_back.png",  "sibb_idle1_back",  "Idle1_Back",  6, 6f),
            ("Idle 1/siberien_black_idle1_left.png",  "sibb_idle1_left",  "Idle1_Left",  6, 6f),
            ("Idle 1/siberien_black_idle1_right.png", "sibb_idle1_right", "Idle1_Right", 6, 6f),

            // Idle2 (6f, 6 FPS) — no back
            ("Idle 2/siberien_black_idle2_face.png",  "sibb_idle2_face",  "Idle2_Front", 6, 6f),
            ("Idle 2/siberien_black_idle2_left.png",  "sibb_idle2_left",  "Idle2_Left",  6, 6f),
            ("Idle 2/siberien_black_idle2_right.png", "sibb_idle2_right", "Idle2_Right", 6, 6f),

            // Idle3 (8f, 8 FPS)
            ("Idle 3/siberien_black_idle3_face.png",  "sibb_idle3_face",  "Idle3_Front", 8, 8f),
            ("Idle 3/siberien_black_idle3_back.png",  "sibb_idle3_back",  "Idle3_Back",  8, 8f),
            ("Idle 3/siberien_black_idle3_left.png",  "sibb_idle3_left",  "Idle3_Left",  8, 8f),
            ("Idle 3/siberien_black_idle3_right.png", "sibb_idle3_right", "Idle3_Right", 8, 8f),

            // Sleep (9f @256, 4.5 FPS)
            ("Sleep/siberien_black_sleeping_face.png",  "sibb_sleeping_face",  "Sleep_Front", 9, 4.5f),
            ("Sleep/siberien_black_sleeping_left.png",  "sibb_sleeping_left",  "Sleep_Left",  9, 4.5f),
            ("Sleep/siberien_black_sleeping_right.png", "sibb_sleeping_right", "Sleep_Right", 9, 4.5f),

            // Eat (12f, 6 FPS)
            ("Eating/siberien_black_eating_face.png",  "sibb_eating_face",  "Eat_Front", 12, 6f),
            ("Eating/siberien_black_eating_left.png",  "sibb_eating_left",  "Eat_Left",  12, 6f),
            ("Eating/siberien_black_eating_right.png", "sibb_eating_right", "Eat_Right", 12, 6f),

            // Drink (8f, 4 FPS)
            ("Drinking/siberien_black_drunking_face.png",  "sibb_drunking_face",  "Drink_Front", 8, 4f),
            ("Drinking/siberien_black_drunking_left.png",  "sibb_drunking_left",  "Drink_Left",  8, 4f),
            ("Drinking/siberien_black_drunking_right.png", "sibb_drunking_right", "Drink_Right", 8, 4f),

            // Cleaning (face 13f @256 6.5 FPS, left/right 12f @256 6 FPS)
            ("Cleaning/siberien_black_cleaning_face.png",  "sibb_cleaning_face",  "Clean_Front", 13, 6.5f),
            ("Cleaning/siberien_black_cleaning_left.png",  "sibb_cleaning_left",  "Clean_Left",  12, 6f),
            ("Cleaning/siberien_black_cleaning_right.png", "sibb_cleaning_right", "Clean_Right", 12, 6f),

            // Playing main (23f @512, 11.5 FPS)
            ("Playing/siberien_black_playing_face.png",  "sibb_playing_face",  "Play_Front", 23, 11.5f),
            ("Playing/siberien_black_playing_left.png",  "sibb_playing_left",  "Play_Left",  23, 11.5f),
            ("Playing/siberien_black_playing_right.png", "sibb_playing_right", "Play_Right", 23, 11.5f),

            // Happy (20f @512, 10 FPS)
            ("Happy/siberien_black_happy_face.png",  "sibb_happy_face",  "Happy_Front", 20, 10f),
            ("Happy/siberien_black_happy_left.png",  "sibb_happy_left",  "Happy_Left",  20, 10f),
            ("Happy/siberien_black_happy_right.png", "sibb_happy_right", "Happy_Right", 20, 10f),

            // Unhappy (6f @512, 6 FPS)
            ("Unhappy/siberien_black_unhappy_face.png",  "sibb_unhappy_face",  "Unhappy_Front", 6, 6f),
            ("Unhappy/siberien_black_unhappy_left.png",  "sibb_unhappy_left",  "Unhappy_Left",  6, 6f),
            ("Unhappy/siberien_black_unhappy_right.png", "sibb_unhappy_right", "Unhappy_Right", 6, 6f),

            // Petting (12f @256, 6 FPS)
            ("Petting/siberien_black_petting_face.png", "sibb_petting_face", "Pet_Front", 12, 6f),

            // Fighting In (15f @512, 7.5 FPS)
            ("Fighting/siberien_black_fighting_in_left.png",  "sibb_fighting_in_left",  "Fight_In_Left",  15, 7.5f),
            ("Fighting/siberien_black_fighting_in_right.png", "sibb_fighting_in_right", "Fight_In_Right", 15, 7.5f),

            // Fighting Out (8f @512, 8 FPS)
            ("Fighting/siberien_black_fighting_out_left.png",  "sibb_fighting_out_left",  "Fight_Out_Left",  8, 8f),
            ("Fighting/siberien_black_fighting_out_right.png", "sibb_fighting_out_right", "Fight_Out_Right", 8, 8f),

            // SadWalk (8f, 12 FPS)
            ("Walk Sad/siberien_black_walk_sad_right.png", "sibb_sadwalk_right", "SadWalk_Right", 8, 12f),

            // Scratch In (7f, 14 FPS → 0.5s)
            ("Scratch/In/siberien_black_Scratch_In_left.png",  "sibb_scratch_in_left",  "Scratch_In_Left",  7, 14f),
            ("Scratch/In/siberien_black_Scratch_in_right.png", "sibb_scratch_in_right", "Scratch_In_Right", 7, 14f),

            // Scratch Boucle (11f, 5.5 FPS → 2s loop)
            ("Scratch/Boucle/siberien_Black_Scratch_Left.png",  "sibb_scratch_boucle_left",  "Scratch_Boucle_Left",  11, 5.5f),
            ("Scratch/Boucle/siberien_Black_Scratch_Right.png", "sibb_scratch_boucle_right", "Scratch_Boucle_Right", 11, 5.5f),

            // Scratch Out (7f, 14 FPS → 0.5s)
            ("Scratch/Out/siberien_black_Scratch_Out_Left.png",  "sibb_scratch_out_left",  "Scratch_Out_Left",  7, 14f),
            ("Scratch/Out/siberien_Black_Scratch_Out_Right.png", "sibb_scratch_out_right", "Scratch_Out_Right", 7, 14f),
        };

        // ==================== SIBERIAN WHITE ====================
        // Same structure as Black, different sprites. Filenames follow same pattern with "white" instead of "black"
        private static readonly (string file, string prefix, string state, int frames, float fps)[] SibWhiteAnimConfigs =
        {
            // Walk (8f, 12 FPS) — same filename typos
            ("Walk/siberien_white_walk_face.png",   "sibw_walk_face",  "Walk_Front", 8, 12f),
            ("Walk/siberien_white_walk_back.png",   "sibw_walk_back",  "Walk_Back",  8, 12f),
            ("Walk/siberienl_white_walk_left.png",  "sibw_walk_left",  "Walk_Left",  8, 12f),
            ("Walk/sibeiren_white_walk_right.png",  "sibw_walk_right", "Walk_Right", 8, 12f),

            // Idle1 (6f, 6 FPS)
            ("Idle 1/siberien_white_idle1_face.png",  "sibw_idle1_face",  "Idle1_Front", 6, 6f),
            ("Idle 1/siberien_white_idle1_back.png",  "sibw_idle1_back",  "Idle1_Back",  6, 6f),
            ("Idle 1/siberien_white_idle1_left.png",  "sibw_idle1_left",  "Idle1_Left",  6, 6f),
            ("Idle 1/siberien_white_idle1_right.png", "sibw_idle1_right", "Idle1_Right", 6, 6f),

            // Idle2 (6f, 6 FPS) — no back
            ("Idle 2/siberien_white_idle2_face.png",  "sibw_idle2_face",  "Idle2_Front", 6, 6f),
            ("Idle 2/siberien_white_idle2_left.png",  "sibw_idle2_left",  "Idle2_Left",  6, 6f),
            ("Idle 2/siberien_white_idle2_right.png", "sibw_idle2_right", "Idle2_Right", 6, 6f),

            // Idle3 (8f, 8 FPS)
            ("Idle 3/siberien_white_idle3_face.png",  "sibw_idle3_face",  "Idle3_Front", 8, 8f),
            ("Idle 3/siberien_white_idle3_back.png",  "sibw_idle3_back",  "Idle3_Back",  8, 8f),
            ("Idle 3/siberien_white_idle3_left.png",  "sibw_idle3_left",  "Idle3_Left",  8, 8f),
            ("Idle 3/siberien_white_idle3_right.png", "sibw_idle3_right", "Idle3_Right", 8, 8f),

            // Sleep (9f @256, 4.5 FPS)
            ("Sleep/siberien_white_sleeping_face.png",  "sibw_sleeping_face",  "Sleep_Front", 9, 4.5f),
            ("Sleep/siberien_white_sleeping_left.png",  "sibw_sleeping_left",  "Sleep_Left",  9, 4.5f),
            ("Sleep/siberien_white_sleeping_right.png", "sibw_sleeping_right", "Sleep_Right", 9, 4.5f),

            // Eat (12f, 6 FPS)
            ("Eating/siberien_white_eating_face.png",  "sibw_eating_face",  "Eat_Front", 12, 6f),
            ("Eating/siberien_white_eating_left.png",  "sibw_eating_left",  "Eat_Left",  12, 6f),
            ("Eating/siberien_white_eating_right.png", "sibw_eating_right", "Eat_Right", 12, 6f),

            // Drink (8f, 4 FPS)
            ("Drinking/siberien_white_drunking_face.png",  "sibw_drunking_face",  "Drink_Front", 8, 4f),
            ("Drinking/siberien_white_drunking_left.png",  "sibw_drunking_left",  "Drink_Left",  8, 4f),
            ("Drinking/siberien_white_drunking_right.png", "sibw_drunking_right", "Drink_Right", 8, 4f),

            // Cleaning (face 13f @256 6.5 FPS, left/right 12f @256 6 FPS)
            ("Cleaning/siberien_white_cleaning_face.png",  "sibw_cleaning_face",  "Clean_Front", 13, 6.5f),
            ("Cleaning/siberien_white_cleaning_left.png",  "sibw_cleaning_left",  "Clean_Left",  12, 6f),
            ("Cleaning/siberien_white_cleaning_right.png", "sibw_cleaning_right", "Clean_Right", 12, 6f),

            // Playing main (23f @512, 11.5 FPS)
            ("Playing/siberien_white_playing_face.png",  "sibw_playing_face",  "Play_Front", 23, 11.5f),
            ("Playing/siberien_white_playing_left.png",  "sibw_playing_left",  "Play_Left",  23, 11.5f),
            ("Playing/siberien_white_playing_right.png", "sibw_playing_right", "Play_Right", 23, 11.5f),

            // Happy (20f @512, 10 FPS)
            ("Happy/siberien_white_happy_face.png",  "sibw_happy_face",  "Happy_Front", 20, 10f),
            ("Happy/siberien_white_happy_left.png",  "sibw_happy_left",  "Happy_Left",  20, 10f),
            ("Happy/siberien_white_happy_right.png", "sibw_happy_right", "Happy_Right", 20, 10f),

            // Unhappy (6f @512, 6 FPS)
            ("Unhappy/siberien_white_unhappy_face.png",  "sibw_unhappy_face",  "Unhappy_Front", 6, 6f),
            ("Unhappy/siberien_white_unhappy_left.png",  "sibw_unhappy_left",  "Unhappy_Left",  6, 6f),
            ("Unhappy/siberien_white_unhappy_right.png", "sibw_unhappy_right", "Unhappy_Right", 6, 6f),

            // Petting (12f @256, 6 FPS)
            ("Petting/siberien_white_petting_face.png", "sibw_petting_face", "Pet_Front", 12, 6f),

            // Fighting In (15f @512, 7.5 FPS)
            ("Fighting/siberien_white_fighting_in_left.png",  "sibw_fighting_in_left",  "Fight_In_Left",  15, 7.5f),
            ("Fighting/siberien_white_fighting_in_right.png", "sibw_fighting_in_right", "Fight_In_Right", 15, 7.5f),

            // Fighting Out (8f @512, 8 FPS)
            ("Fighting/siberien_white_fighting_out_left.png",  "sibw_fighting_out_left",  "Fight_Out_Left",  8, 8f),
            ("Fighting/siberien_white_fighting_out_right.png", "sibw_fighting_out_right", "Fight_Out_Right", 8, 8f),

            // SadWalk (8f, 12 FPS)
            ("Walk Sad/siberien_white_walk_sad_right.png", "sibw_sadwalk_right", "SadWalk_Right", 8, 12f),

            // Scratch In (7f, 14 FPS → 0.5s)
            ("Scratch/In/siberien_white_Scratch_In_left.png",  "sibw_scratch_in_left",  "Scratch_In_Left",  7, 14f),
            ("Scratch/In/siberien_white_Scratch_in_right.png", "sibw_scratch_in_right", "Scratch_In_Right", 7, 14f),

            // Scratch Boucle (11f, 5.5 FPS → 2s loop)
            ("Scratch/Boucle/siberien_white_Scratch_Left.png",  "sibw_scratch_boucle_left",  "Scratch_Boucle_Left",  11, 5.5f),
            ("Scratch/Boucle/siberien_white_Scratch_Right.png", "sibw_scratch_boucle_right", "Scratch_Boucle_Right", 11, 5.5f),

            // Scratch Out (7f, 14 FPS → 0.5s)
            ("Scratch/Out/siberien_white_Scratch_Out_Left.png",  "sibw_scratch_out_left",  "Scratch_Out_Left",  7, 14f),
            ("Scratch/Out/siberien_white_Scratch_Out_Right.png", "sibw_scratch_out_right", "Scratch_Out_Right", 7, 14f),
        };

        // ==================== CLEO (Special Cat) ====================
        private static readonly (string file, string prefix, string state, int frames, float fps)[] CleoAnimConfigs =
        {
            // Walk (8f @256, 12 FPS)
            ("Walk/siamois_cleo_walk_face.png",  "cleo_walk_face",  "Walk_Front", 8, 12f),
            ("Walk/siamois_cleo_walk_back.png",  "cleo_walk_back",  "Walk_Back",  8, 12f),
            ("Walk/siamois_cleo_walk_left.png",  "cleo_walk_left",  "Walk_Left",  8, 12f),
            ("Walk/siamois_cleo_walk_right.png", "cleo_walk_right", "Walk_Right", 8, 12f),

            // Idle1 (6f @256, 6 FPS)
            ("Idle 1/siamois_cleo_idle1_face.png",  "cleo_idle1_face",  "Idle1_Front", 6, 6f),
            ("Idle 1/siamois_cleo_idle1_back.png",  "cleo_idle1_back",  "Idle1_Back",  6, 6f),
            ("Idle 1/siamois_cleo_idle1_left.png",  "cleo_idle1_left",  "Idle1_Left",  6, 6f),
            ("Idle 1/siamois_cleo_idle1_right.png", "cleo_idle1_right", "Idle1_Right", 6, 6f),

            // Idle2 (6f @256, 6 FPS)
            ("Idle 2/siamois_cleo_idle2_face.png",  "cleo_idle2_face",  "Idle2_Front", 6, 6f),
            ("Idle 2/siamois_cleo_idle2_left.png",  "cleo_idle2_left",  "Idle2_Left",  6, 6f),
            ("Idle 2/siamois_cleo_idle2_right.png", "cleo_idle2_right", "Idle2_Right", 6, 6f),

            // Idle3 (8f @256, 8 FPS)
            ("Idle 3/siamois_cleo_idle3_face.png",  "cleo_idle3_face",  "Idle3_Front", 8, 8f),
            ("Idle 3/siamois_cleo_idle3_back.png",  "cleo_idle3_back",  "Idle3_Back",  8, 8f),
            ("Idle 3/siamois_cleo_idle3_left.png",  "cleo_idle3_left",  "Idle3_Left",  8, 8f),
            ("Idle 3/siamois_cleo_idle3_right.png", "cleo_idle3_right", "Idle3_Right", 8, 8f),

            // Sleep (9f @256, 4.5 FPS)
            ("Sleep/siamois_cleo_sleeping_face.png",  "cleo_sleeping_face",  "Sleep_Front", 9, 4.5f),
            ("Sleep/siamois_cleo_sleeping_left.png",  "cleo_sleeping_left",  "Sleep_Left",  9, 4.5f),
            ("Sleep/siamois_cleo_sleeping_right.png", "cleo_sleeping_right", "Sleep_Right", 9, 4.5f),

            // Eat (face 12f @256, 6 FPS | left/right 10f @512, 5 FPS)
            ("Eating/siamois_cleo_eating_face.png",  "cleo_eating_face",  "Eat_Front", 12, 6f),
            ("Eating/siamois_cleo_eating_left.png",  "cleo_eating_left",  "Eat_Left",  10, 5f),
            ("Eating/siamois_cleo_eating_right.png", "cleo_eating_right", "Eat_Right", 10, 5f),

            // Drink (face 8f @256, 4 FPS | left/right 8f @512, 4 FPS)
            ("Drunking/siamois_base_drunking_face.png",  "cleo_drunking_face",  "Drink_Front", 8, 4f),
            ("Drunking/siamois_base_drunking_left.png",  "cleo_drunking_left",  "Drink_Left",  8, 4f),
            ("Drunking/siamois_base_drunking_right.png", "cleo_drunking_right", "Drink_Right", 8, 4f),

            // Cleaning (11f @256, 5.5 FPS)
            ("Cleaning/siamois_cleo_cleaning_face.png",  "cleo_cleaning_face",  "Clean_Front", 11, 5.5f),
            ("Cleaning/siamois_cleo_cleaning_left.png",  "cleo_cleaning_left",  "Clean_Left",  11, 5.5f),
            ("Cleaning/siamois_cleo_cleaning_right.png", "cleo_cleaning_right", "Clean_Right", 11, 5.5f),

            // Playing (23f @512, 11.5 FPS)
            ("Playing/siamois_cleo_playing_face.png",  "cleo_playing_face",  "Play_Front", 23, 11.5f),
            ("Playing/siamois_cleo_playing_left.png",  "cleo_playing_left",  "Play_Left",  23, 11.5f),
            ("Playing/siamois_cleo_playing_right.png", "cleo_playing_right", "Play_Right", 23, 11.5f),

            // Happy (20f @512, 10 FPS)
            ("Happy/siamois_cleo_happy_face.png",  "cleo_happy_face",  "Happy_Front", 20, 10f),
            ("Happy/siamois_cleo_happy_left.png",  "cleo_happy_left",  "Happy_Left",  20, 10f),
            ("Happy/siamois_cleo_happy_right.png", "cleo_happy_right", "Happy_Right", 20, 10f),

            // Unhappy (6f @512, 6 FPS)
            ("Unhappy/siamois_cleo_unhappy_face.png",  "cleo_unhappy_face",  "Unhappy_Front", 6, 6f),
            ("Unhappy/siamois_cleo_unhappy_left.png",  "cleo_unhappy_left",  "Unhappy_Left",  6, 6f),
            ("Unhappy/siamois_cleo_unhappy_right.png", "cleo_unhappy_right", "Unhappy_Right", 6, 6f),

            // Petting (13f @256, 6.5 FPS)
            ("Petting/siamois_cleo_petting_face.png", "cleo_petting_face", "Pet_Front", 13, 6.5f),

            // Fighting In (15f @512, 7.5 FPS)
            ("Fighting/siamois_cleo_fighting_in_left.png",  "cleo_fighting_in_left",  "Fight_In_Left",  15, 7.5f),
            ("Fighting/siamois_cleo_fighting_in_right.png", "cleo_fighting_in_right", "Fight_In_Right", 15, 7.5f),

            // Fighting Out (8f @512, 8 FPS)
            ("Fighting/siamois_cleo_fighting_out_left.png",  "cleo_fighting_out_left",  "Fight_Out_Left",  8, 8f),
            ("Fighting/siamois_cleo_fighting_out_right.png", "cleo_fighting_out_right", "Fight_Out_Right", 8, 8f),

            // SadWalk (8f @256, 12 FPS)
            ("SadWalk/siamois_cleo_walk_sad_right.png", "cleo_sadwalk_right", "SadWalk_Right", 8, 12f),
        };

        // ==================== ARISTOTE (Special Cat) ====================
        private static readonly (string file, string prefix, string state, int frames, float fps)[] AristoteAnimConfigs =
        {
            // Walk (8f @256, 12 FPS)
            ("Walk/europeen_aristote_walk_face.png",  "ari_walk_face",  "Walk_Front", 8, 12f),
            ("Walk/europeen_aristote_walk_back.png",  "ari_walk_back",  "Walk_Back",  8, 12f),
            ("Walk/europeen_aristote_walk_left.png",  "ari_walk_left",  "Walk_Left",  8, 12f),
            ("Walk/europeen_aristote_walk_right.png", "ari_walk_right", "Walk_Right", 8, 12f),

            // Idle1 (6f @256, 6 FPS)
            ("Idle 1/europeen_aristote_idle1_face.png",  "ari_idle1_face",  "Idle1_Front", 6, 6f),
            ("Idle 1/europeen_aristote_idle1_back.png",  "ari_idle1_back",  "Idle1_Back",  6, 6f),
            ("Idle 1/europeen_aristote_idle1_left.png",  "ari_idle1_left",  "Idle1_Left",  6, 6f),
            ("Idle 1/europeen_aristote_idle1_right.png", "ari_idle1_right", "Idle1_Right", 6, 6f),

            // Idle2 (6f @256, 6 FPS)
            ("Idle 2/europeen_aristote_idle2_face.png",  "ari_idle2_face",  "Idle2_Front", 6, 6f),
            ("Idle 2/europeen_aristote_idle2_left.png",  "ari_idle2_left",  "Idle2_Left",  6, 6f),
            ("Idle 2/europeen_aristote_idle2_right.png", "ari_idle2_right", "Idle2_Right", 6, 6f),

            // Idle3 (8f @256, 8 FPS)
            ("Idle 3/europeen_aristote_idle3_face.png",  "ari_idle3_face",  "Idle3_Front", 8, 8f),
            ("Idle 3/europeen_aristote_idle3_back.png",  "ari_idle3_back",  "Idle3_Back",  8, 8f),
            ("Idle 3/europeen_aristote_idle3_left.png",  "ari_idle3_left",  "Idle3_Left",  8, 8f),
            ("Idle 3/europeen_aristote_idle3_right.png", "ari_idle3_right", "Idle3_Right", 8, 8f),

            // Sleep (9f @256, 4.5 FPS)
            ("Sleep/europeen_aristote_sleeping_face.png",  "ari_sleeping_face",  "Sleep_Front", 9, 4.5f),
            ("Sleep/europeen_aristote_sleeping_left.png",  "ari_sleeping_left",  "Sleep_Left",  9, 4.5f),
            ("Sleep/europeen_aristote_sleeping_right.png", "ari_sleeping_right", "Sleep_Right", 9, 4.5f),

            // Eat (face 12f @256, 6 FPS | left/right 10f @512, 5 FPS)
            ("Eating/europeen_aristote_eating_face.png",  "ari_eating_face",  "Eat_Front", 12, 6f),
            ("Eating/europeen_aristote_eating_left.png",  "ari_eating_left",  "Eat_Left",  10, 5f),
            ("Eating/europeen_aristote_eating_right.png", "ari_eating_right", "Eat_Right", 10, 5f),

            // Drink (face 8f @256, 4 FPS | left/right 8f @512, 4 FPS)
            ("Drunking/europeen_aristote_drunking_face.png",  "ari_drunking_face",  "Drink_Front", 8, 4f),
            ("Drunking/europeen_aristote_drunking_left.png",  "ari_drunking_left",  "Drink_Left",  8, 4f),
            ("Drunking/europeen_aristote_drunking_right.png", "ari_drunking_right", "Drink_Right", 8, 4f),

            // Cleaning (11f @256, 5.5 FPS)
            ("Cleaning/europeen_aristote_cleaning_face.png",  "ari_cleaning_face",  "Clean_Front", 11, 5.5f),
            ("Cleaning/europeen_aristote_cleaning_left.png",  "ari_cleaning_left",  "Clean_Left",  11, 5.5f),
            ("Cleaning/europeen_aristote_cleaning_right.png", "ari_cleaning_right", "Clean_Right", 11, 5.5f),

            // Playing (23f @512, 11.5 FPS)
            ("Playing/europeen_aristote_playing_face.png",  "ari_playing_face",  "Play_Front", 23, 11.5f),
            ("Playing/europeen_aristote_playing_left.png",  "ari_playing_left",  "Play_Left",  23, 11.5f),
            ("Playing/europeen_aristote_playing_right.png", "ari_playing_right", "Play_Right", 23, 11.5f),

            // Playing In (3f @512, 6 FPS)
            ("Playing/europeen_aristote_playing_in_face.png",  "ari_playing_in_face",  "Play_In_Front", 3, 6f),
            ("Playing/europeen_aristote_playing_in_left.png",  "ari_playing_in_left",  "Play_In_Left",  3, 6f),
            ("Playing/europeen_aristote_playing_in_right.png", "ari_playing_in_right", "Play_In_Right", 3, 6f),

            // Playing Boucle (16f @512, 8 FPS)
            ("Playing/europeen_aristote_playing_boucle_face.png",  "ari_playing_boucle_face",  "Play_Boucle_Front", 16, 8f),
            ("Playing/europeen_aristote_playing_boucle_left.png",  "ari_playing_boucle_left",  "Play_Boucle_Left",  17, 8.5f),
            ("Playing/europeen_aristote_playing_boucle_right.png", "ari_playing_boucle_right", "Play_Boucle_Right", 17, 8.5f),

            // Playing Out (face 4f, left/right 3f @512, 6 FPS)
            ("Playing/europeen_aristote_playing_out_face.png",  "ari_playing_out_face",  "Play_Out_Front", 4, 6f),
            ("Playing/europeen_aristote_playing_out_left.png",  "ari_playing_out_left",  "Play_Out_Left",  3, 6f),
            ("Playing/europeen_aristote_playing_out_right.png", "ari_playing_out_right", "Play_Out_Right", 3, 6f),

            // Happy (20f @512, 10 FPS)
            ("Happy/europeen_aristote_happy_face.png",  "ari_happy_face",  "Happy_Front", 20, 10f),
            ("Happy/europeen_aristote_happy_left.png",  "ari_happy_left",  "Happy_Left",  20, 10f),
            ("Happy/europeen_aristote_happy_right.png", "ari_happy_right", "Happy_Right", 20, 10f),

            // Unhappy (6f @512, 6 FPS)
            ("Unhappy/europeen_aristote_unhappy_face.png",  "ari_unhappy_face",  "Unhappy_Front", 6, 6f),
            ("Unhappy/europeen_aristote_unhappy_left.png",  "ari_unhappy_left",  "Unhappy_Left",  6, 6f),
            ("Unhappy/europeen_aristote_unhappy_right.png", "ari_unhappy_right", "Unhappy_Right", 6, 6f),

            // Petting (13f @256, 6.5 FPS)
            ("Petting/europeen_aristote_petting_face.png", "ari_petting_face", "Pet_Front", 13, 6.5f),

            // Fighting In (15f @512, 7.5 FPS)
            ("Fighting/europeen_aristote_fighting_in_left.png",  "ari_fighting_in_left",  "Fight_In_Left",  15, 7.5f),
            ("Fighting/europeen_aristote_fighting_in_right.png", "ari_fighting_in_right", "Fight_In_Right", 15, 7.5f),

            // Fighting Out (8f @512, 8 FPS)
            ("Fighting/europeen_aristote_fighting_out_left.png",  "ari_fighting_out_left",  "Fight_Out_Left",  8, 8f),
            ("Fighting/europeen_aristote_fighting_out_right.png", "ari_fighting_out_right", "Fight_Out_Right", 8, 8f),

            // SadWalk (8f @256, 12 FPS)
            ("SadWalk/europeen_aristote_walk_sad_right.png", "ari_sadwalk_right", "SadWalk_Right", 8, 12f),
        };

        // ==================== CHARTREUX ====================
        private static readonly (string file, string prefix, string state, int frames, float fps)[] ChartrAnimConfigs =
        {
            ("Walk/chartreu_base_walk_face.png",  "chr_walk_face",  "Walk_Front", 8, 12f),
            ("Walk/chartreu_base_walk_back.png",  "chr_walk_back",  "Walk_Back",  8, 12f),
            ("Walk/chartreu_base_walk_left.png",  "chr_walk_left",  "Walk_Left",  8, 12f),
            ("Walk/chartreu_base_walk_right.png", "chr_walk_right", "Walk_Right", 8, 12f),
            ("Idle 1/chartreu_base_idle1_face.png",  "chr_idle1_face",  "Idle1_Front", 6, 6f),
            ("Idle 1/chartreu_base_idle1_back.png",  "chr_idle1_back",  "Idle1_Back",  6, 6f),
            ("Idle 1/chartreu_base_idle1_left.png",  "chr_idle1_left",  "Idle1_Left",  6, 6f),
            ("Idle 1/chartreu_base_idle1_right.png", "chr_idle1_right", "Idle1_Right", 6, 6f),
            ("Idle 2/chartreu_base_idle2_face.png",  "chr_idle2_face",  "Idle2_Front", 6, 6f),
            ("Idle 2/chartreu_base_idle2_left.png",  "chr_idle2_left",  "Idle2_Left",  6, 6f),
            ("Idle 2/chartreu_base_idle2_right.png", "chr_idle2_right", "Idle2_Right", 6, 6f),
            ("Idle 3/chartreu_base_idle3_face.png",  "chr_idle3_face",  "Idle3_Front", 8, 8f),
            ("Idle 3/chartreu_base_idle3_back.png",  "chr_idle3_back",  "Idle3_Back",  8, 8f),
            ("Idle 3/chartreu_base_idle3_left.png",  "chr_idle3_left",  "Idle3_Left",  8, 8f),
            ("Idle 3/chartreu_base_idle3_right.png", "chr_idle3_right", "Idle3_Right", 8, 8f),
            ("Sleep/chartreu_base_sleeping_face.png",  "chr_sleeping_face",  "Sleep_Front", 9, 4.5f),
            ("Sleep/chartreu_base_sleeping_left.png",  "chr_sleeping_left",  "Sleep_Left",  9, 4.5f),
            ("Sleep/chartreu_base_sleeping_right.png", "chr_sleeping_right", "Sleep_Right", 9, 4.5f),
            ("Eating/chartreu_base_eating_face.png",  "chr_eating_face",  "Eat_Front", 12, 6f),
            ("Eating/chartreu_base_eating_left.png",  "chr_eating_left",  "Eat_Left",  10, 5f),
            ("Eating/chartreu_base_eating_right.png", "chr_eating_right", "Eat_Right", 10, 5f),
            ("Drunking/chartreu_base_drunking_face.png",  "chr_drunking_face",  "Drink_Front", 8, 4f),
            ("Drunking/chartreu_base_drunking_left.png",  "chr_drunking_left",  "Drink_Left",  8, 4f),
            ("Drunking/chartreu_base_drunking_right.png", "chr_drunking_right", "Drink_Right", 8, 4f),
            ("Cleaning/chartreu_base_cleaning_face.png",  "chr_cleaning_face",  "Clean_Front", 11, 5.5f),
            ("Cleaning/chartreu_base_cleaning_left.png",  "chr_cleaning_left",  "Clean_Left",  11, 5.5f),
            ("Cleaning/chartreu_base_cleaning_right.png", "chr_cleaning_right", "Clean_Right", 11, 5.5f),
            ("Playing/chartreu_base_playing_face.png",  "chr_playing_face",  "Play_Front", 23, 11.5f),
            ("Playing/chartreu_base_playing_left.png",  "chr_playing_left",  "Play_Left",  23, 11.5f),
            ("Playing/chartreu_base_playing_right.png", "chr_playing_right", "Play_Right", 23, 11.5f),
            ("Happy/chartreu_base_happy_face.png",  "chr_happy_face",  "Happy_Front", 20, 10f),
            ("Happy/chartreu_base_happy_left.png",  "chr_happy_left",  "Happy_Left",  20, 10f),
            ("Happy/chartreu_base_happy_right.png", "chr_happy_right", "Happy_Right", 20, 10f),
            ("Unhappy/chartreu_base_unhappy_face.png",  "chr_unhappy_face",  "Unhappy_Front", 6, 6f),
            ("Unhappy/chartreu_base_unhappy_left.png",  "chr_unhappy_left",  "Unhappy_Left",  6, 6f),
            ("Unhappy/chartreu_base_unhappy_right.png", "chr_unhappy_right", "Unhappy_Right", 6, 6f),
            ("Petting/chartreu_base_petting_face.png", "chr_petting_face", "Pet_Front", 13, 6.5f),
            ("Fighting/chartreu_fighting_in_left.png",  "chr_fighting_in_left",  "Fight_In_Left",  15, 7.5f),
            ("Fighting/chartreu_fighting_in_right.png", "chr_fighting_in_right", "Fight_In_Right", 15, 7.5f),
            ("Fighting/chartreu_fighting_out_left.png",  "chr_fighting_out_left",  "Fight_Out_Left",  8, 8f),
            ("Fighting/chartreu_fighting_out_right.png", "chr_fighting_out_right", "Fight_Out_Right", 8, 8f),
            ("Walk Sad/chartreu_base_walk_sad_right.png", "chr_sadwalk_right", "SadWalk_Right", 8, 12f),
            ("Scratch/In/chartreu_Base_Scratch_In_Left.png",  "chr_scratch_in_left",  "Scratch_In_Left",  7, 14f),
            ("Scratch/In/chartreu_Base_Scratch_In_Right.png", "chr_scratch_in_right", "Scratch_In_Right", 7, 14f),
            ("Scratch/Boucle/chartreu_Base_Scratch_Left.png",  "chr_scratch_boucle_left",  "Scratch_Boucle_Left",  11, 5.5f),
            ("Scratch/Boucle/chartreu_Base_Scratch_Right.png", "chr_scratch_boucle_right", "Scratch_Boucle_Right", 11, 5.5f),
            ("Scratch/Out/chartreu_Base_Scratch_out_Left.png",  "chr_scratch_out_left",  "Scratch_Out_Left",  7, 14f),
            ("Scratch/Out/chartreu_Base_Scratch_out_Right.png", "chr_scratch_out_right", "Scratch_Out_Right", 7, 14f),
        };

        // ==================== NAPOLEON (Special - Chartreux) ====================
        // Note: some filenames have typos (chartreu_base_idle1_left, chartreu_nepoleon)
        private static readonly (string file, string prefix, string state, int frames, float fps)[] NapoleonAnimConfigs =
        {
            ("Walk/chartreu_napoleon_walk_face.png",  "nap_walk_face",  "Walk_Front", 8, 12f),
            ("Walk/chartreu_napoleon_walk_back.png",  "nap_walk_back",  "Walk_Back",  8, 12f),
            ("Walk/chartreu_napoleon_walk_left.png",  "nap_walk_left",  "Walk_Left",  8, 12f),
            ("Walk/chartreu_napoleon_walk_right.png", "nap_walk_right", "Walk_Right", 8, 12f),
            ("Idle 1/chartreu_napoleon_idle1_face.png",  "nap_idle1_face",  "Idle1_Front", 6, 6f),
            ("Idle 1/chartreu_napoleon_idle1_back.png",  "nap_idle1_back",  "Idle1_Back",  6, 6f),
            ("Idle 1/chartreu_base_idle1_left.png",      "nap_idle1_left",  "Idle1_Left",  6, 6f),
            ("Idle 1/chartreu_napoleon_idle1_right.png", "nap_idle1_right", "Idle1_Right", 6, 6f),
            ("Idle 2/chartreu_napoleon_idle2_face.png",  "nap_idle2_face",  "Idle2_Front", 6, 6f),
            ("Idle 2/chartreu_napoleon_idle2_left.png",  "nap_idle2_left",  "Idle2_Left",  6, 6f),
            ("Idle 2/chartreu_napoleon_idle2_right.png", "nap_idle2_right", "Idle2_Right", 6, 6f),
            ("Idle 3/chartreu_napoleon_idle3_face.png",    "nap_idle3_face",  "Idle3_Front", 8, 8f),
            ("Idle 3/chartreu_napoleon_idle3_back.png",    "nap_idle3_back",  "Idle3_Back",  8, 8f),
            ("Idle 3/chartreu_nepoleon_idle3_left.png",    "nap_idle3_left",  "Idle3_Left",  8, 8f),
            ("Idle 3/chartreu_nepoleon_idle3_right.png",   "nap_idle3_right", "Idle3_Right", 8, 8f),
            ("Sleep/chartreu_napoleon_sleeping_face.png",  "nap_sleeping_face",  "Sleep_Front", 9, 4.5f),
            ("Sleep/chartreu_napoleon_sleeping_left.png",  "nap_sleeping_left",  "Sleep_Left",  9, 4.5f),
            ("Sleep/chartreu_napoleon_sleeping_right.png", "nap_sleeping_right", "Sleep_Right", 9, 4.5f),
            ("Eating/chartreu_napoleon_eating_face.png",  "nap_eating_face",  "Eat_Front", 12, 6f),
            ("Eating/chartreu_napoleon_eating_left.png",  "nap_eating_left",  "Eat_Left",  10, 5f),
            ("Eating/chartreu_napoleon_eating_right.png", "nap_eating_right", "Eat_Right", 10, 5f),
            ("Drunking/chartreu_napoleon_drunking_face.png",  "nap_drunking_face",  "Drink_Front", 8, 4f),
            ("Drunking/chartreu_napoleon_drunking_left.png",  "nap_drunking_left",  "Drink_Left",  8, 4f),
            ("Drunking/chartreu_napoleon_drunking_right.png", "nap_drunking_right", "Drink_Right", 8, 4f),
            ("Cleaning/chartreu_napoleon_cleaning_face.png",  "nap_cleaning_face",  "Clean_Front", 11, 5.5f),
            ("Cleaning/chartreu_napoleon_cleaning_left.png",  "nap_cleaning_left",  "Clean_Left",  11, 5.5f),
            ("Cleaning/chartreu_napoleon_cleaning_right.png", "nap_cleaning_right", "Clean_Right", 11, 5.5f),
            ("Playing/chartreu_napoleon_playing_face.png",  "nap_playing_face",  "Play_Front", 23, 11.5f),
            ("Playing/chartreu_napoleon_playing_left.png",  "nap_playing_left",  "Play_Left",  23, 11.5f),
            ("Playing/chartreu_napoleon_playing_right.png", "nap_playing_right", "Play_Right", 23, 11.5f),
            ("Happy/chartreu_napoleon_happy_face.png",  "nap_happy_face",  "Happy_Front", 20, 10f),
            ("Happy/chartreu_napoleon_happy_left.png",  "nap_happy_left",  "Happy_Left",  20, 10f),
            ("Happy/chartreu_napoleon_happy_right.png", "nap_happy_right", "Happy_Right", 20, 10f),
            ("Unhappy/chartreu_napoleon_unhappy_face.png",  "nap_unhappy_face",  "Unhappy_Front", 6, 6f),
            ("Unhappy/chartreu_napoleon_unhappy_left.png",  "nap_unhappy_left",  "Unhappy_Left",  6, 6f),
            ("Unhappy/chartreu_napoleon_unhappy_right.png", "nap_unhappy_right", "Unhappy_Right", 6, 6f),
            ("Petting/chartreu_napoleon_petting_face.png", "nap_petting_face", "Pet_Front", 13, 6.5f),
            ("Fighting/chartreu_napoleon_fighting_in_left.png",  "nap_fighting_in_left",  "Fight_In_Left",  15, 7.5f),
            ("Fighting/chartreu_napoleon_fighting_in_right.png", "nap_fighting_in_right", "Fight_In_Right", 15, 7.5f),
            ("Fighting/chartreu_fighting_out_left.png",          "nap_fighting_out_left",  "Fight_Out_Left",  8, 8f),
            ("Fighting/chartreu_napoleon_fighting_out_right.png","nap_fighting_out_right", "Fight_Out_Right", 8, 8f),
            ("Walk Sad/chartreu_napoleon_walk_sad_right.png", "nap_sadwalk_right", "SadWalk_Right", 8, 12f),
            ("Scratch/In/chartreu_napoleon_Scratch_In_Left.png",  "nap_scratch_in_left",  "Scratch_In_Left",  7, 14f),
            ("Scratch/In/chartreu_napoleon_Scratch_In_Right.png", "nap_scratch_in_right", "Scratch_In_Right", 7, 14f),
            ("Scratch/Boucle/chartreu_napoleon_Scratch_Left.png",  "nap_scratch_boucle_left",  "Scratch_Boucle_Left",  11, 5.5f),
            ("Scratch/Boucle/chartreu_napoleon_Scratch_Right.png", "nap_scratch_boucle_right", "Scratch_Boucle_Right", 11, 5.5f),
            ("Scratch/Out/chartreu_napoleon_Scratch_out_Left.png",  "nap_scratch_out_left",  "Scratch_Out_Left",  7, 14f),
            ("Scratch/Out/chartreu_napoleon_Scratch_out_Right.png", "nap_scratch_out_right", "Scratch_Out_Right", 7, 14f),
        };

        // ==================== ORION (Special - Ragdoll) ====================
        // Note: Orion cleaning face=13f, left/right=12f (like Ragdoll corrected), petting=12f
        // Eating/Drinking typos: ragodoll
        private static readonly (string file, string prefix, string state, int frames, float fps)[] OrionAnimConfigs =
        {
            ("Walk/ragdoll_orion_walk_face.png",  "ori_walk_face",  "Walk_Front", 8, 12f),
            ("Walk/ragdoll_orion_walk_back.png",  "ori_walk_back",  "Walk_Back",  8, 12f),
            ("Walk/ragdoll_orion_walk_left.png",  "ori_walk_left",  "Walk_Left",  8, 12f),
            ("Walk/ragdoll_orion_walk_right.png", "ori_walk_right", "Walk_Right", 8, 12f),
            ("Idle 1/ragdoll_orion_idle1_face.png",  "ori_idle1_face",  "Idle1_Front", 6, 6f),
            ("Idle 1/ragdoll_orion_idle1_back.png",  "ori_idle1_back",  "Idle1_Back",  6, 6f),
            ("Idle 1/ragdoll_orion_idle1_left.png",  "ori_idle1_left",  "Idle1_Left",  6, 6f),
            ("Idle 1/ragdoll_orion_idle1_right.png", "ori_idle1_right", "Idle1_Right", 6, 6f),
            ("Idle 2/ragdoll_orion_idle2_face.png",  "ori_idle2_face",  "Idle2_Front", 6, 6f),
            ("Idle 2/ragdoll_orion_idle2_left.png",  "ori_idle2_left",  "Idle2_Left",  6, 6f),
            ("Idle 2/ragdoll_orion_idle2_right.png", "ori_idle2_right", "Idle2_Right", 6, 6f),
            ("Idle 3/ragdoll_orion_idle3_face.png",  "ori_idle3_face",  "Idle3_Front", 8, 8f),
            ("Idle 3/ragdoll_orion_idle3_back.png",  "ori_idle3_back",  "Idle3_Back",  8, 8f),
            ("Idle 3/ragdoll_orion_idle3_left.png",  "ori_idle3_left",  "Idle3_Left",  8, 8f),
            ("Idle 3/ragdoll_orion_idle3_right.png", "ori_idle3_right", "Idle3_Right", 8, 8f),
            ("Sleep/ragdoll_orion_sleeping_face.png",  "ori_sleeping_face",  "Sleep_Front", 9, 4.5f),
            ("Sleep/ragdoll_orion_sleeping_left.png",  "ori_sleeping_left",  "Sleep_Left",  9, 4.5f),
            ("Sleep/ragdoll_orion_sleeping_right.png", "ori_sleeping_right", "Sleep_Right", 9, 4.5f),
            ("Eating/ragdoll_orion_eating_face.png",   "ori_eating_face",  "Eat_Front", 12, 6f),
            ("Eating/ragodoll_orion_eating_left.png",  "ori_eating_left",  "Eat_Left",  12, 6f),
            ("Eating/ragodoll_orion_eating_right.png", "ori_eating_right", "Eat_Right", 12, 6f),
            ("Drunking/ragdoll_orion_drinking_face.png",   "ori_drinking_face",  "Drink_Front", 8, 4f),
            ("Drunking/ragodoll_orion_drinking_left.png",  "ori_drinking_left",  "Drink_Left",  8, 4f),
            ("Drunking/ragodoll_orion_drinking_right.png", "ori_drinking_right", "Drink_Right", 8, 4f),
            ("Cleaning/ragdoll_orion_cleaning_face.png",  "ori_cleaning_face",  "Clean_Front", 13, 6.5f),
            ("Cleaning/ragdoll_orion_cleaning_left.png",  "ori_cleaning_left",  "Clean_Left",  12, 6f),
            ("Cleaning/ragdoll_orion_cleaning_right.png", "ori_cleaning_right", "Clean_Right", 12, 6f),
            ("Playing/ragdoll_orion_playing_face.png",  "ori_playing_face",  "Play_Front", 23, 11.5f),
            ("Playing/ragdoll_orion_playing_left.png",  "ori_playing_left",  "Play_Left",  23, 11.5f),
            ("Playing/ragdoll_orion_playing_right.png", "ori_playing_right", "Play_Right", 23, 11.5f),
            ("Happy/ragdoll_orion_happy_face.png",  "ori_happy_face",  "Happy_Front", 20, 10f),
            ("Happy/ragdoll_orion_happy_left.png",  "ori_happy_left",  "Happy_Left",  20, 10f),
            ("Happy/ragdoll_orion_happy_right.png", "ori_happy_right", "Happy_Right", 20, 10f),
            ("Unhappy/ragdoll_orion_unhappy_face.png",  "ori_unhappy_face",  "Unhappy_Front", 6, 6f),
            ("Unhappy/ragdoll_orion_unhappy_left.png",  "ori_unhappy_left",  "Unhappy_Left",  6, 6f),
            ("Unhappy/ragdoll_orion_unhappy_right.png", "ori_unhappy_right", "Unhappy_Right", 6, 6f),
            ("Petting/ragdoll_orion_petting_face.png", "ori_petting_face", "Pet_Front", 12, 6f),
            ("Fighting/ragdoll_orion_fighting_in_left.png",  "ori_fighting_in_left",  "Fight_In_Left",  15, 7.5f),
            ("Fighting/ragdoll_orion_fighting_in_right.png", "ori_fighting_in_right", "Fight_In_Right", 15, 7.5f),
            ("Fighting/ragdoll_orion_fighting_out_left.png",  "ori_fighting_out_left",  "Fight_Out_Left",  8, 8f),
            ("Fighting/ragdoll_orion_fighting_out_right.png", "ori_fighting_out_right", "Fight_Out_Right", 8, 8f),
            ("Walk Sad/ragdoll_orion_walk_sad_right.png", "ori_sadwalk_right", "SadWalk_Right", 8, 12f),
        };

        [MenuItem("Cat Hotel/Setup Proto Scene")]
        public static void SetupScene()
        {
            ConfigureSpriteImports();
            ConfigureCatSpriteImports();

            // Process all animation spritesheets
            ProcessAnimConfigs(AnimRoot, AnimConfigs);
            ProcessAnimConfigs(Eur2AnimRoot, Eur2AnimConfigs);
            ProcessAnimConfigs(Eur3AnimRoot, Eur3AnimConfigs);
            ProcessAnimConfigs(SiamoisAnimRoot, SiamoisAnimConfigs);
            ProcessAnimConfigs(RagdollAnimRoot, RagdollAnimConfigs);
            ProcessAnimConfigs(Ragdoll2AnimRoot, Ragdoll2AnimConfigs);
            ProcessAnimConfigs(SibBlackAnimRoot, SibBlackAnimConfigs);
            ProcessAnimConfigs(SibWhiteAnimRoot, SibWhiteAnimConfigs);
            ProcessAnimConfigs(CleoAnimRoot, CleoAnimConfigs);
            ProcessAnimConfigs(AristoteAnimRoot, AristoteAnimConfigs);
            ProcessAnimConfigs(ChartrAnimRoot, ChartrAnimConfigs);
            ProcessAnimConfigs(NapoleonAnimRoot, NapoleonAnimConfigs);
            ProcessAnimConfigs(OrionAnimRoot, OrionAnimConfigs);
            ProcessAnimConfigs(CloudRoot, CloudAnimConfigs);
            ProcessAnimConfigs(PettingRoot, HandPetAnimConfigs);
            ProcessAnimConfigs(CoinSpinRoot, CoinSpinAnimConfigs);
            ProcessAnimConfigs(CarpetCosmicAnimRoot, CarpetCosmicAnimConfigs);
            AssetDatabase.Refresh();

            var tiles = CreateTileAssets();
            var eurController = CreateAnimController(CatControllerPath, AnimRoot, AnimConfigs);
            var eur2Controller = CreateAnimController(Eur2ControllerPath, Eur2AnimRoot, Eur2AnimConfigs);
            var eur3Controller = CreateAnimController(Eur3ControllerPath, Eur3AnimRoot, Eur3AnimConfigs);
            var siamoisController = CreateAnimController(SiamoisControllerPath, SiamoisAnimRoot, SiamoisAnimConfigs);
            var ragdollController = CreateAnimController(RagdollControllerPath, RagdollAnimRoot, RagdollAnimConfigs);
            var ragdoll2Controller = CreateAnimController(Ragdoll2ControllerPath, Ragdoll2AnimRoot, Ragdoll2AnimConfigs);
            var sibBlackController = CreateAnimController(SibBlackControllerPath, SibBlackAnimRoot, SibBlackAnimConfigs);
            var sibWhiteController = CreateAnimController(SibWhiteControllerPath, SibWhiteAnimRoot, SibWhiteAnimConfigs);
            var cleoController = CreateAnimController(CleoControllerPath, CleoAnimRoot, CleoAnimConfigs);
            var aristoteController = CreateAnimController(AristoteControllerPath, AristoteAnimRoot, AristoteAnimConfigs);
            var chartrController = CreateAnimController(ChartrControllerPath, ChartrAnimRoot, ChartrAnimConfigs);
            var napoleonController = CreateAnimController(NapoleonControllerPath, NapoleonAnimRoot, NapoleonAnimConfigs);
            var orionController = CreateAnimController(OrionControllerPath, OrionAnimRoot, OrionAnimConfigs);
            var cloudController = CreateAnimController(CloudControllerPath, CloudRoot, CloudAnimConfigs);
            var handPetController = CreateAnimController(HandPetControllerPath, PettingRoot, HandPetAnimConfigs);
            var coinSpinController = CreateAnimController(CoinSpinControllerPath, CoinSpinRoot, CoinSpinAnimConfigs);
            CreateAnimController(CarpetCosmicControllerPath, CarpetCosmicAnimRoot, CarpetCosmicAnimConfigs);
            BuildSceneHierarchy(tiles, eurController, eur2Controller, eur3Controller, siamoisController, ragdollController, ragdoll2Controller, sibBlackController, sibWhiteController, chartrController, cleoController, aristoteController, napoleonController, orionController, cloudController, handPetController, coinSpinController);
            int total = AnimConfigs.Length + Eur2AnimConfigs.Length + Eur3AnimConfigs.Length
                      + SiamoisAnimConfigs.Length + RagdollAnimConfigs.Length + Ragdoll2AnimConfigs.Length
                      + SibBlackAnimConfigs.Length + SibWhiteAnimConfigs.Length
                      + ChartrAnimConfigs.Length + NapoleonAnimConfigs.Length + OrionAnimConfigs.Length
                      + CleoAnimConfigs.Length + AristoteAnimConfigs.Length + CloudAnimConfigs.Length + HandPetAnimConfigs.Length + CoinSpinAnimConfigs.Length + CarpetCosmicAnimConfigs.Length;
            Debug.Log($"Proto scene setup complete. {total} animation clips configured.");
        }

        private static void ProcessAnimConfigs(string animRoot,
            (string file, string prefix, string state, int frames, float fps)[] configs)
        {
            foreach (var cfg in configs)
                ConfigureSpritesheet($"{animRoot}/{cfg.file}", cfg.prefix, cfg.frames);
        }

        private static void ConfigureSpriteImports()
        {
            // tile_empty is 32x32 → PPU 32
            ConfigureSprite($"{SpritesRoot}/Tiles/tile_empty.png", 32, FilterMode.Point);

            // Floor sprites are 256x256 → PPU 256 so 1 sprite = 1 tile
            string[] floorFiles =
            {
                "FLOOR_Basic.png", "FLOOR_Basic_Var01.png",
                "FLOOR_01.png", "FLOOR_01_Var_01.png",
                "FLOOR_02.png", "FLOOR_02_Var01.png",
                "FLOOR_03.png", "FLOOR_03_Var_01.png",
                "FLOOR_04.png", "FLOOR_04_Var_01.png",
                "FLOOR_05.png", "FLOOR_05_Var01.png",
            };
            foreach (var f in floorFiles)
                ConfigureSprite($"{SpritesRoot}/Floors/{f}", 256, FilterMode.Point);
            ConfigureSprite($"{SpritesRoot}/Walls/WALL_H.png", 256, FilterMode.Point);
            ConfigureSprite($"{SpritesRoot}/Walls/WALL_BOT_Middle.png", 256, FilterMode.Point);
            ConfigureSprite($"{SpritesRoot}/Walls/WALL_LEFT_Top.png", 256, FilterMode.Point);
            ConfigureSprite($"{SpritesRoot}/Walls/WALL_LEFT_Middle.png", 256, FilterMode.Point);
            ConfigureSprite($"{SpritesRoot}/Walls/WALL_RIGHT_Top.png", 256, FilterMode.Point);
            ConfigureSprite($"{SpritesRoot}/Walls/WALL_RIGHT_Middle.png", 256, FilterMode.Point);
            ConfigureSprite($"{SpritesRoot}/Walls/WALL_H_Left.png", 256, FilterMode.Point);
            ConfigureSprite($"{SpritesRoot}/Walls/WALL_H_Right.png", 256, FilterMode.Point);

            // Interior wall sprites — centered on grid lines
            string iw = $"{SpritesRoot}/Walls/Interrior";
            // H wall: centered vertically on horizontal grid line
            ConfigureSprite($"{iw}/WALL_h.png", 256, FilterMode.Point, new Vector2(0f, 0.5f));
            // V wall: centered horizontally on vertical grid line
            ConfigureSprite($"{iw}/WALL_v.png", 256, FilterMode.Point, new Vector2(0.5f, 0f));
            // All corners: centered, PPU 245 (26/245 ≈ 0.106, matching wall thickness 26/256 ≈ 0.101)
            ConfigureSprite($"{iw}/CORNER_CROIX.png",         245, FilterMode.Point, new Vector2(0.5f, 0.5f));
            ConfigureSprite($"{iw}/CORNER_T_SHAPE.png",       245, FilterMode.Point, new Vector2(0.5f, 0.5f));
            ConfigureSprite($"{iw}/CORNER_LEFT_BOTTOM.png",   245, FilterMode.Point, new Vector2(0.5f, 0.5f));
            ConfigureSprite($"{iw}/CORNER_LEFT_TOP.png",      245, FilterMode.Point, new Vector2(0.5f, 0.5f));
            ConfigureSprite($"{iw}/CORNER_RIGHT_BOTTOM.png",  245, FilterMode.Point, new Vector2(0.5f, 0.5f));
            ConfigureSprite($"{iw}/CORNER_RIGHT_TOP.png",     245, FilterMode.Point, new Vector2(0.5f, 0.5f));

            // Object sprites — PPU 200 (same as cats) with Bilinear
            string[] objectSprites =
            {
                $"{ObjectsRoot}/Env/SHELF.png",
                $"{ObjectsRoot}/Env/SHELF_Var_01.png",
                $"{ObjectsRoot}/Env/PLANTE.png",
                $"{ObjectsRoot}/Env/PLANT_BIG.png",
                // Beds
                $"{ObjectsRoot}/Beds/BED.png",
                $"{ObjectsRoot}/Beds/BED_SELEC.png",
                $"{ObjectsRoot}/Beds/LUXOUS_BED.png",
                $"{ObjectsRoot}/Beds/LUXOUS_BED_SELEC.png",
                // Pillows
                $"{ObjectsRoot}/Pillows/COUSSIN.png",
                $"{ObjectsRoot}/Pillows/COUSSIN_SELEC.png",
                // Food
                $"{ObjectsRoot}/Food/FOOD_BOWL 1.png",
                $"{ObjectsRoot}/Food/FOOD_BOWL_Full.png",
                $"{ObjectsRoot}/Food/FOOD_BOWL_SELEC.png",
                $"{ObjectsRoot}/Food/FOOD_BOWL_Var_02.png",
                $"{ObjectsRoot}/Food/FOOD_BOWL_Var_02_Full.png",
                $"{ObjectsRoot}/Food/FOOD_BOWL_Var_02_SELEC.png",
                $"{ObjectsRoot}/Food/FOOD_BOWL_Var_03.png",
                $"{ObjectsRoot}/Food/FOOD_BOWL_Var_03_Full.png",
                $"{ObjectsRoot}/Food/FOOD_BOWL_Var_03_SELEC.png",
                $"{ObjectsRoot}/Food/FOOD_BOWL_Var_04.png",
                $"{ObjectsRoot}/Food/FOOD_BOWL_Var_04_Fiull.png",
                $"{ObjectsRoot}/Food/FOOD_BOWL_Var_04_SELEC.png",
                // Water
                $"{ObjectsRoot}/Water/WATER_BOWL.png",
                $"{ObjectsRoot}/Water/WATER_BOWL_Full.png",
                $"{ObjectsRoot}/Water/WATER_BOWL_SELEC.png",
                $"{ObjectsRoot}/Water/WATER_BOWL_04.png",
                $"{ObjectsRoot}/Water/WATER_BOWL_04_Full.png",
                $"{ObjectsRoot}/Water/WATER_BOWL_04_SELEC.png",
                $"{ObjectsRoot}/Water/WATER_BOWL_Var_02.png",
                $"{ObjectsRoot}/Water/WATER_BOWL_Var_02_Full.png",
                $"{ObjectsRoot}/Water/WATER_BOWL_Var_02_SELEC.png",
                $"{ObjectsRoot}/Water/WATER_BOWL_Var_03.png",
                $"{ObjectsRoot}/Water/WATER_BOWL_Var_03_Full.png",
                $"{ObjectsRoot}/Water/WATER_BOWL_Var_03_SELEC.png",
                // Toys
                $"{ObjectsRoot}/Toys/WOOL_BALL.png",
                $"{ObjectsRoot}/Toys/WOOL_BALL_SELEC.png",
                $"{ObjectsRoot}/Toys/CATS_TREE.png",
                $"{ObjectsRoot}/Toys/CATS_TREE_SELEC.png",
                // Scratchers
                $"{ObjectsRoot}/Scratchers/SCRATCHER.png",
                $"{ObjectsRoot}/Scratchers/SCRATCHER_SELEC.png",
                // Litters
                $"{ObjectsRoot}/Litters/LITTER_BOX_Clean.png",
                $"{ObjectsRoot}/Litters/LITTER_BOX_Clean_SELEC.png",
                $"{ObjectsRoot}/Litters/LITTER_BOX_Var_01_Clean.png",
                $"{ObjectsRoot}/Litters/LITTER_BOX_Var_01_Clean_SELEC.png",
                // Deco
                $"{ObjectsRoot}/Deco/FRAME_01.png",
                $"{ObjectsRoot}/Deco/FRAME_01_SELEC.png",
                $"{ObjectsRoot}/Deco/FRAME_02.png",
                $"{ObjectsRoot}/Deco/FRAME_02_SELEC.png",
                $"{ObjectsRoot}/Deco/FRAME_03.png",
                $"{ObjectsRoot}/Deco/FRAME_03_SELEC.png",
                $"{ObjectsRoot}/Deco/FRAME_PAINTING.png",
                $"{ObjectsRoot}/Deco/PAINTING_SELEC.png",
                $"{ObjectsRoot}/Deco/LAMP_BIG.png",
                $"{ObjectsRoot}/Deco/TABLE_COFFEE_TABLE.png",
                $"{ObjectsRoot}/Deco/COFFEE_TABLE_SELEC.png",
                $"{ObjectsRoot}/Deco/TABLE_DRAWER_Small.png",
                $"{ObjectsRoot}/Deco/DRAWER_Small_SELEC.png",
                $"{ObjectsRoot}/Deco/PLANT_BIG.png",
                $"{ObjectsRoot}/Deco/PLANT_BIG_SELEC.png",
                $"{ObjectsRoot}/Deco/PLANT_SMALL.png",
                $"{ObjectsRoot}/Deco/SHELF_0.png",
                $"{ObjectsRoot}/Deco/SHELF_SELEC.png",
                $"{ObjectsRoot}/Deco/SHELF_Var_01.png",
                $"{ObjectsRoot}/Deco/SHELF_Var_01_SELEC.png",
                $"{ObjectsRoot}/Deco/Aquarium.png",
                // Carpets
                $"{ObjectsRoot}/Carpets/CARPET_CONFORT.png",
                $"{ObjectsRoot}/Carpets/CARPET_CONFORT_SELEC.png",
                $"{ObjectsRoot}/Carpets/CARPET_COSMIC.png",
                $"{ObjectsRoot}/Carpets/CARPET_PLAY.png",
                $"{ObjectsRoot}/Carpets/CARPET_PLAY_SELEC.png",
            };
            foreach (var s in objectSprites)
                ConfigureSprite(s, 200, FilterMode.Bilinear);

            // Mood bubble sprites — PPU 200 (same as cats)
            string[] bubbleSprites =
            {
                // Mood
                "Assets/_Project/Art/States/BULLE_EMOTIONS_Joyous.png",
                "Assets/_Project/Art/States/BULLE_EMOTIONS_Very_Happy.png",
                "Assets/_Project/Art/States/BULLE_EMOTIONS_Upset.png",
                "Assets/_Project/Art/States/BULLE_EMOTIONS_Fights.png",
                "Assets/_Project/Art/States/BULLE_EMOTIONS_Mad.png",
                // Needs
                "Assets/_Project/Art/States/BULLE_NEEDS_Hungry.png",
                "Assets/_Project/Art/States/BULLE_NEEDS_Starving.png",
                "Assets/_Project/Art/States/BULLE_NEEDS_Tired.png",
                "Assets/_Project/Art/States/BULLE_NEEDS_Bored.png",
                "Assets/_Project/Art/States/BULLE_NEEDS_Dirty.png",
            };
            foreach (var s in bubbleSprites)
                ConfigureSprite(s, 200, FilterMode.Bilinear);
        }

        private static void ConfigureCatSpriteImports()
        {
            string[] paths =
            {
                // Europeen 1
                $"{CatSpritesRoot}/CAT_EUR_FRONT.png",
                $"{CatSpritesRoot}/CAT_EUR_RIGHT.png",
                $"{CatSpritesRoot}/CAT_EUR_BACK.png",
                // Europeen 2
                $"{Eur2SpritesRoot}/CAT_EUR_02_FRONT.png",
                $"{Eur2SpritesRoot}/CAT_EUR_02_RIGHT.png",
                $"{Eur2SpritesRoot}/CAT_EUR_02_BACK.png",
                // Europeen 3
                $"{Eur3SpritesRoot}/CAT_EUR_03_FRONT.png",
                $"{Eur3SpritesRoot}/CAT_EUR_03_RIGHT.png",
                $"{Eur3SpritesRoot}/CAT_EUR_03_BACK.png",
                // Siamois
                $"{SiamoisSpritesRoot}/CAT_SIAMESE_FRONT.png",
                $"{SiamoisSpritesRoot}/CAT_SIAMESE_RIGHT.png",
                $"{SiamoisSpritesRoot}/CAT_SIAMESE_BACK.png",
                // Ragdoll
                $"{RagdollSpritesRoot}/CAT_RD_FRONT.png",
                $"{RagdollSpritesRoot}/CAT_RD_RIGHT.png",
                $"{RagdollSpritesRoot}/CAT_RD_BACK.png",
                // Aristote (Special)
                $"{AristoteSpritesRoot}/CAT_EUR_Aristote_FRONT.png",
                $"{AristoteSpritesRoot}/CAT_EUR_Aristote_RIGHT.png",
                $"{AristoteSpritesRoot}/CAT_EUR_Aristote_BACK.png",
                // Siberian Black
                $"{SibBlackSpritesRoot}/CAT_SIB_01_FRONT.png",
                $"{SibBlackSpritesRoot}/CAT_SIB_01_RIGHT.png",
                $"{SibBlackSpritesRoot}/CAT_SIB_01_BACK.png",
                // Siberian White
                $"{SibWhiteSpritesRoot}/CAT_SIB_FRONT.png",
                $"{SibWhiteSpritesRoot}/CAT_SIB_RIGHT.png",
                $"{SibWhiteSpritesRoot}/CAT_SIB_BACK.png",
                // Ragdoll 2
                $"{Ragdoll2SpritesRoot}/CAT_RD_02_FRONT.png",
                $"{Ragdoll2SpritesRoot}/CAT_RD_02_RIGHT.png",
                $"{Ragdoll2SpritesRoot}/CAT_RD_02_BACK.png",
                // Chartreux
                $"{ChartrSpritesRoot}/CAT_CHARTREU_FRONT.png",
                $"{ChartrSpritesRoot}/CAT_CHARTREU_RIGHT.png",
                $"{ChartrSpritesRoot}/CAT_CHARTREU_BACK.png",
                // Cleo (Special)
                $"{CleoSpritesRoot}/CAT_EUR_Cleo_FRONT.png",
                $"{CleoSpritesRoot}/CAT_EUR_Cleo_RIGHT.png",
                $"{CleoSpritesRoot}/CAT_EUR_Cleo_BACK.png",
                // Napoleon (Special)
                $"{NapoleonSpritesRoot}/CAT_CHARTREU_Napoléon_FRONT.png",
                $"{NapoleonSpritesRoot}/CAT_CHARTREU_Napoléon_RIGHT.png",
                $"{NapoleonSpritesRoot}/CAT_CHARTREU_Napoléon_BACK.png",
                // Orion (Special)
                $"{OrionSpritesRoot}/CAT_RD_Orion_FRONT.png",
                $"{OrionSpritesRoot}/CAT_RD_Orion_RIGHT.png",
                $"{OrionSpritesRoot}/CAT_RD_Orion_BACK.png",
            };

            foreach (string path in paths)
                ConfigureSprite(path, 200, FilterMode.Bilinear);
        }

        /// <summary>Alias for ConfigureSpritesheet — slices a PNG into N horizontal frames.</summary>
        private static void SliceSpritesheet(string path, int frames, string prefix)
            => ConfigureSpritesheet(path, prefix, frames);

        private static void ConfigureSpritesheet(string sheetPath, string namePrefix, int frameCount)
        {
            var importer = AssetImporter.GetAtPath(sheetPath) as TextureImporter;
            if (importer == null)
            {
                Debug.LogWarning($"Spritesheet not found: {sheetPath}");
                return;
            }

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.spritePixelsPerUnit = 200;
            importer.filterMode = FilterMode.Bilinear;
            importer.textureCompression = TextureImporterCompression.Compressed;
            importer.crunchedCompression = true;
            importer.compressionQuality = 50;

            importer.GetSourceTextureWidthAndHeight(out int texW, out int texH);

            int maxSize = 2048;
            while (maxSize < texW)
                maxSize *= 2;
            if (maxSize > 4096) maxSize = 4096; // safe for all Android GPUs
            importer.maxTextureSize = maxSize;

            // Android: ASTC 6x6 for best quality/size ratio on 2D sprites
            var androidSettings = importer.GetPlatformTextureSettings("Android");
            androidSettings.overridden = true;
            androidSettings.format = TextureImporterFormat.ASTC_6x6;
            androidSettings.maxTextureSize = maxSize;
            androidSettings.crunchedCompression = true;
            androidSettings.compressionQuality = 50;
            importer.SetPlatformTextureSettings(androidSettings);

            int frameW = texW / frameCount;

            var factory = new SpriteDataProviderFactories();
            factory.Init();
            var provider = factory.GetSpriteEditorDataProviderFromObject(importer);
            provider.InitSpriteEditorDataProvider();

            // Read existing rects to preserve spriteIDs (avoids .meta churn)
            var existingRects = provider.GetSpriteRects();
            var existingById = new System.Collections.Generic.Dictionary<string, GUID>();
            if (existingRects != null)
            {
                foreach (var r in existingRects)
                    existingById[r.name] = r.spriteID;
            }

            // Check if slicing already matches
            bool alreadyCorrect = existingRects != null && existingRects.Length == frameCount;
            if (alreadyCorrect)
            {
                for (int i = 0; i < frameCount; i++)
                {
                    string expectedName = $"{namePrefix}_{i}";
                    var expectedRect = new Rect(i * frameW, 0, frameW, texH);
                    if (i >= existingRects.Length ||
                        existingRects[i].name != expectedName ||
                        existingRects[i].rect != expectedRect)
                    {
                        alreadyCorrect = false;
                        break;
                    }
                }
            }

            if (alreadyCorrect) return; // Nothing to change

            var rects = new SpriteRect[frameCount];
            for (int i = 0; i < frameCount; i++)
            {
                string spriteName = $"{namePrefix}_{i}";
                rects[i] = new SpriteRect
                {
                    name = spriteName,
                    spriteID = existingById.TryGetValue(spriteName, out var existingId)
                        ? existingId : GUID.Generate(),
                    rect = new Rect(i * frameW, 0, frameW, texH),
                    alignment = SpriteAlignment.Center,
                    pivot = new Vector2(0.5f, 0.5f)
                };
            }
            provider.SetSpriteRects(rects);
            provider.Apply();

            importer.SaveAndReimport();
        }

        private static RuntimeAnimatorController CreateAnimController(
            string controllerPath, string animRoot,
            (string file, string prefix, string state, int frames, float fps)[] configs)
        {
            AssetDatabase.DeleteAsset(controllerPath);
            var controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
            var rootSM = controller.layers[0].stateMachine;
            bool defaultSet = false;

            foreach (var cfg in configs)
            {
                string sheetPath = $"{animRoot}/{cfg.file}";
                // Anim clips go next to the spritesheet (same folder)
                string sheetDir = System.IO.Path.GetDirectoryName(sheetPath).Replace('\\', '/');
                string clipPath = $"{sheetDir}/{cfg.state}.anim";

                var clip = CreateAnimClip(sheetPath, clipPath, cfg.state, cfg.frames, cfg.fps);
                if (clip == null) continue;

                var state = rootSM.AddState(cfg.state);
                state.motion = clip;

                if (!defaultSet)
                {
                    rootSM.defaultState = state;
                    defaultSet = true;
                }
            }

            AssetDatabase.SaveAssets();
            Debug.Log($"[ProtoSceneSetup] Created {controllerPath} with {rootSM.states.Length} states");
            return controller;
        }

        private static AnimationClip CreateAnimClip(
            string sheetPath, string clipPath, string clipName,
            int frameCount, float fps)
        {
            var sprites = AssetDatabase.LoadAllAssetsAtPath(sheetPath)
                .OfType<Sprite>()
                .OrderBy(s =>
                {
                    // Numeric sort: extract trailing number from sprite name
                    var name = s.name;
                    int i = name.Length - 1;
                    while (i >= 0 && char.IsDigit(name[i])) i--;
                    return int.TryParse(name.Substring(i + 1), out int num) ? num : 0;
                })
                .ToList();

            if (sprites.Count < frameCount)
            {
                Debug.LogWarning($"{clipName}: expected {frameCount} sprites in {sheetPath}, got {sprites.Count}. Skipping.");
                return null;
            }

            var clip = new AnimationClip { frameRate = fps };

            var binding = EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite");
            var keyframes = new ObjectReferenceKeyframe[frameCount + 1];
            for (int i = 0; i <= frameCount; i++)
            {
                keyframes[i] = new ObjectReferenceKeyframe
                {
                    time = i / fps,
                    value = sprites[i % frameCount]
                };
            }
            AnimationUtility.SetObjectReferenceCurve(clip, binding, keyframes);

            var clipSettings = AnimationUtility.GetAnimationClipSettings(clip);
            clipSettings.loopTime = true;
            AnimationUtility.SetAnimationClipSettings(clip, clipSettings);

            AssetDatabase.DeleteAsset(clipPath);
            AssetDatabase.CreateAsset(clip, clipPath);
            return clip;
        }

        private static void ConfigureSprite(string path, int ppu, FilterMode filter)
        {
            ConfigureSprite(path, ppu, filter, null);
        }

        private static void ConfigureSprite(string path, int ppu, FilterMode filter, Vector2? customPivot)
        {
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
            {
                Debug.LogWarning($"Sprite not found: {path}");
                return;
            }

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = ppu;
            importer.filterMode = filter;
            importer.textureCompression = TextureImporterCompression.Compressed;
            importer.crunchedCompression = true;
            importer.compressionQuality = 50;

            // Android: ASTC 6x6, cap 4096
            var androidSpr = importer.GetPlatformTextureSettings("Android");
            androidSpr.overridden = true;
            androidSpr.format = TextureImporterFormat.ASTC_6x6;
            androidSpr.maxTextureSize = 4096;
            androidSpr.crunchedCompression = true;
            androidSpr.compressionQuality = 50;
            importer.SetPlatformTextureSettings(androidSpr);

            if (customPivot.HasValue)
            {
                var settings = new TextureImporterSettings();
                importer.ReadTextureSettings(settings);
                settings.spriteAlignment = (int)SpriteAlignment.Custom;
                settings.spritePivot = customPivot.Value;
                importer.SetTextureSettings(settings);
            }

            importer.SaveAndReimport();
        }

        private struct TileSet
        {
            public TileBase empty;
            public TileBase[] floors;
            public TileBase wallTop, wallBot;
            public TileBase wallLeftTop, wallLeftMid;
            public TileBase wallRightTop, wallRightMid;
            public TileBase wallTopLeft, wallTopRight;
            // Interior walls
            public TileBase intWallH, intWallV;
            public TileBase intCroix, intTShape;
            public TileBase intCornerLB, intCornerLT, intCornerRB, intCornerRT;
        }

        private static readonly string[] FloorSpriteNames =
        {
            "FLOOR_Basic", "FLOOR_Basic_Var01",
            "FLOOR_01", "FLOOR_01_Var_01",
            "FLOOR_02", "FLOOR_02_Var01",
            "FLOOR_03", "FLOOR_03_Var_01",
            "FLOOR_04", "FLOOR_04_Var_01",
            "FLOOR_05", "FLOOR_05_Var01",
        };

        private static TileSet CreateTileAssets()
        {
            const string F = SpritesRoot + "/Floors";
            const string W = SpritesRoot + "/Walls";
            const string IW = W + "/Interrior";

            var floors = new TileBase[FloorSpriteNames.Length];
            for (int i = 0; i < FloorSpriteNames.Length; i++)
            {
                string name = FloorSpriteNames[i];
                floors[i] = CreateTile($"{F}/{name}.png", $"{F}/{name}Tile.asset");
            }

            return new TileSet
            {
                empty        = CreateTile($"{SpritesRoot}/Tiles/tile_empty.png",
                                           $"{SpritesRoot}/Tiles/EmptyTile.asset"),
                floors       = floors,
                wallTop      = CreateTile($"{W}/WALL_H.png",          $"{W}/WallTopTile.asset"),
                wallBot      = CreateTile($"{W}/WALL_BOT_Middle.png", $"{W}/WallBotTile.asset"),
                wallLeftTop  = CreateTile($"{W}/WALL_LEFT_Top.png",   $"{W}/WallLeftTopTile.asset"),
                wallLeftMid  = CreateTile($"{W}/WALL_LEFT_Middle.png",$"{W}/WallLeftMidTile.asset"),
                wallRightTop = CreateTile($"{W}/WALL_RIGHT_Top.png",  $"{W}/WallRightTopTile.asset"),
                wallRightMid = CreateTile($"{W}/WALL_RIGHT_Middle.png",$"{W}/WallRightMidTile.asset"),
                wallTopLeft  = CreateTile($"{W}/WALL_H_Left.png",      $"{W}/WallTopLeftTile.asset"),
                wallTopRight = CreateTile($"{W}/WALL_H_Right.png",     $"{W}/WallTopRightTile.asset"),
                // Interior walls (cropped sprites with custom pivots)
                intWallH    = CreateTile($"{IW}/WALL_h.png",               $"{IW}/IntWallHTile.asset"),
                intWallV    = CreateTile($"{IW}/WALL_v.png",               $"{IW}/IntWallVTile.asset"),
                intCroix    = CreateTile($"{IW}/CORNER_CROIX.png",         $"{IW}/IntCroixTile.asset"),
                intTShape   = CreateTile($"{IW}/CORNER_T_SHAPE.png",       $"{IW}/IntTShapeTile.asset"),
                intCornerLB = CreateTile($"{IW}/CORNER_LEFT_BOTTOM.png",   $"{IW}/IntCornerLBTile.asset"),
                intCornerLT = CreateTile($"{IW}/CORNER_LEFT_TOP.png",      $"{IW}/IntCornerLTTile.asset"),
                intCornerRB = CreateTile($"{IW}/CORNER_RIGHT_BOTTOM.png",  $"{IW}/IntCornerRBTile.asset"),
                intCornerRT = CreateTile($"{IW}/CORNER_RIGHT_TOP.png",     $"{IW}/IntCornerRTTile.asset"),
            };
        }

        private static TileBase CreateTile(string spritePath, string tilePath)
        {
            // Always delete and recreate to avoid stale sprite references
            var existing = AssetDatabase.LoadAssetAtPath<Tile>(tilePath);
            if (existing != null)
                AssetDatabase.DeleteAsset(tilePath);

            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            if (sprite == null)
            {
                Debug.LogError($"[Setup] Cannot load sprite at {spritePath}");
                return null;
            }

            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = sprite;
            tile.color = Color.white;

            AssetDatabase.CreateAsset(tile, tilePath);
            AssetDatabase.SaveAssets();
            Debug.Log($"[Setup] Tile created: {tilePath} → sprite {sprite.name} ({sprite.rect.width}x{sprite.rect.height})");
            return tile;
        }

        private static void BuildSceneHierarchy(
            TileSet tiles,
            RuntimeAnimatorController eurController,
            RuntimeAnimatorController eur2Controller,
            RuntimeAnimatorController eur3Controller,
            RuntimeAnimatorController siamoisController,
            RuntimeAnimatorController ragdollController,
            RuntimeAnimatorController ragdoll2Controller,
            RuntimeAnimatorController sibBlackController,
            RuntimeAnimatorController sibWhiteController,
            RuntimeAnimatorController chartrController,
            RuntimeAnimatorController cleoController,
            RuntimeAnimatorController aristoteController,
            RuntimeAnimatorController napoleonController,
            RuntimeAnimatorController orionController,
            RuntimeAnimatorController cloudController,
            RuntimeAnimatorController handPetController,
            RuntimeAnimatorController coinSpinController)
        {
            // --- Camera ---
            var camObj = Camera.main != null ? Camera.main.gameObject : null;
            if (camObj == null)
            {
                Debug.LogError("No Main Camera in scene.");
                return;
            }

            var cam = camObj.GetComponent<Camera>();
            cam.orthographic = true;
            cam.backgroundColor = new Color(0.12f, 0.12f, 0.15f);
            camObj.transform.position = new Vector3(13f, 16f, -10f);

            var camCtrl = camObj.GetComponent<CameraController>();
            if (camCtrl == null)
                camCtrl = camObj.AddComponent<CameraController>();

            // Force camera bounds for half-size grid
            var camSo = new SerializedObject(camCtrl);
            camSo.FindProperty("_minOrthoSize").floatValue = 1.5f;
            camSo.FindProperty("_maxOrthoSize").floatValue = 5f;
            camSo.FindProperty("_gridMax").vector2Value = new Vector2(26f, 32f);
            camSo.ApplyModifiedProperties();

            // --- Grid parent ---
            var gridObj = FindOrCreate("Grid");
            var grid = gridObj.GetComponent<UnityEngine.Grid>();
            if (grid == null)
                grid = gridObj.AddComponent<UnityEngine.Grid>();
            grid.cellSize = Vector3.one;

            // --- Tilemaps ---
            var tmEmpty      = CreateTilemapChild(gridObj, "Tilemap_Empty",       0);
            var tmFloor      = CreateTilemapChild(gridObj, "Tilemap_Floor",       1);
            var tmWall       = CreateTilemapChild(gridObj, "Tilemap_Wall",        2);
            var tmIntWallHSeg   = CreateTilemapChild(gridObj, "Tilemap_IntWallHSeg",   3);
            var tmIntWallVSeg   = CreateTilemapChild(gridObj, "Tilemap_IntWallVSeg",   4);
            var tmIntWallCorner = CreateTilemapChild(gridObj, "Tilemap_IntWallCorner",  5);
            var tmPreview       = CreateTilemapChild(gridObj, "Tilemap_Preview",        6);

            // Interior wall tilemaps anchor at cell corners (0,0) instead of center
            tmIntWallHSeg.tileAnchor   = Vector3.zero;
            tmIntWallVSeg.tileAnchor   = Vector3.zero;
            tmIntWallCorner.tileAnchor = Vector3.zero;

            // Clean up old tilemaps from previous versions
            foreach (var old in new[] { "Tilemap_WallCorners", "Tilemap_IntWallH",
                                        "Tilemap_IntWallV", "Tilemap_IntWall",
                                        "Tilemap_IntWallSeg", "Tilemap_IntWallCross" })
            {
                var t = gridObj.transform.Find(old);
                if (t != null) Object.DestroyImmediate(t.gameObject);
            }

            // --- GridManager ---
            var mgrObj = FindOrCreate("GridManager");

            var renderer = mgrObj.GetComponent<GridRenderer>();
            if (renderer == null)
                renderer = mgrObj.AddComponent<GridRenderer>();

            var so = new SerializedObject(renderer);
            so.FindProperty("_emptyTilemap").objectReferenceValue      = tmEmpty;
            so.FindProperty("_floorTilemap").objectReferenceValue      = tmFloor;
            so.FindProperty("_wallTilemap").objectReferenceValue       = tmWall;
            so.FindProperty("_intWallCornerTilemap").objectReferenceValue = tmIntWallCorner;
            so.FindProperty("_intWallHSegTilemap").objectReferenceValue  = tmIntWallHSeg;
            so.FindProperty("_intWallVSegTilemap").objectReferenceValue  = tmIntWallVSeg;
            so.FindProperty("_previewTilemap").objectReferenceValue    = tmPreview;
            so.FindProperty("_emptyTile").objectReferenceValue          = tiles.empty;
            var floorsProp = so.FindProperty("_floorTiles");
            floorsProp.arraySize = tiles.floors.Length;
            for (int i = 0; i < tiles.floors.Length; i++)
                floorsProp.GetArrayElementAtIndex(i).objectReferenceValue = tiles.floors[i];
            so.FindProperty("_wallTopTile").objectReferenceValue        = tiles.wallTop;
            so.FindProperty("_wallBotTile").objectReferenceValue        = tiles.wallBot;
            so.FindProperty("_wallLeftTopTile").objectReferenceValue    = tiles.wallLeftTop;
            so.FindProperty("_wallLeftMidTile").objectReferenceValue    = tiles.wallLeftMid;
            so.FindProperty("_wallRightTopTile").objectReferenceValue   = tiles.wallRightTop;
            so.FindProperty("_wallRightMidTile").objectReferenceValue   = tiles.wallRightMid;
            so.FindProperty("_wallTopLeftTile").objectReferenceValue    = tiles.wallTopLeft;
            so.FindProperty("_wallTopRightTile").objectReferenceValue   = tiles.wallTopRight;
            // Interior wall tiles
            so.FindProperty("_intWallH").objectReferenceValue      = tiles.intWallH;
            so.FindProperty("_intWallV").objectReferenceValue      = tiles.intWallV;
            so.FindProperty("_intCroix").objectReferenceValue      = tiles.intCroix;
            so.FindProperty("_intTShape").objectReferenceValue     = tiles.intTShape;
            so.FindProperty("_intCornerLB").objectReferenceValue   = tiles.intCornerLB;
            so.FindProperty("_intCornerLT").objectReferenceValue   = tiles.intCornerLT;
            so.FindProperty("_intCornerRB").objectReferenceValue   = tiles.intCornerRB;
            so.FindProperty("_intCornerRT").objectReferenceValue   = tiles.intCornerRT;
            so.ApplyModifiedProperties();

            var builder = mgrObj.GetComponent<RoomBuilderInput>();
            if (builder == null)
                builder = mgrObj.AddComponent<RoomBuilderInput>();

            var soBuilder = new SerializedObject(builder);
            soBuilder.FindProperty("_gridRenderer").objectReferenceValue = renderer;
            soBuilder.FindProperty("_camera").objectReferenceValue       = cam;
            soBuilder.ApplyModifiedProperties();

            // --- CatSpawner (lightweight service) ---
            var spawner = mgrObj.GetComponent<CatSpawner>();
            if (spawner == null)
                spawner = mgrObj.AddComponent<CatSpawner>();

            var soSpawner = new SerializedObject(spawner);
            soSpawner.FindProperty("_gridRenderer").objectReferenceValue = renderer;
            soSpawner.FindProperty("_fightCloudController").objectReferenceValue = cloudController;
            soSpawner.FindProperty("_handPetController").objectReferenceValue = handPetController;
            soSpawner.ApplyModifiedProperties();

            // --- Create CatBreedData SO assets ---
            var breedAssets = CreateBreedAssets(
                eurController, eur2Controller, eur3Controller,
                siamoisController, ragdollController, ragdoll2Controller,
                sibBlackController, sibWhiteController, chartrController,
                cleoController, aristoteController, napoleonController, orionController);

            // --- Create GameConfig SO asset ---
            var gameConfig = CreateOrLoadAsset<GameConfig>("Assets/_Project/Data/GameConfig.asset");

            // --- Create CatPersonalityConfig SO asset ---
            var personalityConfig = CreateOrLoadAsset<CatPersonalityConfig>("Assets/_Project/Data/CatPersonalityConfig.asset");

            // --- AppLifecycleManager (frame rate, pause, accelerometer) ---
            if (mgrObj.GetComponent<CatHotel.Core.AppLifecycleManager>() == null)
                mgrObj.AddComponent<CatHotel.Core.AppLifecycleManager>();

            // --- EconomyManager ---
            var economyMgr = mgrObj.GetComponent<EconomyManager>();
            if (economyMgr == null)
                economyMgr = mgrObj.AddComponent<EconomyManager>();

            var soEconomy = new SerializedObject(economyMgr);
            soEconomy.FindProperty("_config").objectReferenceValue = gameConfig;
            soEconomy.ApplyModifiedProperties();

            // --- ReputationManager ---
            var repMgr = mgrObj.GetComponent<ReputationManager>();
            if (repMgr == null)
                repMgr = mgrObj.AddComponent<ReputationManager>();

            // --- HotelManager ---
            var hotelMgr = mgrObj.GetComponent<HotelManager>();
            if (hotelMgr == null)
                hotelMgr = mgrObj.AddComponent<HotelManager>();

            var soHotel = new SerializedObject(hotelMgr);
            soHotel.FindProperty("_config").objectReferenceValue = gameConfig;
            soHotel.FindProperty("_personalityConfig").objectReferenceValue = personalityConfig;
            soHotel.FindProperty("_gridRenderer").objectReferenceValue = renderer;
            soHotel.FindProperty("_economy").objectReferenceValue = economyMgr;
            soHotel.FindProperty("_reputation").objectReferenceValue = repMgr;
            soHotel.FindProperty("_catSpawner").objectReferenceValue = spawner;

            var breedsProp = soHotel.FindProperty("_availableBreeds");
            breedsProp.arraySize = breedAssets.Length;
            for (int i = 0; i < breedAssets.Length; i++)
                breedsProp.GetArrayElementAtIndex(i).objectReferenceValue = breedAssets[i];
            soHotel.ApplyModifiedProperties();

            // --- HudManager ---
            var hudMgr = mgrObj.GetComponent<HudManager>();
            if (hudMgr == null)
                hudMgr = mgrObj.AddComponent<HudManager>();

            var soHud = new SerializedObject(hudMgr);
            soHud.FindProperty("_hotel").objectReferenceValue = hotelMgr;
            soHud.FindProperty("_economy").objectReferenceValue = economyMgr;
            soHud.FindProperty("_reputation").objectReferenceValue = repMgr;
            soHud.ApplyModifiedProperties();

            // AuthManager and AdManager are now created in the Boot scene (DontDestroyOnLoad).
            // When running Proto directly in editor, HotelManager.Start() handles fallback init.

            // --- RevenueBoostManager ---
            var boostObj = FindOrCreate("[RevenueBoostManager]");
            var boostMgr = boostObj.GetComponent<RevenueBoostManager>();
            if (boostMgr == null)
                boostMgr = boostObj.AddComponent<RevenueBoostManager>();

            var boostAdConfig = AssetDatabase.LoadAssetAtPath<AdConfig>(
                "Assets/_Project/Data/AdConfig.asset");
            if (boostAdConfig != null)
            {
                var soBoost = new SerializedObject(boostMgr);
                soBoost.FindProperty("_config").objectReferenceValue = boostAdConfig;
                soBoost.ApplyModifiedProperties();
            }

            // --- CatInfoPanel ---
            var catInfoPanel = mgrObj.GetComponent<CatInfoPanel>();
            if (catInfoPanel == null)
                catInfoPanel = mgrObj.AddComponent<CatInfoPanel>();

            var soCatInfo = new SerializedObject(catInfoPanel);
            soCatInfo.FindProperty("_hotel").objectReferenceValue = hotelMgr;
            var pensionIcon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/UI/Icons/Minimalist/pension.png");
            var shelterIcon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/UI/Icons/Minimalist/shelter.png");
            soCatInfo.FindProperty("_pensionIcon").objectReferenceValue = pensionIcon;
            soCatInfo.FindProperty("_shelterIcon").objectReferenceValue = shelterIcon;
            soCatInfo.ApplyModifiedProperties();

            // --- OptionsPanel ---
            var optionsPanel = mgrObj.GetComponent<OptionsPanel>();
            if (optionsPanel == null)
                optionsPanel = mgrObj.AddComponent<OptionsPanel>();

            // --- EndPensionPanel ---
            var endPensionPanel = mgrObj.GetComponent<EndPensionPanel>();
            if (endPensionPanel == null)
                endPensionPanel = mgrObj.AddComponent<EndPensionPanel>();

            var soEndPanel = new SerializedObject(endPensionPanel);
            var fullPickUpClip = AssetDatabase.LoadAssetAtPath<AudioClip>(
                "Assets/_Project/Audio/SFX/UI/UI_PlayerAction_FullPickUp.ogg");
            soEndPanel.FindProperty("_collectSfx").objectReferenceValue = fullPickUpClip;
            soEndPanel.ApplyModifiedProperties();

            // --- ObjectPlacement ---
            var objectPlacement = mgrObj.GetComponent<ObjectPlacement>();
            if (objectPlacement == null)
                objectPlacement = mgrObj.AddComponent<ObjectPlacement>();

            // Load ready/cancel sprite frames
            var readySprites = LoadAllSprites("Assets/_Project/Animations/UI/ready.png");
            var cancelSprites = LoadAllSprites("Assets/_Project/Animations/UI/cancel.png");

            var soPlacement = new SerializedObject(objectPlacement);
            soPlacement.FindProperty("_gridRenderer").objectReferenceValue = renderer;
            soPlacement.FindProperty("_economy").objectReferenceValue = economyMgr;

            var readyArray = soPlacement.FindProperty("_readyFrames");
            readyArray.arraySize = readySprites.Length;
            for (int i = 0; i < readySprites.Length; i++)
                readyArray.GetArrayElementAtIndex(i).objectReferenceValue = readySprites[i];

            var cancelArray = soPlacement.FindProperty("_cancelFrames");
            cancelArray.arraySize = cancelSprites.Length;
            for (int i = 0; i < cancelSprites.Length; i++)
                cancelArray.GetArrayElementAtIndex(i).objectReferenceValue = cancelSprites[i];

            soPlacement.ApplyModifiedProperties();

            // --- ObjectSelector ---
            var objectSelector = mgrObj.GetComponent<ObjectSelector>();
            if (objectSelector == null)
                objectSelector = mgrObj.AddComponent<ObjectSelector>();
            var soSelector = new SerializedObject(objectSelector);
            soSelector.FindProperty("_catSpawner").objectReferenceValue = spawner;
            soSelector.FindProperty("_objectPlacement").objectReferenceValue = objectPlacement;
            soSelector.ApplyModifiedProperties();

            // --- ShopPanel ---
            var shopPanel = mgrObj.GetComponent<ShopPanel>();
            if (shopPanel == null)
                shopPanel = mgrObj.AddComponent<ShopPanel>();
            var shopItemPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/_Project/Prefabs/UI/MainShopItem.prefab");
            var soShop = new SerializedObject(shopPanel);
            soShop.FindProperty("_itemPrefab").objectReferenceValue = shopItemPrefab;
            soShop.FindProperty("_economy").objectReferenceValue = economyMgr;
            soShop.FindProperty("_placement").objectReferenceValue = objectPlacement;

            // Wire all shop object assets
            const string ObjDir = "Assets/_Project/Data/Objects";
            var shopObjGuids = AssetDatabase.FindAssets("t:HotelObjectData", new[] { ObjDir });
            var shopObjArray = soShop.FindProperty("_allShopObjects");
            shopObjArray.arraySize = shopObjGuids.Length;
            for (int i = 0; i < shopObjGuids.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(shopObjGuids[i]);
                var objData = AssetDatabase.LoadAssetAtPath<HotelObjectData>(path);
                shopObjArray.GetArrayElementAtIndex(i).objectReferenceValue = objData;
            }

            soShop.ApplyModifiedProperties();

            // --- FloatingCoinView ---
            var coinView = mgrObj.GetComponent<FloatingCoinView>();
            if (coinView == null)
                coinView = mgrObj.AddComponent<FloatingCoinView>();

            var coinSprite = AssetDatabase.LoadAssetAtPath<Sprite>(
                "Assets/_Project/Art/UI/Icons/Minimalist/catCoin.png");
            var soCoinView = new SerializedObject(coinView);
            soCoinView.FindProperty("_economy").objectReferenceValue = economyMgr;
            soCoinView.FindProperty("_coinSprite").objectReferenceValue = coinSprite;
            soCoinView.FindProperty("_coinAnimController").objectReferenceValue = coinSpinController;
            soCoinView.FindProperty("_catSpawner").objectReferenceValue = spawner;
            soCoinView.FindProperty("_catInfoPanel").objectReferenceValue = catInfoPanel;

            // Wire coin SFX clips
            var coinClipsProp = soCoinView.FindProperty("_coinCollectClips");
            coinClipsProp.arraySize = 5;
            for (int i = 0; i < 5; i++)
            {
                var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(
                    $"Assets/_Project/Audio/SFX/UI/UI_PlayerAction_Coin-{(i + 1):D3}.ogg");
                coinClipsProp.GetArrayElementAtIndex(i).objectReferenceValue = clip;
            }
            var fullPickUp = AssetDatabase.LoadAssetAtPath<AudioClip>(
                "Assets/_Project/Audio/SFX/UI/UI_PlayerAction_FullPickUp.ogg");
            soCoinView.FindProperty("_fullPickUpClip").objectReferenceValue = fullPickUp;

            soCoinView.ApplyModifiedProperties();

            // Wire FloatingCoinView on HotelManager (created after initial soHotel pass)
            var soHotel2 = new SerializedObject(hotelMgr);
            soHotel2.FindProperty("_floatingCoinView").objectReferenceValue = coinView;
            soHotel2.FindProperty("_moodHappy").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/States/BULLE_EMOTIONS_Joyous.png");
            soHotel2.FindProperty("_moodEcstatic").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/States/BULLE_EMOTIONS_Very_Happy.png");
            soHotel2.FindProperty("_moodDepressed").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/States/BULLE_EMOTIONS_Upset.png");
            soHotel2.FindProperty("_moodAggressive").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/States/BULLE_EMOTIONS_Fights.png");
            soHotel2.FindProperty("_moodAngry").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/States/BULLE_EMOTIONS_Mad.png");
            soHotel2.FindProperty("_needHungry").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/States/BULLE_NEEDS_Hungry.png");
            soHotel2.FindProperty("_needThirsty").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/States/BULLE_NEEDS_Starving.png");
            soHotel2.FindProperty("_needTired").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/States/BULLE_NEEDS_Tired.png");
            soHotel2.FindProperty("_needBored").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/States/BULLE_NEEDS_Bored.png");
            soHotel2.FindProperty("_needDirty").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/States/BULLE_NEEDS_Dirty.png");
            soHotel2.ApplyModifiedProperties();

            // --- Interactive Hotel Objects ---
            PlaceHotelObjects();

            // --- Decorations (shelves, plants, lamps) ---
            PlaceDecorations();

            // --- Canvas ---
            var canvasObj = FindOrCreate("Canvas");
            var canvas = canvasObj.GetComponent<Canvas>();
            if (canvas == null)
                canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            var scaler = canvasObj.GetComponent<UnityEngine.UI.CanvasScaler>();
            if (scaler == null)
                scaler = canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            if (canvasObj.GetComponent<UnityEngine.UI.GraphicRaycaster>() == null)
                canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            // EventSystem
            if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                var esObj = new GameObject("EventSystem");
                esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                esObj.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            }

            // --- 4 UI zones (32px margin from screen edges) ---
            const float m = 32f; // margin in reference pixels

            // Top: 15% height, full width, inset 32px from top/left/right
            var zoneTop    = CreateUIZone(canvasObj, "ZoneTop",
                new Vector2(0f, 0.85f), new Vector2(1f, 1f),
                new Color(0.83f, 0.31f, 0.31f, 0.15f),
                new Vector2(m, 0f), new Vector2(-m, -m));

            // Left: 10% width, below top to bottom, inset 32px from left/bottom
            var zoneLeft   = CreateUIZone(canvasObj, "ZoneLeft",
                new Vector2(0f, 0f), new Vector2(0.10f, 0.85f),
                new Color(0.77f, 0.65f, 0.28f, 0.15f),
                new Vector2(m, m), new Vector2(0f, 0f));

            // Right: 20% width, below top to bottom, no margin (buttons go to edge)
            var zoneRight  = CreateUIZone(canvasObj, "ZoneRight",
                new Vector2(0.80f, 0f), new Vector2(1f, 0.85f),
                new Color(0.77f, 0.65f, 0.28f, 0.15f),
                new Vector2(0f, m), new Vector2(0f, 0f));

            // Center: fills remainder, inset 32px from bottom
            var zoneCenter = CreateUIZone(canvasObj, "ZoneCenter",
                new Vector2(0.10f, 0f), new Vector2(0.80f, 0.85f),
                new Color(0f, 0f, 0f, 0f),
                new Vector2(0f, m), new Vector2(0f, 0f));

            // --- ProtoUI ---
            var protoUI = mgrObj.GetComponent<ProtoUI>();
            if (protoUI == null)
                protoUI = mgrObj.AddComponent<ProtoUI>();

            var soUI = new SerializedObject(protoUI);
            soUI.FindProperty("_roomBuilder").objectReferenceValue      = builder;
            soUI.FindProperty("_catSpawner").objectReferenceValue       = spawner;
            soUI.FindProperty("_cameraController").objectReferenceValue =
                camObj.GetComponent<CameraController>();
            soUI.FindProperty("_zoneTop").objectReferenceValue     = zoneTop;
            soUI.FindProperty("_zoneLeft").objectReferenceValue   = zoneLeft;
            soUI.FindProperty("_zoneRight").objectReferenceValue  = zoneRight;
            soUI.FindProperty("_zoneCenter").objectReferenceValue = zoneCenter;
            soUI.ApplyModifiedProperties();

            // --- Mark dirty ---
            EditorUtility.SetDirty(camObj);
            EditorUtility.SetDirty(gridObj);
            EditorUtility.SetDirty(mgrObj);
            EditorUtility.SetDirty(canvasObj);

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        }

        private static void PlaceHotelObjects()
        {
            // Destroy previous container
            var old = GameObject.Find("HotelObjects");
            if (old != null) Object.DestroyImmediate(old);

            var root = new GameObject("HotelObjects");
            const string D = "Assets/_Project/Data/Objects";

            // ==================== ALL SHOP OBJECTS ====================
            // These ScriptableObjects are used both for scene placement AND shop display.

            // --- Beds (Sleep) ---
            CreateObjectAsset($"{D}/Obj_Bed.asset", "Lit",
                ObjectCategory.Sleep, 80, 1.3f, 6f, maxUsers: 1,
                spritePath: $"{ObjectsRoot}/Beds/BED.png", size: Vector2Int.one,
                selectedSpritePath: $"{ObjectsRoot}/Beds/BED_SELEC.png");
            CreateObjectAsset($"{D}/Obj_LuxuryBed.asset", "Lit de luxe",
                ObjectCategory.Sleep, 200, 1.8f, 6f, maxUsers: 1,
                spritePath: $"{ObjectsRoot}/Beds/LUXOUS_BED.png", size: new Vector2Int(2, 2),
                selectedSpritePath: $"{ObjectsRoot}/Beds/LUXOUS_BED_SELEC.png");

            // --- Pillows (Sleep) ---
            CreateObjectAsset($"{D}/Obj_Coussin.asset", "Coussin",
                ObjectCategory.Sleep, 40, 1f, 5f, maxUsers: 1,
                spritePath: $"{ObjectsRoot}/Pillows/COUSSIN.png", size: Vector2Int.one,
                selectedSpritePath: $"{ObjectsRoot}/Pillows/COUSSIN_SELEC.png");

            // --- Croquettes / Food (Hunger) ---
            CreateObjectAsset($"{D}/Obj_FoodBowl.asset", "Gamelle",
                ObjectCategory.Food, 25, 1f, 5f, maxUsers: 1,
                spritePath: $"{ObjectsRoot}/Food/FOOD_BOWL_Full.png", size: Vector2Int.one,
                visualScale: 0.5f,
                selectedSpritePath: $"{ObjectsRoot}/Food/FOOD_BOWL_SELEC.png");
            CreateObjectAsset($"{D}/Obj_FoodBowlVar02.asset", "Gamelle var. 2",
                ObjectCategory.Food, 30, 1.1f, 5f, maxUsers: 1,
                spritePath: $"{ObjectsRoot}/Food/FOOD_BOWL_Var_02_Full.png", size: Vector2Int.one,
                visualScale: 0.5f,
                selectedSpritePath: $"{ObjectsRoot}/Food/FOOD_BOWL_Var_02_SELEC.png");
            CreateObjectAsset($"{D}/Obj_FoodBowlVar03.asset", "Gamelle var. 3",
                ObjectCategory.Food, 30, 1.1f, 5f, maxUsers: 1,
                spritePath: $"{ObjectsRoot}/Food/FOOD_BOWL_Var_03_Full.png", size: Vector2Int.one,
                visualScale: 0.5f,
                selectedSpritePath: $"{ObjectsRoot}/Food/FOOD_BOWL_Var_03_SELEC.png");
            CreateObjectAsset($"{D}/Obj_FoodBowlVar04.asset", "Gamelle var. 4",
                ObjectCategory.Food, 35, 1.2f, 5f, maxUsers: 1,
                spritePath: $"{ObjectsRoot}/Food/FOOD_BOWL_Var_04_Fiull.png", size: Vector2Int.one,
                visualScale: 0.5f,
                selectedSpritePath: $"{ObjectsRoot}/Food/FOOD_BOWL_Var_04_SELEC.png");

            // --- Water (Thirst) ---
            CreateObjectAsset($"{D}/Obj_WaterBowl.asset", "Bol d'eau",
                ObjectCategory.Water, 25, 1f, 4f, maxUsers: 1,
                spritePath: $"{ObjectsRoot}/Water/WATER_BOWL_Full.png", size: Vector2Int.one,
                visualScale: 0.5f,
                selectedSpritePath: $"{ObjectsRoot}/Water/WATER_BOWL_SELEC.png");
            CreateObjectAsset($"{D}/Obj_WaterBowl04.asset", "Bol d'eau moderne",
                ObjectCategory.Water, 35, 1.2f, 4f, maxUsers: 1,
                spritePath: $"{ObjectsRoot}/Water/WATER_BOWL_04_Full.png", size: Vector2Int.one,
                visualScale: 0.5f,
                selectedSpritePath: $"{ObjectsRoot}/Water/WATER_BOWL_04_SELEC.png");
            CreateObjectAsset($"{D}/Obj_WaterBowlVar02.asset", "Bol d'eau var. 2",
                ObjectCategory.Water, 30, 1.1f, 4f, maxUsers: 1,
                spritePath: $"{ObjectsRoot}/Water/WATER_BOWL_Var_02_Full.png", size: Vector2Int.one,
                visualScale: 0.5f,
                selectedSpritePath: $"{ObjectsRoot}/Water/WATER_BOWL_Var_02_SELEC.png");
            CreateObjectAsset($"{D}/Obj_WaterBowlVar03.asset", "Bol d'eau var. 3",
                ObjectCategory.Water, 30, 1.1f, 4f, maxUsers: 1,
                spritePath: $"{ObjectsRoot}/Water/WATER_BOWL_Var_03_Full.png", size: Vector2Int.one,
                visualScale: 0.5f,
                selectedSpritePath: $"{ObjectsRoot}/Water/WATER_BOWL_Var_03_SELEC.png");

            // --- Toys (Play) ---
            CreateObjectAsset($"{D}/Obj_WoolBall.asset", "Balle de laine",
                ObjectCategory.Play, 20, 1f, 4f, maxUsers: 1,
                spritePath: $"{ObjectsRoot}/Toys/WOOL_BALL.png", size: Vector2Int.one,
                visualScale: 0.25f,
                selectedSpritePath: $"{ObjectsRoot}/Toys/WOOL_BALL_SELEC.png");
            CreateObjectAsset($"{D}/Obj_CatTree.asset", "Arbre à chat",
                ObjectCategory.Play, 150, 1.5f, 6f, maxUsers: 2,
                spritePath: $"{ObjectsRoot}/Toys/CATS_TREE.png", size: new Vector2Int(1, 2),
                selectedSpritePath: $"{ObjectsRoot}/Toys/CATS_TREE_SELEC.png");

            // --- Scratchers (Play) ---
            CreateObjectAsset($"{D}/Obj_Scratcher.asset", "Griffoir",
                ObjectCategory.Play, 50, 1.2f, 5f, maxUsers: 1,
                spritePath: $"{ObjectsRoot}/Scratchers/SCRATCHER.png", size: Vector2Int.one,
                selectedSpritePath: $"{ObjectsRoot}/Scratchers/SCRATCHER_SELEC.png");

            // --- Litters (Clean) ---
            CreateObjectAsset($"{D}/Obj_Litter.asset", "Litière",
                ObjectCategory.Clean, 35, 1f, 4f, maxUsers: 1,
                spritePath: $"{ObjectsRoot}/Litters/LITTER_BOX_Clean.png", size: Vector2Int.one,
                selectedSpritePath: $"{ObjectsRoot}/Litters/LITTER_BOX_Clean_SELEC.png");
            CreateObjectAsset($"{D}/Obj_LitterVar01.asset", "Litière var.",
                ObjectCategory.Clean, 45, 1.2f, 4f, maxUsers: 1,
                spritePath: $"{ObjectsRoot}/Litters/LITTER_BOX_Var_01_Clean.png", size: new Vector2Int(2, 2),
                visualScale: 0.75f,
                selectedSpritePath: $"{ObjectsRoot}/Litters/LITTER_BOX_Var_01_Clean_SELEC.png");

            // --- Decorations: Frames (wall-mount) ---
            CreateObjectAsset($"{D}/Obj_Frame01.asset", "Cadre 1",
                ObjectCategory.Decoration, 60, 0f, 0f, maxUsers: 0,
                spritePath: $"{ObjectsRoot}/Deco/FRAME_01.png", size: Vector2Int.one, wallMount: true,
                selectedSpritePath: $"{ObjectsRoot}/Deco/FRAME_01_SELEC.png");
            CreateObjectAsset($"{D}/Obj_Frame02.asset", "Cadre 2",
                ObjectCategory.Decoration, 60, 0f, 0f, maxUsers: 0,
                spritePath: $"{ObjectsRoot}/Deco/FRAME_02.png", size: Vector2Int.one, wallMount: true,
                selectedSpritePath: $"{ObjectsRoot}/Deco/FRAME_02_SELEC.png");
            CreateObjectAsset($"{D}/Obj_Frame03.asset", "Cadre 3",
                ObjectCategory.Decoration, 60, 0f, 0f, maxUsers: 0,
                spritePath: $"{ObjectsRoot}/Deco/FRAME_03.png", size: Vector2Int.one, wallMount: true,
                selectedSpritePath: $"{ObjectsRoot}/Deco/FRAME_03_SELEC.png");
            CreateObjectAsset($"{D}/Obj_FramePainting.asset", "Peinture",
                ObjectCategory.Decoration, 120, 0f, 0f, maxUsers: 0,
                spritePath: $"{ObjectsRoot}/Deco/FRAME_PAINTING.png", size: Vector2Int.one, wallMount: true,
                selectedSpritePath: $"{ObjectsRoot}/Deco/PAINTING_SELEC.png");

            // --- Decorations: Lamps ---
            CreateObjectAsset($"{D}/Obj_LampBig.asset", "Grande lampe",
                ObjectCategory.Decoration, 100, 0f, 0f, maxUsers: 0,
                spritePath: $"{ObjectsRoot}/Deco/LAMP_BIG.png", size: Vector2Int.one);

            // --- Decorations: Tables ---
            CreateObjectAsset($"{D}/Obj_TableCoffee.asset", "Table basse",
                ObjectCategory.Decoration, 80, 0f, 0f, maxUsers: 0,
                spritePath: $"{ObjectsRoot}/Deco/TABLE_COFFEE_TABLE.png", size: Vector2Int.one, visualScale: 2f,
                selectedSpritePath: $"{ObjectsRoot}/Deco/COFFEE_TABLE_SELEC.png", isTable: true);
            CreateObjectAsset($"{D}/Obj_TableDrawer.asset", "Commode",
                ObjectCategory.Decoration, 90, 0f, 0f, maxUsers: 0,
                spritePath: $"{ObjectsRoot}/Deco/TABLE_DRAWER_Small.png", size: Vector2Int.one, visualScale: 1f,
                selectedSpritePath: $"{ObjectsRoot}/Deco/DRAWER_Small_SELEC.png", isTable: true);

            // --- Decorations: Plants ---
            CreateObjectAsset($"{D}/Obj_PlantBig.asset", "Grande plante",
                ObjectCategory.Decoration, 50, 0f, 0f, maxUsers: 0,
                spritePath: $"{ObjectsRoot}/Deco/PLANT_BIG.png", size: Vector2Int.one,
                selectedSpritePath: $"{ObjectsRoot}/Deco/PLANT_BIG_SELEC.png");
            CreateObjectAsset($"{D}/Obj_PlantSmall.asset", "Petite plante",
                ObjectCategory.Decoration, 30, 0f, 0f, maxUsers: 0,
                spritePath: $"{ObjectsRoot}/Deco/PLANT_SMALL.png", size: Vector2Int.one);

            // --- Decorations: Shelves (wall-mount) ---
            CreateObjectAsset($"{D}/Obj_Shelf0.asset", "Étagère",
                ObjectCategory.Decoration, 70, 0f, 0f, maxUsers: 0,
                spritePath: $"{ObjectsRoot}/Deco/SHELF_0.png", size: new Vector2Int(2, 1), wallMount: true,
                selectedSpritePath: $"{ObjectsRoot}/Deco/SHELF_SELEC.png");
            CreateObjectAsset($"{D}/Obj_ShelfVar01.asset", "Étagère var.",
                ObjectCategory.Decoration, 70, 0f, 0f, maxUsers: 0,
                spritePath: $"{ObjectsRoot}/Deco/SHELF_Var_01.png", size: new Vector2Int(2, 1), wallMount: true,
                selectedSpritePath: $"{ObjectsRoot}/Deco/SHELF_Var_01_SELEC.png");

            // --- Decorations: Aquarium (static sprite only — spritesheet removed) ---
            CreateObjectAsset($"{D}/Obj_Aquarium.asset", "Aquarium",
                ObjectCategory.Decoration, 200, 0f, 0f, maxUsers: 0,
                spritePath: $"{ObjectsRoot}/Deco/Aquarium.png", size: Vector2Int.one,
                visualScale: 2f, requiresTable: true);

            // --- Carpets ---
            CreateObjectAsset($"{D}/Obj_CarpetConfort.asset", "Tapis Confort",
                ObjectCategory.Decoration, 120, 0f, 0f, maxUsers: 0,
                spritePath: $"{ObjectsRoot}/Carpets/CARPET_CONFORT.png", size: new Vector2Int(2, 2),
                selectedSpritePath: $"{ObjectsRoot}/Carpets/CARPET_CONFORT_SELEC.png");
            CreateObjectAsset($"{D}/Obj_CarpetPlay.asset", "Tapis Jeu",
                ObjectCategory.Decoration, 120, 0f, 0f, maxUsers: 0,
                spritePath: $"{ObjectsRoot}/Carpets/CARPET_PLAY.png", size: new Vector2Int(2, 2),
                selectedSpritePath: $"{ObjectsRoot}/Carpets/CARPET_PLAY_SELEC.png");
            var carpetCosmicCtrl = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(CarpetCosmicControllerPath);
            CreateObjectAsset($"{D}/Obj_CarpetCosmic.asset", "Tapis Cosmique",
                ObjectCategory.Decoration, 250, 0f, 0f, maxUsers: 0,
                spritePath: $"{ObjectsRoot}/Carpets/CARPET_COSMIC.png", size: new Vector2Int(2, 2),
                animController: carpetCosmicCtrl);

            // No default objects placed — player buys everything from the shop

            EditorUtility.SetDirty(root);
            AssetDatabase.SaveAssets();
            Debug.Log("[Setup] Created all shop object assets + placed default hotel objects.");
        }

        private static HotelObjectData CreateObjectAsset(string path, string displayName,
            ObjectCategory category, int cost, float efficiency, float useDuration,
            int maxUsers = 1, string spritePath = null, Vector2Int? size = null,
            float visualScale = 1f, bool wallMount = false, bool requiresTable = false,
            RuntimeAnimatorController animController = null, Sprite iconOverride = null,
            string selectedSpritePath = null, bool isTable = false)
        {
            var data = CreateOrLoadAsset<HotelObjectData>(path);
            var so = new SerializedObject(data);
            so.FindProperty("displayName").stringValue = displayName;
            so.FindProperty("category").enumValueIndex = (int)category;
            so.FindProperty("cost").intValue = cost;
            so.FindProperty("efficiency").floatValue = efficiency;
            so.FindProperty("useDuration").floatValue = useDuration;
            so.FindProperty("maxUsers").intValue = maxUsers;
            so.FindProperty("visualScale").floatValue = visualScale;
            so.FindProperty("wallMount").boolValue = wallMount;
            so.FindProperty("requiresTable").boolValue = requiresTable;
            so.FindProperty("isTable").boolValue = isTable;
            so.FindProperty("worldAnimController").objectReferenceValue = animController;

            if (spritePath != null)
            {
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
                so.FindProperty("icon").objectReferenceValue = iconOverride != null ? iconOverride : sprite;
                so.FindProperty("worldSprite").objectReferenceValue = sprite;
            }
            else if (iconOverride != null)
            {
                so.FindProperty("icon").objectReferenceValue = iconOverride;
            }

            if (size.HasValue)
            {
                var sizeProp = so.FindProperty("size");
                sizeProp.vector2IntValue = size.Value;
            }

            if (selectedSpritePath != null)
            {
                var selSprite = AssetDatabase.LoadAssetAtPath<Sprite>(selectedSpritePath);
                so.FindProperty("selectedSprite").objectReferenceValue = selSprite;
            }

            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(data);
            return data;
        }

        private static void PlaceHotelObject(GameObject parent, string name, string spritePath,
            Vector2Int gridPos, HotelObjectData data)
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            if (sprite == null)
            {
                Debug.LogWarning($"[Setup] Object sprite not found: {spritePath}");
                return;
            }

            var go = new GameObject(name);
            go.transform.SetParent(parent.transform, false);
            go.transform.position = new Vector3(gridPos.x + 0.5f, gridPos.y + 0.5f, 0f);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = 5;

            // Scale object to fit within its grid size, applying visualScale
            Vector2Int objSize = data != null ? data.size : Vector2Int.one;
            float vScale = data != null ? data.visualScale : 1f;
            float spriteW = sprite.bounds.size.x;
            float spriteH = sprite.bounds.size.y;
            if (spriteW > 0f && spriteH > 0f)
            {
                float scale = Mathf.Min(objSize.x / spriteW, objSize.y / spriteH) * vScale;
                go.transform.localScale = new Vector3(scale, scale, 1f);
            }

            var hotelObj = go.AddComponent<HotelObject>();
            // Use SerializedObject to set private _data field
            var so = new SerializedObject(hotelObj);
            so.FindProperty("_data").objectReferenceValue = data;
            so.ApplyModifiedProperties();

            // Store grid position (Init is called at runtime via OnEnable, but we set it here)
            hotelObj.Init(data, gridPos);
        }

        private static void PlaceDecorations()
        {
            // Destroy previous decorations container
            var old = GameObject.Find("Decorations");
            if (old != null) Object.DestroyImmediate(old);

            // Empty room at start — player places decorations via shop
            var root = new GameObject("Decorations");
            EditorUtility.SetDirty(root);
        }

        private static void PlaceSprite(GameObject parent, string name, string spritePath,
            Vector3 position, int sortingOrder)
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            if (sprite == null)
            {
                Debug.LogWarning($"Decoration sprite not found: {spritePath}");
                return;
            }

            var go = new GameObject(name);
            go.transform.SetParent(parent.transform, false);
            go.transform.position = position;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = sortingOrder;

            // Clamp so decoration fits within 1 tile max
            float spriteW = sprite.bounds.size.x;
            float spriteH = sprite.bounds.size.y;
            if (spriteW > 1f || spriteH > 1f)
            {
                float scale = Mathf.Min(1f / spriteW, 1f / spriteH);
                go.transform.localScale = new Vector3(scale, scale, 1f);
            }
        }


        private static Tilemap CreateTilemapChild(GameObject parent, string name, int sortOrder)
        {
            var existing = parent.transform.Find(name);
            GameObject obj;
            if (existing != null)
            {
                obj = existing.gameObject;
            }
            else
            {
                obj = new GameObject(name);
                obj.transform.SetParent(parent.transform, false);
            }

            var tm = obj.GetComponent<Tilemap>();
            if (tm == null)
                tm = obj.AddComponent<Tilemap>();

            var tr = obj.GetComponent<TilemapRenderer>();
            if (tr == null)
                tr = obj.AddComponent<TilemapRenderer>();
            tr.sortingOrder = sortOrder;

            return tm;
        }

        private static RectTransform CreateUIZone(GameObject canvasObj, string name,
            Vector2 anchorMin, Vector2 anchorMax, Color bgColor,
            Vector2 offsetMin, Vector2 offsetMax)
        {
            var existing = canvasObj.transform.Find(name);
            GameObject obj;
            if (existing != null)
            {
                obj = existing.gameObject;
            }
            else
            {
                obj = new GameObject(name, typeof(RectTransform));
                obj.transform.SetParent(canvasObj.transform, false);
            }

            var rect = obj.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;

            var img = obj.GetComponent<UnityEngine.UI.Image>();
            if (img == null)
                img = obj.AddComponent<UnityEngine.UI.Image>();
            img.color = bgColor;
            img.raycastTarget = bgColor.a > 0.01f;

            return rect;
        }

        private static GameObject FindOrCreate(string name)
        {
            var obj = GameObject.Find(name);
            if (obj == null)
                obj = new GameObject(name);
            return obj;
        }

        // --- ScriptableObject asset creation ---

        /// <summary>Load all sub-sprites from a multi-sprite texture asset.</summary>
        private static Sprite[] LoadAllSprites(string path)
        {
            var assets = AssetDatabase.LoadAllAssetsAtPath(path);
            var sprites = new System.Collections.Generic.List<Sprite>();
            foreach (var a in assets)
            {
                if (a is Sprite s)
                    sprites.Add(s);
            }
            // Sort by name to keep frame order
            sprites.Sort((a, b) => string.Compare(a.name, b.name, System.StringComparison.Ordinal));
            return sprites.ToArray();
        }

        private static T CreateOrLoadAsset<T>(string path) where T : ScriptableObject
        {
            var existing = AssetDatabase.LoadAssetAtPath<T>(path);
            if (existing != null) return existing;

            // Ensure directory exists
            string dir = System.IO.Path.GetDirectoryName(path);
            if (!AssetDatabase.IsValidFolder(dir))
            {
                string[] parts = dir.Replace("\\", "/").Split('/');
                string current = parts[0];
                for (int i = 1; i < parts.Length; i++)
                {
                    string next = current + "/" + parts[i];
                    if (!AssetDatabase.IsValidFolder(next))
                        AssetDatabase.CreateFolder(current, parts[i]);
                    current = next;
                }
            }

            var asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }

        private static CatBreedData CreateBreedAsset(string assetPath, string breedName,
            string spritesRoot, string frontFile, string rightFile, string backFile,
            RuntimeAnimatorController controller,
            float demand = 1f, float hTrait = 1f, float sTrait = 1f, float pTrait = 1f, float cTrait = 1f,
            float size = 1f, float speed = 1f, int minRep = 0, bool aggressive = false,
            string breedNamePlural = null)
        {
            var breed = CreateOrLoadAsset<CatBreedData>(assetPath);
            var so = new SerializedObject(breed);

            so.FindProperty("breedName").stringValue = breedName;
            so.FindProperty("breedNamePlural").stringValue = breedNamePlural ?? (breedName + "s");
            so.FindProperty("minReputation").intValue = minRep;
            so.FindProperty("isAggressive").boolValue = aggressive;
            so.FindProperty("frontSprite").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Sprite>($"{spritesRoot}/{frontFile}");
            so.FindProperty("rightSprite").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Sprite>($"{spritesRoot}/{rightFile}");
            so.FindProperty("backSprite").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Sprite>($"{spritesRoot}/{backFile}");
            so.FindProperty("controller").objectReferenceValue = controller;
            so.FindProperty("demandMultiplier").floatValue = demand;
            so.FindProperty("hungerTrait").floatValue = hTrait;
            so.FindProperty("sleepTrait").floatValue = sTrait;
            so.FindProperty("playTrait").floatValue = pTrait;
            so.FindProperty("cleanTrait").floatValue = cTrait;
            so.FindProperty("size").floatValue = size;
            so.FindProperty("speed").floatValue = speed;

            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(breed);
            return breed;
        }

        private static CatBreedData[] CreateBreedAssets(
            RuntimeAnimatorController eurCtrl, RuntimeAnimatorController eur2Ctrl,
            RuntimeAnimatorController eur3Ctrl, RuntimeAnimatorController siamoisCtrl,
            RuntimeAnimatorController ragdollCtrl, RuntimeAnimatorController ragdoll2Ctrl,
            RuntimeAnimatorController sibBlackCtrl, RuntimeAnimatorController sibWhiteCtrl,
            RuntimeAnimatorController chartrCtrl,
            RuntimeAnimatorController cleoCtrl, RuntimeAnimatorController aristoteCtrl,
            RuntimeAnimatorController napoleonCtrl, RuntimeAnimatorController orionCtrl)
        {
            const string D = "Assets/_Project/Data/Breeds";

            // Europeen variants (all minRep 0, balanced stats)
            var eur1 = CreateBreedAsset($"{D}/Breed_Europeen.asset", "Européen",
                CatSpritesRoot, "CAT_EUR_FRONT.png", "CAT_EUR_RIGHT.png", "CAT_EUR_BACK.png",
                eurCtrl);

            var eur2 = CreateBreedAsset($"{D}/Breed_Europeen2.asset", "Européen",
                Eur2SpritesRoot, "CAT_EUR_02_FRONT.png", "CAT_EUR_02_RIGHT.png", "CAT_EUR_02_BACK.png",
                eur2Ctrl);

            var eur3 = CreateBreedAsset($"{D}/Breed_Europeen3.asset", "Européen",
                Eur3SpritesRoot, "CAT_EUR_03_FRONT.png", "CAT_EUR_03_RIGHT.png", "CAT_EUR_03_BACK.png",
                eur3Ctrl);

            // Siamois (GDD: Play +50%, demand 1.2, speed 1.2, size 0.9, minRep 2)
            var siam = CreateBreedAsset($"{D}/Breed_Siamois.asset", "Siamois",
                SiamoisSpritesRoot, "CAT_SIAMESE_FRONT.png", "CAT_SIAMESE_RIGHT.png", "CAT_SIAMESE_BACK.png",
                siamoisCtrl,
                demand: 1.2f, pTrait: 1.5f, size: 0.9f, speed: 1.2f, minRep: 2,
                breedNamePlural: "Siamois");

            // Ragdoll (GDD: Sleep +30%, demand 1.1, speed 0.8, size 1.1, minRep 1)
            var ragdoll = CreateBreedAsset($"{D}/Breed_Ragdoll.asset", "Ragdoll",
                RagdollSpritesRoot, "CAT_RD_FRONT.png", "CAT_RD_RIGHT.png", "CAT_RD_BACK.png",
                ragdollCtrl,
                demand: 1.1f, sTrait: 1.3f, size: 1.1f, speed: 0.8f, minRep: 1);

            // Ragdoll 2 (variant, same breed stats as Ragdoll)
            var ragdoll2 = CreateBreedAsset($"{D}/Breed_Ragdoll2.asset", "Ragdoll",
                Ragdoll2SpritesRoot, "CAT_RD_02_FRONT.png", "CAT_RD_02_RIGHT.png", "CAT_RD_02_BACK.png",
                ragdoll2Ctrl,
                demand: 1.1f, sTrait: 1.3f, size: 1.1f, speed: 0.8f, minRep: 1);

            // Siberian Black (thick fur, sturdy, GDD: Hunger +30%, demand 1.1, speed 0.9, size 1.15)
            var sibBlack = CreateBreedAsset($"{D}/Breed_SiberienBlack.asset", "Sibérien",
                SibBlackSpritesRoot, "CAT_SIB_01_FRONT.png", "CAT_SIB_01_RIGHT.png", "CAT_SIB_01_BACK.png",
                sibBlackCtrl,
                demand: 1.1f, hTrait: 1.3f, size: 1.15f, speed: 0.9f, minRep: 0);

            // Siberian White (variant, same breed stats)
            var sibWhite = CreateBreedAsset($"{D}/Breed_SiberienWhite.asset", "Sibérien",
                SibWhiteSpritesRoot, "CAT_SIB_FRONT.png", "CAT_SIB_RIGHT.png", "CAT_SIB_BACK.png",
                sibWhiteCtrl,
                demand: 1.1f, hTrait: 1.3f, size: 1.15f, speed: 0.9f, minRep: 0);

            // Wire special cat data on Europeen breed (Aristote)
            var soEur = new SerializedObject(eur1);
            soEur.FindProperty("hasSpecialVariant").boolValue = true;
            soEur.FindProperty("specialName").stringValue = "Aristote";
            soEur.FindProperty("specialChance").floatValue = 0.08f;
            soEur.FindProperty("specialRevenueMult").floatValue = 2f;
            soEur.FindProperty("specialDemandMult").floatValue = 1.5f;
            soEur.FindProperty("specialFrontSprite").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Sprite>($"{AristoteSpritesRoot}/CAT_EUR_Aristote_FRONT.png");
            soEur.FindProperty("specialRightSprite").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Sprite>($"{AristoteSpritesRoot}/CAT_EUR_Aristote_RIGHT.png");
            soEur.FindProperty("specialBackSprite").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Sprite>($"{AristoteSpritesRoot}/CAT_EUR_Aristote_BACK.png");
            soEur.FindProperty("specialController").objectReferenceValue = aristoteCtrl;
            soEur.ApplyModifiedProperties();

            // Wire Cleo as special on Siamois
            var soSiam = new SerializedObject(siam);
            soSiam.FindProperty("hasSpecialVariant").boolValue = true;
            soSiam.FindProperty("specialName").stringValue = "Cléo";
            soSiam.FindProperty("specialChance").floatValue = 0.07f;
            soSiam.FindProperty("specialRevenueMult").floatValue = 2.2f;
            soSiam.FindProperty("specialDemandMult").floatValue = 1.6f;
            soSiam.FindProperty("specialFrontSprite").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Sprite>($"{CleoSpritesRoot}/CAT_EUR_Cleo_FRONT.png");
            soSiam.FindProperty("specialRightSprite").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Sprite>($"{CleoSpritesRoot}/CAT_EUR_Cleo_RIGHT.png");
            soSiam.FindProperty("specialBackSprite").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Sprite>($"{CleoSpritesRoot}/CAT_EUR_Cleo_BACK.png");
            soSiam.FindProperty("specialController").objectReferenceValue = cleoCtrl;
            soSiam.ApplyModifiedProperties();

            // Chartreux (GDD: Dominant, Équilibré, demand 1.6, size 1.1, speed 1.0, Aggressive, minRep 8)
            var chartr = CreateBreedAsset($"{D}/Breed_Chartreux.asset", "Chartreux",
                ChartrSpritesRoot, "CAT_CHARTREU_FRONT.png", "CAT_CHARTREU_RIGHT.png", "CAT_CHARTREU_BACK.png",
                chartrCtrl,
                demand: 1.6f, size: 1.1f, speed: 1f, minRep: 8, aggressive: true,
                breedNamePlural: "Chartreux");

            // Wire Napoleon as special on Chartreux (GDD: 4%, rev 3.2, demand 2.3)
            var soChartr = new SerializedObject(chartr);
            soChartr.FindProperty("hasSpecialVariant").boolValue = true;
            soChartr.FindProperty("specialName").stringValue = "Napoléon";
            soChartr.FindProperty("specialChance").floatValue = 0.04f;
            soChartr.FindProperty("specialRevenueMult").floatValue = 3.2f;
            soChartr.FindProperty("specialDemandMult").floatValue = 2.3f;
            soChartr.FindProperty("specialFrontSprite").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Sprite>($"{NapoleonSpritesRoot}/CAT_CHARTREU_Napoléon_FRONT.png");
            soChartr.FindProperty("specialRightSprite").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Sprite>($"{NapoleonSpritesRoot}/CAT_CHARTREU_Napoléon_RIGHT.png");
            soChartr.FindProperty("specialBackSprite").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Sprite>($"{NapoleonSpritesRoot}/CAT_CHARTREU_Napoléon_BACK.png");
            soChartr.FindProperty("specialController").objectReferenceValue = napoleonCtrl;
            soChartr.ApplyModifiedProperties();

            // Wire Orion as special on Ragdoll (GDD: 10%, rev 2.5, demand 1.8)
            var soRd = new SerializedObject(ragdoll);
            soRd.FindProperty("hasSpecialVariant").boolValue = true;
            soRd.FindProperty("specialName").stringValue = "Orion";
            soRd.FindProperty("specialChance").floatValue = 0.10f;
            soRd.FindProperty("specialRevenueMult").floatValue = 2.5f;
            soRd.FindProperty("specialDemandMult").floatValue = 1.8f;
            soRd.FindProperty("specialFrontSprite").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Sprite>($"{OrionSpritesRoot}/CAT_RD_Orion_FRONT.png");
            soRd.FindProperty("specialRightSprite").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Sprite>($"{OrionSpritesRoot}/CAT_RD_Orion_RIGHT.png");
            soRd.FindProperty("specialBackSprite").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Sprite>($"{OrionSpritesRoot}/CAT_RD_Orion_BACK.png");
            soRd.FindProperty("specialController").objectReferenceValue = orionCtrl;
            soRd.ApplyModifiedProperties();

            AssetDatabase.SaveAssets();
            Debug.Log("[Setup] Created breed SO assets.");

            return new[] { eur1, eur2, eur3, siam, ragdoll, ragdoll2, sibBlack, sibWhite, chartr };
        }
    }
}
