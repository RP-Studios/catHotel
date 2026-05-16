using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CatHotel.Services;
using CatHotel.Core;

namespace CatHotel.UI
{
    /// <summary>
    /// Popup RGPD affichee au premier lancement, dans la scene Boot.
    /// Auto-wire et localise les elements par nom (recherche recursive) :
    ///   TitleLabel, DescriptionLabel (TMP_Text), AcceptButton, RefuseButton (Button).
    /// Le BootManager attend ConsentManager.HasMadeChoice avant d'initialiser les pubs.
    /// </summary>
    public class ConsentPanel : MonoBehaviour
    {
        [Tooltip("Racine du panel a masquer apres choix. Si null, utilise ce GameObject.")]
        [SerializeField] private GameObject _panelRoot;

        [SerializeField] private TMP_Text _titleLabel;
        [SerializeField] private TMP_Text _descriptionLabel;
        [SerializeField] private Button _acceptButton;
        [SerializeField] private Button _refuseButton;

        private void Awake()
        {
            if (_panelRoot == null) _panelRoot = gameObject;

            if (_titleLabel == null) _titleLabel = FindChild<TMP_Text>("TitleLabel");
            if (_descriptionLabel == null) _descriptionLabel = FindChild<TMP_Text>("DescriptionLabel");
            if (_acceptButton == null) _acceptButton = FindChild<Button>("AcceptButton");
            if (_refuseButton == null) _refuseButton = FindChild<Button>("RefuseButton");

            // Already chosen: skip popup entirely
            if (ConsentManager.HasMadeChoice)
            {
                _panelRoot.SetActive(false);
                return;
            }

            // Awake() runs before BootManager.Start() resolves the language.
            // InitFromSystem() is idempotent → ensures the popup is in the device language.
            LocalizedStrings.InitFromSystem();
            ApplyLocalizedText();

            _panelRoot.SetActive(true);
            if (_acceptButton != null) _acceptButton.onClick.AddListener(OnAccept);
            if (_refuseButton != null) _refuseButton.onClick.AddListener(OnRefuse);
        }

        private void ApplyLocalizedText()
        {
            if (_titleLabel != null)
                _titleLabel.text = LocalizedStrings.Get("consent.title");
            if (_descriptionLabel != null)
                _descriptionLabel.text = LocalizedStrings.Get("consent.body");

            SetButtonLabel(_acceptButton, LocalizedStrings.Get("consent.accept"));
            SetButtonLabel(_refuseButton, LocalizedStrings.Get("consent.refuse"));
        }

        private static void SetButtonLabel(Button button, string text)
        {
            if (button == null) return;
            var label = button.GetComponentInChildren<TMP_Text>(true);
            if (label != null) label.text = text;
        }

        private T FindChild<T>(string childName) where T : Component
        {
            foreach (Transform t in GetComponentsInChildren<Transform>(true))
            {
                if (t.name == childName)
                {
                    var comp = t.GetComponent<T>();
                    if (comp != null) return comp;
                }
            }
            return null;
        }

        private void OnAccept()
        {
            ConsentManager.SetConsent(true);
            _panelRoot.SetActive(false);
        }

        private void OnRefuse()
        {
            ConsentManager.SetConsent(false);
            _panelRoot.SetActive(false);
        }
    }
}
