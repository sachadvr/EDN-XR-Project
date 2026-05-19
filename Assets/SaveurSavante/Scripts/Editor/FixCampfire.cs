using UnityEngine;
using UnityEditor;
using SaveurSavante.Chapters.Vikings;

namespace SaveurSavante.EditorTools
{
    public static class FixCampfire
    {
        [MenuItem("SaveurSavante/Fix Campfire")]
        public static void Run()
        {
            int n = 0;
            // Compensate for transform scale so world size stays consistent
            Vector3 worldSize = new Vector3(0.6f, 0.6f, 0.6f);

            foreach (var cs in Object.FindObjectsOfType<CookingStation>(true))
            {
                Undo.RecordObject(cs, "Fix Campfire");

                var ls = cs.transform.lossyScale;
                Vector3 localSize = new Vector3(
                    Mathf.Abs(ls.x) > 0.0001f ? worldSize.x / Mathf.Abs(ls.x) : worldSize.x,
                    Mathf.Abs(ls.y) > 0.0001f ? worldSize.y / Mathf.Abs(ls.y) : worldSize.y,
                    Mathf.Abs(ls.z) > 0.0001f ? worldSize.z / Mathf.Abs(ls.z) : worldSize.z);

                cs.cookingZoneSize = localSize;

                var box = cs.GetComponent<BoxCollider>();
                if (box != null)
                {
                    Undo.RecordObject(box, "Fix Campfire Box");
                    box.isTrigger = true;
                    box.size = localSize;
                    box.center = new Vector3(0f, localSize.y * 0.5f, 0f);
                    EditorUtility.SetDirty(box);
                }

                // Strip any non-trigger blocking colliders + rigidbodies on station and children
                foreach (var col in cs.GetComponentsInChildren<Collider>(true))
                {
                    if (col == box) continue;
                    Object.DestroyImmediate(col);
                }
                foreach (var rb in cs.GetComponentsInChildren<Rigidbody>(true))
                {
                    Object.DestroyImmediate(rb);
                }

                EditorUtility.SetDirty(cs);
                n++;
            }

            // Also strip blocking colliders on any object whose name suggests a campfire
            string[] keywords = { "campfire", "feu", "fire", "bonfire", "foyer" };
            foreach (var t in Object.FindObjectsOfType<Transform>(true))
            {
                string nm = t.name.ToLowerInvariant();
                bool isCamp = false;
                foreach (var k in keywords) { if (nm.Contains(k)) { isCamp = true; break; } }
                if (!isCamp) continue;
                if (t.GetComponent<CookingStation>() != null) continue;

                foreach (var col in t.GetComponentsInChildren<Collider>(true))
                {
                    if (col.GetComponentInParent<CookingStation>() != null && col.isTrigger) continue;
                    if (!col.isTrigger) Object.DestroyImmediate(col);
                }
                foreach (var rb in t.GetComponentsInChildren<Rigidbody>(true))
                {
                    Object.DestroyImmediate(rb);
                }
            }

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            Debug.Log($"✅ Fix Campfire: {n} station(s).");
        }
    }
}
