using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using SaveurSavante.Chapters.Egypte;

namespace SaveurSavante.Interactions
{
    [RequireComponent(typeof(XRGrabInteractable))]
    public class SaltShaker : MonoBehaviour
    {
        public ParticleSystem saltParticles;
        public Transform emissionPoint;
        public float passiveRate = 5f;
        public float activeRate = 60f;
        public float saltRadius = 0.25f;

        XRGrabInteractable grab;
        bool isHeld;
        bool isShaking;

        void Awake()
        {
            grab = GetComponent<XRGrabInteractable>();
            grab.selectEntered.AddListener(OnGrab);
            grab.selectExited.AddListener(OnRelease);
            grab.activated.AddListener(OnActivate);
            grab.deactivated.AddListener(OnDeactivate);
            UpdateEmission();
        }

        void OnDestroy()
        {
            if (grab == null) return;
            grab.selectEntered.RemoveListener(OnGrab);
            grab.selectExited.RemoveListener(OnRelease);
            grab.activated.RemoveListener(OnActivate);
            grab.deactivated.RemoveListener(OnDeactivate);
        }

        void OnGrab(SelectEnterEventArgs a) { isHeld = true; UpdateEmission(); }
        void OnRelease(SelectExitEventArgs a) { isHeld = false; isShaking = false; UpdateEmission(); }
        void OnActivate(ActivateEventArgs a) { isShaking = true; UpdateEmission(); }
        void OnDeactivate(DeactivateEventArgs a) { isShaking = false; UpdateEmission(); }

        void Update()
        {
            if (!isHeld) return;
            Vector3 origin = emissionPoint != null ? emissionPoint.position : transform.position;
            Collider[] hits = Physics.OverlapSphere(origin, saltRadius);
            foreach (var c in hits)
            {
                var sa = c.GetComponentInParent<SaltApplication>();
                if (sa != null && !sa.hasSalt) sa.ApplySalt();
            }
        }

        void UpdateEmission()
        {
            if (saltParticles == null) return;
            var emission = saltParticles.emission;
            if (!isHeld) { emission.rateOverTime = 0f; if (saltParticles.isPlaying) saltParticles.Stop(); return; }
            emission.rateOverTime = isShaking ? activeRate : passiveRate;
            if (!saltParticles.isPlaying) saltParticles.Play();
        }
    }
}
