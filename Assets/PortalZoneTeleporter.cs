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

    private static float s_lastTeleportTime = -1000f;

    private void OnTriggerEnter(Collider other)
    {
        if (Time.time - s_lastTeleportTime < cooldownSeconds)
            return;

        var xrOrigin = other.GetComponentInParent<XROrigin>();
        if (xrOrigin == null)
            return;

        var target = destinationPosition;
        if (keepCurrentY)
            target.y = xrOrigin.transform.position.y;

        xrOrigin.transform.position = target + Vector3.up * verticalOffset;
        s_lastTeleportTime = Time.time;
    }
}
