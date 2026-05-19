using SaveurSavante.Core;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SaveurSavante.Chapters.Egypte
{
    public class OfferingZone : MonoBehaviour
    {
        [Header("Configuration")]
        public string chapterName = "Egypte";
        public int requiredFoodCount = 1;
        public float jarDetectionRadius = 3f;

        [Header("Positions")]
        public Transform jarreResetPosition;
        public Transform[] foodResetPositions;

        [Header("Feedback")]
        public AudioClip successSound;
        public AudioClip failureSound;
        public ParticleSystem successParticles;
        public ParticleSystem failureParticles;
        public GameObject completionText;

        [Header("UI")]
        public GameObject validationText;

        private List<SaltApplication> validatedFoods = new List<SaltApplication>();
        private Jarre currentJarre = null;
        private bool isValidating = false;

        private void Update()
        {
            if (isValidating || currentJarre != null) return;
            float bestSqr = jarDetectionRadius * jarDetectionRadius;
            Jarre best = null;
            foreach (var j in FindObjectsOfType<Jarre>())
            {
                if (j.foodsInJar.Count < requiredFoodCount) continue;
                if (j.isComplete) continue;
                var grab = j.GetComponent<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>();
                if (grab != null && grab.isSelected) continue;
                float d = (j.transform.position - transform.position).sqrMagnitude;
                if (d < bestSqr) { bestSqr = d; best = j; }
            }
            if (best != null)
            {
                currentJarre = best;
                ValidateOffering();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isValidating) return;

            Jarre jarre = other.GetComponentInParent<Jarre>();
            if (jarre != null && currentJarre == null)
            {
                currentJarre = jarre;
                ValidateOffering();
            }
        }

        public void TriggerValidation(Jarre jarre)
        {
            if (isValidating || currentJarre != null) return;
            currentJarre = jarre;
            ValidateOffering();
        }

        private void ValidateOffering()
        {
            if (currentJarre == null || isValidating) return;

            isValidating = true;

            // Récupérer tous les aliments via la méthode GetFoodsInJar
            List<SaltApplication> foodsInJar = currentJarre.GetFoodsInJar();
            int totalCount = foodsInJar.Count;

            int preservedCount = 0;
            foreach (var food in foodsInJar)
                if (food != null && food.isPreserved) preservedCount++;

            bool allPreserved = (preservedCount == totalCount) && (totalCount > 0);
            bool countValid = totalCount >= requiredFoodCount;

            Debug.Log($"Validation: {preservedCount}/{totalCount} sales, {requiredFoodCount} requis");

            if (allPreserved && countValid)
                StartCoroutine(SuccessSequence());
            else
                StartCoroutine(FailureSequence(allPreserved, countValid, totalCount));
        }

        private IEnumerator SuccessSequence()
        {
            Debug.Log("✅ Offrande acceptée ! Tous les aliments sont parfaitement conservés !");

            // Son de succès
            if (successSound != null)
            {
                AudioSource.PlayClipAtPoint(successSound, transform.position);
            }

            // Particules de succès
            if (successParticles != null)
            {
                successParticles.Play();
            }

            // Montrer le texte de validation
            if (validationText != null)
            {
                validationText.SetActive(true);
                yield return new WaitForSeconds(2f);
                validationText.SetActive(false);
            }

            // Texte de complétion
            if (completionText != null)
            {
                completionText.SetActive(true);
            }

            // Notifier le GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CompleteChapter(chapterName);
            }

            if (StoryManager.Instance != null)
            {
                StoryManager.Instance.ShowSuccess(chapterName);
            }

            isValidating = false;
        }

        private IEnumerator FailureSequence(bool allPreserved, bool countValid, int totalCount)
        {
            Debug.Log("❌ Offrande rejetée ! Cléopâtre exige la perfection !");

            // Son d'échec
            if (failureSound != null)
            {
                AudioSource.PlayClipAtPoint(failureSound, transform.position);
            }

            // Particules d'échec
            if (failureParticles != null)
            {
                failureParticles.Play();
            }

            // Afficher le message d'échec
            string failureReason = "";
            if (!countValid)
            {
                failureReason = $"Il manque des aliments ! ({totalCount}/{requiredFoodCount})";
            }
            else if (!allPreserved)
            {
                failureReason = "Certains aliments ne sont pas conservés avec du sel !";
            }

            Debug.Log($"💔 Échec: {failureReason}");

            yield return new WaitForSeconds(1f);

            // RESET: Remettre la jarre à sa position
            if (currentJarre != null && jarreResetPosition != null)
            {
                currentJarre.transform.position = jarreResetPosition.position;
                currentJarre.transform.rotation = jarreResetPosition.rotation;

                // Vider la jarre et remettre les aliments
                ResetJarreContents();
            }

            if (StoryManager.Instance != null)
            {
                StoryManager.Instance.ShowFailure(chapterName);
            }

            currentJarre = null;
            isValidating = false;
        }

        private void ResetJarreContents()
        {
            if (currentJarre == null) return;

            // Récupérer tous les aliments de la jarre via GetFoodsInJar
            List<SaltApplication> foods = currentJarre.GetFoodsInJar();

            for (int i = 0; i < foods.Count; i++)
            {
                var food = foods[i];
                if (food == null) continue;

                // Réactiver l'aliment
                food.gameObject.SetActive(true);
                food.transform.SetParent(null);
                
                var rb = food.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = false;

                // Remettre à sa position initiale ou une position de reset
                if (i < foodResetPositions.Length && foodResetPositions[i] != null)
                {
                    food.transform.position = foodResetPositions[i].position;
                    food.transform.rotation = foodResetPositions[i].rotation;
                }
                else if (foodResetPositions.Length > 0 && foodResetPositions[0] != null)
                {
                    // Position par défaut avec offset
                    food.transform.position = foodResetPositions[0].position + Vector3.right * (i * 0.5f);
                }

                // Réactiver le grab
                var xrGrab = food.GetComponent<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>();
                if (xrGrab != null)
                {
                    xrGrab.enabled = true;
                }
            }

            // Vider la liste de la jarre
            currentJarre.ClearFoods();
        }

        private void OnTriggerExit(Collider other)
        {
            Jarre jarre = other.GetComponent<Jarre>();
            if (jarre != null && jarre == currentJarre && !isValidating)
            {
                currentJarre = null;
            }
        }
    }
}
