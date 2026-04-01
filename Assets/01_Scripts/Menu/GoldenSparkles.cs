// Script: GoldenSparkles.cs

using UnityEngine;

public class GoldenSparkles : MonoBehaviour
{
    void Start()
    {
        CreateSparkles();
    }

    void CreateSparkles()
    {
        GameObject sparkleObj = new GameObject("ParticleSystem_Sparkles");
        sparkleObj.transform.SetParent(this.transform);
        sparkleObj.transform.localPosition = new Vector3(0, 2f, 0);

        ParticleSystem ps = sparkleObj.AddComponent<ParticleSystem>();

        var main = ps.main;
        main.startLifetime = new ParticleSystem.MinMaxCurve(2f, 4f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.2f, 0.8f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.15f);
        main.startRotation = new ParticleSystem.MinMaxCurve(0, 360 * Mathf.Deg2Rad);
        main.maxParticles = 100;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        // Emission
        var emission = ps.emission;
        emission.rateOverTime = 15;

        // Bursts periódicos
        ParticleSystem.Burst burst = new ParticleSystem.Burst(0f, 20);
        emission.SetBurst(0, burst);

        // Shape - Box grande sobre el castillo
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(3f, 0.5f, 3f);

        // Color - Dorado brillante
        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;

        Gradient sparkleGradient = new Gradient();
        sparkleGradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(new Color(1f, 1f, 0.8f), 0.0f),
                new GradientColorKey(new Color(1f, 0.84f, 0f), 0.5f),
                new GradientColorKey(new Color(1f, 0.6f, 0f), 1.0f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0f, 0.0f),
                new GradientAlphaKey(1f, 0.2f),
                new GradientAlphaKey(1f, 0.8f),
                new GradientAlphaKey(0f, 1.0f)
            }
        );

        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(sparkleGradient);

        // Velocity over lifetime - caída suave
        var velocityOverLifetime = ps.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(-0.5f);
        velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(-0.2f, 0.2f);
        velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(-0.2f, 0.2f);

        // Size over lifetime - parpadeo
        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(0.0f, 0f);
        sizeCurve.AddKey(0.1f, 1f);
        sizeCurve.AddKey(0.5f, 0.8f);
        sizeCurve.AddKey(0.9f, 1f);
        sizeCurve.AddKey(1.0f, 0f);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        // Rotation over lifetime
        var rotationOverLifetime = ps.rotationOverLifetime;
        rotationOverLifetime.enabled = true;
        rotationOverLifetime.z = new ParticleSystem.MinMaxCurve(-180 * Mathf.Deg2Rad, 180 * Mathf.Deg2Rad);

        // Renderer
        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.material = CreateSparkleMaterial();
    }

    Material CreateSparkleMaterial()
    {
        Material mat = new Material(Shader.Find("Particles/Standard Unlit"));
        mat.SetColor("_Color", new Color(1f, 0.84f, 0f));
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", new Color(1f, 0.84f, 0f) * 5f);
        return mat;
    }
}