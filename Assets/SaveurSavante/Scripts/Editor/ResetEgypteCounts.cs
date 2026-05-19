using UnityEngine;
using UnityEditor;
using SaveurSavante.Chapters.Egypte;

namespace SaveurSavante.EditorTools
{
    public static class ResetEgypteCounts
    {
        [MenuItem("SaveurSavante/Reset Egypte Counts")]
        public static void Run()
        {
            int n = 0;
            foreach (var j in Object.FindObjectsOfType<Jarre>(true))
            {
                Undo.RecordObject(j, "Reset Jarre Count");
                j.requiredFoodCount = 1;
                EditorUtility.SetDirty(j);
                n++;
            }
            foreach (var z in Object.FindObjectsOfType<OfferingZone>(true))
            {
                Undo.RecordObject(z, "Reset Zone Count");
                z.requiredFoodCount = 1;
                EditorUtility.SetDirty(z);
                n++;
            }
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            Debug.Log($"✅ Reset {n} Egypte count(s) to 1.");
        }
    }
}
