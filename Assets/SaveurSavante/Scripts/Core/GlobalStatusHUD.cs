using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SaveurSavante.Core
{
    public class GlobalStatusHUD : MonoBehaviour
    {
        private TextMeshProUGUI hudText;
        private GameObject canvasObj;
        private Transform mainCamera;

        void Update()
        {
            // Toujours s'assurer qu'on a une camera et un sidebar
            EnsureCamera();
            EnsureSidebar();
            ForceVisible();
            RefreshContent();
        }

        void LateUpdate()
        {
            ForceVisible();
        }

        private void EnsureCamera()
        {
            if (mainCamera != null && mainCamera.gameObject.activeInHierarchy) return;
            var cam = Camera.main;
            if (cam == null)
            {
                cam = FindObjectOfType<Camera>();
            }
            if (cam != null) mainCamera = cam.transform;
        }

        private void EnsureSidebar()
        {
            if (mainCamera == null) return;

            if (canvasObj == null)
            {
                CreateMinecraftStyleSidebar();
                return;
            }
            // Reparent si Camera.main a change
            if (canvasObj.transform.parent != mainCamera)
            {
                canvasObj.transform.SetParent(mainCamera, false);
                canvasObj.transform.localPosition = new Vector3(0.8f, 0.1f, 1.2f);
                canvasObj.transform.localRotation = Quaternion.identity;
                canvasObj.transform.localScale = new Vector3(0.0015f, 0.0015f, 0.0015f);
            }
        }

        private void ForceVisible()
        {
            if (canvasObj == null) return;
            if (!canvasObj.activeSelf) canvasObj.SetActive(true);
            foreach (var t in canvasObj.GetComponentsInChildren<Transform>(true))
            {
                if (!t.gameObject.activeSelf) t.gameObject.SetActive(true);
            }
            var cg = canvasObj.GetComponent<CanvasGroup>();
            if (cg != null) { cg.alpha = 1f; cg.interactable = true; cg.blocksRaycasts = true; }
        }

        void CreateMinecraftStyleSidebar()
        {
            canvasObj = new GameObject("SaveurSavante_Sidebar");
            canvasObj.transform.SetParent(mainCamera);
            canvasObj.transform.localPosition = new Vector3(0.8f, 0.1f, 1.2f);
            canvasObj.transform.localRotation = Quaternion.identity;
            canvasObj.transform.localScale = new Vector3(0.0015f, 0.0015f, 0.0015f);

            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvasObj.AddComponent<CanvasGroup>();

            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(300f, 400f);

            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(canvasObj.transform, false);
            Image bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0f, 0f, 0f, 0.65f);
            RectTransform bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0, 0);
            bgRect.anchorMax = new Vector2(1, 1);
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(canvasObj.transform, false);
            hudText = textObj.AddComponent<TextMeshProUGUI>();
            hudText.fontSize = 20f;
            hudText.alignment = TextAlignmentOptions.TopLeft;
            hudText.color = new Color(1f, 0.85f, 0.3f);
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0, 0);
            textRect.anchorMax = new Vector2(1, 1);
            textRect.offsetMin = new Vector2(15f, 15f);
            textRect.offsetMax = new Vector2(-15f, -15f);
        }

        private void RefreshContent()
        {
            if (hudText == null || GameManager.Instance == null) return;

            string status = "<color=white><b>OBJECTIFS</b></color>\n\n";
            int completed = GameManager.Instance.GetCompletedChaptersCount();
            status += $"Progression: {completed}/4\n\n";

            var plateManager = FindObjectOfType<Chapters.Titanic.PlateManager>();
            if (plateManager != null)
            {
                status += $"<color=lightblue>Titanic</color>\nAssiette: {plateManager.placedFoods.Count}/5\nScore: {plateManager.presentationScore}\n\n";
            }

            var nutritionManager = FindObjectOfType<Chapters.Vikings.NutritionManager>();
            if (nutritionManager != null)
            {
                status += $"<color=orange>Vikings</color>\nCuits: {nutritionManager.placedFoods.Count}\nSatiete: {nutritionManager.currentSatiety:F0}/{nutritionManager.targetSatiety:F0}\n\n";
            }

            var jarre = FindObjectOfType<Chapters.Egypte.Jarre>();
            if (jarre != null)
            {
                status += $"<color=orange>Egypte</color>\nJarre: {jarre.foodsInJar.Count}/{jarre.requiredFoodCount}\n\n";
            }

            var bowl = FindObjectOfType<Chapters.Gandhi.BowlManager>();
            if (bowl != null)
            {
                status += $"<color=green>Gandhi</color>\nBol: {bowl.placedFoods.Count} offrandes\n\n";
            }

            hudText.text = status;
        }

        public void UpdateHUDText(string text)
        {
            if (hudText != null) hudText.text = text;
        }
    }
}
