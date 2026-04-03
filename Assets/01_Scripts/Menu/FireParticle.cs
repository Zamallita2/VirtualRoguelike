using UnityEngine;
using UnityEngine.Rendering;

public class FireParticle : MonoBehaviour
{
    void Start()
    {
        CreateFire();
    }

    void CreateFire()
    {
        ParticleSystem ps = gameObject.AddComponent<ParticleSystem>();

        // ✅ IGUAL QUE EL ORIGINAL — escala local controla el tamaño
        ps.transform.localScale = Vector3.one * 0.01f;

        var main = ps.main;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.5f, 1f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.02f, 0.05f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.01f, 0.03f);
        main.maxParticles = 20;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        main.scalingMode = ParticleSystemScalingMode.Local; // ✅ igual al original

        // ✅ emission se guarda en variable ANTES de modificar (fix del error)
        var emission = ps.emission;
        emission.rateOverTime = 5;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 8f;
        shape.radius = 0.02f;

        // 🎨 COLORES DARK: ascua apagada en lugar de llama brillante
        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(new Color(0.85f, 0.38f, 0.05f), 0f),   // naranja quemado
                new GradientColorKey(new Color(0.5f,  0.10f, 0.0f),  0.5f), // rojo oscuro
                new GradientColorKey(new Color(0.12f, 0.03f, 0.0f),  1f)    // casi negro
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0.75f, 0f),
                new GradientAlphaKey(0.4f,  0.6f),
                new GradientAlphaKey(0f,    1f)
            }
        );
        colorOverLifetime.color = gradient;

        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(
            0.5f, AnimationCurve.EaseInOut(0, 0, 1, 0.5f));

        var rend = ps.GetComponent<ParticleSystemRenderer>();
        rend.renderMode = ParticleSystemRenderMode.Billboard;
        rend.material = BuildTransparentMaterial(new Color(0.55f, 0.15f, 0.01f, 1f));

        ps.Play();
    }

    // ─── MATERIAL TRANSPARENTE URP — sin fondo negro ─────────────────────
    // Estático para que GoldenSparkles, MagicAuraParticles y DoorGlowParticles lo reutilicen
    public static Material BuildTransparentMaterial(Color tint)
    {
        Shader shader = null;
        if (GraphicsSettings.currentRenderPipeline != null)
            shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        if (shader == null) shader = Shader.Find("Particles/Standard Unlit");
        if (shader == null) shader = Shader.Find("Legacy Shaders/Particles/Additive (Soft)");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        if (shader == null)
        {
            Debug.LogError("[Particles] No se encontró shader de partículas");
            return new Material(Shader.Find("Standard"));
        }

        Material mat = new Material(shader);

        // ── ACTIVA TRANSPARENCIA EN URP (sin esto → fondo negro) ──
        mat.SetFloat("_Surface", 1f); // 0=Opaque 1=Transparent
        mat.SetFloat("_Blend", 0f); // 0=Alpha
        mat.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
        mat.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
        mat.SetFloat("_ZWrite", 0f);
        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = (int)RenderQueue.Transparent;

        if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", tint);
        if (mat.HasProperty("_Color")) mat.SetColor("_Color", tint);
        if (mat.HasProperty("_TintColor")) mat.SetColor("_TintColor", tint);

        mat.mainTexture = BuildSoftCircleTexture();
        return mat;
    }

    // ─── TEXTURA CIRCULAR SUAVE — sin bordes cuadrados ───────────────────
    public static Texture2D BuildSoftCircleTexture()
    {
        int size = 32;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[size * size];
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f;

        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                float alpha = Mathf.Clamp01(1f - dist / radius);
                alpha = alpha * alpha; // suavizado
                pixels[y * size + x] = new Color(1f, 1f, 1f, alpha);
            }

        tex.SetPixels(pixels);
        tex.Apply();
        tex.wrapMode = TextureWrapMode.Clamp;
        return tex;
    }
}