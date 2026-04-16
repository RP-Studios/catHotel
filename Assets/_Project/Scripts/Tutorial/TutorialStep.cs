using System;
using UnityEngine;
using CatHotel.Core;

namespace CatHotel.Tutorial
{
    public enum TutorialTrigger
    {
        WaitForTap,
        WaitForCatSelected,
        WaitForObjectSelected,
        WaitForObjectPlaced,
        WaitForObjectMoved,
        WaitForCatPetted,
        WaitForShopOpened,
        WaitForFloorChanged,
        WaitForCoinCollected,
        WaitForCatServiceUsed,
        WaitForDelay,
        Action
    }

    public enum TutorialAction
    {
        None,
        SpawnFirstCat,
        SpawnRefugeCatHungry,
        FocusCameraOnLastSpawnedCat,
        FocusCameraOnPensionEntrance,
        FocusCameraOnRefugeEntrance,
        FocusCameraOnUnhappyExit,
        ReleaseCameraFocus,
        SetRefugeCatThirsty,
        SetRefugeCatSleepy,
        SetRefugeCatDirty,
        SetRefugeCatBored,
        EnableShopFood,
        EnableShopWater,
        EnableShopSleep,
        EnableShopClean,
        EnableShopBalls,
        EnableFullShop,
    }

    [Serializable]
    public class TutorialStep
    {
        [Header("Speaker")]
        public string speakerLabelKey;
        public Sprite speakerPortrait;

        [Header("Dialogue")]
        public string textKey;

        [Header("Progression")]
        public TutorialTrigger trigger;
        [Tooltip("For WaitForObjectPlaced: which category is required.")]
        public ObjectCategory requiredCategory;
        [Tooltip("For WaitForDelay: seconds to wait.")]
        public float delaySeconds = 3f;

        [Header("Actions")]
        public TutorialAction actionOnStart;
        public TutorialAction actionOnComplete;

        public bool HasDialogue => !string.IsNullOrEmpty(textKey);
    }
}
