using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
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
        public bool isInJar = false;

        public float jarDetectionRadius = 1.2f;

        private GrabbableObject grabbableObject;
        private Renderer objectRenderer;
        private XRGrabInteractable grab;

        private void Awake()
        {
            grabbableObject = GetComponent<GrabbableObject>();
            objectRenderer = GetComponentInChildren<Renderer>();
            grab = GetComponent<XRGrabInteractable>();
            if (grab != null) grab.selectExited.AddListener(OnReleased);
        }

        private void OnDestroy()
        {
            if (grab != null) grab.selectExited.RemoveListener(OnReleased);
        }

        private void OnReleased(SelectExitEventArgs args)
        {
            if (isInJar) return;
            if (!hasSalt) return;

            Jarre jar = FindNearestJar(jarDetectionRadius);
            if (jar != null)
            {
                jar.AddFood(this);
                isInJar = true;
            }
        }

        private Jarre FindNearestJar(float maxDistance)
        {
            Jarre best = null;
            float bestSqr = maxDistance * maxDistance;
            foreach (var j in FindObjectsOfType<Jarre>())
            {
                float d = (j.transform.position - transform.position).sqrMagnitude;
                if (d < bestSqr) { bestSqr = d; best = j; }
            }
            return best;
        }

        private void OnTriggerEnter(Collider other)
        {
            // Vérifier si c'est du sel qui touche l'aliment
            GrabbableObject otherGrabbable = other.GetComponent<GrabbableObject>();
            if (otherGrabbable != null && otherGrabbable.objectType == "sel")
            {
                if (!hasSalt)
                {
                    ApplySalt();
                    // On ne détruit plus le sel pour qu'il puisse être réutilisé sur les autres aliments !
                }
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

    }
}
