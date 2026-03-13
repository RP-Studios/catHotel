using System;
using UnityEngine;

namespace CatHotel.Services
{
    public class RevenueBoostManager : MonoBehaviour
    {
        public static RevenueBoostManager Instance { get; private set; }

        [SerializeField] private AdConfig _config;

        private float _boostTimer;

        public bool IsBoosted => _boostTimer > 0f;
        public float BoostMultiplier => IsBoosted ? _config.boostMultiplier : 1f;
        public float BoostTimeRemaining => _boostTimer;

        public event Action<bool> OnBoostChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (_boostTimer <= 0f) return;

            _boostTimer -= Time.deltaTime;
            if (_boostTimer <= 0f)
            {
                _boostTimer = 0f;
                Debug.Log("[Boost] Revenue boost expired");
                OnBoostChanged?.Invoke(false);
            }
        }

        public void ActivateBoost()
        {
            _boostTimer = _config.boostDuration;
            Debug.Log($"[Boost] Revenue x{_config.boostMultiplier} for {_config.boostDuration}s");
            OnBoostChanged?.Invoke(true);
        }
    }
}
