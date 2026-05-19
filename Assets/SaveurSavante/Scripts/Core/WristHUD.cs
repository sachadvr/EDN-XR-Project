using UnityEngine;
using TMPro;

namespace SaveurSavante.Core
{
    public class WristHUD : MonoBehaviour
    {
        public static WristHUD Instance { get; private set; }

        public TextMeshPro statusText;
        public TextMeshPro storyText;

        [Tooltip("Si true, le HUD au poignet est invisible. Les textes sont mirrorés sur la sidebar et les holo texts du chapitre.")]
        public bool hideWristPanel = true;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            if (hideWristPanel)
            {
                // Hide all child renderers so nothing shows on the controller
                foreach (var r in GetComponentsInChildren<Renderer>(true)) r.enabled = false;
                foreach (var tmp in GetComponentsInChildren<TextMeshPro>(true)) tmp.enabled = false;
            }
            else
            {
                if (statusText != null) statusText.text = "";
                if (storyText != null) storyText.text = "Bienvenue dans Saveur Savante.\nChoisis un portail pour commencer.";
            }
        }

        public void SetStatus(string s)
        {
            if (!hideWristPanel && statusText != null) { statusText.text = s; statusText.gameObject.SetActive(true); }
            // Mirror to sidebar always
            if (StoryManager.Instance != null && StoryManager.Instance.storyText != null)
            {
                // status appended via dedicated method
                MirrorToSidebar(null, s);
            }
        }

        public void SetStory(string s)
        {
            if (!hideWristPanel && storyText != null) { storyText.text = s; storyText.gameObject.SetActive(true); }
            if (StoryManager.Instance != null)
            {
                MirrorToSidebar(s, null);
            }
        }

        private string lastStory = "";
        private string lastStatus = "";

        private void MirrorToSidebar(string story, string status)
        {
            if (story != null) lastStory = story;
            if (status != null) lastStatus = status;

            if (StoryManager.Instance != null && StoryManager.Instance.storyText != null)
            {
                string combined = lastStory;
                if (!string.IsNullOrEmpty(lastStatus)) combined += "\n\n" + lastStatus;
                StoryManager.Instance.storyText.text = combined;
                if (StoryManager.Instance.storyPanel != null && !StoryManager.Instance.storyPanel.activeSelf)
                    StoryManager.Instance.storyPanel.SetActive(true);
            }
        }
    }
}
