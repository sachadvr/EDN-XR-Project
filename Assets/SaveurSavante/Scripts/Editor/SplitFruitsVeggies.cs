using UnityEngine;
using UnityEditor;
using UnityEngine.XR.Interaction.Toolkit;
using SaveurSavante.Chapters.Gandhi;

namespace SaveurSavante.EditorTools
{
    public static class SplitFruitsVeggies
    {
        [MenuItem("SaveurSavante/Split Fruits Veggies To Children")]
        public static void Split()
        {
            int processed = 0;
            int created = 0;

            var manager = Object.FindObjectOfType<XRInteractionManager>(true);

            var allRoots = Object.FindObjectsOfType<GameObject>(true);
            foreach (var root in allRoots)
            {
                if (root.name != "lowpoly_fruits__vegetables") continue;

                var rootNode = FindRootNode(root.transform);
                if (rootNode == null)
                {
                    Debug.LogWarning($"⚠️ RootNode introuvable sous {root.name}");
                    continue;
                }

                // Each direct child of RootNode = one fruit/veg
                foreach (Transform fruit in rootNode)
                {
                    SetupFruit(fruit, manager);
                    created++;
                }

                // Remove parent components
                RemoveIfPresent<GandhiFood>(root);
                RemoveIfPresent<XRGrabInteractable>(root);
                RemoveIfPresent<Rigidbody>(root);
                RemoveIfPresent<MeshCollider>(root);

                processed++;
            }

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

            Debug.Log($"✅ SplitFruitsVeggies: processed {processed} parent(s), set up {created} fruit(s).");
        }

        private static Transform FindRootNode(Transform t)
        {
            foreach (Transform c in t)
            {
                if (c.name == "RootNode") return c;
                var rn = FindRootNode(c);
                if (rn != null) return rn;
            }
            return null;
        }

        private static void SetupFruit(Transform fruit, XRInteractionManager manager)
        {
            var go = fruit.gameObject;
            go.tag = "Food";

            var food = go.GetComponent<GandhiFood>();
            if (food == null) food = go.AddComponent<GandhiFood>();
            if (string.IsNullOrEmpty(food.foodName)) food.foodName = fruit.name;
            if (string.IsNullOrEmpty(food.foodType)) food.foodType = GuessType(fruit.name);

            var rb = go.GetComponent<Rigidbody>();
            if (rb == null) rb = go.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;

            var grab = go.GetComponent<XRGrabInteractable>();
            if (grab == null) grab = go.AddComponent<XRGrabInteractable>();
            if (manager != null)
            {
                var so = new SerializedObject(grab);
                var prop = so.FindProperty("m_InteractionManager");
                if (prop != null) { prop.objectReferenceValue = manager; so.ApplyModifiedPropertiesWithoutUndo(); }
            }
            grab.forceGravityOnDetach = false;

            // Collider on mesh leaf
            var mf = go.GetComponentInChildren<MeshFilter>(true);
            if (mf != null && mf.gameObject.GetComponent<Collider>() == null)
            {
                var mc = mf.gameObject.AddComponent<MeshCollider>();
                mc.convex = true;
            }
        }

        private static string GuessType(string n)
        {
            n = n.ToLower();
            if (n.Contains("apple") || n.Contains("banana") || n.Contains("pear")
                || n.Contains("mandarine") || n.Contains("pomegranate")) return "fruit";
            return "legume";
        }

        private static void RemoveIfPresent<T>(GameObject go) where T : Component
        {
            var c = go.GetComponent<T>();
            if (c != null) Object.DestroyImmediate(c, true);
        }
    }
}
