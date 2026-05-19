using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using SaveurSavante.Interactions;

namespace SaveurSavante.Chapters.Gandhi
{
    public class GandhiFood : MonoBehaviour
    {
        [Header("Propriétés")]
        public string foodName;
        public string foodType; // "fruit", "legume", "graine"
        public float energyValue = 25f;

        [Header("Placement")]
        public Transform spawnPosition;
        public Transform bowlCenter;
        public bool isInBowl = false;

        [Header("Visual")]
        public Material highlightMaterial;
        private Material originalMaterial;

        [Header("État")]
        public Vector3 originalPosition;
        public Quaternion originalRotation;

        private XRGrabInteractable grabInteractable;
        private Renderer objectRenderer;
        private GrabOutlineFeedback outlineFeedback;
        private Rigidbody rb;

        private void Awake()
        {
            grabInteractable = GetComponent<XRGrabInteractable>();
            objectRenderer = GetComponentInChildren<Renderer>();
            outlineFeedback = new GrabOutlineFeedback(gameObject);
            rb = GetComponent<Rigidbody>();

            // Sauvegarder la position initiale
            originalPosition = transform.position;
            originalRotation = transform.rotation;

            if (objectRenderer != null)
            {
                originalMaterial = objectRenderer.material;
            }

            if (grabInteractable != null)
            {
                grabInteractable.selectEntered.AddListener(OnGrabbed);
                grabInteractable.selectExited.AddListener(OnReleased);
                grabInteractable.hoverEntered.AddListener(OnHoverEntered);
                grabInteractable.hoverExited.AddListener(OnHoverExited);
            }
        }

        private void OnGrabbed(SelectEnterEventArgs args)
        {
            outlineFeedback?.SetVisible(false);

            // Highlight quand on prend
            if (objectRenderer != null && highlightMaterial != null)
            {
                objectRenderer.material = highlightMaterial;
            }

            // Retirer du bol si on le reprend
            if (isInBowl)
            {
                isInBowl = false;
                transform.SetParent(null);
            }
        }

        private void OnHoverEntered(HoverEnterEventArgs args)
        {
            if (objectRenderer != null && highlightMaterial != null)
            {
                objectRenderer.material = highlightMaterial;
                return;
            }

            outlineFeedback?.SetVisible(true);
        }

        private void OnHoverExited(HoverExitEventArgs args)
        {
            if (objectRenderer != null && originalMaterial != null)
            {
                objectRenderer.material = originalMaterial;
                return;
            }

            outlineFeedback?.SetVisible(false);
        }

        private void OnReleased(SelectExitEventArgs args)
        {
            // Restaurer material
            if (objectRenderer != null && originalMaterial != null)
            {
                objectRenderer.material = originalMaterial;
            }

            if (isInBowl) return;

            BowlManager bowl = FindNearestBowl(1.2f);
            if (bowl != null)
            {
                TreasureHunt treasureHunt = FindObjectOfType<TreasureHunt>();
                if (treasureHunt != null && !treasureHunt.isComplete)
                {
                    bool correct = treasureHunt.TrySolveRiddle(this, transform);
                    if (correct) { SnapToBowl(bowl.transform); return; }
                    ReturnToSpawn();
                    return;
                }
                SnapToBowl(bowl.transform);
                return;
            }
        }

        private BowlManager FindNearestBowl(float maxDistance)
        {
            BowlManager best = null;
            float bestSqr = maxDistance * maxDistance;
            foreach (var b in FindObjectsOfType<BowlManager>())
            {
                float d = (b.transform.position - transform.position).sqrMagnitude;
                if (d < bestSqr) { bestSqr = d; best = b; }
            }
            return best;
        }

        private void SnapToBowl(Transform bowl)
        {
            isInBowl = true;

            // Position aléatoire dans le bol
            Vector3 randomOffset = new Vector3(
                Random.Range(-0.1f, 0.1f),
                0.05f,
                Random.Range(-0.1f, 0.1f)
            );

            transform.position = bowl.position + randomOffset;
            transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            transform.SetParent(bowl);

            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            // Désactiver le grab une fois dans le bol
            if (grabInteractable != null)
            {
                grabInteractable.enabled = false;
            }

            // Notifier le BowlManager
            BowlManager manager = bowl.GetComponent<BowlManager>();
            if (manager != null)
            {
                manager.AddFood(this);
            }

            Debug.Log($"🧘 {foodName} ({foodType}) placé dans le bol !");
        }

        public void ReturnToSpawn()
        {
            isInBowl = false;
            transform.SetParent(null);

            if (spawnPosition != null)
            {
                transform.position = spawnPosition.position;
                transform.rotation = spawnPosition.rotation;
            }
            else
            {
                transform.position = originalPosition;
                transform.rotation = originalRotation;
            }

            // Réactiver le grab
            if (grabInteractable != null)
            {
                grabInteractable.enabled = true;
            }
        }

        public void ResetFood()
        {
            ReturnToSpawn();
        }

        private void OnDestroy()
        {
            if (grabInteractable != null)
            {
                grabInteractable.selectEntered.RemoveListener(OnGrabbed);
                grabInteractable.selectExited.RemoveListener(OnReleased);
                grabInteractable.hoverEntered.RemoveListener(OnHoverEntered);
                grabInteractable.hoverExited.RemoveListener(OnHoverExited);
            }

            outlineFeedback?.Dispose();
        }
    }
}
