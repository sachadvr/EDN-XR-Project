using UnityEngine;
using UnityEditor;
using SaveurSavante.Chapters.Vikings;
using SaveurSavante.Chapters.Titanic;
using SaveurSavante.Chapters.Gandhi;
using SaveurSavante.Interactions;

namespace SaveurSavante.EditorTools
{
    public static class FillFoodProperties
    {
        [MenuItem("SaveurSavante/Fill Food Properties")]
        public static void Run()
        {
            int updated = 0;

            // TitanicFood
            foreach (var tf in Object.FindObjectsOfType<TitanicFood>(true))
            {
                bool dirty = false;
                string nm = tf.gameObject.name;

                if (string.IsNullOrEmpty(tf.foodName)) { tf.foodName = nm; dirty = true; }

                string newFlav = ClassifyTitanicFlavor(nm);
                if (string.IsNullOrEmpty(tf.flavorProfile)) { tf.flavorProfile = newFlav; dirty = true; }

                string newCat = ClassifyTitanicCategory(nm);
                if (string.IsNullOrEmpty(tf.foodCategory)) { tf.foodCategory = newCat; dirty = true; }

                if (tf.presentationValue <= 0f) { tf.presentationValue = 20f; dirty = true; }

                if (dirty) { Undo.RecordObject(tf, "Fill TitanicFood"); EditorUtility.SetDirty(tf); updated++; }
            }

            // VikingFood
            foreach (var vf in Object.FindObjectsOfType<VikingFood>(true))
            {
                bool dirty = false;
                string nm = vf.gameObject.name;

                if (string.IsNullOrEmpty(vf.foodName)) { vf.foodName = nm; dirty = true; }
                string vt = ClassifyVikingType(nm);
                if (string.IsNullOrEmpty(vf.foodType)) { vf.foodType = vt; dirty = true; }

                if (dirty) { Undo.RecordObject(vf, "Fill VikingFood"); EditorUtility.SetDirty(vf); updated++; }
            }

            // GandhiFood
            foreach (var gf in Object.FindObjectsOfType<GandhiFood>(true))
            {
                bool dirty = false;
                if (string.IsNullOrEmpty(gf.foodName)) { gf.foodName = gf.gameObject.name; dirty = true; }
                if (dirty) { Undo.RecordObject(gf, "Fill GandhiFood"); EditorUtility.SetDirty(gf); updated++; }
            }

            // GrabbableObject
            var hl = EnsureHighlightMat();
            foreach (var go in Object.FindObjectsOfType<GrabbableObject>(true))
            {
                bool dirty = false;
                string nm = go.gameObject.name;
                if (string.IsNullOrEmpty(go.objectName)) { go.objectName = nm; dirty = true; }
                if (string.IsNullOrEmpty(go.objectType)) { go.objectType = ClassifyObjectType(nm); dirty = true; }
                if (go.highlightMaterial == null && hl != null) { go.highlightMaterial = hl; dirty = true; }
                if (dirty) { Undo.RecordObject(go, "Fill GrabbableObject"); EditorUtility.SetDirty(go); updated++; }
            }

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

            Debug.Log($"✅ FillFoodProperties: updated {updated} component(s).");
        }

        static string ClassifyTitanicFlavor(string raw)
        {
            string n = raw.ToLowerInvariant();
            string[] sucre = { "pancake", "cookie", "coockie", "muffin", "pie", "chocolate", "icecream", "cake", "waffle", "coctail", "cocktail" };
            string[] amer  = { "coffee", "espresso" };
            string[] acide = { "lemon", "vinegar" };
            foreach (var k in sucre) if (n.Contains(k)) return "sucré";
            foreach (var k in amer)  if (n.Contains(k)) return "amer";
            foreach (var k in acide) if (n.Contains(k)) return "acide";
            return "salé";
        }

        static string ClassifyTitanicCategory(string raw)
        {
            string n = raw.ToLowerInvariant();
            string[] dessert = { "pancake", "cookie", "coockie", "muffin", "pie", "chocolate", "icecream", "cake", "waffle" };
            string[] entree  = { "cheese", "soap", "soup" };
            string[] garn    = { "pottatos", "potato", "popcorn", "noodle" };
            foreach (var k in dessert) if (n.Contains(k)) return "dessert";
            foreach (var k in entree)  if (n.Contains(k)) return "entrée";
            foreach (var k in garn)    if (n.Contains(k)) return "garniture";
            return "plat";
        }

        static Material EnsureHighlightMat()
        {
            const string dir = "Assets/SaveurSavante/Resources/GeneratedMaterials";
            const string path = dir + "/HighlightMat.mat";
            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null) return existing;
            System.IO.Directory.CreateDirectory(dir);
            var sh = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Unlit/Color") ?? Shader.Find("Standard");
            var m = new Material(sh) { name = "HighlightMat" };
            var c = new Color(1f, 0.9f, 0.2f, 1f);
            if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
            else m.color = c;
            if (m.HasProperty("_EmissionColor"))
            {
                m.EnableKeyword("_EMISSION");
                m.SetColor("_EmissionColor", c * 0.6f);
            }
            AssetDatabase.CreateAsset(m, path);
            AssetDatabase.SaveAssets();
            return m;
        }

        static string ClassifyObjectType(string raw)
        {
            string n = raw.ToLowerInvariant();
            if (n.Contains("salt") || n.Contains("sel") || n.Contains("grain")) return "sel";
            if (n.Contains("jar") || n.Contains("amphora") || n.Contains("jarre")) return "jarre";
            if (n.Contains("bowl") || n.Contains("bol")) return "bol";
            if (n.Contains("plate") || n.Contains("assiette")) return "assiette";
            if (n.Contains("food") || n.Contains("apple") || n.Contains("banana") || n.Contains("meat") || n.Contains("fish")) return "aliment";
            return "objet";
        }

        static string ClassifyVikingType(string raw)
        {
            string n = raw.ToLowerInvariant();
            if (n.Contains("fish")) return "poisson";
            if (n.Contains("bread") || n.Contains("pain")) return "pain";
            if (n.Contains("meat") || n.Contains("ham") || n.Contains("chicken")) return "viande";
            return "legume";
        }
    }
}
