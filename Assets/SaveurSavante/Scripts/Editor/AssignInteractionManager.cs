using UnityEngine;
using UnityEditor;
using UnityEngine.XR.Interaction.Toolkit;

namespace SaveurSavante.EditorTools
{
    public static class AssignInteractionManager
    {
        [MenuItem("SaveurSavante/Assign XR Interaction Manager To All")]
        public static void Assign()
        {
            var manager = Object.FindObjectOfType<XRInteractionManager>(true);
            if (manager == null)
            {
                Debug.LogError("❌ XRInteractionManager introuvable dans la scène.");
                return;
            }

            int count = 0;
            var grabs = Object.FindObjectsOfType<XRBaseInteractable>(true);
            foreach (var g in grabs)
            {
                var so = new SerializedObject(g);
                var prop = so.FindProperty("m_InteractionManager");
                if (prop != null)
                {
                    prop.objectReferenceValue = manager;
                    so.ApplyModifiedPropertiesWithoutUndo();
                    count++;
                }
            }

            var interactors = Object.FindObjectsOfType<XRBaseInteractor>(true);
            foreach (var i in interactors)
            {
                var so = new SerializedObject(i);
                var prop = so.FindProperty("m_InteractionManager");
                if (prop != null)
                {
                    prop.objectReferenceValue = manager;
                    so.ApplyModifiedPropertiesWithoutUndo();
                    count++;
                }
            }

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

            Debug.Log($"✅ AssignInteractionManager: assigned to {count} interactable(s)/interactor(s).");
        }
    }
}
