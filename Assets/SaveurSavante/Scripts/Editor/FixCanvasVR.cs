using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace SaveurSavante.EditorScripts
{
    public class FixCanvasVR : EditorWindow
    {
        [MenuItem("Tools/Saveur Savante/Fix UI Canvas for VR")]
        public static void FixCanvasForVR()
        {
            Canvas[] allCanvases = FindObjectsOfType<Canvas>();
            int fixedCount = 0;

            foreach (Canvas canvas in allCanvases)
            {
                if (canvas.renderMode != RenderMode.WorldSpace)
                {
                    Undo.RecordObject(canvas, "Fix Canvas Render Mode");
                    canvas.renderMode = RenderMode.WorldSpace;
                    
                    // Adjust scale so it doesn't take up the whole world
                    RectTransform rt = canvas.GetComponent<RectTransform>();
                    if (rt != null)
                    {
                        Undo.RecordObject(rt, "Fix Canvas Scale");
                        rt.localScale = new Vector3(0.002f, 0.002f, 0.002f);
                        // Center it roughly in front of the origin or camera
                        if (rt.position == Vector3.zero)
                        {
                            rt.position = new Vector3(0, 1.5f, 2f);
                        }
                    }

                    fixedCount++;
                    Debug.Log($"[UI Fix] Converted {canvas.name} to World Space for VR.");
                }

                // Add XR UI Raycaster for interaction if needed
                GraphicRaycaster gr = canvas.GetComponent<GraphicRaycaster>();
                if (gr != null && canvas.GetComponent<TrackedDeviceGraphicRaycaster>() == null)
                {
                    Undo.AddComponent<TrackedDeviceGraphicRaycaster>(canvas.gameObject);
                    Undo.DestroyObjectImmediate(gr); // GraphicRaycaster isn't usually needed when you have TrackedDeviceGraphicRaycaster, or you can keep it. TrackedDevice inherits from it in 2.x? No, TrackedDeviceGraphicRaycaster IS the UI raycaster.
                    Debug.Log($"[UI Fix] Added TrackedDeviceGraphicRaycaster to {canvas.name} for VR pointer support.");
                }
            }

            Debug.Log($"[UI Fix] Finished! Fixed {fixedCount} Canvases.");
            // No DisplayDialog to avoid freezing MCP!
        }
    }
}
