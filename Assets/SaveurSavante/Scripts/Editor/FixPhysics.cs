using UnityEngine;
using UnityEditor;

namespace SaveurSavante.EditorScripts
{
    public class FixPhysics : EditorWindow
    {
        [MenuItem("Tools/Saveur Savante/Fix Missing Food & Physics")]
        public static void AutoFixPhysics()
        {
            MeshCollider[] allMeshColliders = FindObjectsOfType<MeshCollider>();
            int fixedCount = 0;

            foreach (MeshCollider mc in allMeshColliders)
            {
                Rigidbody rb = mc.GetComponent<Rigidbody>();
                // In Unity, a dynamic Rigidbody (isKinematic = false) MUST have Convex = true on a MeshCollider
                if (rb != null && !rb.isKinematic && !mc.convex)
                {
                    // Record undo so the user can ctrl+Z
                    Undo.RecordObject(mc, "Fix MeshCollider Convex");
                    mc.convex = true;
                    fixedCount++;
                    Debug.Log($"[Physics Fix] Fixed non-convex MeshCollider on dynamic object: {mc.gameObject.name}", mc.gameObject);
                }
            }

            Debug.Log($"[Physics Fix] Finished! Fixed {fixedCount} broken colliders.");
            EditorUtility.DisplayDialog("Fix Physics", $"Succès ! {fixedCount} modèles qui passaient à travers le sol ont été corrigés.\n(Leur MeshCollider est maintenant Convex).", "OK");
        }
    }
}
