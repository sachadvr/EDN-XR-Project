using UnityEngine;
using TMPro;

namespace SaveurSavante.UI
{
    public class HolographicTextAnimator : MonoBehaviour
    {
        [Header("Animation")]
        [SerializeField] private float floatAmplitude = 0.1f;
        [SerializeField] private float floatSpeed = 1f;
        [SerializeField] private float rotationAmplitude = 2f;
        [SerializeField] private float rotationSpeed = 0.5f;

        [Header("Pulse Effect")]
        [SerializeField] private bool enablePulse = true;
        [SerializeField] private float pulseSpeed = 2f;
        [SerializeField] private float pulseMinAlpha = 0.6f;
        [SerializeField] private float pulseMaxAlpha = 1f;

        [Header("Glitch Effect")]
        [SerializeField] private bool enableGlitch = false;
        [SerializeField] private float glitchChance = 0.05f;
        [SerializeField] private float glitchDuration = 0.1f;

        private TextMeshPro textMesh;
        private Vector3 initialPosition;
        private Quaternion initialRotation;
        private Color initialColor;
        private float glitchTimer = 0f;
        private bool isGlitching = false;

        private void Awake()
        {
            textMesh = GetComponent<TextMeshPro>();
            initialPosition = transform.localPosition;
            initialRotation = transform.localRotation;
            
            if (textMesh != null)
            {
                initialColor = textMesh.color;
                
                // Configuration pour effet holographique
                textMesh.fontSharedMaterial.SetFloat("_OutlineWidth", 0.1f);
                textMesh.outlineColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0.5f);
                textMesh.fontSharedMaterial.EnableKeyword("UNDERLAY_ON");
            }
        }

        private void Update()
        {
            if (textMesh == null) return;

            // Animation flottante
            float floatY = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
            transform.localPosition = initialPosition + Vector3.up * floatY;

            // Légère rotation
            float rotY = Mathf.Sin(Time.time * rotationSpeed) * rotationAmplitude;
            transform.localRotation = initialRotation * Quaternion.Euler(0, rotY, 0);

            // Effet pulse (clignotement d'opacité)
            if (enablePulse)
            {
                float pulse = Mathf.Lerp(pulseMinAlpha, pulseMaxAlpha, 
                    (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
                
                Color newColor = textMesh.color;
                newColor.a = pulse;
                textMesh.color = newColor;
            }

            // Effet glitch (rare)
            if (enableGlitch)
            {
                HandleGlitchEffect();
            }
        }

        private void HandleGlitchEffect()
        {
            if (isGlitching)
            {
                glitchTimer -= Time.deltaTime;
                if (glitchTimer <= 0)
                {
                    isGlitching = false;
                    textMesh.color = initialColor;
                    transform.localPosition = initialPosition;
                }
            }
            else if (Random.value < glitchChance * Time.deltaTime)
            {
                isGlitching = true;
                glitchTimer = glitchDuration;
                
                // Glitch visuel
                textMesh.color = Color.white;
                transform.localPosition = initialPosition + new Vector3(
                    Random.Range(-0.1f, 0.1f), 
                    Random.Range(-0.05f, 0.05f), 
                    0);
            }
        }

        // Appelé quand le joueur regarde le texte
        public void OnPlayerLookAt()
        {
            enablePulse = false;
            textMesh.color = initialColor;
            
            // Agrandissement temporaire
            transform.localScale = Vector3.one * 1.1f;
        }

        public void OnPlayerLookAway()
        {
            enablePulse = true;
            transform.localScale = Vector3.one;
        }
    }
}
