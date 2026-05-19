using UnityEngine;
using UnityEditor;
using TMPro;
using SaveurSavante.Chapters.Egypte;
using SaveurSavante.Chapters.Vikings;
using SaveurSavante.Chapters.Titanic;
using SaveurSavante.Chapters.Gandhi;

namespace SaveurSavante.EditorTools
{
    public static class WireChapterRefs
    {
        [MenuItem("SaveurSavante/Wire Chapter Refs")]
        public static void Wire()
        {
            int created = 0, wired = 0;

            // === EGYPTE: OfferingZone.jarreResetPosition ===
            foreach (var oz in Object.FindObjectsOfType<OfferingZone>(true))
            {
                if (oz.jarreResetPosition == null)
                {
                    var jarre = Object.FindObjectOfType<Jarre>(true);
                    Vector3 pos = jarre != null ? jarre.transform.position : oz.transform.position + Vector3.left * 2f;
                    var go = new GameObject("JarreResetPosition");
                    go.transform.SetParent(oz.transform.parent, false);
                    go.transform.position = pos;
                    Undo.RecordObject(oz, "Wire");
                    oz.jarreResetPosition = go.transform;
                    EditorUtility.SetDirty(oz);
                    created++; wired++;
                }
            }

            // === VIKINGS: CookingStation FX ===
            foreach (var cs in Object.FindObjectsOfType<CookingStation>(true))
            {
                if (cs.fireParticles == null)
                {
                    cs.fireParticles = CreateParticleChild(cs.gameObject, "FireParticles", new Color(1f, 0.5f, 0.1f), true);
                    created++; wired++;
                }
                if (cs.smokeParticles == null)
                {
                    cs.smokeParticles = CreateParticleChild(cs.gameObject, "SmokeParticles", new Color(0.4f, 0.4f, 0.4f), false);
                    created++; wired++;
                }
                if (cs.fireLight == null)
                {
                    var lightGo = new GameObject("FireLight");
                    lightGo.transform.SetParent(cs.transform, false);
                    lightGo.transform.localPosition = new Vector3(0, 0.5f, 0);
                    var lt = lightGo.AddComponent<Light>();
                    lt.type = LightType.Point;
                    lt.color = new Color(1f, 0.5f, 0.1f);
                    lt.intensity = 2f;
                    lt.range = 5f;
                    cs.fireLight = lt;
                    created++; wired++;
                }
                EditorUtility.SetDirty(cs);
            }

            // === VIKINGS: NutritionManager UI ===
            foreach (var nm in Object.FindObjectsOfType<NutritionManager>(true))
            {
                var canvas = EnsureWorldCanvas(nm.gameObject, "VikingHUD", new Vector3(0, 1.5f, 0));
                if (nm.statusText == null)
                {
                    nm.statusText = CreateTMP(canvas, "StatusText", new Vector3(0, 0.4f, 0), "");
                    created++; wired++;
                }
                if (nm.energyText == null)
                {
                    nm.energyText = CreateTMP(canvas, "EnergyText", new Vector3(-0.3f, 0.1f, 0), "Énergie: 0");
                    created++; wired++;
                }
                if (nm.satietyText == null)
                {
                    nm.satietyText = CreateTMP(canvas, "SatietyText", new Vector3(0.3f, 0.1f, 0), "Satiété: 0");
                    created++; wired++;
                }
                if (nm.energyBar == null)
                {
                    nm.energyBar = CreateBar(canvas, "EnergyBar", new Vector3(-0.3f, -0.1f, 0), Color.green, out var fill);
                    nm.energyBarFill = fill;
                    created += 2; wired += 2;
                }
                if (nm.satietyBar == null)
                {
                    nm.satietyBar = CreateBar(canvas, "SatietyBar", new Vector3(0.3f, -0.1f, 0), Color.yellow, out var fill);
                    nm.satietyBarFill = fill;
                    created += 2; wired += 2;
                }
                EditorUtility.SetDirty(nm);
            }

            // === VIKINGS: VikingFood.placementSpot ===
            var nutMgr = Object.FindObjectOfType<NutritionManager>(true);
            Transform tableSpot = null;
            if (nutMgr != null)
            {
                tableSpot = nutMgr.transform.Find("FoodPlacementSpot");
                if (tableSpot == null)
                {
                    var tg = new GameObject("FoodPlacementSpot");
                    tg.transform.SetParent(nutMgr.transform, false);
                    tg.transform.localPosition = new Vector3(0, 0.5f, 0);
                    tableSpot = tg.transform;
                    created++;
                }
            }
            foreach (var vf in Object.FindObjectsOfType<VikingFood>(true))
            {
                if (vf.placementSpot == null && tableSpot != null)
                {
                    Undo.RecordObject(vf, "Wire");
                    vf.placementSpot = tableSpot;
                    EditorUtility.SetDirty(vf);
                    wired++;
                }
            }

            // === TITANIC ===
            foreach (var pm in Object.FindObjectsOfType<PlateManager>(true))
            {
                var canvas = EnsureWorldCanvas(pm.gameObject, "TitanicHUD", new Vector3(0, 0.6f, 0));
                if (pm.completionText == null)
                {
                    var go = new GameObject("CompletionText");
                    go.transform.SetParent(canvas.transform, false);
                    var tmp = go.AddComponent<TextMeshPro>();
                    tmp.fontSize = 0.3f;
                    tmp.alignment = TextAlignmentOptions.Center;
                    tmp.text = "✅ Repas servi !";
                    go.SetActive(false);
                    pm.completionText = go;
                    created++; wired++;
                }
                EditorUtility.SetDirty(pm);
            }
            foreach (var fg in Object.FindObjectsOfType<FoodGuidance>(true))
            {
                if (fg.instructionPanel == null)
                {
                    var canvas = EnsureWorldCanvas(fg.gameObject, "TitanicGuidance", new Vector3(0, 1.2f, 0));
                    var go = new GameObject("InstructionPanel");
                    go.transform.SetParent(canvas.transform, false);
                    var tmp = go.AddComponent<TextMeshPro>();
                    tmp.fontSize = 0.25f;
                    tmp.alignment = TextAlignmentOptions.Center;
                    tmp.text = "Place les aliments dans l'ordre.";
                    fg.instructionPanel = go;
                    created++; wired++;
                }
                EditorUtility.SetDirty(fg);
            }

            // === GANDHI ===
            foreach (var bm in Object.FindObjectsOfType<BowlManager>(true))
            {
                if (bm.statusText == null)
                {
                    var canvas = EnsureWorldCanvas(bm.gameObject, "GandhiHUD", new Vector3(0, 0.8f, 0));
                    bm.statusText = CreateTMP(canvas, "StatusText", Vector3.zero, "");
                    created++; wired++;
                }
                EditorUtility.SetDirty(bm);
            }
            foreach (var th in Object.FindObjectsOfType<TreasureHunt>(true))
            {
                var canvas = EnsureWorldCanvas(th.gameObject, "GandhiRiddle", new Vector3(0, 1.3f, 0));
                if (th.riddlePanel == null)
                {
                    var panel = new GameObject("RiddlePanel");
                    panel.transform.SetParent(canvas.transform, false);
                    th.riddlePanel = panel;
                    created++; wired++;
                }
                if (th.riddleText == null)
                {
                    th.riddleText = CreateTMP(th.riddlePanel, "RiddleText", new Vector3(0, 0.15f, 0), "Énigme...");
                    created++; wired++;
                }
                if (th.feedbackText == null)
                {
                    th.feedbackText = CreateTMP(th.riddlePanel, "FeedbackText", new Vector3(0, -0.15f, 0), "");
                    created++; wired++;
                }
                EditorUtility.SetDirty(th);
            }

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

            Debug.Log($"✅ WireChapterRefs: created {created} GameObject(s), wired {wired} reference(s).");
        }

        static ParticleSystem CreateParticleChild(GameObject parent, string name, Color color, bool autoPlay)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent.transform, false);
            go.transform.localPosition = new Vector3(0, 0.3f, 0);
            var ps = go.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.startColor = color;
            main.startSize = 0.2f;
            main.startLifetime = 1f;
            if (!autoPlay) ps.Stop();
            return ps;
        }

        static GameObject EnsureWorldCanvas(GameObject anchor, string name, Vector3 localOffset)
        {
            var existing = anchor.transform.Find(name);
            if (existing != null) return existing.gameObject;

            var go = new GameObject(name);
            go.transform.SetParent(anchor.transform, false);
            go.transform.localPosition = localOffset;
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            go.transform.localScale = Vector3.one * 0.01f;
            return go;
        }

        static TextMeshPro CreateTMP(GameObject parent, string name, Vector3 localPos, string text)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent.transform, false);
            go.transform.localPosition = localPos;
            var tmp = go.AddComponent<TextMeshPro>();
            tmp.fontSize = 4f;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.text = text;
            tmp.rectTransform.sizeDelta = new Vector2(40, 8);
            return tmp;
        }

        static GameObject CreateBar(GameObject parent, string name, Vector3 localPos, Color fillColor, out Transform fillOut)
        {
            var bar = new GameObject(name);
            bar.transform.SetParent(parent.transform, false);
            bar.transform.localPosition = localPos;
            bar.transform.localScale = new Vector3(0.4f, 0.05f, 0.01f);

            var bg = GameObject.CreatePrimitive(PrimitiveType.Quad);
            bg.name = "BG";
            bg.transform.SetParent(bar.transform, false);
            Object.DestroyImmediate(bg.GetComponent<Collider>());
            var bgRend = bg.GetComponent<MeshRenderer>();
            bgRend.sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Unlit/Color"));
            if (bgRend.sharedMaterial.HasProperty("_BaseColor")) bgRend.sharedMaterial.SetColor("_BaseColor", Color.gray);
            else bgRend.sharedMaterial.color = Color.gray;

            var fill = GameObject.CreatePrimitive(PrimitiveType.Quad);
            fill.name = "Fill";
            fill.transform.SetParent(bar.transform, false);
            fill.transform.localPosition = new Vector3(0, 0, -0.001f);
            Object.DestroyImmediate(fill.GetComponent<Collider>());
            var fillRend = fill.GetComponent<MeshRenderer>();
            fillRend.sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Unlit/Color"));
            if (fillRend.sharedMaterial.HasProperty("_BaseColor")) fillRend.sharedMaterial.SetColor("_BaseColor", fillColor);
            else fillRend.sharedMaterial.color = fillColor;

            fillOut = fill.transform;
            return bar;
        }
    }
}
