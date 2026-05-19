using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace SaveurSavante.Core
{
    public class StoryManager : MonoBehaviour
    {
        public static StoryManager Instance { get; private set; }

        [System.Serializable]
        public class ChapterStory
        {
            public string chapterName;
            [TextArea(3, 10)]
            public string introduction;
            [TextArea(2, 5)]
            public string[] hints;
            [TextArea(2, 5)]
            public string successMessage;
            [TextArea(2, 5)]
            public string failureMessage;
        }

        [Header("Histoires par chapitre")]
        public ChapterStory[] chapterStories;

        [Header("UI References")]
        public GameObject storyPanel;
        public TMPro.TextMeshProUGUI storyText;
        public float displayDuration = 5f;
        public float fadeDuration = 0.5f;

        private Dictionary<string, ChapterStory> storyDict;
        private CanvasGroup canvasGroup;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeDictionary();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeDictionary()
        {
            storyDict = new Dictionary<string, ChapterStory>();
            foreach (var story in chapterStories)
            {
                storyDict[story.chapterName] = story;
            }
        }

        private void Start()
        {
            if (storyPanel == null || storyText == null)
            {
                Debug.LogWarning("StoryManager: storyPanel ou storyText n'est pas assigne dans l'inspecteur.");
                return;
            }

            canvasGroup = storyPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = storyPanel.AddComponent<CanvasGroup>();
            }
            // Sidebar always visible
            storyPanel.SetActive(true);
            canvasGroup.alpha = 1f;
        }

        public void ShowIntroduction(string chapterName)
        {
            if (storyDict.TryGetValue(chapterName, out ChapterStory story))
            {
                ShowStoryText(story.introduction);
            }
        }

        public void ShowHint(string chapterName, int hintIndex)
        {
            if (storyDict.TryGetValue(chapterName, out ChapterStory story))
            {
                if (hintIndex >= 0 && hintIndex < story.hints.Length)
                {
                    ShowStoryText(story.hints[hintIndex]);
                }
            }
        }

        public void ShowSuccess(string chapterName)
        {
            if (storyDict.TryGetValue(chapterName, out ChapterStory story))
            {
                ShowStoryText(story.successMessage);
            }
        }

        public void ShowFailure(string chapterName)
        {
            if (storyDict.TryGetValue(chapterName, out ChapterStory story))
            {
                ShowStoryText(story.failureMessage);
            }
        }

        private void ShowStoryText(string text)
        {
            // Mirror to wrist HUD if present
            if (WristHUD.Instance != null)
            {
                WristHUD.Instance.SetStory(text);
            }

            if (storyText != null)
            {
                storyText.text = text;
            }

            // Sidebar stays always visible — no fade
            if (storyPanel != null && !storyPanel.activeSelf)
            {
                storyPanel.SetActive(true);
            }
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }

            Debug.Log($"📜 Histoire: {text}");
        }

        private IEnumerator ShowTextCoroutine()
        {
            storyPanel.SetActive(true);
            canvasGroup.alpha = 1f;
            yield break;
        }

        public string GetChapterIntroduction(string chapterName)
        {
            if (storyDict.TryGetValue(chapterName, out ChapterStory story))
            {
                return story.introduction;
            }
            return "";
        }
    }
}
