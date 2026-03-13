using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEditor.Animations;
using CatHotel.Grid;
using CatHotel.Input;
using CatHotel.Cats;
using CatHotel.UI;
using CatHotel.Core;
using CatHotel.Economy;
using CatHotel.Hotel;

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

        private const string CleoSpritesRoot = "Assets/_Project/Art/SpecialCats/Cleo";
        private const string CleoAnimRoot = CleoSpritesRoot;
        private const string CleoControllerPath = CleoSpritesRoot + "/CatCleo.controller";

        private const string AristoteSpritesRoot = "Assets/_Project/Art/SpecialCats/Aristote";
        private const string AristoteAnimRoot = AristoteSpritesRoot;
        private const string AristoteControllerPath = AristoteSpritesRoot + "/CatAristote.controller";

        private const string CloudRoot = "Assets/_Project/Art/Effects/Combat";
        private const string CloudControllerPath = CloudRoot + "/FightCloud.controller";

        private static readonly (string file, string prefix, string state, int frames, float fps)[] CloudAnimConfigs =
        {
            ("fighting_cloud.png", "fighting_cloud", "FightCloud", 6, 6f),
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
            ProcessAnimConfigs(CleoAnimRoot, CleoAnimConfigs);
            ProcessAnimConfigs(AristoteAnimRoot, AristoteAnimConfigs);
            ProcessAnimConfigs(CloudRoot, CloudAnimConfigs);
            ProcessAnimConfigs(PettingRoot, HandPetAnimConfigs);
            AssetDatabase.Refresh();

            var tiles = CreateTileAssets();
            var eurController = CreateAnimController(CatControllerPath, AnimRoot, AnimConfigs);
            var eur2Controller = CreateAnimController(Eur2ControllerPath, Eur2AnimRoot, Eur2AnimConfigs);
            var eur3Controller = CreateAnimController(Eur3ControllerPath, Eur3AnimRoot, Eur3AnimConfigs);
            var siamoisController = CreateAnimController(SiamoisControllerPath, SiamoisAnimRoot, SiamoisAnimConfigs);
            var cleoController = CreateAnimController(CleoControllerPath, CleoAnimRoot, CleoAnimConfigs);
            var aristoteController = CreateAnimController(AristoteControllerPath, AristoteAnimRoot, AristoteAnimConfigs);
            var cloudController = CreateAnimController(CloudControllerPath, CloudRoot, CloudAnimConfigs);
            var handPetController = CreateAnimController(HandPetControllerPath, PettingRoot, HandPetAnimConfigs);
            BuildSceneHierarchy(tiles, eurController, eur2Controller, eur3Controller, siamoisController, cleoController, aristoteController, cloudController, handPetController);
            int total = AnimConfigs.Length + Eur2AnimConfigs.Length + Eur3AnimConfigs.Length
                      + SiamoisAnimConfigs.Length + CleoAnimConfigs.Length + AristoteAnimConfigs.Length + CloudAnimConfigs.Length + HandPetAnimConfigs.Length;
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
                $"{ObjectsRoot}/Beds/BED.png",
                $"{ObjectsRoot}/Beds/COUSSIN.png",
                $"{ObjectsRoot}/Beds/LUXOUS_BED.png",
            };
            foreach (var s in objectSprites)
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
                // Cleo (Special)
                $"{CleoSpritesRoot}/CAT_EUR_Cleo_FRONT.png",
                $"{CleoSpritesRoot}/CAT_EUR_Cleo_RIGHT.png",
                $"{CleoSpritesRoot}/CAT_EUR_Cleo_BACK.png",
            };

            foreach (string path in paths)
                ConfigureSprite(path, 200, FilterMode.Bilinear);
        }

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
            importer.textureCompression = TextureImporterCompression.Uncompressed;

            importer.GetSourceTextureWidthAndHeight(out int texW, out int texH);

            // Auto-set maxTextureSize to next power of 2 >= texture width (min 2048)
            int maxSize = 2048;
            while (maxSize < texW)
                maxSize *= 2;
            importer.maxTextureSize = maxSize;

            int frameW = texW / frameCount;

            var metas = new SpriteMetaData[frameCount];
            for (int i = 0; i < frameCount; i++)
            {
                metas[i] = new SpriteMetaData
                {
                    name = $"{namePrefix}_{i}",
                    rect = new Rect(i * frameW, 0, frameW, texH),
                    alignment = (int)SpriteAlignment.Center,
                    pivot = new Vector2(0.5f, 0.5f)
                };
            }
            importer.spritesheet = metas;
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
                .OrderBy(s => s.name)
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
            importer.spritePixelsPerUnit = ppu;
            importer.filterMode = filter;
            importer.textureCompression = TextureImporterCompression.Uncompressed;

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
            RuntimeAnimatorController cleoController,
            RuntimeAnimatorController aristoteController,
            RuntimeAnimatorController cloudController,
            RuntimeAnimatorController handPetController)
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
            camObj.transform.position = new Vector3(12f, 8f, -10f);

            var camCtrl = camObj.GetComponent<CameraController>();
            if (camCtrl == null)
                camCtrl = camObj.AddComponent<CameraController>();

            // Force camera bounds for half-size grid
            var camSo = new SerializedObject(camCtrl);
            camSo.FindProperty("_minOrthoSize").floatValue = 1.5f;
            camSo.FindProperty("_maxOrthoSize").floatValue = 5f;
            camSo.FindProperty("_gridMax").vector2Value = new Vector2(24f, 16f);
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
                siamoisController, cleoController, aristoteController);

            // --- Create GameConfig SO asset ---
            var gameConfig = CreateOrLoadAsset<GameConfig>("Assets/_Project/Data/GameConfig.asset");

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

            // --- CatInfoPanel ---
            var catInfoPanel = mgrObj.GetComponent<CatInfoPanel>();
            if (catInfoPanel == null)
                catInfoPanel = mgrObj.AddComponent<CatInfoPanel>();

            var soCatInfo = new SerializedObject(catInfoPanel);
            soCatInfo.FindProperty("_hotel").objectReferenceValue = hotelMgr;
            soCatInfo.ApplyModifiedProperties();

            // --- OptionsPanel ---
            var optionsPanel = mgrObj.GetComponent<OptionsPanel>();
            if (optionsPanel == null)
                optionsPanel = mgrObj.AddComponent<OptionsPanel>();

            // --- EndPensionPanel ---
            var endPensionPanel = mgrObj.GetComponent<EndPensionPanel>();
            if (endPensionPanel == null)
                endPensionPanel = mgrObj.AddComponent<EndPensionPanel>();

            // --- FloatingCoinView ---
            var coinView = mgrObj.GetComponent<FloatingCoinView>();
            if (coinView == null)
                coinView = mgrObj.AddComponent<FloatingCoinView>();

            var coinSprite = AssetDatabase.LoadAssetAtPath<Sprite>(
                "Assets/_Project/Art/UI/Icons/Minimalist/catCoin.png");
            var soCoinView = new SerializedObject(coinView);
            soCoinView.FindProperty("_economy").objectReferenceValue = economyMgr;
            soCoinView.FindProperty("_coinSprite").objectReferenceValue = coinSprite;
            soCoinView.FindProperty("_catSpawner").objectReferenceValue = spawner;
            soCoinView.FindProperty("_catInfoPanel").objectReferenceValue = catInfoPanel;
            soCoinView.ApplyModifiedProperties();

            // --- Interactive Hotel Objects ---
            PlaceHotelObjects();

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

            // --- Cleanup stale decorations ---
            var oldDeco = GameObject.Find("Decorations");
            if (oldDeco != null) Object.DestroyImmediate(oldDeco);

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

            // Central room: x=2-21, y=2-13 (floor area)
            // Place objects spread around the room — 1 per need type + 1 extra food

            // Food bowls — satisfies Hunger (maxUsers=3 each to avoid starvation)
            var foodData = CreateObjectAsset($"{D}/Obj_FoodBowl.asset", "Gamelle",
                ObjectCategory.Food, 30, 1f, 5f, maxUsers: 3);
            PlaceHotelObject(root, "FoodBowl_1", $"{ObjectsRoot}/Food/FOOD_BOWL_Full.png",
                new Vector2Int(5, 4), foodData);
            PlaceHotelObject(root, "FoodBowl_2", $"{ObjectsRoot}/Food/FOOD_BOWL_Full.png",
                new Vector2Int(17, 10), foodData);

            // Litter box — satisfies Clean
            var litterData = CreateObjectAsset($"{D}/Obj_Litter.asset", "Litière",
                ObjectCategory.Clean, 35, 1f, 4f, maxUsers: 2);
            PlaceHotelObject(root, "Litter_1", $"{ObjectsRoot}/Litter/LITTER_BOX_Clean.png",
                new Vector2Int(19, 3), litterData);

            // Wool ball — satisfies Play
            var ballData = CreateObjectAsset($"{D}/Obj_WoolBall.asset", "Balle de laine",
                ObjectCategory.Play, 20, 1f, 4f, maxUsers: 2);
            PlaceHotelObject(root, "WoolBall_1", $"{ObjectsRoot}/Toys/WOOL_BALL.png",
                new Vector2Int(10, 11), ballData);

            // Bed — satisfies Sleep
            var bedData = CreateObjectAsset($"{D}/Obj_Bed.asset", "Lit",
                ObjectCategory.Sleep, 80, 1.3f, 6f, maxUsers: 2);
            PlaceHotelObject(root, "Bed_1", $"{ObjectsRoot}/Beds/BED.png",
                new Vector2Int(14, 7), bedData);

            EditorUtility.SetDirty(root);
            AssetDatabase.SaveAssets();
            Debug.Log("[Setup] Placed hotel objects with HotelObject components.");
        }

        private static HotelObjectData CreateObjectAsset(string path, string displayName,
            ObjectCategory category, int cost, float efficiency, float useDuration, int maxUsers = 1)
        {
            var data = CreateOrLoadAsset<HotelObjectData>(path);
            var so = new SerializedObject(data);
            so.FindProperty("displayName").stringValue = displayName;
            so.FindProperty("category").enumValueIndex = (int)category;
            so.FindProperty("cost").intValue = cost;
            so.FindProperty("efficiency").floatValue = efficiency;
            so.FindProperty("useDuration").floatValue = useDuration;
            so.FindProperty("maxUsers").intValue = maxUsers;
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

            // Clamp so object fits within its grid size (max 1 tile per cell)
            Vector2Int objSize = data != null ? data.size : Vector2Int.one;
            float spriteW = sprite.bounds.size.x;
            float spriteH = sprite.bounds.size.y;
            float maxW = objSize.x; // 1 world unit per tile
            float maxH = objSize.y;
            if (spriteW > maxW || spriteH > maxH)
            {
                float scale = Mathf.Min(maxW / spriteW, maxH / spriteH);
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

            var root = new GameObject("Decorations");

            // Central room: x=1-22, y=1-14. Top wall at y=14.

            // --- Shelves on top wall ---
            PlaceSprite(root, "Shelf_High_1",    $"{ObjectsRoot}/Env/SHELF.png",        new Vector3(7.5f, 14.65f, 0), 3);
            PlaceSprite(root, "ShelfVar_Low_1",  $"{ObjectsRoot}/Env/SHELF_Var_01.png", new Vector3(16.5f, 14.25f, 0), 3);

            // --- Plants ---
            PlaceSprite(root, "Plante_1",   $"{ObjectsRoot}/Env/PLANTE.png",    new Vector3(3.5f, 13.5f, 0), 5);
            PlaceSprite(root, "PlantBig_1", $"{ObjectsRoot}/Env/PLANT_BIG.png", new Vector3(20.5f, 13.5f, 0), 5);

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
            float size = 1f, float speed = 1f, int minRep = 0, bool aggressive = false)
        {
            var breed = CreateOrLoadAsset<CatBreedData>(assetPath);
            var so = new SerializedObject(breed);

            so.FindProperty("breedName").stringValue = breedName;
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
            RuntimeAnimatorController cleoCtrl, RuntimeAnimatorController aristoteCtrl)
        {
            const string D = "Assets/_Project/Data/Breeds";

            // Europeen variants (all minRep 0, balanced stats)
            var eur1 = CreateBreedAsset($"{D}/Breed_Europeen.asset", "Européen",
                CatSpritesRoot, "CAT_EUR_FRONT.png", "CAT_EUR_RIGHT.png", "CAT_EUR_BACK.png",
                eurCtrl);

            var eur2 = CreateBreedAsset($"{D}/Breed_Europeen2.asset", "Européen 2",
                Eur2SpritesRoot, "CAT_EUR_02_FRONT.png", "CAT_EUR_02_RIGHT.png", "CAT_EUR_02_BACK.png",
                eur2Ctrl);

            var eur3 = CreateBreedAsset($"{D}/Breed_Europeen3.asset", "Européen 3",
                Eur3SpritesRoot, "CAT_EUR_03_FRONT.png", "CAT_EUR_03_RIGHT.png", "CAT_EUR_03_BACK.png",
                eur3Ctrl);

            // Siamois (minRep 2, plays more, GDD: Play +50%, demand 1.2, speed 1.2, size 0.9)
            var siam = CreateBreedAsset($"{D}/Breed_Siamois.asset", "Siamois",
                SiamoisSpritesRoot, "CAT_SIAMESE_FRONT.png", "CAT_SIAMESE_RIGHT.png", "CAT_SIAMESE_BACK.png",
                siamoisCtrl,
                demand: 1.2f, pTrait: 1.5f, size: 0.9f, speed: 1.2f, minRep: 2);

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

            AssetDatabase.SaveAssets();
            Debug.Log("[Setup] Created breed SO assets.");

            return new[] { eur1, eur2, eur3, siam };
        }
    }
}
