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
            "Gandhi a besoin d'un fruit.\nExemples: pomme, banane, orange, fraise.",
            "Gandhi a besoin d'un legume.\nExemples: carotte, tomate, poivron, salade.",
            "Gandhi a besoin d'une graine.\nExemples: riz, ble, mais, lentille."
        };

        [Header("Réponses attendues")]
        public string[] expectedFoodTypes = new string[] { "fruit", "legume", "graine" };

        [Header("UI")]
        public TMP_Text riddleText;
        public TMP_Text feedbackText;
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

        private bool huntStarted = false;

        private void Start()
        {
            if (riddleText == null)
                riddleText = SaveurSavante.Core.HoloStatusBootstrap.EnsureHoloText(transform, "HoloRiddle_Gandhi", new Vector3(0, 3.2f, 0), 1.3f);
            if (feedbackText == null)
                feedbackText = SaveurSavante.Core.HoloStatusBootstrap.EnsureHoloText(transform, "HoloFeedback_Gandhi", new Vector3(0, 2.0f, 0), 1.0f);
        }

        public void StartHunt()
        {
            if (huntStarted) return;
            huntStarted = true;
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

                string successMessage = $"Bravo ! {food.foodName} est la bonne reponse !";
                if (currentRiddleIndex < riddles.Length)
                {
                    successMessage += "\nGandhi est reconnaissant.";
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
                ShowFeedback($"Ce n'est pas ce qu'il faut.\nIndice: {hint}");

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
            if (currentRiddleIndex >= riddles.Length) return;

            string riddle = riddles[currentRiddleIndex];
            string instr = $"Énigme {currentRiddleIndex + 1}/{riddles.Length}\n{riddle}";

            if (riddleText != null) { riddleText.text = instr; riddleText.gameObject.SetActive(true); }
            if (WristHUD.Instance != null) WristHUD.Instance.SetStatus(instr);

            if (riddleSound != null)
            {
                AudioSource.PlayClipAtPoint(riddleSound, transform.position);
            }

            Debug.Log($"🧩 Énigme {currentRiddleIndex + 1}/{riddles.Length}: {riddle}");
        }

        private void ShowFeedback(string message)
        {
            if (WristHUD.Instance != null) WristHUD.Instance.SetStory(message);
            if (feedbackText != null)
            {
                feedbackText.text = message;
                feedbackText.gameObject.SetActive(true);
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

    }
}
