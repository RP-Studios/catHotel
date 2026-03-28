using UnityEngine;
using CatHotel.Core;
using CatHotel.Hotel;

namespace CatHotel.Cats
{
    /// <summary>
    /// Handles breed affinity: happiness bonus/malus when near liked/disliked breeds,
    /// follow behavior for liked cats, anger trigger for disliked cats.
    /// </summary>
    public class CatAffinity : MonoBehaviour
    {
        private CatBreedData _likedBreed;
        private CatBreedData _dislikedBreed;
        private CatHappiness _happiness;
        private CatEntity _entity;
        private CatMoodBubble _moodBubble;
        private CatSpawner _spawner;
        private GameConfig _config;

        private float _tickTimer;
        private bool _isNearLiked;
        private bool _isNearDisliked;

        public CatBreedData LikedBreed => _likedBreed;
        public CatBreedData DislikedBreed => _dislikedBreed;
        public bool IsNearLiked => _isNearLiked;
        public bool IsNearDisliked => _isNearDisliked;

        public void Init(CatBreedData liked, CatBreedData disliked,
            CatHappiness happiness, CatEntity entity, CatSpawner spawner, GameConfig config)
        {
            _likedBreed = liked;
            _dislikedBreed = disliked;
            _happiness = happiness;
            _entity = entity;
            _spawner = spawner;
            _config = config;
            _moodBubble = GetComponent<CatMoodBubble>();
        }

        private void Update()
        {
            if (_config == null || (_likedBreed == null && _dislikedBreed == null)) return;

            _tickTimer += Time.deltaTime;
            if (_tickTimer < _config.affinityCheckInterval) return;
            _tickTimer = 0f;

            CheckProximity();
        }

        private void CheckProximity()
        {
            _isNearLiked = false;
            _isNearDisliked = false;

            CatEntity closestLiked = null;
            float closestLikedDist = float.MaxValue;

            float radius = _config.affinityCheckRadius;
            Vector2 myPos = transform.position;

            foreach (var other in _spawner.AllCats)
            {
                if (other == _entity) continue;

                float dist = Vector2.Distance(myPos, other.transform.position);
                if (dist > radius) continue;

                string otherBreedName = other.Breed != null ? other.Breed.breedName : null;
                if (otherBreedName == null) continue;

                // Check liked
                if (_likedBreed != null && otherBreedName == _likedBreed.breedName)
                {
                    _isNearLiked = true;
                    if (dist < closestLikedDist)
                    {
                        closestLikedDist = dist;
                        closestLiked = other;
                    }
                }

                // Check disliked
                if (_dislikedBreed != null && otherBreedName == _dislikedBreed.breedName)
                {
                    _isNearDisliked = true;
                }
            }

            float dt = _config.affinityCheckInterval;

            // Apply liked effects
            if (_isNearLiked)
            {
                _happiness.ApplyAffinityBonus(_config.likedHappinessBonus * dt);
            }

            // Apply disliked effects
            if (_isNearDisliked)
            {
                _happiness.ApplyAffinityPenalty(_config.dislikedHappinessPenalty * dt);

                // Chance to become angry
                if (_moodBubble != null && Random.value < _config.angerOnDislikeChance)
                {
                    _moodBubble.SetOverrideMood(CatMood.Angry);
                }
            }
            else
            {
                // Clear anger override if we're no longer near disliked breed
                if (_moodBubble != null && _moodBubble.CurrentMood == CatMood.Angry)
                {
                    _moodBubble.ClearOverride();
                }
            }
        }
    }
}
