using UnityEngine;

namespace CatHotel.Tutorial
{
    [CreateAssetMenu(fileName = "NewTutorialSequence", menuName = "Cat Hotel/Tutorial Sequence")]
    public class TutorialSequenceData : ScriptableObject
    {
        [Tooltip("Ordered list of steps. The tutorial plays them top-to-bottom.")]
        public TutorialStep[] steps;

        public int StepCount => steps != null ? steps.Length : 0;
    }
}
