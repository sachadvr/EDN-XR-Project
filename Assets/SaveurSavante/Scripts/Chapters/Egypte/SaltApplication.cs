using UnityEngine;
using SaveurSavante.Interactions;

namespace SaveurSavante.Chapters.Egypte
{
    public class SaltApplication : MonoBehaviour
    {
        [Header("Références")]
        public GameObject saltPrefab;
        public Material preservedMaterial;
        public Material spoiledMaterial;

        [Header("État")]
        public bool hasSalt = false;
        public bool isPreserved = false;

        private GrabbableObject grabbableObject;
        private Renderer objectRenderer;

        private void Awake()
        {
            grabbableObject = GetComponent<GrabbableObject>();
            objectRenderer = GetComponentInChildren<Renderer>();
        }

        private void OnTriggerEnter(Collider other)
        {
            // Vérifier si c'est du sel qui touche l'aliment
            GrabbableObject otherGrabbable = other.GetComponent<GrabbableObject>();
            if (otherGrabbable != null && otherGrabbable.objectType == "sel")
            {
                ApplySalt();
                Destroy(other.gameObject); // Consommer le sel
            }

            // Vérifier si l'aliment salé touche une jarre
            if (other.CompareTag("Jarre") && hasSalt)
            {
                Jarre jarre = other.GetComponent<Jarre>();
                if (jarre != null)
                {
                    jarre.AddFood(this);
                    Debug.Log("🫙 Aliment déposé dans la jarre !");
                }
            }
        }

        public void ApplySalt()
        {
            hasSalt = true;
            isPreserved = true;

            if (objectRenderer != null && preservedMaterial != null)
            {
                objectRenderer.material = preservedMaterial;
            }

            Debug.Log($"🧂 Sel appliqué sur {gameObject.name} ! L'aliment est maintenant conservé.");
        }

        public void Spoil()
        {
            if (!hasSalt)
            {
                if (objectRenderer != null && spoiledMaterial != null)
                {
                    objectRenderer.material = spoiledMaterial;
                }
                Debug.Log($"⚠️ {gameObject.name} s'est détérioré (pas de sel)");
            }
        }
    }
}
