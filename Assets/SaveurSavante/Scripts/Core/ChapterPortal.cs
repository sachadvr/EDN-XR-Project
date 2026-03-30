using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;
using SaveurSavante.Core;

namespace SaveurSavante.Core
{
    public class ChapterPortal : MonoBehaviour
    {
        [Header("Configuration")]
        public string chapterName; // "Egypte", "Vikings", "Titanic", "Gandhi"
        public Transform teleportDestination;
        public bool isCompleted = false;

        [Header("Visual")]
        public Material activeMaterial;
        public Material completedMaterial;
        public ParticleSystem portalEffect;

        [Header("Audio")]
        public AudioClip teleportSound;

        private Renderer portalRenderer;
        private XRSimpleInteractable simpleInteractable;

        private void Awake()
        {
            portalRenderer = GetComponent<Renderer>();
            simpleInteractable = GetComponent<XRSimpleInteractable>();

            if (simpleInteractable != null)
            {
                simpleInteractable.selectEntered.AddListener(OnPortalActivated);
            }
        }

        private void Start()
        {
            UpdateVisualState();

            // S'abonner aux événements du GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnChapterCompleted += OnAnyChapterCompleted;
            }
        }

        private void OnAnyChapterCompleted()
        {
            // Vérifier si ce chapitre est complété
            CheckCompletionStatus();
        }

        private void CheckCompletionStatus()
        {
            if (GameManager.Instance == null) return;

            switch (chapterName.ToLower())
            {
                case "egypte":
                    isCompleted = GameManager.Instance.egypteComplete;
                    break;
                case "vikings":
                    isCompleted = GameManager.Instance.vikingsComplete;
                    break;
                case "titanic":
                    isCompleted = GameManager.Instance.titanicComplete;
                    break;
                case "gandhi":
                    isCompleted = GameManager.Instance.gandhiComplete;
                    break;
            }

            UpdateVisualState();
        }

        private void UpdateVisualState()
        {
            if (isCompleted && completedMaterial != null)
            {
                portalRenderer.material = completedMaterial;
                if (portalEffect != null)
                {
                    portalEffect.Stop();
                }
            }
            else if (activeMaterial != null)
            {
                portalRenderer.material = activeMaterial;
                if (portalEffect != null && !portalEffect.isPlaying)
                {
                    portalEffect.Play();
                }
            }
        }

        private void OnPortalActivated(SelectEnterEventArgs args)
        {
            if (isCompleted)
            {
                Debug.Log($"🚪 Portail {chapterName} déjà complété !");
                return;
            }

            TeleportPlayer(args.interactorObject.transform);
        }

        private void TeleportPlayer(Transform playerTransform)
        {
            if (teleportDestination == null)
            {
                Debug.LogError($"❌ Destination de téléportation non définie pour {chapterName}");
                return;
            }

            // Jouer le son
            if (teleportSound != null)
            {
                AudioSource.PlayClipAtPoint(teleportSound, transform.position);
            }

            // Téléporter le joueur
            XROrigin xrOrigin = FindObjectOfType<XROrigin>();
            if (xrOrigin != null)
            {
                xrOrigin.transform.position = teleportDestination.position;
                xrOrigin.transform.rotation = teleportDestination.rotation;

                Debug.Log($"✨ Téléportation vers {chapterName} ! Position: {teleportDestination.position}");

                if (StoryManager.Instance != null)
                {
                    StoryManager.Instance.ShowIntroduction(chapterName);
                }
            }
        }

        private void OnDestroy()
        {
            if (simpleInteractable != null)
            {
                simpleInteractable.selectEntered.RemoveListener(OnPortalActivated);
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnChapterCompleted -= OnAnyChapterCompleted;
            }
        }
    }
}
