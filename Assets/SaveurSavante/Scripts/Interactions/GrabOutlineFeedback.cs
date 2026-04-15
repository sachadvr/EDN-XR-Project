using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace SaveurSavante.Interactions
{
    internal sealed class GrabOutlineFeedback
    {
        private static Material sharedOutlineMaterial;

        private readonly List<Renderer> outlineRenderers = new List<Renderer>();

        public GrabOutlineFeedback(GameObject target, float scaleMultiplier = 1.04f)
        {
            Material outlineMaterial = GetOutlineMaterial();
            if (target == null || outlineMaterial == null)
            {
                return;
            }

            foreach (MeshFilter meshFilter in target.GetComponentsInChildren<MeshFilter>(true))
            {
                MeshRenderer sourceRenderer = meshFilter.GetComponent<MeshRenderer>();
                if (sourceRenderer == null || meshFilter.sharedMesh == null)
                {
                    continue;
                }

                GameObject outlineObject = new GameObject($"{meshFilter.name}_GrabOutline")
                {
                    hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild
                };

                outlineObject.layer = meshFilter.gameObject.layer;
                outlineObject.transform.SetParent(meshFilter.transform, false);
                outlineObject.transform.localScale = Vector3.one * scaleMultiplier;

                MeshFilter outlineFilter = outlineObject.AddComponent<MeshFilter>();
                outlineFilter.sharedMesh = meshFilter.sharedMesh;

                MeshRenderer outlineRenderer = outlineObject.AddComponent<MeshRenderer>();
                outlineRenderer.sharedMaterial = outlineMaterial;
                outlineRenderer.shadowCastingMode = ShadowCastingMode.Off;
                outlineRenderer.receiveShadows = false;
                outlineRenderer.lightProbeUsage = LightProbeUsage.Off;
                outlineRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
                outlineRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
                outlineRenderer.enabled = false;

                outlineRenderers.Add(outlineRenderer);
            }
        }

        public void SetVisible(bool visible)
        {
            foreach (Renderer outlineRenderer in outlineRenderers)
            {
                if (outlineRenderer != null)
                {
                    outlineRenderer.enabled = visible;
                }
            }
        }

        public void Dispose()
        {
            foreach (Renderer outlineRenderer in outlineRenderers)
            {
                if (outlineRenderer != null)
                {
                    Object.Destroy(outlineRenderer.gameObject);
                }
            }

            outlineRenderers.Clear();
        }

        private static Material GetOutlineMaterial()
        {
            if (sharedOutlineMaterial != null)
            {
                return sharedOutlineMaterial;
            }

            Shader shader = Shader.Find("Universal Render Pipeline/Unlit")
                ?? Shader.Find("Unlit/Color")
                ?? Shader.Find("Sprites/Default")
                ?? Shader.Find("Standard");

            if (shader == null)
            {
                return null;
            }

            sharedOutlineMaterial = new Material(shader)
            {
                name = "GrabOutlineMaterial"
            };

            Color outlineColor = new Color(0.2f, 1f, 0.2f, 1f);

            if (sharedOutlineMaterial.HasProperty("_BaseColor"))
            {
                sharedOutlineMaterial.SetColor("_BaseColor", outlineColor);
            }

            if (sharedOutlineMaterial.HasProperty("_Color"))
            {
                sharedOutlineMaterial.SetColor("_Color", outlineColor);
            }

            if (sharedOutlineMaterial.HasProperty("_EmissionColor"))
            {
                sharedOutlineMaterial.EnableKeyword("_EMISSION");
                sharedOutlineMaterial.SetColor("_EmissionColor", outlineColor * 0.5f);
            }

            if (sharedOutlineMaterial.HasProperty("_Cull"))
            {
                sharedOutlineMaterial.SetFloat("_Cull", (float)CullMode.Front);
            }

            if (sharedOutlineMaterial.HasProperty("_CullMode"))
            {
                sharedOutlineMaterial.SetFloat("_CullMode", (float)CullMode.Front);
            }

            return sharedOutlineMaterial;
        }
    }
}
