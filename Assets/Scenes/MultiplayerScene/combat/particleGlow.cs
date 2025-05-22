using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
[RequireComponent(typeof(ParticleSystem))]
public class ParticleGlow : MonoBehaviour
{
    [ColorUsage(true, true)] public Color glowColor = Color.white;
    [Range(1f, 20f)] public float intensity = 4f;

    void OnEnable()   => Apply();
    void OnValidate() => Apply();

    void Apply()
    {
        var ps   = GetComponent<ParticleSystem>();
        var main = ps.main;
        main.startColor = glowColor * intensity;

        var rnd = ps.GetComponent<ParticleSystemRenderer>();
        if (rnd.sharedMaterial == null)
        {
            Shader s = GraphicsSettings.currentRenderPipeline == null
                ? Shader.Find("Particles/Additive")
                : Shader.Find("Universal Render Pipeline/Particles/Unlit");
            if (s != null) rnd.sharedMaterial = new Material(s);
        }
    }
}
