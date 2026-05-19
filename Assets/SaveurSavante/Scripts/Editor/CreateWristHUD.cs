using UnityEngine;
using UnityEditor;
using TMPro;
using SaveurSavante.Core;
using SaveurSavante.Chapters.Egypte;
using SaveurSavante.Chapters.Vikings;
using SaveurSavante.Chapters.Titanic;
using SaveurSavante.Chapters.Gandhi;

namespace SaveurSavante.EditorTools
{
    public static class CreateWristHUD
    {
        [MenuItem("SaveurSavante/Create Wrist HUD")]
        public static void Run()
        {
            var leftCtrl = GameObject.Find("Left Controller");
            if (leftCtrl == null)
            {
                Debug.LogError("❌ Left Controller introuvable.");
                return;
            }

            var existing = leftCtrl.transform.Find("WristHUD");
            GameObject hudGo;
            if (existing != null) hudGo = existing.gameObject;
            else
            {
                hudGo = new GameObject("WristHUD");
                hudGo.transform.SetParent(leftCtrl.transform, false);
            }
            hudGo.transform.localPosition = new Vector3(0f, 0.18f, -0.02f);
            hudGo.transform.localRotation = Quaternion.Euler(60f, 0f, 0f);
            hudGo.transform.localScale = Vector3.one * 0.02f;

            var hud = hudGo.GetComponent<WristHUD>();
            if (hud == null) hud = hudGo.AddComponent<WristHUD>();

            // Remove old BG if present (was opaque, masked text)
            var bgT = hudGo.transform.Find("BG");
            if (bgT != null) Object.DestroyImmediate(bgT.gameObject);

            hud.statusText = MakeText(hudGo, "StatusText", new Vector3(0, 290, 0), 20f, hud.statusText != null ? hud.statusText.text : "");
            hud.storyText  = MakeText(hudGo, "StoryText",  new Vector3(0, 240, 0), 16f, hud.storyText  != null ? hud.storyText.text  : "Bienvenue dans Saveur Savante.");

            EditorUtility.SetDirty(hud);

            // Wire all chapter statusText refs to wrist
            int wired = 0;
            foreach (var j in Object.FindObjectsOfType<Jarre>(true))
                if (j.statusText != hud.statusText) { j.statusText = hud.statusText; EditorUtility.SetDirty(j); wired++; }
            foreach (var nm in Object.FindObjectsOfType<NutritionManager>(true))
                if (nm.statusText != hud.statusText) { nm.statusText = hud.statusText; EditorUtility.SetDirty(nm); wired++; }
            foreach (var pm in Object.FindObjectsOfType<PlateManager>(true))
                if (pm.statusText != hud.statusText) { pm.statusText = hud.statusText; EditorUtility.SetDirty(pm); wired++; }
            foreach (var bm in Object.FindObjectsOfType<BowlManager>(true))
                if (bm.statusText != hud.statusText) { bm.statusText = hud.statusText; EditorUtility.SetDirty(bm); wired++; }

            // Wire StoryManager
            foreach (var sm in Object.FindObjectsOfType<StoryManager>(true))
            {
                sm.storyPanel = hud.storyText.gameObject;
                // storyText is TMP_UGUI in StoryManager; convert later. For now skip.
                EditorUtility.SetDirty(sm);
            }

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

            Debug.Log($"✅ WristHUD: created/updated. Wired {wired} statusText refs.");
        }

        static TextMeshPro MakeText(GameObject parent, string name, Vector3 localPos, float fontSize, string defaultText)
        {
            var existing = parent.transform.Find(name);
            TextMeshPro tmp;
            GameObject go;
            if (existing != null)
            {
                go = existing.gameObject;
                tmp = go.GetComponent<TextMeshPro>();
                if (tmp == null) tmp = go.AddComponent<TextMeshPro>();
            }
            else
            {
                go = new GameObject(name);
                go.transform.SetParent(parent.transform, false);
                tmp = go.AddComponent<TextMeshPro>();
            }
            go.transform.localPosition = localPos;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            tmp.fontSize = fontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.text = defaultText;
            tmp.color = Color.white;
            tmp.rectTransform.sizeDelta = new Vector2(70, 100);
            tmp.enableWordWrapping = true;
            tmp.overflowMode = TextOverflowModes.Overflow;
            tmp.lineSpacing = 15f;
            tmp.outlineWidth = 0.2f;
            tmp.outlineColor = Color.black;
            EditorUtility.SetDirty(tmp);
            return tmp;
        }
    }
}
