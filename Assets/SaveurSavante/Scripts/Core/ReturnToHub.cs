using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;

namespace SaveurSavante.Core
{
    public class ReturnToHub : MonoBehaviour
    {
        [Header("Configuration")]
        public Transform hubDestination;
        public bool requireChapterComplete = true;

        [Header("Visual")]
        public Material activeMaterial;
        public ParticleSystem returnEffect;

        [Header("Audio")]
        public AudioClip returnSound;

        private XRSimpleInteractable simpleInteractable;
        private bool isActive = false;

        private void Awake()
        {
            simpleInteractable = GetComponent<XRSimpleInteractable>();

            if (simpleInteractable != null)
            {
                simpleInteractable.selectEntered.AddListener(OnReturnActivated);
            }

            // Cacher au début
            gameObject.SetActive(false);
        }

        public void Activate()
        {
            isActive = true;
            gameObject.SetActive(true);

            if (returnEffect != null)
            {
                returnEffect.Play();
            }

            Debug.Log("🏠 Retour au Hub activé !");
        }

        private void OnReturnActivated(SelectEnterEventArgs args)
        {
            if (!isActive)
            {
                Debug.Log("🚫 Le portail de retour n'est pas encore actif");
                return;
            }

            ReturnPlayer();
        }

        private void ReturnPlayer()
        {
            if (hubDestination == null)
            {
                // Utiliser la position du hub depuis le GameManager
                if (GameManager.Instance != null)
                {
                    XROrigin xrOrigin = FindObjectOfType<XROrigin>();
                    if (xrOrigin != null)
                    {
                        xrOrigin.transform.position = GameManager.Instance.hubPosition;
                        Debug.Log("🏠 Retour au Hub !");
                    }
                }
            }
            else
            {
                XROrigin xrOrigin = FindObjectOfType<XROrigin>();
                if (xrOrigin != null)
                {
                    xrOrigin.transform.position = hubDestination.position;
                    xrOrigin.transform.rotation = hubDestination.rotation;
                    Debug.Log("🏠 Retour au Hub à la position définie !");
                }
            }

            if (returnSound != null)
            {
                AudioSource.PlayClipAtPoint(returnSound, transform.position);
            }

            // Désactiver après utilisation
            gameObject.SetActive(false);
            isActive = false;
        }

        private void OnDestroy()
        {
            if (simpleInteractable != null)
            {
                simpleInteractable.selectEntered.RemoveListener(OnReturnActivated);
            }
        }
    }
}
