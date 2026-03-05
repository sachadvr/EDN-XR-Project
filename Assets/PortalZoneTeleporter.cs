using UnityEngine;
using Unity.XR.CoreUtils;

public class PortalZoneTeleporter : MonoBehaviour
{
    [SerializeField]
    private Vector3 destinationPosition;

    [SerializeField]
    private float verticalOffset = 1.2f;

    [SerializeField]
    private float cooldownSeconds = 1f;

    [SerializeField]
    private bool keepCurrentY = false;

    [Header("Audio")]
    [SerializeField]
    private AudioClip proximitySound;

    [SerializeField]
    [Range(0f, 1f)]
    private float proximityVolume = 1f;

    [SerializeField]
    private float soundHeightOffset = 1.2f;

    private static float s_lastTeleportTime = -1000f;

    private void OnTriggerEnter(Collider other)
    {
        if (Time.time - s_lastTeleportTime < cooldownSeconds)
            return;

        var xrOrigin = other.GetComponentInParent<XROrigin>();
        if (xrOrigin == null)
            return;

        if (proximitySound != null)
        {
            var soundPosition = transform.position + Vector3.up * soundHeightOffset;
            AudioSource.PlayClipAtPoint(proximitySound, soundPosition, proximityVolume);
        }

        var target = destinationPosition;
        if (keepCurrentY)
            target.y = xrOrigin.transform.position.y;

        xrOrigin.transform.position = target + Vector3.up * verticalOffset;
        s_lastTeleportTime = Time.time;
    }
}
