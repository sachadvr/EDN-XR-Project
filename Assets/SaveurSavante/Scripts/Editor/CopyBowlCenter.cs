using UnityEngine;
using UnityEditor;
using SaveurSavante.Chapters.Gandhi;

namespace SaveurSavante.EditorTools
{
    public static class CopyBowlCenter
    {
        [MenuItem("SaveurSavante/Copy Bowl Center To All Foods")]
        public static void Copy()
        {
            // Trouver apple03_lr
            GandhiFood source = null;
            var allFoods = Object.FindObjectsOfType<GandhiFood>(true);
            foreach (var f in allFoods)
            {
                if (f.gameObject.name == "apple03_lr")
                {
                    source = f;
                    break;
                }
            }

            if (source == null)
            {
                Debug.LogError("❌ apple03_lr introuvable.");
                return;
            }

            if (source.bowlCenter == null)
            {
                Debug.LogError("❌ apple03_lr n'a pas de bowlCenter assigné.");
                return;
            }

            int updated = 0;
            foreach (var f in allFoods)
            {
                if (f == source) continue;
                if (f.bowlCenter == source.bowlCenter) continue;

                Undo.RecordObject(f, "Copy bowlCenter");
                f.bowlCenter = source.bowlCenter;
                if (source.spawnPosition != null && f.spawnPosition == null)
                {
                    f.spawnPosition = source.spawnPosition;
                }
                EditorUtility.SetDirty(f);
                updated++;
            }

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

            Debug.Log($"✅ CopyBowlCenter: bowlCenter copié sur {updated} GandhiFood(s).");
        }
    }
}
