using SaveurSavante.Core;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

namespace SaveurSavante.Chapters.Titanic
{
    public class FoodGuidance : MonoBehaviour
    {
        [Header("Configuration")]
        public string chapterName = "Titanic";

        [Header("Profils requis")]
        public int requiredSweet = 1;
        public int requiredSavory = 2;
        public int requiredAcidic = 1;
        public int requiredBitter = 1;

        [Header("UI")]
        public TextMeshPro guidanceText;
        public TextMeshPro validationText;
        public TextMeshPro presentationScoreText;
        public GameObject instructionPanel;

        [Header("Messages")]
        public string[] guidanceMessages = new string[]
        {
            "Le plat a besoin de quelque chose de sucré...",
            "🧂 Un élément salé apporterait de la profondeur...",
            "🍋 Une touche acide équilibrerait les saveurs...",
            "☕ Quelque chose d'amer pour la complexité..."
        };

        [Header("Feedback")]
        public AudioClip correctSound;
        public AudioClip wrongSound;
        public ParticleSystem correctParticles;
        public ParticleSystem wrongParticles;

        private int currentStep = 0;
        private int sweetCount = 0;
        private int savoryCount = 0;
        private int acidicCount = 0;
        private int bitterCount = 0;
        private List<TitanicFood> placedFoods = new List<TitanicFood>();

        private void Start()
        {
            ShowGuidance();
        }

        public void ValidateFood(TitanicFood food, Transform spot)
        {
            bool isCorrect = false;
            string message = "";

            switch (currentStep)
            {
                case 0: // Sucré
                    if (food.flavorProfile == "sucré" && sweetCount < requiredSweet)
                    {
                        isCorrect = true;
                        sweetCount++;
                        message = "✅ Parfait ! Une touche sucrée délicieuse !";
                        currentStep++;
                    }
                    else
                    {
                        message = "❌ Non, ce n'est pas ce qu'il faut maintenant...";
                    }
                    break;

                case 1: // Salé (x2)
                    if (food.flavorProfile == "salé" && savoryCount < requiredSavory)
                    {
                        isCorrect = true;
                        savoryCount++;
                        message = $"✅ Excellent ! { savoryCount}/{requiredSavory} éléments salés !";
                        if (savoryCount >= requiredSavory) currentStep++;
                    }
                    else
                    {
                        message = "❌ Ce n'est pas ce qu'il faut. Essaie autre chose...";
                    }
                    break;

                case 2: // Acide
                    if (food.flavorProfile == "acide" && acidicCount < requiredAcidic)
                    {
                        isCorrect = true;
                        acidicCount++;
                        message = "✅ Magnifique ! Une touche acide équilibrée !";
                        currentStep++;
                    }
                    else
                    {
                        message = "❌ Pas celui-là. Cherche autre chose...";
                    }
                    break;

                case 3: // Amer
                    if (food.flavorProfile == "amer" && bitterCount < requiredBitter)
                    {
                        isCorrect = true;
                        bitterCount++;
                        message = "✅ Superbe ! Une note amère sophistiquée !";
                        currentStep++;
                    }
                    else
                    {
                        message = "❌ Ce n'est pas ce qu'il faut pour finir...";
                    }
                    break;

                default:
                    // Validation de présentation
                    isCorrect = true;
                    message = "✅ L'assiette est complète ! Analysons la présentation...";
                    break;
            }

            // Feedback sonore
            if (isCorrect && correctSound != null)
            {
                AudioSource.PlayClipAtPoint(correctSound, spot.position);
                if (correctParticles != null)
                {
                    correctParticles.transform.position = spot.position;
                    correctParticles.Play();
                }
            }
            else if (!isCorrect && wrongSound != null)
            {
                AudioSource.PlayClipAtPoint(wrongSound, spot.position);
                if (wrongParticles != null)
                {
                    wrongParticles.transform.position = spot.position;
                    wrongParticles.Play();
                }
            }

            // Afficher le message
            ShowValidation(message);

            // Ajouter l'aliment
            placedFoods.Add(food);

            // Mettre à jour le guidance
            if (isCorrect)
            {
                ShowGuidance();
            }

            Debug.Log($"🍽️ {food.foodName} ({food.flavorProfile}): {(isCorrect ? "✅" : "❌")} - {message}");
        }

        private void ShowGuidance()
        {
            string guidance = "";

            switch (currentStep)
            {
                case 0:
                    guidance = guidanceMessages[0]; // Sucré
                    break;
                case 1:
                    guidance = guidanceMessages[1] + $" ({savoryCount}/{requiredSavory})"; // Salé
                    break;
                case 2:
                    guidance = guidanceMessages[2]; // Acide
                    break;
                case 3:
                    guidance = guidanceMessages[3]; // Amer
                    break;
                default:
                    guidance = "✨ L'assiette est complète ! Vérifie la présentation...";
                    break;
            }

            if (guidanceText != null)
            {
                guidanceText.text = guidance;
            }
        }

        private void ShowValidation(string message)
        {
            if (validationText != null)
            {
                validationText.text = message;
                validationText.gameObject.SetActive(true);
                CancelInvoke(nameof(HideValidation));
                Invoke(nameof(HideValidation), 3f);
            }
        }

        private void HideValidation()
        {
            if (validationText != null)
            {
                validationText.gameObject.SetActive(false);
            }
        }

        public void UpdatePresentationScore(float score)
        {
            if (presentationScoreText != null)
            {
                presentationScoreText.text = $"🎨 Présentation: {score:F0}/100";
            }
        }

        public bool IsComplete()
        {
            return sweetCount >= requiredSweet &&
                   savoryCount >= requiredSavory &&
                   acidicCount >= requiredAcidic &&
                   bitterCount >= requiredBitter;
        }

        public void ShowSuccess()
        {
            if (guidanceText != null)
            {
                guidanceText.text = "🎉 Assiette parfaite ! Un chef d'œuvre !";
            }
        }
    }
}
