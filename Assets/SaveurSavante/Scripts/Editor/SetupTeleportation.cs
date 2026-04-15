using UnityEngine;
using UnityEditor;
using UnityEngine.XR.Interaction.Toolkit;

namespace SaveurSavante.EditorScripts
{
    public class SetupTeleportation : EditorWindow
    {
        [MenuItem("Tools/Saveur Savante/Setup Teleportation Everywhere")]
        public static void AutoSetupTeleportation()
        {
            // Find Interaction Manager
            XRInteractionManager interactionManager = FindObjectOfType<XRInteractionManager>();
            if (interactionManager == null)
            {
                Debug.LogError("SetupTeleportation: No XRInteractionManager found in the scene! Please ensure XR is set up.");
                return;
            }

            // Find all MeshColliders and BoxColliders that could be floors
            Collider[] colliders = FindObjectsOfType<Collider>();
            int addedCount = 0;

            foreach (Collider col in colliders)
            {
                if (col.isTrigger) continue;
                
                // Exclude objects with GrabbableObject or grab interactables
                if (col.GetComponent<XRGrabInteractable>() != null) continue;
                if (col.GetComponent<Rigidbody>() != null) continue;

                string nameLower = col.gameObject.name.ToLower();
                
                // Logic to determine if it's a floor. We check common names for floors, grounds, and planes.
                bool isLikelyFloor = nameLower.Contains("floor") || 
                                     nameLower.Contains("ground") || 
                                     nameLower.Contains("sol") || 
                                     nameLower.Contains("terrain") ||
                                     nameLower.Contains("plane");

                // We can also check if the normal is pointing up if it's a MeshCollider, but doing it by name is usually enough in simple VR projects.
                if (isLikelyFloor)
                {
                    if (col.GetComponent<TeleportationArea>() == null && col.GetComponent<TeleportationAnchor>() == null)
                    {
                        Undo.AddComponent<TeleportationArea>(col.gameObject);
                        TeleportationArea ta = col.GetComponent<TeleportationArea>();
                        ta.interactionManager = interactionManager;
                        
                        // By default, teleportation requires a custom reticle or uses default
                        addedCount++;
                        Debug.Log($"[Teleport Setup] Added TeleportationArea to: {col.gameObject.name}", col.gameObject);
                    }
                }
            }

            Debug.Log($"[Teleport Setup] Finished! Added {addedCount} TeleportationAreas.");
            EditorUtility.DisplayDialog("Setup Teleportation", $"Successfully added {addedCount} TeleportationAreas to floors/grounds in the scene.", "OK");
        }
    }
}
