using UnityEngine;
using UnityEditor;
using SaveurSavante.Chapters.Titanic;

namespace SaveurSavante.EditorTools
{
    public static class CleanFoodPack
    {
        [MenuItem("SaveurSavante/Clean Titanic FoodPack Components")]
        public static void Clean()
        {
            int removed = 0;
            var allFoods = Object.FindObjectsOfType<TitanicFood>(true);
            foreach (var food in allFoods)
            {
                bool keep = food.GetComponent<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>() != null
                            && food.GetComponent<Rigidbody>() != null;

                if (!keep)
                {
                    var box = food.GetComponent<BoxCollider>();
                    if (box != null)
                    {
                        Object.DestroyImmediate(box, true);
                        removed++;
                    }
                    Object.DestroyImmediate(food, true);
                    removed++;
                }
            }

            if (UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().IsValid())
            {
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                    UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            }

            // Aussi nettoyer les ancêtres FBX/RootNode/asset_pack qui ont Rigidbody/XRGrabInteractable sans TitanicFood
            var allGrabs = Object.FindObjectsOfType<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>(true);
            foreach (var grab in allGrabs)
            {
                if (grab.GetComponent<TitanicFood>() != null) continue;

                string n = grab.gameObject.name;
                bool isContainer = n.Contains("free_low_poly_food_asset_pack")
                                   || n.Contains(".fbx")
                                   || n == "RootNode"
                                   || n == "FoodPack";
                if (!isContainer) continue;

                var rb = grab.GetComponent<Rigidbody>();
                if (rb != null) { Object.DestroyImmediate(rb, true); removed++; }
                Object.DestroyImmediate(grab, true);
                removed++;
            }

            Debug.Log($"✅ CleanFoodPack: removed {removed} component(s) from non-food nodes.");
        }
    }
}
