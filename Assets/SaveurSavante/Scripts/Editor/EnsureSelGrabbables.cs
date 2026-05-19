using UnityEngine;
using UnityEditor;
using SaveurSavante.Interactions;

namespace SaveurSavante.EditorTools
{
    public static class EnsureSelGrabbables
    {
        static readonly string[] SelNames = { "Grain", "Grain2", "salt_rock_lamp_game_ready__2k_pbr", "salt_rock_lamp" };

        [MenuItem("SaveurSavante/Ensure Sel Grabbables")]
        public static void Run()
        {
            int added = 0, ok = 0;
            var allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
            foreach (var name in SelNames)
            {
                GameObject go = null;
                foreach (var t in allTransforms)
                {
                    if (t.gameObject.name == name && t.gameObject.scene.IsValid())
                    { go = t.gameObject; break; }
                }
                if (go == null) { Debug.LogWarning($"⚠️ {name} introuvable"); continue; }
                var grab = go.GetComponent<GrabbableObject>();
                if (grab == null)
                {
                    grab = Undo.AddComponent<GrabbableObject>(go);
                    added++;
                }
                else ok++;
                grab.objectName = go.name;
                grab.objectType = "sel";
                EditorUtility.SetDirty(grab);
            }

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

            Debug.Log($"✅ EnsureSelGrabbables: added {added}, already ok {ok}.");
        }
    }
}
