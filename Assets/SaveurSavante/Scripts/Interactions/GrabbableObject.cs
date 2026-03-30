using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace SaveurSavante.Interactions
{
    public class GrabbableObject : MonoBehaviour
    {
        [Header("Propriétés")]
        public string objectName;
        public string objectType; // "sel", "aliment", "jarre", "bol", "assiette", etc.

        [Header("État")]
        public bool isGrabbed = false;
        public bool isUsed = false;

        [Header("Visual Feedback")]
        public Material highlightMaterial;
        private Material originalMaterial;
        private Renderer objectRenderer;

        private XRGrabInteractable grabInteractable;

        private void Awake()
        {
            grabInteractable = GetComponent<XRGrabInteractable>();
            objectRenderer = GetComponentInChildren<Renderer>();

            if (objectRenderer != null)
            {
                originalMaterial = objectRenderer.material;
            }

            if (grabInteractable != null)
            {
                grabInteractable.selectEntered.AddListener(OnGrab);
                grabInteractable.selectExited.AddListener(OnRelease);
                grabInteractable.hoverEntered.AddListener(OnHoverEnter);
                grabInteractable.hoverExited.AddListener(OnHoverExit);
            }
        }

        private void OnGrab(SelectEnterEventArgs args)
        {
            isGrabbed = true;
            Debug.Log($"✋ Object grabé : {objectName}");
        }

        private void OnRelease(SelectExitEventArgs args)
        {
            isGrabbed = false;
            Debug.Log($"👋 Object relâché : {objectName}");
        }

        private void OnHoverEnter(HoverEnterEventArgs args)
        {
            if (objectRenderer != null && highlightMaterial != null)
            {
                objectRenderer.material = highlightMaterial;
            }
        }

        private void OnHoverExit(HoverExitEventArgs args)
        {
            if (objectRenderer != null && originalMaterial != null)
            {
                objectRenderer.material = originalMaterial;
            }
        }

        private void OnDestroy()
        {
            if (grabInteractable != null)
            {
                grabInteractable.selectEntered.RemoveListener(OnGrab);
                grabInteractable.selectExited.RemoveListener(OnRelease);
                grabInteractable.hoverEntered.RemoveListener(OnHoverEnter);
                grabInteractable.hoverExited.RemoveListener(OnHoverExit);
            }
        }
    }
}
