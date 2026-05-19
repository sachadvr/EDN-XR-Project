using UnityEngine;
using UnityEditor;
using UnityEngine.XR.Interaction.Toolkit;

namespace SaveurSavante.EditorTools
{
    public static class FixEmptyColliders
    {
        [MenuItem("SaveurSavante/Fix Empty Colliders On Foods")]
        public static void Run()
        {
            int fixedCount = 0, missing = 0;
            foreach (var grab in Object.FindObjectsOfType<XRGrabInteractable>(true))
            {
                var go = grab.gameObject;
                var cols = go.GetComponentsInChildren<Collider>(true);
                bool hasValid = false;
                foreach (var c in cols)
                {
                    if (!c.enabled) continue;
                    if (c is MeshCollider mc && mc.sharedMesh == null) continue;
                    if (c.bounds.size == Vector3.zero) continue;
                    hasValid = true; break;
                }
                if (hasValid) continue;

                missing++;

                // Try assign mesh from own MeshFilter to MeshCollider
                var emptyMc = go.GetComponent<MeshCollider>();
                var ownMf = go.GetComponent<MeshFilter>();
                if (emptyMc != null && ownMf != null && ownMf.sharedMesh != null)
                {
                    emptyMc.sharedMesh = ownMf.sharedMesh;
                    emptyMc.convex = true;
                    EditorUtility.SetDirty(emptyMc);
                    fixedCount++;
                    Debug.Log($"  fixed mesh on {go.name}");
                    continue;
                }

                // Fallback: BoxCollider from renderer bounds
                var rends = go.GetComponentsInChildren<Renderer>(true);
                if (rends.Length == 0) { Debug.LogWarning($"  no renderer on {go.name}"); continue; }

                Bounds b = rends[0].bounds;
                foreach (var r in rends) b.Encapsulate(r.bounds);

                if (emptyMc != null) Object.DestroyImmediate(emptyMc);
                var box = go.GetComponent<BoxCollider>();
                if (box == null) box = Undo.AddComponent<BoxCollider>(go);
                Vector3 localCenter = go.transform.InverseTransformPoint(b.center);
                Vector3 localSize = go.transform.InverseTransformVector(b.size);
                localSize = new Vector3(Mathf.Abs(localSize.x), Mathf.Abs(localSize.y), Mathf.Abs(localSize.z));
                box.center = localCenter;
                box.size = localSize;
                EditorUtility.SetDirty(box);
                fixedCount++;
                Debug.Log($"  added box on {go.name}");
            }

            Debug.Log($"Total empty/missing: {missing}");

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

            Debug.Log($"✅ FixEmptyColliders: fixed {fixedCount} grabbable(s).");
        }
    }
}
