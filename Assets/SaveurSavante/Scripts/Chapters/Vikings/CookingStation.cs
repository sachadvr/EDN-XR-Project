using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SaveurSavante.Chapters.Vikings
{
    [RequireComponent(typeof(BoxCollider))]
    public class CookingStation : MonoBehaviour
    {
        [Header("Configuration")]
        public float cookingTime = 3f;
        public Vector3 cookingZoneSize = new Vector3(1.2f, 1.2f, 1.2f);
        public string chapterName = "Vikings";

        [Header("Effets")]
        public ParticleSystem fireParticles;
        public ParticleSystem smokeParticles;
        public Light fireLight;
        public AudioClip cookingSound;
        public AudioClip cookedSound;

        [Header("Visual")]
        public GameObject cookedVisualPrefab;

        private AudioSource audioSource;
        private BoxCollider cookingZone;
        private readonly HashSet<VikingFood> foodsBeingCooked = new HashSet<VikingFood>();
        private bool isActive = true;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            cookingZone = GetComponent<BoxCollider>();
            cookingZone.isTrigger = true;
            cookingZone.size = cookingZoneSize;

            // Ensure no collider on the cooking station blocks the player or food grabbing
            foreach (var c in GetComponentsInChildren<Collider>(true))
            {
                c.isTrigger = true;
            }
            // Strip any rigidbody to avoid physics interactions
            foreach (var rbToRemove in GetComponentsInChildren<Rigidbody>(true))
            {
                Destroy(rbToRemove);
            }

            // Put on Ignore Raycast layer so the XR ray-cast does not stop on this trigger
            int ignoreLayer = LayerMask.NameToLayer("Ignore Raycast");
            if (ignoreLayer >= 0)
            {
                gameObject.layer = ignoreLayer;
                foreach (var t in GetComponentsInChildren<Transform>(true)) t.gameObject.layer = ignoreLayer;
            }
        }

        private void Start()
        {
            // Activer les effets de feu
            if (fireParticles != null)
            {
                fireParticles.Play();
            }
            if (fireLight != null)
            {
                fireLight.enabled = true;
            }
        }

        private void OnTriggerEnter(Collider other) { TryCook(other); }
        private void OnTriggerStay(Collider other) { TryCook(other); }

        private void TryCook(Collider other)
        {
            if (!isActive) return;
            VikingFood food = other.GetComponentInParent<VikingFood>();
            if (food == null) return;
            if (food.isCooked || !food.canBeCooked) return;
            if (foodsBeingCooked.Contains(food)) return;
            // Ne pas cuire pendant que tenu par joueur
            var grab = food.GetComponent<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>();
            if (grab != null && grab.isSelected) return;
            StartCoroutine(CookFood(food));
        }

        private IEnumerator CookFood(VikingFood food)
        {
            foodsBeingCooked.Add(food);

            string displayName = string.IsNullOrWhiteSpace(food.foodName) ? food.gameObject.name : food.foodName;
            Debug.Log($"🔥 Cuisson de {displayName} commencée...");

            // Son de cuisson
            if (cookingSound != null && audioSource != null)
            {
                audioSource.clip = cookingSound;
                audioSource.loop = true;
                audioSource.Play();
            }

            // Fumée
            if (smokeParticles != null)
            {
                smokeParticles.Play();
            }

            // Attendre le temps de cuisson
            yield return new WaitForSeconds(cookingTime);

            // Cuisiner l'aliment
            food.Cook();

            yield return StartCoroutine(BumpStation());

            // Son de fin de cuisson
            if (cookedSound != null)
            {
                AudioSource.PlayClipAtPoint(cookedSound, transform.position);
            }

            // Arrêter le son de cuisson
            if (audioSource != null)
            {
                audioSource.Stop();
            }

            // Arrêter la fumée
            if (smokeParticles != null)
            {
                smokeParticles.Stop();
            }

            Debug.Log($"✅ {displayName} est parfaitement cuit !");
            foodsBeingCooked.Remove(food);
        }

        private IEnumerator BumpStation()
        {
            Vector3 startPos = transform.localPosition;
            Vector3 downPos = startPos + Vector3.down * 0.12f;

            float elapsed = 0f;
            while (elapsed < 0.1f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / 0.1f;
                transform.localPosition = Vector3.Lerp(startPos, downPos, t);
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < 0.1f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / 0.1f;
                transform.localPosition = Vector3.Lerp(downPos, startPos, t);
                yield return null;
            }

            transform.localPosition = startPos;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (cookingZone == null)
            {
                cookingZone = GetComponent<BoxCollider>();
            }

            if (cookingZone != null)
            {
                cookingZone.isTrigger = true;
                cookingZone.size = cookingZoneSize;
            }
        }

        private void OnDrawGizmosSelected()
        {
            var zone = GetComponent<BoxCollider>();
            if (zone == null)
                return;

            Gizmos.color = new Color(1f, 0.55f, 0.15f, 0.25f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(zone.center, zone.size);
            Gizmos.color = new Color(1f, 0.55f, 0.15f, 1f);
            Gizmos.DrawWireCube(zone.center, zone.size);
        }
#endif
    }
}
