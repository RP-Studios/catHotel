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
using CatHotel.Shop;

namespace CatHotel.Editor
{
    public static class ProtoSceneSetup
    {
        // ==================== UI / SHOP ====================
        private const string UIRoot = "Assets/_Project/Art/UI";
        private const string ShopDataRoot = "Assets/_Project/Data/Shop";

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
        };

        [MenuItem("Cat Hotel/Setup Proto Scene")]
        public static void SetupScene()
        {
            ConfigureSpriteImports();
            ConfigureCatSpriteImports();
            ConfigureUISprites();
            var shopItems = CreateShopItemAssets();

            // Process all animation spritesheets
            ProcessAnimConfigs(AnimRoot, AnimConfigs);
            ProcessAnimConfigs(Eur2AnimRoot, Eur2AnimConfigs);
            ProcessAnimConfigs(Eur3AnimRoot, Eur3AnimConfigs);
            ProcessAnimConfigs(SiamoisAnimRoot, SiamoisAnimConfigs);
            ProcessAnimConfigs(CleoAnimRoot, CleoAnimConfigs);
            ProcessAnimConfigs(CloudRoot, CloudAnimConfigs);
            ProcessAnimConfigs(PettingRoot, HandPetAnimConfigs);
            AssetDatabase.Refresh();

            var tiles = CreateTileAssets();
            var eurController = CreateAnimController(CatControllerPath, AnimRoot, AnimConfigs);
            var eur2Controller = CreateAnimController(Eur2ControllerPath, Eur2AnimRoot, Eur2AnimConfigs);
            var eur3Controller = CreateAnimController(Eur3ControllerPath, Eur3AnimRoot, Eur3AnimConfigs);
            var siamoisController = CreateAnimController(SiamoisControllerPath, SiamoisAnimRoot, SiamoisAnimConfigs);
            var cleoController = CreateAnimController(CleoControllerPath, CleoAnimRoot, CleoAnimConfigs);
            var cloudController = CreateAnimController(CloudControllerPath, CloudRoot, CloudAnimConfigs);
            var handPetController = CreateAnimController(HandPetControllerPath, PettingRoot, HandPetAnimConfigs);
            BuildSceneHierarchy(tiles, eurController, eur2Controller, eur3Controller, siamoisController, cleoController, cloudController, handPetController, shopItems);
            int total = AnimConfigs.Length + Eur2AnimConfigs.Length + Eur3AnimConfigs.Length
                      + SiamoisAnimConfigs.Length + CleoAnimConfigs.Length + CloudAnimConfigs.Length + HandPetAnimConfigs.Length;
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
            };
        }

        private static TileBase CreateTile(string spritePath, string tilePath)
        {
            var existing = AssetDatabase.LoadAssetAtPath<Tile>(tilePath);
            if (existing != null)
            {
                existing.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
                EditorUtility.SetDirty(existing);
                AssetDatabase.SaveAssets();
                return existing;
            }

            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            if (sprite == null)
            {
                Debug.LogError($"Cannot load sprite at {spritePath}");
                return null;
            }

            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = sprite;
            tile.color = Color.white;

            AssetDatabase.CreateAsset(tile, tilePath);
            AssetDatabase.SaveAssets();
            return tile;
        }

        private static void BuildSceneHierarchy(
            TileSet tiles,
            RuntimeAnimatorController eurController,
            RuntimeAnimatorController eur2Controller,
            RuntimeAnimatorController eur3Controller,
            RuntimeAnimatorController siamoisController,
            RuntimeAnimatorController cleoController,
            RuntimeAnimatorController cloudController,
            RuntimeAnimatorController handPetController,
            ShopItemData[] shopItems)
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
            camObj.transform.position = new Vector3(24f, 16f, -10f);

            if (camObj.GetComponent<CameraController>() == null)
                camObj.AddComponent<CameraController>();

            // --- Grid parent ---
            var gridObj = FindOrCreate("Grid");
            var grid = gridObj.GetComponent<UnityEngine.Grid>();
            if (grid == null)
                grid = gridObj.AddComponent<UnityEngine.Grid>();
            grid.cellSize = Vector3.one;

            // --- Tilemaps ---
            var tmEmpty    = CreateTilemapChild(gridObj, "Tilemap_Empty",       0);
            var tmFloor    = CreateTilemapChild(gridObj, "Tilemap_Floor",       1);
            var tmWall     = CreateTilemapChild(gridObj, "Tilemap_Wall",        2);
            var tmPreview  = CreateTilemapChild(gridObj, "Tilemap_Preview",     3);

            // --- GridManager ---
            var mgrObj = FindOrCreate("GridManager");

            var renderer = mgrObj.GetComponent<GridRenderer>();
            if (renderer == null)
                renderer = mgrObj.AddComponent<GridRenderer>();

            var so = new SerializedObject(renderer);
            so.FindProperty("_emptyTilemap").objectReferenceValue      = tmEmpty;
            so.FindProperty("_floorTilemap").objectReferenceValue      = tmFloor;
            so.FindProperty("_wallTilemap").objectReferenceValue       = tmWall;
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
            so.ApplyModifiedProperties();

            var builder = mgrObj.GetComponent<RoomBuilderInput>();
            if (builder == null)
                builder = mgrObj.AddComponent<RoomBuilderInput>();

            var soBuilder = new SerializedObject(builder);
            soBuilder.FindProperty("_gridRenderer").objectReferenceValue = renderer;
            soBuilder.FindProperty("_camera").objectReferenceValue       = cam;
            soBuilder.ApplyModifiedProperties();

            // --- CatSpawner ---
            var spawner = mgrObj.GetComponent<CatSpawner>();
            if (spawner == null)
                spawner = mgrObj.AddComponent<CatSpawner>();

            var soSpawner = new SerializedObject(spawner);
            soSpawner.FindProperty("_gridRenderer").objectReferenceValue = renderer;

            // Wire breeds array (4 breeds)
            var breedsProperty = soSpawner.FindProperty("_breeds");
            breedsProperty.arraySize = 4;

            // Breed 0: Europeen
            SetBreed(breedsProperty, 0, "Europeen", CatSpritesRoot,
                "CAT_EUR_FRONT.png", "CAT_EUR_RIGHT.png", "CAT_EUR_BACK.png",
                eurController);

            // Breed 1: Europeen2
            SetBreed(breedsProperty, 1, "Europeen2", Eur2SpritesRoot,
                "CAT_EUR_02_FRONT.png", "CAT_EUR_02_RIGHT.png", "CAT_EUR_02_BACK.png",
                eur2Controller);

            // Breed 2: Europeen3
            SetBreed(breedsProperty, 2, "Europeen3", Eur3SpritesRoot,
                "CAT_EUR_03_FRONT.png", "CAT_EUR_03_RIGHT.png", "CAT_EUR_03_BACK.png",
                eur3Controller);

            // Breed 3: Siamois
            SetBreed(breedsProperty, 3, "Siamois", SiamoisSpritesRoot,
                "CAT_SIAMESE_FRONT.png", "CAT_SIAMESE_RIGHT.png", "CAT_SIAMESE_BACK.png",
                siamoisController);

            // Wire Cleo (special unique cat)
            var cleoProp = soSpawner.FindProperty("_cleoBreed");
            cleoProp.FindPropertyRelative("name").stringValue = "Cleo";
            cleoProp.FindPropertyRelative("frontSprite").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Sprite>($"{CleoSpritesRoot}/CAT_EUR_Cleo_FRONT.png");
            cleoProp.FindPropertyRelative("rightSprite").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Sprite>($"{CleoSpritesRoot}/CAT_EUR_Cleo_RIGHT.png");
            cleoProp.FindPropertyRelative("backSprite").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Sprite>($"{CleoSpritesRoot}/CAT_EUR_Cleo_BACK.png");
            cleoProp.FindPropertyRelative("controller").objectReferenceValue = cleoController;

            soSpawner.FindProperty("_fightCloudController").objectReferenceValue = cloudController;
            soSpawner.FindProperty("_handPetController").objectReferenceValue = handPetController;
            soSpawner.ApplyModifiedProperties();

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

            // --- Shop UI ---
            BuildShopUI(canvasObj, mgrObj, shopItems);

            // --- Decorations ---
            PlaceDecorations();

            // --- Mark dirty ---
            EditorUtility.SetDirty(camObj);
            EditorUtility.SetDirty(gridObj);
            EditorUtility.SetDirty(mgrObj);
            EditorUtility.SetDirty(canvasObj);

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        }

        // ==================== UI SPRITE IMPORTS ====================
        private static void ConfigureUISprites()
        {
            // 9-slice containers (Sprite mode = Single, mesh type = FullRect for 9-slice)
            string[] nineSliceSprites =
            {
                $"{UIRoot}/Components/RegularShop/shop_items_container.png",
                $"{UIRoot}/Components/RegularShop/shop_item_background_container.png",
                $"{UIRoot}/Components/RegularShop/shop_item_price_tag.png",
                $"{UIRoot}/Components/RegularShop/shop_item_label.png",
                $"{UIRoot}/Components/RegularShop/shop_item_icon_container.png",
                $"{UIRoot}/Components/RegularShop/regular_shop_items_container.png",
                $"{UIRoot}/Components/ObjectInformation/object_information_container.png",
                $"{UIRoot}/Components/ObjectInformation/price_tag.png",
                $"{UIRoot}/Panels/menu_panel_container.png",
            };
            foreach (var s in nineSliceSprites)
                ConfigureSprite(s, 100, FilterMode.Bilinear);

            // Regular sprites (icons, buttons)
            string[] regularSprites =
            {
                $"{UIRoot}/Icons/Minimalist/shop.png",
                $"{UIRoot}/Icons/Minimalist/cat_coin.png",
                $"{UIRoot}/Icons/Minimalist/plus.png",
                $"{UIRoot}/Buttons/back_btn.png",
                $"{UIRoot}/Buttons/game_menu_btn.png",
                $"{UIRoot}/Components/ObjectInformation/close_btn.png",
                $"{UIRoot}/Components/ObjectInformation/sell_btn.png",
                $"{UIRoot}/Banners/full_banner.png",
            };
            foreach (var s in regularSprites)
                ConfigureSprite(s, 100, FilterMode.Bilinear);
        }

        // ==================== SHOP ITEM DATA ====================
        private static ShopItemData[] CreateShopItemAssets()
        {
            // Ensure directory exists
            if (!AssetDatabase.IsValidFolder("Assets/_Project/Data"))
                AssetDatabase.CreateFolder("Assets/_Project", "Data");
            if (!AssetDatabase.IsValidFolder(ShopDataRoot))
                AssetDatabase.CreateFolder("Assets/_Project/Data", "Shop");

            var items = new (string name, string display, string spritePath, int price, ShopCategory cat)[]
            {
                ("Bed",       "Lit",         $"{ObjectsRoot}/Beds/BED.png",        50,  ShopCategory.Beds),
                ("Coussin",   "Coussin",     $"{ObjectsRoot}/Beds/COUSSIN.png",    30,  ShopCategory.Beds),
                ("LuxousBed", "Lit de luxe", $"{ObjectsRoot}/Beds/LUXOUS_BED.png", 150, ShopCategory.Beds),
            };

            var result = new ShopItemData[items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                var (name, display, spritePath, price, cat) = items[i];
                string assetPath = $"{ShopDataRoot}/ShopItem_{name}.asset";

                var existing = AssetDatabase.LoadAssetAtPath<ShopItemData>(assetPath);
                if (existing != null)
                {
                    result[i] = existing;
                    continue;
                }

                var data = ScriptableObject.CreateInstance<ShopItemData>();
                data.displayName = display;
                data.icon = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
                data.price = price;
                data.category = cat;
                data.spritePath = spritePath;

                AssetDatabase.CreateAsset(data, assetPath);
                result[i] = data;
            }

            AssetDatabase.SaveAssets();
            Debug.Log($"[ProtoSceneSetup] {result.Length} shop items configured.");
            return result;
        }

        // ==================== SHOP UI HIERARCHY ====================
        private static void BuildShopUI(GameObject canvasObj, GameObject mgrObj, ShopItemData[] shopItems)
        {
            // --- Shop Panel (fullscreen overlay, starts hidden) ---
            var panelObj = FindOrCreateChild(canvasObj, "ShopPanel");
            var panelRect = EnsureRectTransform(panelObj);
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            // Semi-transparent background
            var panelImg = panelObj.GetComponent<Image>();
            if (panelImg == null)
                panelImg = panelObj.AddComponent<Image>();
            // Use the shop container sprite if available, otherwise solid color
            var containerSprite = AssetDatabase.LoadAssetAtPath<Sprite>(
                $"{UIRoot}/Components/RegularShop/shop_items_container.png");
            if (containerSprite != null)
            {
                panelImg.sprite = containerSprite;
                panelImg.type = Image.Type.Sliced;
                panelImg.color = new Color(0.96f, 0.90f, 0.78f, 0.97f); // Crème
            }
            else
            {
                panelImg.color = new Color(0.96f, 0.90f, 0.78f, 0.95f);
            }

            // --- Title banner ---
            var titleObj = FindOrCreateChild(panelObj, "Title");
            var titleRect = EnsureRectTransform(titleObj);
            titleRect.anchorMin = new Vector2(0.15f, 0.85f);
            titleRect.anchorMax = new Vector2(0.85f, 0.95f);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;

            var bannerSprite = AssetDatabase.LoadAssetAtPath<Sprite>(
                $"{UIRoot}/Banners/full_banner.png");
            var titleImg = titleObj.GetComponent<Image>();
            if (titleImg == null)
                titleImg = titleObj.AddComponent<Image>();
            if (bannerSprite != null)
            {
                titleImg.sprite = bannerSprite;
                titleImg.type = Image.Type.Sliced;
                titleImg.preserveAspect = true;
            }

            var titleText = FindOrCreateChild(titleObj, "Text");
            var titleTextRect = EnsureRectTransform(titleText);
            titleTextRect.anchorMin = Vector2.zero;
            titleTextRect.anchorMax = Vector2.one;
            titleTextRect.offsetMin = Vector2.zero;
            titleTextRect.offsetMax = Vector2.zero;
            var txt = titleText.GetComponent<Text>();
            if (txt == null)
                txt = titleText.AddComponent<Text>();
            txt.text = "Boutique";
            txt.alignment = TextAnchor.MiddleCenter;
            txt.fontSize = 42;
            txt.fontStyle = FontStyle.Bold;
            txt.color = new Color(0.35f, 0.15f, 0.15f, 1f);
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            // --- Close button (top-right) ---
            var closeBtnObj = FindOrCreateChild(panelObj, "CloseBtn");
            var closeBtnRect = EnsureRectTransform(closeBtnObj);
            closeBtnRect.anchorMin = new Vector2(0.90f, 0.88f);
            closeBtnRect.anchorMax = new Vector2(0.97f, 0.97f);
            closeBtnRect.offsetMin = Vector2.zero;
            closeBtnRect.offsetMax = Vector2.zero;

            var closeBtnSprite = AssetDatabase.LoadAssetAtPath<Sprite>(
                $"{UIRoot}/Components/ObjectInformation/close_btn.png");
            var closeBtnImg = closeBtnObj.GetComponent<Image>();
            if (closeBtnImg == null)
                closeBtnImg = closeBtnObj.AddComponent<Image>();
            if (closeBtnSprite != null)
                closeBtnImg.sprite = closeBtnSprite;
            closeBtnImg.color = Color.white;

            if (closeBtnObj.GetComponent<Button>() == null)
                closeBtnObj.AddComponent<Button>();

            // --- ScrollRect area ---
            var scrollObj = FindOrCreateChild(panelObj, "ScrollArea");
            var scrollRect = EnsureRectTransform(scrollObj);
            scrollRect.anchorMin = new Vector2(0.05f, 0.08f);
            scrollRect.anchorMax = new Vector2(0.95f, 0.82f);
            scrollRect.offsetMin = Vector2.zero;
            scrollRect.offsetMax = Vector2.zero;

            var scrollComp = scrollObj.GetComponent<ScrollRect>();
            if (scrollComp == null)
                scrollComp = scrollObj.AddComponent<ScrollRect>();
            scrollComp.horizontal = true;
            scrollComp.vertical = false;
            scrollComp.movementType = ScrollRect.MovementType.Elastic;
            scrollComp.elasticity = 0.1f;
            scrollComp.inertia = true;
            scrollComp.decelerationRate = 0.135f;

            // Mask
            var scrollImg = scrollObj.GetComponent<Image>();
            if (scrollImg == null)
                scrollImg = scrollObj.AddComponent<Image>();
            scrollImg.color = new Color(1f, 1f, 1f, 0.01f); // near-invisible, needed for mask
            var mask = scrollObj.GetComponent<Mask>();
            if (mask == null)
                mask = scrollObj.AddComponent<Mask>();
            mask.showMaskGraphic = false;

            // Content (horizontal layout)
            var contentObj = FindOrCreateChild(scrollObj, "Content");
            var contentRect = EnsureRectTransform(contentObj);
            contentRect.anchorMin = new Vector2(0f, 0f);
            contentRect.anchorMax = new Vector2(0f, 1f); // stretch vertically, grow right
            contentRect.pivot = new Vector2(0f, 0.5f);
            contentRect.offsetMin = Vector2.zero;
            contentRect.offsetMax = Vector2.zero;

            var hlg = contentObj.GetComponent<HorizontalLayoutGroup>();
            if (hlg == null)
                hlg = contentObj.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 20f;
            hlg.padding = new RectOffset(20, 20, 10, 10);
            hlg.childAlignment = TextAnchor.MiddleLeft;
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = true;
            hlg.childControlWidth = false;
            hlg.childControlHeight = true;

            var csf = contentObj.GetComponent<ContentSizeFitter>();
            if (csf == null)
                csf = contentObj.AddComponent<ContentSizeFitter>();
            csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            csf.verticalFit = ContentSizeFitter.FitMode.Unconstrained;

            scrollComp.content = contentRect;

            // --- Item Prefab Template (created in scene, will be used as template) ---
            var itemTemplate = BuildShopItemTemplate(panelObj);

            // --- ShopUI component ---
            var shopUI = mgrObj.GetComponent<ShopUI>();
            if (shopUI == null)
                shopUI = mgrObj.AddComponent<ShopUI>();

            var soShop = new SerializedObject(shopUI);
            soShop.FindProperty("_panel").objectReferenceValue = panelRect;
            soShop.FindProperty("_content").objectReferenceValue = contentRect;
            soShop.FindProperty("_closeBtn").objectReferenceValue = closeBtnObj.GetComponent<Button>();
            soShop.FindProperty("_itemPrefab").objectReferenceValue = itemTemplate;

            // Wire shop items
            var itemsProp = soShop.FindProperty("_items");
            itemsProp.arraySize = shopItems.Length;
            for (int i = 0; i < shopItems.Length; i++)
                itemsProp.GetArrayElementAtIndex(i).objectReferenceValue = shopItems[i];

            soShop.ApplyModifiedProperties();

            // Wire ProtoUI shop reference
            var protoUI = mgrObj.GetComponent<ProtoUI>();
            if (protoUI != null)
            {
                var soUI = new SerializedObject(protoUI);
                soUI.FindProperty("_shopUI").objectReferenceValue = shopUI;
                soUI.ApplyModifiedProperties();
            }

            EditorUtility.SetDirty(panelObj);
            Debug.Log("[ProtoSceneSetup] Shop UI hierarchy built.");
        }

        private static GameObject BuildShopItemTemplate(GameObject parent)
        {
            // This template lives in the scene (inactive). ShopUI instantiates copies at runtime.
            var tmpl = FindOrCreateChild(parent, "ShopItemTemplate");
            tmpl.SetActive(false);

            var tmplRect = EnsureRectTransform(tmpl);
            var le = tmpl.GetComponent<LayoutElement>();
            if (le == null)
                le = tmpl.AddComponent<LayoutElement>();
            le.preferredWidth = 220f;
            le.preferredHeight = 300f;

            // Card background
            var cardBgSprite = AssetDatabase.LoadAssetAtPath<Sprite>(
                $"{UIRoot}/Components/RegularShop/shop_item_background_container.png");
            var cardImg = tmpl.GetComponent<Image>();
            if (cardImg == null)
                cardImg = tmpl.AddComponent<Image>();
            if (cardBgSprite != null)
            {
                cardImg.sprite = cardBgSprite;
                cardImg.type = Image.Type.Sliced;
            }
            cardImg.color = new Color(0.96f, 0.90f, 0.78f, 1f);

            // Button on the whole card
            var btn = tmpl.GetComponent<Button>();
            if (btn == null)
                btn = tmpl.AddComponent<Button>();
            btn.targetGraphic = cardImg;

            // Icon area (top 60%)
            var iconObj = FindOrCreateChild(tmpl, "Icon");
            var iconRect = EnsureRectTransform(iconObj);
            iconRect.anchorMin = new Vector2(0.1f, 0.35f);
            iconRect.anchorMax = new Vector2(0.9f, 0.90f);
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;

            var iconImg = iconObj.GetComponent<Image>();
            if (iconImg == null)
                iconImg = iconObj.AddComponent<Image>();
            iconImg.preserveAspect = true;
            iconImg.raycastTarget = false;
            iconImg.color = Color.white;

            // Name label (middle)
            var nameObj = FindOrCreateChild(tmpl, "Name");
            var nameRect = EnsureRectTransform(nameObj);
            nameRect.anchorMin = new Vector2(0.05f, 0.18f);
            nameRect.anchorMax = new Vector2(0.95f, 0.35f);
            nameRect.offsetMin = Vector2.zero;
            nameRect.offsetMax = Vector2.zero;

            var nameTxt = nameObj.GetComponent<Text>();
            if (nameTxt == null)
                nameTxt = nameObj.AddComponent<Text>();
            nameTxt.text = "Item";
            nameTxt.alignment = TextAnchor.MiddleCenter;
            nameTxt.fontSize = 24;
            nameTxt.fontStyle = FontStyle.Bold;
            nameTxt.color = new Color(0.35f, 0.15f, 0.15f, 1f);
            nameTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            nameTxt.raycastTarget = false;

            // Price tag (bottom)
            var priceObj = FindOrCreateChild(tmpl, "PriceTag");
            var priceRect = EnsureRectTransform(priceObj);
            priceRect.anchorMin = new Vector2(0.15f, 0.02f);
            priceRect.anchorMax = new Vector2(0.85f, 0.18f);
            priceRect.offsetMin = Vector2.zero;
            priceRect.offsetMax = Vector2.zero;

            var priceTagSprite = AssetDatabase.LoadAssetAtPath<Sprite>(
                $"{UIRoot}/Components/RegularShop/shop_item_price_tag.png");
            var priceImg = priceObj.GetComponent<Image>();
            if (priceImg == null)
                priceImg = priceObj.AddComponent<Image>();
            if (priceTagSprite != null)
            {
                priceImg.sprite = priceTagSprite;
                priceImg.type = Image.Type.Sliced;
            }
            priceImg.color = new Color(0.78f, 0.47f, 0.47f, 1f);
            priceImg.raycastTarget = false;

            // Price text inside tag
            var priceTxtObj = FindOrCreateChild(priceObj, "PriceText");
            var priceTxtRect = EnsureRectTransform(priceTxtObj);
            priceTxtRect.anchorMin = Vector2.zero;
            priceTxtRect.anchorMax = Vector2.one;
            priceTxtRect.offsetMin = Vector2.zero;
            priceTxtRect.offsetMax = Vector2.zero;

            var priceTxt = priceTxtObj.GetComponent<Text>();
            if (priceTxt == null)
                priceTxt = priceTxtObj.AddComponent<Text>();
            priceTxt.text = "0";
            priceTxt.alignment = TextAnchor.MiddleCenter;
            priceTxt.fontSize = 22;
            priceTxt.fontStyle = FontStyle.Bold;
            priceTxt.color = Color.white;
            priceTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            priceTxt.raycastTarget = false;

            // Wire ShopItemUI component
            var itemUI = tmpl.GetComponent<ShopItemUI>();
            if (itemUI == null)
                itemUI = tmpl.AddComponent<ShopItemUI>();

            var soItem = new SerializedObject(itemUI);
            soItem.FindProperty("_background").objectReferenceValue = cardImg;
            soItem.FindProperty("_iconImage").objectReferenceValue = iconImg;
            soItem.FindProperty("_nameLabel").objectReferenceValue = nameTxt;
            soItem.FindProperty("_priceLabel").objectReferenceValue = priceTxt;
            soItem.FindProperty("_priceTag").objectReferenceValue = priceImg;
            soItem.FindProperty("_button").objectReferenceValue = btn;
            soItem.ApplyModifiedProperties();

            return tmpl;
        }

        private static void AddToolbarIcon(GameObject zone, string name, string spritePath, int siblingIndex)
        {
            var existing = zone.transform.Find(name);
            GameObject obj;
            if (existing != null)
            {
                obj = existing.gameObject;
            }
            else
            {
                obj = new GameObject(name, typeof(RectTransform));
                obj.transform.SetParent(zone.transform, false);
            }

            var rect = obj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(80f, 80f);

            var img = obj.GetComponent<Image>();
            if (img == null)
                img = obj.AddComponent<Image>();

            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            if (sprite != null)
                img.sprite = sprite;
            img.preserveAspect = true;

            // Place it as the N-th child (toolbar position)
            obj.transform.SetSiblingIndex(siblingIndex);
        }

        private static GameObject FindOrCreateChild(GameObject parent, string name)
        {
            var existing = parent.transform.Find(name);
            if (existing != null)
                return existing.gameObject;

            var obj = new GameObject(name, typeof(RectTransform));
            obj.transform.SetParent(parent.transform, false);
            return obj;
        }

        private static RectTransform EnsureRectTransform(GameObject obj)
        {
            var rect = obj.GetComponent<RectTransform>();
            if (rect == null)
                rect = obj.AddComponent<RectTransform>();
            return rect;
        }

        private static void PlaceDecorations()
        {
            // Destroy previous decorations container
            var old = GameObject.Find("Decorations");
            if (old != null) Object.DestroyImmediate(old);

            var root = new GameObject("Decorations");

            // Central room: x=5-21, y=10-23. Top wall at y=24.

            // --- Shelves on top wall tiles (2 heights) ---
            // High (y=24.65) and low (y=24.25), both on the wall row y=24
            PlaceSprite(root, "Shelf_High_1",    $"{ObjectsRoot}/Env/SHELF.png",        new Vector3(8.5f,  24.65f, 0), 3);
            PlaceSprite(root, "ShelfVar_Low_1",  $"{ObjectsRoot}/Env/SHELF_Var_01.png", new Vector3(12.5f, 24.25f, 0), 3);
            PlaceSprite(root, "Shelf_High_2",    $"{ObjectsRoot}/Env/SHELF.png",        new Vector3(16.5f, 24.65f, 0), 3);
            PlaceSprite(root, "ShelfVar_Low_2",  $"{ObjectsRoot}/Env/SHELF_Var_01.png", new Vector3(19.5f, 24.25f, 0), 3);

            // --- Plants ---
            PlaceSprite(root, "Plante_1",   $"{ObjectsRoot}/Env/PLANTE.png",    new Vector3(6.5f,  22.5f, 0), 5);
            PlaceSprite(root, "PlantBig_1", $"{ObjectsRoot}/Env/PLANT_BIG.png", new Vector3(20.5f, 21.5f, 0), 5);
            PlaceSprite(root, "Plante_2",   $"{ObjectsRoot}/Env/PLANTE.png",    new Vector3(19.5f, 11.5f, 0), 5);
            PlaceSprite(root, "PlantBig_2", $"{ObjectsRoot}/Env/PLANT_BIG.png", new Vector3(6.5f,  12.5f, 0), 5);

            // --- Beds ---
            PlaceSprite(root, "Bed_1",       $"{ObjectsRoot}/Beds/BED.png",        new Vector3(9.5f,  18.5f, 0), 5);
            PlaceSprite(root, "Coussin_1",   $"{ObjectsRoot}/Beds/COUSSIN.png",    new Vector3(14.5f, 13.5f, 0), 5);
            PlaceSprite(root, "LuxBed_1",    $"{ObjectsRoot}/Beds/LUXOUS_BED.png", new Vector3(12.5f, 20.5f, 0), 5);
            PlaceSprite(root, "Coussin_2",   $"{ObjectsRoot}/Beds/COUSSIN.png",    new Vector3(17.5f, 16.5f, 0), 5);

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
        }

        private static void SetBreed(SerializedProperty breedsProperty, int index,
            string name, string spritesRoot,
            string frontFile, string rightFile, string backFile,
            RuntimeAnimatorController controller)
        {
            var b = breedsProperty.GetArrayElementAtIndex(index);
            b.FindPropertyRelative("name").stringValue = name;
            b.FindPropertyRelative("frontSprite").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Sprite>($"{spritesRoot}/{frontFile}");
            b.FindPropertyRelative("rightSprite").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Sprite>($"{spritesRoot}/{rightFile}");
            b.FindPropertyRelative("backSprite").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Sprite>($"{spritesRoot}/{backFile}");
            b.FindPropertyRelative("controller").objectReferenceValue = controller;
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
    }
}
