using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Text;
using System.Collections.Generic;

namespace SaveurSavante.EditorTools
{
    public static class AuditChapterRefs
    {
        static readonly string[] TargetTypes = {
            "SaveurSavante.Chapters.Egypte.Jarre",
            "SaveurSavante.Chapters.Egypte.OfferingZone",
            "SaveurSavante.Chapters.Egypte.SaltApplication",
            "SaveurSavante.Chapters.Vikings.CookingStation",
            "SaveurSavante.Chapters.Vikings.NutritionManager",
            "SaveurSavante.Chapters.Vikings.VikingFood",
            "SaveurSavante.Chapters.Titanic.PlateManager",
            "SaveurSavante.Chapters.Titanic.FoodGuidance",
            "SaveurSavante.Chapters.Titanic.TitanicFood",
            "SaveurSavante.Chapters.Gandhi.BowlManager",
            "SaveurSavante.Chapters.Gandhi.TreasureHunt",
            "SaveurSavante.Chapters.Gandhi.GandhiFood",
        };

        [MenuItem("SaveurSavante/Audit Chapter Refs")]
        public static void Audit()
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== AUDIT CHAPTER REFS ===");

            foreach (var typeName in TargetTypes)
            {
                var t = System.Type.GetType(typeName + ", Assembly-CSharp");
                if (t == null) { sb.AppendLine($"⚠️ Type introuvable: {typeName}"); continue; }

                var instances = Object.FindObjectsOfType(t, true);
                sb.AppendLine($"\n--- {typeName} ({instances.Length} instance{(instances.Length>1?"s":"")}) ---");

                foreach (var inst in instances)
                {
                    var comp = inst as Component;
                    var go = comp != null ? comp.gameObject.name : inst.name;

                    var missing = new List<string>();
                    var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                    foreach (var f in t.GetFields(flags))
                    {
                        if (!f.IsPublic && f.GetCustomAttribute<SerializeField>() == null) continue;
                        if (f.FieldType == typeof(string)) continue;
                        if (f.FieldType.IsValueType) continue;
                        if (f.FieldType.IsArray) continue;
                        if (f.FieldType.IsGenericType) continue;
                        var v = f.GetValue(inst);
                        if (v == null || v.Equals(null)) missing.Add(f.Name);
                    }

                    if (missing.Count == 0)
                        sb.AppendLine($"  ✅ {go}");
                    else
                        sb.AppendLine($"  ❌ {go}: missing [{string.Join(", ", missing)}]");
                }
            }

            Debug.Log(sb.ToString());
        }
    }
}
