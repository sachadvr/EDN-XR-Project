using UnityEngine;
using UnityEditor;
using SaveurSavante.Chapters.Gandhi;

namespace SaveurSavante.EditorTools
{
    public static class ResetGandhiRiddles
    {
        [MenuItem("SaveurSavante/Reset Gandhi Riddles")]
        public static void Run()
        {
            int n = 0;
            foreach (var th in Object.FindObjectsOfType<TreasureHunt>(true))
            {
                Undo.RecordObject(th, "Reset Riddles");
                th.riddles = new[]
                {
                    "🍎 Gandhi a besoin d'un fruit.",
                    "🥕 Gandhi a besoin d'un légume.",
                    "🌾 Gandhi a besoin d'une graine."
                };
                th.expectedFoodTypes = new[] { "fruit", "legume", "graine" };
                EditorUtility.SetDirty(th);
                n++;
            }
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            Debug.Log($"✅ Reset {n} TreasureHunt riddles.");
        }
    }
}
