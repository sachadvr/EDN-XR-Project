using SaveurSavante.Core;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

namespace SaveurSavante.Chapters.Gandhi
{
    public class TreasureHunt : MonoBehaviour
    {
        [Header("Configuration")]
        public string chapterName = "Gandhi";

        [Header("Énigmes")]
        public string[] riddles = new string[]
        {
            "🧘 'Gandhi marche vers la mer, il lui faut de la force intérieure. Quel fruit lui donnera de l'énergie ?'",
            "🌱 'La vie vient de la terre. Quel légume le nourrira dans sa quête ?'",
            "🌾 'Les graines de la patience portent leurs fruits. Que faut-il pour compléter le bol ?'",
            "🥜 'La force des petites choses. Quelle graine donne la vitalité ?'"
        };

        [Header("Réponses attendues")]
        public string[] expectedFoodTypes = new string[] { "fruit", "legume", "graine", "graine" };

        [Header("UI")]
        public TextMeshPro riddleText;
        public TextMeshPro feedbackText;
        public GameObject riddlePanel;

        [Header("Feedback")]
        public AudioClip correctSound;
        public AudioClip wrongSound;
        public AudioClip riddleSound;
        public ParticleSystem successParticles;

        [Header("État")]
        public int currentRiddleIndex = 0;
        public List<GandhiFood> foundFoods = new List<GandhiFood>();
        public bool isComplete = false;

        private void Start()
        {
            ShowCurrentRiddle();
        }

        public bool TrySolveRiddle(GandhiFood food, Transform spot)
        {
            if (isComplete) return false;
            if (currentRiddleIndex >= expectedFoodTypes.Length) return false;

            string expectedType = expectedFoodTypes[currentRiddleIndex];

            if (food.foodType == expectedType)
            {
                // Bonne réponse !
                foundFoods.Add(food);
                currentRiddleIndex++;

                // Feedback
                if (correctSound != null)
                {
                    AudioSource.PlayClipAtPoint(correctSound, spot.position);
                }

                if (successParticles != null)
                {
                    successParticles.transform.position = spot.position;
                    successParticles.Play();
                }

                string successMessage = $"✅ {food.foodName} est la bonne réponse !";
                if (currentRiddleIndex < riddles.Length)
                {
                    successMessage += "\n🧘 Gandhi est reconnaissant...";
                }
                ShowFeedback(successMessage);

                // Prochaine énigme ou fin
                if (currentRiddleIndex < riddles.Length)
                {
                    ShowCurrentRiddle();
                }
                else
                {
                    CompleteHunt();
                }

                Debug.Log($"🧘 {food.foodName} ({food.foodType}) = bonne réponse !");
                return true;
            }
            else
            {
                // Mauvaise réponse
                if (wrongSound != null)
                {
                    AudioSource.PlayClipAtPoint(wrongSound, spot.position);
                }

                string hint = GetHintForType(expectedType);
                ShowFeedback($"❌ Ce n'est pas ce qu'il faut...\n💡 Indice: {hint}");

                // Retourner la nourriture à sa position d'origine
                food.ReturnToSpawn();

                Debug.Log($"🧘 {food.foodName} ({food.foodType}) ≠ {expectedType}...");
                return false;
            }
        }

        private string GetHintForType(string foodType)
        {
            switch (foodType)
            {
                case "fruit":
                    return "Cherche quelque chose de sucré qui pousse sur les arbres...";
                case "legume":
                    return "Cherche quelque chose qui pousse dans la terre...";
                case "graine":
                    return "Cherche quelque chose de petit mais plein d'énergie...";
                default:
                    return "Regarde bien l'énigme...";
            }
        }

        private void ShowCurrentRiddle()
        {
            if (currentRiddleIndex < riddles.Length && riddleText != null)
            {
                riddleText.text = riddles[currentRiddleIndex];

                // Son d'énigme
                if (riddleSound != null)
                {
                    AudioSource.PlayClipAtPoint(riddleSound, transform.position);
                }

                Debug.Log($"🧩 Énigme {currentRiddleIndex + 1}/{riddles.Length}: {riddles[currentRiddleIndex]}");
            }
        }

        private void ShowFeedback(string message)
        {
            if (feedbackText != null)
            {
                feedbackText.text = message;
                feedbackText.gameObject.SetActive(true);
                CancelInvoke(nameof(HideFeedback));
                Invoke(nameof(HideFeedback), 4f);
            }
        }

        private void HideFeedback()
        {
            if (feedbackText != null)
            {
                feedbackText.gameObject.SetActive(false);
            }
        }

        private void CompleteHunt()
        {
            isComplete = true;

            if (riddleText != null)
            {
                riddleText.text = "🎉 Toutes les énigmes sont résolues !\nGandhi peut maintenant résister à la marche du sel !";
            }

            ShowFeedback("✅ Chasse au trésor complétée ! Un repas équilibré pour l'âme !");

            // Notifier le BowlManager de compléter le chapitre
            BowlManager bowlManager = FindObjectOfType<BowlManager>();
            if (bowlManager != null)
            {
                bowlManager.CompleteFromTreasureHunt();
            }

            Debug.Log("🧘 Chasse au trésor complétée ! Gandhi est fort !");

            if (StoryManager.Instance != null)
            {
                StoryManager.Instance.ShowSuccess(chapterName);
            }
        }

        public void ShowHint()
        {
            if (StoryManager.Instance != null && currentRiddleIndex < riddles.Length)
            {
                StoryManager.Instance.ShowHint(chapterName, currentRiddleIndex);
            }
        }
    }
}
