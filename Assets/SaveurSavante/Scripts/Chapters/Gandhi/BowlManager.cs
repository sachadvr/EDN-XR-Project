using UnityEngine;
using System.Collections.Generic;
using SaveurSavante.Core;
using TMPro;

namespace SaveurSavante.Chapters.Gandhi
{
    public class BowlManager : MonoBehaviour
    {
        [Header("Configuration")]
        public string chapterName = "Gandhi";
        public int requiredFruits = 1;
        public int requiredVegetables = 1;
        public int requiredGrains = 2;

        [Header("Feedback")]
        public AudioClip successSound;
        public AudioClip zenSound;
        public GameObject completionText;
        public ParticleSystem zenEffect;

        [Header("UI")]
        public GameObject balanceIndicator;
        public TextMeshPro statusText;

        [Header("État")]
        public List<GandhiFood> foodsInBowl = new List<GandhiFood>();
        public int fruitCount = 0;
        public int vegetableCount = 0;
        public int grainCount = 0;
        public bool isComplete = false;
        public bool hasShownIntro = false;

        private TreasureHunt treasureHunt;

        private void Start()
        {
            treasureHunt = GetComponent<TreasureHunt>();

            if (!hasShownIntro)
            {
                hasShownIntro = true;
                ShowStatus("🧘 Gandhi a besoin de ton aide !\nRésous les énigmes pour remplir le bol.", 5f);
            }
        }

        public void AddFood(GandhiFood food)
        {
            if (isComplete) return;

            foodsInBowl.Add(food);

            // Compter par type
            switch (food.foodType)
            {
                case "fruit":
                    fruitCount++;
                    break;
                case "legume":
                    vegetableCount++;
                    break;
                case "graine":
                    grainCount++;
                    break;
            }

            UpdateBalanceUI();

            string status = $"🧘 {food.foodName} ajouté ! Fruits: {fruitCount}, Légumes: {vegetableCount}, Graines: {grainCount}";
            ShowStatus(status, 3f);

            Debug.Log(status);

            // Si pas de TreasureHunt, vérifier directement
            if (treasureHunt == null || treasureHunt.isComplete)
            {
                CheckCompletion();
            }
        }

        private void UpdateBalanceUI()
        {
            if (balanceIndicator != null)
            {
                // Créer un affichage visuel de l'équilibre
                string balance = $"🍎 {fruitCount}/{requiredFruits}  🥕 {vegetableCount}/{requiredVegetables}  🌾 {grainCount}/{requiredGrains}";
                // Mettre à jour l'UI si présent
            }

            if (statusText != null)
            {
                statusText.text = $"Bol: {foodsInBowl.Count} aliments\n🍎 {fruitCount}  🥕 {vegetableCount}  🌾 {grainCount}";
            }
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

        public void CheckCompletion()
        {
            if (isComplete) return;

            bool fruitsOk = fruitCount >= requiredFruits;
            bool vegetablesOk = vegetableCount >= requiredVegetables;
            bool grainsOk = grainCount >= requiredGrains;

            if (fruitsOk && vegetablesOk && grainsOk)
            {
                CompleteChapter();
            }
            else
            {
                // Feedback sur ce qui manque
                string missing = "";
                if (!fruitsOk) missing += $"🍎 Il manque {requiredFruits - fruitCount} fruit(s). ";
                if (!vegetablesOk) missing += $"🥕 Il manque {requiredVegetables - vegetableCount} légume(s). ";
                if (!grainsOk) missing += $"🌾 Il manque {requiredGrains - grainCount} graine(s).";

                ShowStatus(missing, 4f);
            }
        }

        public void CompleteFromTreasureHunt()
        {
            // Appelé par TreasureHunt quand toutes les énigmes sont résolues
            CompleteChapter();
        }

        private void CompleteChapter()
        {
            isComplete = true;

            if (successSound != null)
            {
                AudioSource.PlayClipAtPoint(successSound, transform.position);
            }

            if (zenSound != null)
            {
                AudioSource.PlayClipAtPoint(zenSound, transform.position);
            }

            if (zenEffect != null)
            {
                zenEffect.Play();
            }

            if (completionText != null)
            {
                completionText.SetActive(true);
            }

            ShowStatus("🎉 Un repas parfait pour Gandhi ! La sagesse est dans l'équilibre !", 5f);

            Debug.Log("✅ Chapitre Gandhi complété ! Un repas équilibré pour l'âme !");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.CompleteChapter(chapterName);
            }
        }

        public void ResetBowl()
        {
            // Remettre tous les aliments à leur position initiale
            foreach (var food in foodsInBowl)
            {
                if (food != null)
                {
                    food.ResetFood();
                }
            }

            foodsInBowl.Clear();
            fruitCount = 0;
            vegetableCount = 0;
            grainCount = 0;
            isComplete = false;

            ShowStatus("🧘 Le bol a été vidé. Recommence la quête.", 3f);
        }
    }
}
