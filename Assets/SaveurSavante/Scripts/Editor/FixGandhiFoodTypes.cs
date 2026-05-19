using UnityEngine;
using UnityEditor;
using SaveurSavante.Chapters.Gandhi;

namespace SaveurSavante.EditorTools
{
    public static class FixGandhiFoodTypes
    {
        static readonly string[] FruitKeywords = {
            "apple", "banana", "pear", "mandarine", "mandarin", "mango", "orange",
            "strawberry", "pineapple", "lemon", "kiwi", "kiwano", "melon", "pomegranate",
            "papaya", "peach", "grape", "fig", "grapefruit", "custard_apple", "avocado",
            "raspberry", "blueberry"
        };

        static readonly string[] GraineKeywords = {
            "graine", "grain", "seed", "rice", "wheat"
        };

        // Tout le reste = legume (pomme de terre, oignon, carotte, brocoli, poivron,
        // turnip, garlic, beetroot, zucchini, fennel, cabbage, broccoli, artichoke,
        // celery, radicchio, gourd, kohlrabi, parsnip, radish, calabaza, pumpkin,
        // heirloom_tomato, napa_cabbage, hokaido_pumpkin)

        [MenuItem("SaveurSavante/FixGandhiFoodTypes")]
        public static void Fix()
        {
            int updated = 0;
            var foods = Object.FindObjectsOfType<GandhiFood>(true);

            foreach (var food in foods)
            {
                string source = string.IsNullOrEmpty(food.foodName) ? food.gameObject.name : food.foodName;
                string newType = Classify(source);

                if (food.foodType != newType)
                {
                    Undo.RecordObject(food, "Fix Gandhi Food Type");
                    food.foodType = newType;
                    EditorUtility.SetDirty(food);
                    updated++;
                }
            }

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

            Debug.Log($"✅ FixGandhiFoodTypes: updated {updated} food(s).");
        }

        static string Classify(string raw)
        {
            string n = raw.ToLower();

            // "Pomme de terre" → legume (avant test apple/pomme)
            if (n.Contains("pomme de terre") || n.Contains("pomme_de_terre")
                || n.Contains("potato") || n.Contains("pommedeterre"))
                return "legume";

            // Graines
            foreach (var k in GraineKeywords)
                if (n.Contains(k)) return "graine";

            // Fruits
            if (n.Contains("pomme")) return "fruit"; // Pomme isolée = apple
            foreach (var k in FruitKeywords)
                if (n.Contains(k)) return "fruit";

            // Default: legume
            return "legume";
        }
    }
}
