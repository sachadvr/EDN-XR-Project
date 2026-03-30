using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

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

        private XRGrabInteractable grabInteractable;
        private XRBaseInteractor currentInteractor;
        private Renderer objectRenderer;
        private float lastEatTime = 0f;

        private void Awake()
        {
            grabInteractable = GetComponent<XRGrabInteractable>();
            objectRenderer = GetComponentInChildren<Renderer>();

            // Valeurs initiales (aliment cru)
            currentEnergy = rawEnergyValue;
            currentSatiety = rawSatietyValue;

            if (grabInteractable != null)
            {
                grabInteractable.selectEntered.AddListener(OnGrabbed);
                grabInteractable.selectExited.AddListener(OnReleased);
                grabInteractable.activated.AddListener(OnActivated);
            }
        }

        private void OnGrabbed(SelectEnterEventArgs args)
        {
            currentInteractor = args.interactorObject as XRBaseInteractor;
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

            // Augmenter les valeurs nutritionnelles
            currentEnergy = cookedEnergyValue;
            currentSatiety = cookedSatietyValue;

            // Changer le material
            if (objectRenderer != null && cookedMaterial != null)
            {
                objectRenderer.material = cookedMaterial;
            }

            // Petit effet de scale pour montrer la cuisson
            StartCoroutine(CookingAnimation());

            Debug.Log($"🔥 {foodName} est cuit ! Énergie: {currentEnergy}, Satiété: {currentSatiety}");
        }

        private System.Collections.IEnumerator CookingAnimation()
        {
            Vector3 originalScale = transform.localScale;
            Vector3 targetScale = originalScale * 0.9f;

            // Rétrécir
            float elapsed = 0f;
            while (elapsed < 0.2f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / 0.2f;
                transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
                yield return null;
            }

            // Revenir à la taille normale
            elapsed = 0f;
            while (elapsed < 0.2f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / 0.2f;
                transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
                yield return null;
            }

            transform.localScale = originalScale;
        }

        private void PlaceOnTable(Transform spot)
        {
            placementSpot = spot;
            isPlaced = true;

            // Snap à la position
            transform.position = spot.position;
            transform.rotation = spot.rotation;

            // Désactiver le grab une fois placé
            if (grabInteractable != null)
            {
                grabInteractable.enabled = false;
            }

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

            if (grabInteractable != null)
            {
                grabInteractable.enabled = true;
            }
        }

        private void OnDestroy()
        {
            if (grabInteractable != null)
            {
                grabInteractable.selectEntered.RemoveListener(OnGrabbed);
                grabInteractable.selectExited.RemoveListener(OnReleased);
                grabInteractable.activated.RemoveListener(OnActivated);
            }
        }
    }
}
