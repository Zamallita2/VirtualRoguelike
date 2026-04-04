using UnityEngine;
using UnityEngine.Rendering;

public class MagicAuraParticles : MonoBehaviour
{
    void Start()
    {
        CreateMagicAura();
    }

    void CreateMagicAura()
    {
        ParticleSystem ps = gameObject.AddComponent<ParticleSystem>();

        // ✅ IGUAL QUE EL ORIGINAL
        ps.transform.localScale = Vector3.one * 0.02f;

        var main = ps.main;
        main.startLifetime = 2f;
        main.startSpeed = 0.02f;
        main.startSize = 0.02f;
        main.startColor = new Color(0.25f, 0f, 0.4f, 0.5f); // púrpura muy oscuro
        main.maxParticles = 30;
        main.loop = true;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        main.scalingMode = ParticleSystemScalingMode.Local; // ✅ igual al original

        // ✅ emission en variable primero
        var emission = ps.emission;
        emission.rateOverTime = 6;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.2f;

        // 🎨 COLORES DARK: púrpura sombrío → azul noche → negro
        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(new Color(0.3f,  0.0f,  0.45f), 0f),   // púrpura oscuro
                new GradientColorKey(new Color(0.05f, 0.08f, 0.3f),  0.5f), // azul noche
                new GradientColorKey(new Color(0.02f, 0.02f, 0.07f), 1f)    // casi negro
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0.55f, 0f),
                new GradientAlphaKey(0.3f,  0.5f),
                new GradientAlphaKey(0f,    1f)
            }
        );
        colorOverLifetime.color = gradient;

        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(
            0.6f, AnimationCurve.EaseInOut(0, 0, 1, 0.6f));

        var noise = ps.noise;
        noise.enabled = true;
        noise.strength = 0.01f;
        noise.frequency = 0.3f;

        var rend = ps.GetComponent<ParticleSystemRenderer>();
        rend.renderMode = ParticleSystemRenderMode.Billboard;
        rend.material = FireParticle.BuildTransparentMaterial(new Color(0.22f, 0f, 0.38f, 1f));

        ps.Play();
    }
}