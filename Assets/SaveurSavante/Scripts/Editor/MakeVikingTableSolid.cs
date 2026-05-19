using UnityEngine;
using UnityEditor;

namespace SaveurSavante.EditorTools
{
    public static class MakeVikingTableSolid
    {
        [MenuItem("SaveurSavante/Make Viking Table Solid")]
        public static void Run()
        {
            var table = GameObject.Find("wood_table");
            if (table == null)
            {
                Debug.LogError("❌ wood_table introuvable.");
                return;
            }

            int added = 0, fixedExisting = 0;
            foreach (var mf in table.GetComponentsInChildren<MeshFilter>(true))
            {
                bool skip = mf.sharedMesh == null
                    || mf.GetComponent<TMPro.TextMeshPro>() != null
                    || mf.name.Contains("Bar") || mf.name == "BG" || mf.name == "Fill";
                if (skip)
                {
                    var stale = mf.GetComponent<MeshCollider>();
                    if (stale != null) Object.DestroyImmediate(stale);
                    continue;
                }

                var mc = mf.GetComponent<MeshCollider>();
                if (mc == null)
                {
                    mc = Undo.AddComponent<MeshCollider>(mf.gameObject);
                    added++;
                }
                else fixedExisting++;

                mc.sharedMesh = mf.sharedMesh;
                mc.convex = false;
                mc.isTrigger = false;

                var rb = mf.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                    rb.useGravity = false;
                    EditorUtility.SetDirty(rb);
                }

                EditorUtility.SetDirty(mc);
            }

            table.isStatic = true;
            foreach (var t in table.GetComponentsInChildren<Transform>(true))
                t.gameObject.isStatic = true;

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

            Debug.Log($"✅ MakeVikingTableSolid: added {added} MeshCollider(s), updated {fixedExisting} existing.");
        }
    }
}
