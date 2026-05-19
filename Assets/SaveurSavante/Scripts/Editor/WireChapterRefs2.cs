using UnityEngine;
using UnityEditor;
using TMPro;
using SaveurSavante.Chapters.Egypte;
using SaveurSavante.Chapters.Vikings;
using SaveurSavante.Chapters.Titanic;
using SaveurSavante.Chapters.Gandhi;

namespace SaveurSavante.EditorTools
{
    public static class WireChapterRefs2
    {
        static Material sHighlight, sPreserved, sSpoiled, sRaw, sCooked;

        [MenuItem("SaveurSavante/Wire Chapter Refs 2 (FX+Mats)")]
        public static void Wire()
        {
            int created = 0, wired = 0;

            EnsureSharedMats();

            // === EGYPTE: Jarre ===
            foreach (var j in Object.FindObjectsOfType<Jarre>(true))
            {
                if (j.successParticles == null)
                { j.successParticles = MakeParticles(j.gameObject, "SuccessParticles", new Color(1f, 0.9f, 0.3f)); created++; wired++; }
                if (j.completionText == null)
                { j.completionText = MakeWorldText(j.gameObject, "CompletionText", "✅ Réussi !", new Vector3(0, 0.8f, 0)); created++; wired++; }
                EditorUtility.SetDirty(j);
            }

            // === EGYPTE: OfferingZone ===
            foreach (var oz in Object.FindObjectsOfType<OfferingZone>(true))
            {
                if (oz.successParticles == null)
                { oz.successParticles = MakeParticles(oz.gameObject, "SuccessParticles", new Color(0.3f, 1f, 0.3f)); created++; wired++; }
                if (oz.failureParticles == null)
                { oz.failureParticles = MakeParticles(oz.gameObject, "FailureParticles", new Color(1f, 0.2f, 0.2f), false); created++; wired++; }
                if (oz.completionText == null)
                { oz.completionText = MakeWorldText(oz.gameObject, "CompletionText", "✅ Offrande complète !", new Vector3(0, 1f, 0)); created++; wired++; }
                if (oz.validationText == null)
                { oz.validationText = MakeWorldText(oz.gameObject, "ValidationText", "", new Vector3(0, 0.7f, 0)); created++; wired++; }
                EditorUtility.SetDirty(oz);
            }

            // === EGYPTE: SaltApplication ===
            foreach (var sa in Object.FindObjectsOfType<SaltApplication>(true))
            {
                if (sa.preservedMaterial == null) { sa.preservedMaterial = sPreserved; wired++; }
                if (sa.spoiledMaterial == null) { sa.spoiledMaterial = sSpoiled; wired++; }
                EditorUtility.SetDirty(sa);
            }

            // === VIKINGS: NutritionManager.completionText ===
            foreach (var nm in Object.FindObjectsOfType<NutritionManager>(true))
            {
                if (nm.completionText == null)
                { nm.completionText = MakeWorldText(nm.gameObject, "CompletionText", "✅ Repas complet !", new Vector3(0, 1.8f, 0)); created++; wired++; }
                EditorUtility.SetDirty(nm);
            }

            // === VIKINGS: VikingFood materials + eatParticles ===
            foreach (var vf in Object.FindObjectsOfType<VikingFood>(true))
            {
                if (vf.rawMaterial == null) { vf.rawMaterial = sRaw; wired++; }
                if (vf.cookedMaterial == null) { vf.cookedMaterial = sCooked; wired++; }
                if (vf.eatParticles == null)
                { vf.eatParticles = MakeParticles(vf.gameObject, "EatParticles", new Color(1f, 0.7f, 0.2f), false); created++; wired++; }
                EditorUtility.SetDirty(vf);
            }

            // === TITANIC: PlateManager.sparkleEffect ===
            foreach (var pm in Object.FindObjectsOfType<PlateManager>(true))
            {
                if (pm.sparkleEffect == null)
                { pm.sparkleEffect = MakeParticles(pm.gameObject, "SparkleEffect", new Color(1f, 1f, 0.4f), false); created++; wired++; }
                EditorUtility.SetDirty(pm);
            }

            // === TITANIC: FoodGuidance particles ===
            foreach (var fg in Object.FindObjectsOfType<FoodGuidance>(true))
            {
                if (fg.correctParticles == null)
                { fg.correctParticles = MakeParticles(fg.gameObject, "CorrectParticles", new Color(0.3f, 1f, 0.3f), false); created++; wired++; }
                if (fg.wrongParticles == null)
                { fg.wrongParticles = MakeParticles(fg.gameObject, "WrongParticles", new Color(1f, 0.2f, 0.2f), false); created++; wired++; }
                EditorUtility.SetDirty(fg);
            }

            // === TITANIC: TitanicFood.highlightMaterial ===
            foreach (var tf in Object.FindObjectsOfType<TitanicFood>(true))
            {
                if (tf.highlightMaterial == null) { tf.highlightMaterial = sHighlight; wired++; EditorUtility.SetDirty(tf); }
            }

            // === GANDHI: BowlManager.zenEffect / completionText / balanceIndicator ===
            foreach (var bm in Object.FindObjectsOfType<BowlManager>(true))
            {
                if (bm.zenEffect == null)
                { bm.zenEffect = MakeParticles(bm.gameObject, "ZenEffect", new Color(0.6f, 0.9f, 1f), false); created++; wired++; }
                if (bm.completionText == null)
                { bm.completionText = MakeWorldText(bm.gameObject, "CompletionText", "🙏 Équilibre atteint !", new Vector3(0, 1f, 0)); created++; wired++; }
                if (bm.balanceIndicator == null)
                {
                    var bi = new GameObject("BalanceIndicator");
                    bi.transform.SetParent(bm.transform, false);
                    bi.transform.localPosition = new Vector3(0, 0.5f, 0);
                    bm.balanceIndicator = bi;
                    created++; wired++;
                }
                EditorUtility.SetDirty(bm);
            }

            // === GANDHI: TreasureHunt.successParticles ===
            foreach (var th in Object.FindObjectsOfType<TreasureHunt>(true))
            {
                if (th.successParticles == null)
                { th.successParticles = MakeParticles(th.gameObject, "SuccessParticles", new Color(1f, 0.9f, 0.3f), false); created++; wired++; }
                EditorUtility.SetDirty(th);
            }

            // === GANDHI: GandhiFood.spawnPosition + highlightMaterial ===
            foreach (var gf in Object.FindObjectsOfType<GandhiFood>(true))
            {
                if (gf.highlightMaterial == null) { gf.highlightMaterial = sHighlight; wired++; }
                if (gf.spawnPosition == null)
                {
                    var sp = new GameObject("SpawnPosition");
                    sp.transform.SetParent(gf.transform.parent, false);
                    sp.transform.position = gf.transform.position;
                    sp.transform.rotation = gf.transform.rotation;
                    gf.spawnPosition = sp.transform;
                    created++; wired++;
                }
                EditorUtility.SetDirty(gf);
            }

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

            Debug.Log($"✅ WireChapterRefs2: created {created} GameObject(s), wired {wired} reference(s).");
        }

        static void EnsureSharedMats()
        {
            var shader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Unlit/Color") ?? Shader.Find("Standard");
            sHighlight = MakeMat(shader, "HighlightMat", new Color(1f, 0.9f, 0.2f, 1f), true);
            sPreserved = MakeMat(shader, "PreservedMat", new Color(0.7f, 0.95f, 0.7f));
            sSpoiled   = MakeMat(shader, "SpoiledMat",   new Color(0.45f, 0.3f, 0.15f));
            sRaw       = MakeMat(shader, "RawMat",       new Color(0.85f, 0.35f, 0.35f));
            sCooked    = MakeMat(shader, "CookedMat",    new Color(0.55f, 0.3f, 0.15f));
        }

        static Material MakeMat(Shader s, string name, Color c, bool emissive = false)
        {
            var m = new Material(s) { name = name };
            if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
            else m.color = c;
            if (emissive && m.HasProperty("_EmissionColor"))
            {
                m.EnableKeyword("_EMISSION");
                m.SetColor("_EmissionColor", c * 0.6f);
            }
            return m;
        }

        static ParticleSystem MakeParticles(GameObject parent, string name, Color color, bool autoPlay = true)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent.transform, false);
            go.transform.localPosition = new Vector3(0, 0.5f, 0);
            var ps = go.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.startColor = color;
            main.startSize = 0.15f;
            main.startLifetime = 1.2f;
            if (!autoPlay) ps.Stop();
            return ps;
        }

        static GameObject MakeWorldText(GameObject anchor, string name, string text, Vector3 localOffset)
        {
            var go = new GameObject(name);
            go.transform.SetParent(anchor.transform, false);
            go.transform.localPosition = localOffset;
            go.transform.localScale = Vector3.one * 0.01f;
            var tmp = go.AddComponent<TextMeshPro>();
            tmp.fontSize = 4f;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.text = text;
            tmp.rectTransform.sizeDelta = new Vector2(40, 8);
            go.SetActive(false);
            return go;
        }
    }
}
