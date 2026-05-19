using UnityEngine;
using System.Collections.Generic;
using SaveurSavante.Core;
using TMPro;

namespace SaveurSavante.Chapters.Vikings
{
    public class NutritionManager : MonoBehaviour
    {
        [Header("Configuration")]
        public float targetSatiety = 100f;
        public float satietyPerFood = 25f;
        public string chapterName = "Vikings";

        [Header("Feedback")]
        public AudioClip successSound;
        public AudioClip eatSound;
        public GameObject completionText;

        [Header("UI - Jauges")]
        public TMP_Text satietyText;
        public Transform satietyBarFill;

        [Header("UI - Status")]
        public TMP_Text statusText;
        public string introductionText = "Le jarl attend son festin ! Cuis les aliments au feu et mange-les pour regagner des forces.";

        [Header("État")]
        public float currentSatiety = 0f;
        public List<VikingFood> placedFoods = new List<VikingFood>();
        public bool isComplete = false;
        public bool hasShownIntro = false;

        private void Start()
        {
            if (statusText == null)
                statusText = SaveurSavante.Core.HoloStatusBootstrap.EnsureHoloText(transform, "HoloStatus_Vikings", new Vector3(0, 2.5f, 0), 1.2f);
            if (satietyText == null)
                satietyText = SaveurSavante.Core.HoloStatusBootstrap.EnsureHoloText(transform, "HoloSatiety_Vikings", new Vector3(0, 3.5f, 0), 1.4f);
            UpdateUI();
        }

        public void ShowIntro()
        {
            if (hasShownIntro) return;
            hasShownIntro = true;
            if (WristHUD.Instance != null)
            {
                WristHUD.Instance.SetStory($"Vikings - {introductionText}");
                WristHUD.Instance.SetStatus("Cuis les aliments au feu puis pose-les sur la table. Satiete: 0/100");
            }
        }

        public void AddFood(VikingFood food, Transform placementSpot)
        {
            if (isComplete) return;

            foreach (var placedFood in placedFoods)
            {
                if (placedFood == food) return;
                if (placedFood.placementSpot == placementSpot && placementSpot != null)
                {
                    ShowStatus("Cette place est deja occupee !", 2f);
                    return;
                }
            }

            placedFoods.Add(food);
            currentSatiety = Mathf.Min(currentSatiety + satietyPerFood, targetSatiety);

            UpdateUI();

            string cookStatus = food.isCooked ? "cuit" : "cru";
            ShowStatus($"{food.foodName} ({cookStatus}) pose !\nSatiete: {currentSatiety:F0}/{targetSatiety:F0}", 3f);

            Debug.Log($"{food.foodName} ajoute. Satiete {currentSatiety:F0}/{targetSatiety:F0}");

            CheckCompletion();
        }

        // Legacy stub: VikingFood.EatFood still calls this; eating no longer affects score.
        public void EatFood(VikingFood food) { }

        public void RegisterCookedFood(VikingFood food)
        {
            if (isComplete) return;
            if (food == null) return;
            if (placedFoods.Contains(food)) return;

            placedFoods.Add(food);
            currentSatiety = Mathf.Min(currentSatiety + satietyPerFood, targetSatiety);

            // Re-bootstrap if scene ref was broken (UGUI mismatch -> null)
            if (satietyText == null)
                satietyText = SaveurSavante.Core.HoloStatusBootstrap.EnsureHoloText(transform, "HoloSatiety_Vikings", new Vector3(0, 3.5f, 0), 1.4f);

            UpdateUI();
            ShowStatus($"{food.foodName} cuit !\nSatiete: {currentSatiety:F0}/{targetSatiety:F0}", 3f);
            Debug.Log($"Cuit: {food.foodName}. Satiete {currentSatiety:F0}/{targetSatiety:F0}");

            CheckCompletion();
        }

        private void UpdateUI()
        {
            if (satietyBarFill != null)
            {
                float satietyPercent = Mathf.Clamp01(currentSatiety / targetSatiety);
                satietyBarFill.localScale = new Vector3(satietyPercent, 1, 1);
            }

            if (satietyText != null)
            {
                float pct = Mathf.Clamp01(currentSatiety / targetSatiety) * 100f;
                satietyText.text = $"Satiete\n{currentSatiety:F0}/{targetSatiety:F0}  ({pct:F0}%)";
                satietyText.gameObject.SetActive(true);
                satietyText.enabled = true;
            }
        }

        private void ShowStatus(string message, float duration)
        {
            // Toujours mettre a jour le holo monde si present
            if (statusText != null)
            {
                statusText.text = message;
                statusText.gameObject.SetActive(true);
            }
            // Et mirrorer vers sidebar via WristHUD
            if (SaveurSavante.Core.WristHUD.Instance != null)
            {
                SaveurSavante.Core.WristHUD.Instance.SetStatus(message);
            }
        }

        private void CheckCompletion()
        {
            if (currentSatiety >= targetSatiety)
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

            if (completionText != null)
            {
                completionText.SetActive(true);
            }

            ShowStatus("Felicitations ! Le jarl est rassasie !", 5f);

            Debug.Log("✅ Chapitre Vikings complété ! Le repas du guerrier est parfait !");

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
