using UnityEngine;
using System.Collections.Generic;
using SaveurSavante.Core;
using TMPro;

namespace SaveurSavante.Chapters.Egypte
{
    public class Jarre : MonoBehaviour
    {
        [Header("Configuration")]
        public int requiredFoodCount = 3;
        public string chapterName = "Egypte";

        [Header("Feedback")]
        public AudioClip successSound;
        public ParticleSystem successParticles;
        public GameObject completionText;

        [Header("UI")]
        public TextMeshPro statusText;

        [Header("État")]
        public List<SaltApplication> foodsInJar = new List<SaltApplication>();
        public bool isComplete = false;
        public bool hasShownIntro = false;

        private void Start()
        {
        }

        public void AddFood(SaltApplication food)
        {
            if (isComplete) return;

            if (!hasShownIntro)
            {
                hasShownIntro = true;
                ShowStatus("🫙 Cléopâtre exige une offrande parfaite !\nSale les aliments et dépose-les dans la jarre.", 5f);
            }

            foodsInJar.Add(food);
            food.gameObject.SetActive(false); // Cacher l'aliment

            string status = $"🫙 Aliment ajouté : {foodsInJar.Count}/{requiredFoodCount}";
            ShowStatus(status, 2f);

            Debug.Log(status);

            if (foodsInJar.Count >= requiredFoodCount)
            {
                ShowStatus("✨ Jarre pleine ! Déplace-la vers la zone d'offrande !", 3f);
            }
        }

        public void ClearFoods()
        {
            // Vider la jarre (appelé par OfferingZone en cas d'échec)
            foreach (var food in foodsInJar)
            {
                if (food != null)
                {
                    food.gameObject.SetActive(true);
                    // La position de reset est gérée par OfferingZone
                }
            }

            foodsInJar.Clear();
            isComplete = false;

            ShowStatus("❌ Offrande rejetée ! La jarre a été vidée.", 3f);
            Debug.Log("🫙 Jarre vidée et réinitialisée.");
        }

        public List<SaltApplication> GetFoodsInJar()
        {
            return new List<SaltApplication>(foodsInJar);
        }

        private void ShowStatus(string message, float duration)
        {
            if (statusText != null)
            {
                statusText.text = message;
                statusText.gameObject.SetActive(true);
                CancelInvoke(nameof(HideStatus));
                Invoke(nameof(HideStatus), duration);
            }
        }

        private void HideStatus()
        {
            if (statusText != null)
            {
                statusText.gameObject.SetActive(false);
            }
        }

        private void CompleteChapter()
        {
            isComplete = true;

            if (successSound != null)
            {
                AudioSource.PlayClipAtPoint(successSound, transform.position);
            }

            if (successParticles != null)
            {
                successParticles.Play();
            }

            if (completionText != null)
            {
                completionText.SetActive(true);
            }

            ShowStatus("✅ Cléopâtre est satisfaite ! L'offrande est parfaite !", 5f);

            Debug.Log("✅ Chapitre Égypte complété ! L'offrande est parfaite !");

            // Notifier le GameManager
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
