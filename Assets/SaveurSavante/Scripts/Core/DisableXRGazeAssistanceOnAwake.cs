using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace SaveurSavante.Core
{
    public class DisableXRGazeAssistanceOnAwake : MonoBehaviour
    {
        private void Awake()
        {
            var assists = FindObjectsOfType<XRGazeAssistance>(true);
            foreach (var assist in assists)
            {
                if (assist != null)
                {
                    Destroy(assist);
                }
            }
        }
    }
}
