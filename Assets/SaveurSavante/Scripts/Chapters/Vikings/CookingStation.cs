using UnityEngine;
using System.Collections;

namespace SaveurSavante.Chapters.Vikings
{
    public class CookingStation : MonoBehaviour
    {
        [Header("Configuration")]
        public float cookingTime = 3f;
        public float cookingRadius = 1f;
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
        private bool isActive = true;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
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

        private void OnTriggerEnter(Collider other)
        {
            if (!isActive) return;

            VikingFood food = other.GetComponent<VikingFood>();
            if (food != null && !food.isCooked && food.canBeCooked)
            {
                StartCoroutine(CookFood(food));
            }
        }

        private IEnumerator CookFood(VikingFood food)
        {
            Debug.Log($"🔥 Cuisson de {food.foodName} commencée...");

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

            Debug.Log($"✅ {food.foodName} est parfaitement cuit !");
        }

        public void ExtinguishFire()
        {
            isActive = false;

            if (fireParticles != null)
            {
                fireParticles.Stop();
            }
            if (fireLight != null)
            {
                fireLight.enabled = false;
            }
            if (audioSource != null)
            {
                audioSource.Stop();
            }

            Debug.Log("🔥 Le feu de camp s'est éteint.");
        }

        public void RelightFire()
        {
            isActive = true;

            if (fireParticles != null)
            {
                fireParticles.Play();
            }
            if (fireLight != null)
            {
                fireLight.enabled = true;
            }

            Debug.Log("🔥 Le feu de camp est rallumé !");
        }
    }
}
