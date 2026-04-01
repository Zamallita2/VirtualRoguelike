// Script: MagicAuraParticles.cs

using UnityEngine;

public class MagicAuraParticles : MonoBehaviour
{
    void Start()
    {
        CreateMagicAura();
    }

    void CreateMagicAura()
    {
        GameObject particleObj = new GameObject("ParticleSystem_MagicAura");
        particleObj.transform.SetParent(this.transform);
        particleObj.transform.localPosition = new Vector3(0, 0.5f, 0);

        ParticleSystem ps = particleObj.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startLifetime = 3f;
        main.startSpeed = 0.5f;
        main.startSize = 0.2f;
        main.startColor = new Color(0.5f, 0f, 1f, 0.8f); // Púrpura
        main.maxParticles = 200;
        main.loop = true;

        // Emisión
        var emission = ps.emission;
        emission.rateOverTime = 30;

        // Forma - Esfera alrededor del castillo
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 2f;
        shape.radiusThickness = 0.5f;

        // Color over lifetime - degradado mágico
        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(new Color(0.5f, 0f, 1f), 0.0f),    // Púrpura
                new GradientColorKey(new Color(0f, 0.5f, 1f), 0.5f),     // Azul
                new GradientColorKey(new Color(1f, 0f, 0.5f), 1.0f)      // Rosa
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0.8f, 0.0f),
                new GradientAlphaKey(0.5f, 0.5f),
                new GradientAlphaKey(0.0f, 1.0f)
            }
        );

        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);

        // Tamaño over lifetime
        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(0.0f, 0.3f);
        sizeCurve.AddKey(0.5f, 1.0f);
        sizeCurve.AddKey(1.0f, 0.0f);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        // Velocity over lifetime - movimiento orbital
        var velocityOverLifetime = ps.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.space = ParticleSystemSimulationSpace.World;
        velocityOverLifetime.orbitalY = 0.5f;
        velocityOverLifetime.orbitalZ = 0.3f;

        // Renderer
        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.material = CreateGlowMaterial();
    }

    Material CreateGlowMaterial()
    {
        Material mat = new Material(Shader.Find("Particles/Standard Unlit"));
        mat.SetColor("_Color", Color.white);
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", new Color(1f, 0.5f, 1f) * 2f);
        return mat;
    }
}