using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;
using SaveurSavante.Core;
using SaveurSavante.Chapters.Gandhi;
using SaveurSavante.Chapters.Egypte;
using SaveurSavante.Chapters.Vikings;
using SaveurSavante.Chapters.Titanic;

namespace SaveurSavante.Core
{
    [RequireComponent(typeof(Collider))]
    public class ChapterPortal : MonoBehaviour
    {
        [Header("Configuration")]
        public string chapterName; // "Egypte", "Vikings", "Titanic", "Gandhi"
        public Transform teleportDestination;
        public bool isCompleted = false;
        public float teleportCooldown = 0.5f;

        [Header("Visual")]
        public Material activeMaterial;
        public Material completedMaterial;
        public ParticleSystem portalEffect;

        [Header("Audio")]
        public AudioClip teleportSound;

        private Renderer portalRenderer;
        private XRSimpleInteractable simpleInteractable;
        private float lastTeleportTime = float.NegativeInfinity;

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
            CheckCompletionStatus();
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
            XROrigin xrOrigin = args.interactorObject.transform.GetComponentInParent<XROrigin>();
            if (xrOrigin == null)
            {
                xrOrigin = FindObjectOfType<XROrigin>();
            }

            TeleportPlayer(xrOrigin);
        }

        private void OnTriggerEnter(Collider other)
        {
            XROrigin xrOrigin = other.GetComponentInParent<XROrigin>();
            if (xrOrigin == null)
            {
                return;
            }

            TeleportPlayer(xrOrigin);
        }

        private void TeleportPlayer(XROrigin xrOrigin)
        {
            if (isCompleted)
            {
                Debug.Log($"🚪 Portail {chapterName} déjà complété !");
                return;
            }

            if (teleportDestination == null)
            {
                Debug.LogError($"❌ Destination de téléportation non définie pour {chapterName}");
                return;
            }

            if (xrOrigin == null)
            {
                xrOrigin = FindObjectOfType<XROrigin>();
                if (xrOrigin == null)
                {
                    Debug.LogError($"❌ Aucun XROrigin trouvé pour téléporter vers {chapterName}");
                    return;
                }
            }

            if (Time.time - lastTeleportTime < teleportCooldown)
            {
                return;
            }

            lastTeleportTime = Time.time;

            // Jouer le son
            if (teleportSound != null)
            {
                AudioSource.PlayClipAtPoint(teleportSound, transform.position);
            }

            // Téléporter le joueur
            xrOrigin.transform.SetPositionAndRotation(teleportDestination.position, teleportDestination.rotation);

            Debug.Log($"✨ Téléportation vers {chapterName} ! Position: {teleportDestination.position}");

            if (StoryManager.Instance != null)
            {
                StoryManager.Instance.ShowIntroduction(chapterName);
            }

            ActivateChapter();
        }

        private void ActivateChapter()
        {
            switch (chapterName.ToLower())
            {
                case "gandhi":
                    var bowl = FindObjectOfType<BowlManager>();
                    if (bowl != null) bowl.ShowIntro();
                    break;
                case "egypte":
                    var jar = FindObjectOfType<Jarre>();
                    if (jar != null) jar.ShowIntro();
                    break;
                case "vikings":
                    var nm = FindObjectOfType<NutritionManager>();
                    if (nm != null) nm.ShowIntro();
                    break;
                case "titanic":
                    var pm = FindObjectOfType<PlateManager>();
                    if (pm != null) pm.ShowIntro();
                    break;
            }
        }

        public void MarkCompletedAndDisable()
        {
            isCompleted = true;
            UpdateVisualState();
            if (simpleInteractable != null) simpleInteractable.enabled = false;
            var col = GetComponent<Collider>();
            if (col != null) col.enabled = false;
            if (portalEffect != null) portalEffect.Stop();
            Debug.Log($"Portail {chapterName} desactive (chapitre termine).");
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
