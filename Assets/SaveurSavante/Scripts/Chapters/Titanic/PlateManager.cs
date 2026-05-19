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
        public float requiredBalance = 0.8f;
        public float requiredPresentation = 100f;
        public int requiredFoodCount = 4;

        [Header("Feedback")]
        public AudioClip successSound;
        public GameObject completionText;
        public ParticleSystem sparkleEffect;

        [Header("UI")]
        public TMP_Text statusText;
        public TMP_Text presentationText;
        public TMP_Text balanceText;

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
            if (statusText == null)
                statusText = SaveurSavante.Core.HoloStatusBootstrap.EnsureHoloText(transform, "HoloStatus_Titanic", new Vector3(0, 2.5f, 0), 1.2f);
        }

        public void ShowIntro()
        {
            if (hasShownIntro) return;
            hasShownIntro = true;
            if (WristHUD.Instance != null)
            {
                WristHUD.Instance.SetStory("Titanic - Dresse une assiette equilibree.\nPlace 4 aliments sur l'assiette.");
                WristHUD.Instance.SetStatus("Aliments: 0/4");
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

            string status = $"+{food.foodName} ({food.flavorProfile})\nAliments: {placedFoods.Count}/{requiredFoodCount}\nSaveurs - sucre:{sweet} sale:{savory} acide:{acidic} amer:{bitter}";
            ShowStatus(status, 2f);

            UpdateScoreUI();

            Debug.Log(status);

            CheckCompletion();
        }

        private void CalculateScores()
        {
            // Présentation = +25 par aliment placé, max 100 (4 aliments = 100%)
            presentationScore = Mathf.Min(placedFoods.Count * 25f, 100f);

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
                presentationText.text = $"Presentation: {presentationScore:F0}/100";
            }

            if (balanceText != null)
            {
                balanceText.text = $"Equilibre: {balanceScore:F0}/100";
            }
        }

        private void ShowStatus(string message, float duration)
        {
            if (statusText != null)
            {
                statusText.text = message;
                statusText.gameObject.SetActive(true);
            }
            if (SaveurSavante.Core.WristHUD.Instance != null)
            {
                SaveurSavante.Core.WristHUD.Instance.SetStatus(message);
            }
        }

        public void CheckCompletion()
        {
            if (isComplete) return;

            if (placedFoods.Count >= requiredFoodCount)
            {
                CompleteChapter();
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

            ShowStatus("Felicitations ! Tu as termine le niveau Titanic !", 5f);

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
