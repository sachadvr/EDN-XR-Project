using UnityEngine;
using TMPro;

namespace SaveurSavante.Core
{
    public class WristHUD : MonoBehaviour
    {
        public static WristHUD Instance { get; private set; }

        public TextMeshPro statusText;
        public TextMeshPro storyText;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            if (statusText != null) statusText.text = "";
            if (storyText != null) storyText.text = "Bienvenue dans Saveur Savante.\nChoisis un portail pour commencer.";
        }

        public void SetStatus(string s) { if (statusText != null) { statusText.text = s; statusText.gameObject.SetActive(true); } }
        public void SetStory(string s)  { if (storyText  != null) { storyText.text  = s; storyText.gameObject.SetActive(true); } }
    }
}
