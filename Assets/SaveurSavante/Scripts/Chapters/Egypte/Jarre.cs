using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;
using SaveurSavante.Core;
using TMPro;

namespace SaveurSavante.Chapters.Egypte
{
    public class Jarre : MonoBehaviour
    {
        public float offeringDetectionRadius = 1.5f;
        private XRGrabInteractable grab;
        [Header("Configuration")]
        public int requiredFoodCount = 1;
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
            grab = GetComponent<XRGrabInteractable>();
            if (grab == null) grab = GetComponentInChildren<XRGrabInteractable>();
        }

        private void Update()
        {
            if (isComplete) return;
            if (foodsInJar.Count < requiredFoodCount) return;
            if (grab != null && grab.isSelected) return;

            OfferingZone zone = FindNearestZone(offeringDetectionRadius);
            if (zone != null)
            {
                zone.TriggerValidation(this);
            }
        }

        private OfferingZone FindNearestZone(float maxDistance)
        {
            OfferingZone best = null;
            float bestSqr = maxDistance * maxDistance;
            foreach (var z in FindObjectsOfType<OfferingZone>())
            {
                float d = (z.transform.position - transform.position).sqrMagnitude;
                if (d < bestSqr) { bestSqr = d; best = z; }
            }
            return best;
        }

        public void ShowIntro()
        {
            if (hasShownIntro) return;
            hasShownIntro = true;
            if (WristHUD.Instance != null)
            {
                WristHUD.Instance.SetStory("🫙 Égypte — Cléopâtre exige une offrande parfaite. Sale les aliments puis dépose-les dans la jarre.");
                WristHUD.Instance.SetStatus($"Aliments salés à déposer: 0/{requiredFoodCount}");
            }
        }

        public void AddFood(SaltApplication food)
        {
            if (isComplete) return;

            foodsInJar.Add(food);
            
            // Désactiver la physique et le grab pour un relachement propre
            var grab = food.GetComponent<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>();
            if (grab != null) grab.enabled = false;
            
            var rb = food.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;
            
            // Le placer visuellement à l'intérieur
            food.transform.SetParent(this.transform);
            food.transform.localPosition = new Vector3(0, 0.2f + (foodsInJar.Count * 0.15f), 0);
            food.transform.localRotation = Quaternion.identity;

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
