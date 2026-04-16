using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SaveurSavante.Core
{
    public class GlobalStatusHUD : MonoBehaviour
    {
        private TextMeshProUGUI hudText;
        private Transform mainCamera;

        void Start()
        {
            if (Camera.main != null)
            {
                mainCamera = Camera.main.transform;
                CreateMinecraftStyleSidebar();
            }
        }

        void CreateMinecraftStyleSidebar()
        {
            // 1. Création du Canvas WorldSpace
            GameObject canvasObj = new GameObject("SaveurSavante_Sidebar");
            canvasObj.transform.SetParent(mainCamera);
            
            // Placement à droite, légèrement en haut, à 1 mètre de distance
            canvasObj.transform.localPosition = new Vector3(0.8f, 0.1f, 1.2f);
            canvasObj.transform.localRotation = Quaternion.identity;
            canvasObj.transform.localScale = new Vector3(0.0015f, 0.0015f, 0.0015f);

            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(300f, 400f);

            // 2. Création du fond noir semi-transparent (genre Minecraft)
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(canvasObj.transform, false);
            
            Image bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0f, 0f, 0f, 0.65f); // Noir à 65% d'opacité
            
            RectTransform bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0, 0);
            bgRect.anchorMax = new Vector2(1, 1);
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            // 3. Création du Texte
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(canvasObj.transform, false);
            
            hudText = textObj.AddComponent<TextMeshProUGUI>();
            hudText.fontSize = 20f;
            hudText.alignment = TextAlignmentOptions.TopLeft;
            hudText.color = new Color(1f, 0.85f, 0.3f); // Texte un peu doré
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0, 0);
            textRect.anchorMax = new Vector2(1, 1);
            textRect.offsetMin = new Vector2(15f, 15f); // Marge interne
            textRect.offsetMax = new Vector2(-15f, -15f); // Marge interne

            UpdateHUDText("Chargement...");
        }

        void Update()
        {
            if (hudText != null && GameManager.Instance != null)
            {
                string status = "<color=white><b>OBJECTIFS</b></color>\n\n";
                int completed = GameManager.Instance.GetCompletedChaptersCount();
                status += $"Progression: {completed}/4\n\n";

                // Titanic
                var plateManager = FindObjectOfType<Chapters.Titanic.PlateManager>();
                if (plateManager != null)
                {
                    status += $"<color=lightblue>Titanic</color>\nAssiette: {plateManager.placedFoods.Count}/5\nScore: {plateManager.presentationScore}\n\n";
                }

                // Vikings
                var nutritionManager = FindObjectOfType<Chapters.Vikings.NutritionManager>();
                if (nutritionManager != null)
                {
                    status += $"<color=orange>Vikings</color>\nPosés: {nutritionManager.placedFoods.Count}\nMangés: {nutritionManager.eatenFoods.Count}\nÉnergie: {nutritionManager.currentEnergy:F0}/{nutritionManager.targetEnergy:F0}\nSatiété: {nutritionManager.currentSatiety:F0}/{nutritionManager.targetSatiety:F0}\n\n";
                }

                // Egypte
                var jarre = FindObjectOfType<Chapters.Egypte.Jarre>();
                if (jarre != null)
                {
                    status += $"<color=orange>Egypte</color>\nJarre: {jarre.foodsInJar.Count}/3\n\n";
                }

                // Gandhi
                var bowl = FindObjectOfType<Chapters.Gandhi.BowlManager>();
                if (bowl != null)
                {
                    status += $"<color=green>Gandhi</color>\nBol: {bowl.placedFoods.Count} offrandes\n\n";
                }

                UpdateHUDText(status);
            }
        }

        public void UpdateHUDText(string text)
        {
            if (hudText != null)
            {
                hudText.text = text;
            }
        }
    }
}
