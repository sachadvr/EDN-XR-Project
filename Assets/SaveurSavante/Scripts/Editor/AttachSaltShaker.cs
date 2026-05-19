using UnityEngine;
using UnityEditor;
using SaveurSavante.Interactions;

namespace SaveurSavante.EditorTools
{
    public static class AttachSaltShaker
    {
        [MenuItem("SaveurSavante/Attach Salt Shaker")]
        public static void Run()
        {
            int updated = 0;
            foreach (var grab in Object.FindObjectsOfType<GrabbableObject>(true))
            {
                if (grab.objectType != "sel") continue;

                var go = grab.gameObject;
                var shaker = go.GetComponent<SaltShaker>();
                if (shaker == null) shaker = Undo.AddComponent<SaltShaker>(go);

                // Particle child
                var psT = go.transform.Find("SaltParticles");
                ParticleSystem ps;
                if (psT != null)
                {
                    ps = psT.GetComponent<ParticleSystem>();
                }
                else
                {
                    var psGo = new GameObject("SaltParticles");
                    psGo.transform.SetParent(go.transform, false);
                    psGo.transform.localPosition = Vector3.zero;
                    ps = psGo.AddComponent<ParticleSystem>();
                }

                ConfigurePS(ps);
                shaker.saltParticles = ps;
                shaker.emissionPoint = ps.transform;

                EditorUtility.SetDirty(shaker);
                EditorUtility.SetDirty(ps);
                updated++;
            }

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

            Debug.Log($"✅ AttachSaltShaker: configured {updated} sel(s).");
        }

        static void ConfigurePS(ParticleSystem ps)
        {
            var main = ps.main;
            main.duration = 5f;
            main.loop = true;
            main.startLifetime = 1.2f;
            main.startSpeed = 0.4f;
            main.startSize = 0.015f;
            main.startColor = new Color(1f, 1f, 1f, 1f);
            main.gravityModifier = 1.5f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.maxParticles = 500;

            var emission = ps.emission;
            emission.rateOverTime = 0f;

            var shape = ps.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 25f;
            shape.radius = 0.02f;
            shape.rotation = new Vector3(90, 0, 0); // point downward

            var renderer = ps.GetComponent<ParticleSystemRenderer>();
            var sh = Shader.Find("Universal Render Pipeline/Particles/Unlit") ?? Shader.Find("Particles/Standard Unlit") ?? Shader.Find("Sprites/Default");
            var mat = new Material(sh) { name = "SaltParticleMat" };
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", Color.white);
            else mat.color = Color.white;
            renderer.sharedMaterial = mat;

            ps.Stop();
            ps.Clear();
        }
    }
}
