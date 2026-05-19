using UnityEngine;
using System.Collections.Generic;
using SaveurSavante.Core;
using TMPro;

namespace SaveurSavante.Chapters.Vikings
{
    public class NutritionManager : MonoBehaviour
    {
        [Header("Configuration")]
        public float targetEnergy = 100f;
        public float targetSatiety = 100f;
        public float tolerance = 10f;
        public string chapterName = "Vikings";

        [Header("Feedback")]
        public AudioClip successSound;
        public AudioClip eatSound;
        public GameObject completionText;

        [Header("UI - Jauges")]
        public GameObject energyBar;
        public GameObject satietyBar;
        public TextMeshPro energyText;
        public TextMeshPro satietyText;
        public Transform energyBarFill;
        public Transform satietyBarFill;

        [Header("UI - Status")]
        public TextMeshPro statusText;
        public string introductionText = "Le jarl attend son festin ! Cuis les aliments au feu et mange-les pour regagner des forces.";

        [Header("État")]
        public float currentEnergy = 0f;
        public float currentSatiety = 0f;
        public List<VikingFood> placedFoods = new List<VikingFood>();
        public List<VikingFood> eatenFoods = new List<VikingFood>();
        public bool isComplete = false;
        public bool hasShownIntro = false;

        private void Start()
        {
            UpdateUI();
        }

        public void ShowIntro()
        {
            if (hasShownIntro) return;
            hasShownIntro = true;
            if (WristHUD.Instance != null)
            {
                WristHUD.Instance.SetStory($"🛡️ Vikings — {introductionText}");
                WristHUD.Instance.SetStatus("Cuis les aliments au feu, place-les sur la table puis mange-les.");
            }
        }

        public void AddFood(VikingFood food, Transform placementSpot)
        {
            if (isComplete) return;

            // Vérifier si la place est déjà occupée
            foreach (var placedFood in placedFoods)
            {
                if (placedFood.placementSpot == placementSpot)
                {
                    Debug.Log("🍖 Cette place est déjà occupée !");
                    ShowStatus("Cette place est déjà occupée !", 2f);
                    return;
                }
            }

            placedFoods.Add(food);

            string cookStatus = food.isCooked ? "cuit" : "cru";
            ShowStatus($"{food.foodName} ({cookStatus}) posé. Clique pour manger !", 3f);

            Debug.Log($"🍖 {food.foodName} ajouté sur la table ! Clique pour manger !");

            // Montrer un hint
            if (StoryManager.Instance != null && placedFoods.Count == 1)
            {
                StoryManager.Instance.ShowHint(chapterName, 0);
            }
        }

        public void EatFood(VikingFood food)
        {
            if (isComplete) return;
            if (eatenFoods.Contains(food)) return;

            eatenFoods.Add(food);

            // Ajouter les valeurs nutritionnelles
            currentEnergy += food.currentEnergy;
            currentSatiety += food.currentSatiety;

            // Clamp les valeurs
            currentEnergy = Mathf.Clamp(currentEnergy, 0f, targetEnergy * 1.5f);
            currentSatiety = Mathf.Clamp(currentSatiety, 0f, targetSatiety * 1.5f);

            Debug.Log($"🍖 Miam ! {food.foodName} mangé ! Énergie: {currentEnergy:F0}/{targetEnergy:F0}, Satiété: {currentSatiety:F0}/{targetSatiety:F0}");

            // Mettre à jour l'UI
            UpdateUI();

            // Montrer le status
            ShowStatus($"Énergie: {currentEnergy:F0}/{targetEnergy:F0}\nSatiété: {currentSatiety:F0}/{targetSatiety:F0}", 3f);

            // Vérifier si le repas est équilibré
            CheckCompletion();
        }

        private void UpdateUI()
        {
            // Mise à jour des barres de jauge
            if (energyBarFill != null)
            {
                float energyPercent = Mathf.Clamp01(currentEnergy / targetEnergy);
                energyBarFill.localScale = new Vector3(energyPercent, 1, 1);
            }

            if (satietyBarFill != null)
            {
                float satietyPercent = Mathf.Clamp01(currentSatiety / targetSatiety);
                satietyBarFill.localScale = new Vector3(satietyPercent, 1, 1);
            }

            // Mise à jour des textes
            if (energyText != null)
            {
                energyText.text = $"⚡ {currentEnergy:F0}/{targetEnergy:F0}";
            }

            if (satietyText != null)
            {
                satietyText.text = $"🍖 {currentSatiety:F0}/{targetSatiety:F0}";
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

        private void CheckCompletion()
        {
            // Vérifier que tous les aliments placés ont été mangés
            bool allEaten = true;
            foreach (var food in placedFoods)
            {
                if (!eatenFoods.Contains(food))
                {
                    allEaten = false;
                    break;
                }
            }

            bool energyOk = Mathf.Abs(currentEnergy - targetEnergy) <= tolerance;
            bool satietyOk = Mathf.Abs(currentSatiety - targetSatiety) <= tolerance;

            // Montrer la progression
            if (allEaten)
            {
                if (energyOk && satietyOk)
                {
                    CompleteChapter();
                }
                else
                {
                    ShowStatus($"Tu as mangé tout ! Mais équilibre imparfait. Énergie: {currentEnergy:F0}, Satiété: {currentSatiety:F0}", 4f);

                    // Hint pour améliorer
                    if (!energyOk)
                    {
                        ShowStatus("💡 Il te manque de l'énergie ! Mange plus de viande !", 3f);
                    }
                    else if (!satietyOk)
                    {
                        ShowStatus("💡 Tu n'as pas assez mangé ! Ajoute du poisson ou du pain !", 3f);
                    }
                }
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

            ShowStatus("✅ Repas parfait ! Le jarl est satisfait !", 5f);

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
