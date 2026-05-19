using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using SaveurSavante.Interactions;

namespace SaveurSavante.Chapters.Titanic
{
    public class TitanicFood : MonoBehaviour
    {
        [Header("Propriétés")]
        public string foodName;
        public string flavorProfile; // "sucré", "salé", "acide", "amer"
        public string foodCategory; // "entrée", "plat", "garniture", "dessert"
        public float presentationValue = 20f;

        [Header("Visual")]
        public GameObject platedPrefab;
        public Material highlightMaterial;
        private Material originalMaterial;

        [Header("État")]
        public bool isPlaced = false;

        private XRGrabInteractable grabInteractable;
        private Renderer objectRenderer;
        private GrabOutlineFeedback outlineFeedback;
        private Rigidbody rb;
        private Vector3 originalPosition;
        private Quaternion originalRotation;

        private void Awake()
        {
            grabInteractable = GetComponent<XRGrabInteractable>();
            objectRenderer = GetComponentInChildren<Renderer>();
            rb = GetComponent<Rigidbody>();
            outlineFeedback = new GrabOutlineFeedback(gameObject);

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

            // Réinitialiser si on reprend un aliment déjà placé
            if (isPlaced)
            {
                isPlaced = false;
                transform.SetParent(null);
            }

            // Highlight
            if (objectRenderer != null && highlightMaterial != null)
            {
                objectRenderer.material = highlightMaterial;
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

            if (isPlaced) return;

            PlateManager plate = FindNearestPlate(1.2f);
            if (plate != null)
            {
                Transform spot = FindNearestSpot(plate.transform);
                if (spot != null) { PlaceOnPlate(plate, spot); return; }
                PlaceOnPlate(plate, plate.transform);
                return;
            }

            // Pas placé sur assiette → reste figé (pas de gravité)
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
                rb.useGravity = false;
            }
        }

        private PlateManager FindNearestPlate(float maxDistance)
        {
            PlateManager best = null;
            float bestSqr = maxDistance * maxDistance;
            foreach (var p in FindObjectsOfType<PlateManager>())
            {
                float d = (p.transform.position - transform.position).sqrMagnitude;
                if (d < bestSqr) { bestSqr = d; best = p; }
            }
            return best;
        }

        private Transform FindNearestSpot(Transform plateTransform)
        {
            // Chercher les spots d'assiette disponibles
            Transform[] spots = plateTransform.GetComponentsInChildren<Transform>();
            foreach (var spot in spots)
            {
                if (spot.CompareTag("PlateSpot") && spot.childCount == 0)
                {
                    return spot;
                }
            }
            return null;
        }

        private void PlaceOnPlate(PlateManager plate, Transform spot)
        {
            isPlaced = true;

            transform.position = spot.position;
            transform.rotation = spot.rotation;
            transform.SetParent(spot);

            // Lock physics: kinematic + no gravity pour rester sur le spot
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            // Désactiver le grab une fois placé
            if (grabInteractable != null)
            {
                grabInteractable.enabled = false;
            }

            // Essayer d'abord avec FoodGuidance si présent
            FoodGuidance guidance = FindObjectOfType<FoodGuidance>();
            if (guidance != null)
            {
                guidance.ValidateFood(this, spot);
            }

            // Puis avec PlateManager
            plate.AddFood(this, spot);

            Debug.Log($"🍽️ {foodName} ({flavorProfile}) placé sur l'assiette !");
        }

        public void ReturnToSpawn()
        {
            isPlaced = false;
            transform.SetParent(null);
            transform.position = originalPosition;
            transform.rotation = originalRotation;

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
