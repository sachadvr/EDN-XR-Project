using UnityEngine;
using UnityEditor;
using SaveurSavante.Interactions;

namespace SaveurSavante.EditorTools
{
    public static class CleanGrabbableObjects
    {
        [MenuItem("SaveurSavante/Clean GrabbableObjects Keep Sel")]
        public static void Run()
        {
            int removed = 0, kept = 0;
            foreach (var go in Object.FindObjectsOfType<GrabbableObject>(true))
            {
                bool isSel = go.objectType == "sel"
                    || go.gameObject.name.ToLowerInvariant().Contains("salt")
                    || go.gameObject.name.ToLowerInvariant().Contains("grain")
                    || go.gameObject.name.ToLowerInvariant().Contains("sel");

                if (isSel)
                {
                    if (go.objectType != "sel") { go.objectType = "sel"; EditorUtility.SetDirty(go); }
                    kept++;
                }
                else
                {
                    Undo.DestroyObjectImmediate(go);
                    removed++;
                }
            }

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

            Debug.Log($"✅ CleanGrabbableObjects: removed {removed}, kept {kept} (sel).");
        }
    }
}
