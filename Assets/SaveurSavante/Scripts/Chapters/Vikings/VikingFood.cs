using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using SaveurSavante.Interactions;

namespace SaveurSavante.Chapters.Vikings
{
    public class VikingFood : MonoBehaviour
    {
        [Header("Propriétés")]
        public string foodName;
        public float rawEnergyValue = 15f;
        public float rawSatietyValue = 15f;
        public float cookedEnergyValue = 30f;
        public float cookedSatietyValue = 30f;
        public float currentEnergy;
        public float currentSatiety;
        public string foodType; // "viande", "poisson", "pain", "legume"

        [Header("Cuisson")]
        public bool canBeCooked = true;
        public bool isCooked = false;
        public Material rawMaterial;
        public Material cookedMaterial;

        [Header("Manger")]
        public bool canBeEaten = true;
        public bool hasBeenEaten = false;
        public float eatDelay = 0.5f;

        [Header("Placement")]
        public Transform placementSpot;
        public bool isPlaced = false;

        [Header("Feedback")]
        public AudioClip eatSound;
        public ParticleSystem eatParticles;

        [Header("Cooked Variant")]
        public GameObject cookedVariant;
        public bool autoFindCookedVariant = true;

        private XRGrabInteractable grabInteractable;
        private XRBaseInteractor currentInteractor;
        private Renderer objectRenderer;
        private Rigidbody rb;
        private GrabOutlineFeedback outlineFeedback;
        private float lastEatTime = 0f;

        private void Awake()
        {
            grabInteractable = GetComponent<XRGrabInteractable>();
            objectRenderer = GetComponentInChildren<Renderer>();
            rb = GetComponent<Rigidbody>();
            outlineFeedback = new GrabOutlineFeedback(gameObject);

            // Valeurs initiales (aliment cru)
            currentEnergy = rawEnergyValue;
            currentSatiety = rawSatietyValue;

            if (grabInteractable != null)
            {
                grabInteractable.selectEntered.AddListener(OnGrabbed);
                grabInteractable.selectExited.AddListener(OnReleased);
                grabInteractable.activated.AddListener(OnActivated);
                grabInteractable.hoverEntered.AddListener(OnHoverEntered);
                grabInteractable.hoverExited.AddListener(OnHoverExited);
            }

            if (autoFindCookedVariant && cookedVariant == null)
            {
                cookedVariant = FindCookedVariant();
            }

            if (IsCookedVariantObject())
            {
                isCooked = true;
                canBeCooked = false;
                currentEnergy = cookedEnergyValue;
                currentSatiety = cookedSatietyValue;
            }

            ApplyPhysicsState(!isPlaced);
        }

        private void OnGrabbed(SelectEnterEventArgs args)
        {
            currentInteractor = args.interactorObject as XRBaseInteractor;
            outlineFeedback?.SetVisible(false);

            if (isPlaced && canBeEaten && !hasBeenEaten)
            {
                // Grabbing food from table = eating it
                ApplyPhysicsState(true);
                EatFood();
                return;
            }

            ApplyPhysicsState(true);
        }

        private void OnReleased(SelectExitEventArgs args)
        {
            currentInteractor = null;

            // Vérifier si l'aliment est proche d'une place sur la table
            if (!isPlaced && !hasBeenEaten)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, 0.5f);
                foreach (var collider in colliders)
                {
                    if (collider.CompareTag("TableSpot"))
                    {
                        PlaceOnTable(collider.transform);
                        return;
                    }
                }
            }

            if (!isPlaced)
            {
                ApplyPhysicsState(true);
            }
        }

        private void OnActivated(ActivateEventArgs args)
        {
            // Manger l'aliment quand on clique sur la gâchette
            if (isPlaced && canBeEaten && !hasBeenEaten)
            {
                if (Time.time - lastEatTime >= eatDelay)
                {
                    EatFood();
                }
            }
        }

        private void OnHoverEntered(HoverEnterEventArgs args)
        {
            outlineFeedback?.SetVisible(true);
        }

        private void OnHoverExited(HoverExitEventArgs args)
        {
            outlineFeedback?.SetVisible(false);
        }

        private void EatFood()
        {
            lastEatTime = Time.time;
            hasBeenEaten = true;

            // Son de manger
            if (eatSound != null)
            {
                AudioSource.PlayClipAtPoint(eatSound, transform.position);
            }

            // Particules
            if (eatParticles != null)
            {
                eatParticles.Play();
            }

            // Notifier le NutritionManager
            NutritionManager manager = FindObjectOfType<NutritionManager>();
            if (manager != null)
            {
                manager.EatFood(this);
            }

            // Animation de "manger" (réduire la taille puis disparaître)
            StartCoroutine(EatAnimation());

            Debug.Log($"🍖 {foodName} mangé ! +{currentEnergy} énergie, +{currentSatiety} satiété");
        }

        private System.Collections.IEnumerator EatAnimation()
        {
            float duration = 0.5f;
            float elapsed = 0f;
            Vector3 originalScale = transform.localScale;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
                yield return null;
            }

            // Désactiver l'aliment après l'avoir mangé
            gameObject.SetActive(false);
        }

        public void Cook()
        {
            if (isCooked) return;
            isCooked = true;
            canBeCooked = false;

            currentEnergy = cookedEnergyValue;
            currentSatiety = cookedSatietyValue;

            if (objectRenderer != null && cookedMaterial != null)
            {
                objectRenderer.material = cookedMaterial;
            }

            // Notifier le NutritionManager (incremente la satiete directement)
            NutritionManager manager = FindObjectOfType<NutritionManager>();
            if (manager != null) manager.RegisterCookedFood(this);

            // Disable grab/collisions
            if (grabInteractable != null) grabInteractable.enabled = false;
            foreach (var col in GetComponentsInChildren<Collider>(true)) col.enabled = false;
            if (rb != null) { rb.isKinematic = true; rb.useGravity = false; rb.velocity = Vector3.zero; }

            StartCoroutine(FlyToSkyAndDestroy());

            Debug.Log($"{foodName} cuit ! Envol vers le ciel.");
        }

        private System.Collections.IEnumerator FlyToSkyAndDestroy()
        {
            float duration = 1.5f;
            float elapsed = 0f;
            Vector3 startPos = transform.position;
            Vector3 endPos = startPos + Vector3.up * 4f;
            Vector3 startScale = transform.localScale;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                transform.position = Vector3.Lerp(startPos, endPos, t);
                transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
                transform.Rotate(0f, 180f * Time.deltaTime, 0f, Space.Self);
                yield return null;
            }

            gameObject.SetActive(false);
        }

        private bool TrySwapToCookedVariant()
        {
            if (cookedVariant == null)
                return false;

            var cookedFood = cookedVariant.GetComponent<VikingFood>();
            if (cookedFood == null)
                return false;

            cookedVariant.transform.SetPositionAndRotation(transform.position, transform.rotation);
            cookedVariant.transform.localScale = transform.localScale;

            cookedFood.foodName = string.IsNullOrWhiteSpace(foodName) ? gameObject.name : foodName;
            cookedFood.currentEnergy = cookedFood.cookedEnergyValue;
            cookedFood.currentSatiety = cookedFood.cookedSatietyValue;
            cookedFood.isCooked = true;
            cookedFood.canBeCooked = false;
            cookedFood.hasBeenEaten = false;
            cookedFood.isPlaced = isPlaced;
            cookedFood.placementSpot = placementSpot;
            cookedFood.ApplyPhysicsState(!isPlaced);

            if (cookedMaterial != null)
            {
                var cookedRenderer = cookedVariant.GetComponentInChildren<Renderer>();
                if (cookedRenderer != null)
                {
                    cookedRenderer.material = cookedMaterial;
                }
            }

            cookedVariant.SetActive(true);
            gameObject.SetActive(false);

            return true;
        }

        private GameObject FindCookedVariant()
        {
            if (IsCookedVariantObject() || transform.parent == null)
                return null;

            var objectName = gameObject.name.ToLowerInvariant();
            string cookedName = null;

            if (objectName.Contains("fresh fish") || objectName.Contains("fish"))
                cookedName = "fried_fish";
            else if (objectName.Contains("ham"))
                cookedName = "fried_ham";
            else if (objectName.Contains("meat"))
                cookedName = "fried_meat";

            return cookedName == null ? null : transform.parent.Find(cookedName)?.gameObject;
        }

        private bool IsCookedVariantObject()
        {
            return gameObject.name.StartsWith("fried_");
        }

        private System.Collections.IEnumerator CookingAnimation()
        {
            Vector3 originalScale = transform.localScale;
            Vector3 originalPosition = transform.position;
            Vector3 targetScale = originalScale * 0.92f;
            Vector3 lowerPosition = originalPosition + Vector3.down * 0.08f;

            // Petit bump vers le bas puis retour pour signaler la cuisson
            float elapsed = 0f;
            while (elapsed < 0.12f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / 0.12f;
                transform.position = Vector3.Lerp(originalPosition, lowerPosition, t);
                transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
                yield return null;
            }

            // Revenir au point d'origine
            elapsed = 0f;
            while (elapsed < 0.12f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / 0.12f;
                transform.position = Vector3.Lerp(lowerPosition, originalPosition, t);
                transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
                yield return null;
            }

            transform.position = originalPosition;
            transform.localScale = originalScale;
        }

        private void PlaceOnTable(Transform spot)
        {
            placementSpot = spot;
            isPlaced = true;

            // Snap à la position
            transform.position = spot.position;
            transform.rotation = spot.rotation;
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            ApplyPhysicsState(false);

            // Notifier le NutritionManager
            NutritionManager manager = FindObjectOfType<NutritionManager>();
            if (manager != null)
            {
                manager.AddFood(this, spot);
            }

            Debug.Log($"🍖 {foodName} placé sur la table ! Clique pour manger !");
        }

        public void ResetFood()
        {
            isPlaced = false;
            hasBeenEaten = false;
            gameObject.SetActive(true);
            transform.localScale = Vector3.one;
            ApplyPhysicsState(true);

            if (grabInteractable != null)
            {
                grabInteractable.enabled = true;
            }
        }

        private void ApplyPhysicsState(bool dynamic)
        {
            if (rb == null)
                return;

            // No gravity — items stay fixed in air when released
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.detectCollisions = true;
            rb.interpolation = RigidbodyInterpolation.None;
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        private void OnDestroy()
        {
            if (grabInteractable != null)
            {
                grabInteractable.selectEntered.RemoveListener(OnGrabbed);
                grabInteractable.selectExited.RemoveListener(OnReleased);
                grabInteractable.activated.RemoveListener(OnActivated);
                grabInteractable.hoverEntered.RemoveListener(OnHoverEntered);
                grabInteractable.hoverExited.RemoveListener(OnHoverExited);
            }

            outlineFeedback?.Dispose();
        }
    }
}
