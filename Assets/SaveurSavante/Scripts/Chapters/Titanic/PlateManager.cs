using UnityEngine;
using System.Collections.Generic;
using SaveurSavante.Core;
using TMPro;

namespace SaveurSavante.Chapters.Titanic
{
    public class PlateManager : MonoBehaviour
    {
        [Header("Configuration")]
        public string chapterName = "Titanic";
        public int maxFoodItems = 5;
        public float requiredBalance = 0.8f; // Score minimum pour valider
        public float requiredPresentation = 70f; // Score minimum de présentation

        [Header("Feedback")]
        public AudioClip successSound;
        public GameObject completionText;
        public ParticleSystem sparkleEffect;

        [Header("UI")]
        public TextMeshPro statusText;
        public TextMeshPro presentationText;
        public TextMeshPro balanceText;

        [Header("État")]
        public List<TitanicFood> placedFoods = new List<TitanicFood>();
        public float presentationScore = 0f;
        public float balanceScore = 0f;
        public bool isComplete = false;
        public bool hasShownIntro = false;

        private FoodGuidance foodGuidance;

        private void Start()
        {
            foodGuidance = FindObjectOfType<FoodGuidance>();
        }

        public void ShowIntro()
        {
            if (hasShownIntro) return;
            hasShownIntro = true;
            if (WristHUD.Instance != null)
            {
                WristHUD.Instance.SetStory("🚢 Titanic\nDresse une assiette équilibrée.\nMix les saveurs: sucré, salé, acide, amer.");
                WristHUD.Instance.SetStatus("Saveurs: 0/4\nVise au moins 3 saveurs différentes.");
            }
        }

        public void AddFood(TitanicFood food, Transform spot)
        {
            if (isComplete) return;

            if (placedFoods.Count >= maxFoodItems)
            {
                ShowStatus("🍽️ L'assiette est pleine !", 2f);
                return;
            }

            placedFoods.Add(food);
            CalculateScores();

            // Mettre à jour l'UI du FoodGuidance si présent
            if (foodGuidance != null)
            {
                foodGuidance.UpdatePresentationScore(presentationScore);
            }

            int sweet = 0, savory = 0, acidic = 0, bitter = 0;
            foreach (var f in placedFoods)
            {
                switch (f.flavorProfile)
                {
                    case "sucré": sweet++; break;
                    case "salé": savory++; break;
                    case "acide": acidic++; break;
                    case "amer": bitter++; break;
                }
            }
            int variety = (sweet > 0 ? 1 : 0) + (savory > 0 ? 1 : 0) + (acidic > 0 ? 1 : 0) + (bitter > 0 ? 1 : 0);

            string status = $"+{food.foodName} ({food.flavorProfile})\nSaveurs: {variety}/4\n🍬{sweet} 🧂{savory} 🍋{acidic} 🌿{bitter}";
            ShowStatus(status, 2f);

            UpdateScoreUI();

            Debug.Log(status);

            CheckCompletion();
        }

        private void CalculateScores()
        {
            // Score de présentation basé sur le nombre d'éléments et leur valeur
            float totalPresentation = 0f;
            foreach (var food in placedFoods)
            {
                totalPresentation += food.presentationValue;
            }
            presentationScore = Mathf.Min(totalPresentation, 100f);

            // Score d'équilibre basé sur la diversité
            int sweetCount = 0, savoryCount = 0, acidicCount = 0, bitterCount = 0;

            foreach (var food in placedFoods)
            {
                switch (food.flavorProfile)
                {
                    case "sucré": sweetCount++; break;
                    case "salé": savoryCount++; break;
                    case "acide": acidicCount++; break;
                    case "amer": bitterCount++; break;
                }
            }

            // Un bon équilibre a au moins 3 profils différents
            int varietyCount = 0;
            if (sweetCount > 0) varietyCount++;
            if (savoryCount > 0) varietyCount++;
            if (acidicCount > 0) varietyCount++;
            if (bitterCount > 0) varietyCount++;

            balanceScore = varietyCount * 25f;

            // Pénalité si trop d'un même profil
            int maxCount = Mathf.Max(sweetCount, savoryCount, acidicCount, bitterCount);
            if (maxCount > 2)
            {
                balanceScore -= (maxCount - 2) * 10f;
            }

            balanceScore = Mathf.Clamp(balanceScore, 0f, 100f);
        }

        private void UpdateScoreUI()
        {
            if (presentationText != null)
            {
                presentationText.text = $"🎨 Présentation: {presentationScore:F0}/100";
            }

            if (balanceText != null)
            {
                balanceText.text = $"⚖️ Équilibre: {balanceScore:F0}/100";
            }
        }

        private void ShowStatus(string message, float duration)
        {
            if (SaveurSavante.Core.WristHUD.Instance != null)
            {
                SaveurSavante.Core.WristHUD.Instance.SetStatus(message);
            }
            else if (statusText != null)
            {
                statusText.text = message;
                statusText.gameObject.SetActive(true);
            }
        }

        public void CheckCompletion()
        {
            if (isComplete) return;

            if (presentationScore >= requiredPresentation && placedFoods.Count >= 4)
            {
                CompleteChapter();
            }
            else if (placedFoods.Count >= 4)
            {
                string feedback = $"La présentation pourrait être meilleure... ({presentationScore:F0}/100)";
                ShowStatus(feedback, 4f);
            }
        }

        private void CompleteChapter()
        {
            isComplete = true;

            if (successSound != null)
            {
                AudioSource.PlayClipAtPoint(successSound, transform.position);
            }

            if (sparkleEffect != null)
            {
                sparkleEffect.Play();
            }

            if (completionText != null)
            {
                completionText.SetActive(true);
            }

            // Notifier le FoodGuidance
            if (foodGuidance != null)
            {
                foodGuidance.ShowSuccess();
            }

            ShowStatus("🎉 Un chef-d'œuvre gastronomique ! Le Titanic est fier de toi !", 5f);

            Debug.Log("✅ Chapitre Titanic complété ! Un plat raffiné !");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.CompleteChapter(chapterName);
            }

            if (StoryManager.Instance != null)
            {
                StoryManager.Instance.ShowSuccess(chapterName);
            }
        }
    }
}
