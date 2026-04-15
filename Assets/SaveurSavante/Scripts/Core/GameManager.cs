using UnityEngine;
using System;

namespace SaveurSavante.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Chapitres complétés")]
        public bool egypteComplete = false;
        public bool vikingsComplete = false;
        public bool titanicComplete = false;
        public bool gandhiComplete = false;

        [Header("Position retour Hub")]
        public Vector3 hubPosition = new Vector3(0, 1.5f, 0);

        public event Action OnChapterCompleted;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                
                // Force le HUD global 3D devant les yeux
                gameObject.AddComponent<GlobalStatusHUD>();
                
                // --- AUTO FIX VR SCENE ---
                // Corrige les aliments qui tombent à l'infini (MeshCollider non convex)
                foreach (MeshCollider mc in FindObjectsOfType<MeshCollider>(true))
                {
                    Rigidbody rb = mc.GetComponent<Rigidbody>();
                    if (rb != null && !rb.isKinematic && !mc.convex)
                    {
                        mc.convex = true;
                    }
                }
                
                // Corrige les Canvas (Panel) invisibles en VR
                foreach (Canvas canvas in FindObjectsOfType<Canvas>(true))
                {
                    if (canvas.renderMode != RenderMode.WorldSpace)
                    {
                        canvas.renderMode = RenderMode.WorldSpace;
                        RectTransform rt = canvas.GetComponent<RectTransform>();
                        if (rt != null)
                        {
                            rt.localScale = new Vector3(0.002f, 0.002f, 0.002f);
                            if (rt.position == Vector3.zero) rt.position = new Vector3(0, 1.5f, 2f);
                        }
                        
                        var gr = canvas.GetComponent<UnityEngine.UI.GraphicRaycaster>();
                        if (gr != null && canvas.GetComponent<UnityEngine.XR.Interaction.Toolkit.UI.TrackedDeviceGraphicRaycaster>() == null)
                        {
                            canvas.gameObject.AddComponent<UnityEngine.XR.Interaction.Toolkit.UI.TrackedDeviceGraphicRaycaster>();
                            Destroy(gr); // Remplacé par la version XR
                        }

                        // Attach the HUD tracking so it stays on screen
                        if (canvas.GetComponent<SaveurSavante.Core.VRHUD>() == null)
                        {
                            canvas.gameObject.AddComponent<SaveurSavante.Core.VRHUD>();
                        }
                    }
                }
                // -------------------------
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void CompleteChapter(string chapterName)
        {
            switch (chapterName.ToLower())
            {
                case "egypte":
                    egypteComplete = true;
                    break;
                case "vikings":
                    vikingsComplete = true;
                    break;
                case "titanic":
                    titanicComplete = true;
                    break;
                case "gandhi":
                    gandhiComplete = true;
                    break;
            }

            OnChapterCompleted?.Invoke();

            // Vérifier si tous les chapitres sont complétés
            if (AllChaptersComplete())
            {
                Debug.Log("🎉 Félicitations ! Vous avez complété tous les chapitres de Saveur Savante !");
            }
        }

        public bool AllChaptersComplete()
        {
            return egypteComplete && vikingsComplete && titanicComplete && gandhiComplete;
        }

        public int GetCompletedChaptersCount()
        {
            int count = 0;
            if (egypteComplete) count++;
            if (vikingsComplete) count++;
            if (titanicComplete) count++;
            if (gandhiComplete) count++;
            return count;
        }
    }
}
