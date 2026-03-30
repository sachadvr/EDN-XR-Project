using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

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
        private Vector3 originalPosition;
        private Quaternion originalRotation;

        private void Awake()
        {
            grabInteractable = GetComponent<XRGrabInteractable>();
            objectRenderer = GetComponentInChildren<Renderer>();

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
            }
        }

        private void OnGrabbed(SelectEnterEventArgs args)
        {
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

        private void OnReleased(SelectExitEventArgs args)
        {
            // Restaurer material
            if (objectRenderer != null && originalMaterial != null)
            {
                objectRenderer.material = originalMaterial;
            }

            if (isPlaced) return;

            // Vérifier si l'aliment est au-dessus de l'assiette
            Collider[] colliders = Physics.OverlapSphere(transform.position, 0.3f);
            foreach (var collider in colliders)
            {
                PlateManager plate = collider.GetComponent<PlateManager>();
                if (plate != null)
                {
                    // Trouver la place libre la plus proche
                    Transform spot = FindNearestSpot(plate.transform);
                    if (spot != null)
                    {
                        PlaceOnPlate(plate, spot);
                        return;
                    }
                }
            }
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
            }
        }
    }
}
