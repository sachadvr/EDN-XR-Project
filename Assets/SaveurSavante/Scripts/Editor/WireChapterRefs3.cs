using System.IO;
using UnityEngine;
using UnityEditor;
using SaveurSavante.Chapters.Egypte;
using SaveurSavante.Chapters.Vikings;
using SaveurSavante.Chapters.Titanic;
using SaveurSavante.Chapters.Gandhi;

namespace SaveurSavante.EditorTools
{
    public static class WireChapterRefs3
    {
        const string AudioDir = "Assets/SaveurSavante/Resources/GeneratedAudio";
        const string PrefabDir = "Assets/SaveurSavante/Resources/GeneratedPrefabs";

        [MenuItem("SaveurSavante/Wire Chapter Refs 3 (Audio+Prefabs)")]
        public static void Wire()
        {
            Directory.CreateDirectory(AudioDir);
            Directory.CreateDirectory(PrefabDir);

            // Generate WAV clips
            var clipSuccess = GenClip("success", BuildSuccess());
            var clipFailure = GenClip("failure", BuildFailure());
            var clipEat     = GenClip("eat",     BuildEat());
            var clipCook    = GenClip("cooking", BuildCookingLoop());
            var clipCooked  = GenClip("cooked",  BuildCooked());
            var clipZen     = GenClip("zen",     BuildZen());
            var clipRiddle  = GenClip("riddle",  BuildRiddle());
            var clipCorrect = GenClip("correct", BuildCorrect());
            var clipWrong   = GenClip("wrong",   BuildWrong());

            // Generate prefabs
            var saltPrefab        = GenSpherePrefab("SaltPrefab",        new Color(1f, 1f, 1f),       0.04f);
            var cookedVisualPrefab= GenSpherePrefab("CookedVisualPrefab",new Color(0.55f,0.3f,0.15f), 0.25f);
            var platedPrefab      = GenCylinderPrefab("PlatedPrefab",    new Color(0.95f,0.95f,0.9f), 0.3f, 0.02f);

            int wired = 0;

            // EGYPTE Jarre
            foreach (var j in Object.FindObjectsOfType<Jarre>(true))
            {
                if (j.successSound == null) { j.successSound = clipSuccess; wired++; }
                EditorUtility.SetDirty(j);
            }

            // EGYPTE OfferingZone
            foreach (var oz in Object.FindObjectsOfType<OfferingZone>(true))
            {
                if (oz.successSound == null) { oz.successSound = clipSuccess; wired++; }
                if (oz.failureSound == null) { oz.failureSound = clipFailure; wired++; }
                EditorUtility.SetDirty(oz);
            }

            // EGYPTE SaltApplication
            foreach (var sa in Object.FindObjectsOfType<SaltApplication>(true))
            {
                if (sa.saltPrefab == null) { sa.saltPrefab = saltPrefab; wired++; }
                EditorUtility.SetDirty(sa);
            }

            // VIKINGS CookingStation
            foreach (var cs in Object.FindObjectsOfType<CookingStation>(true))
            {
                if (cs.cookingSound == null) { cs.cookingSound = clipCook; wired++; }
                if (cs.cookedSound  == null) { cs.cookedSound  = clipCooked; wired++; }
                if (cs.cookedVisualPrefab == null) { cs.cookedVisualPrefab = cookedVisualPrefab; wired++; }
                EditorUtility.SetDirty(cs);
            }

            // VIKINGS NutritionManager
            foreach (var nm in Object.FindObjectsOfType<NutritionManager>(true))
            {
                if (nm.successSound == null) { nm.successSound = clipSuccess; wired++; }
                if (nm.eatSound     == null) { nm.eatSound     = clipEat; wired++; }
                EditorUtility.SetDirty(nm);
            }

            // VIKINGS VikingFood
            foreach (var vf in Object.FindObjectsOfType<VikingFood>(true))
            {
                if (vf.eatSound == null) { vf.eatSound = clipEat; wired++; }
                if (vf.cookedVariant == null) { vf.cookedVariant = cookedVisualPrefab; wired++; }
                EditorUtility.SetDirty(vf);
            }

            // TITANIC PlateManager
            foreach (var pm in Object.FindObjectsOfType<PlateManager>(true))
            {
                if (pm.successSound == null) { pm.successSound = clipSuccess; wired++; }
                EditorUtility.SetDirty(pm);
            }

            // TITANIC FoodGuidance
            foreach (var fg in Object.FindObjectsOfType<FoodGuidance>(true))
            {
                if (fg.correctSound == null) { fg.correctSound = clipCorrect; wired++; }
                if (fg.wrongSound   == null) { fg.wrongSound   = clipWrong; wired++; }
                EditorUtility.SetDirty(fg);
            }

            // TITANIC TitanicFood
            foreach (var tf in Object.FindObjectsOfType<TitanicFood>(true))
            {
                if (tf.platedPrefab == null) { tf.platedPrefab = platedPrefab; wired++; EditorUtility.SetDirty(tf); }
            }

            // GANDHI BowlManager
            foreach (var bm in Object.FindObjectsOfType<BowlManager>(true))
            {
                if (bm.successSound == null) { bm.successSound = clipSuccess; wired++; }
                if (bm.zenSound     == null) { bm.zenSound     = clipZen; wired++; }
                EditorUtility.SetDirty(bm);
            }

            // GANDHI TreasureHunt
            foreach (var th in Object.FindObjectsOfType<TreasureHunt>(true))
            {
                if (th.correctSound == null) { th.correctSound = clipCorrect; wired++; }
                if (th.wrongSound   == null) { th.wrongSound   = clipWrong; wired++; }
                if (th.riddleSound  == null) { th.riddleSound  = clipRiddle; wired++; }
                EditorUtility.SetDirty(th);
            }

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

            Debug.Log($"✅ WireChapterRefs3: wired {wired} reference(s).");
        }

        // ===== WAV gen =====
        const int SR = 22050;

        static AudioClip GenClip(string name, float[] samples)
        {
            string path = $"{AudioDir}/{name}.wav";
            WriteWav(path, samples, SR);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
            return AssetDatabase.LoadAssetAtPath<AudioClip>(path);
        }

        static void WriteWav(string path, float[] samples, int sr)
        {
            using (var fs = new FileStream(path, FileMode.Create))
            using (var bw = new BinaryWriter(fs))
            {
                int byteCount = samples.Length * 2;
                bw.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
                bw.Write(36 + byteCount);
                bw.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));
                bw.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
                bw.Write(16);
                bw.Write((short)1);     // PCM
                bw.Write((short)1);     // mono
                bw.Write(sr);
                bw.Write(sr * 2);
                bw.Write((short)2);
                bw.Write((short)16);
                bw.Write(System.Text.Encoding.ASCII.GetBytes("data"));
                bw.Write(byteCount);
                foreach (var s in samples)
                {
                    short v = (short)(Mathf.Clamp(s, -1f, 1f) * 32767);
                    bw.Write(v);
                }
            }
        }

        static float Env(int i, int n, float a = 0.05f, float r = 0.2f)
        {
            float t = (float)i / n;
            float atk = Mathf.Min(1f, t / a);
            float rel = Mathf.Min(1f, (1f - t) / r);
            return atk * rel;
        }

        static float[] BuildSuccess()
        {
            int n = SR; var s = new float[n];
            for (int i = 0; i < n; i++)
            {
                float t = (float)i / SR;
                float f = Mathf.Lerp(440f, 880f, t);
                s[i] = Mathf.Sin(2 * Mathf.PI * f * t) * 0.4f * Env(i, n, 0.05f, 0.4f);
            }
            return s;
        }

        static float[] BuildFailure()
        {
            int n = SR; var s = new float[n];
            for (int i = 0; i < n; i++)
            {
                float t = (float)i / SR;
                float f = Mathf.Lerp(330f, 110f, t);
                s[i] = Mathf.Sin(2 * Mathf.PI * f * t) * 0.4f * Env(i, n, 0.02f, 0.4f);
            }
            return s;
        }

        static float[] BuildEat()
        {
            int n = SR / 4; var s = new float[n];
            var rnd = new System.Random(1);
            for (int i = 0; i < n; i++)
            {
                float noise = (float)(rnd.NextDouble() * 2 - 1);
                s[i] = noise * 0.3f * Env(i, n, 0.02f, 0.5f);
            }
            return s;
        }

        static float[] BuildCookingLoop()
        {
            int n = SR * 2; var s = new float[n];
            var rnd = new System.Random(2);
            for (int i = 0; i < n; i++)
            {
                float noise = (float)(rnd.NextDouble() * 2 - 1);
                s[i] = noise * 0.15f;
            }
            return s;
        }

        static float[] BuildCooked()
        {
            int n = SR / 2; var s = new float[n];
            for (int i = 0; i < n; i++)
            {
                float t = (float)i / SR;
                float f = 660f;
                s[i] = Mathf.Sin(2 * Mathf.PI * f * t) * 0.4f * Env(i, n, 0.05f, 0.5f);
            }
            return s;
        }

        static float[] BuildZen()
        {
            int n = SR * 2; var s = new float[n];
            for (int i = 0; i < n; i++)
            {
                float t = (float)i / SR;
                float a = Mathf.Sin(2 * Mathf.PI * 220f * t);
                float b = Mathf.Sin(2 * Mathf.PI * 330f * t);
                s[i] = (a + b) * 0.2f * Env(i, n, 0.2f, 0.5f);
            }
            return s;
        }

        static float[] BuildRiddle()
        {
            int n = SR; var s = new float[n];
            float[] freqs = { 523f, 659f, 784f };
            int seg = n / freqs.Length;
            for (int k = 0; k < freqs.Length; k++)
                for (int i = 0; i < seg; i++)
                {
                    float t = (float)i / SR;
                    s[k * seg + i] = Mathf.Sin(2 * Mathf.PI * freqs[k] * t) * 0.35f * Env(i, seg, 0.05f, 0.3f);
                }
            return s;
        }

        static float[] BuildCorrect()
        {
            int n = SR / 2; var s = new float[n];
            for (int i = 0; i < n; i++)
            {
                float t = (float)i / SR;
                float f = i < n / 2 ? 660f : 990f;
                s[i] = Mathf.Sin(2 * Mathf.PI * f * t) * 0.4f * Env(i, n, 0.02f, 0.3f);
            }
            return s;
        }

        static float[] BuildWrong()
        {
            int n = SR / 2; var s = new float[n];
            for (int i = 0; i < n; i++)
            {
                float t = (float)i / SR;
                s[i] = Mathf.Sin(2 * Mathf.PI * 150f * t) * 0.4f * Env(i, n, 0.02f, 0.3f);
            }
            return s;
        }

        // ===== Prefabs =====
        static GameObject GenSpherePrefab(string name, Color color, float scale)
        {
            string path = $"{PrefabDir}/{name}.prefab";
            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existing != null) return existing;

            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = name;
            go.transform.localScale = Vector3.one * scale;
            ApplyMat(go, color);
            var prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            return prefab;
        }

        static GameObject GenCylinderPrefab(string name, Color color, float radius, float height)
        {
            string path = $"{PrefabDir}/{name}.prefab";
            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existing != null) return existing;

            var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = name;
            go.transform.localScale = new Vector3(radius * 2, height, radius * 2);
            ApplyMat(go, color);
            var prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            return prefab;
        }

        static void ApplyMat(GameObject go, Color color)
        {
            var rend = go.GetComponent<MeshRenderer>();
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            var mat = new Material(shader) { name = go.name + "_Mat" };
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
            else mat.color = color;
            rend.sharedMaterial = mat;
        }
    }
}
