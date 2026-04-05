using UnityEngine;
using UnityEngine.Rendering;

public class GoldenSparkles : MonoBehaviour
{
    void Start()
    {
        ParticleSystem ps = gameObject.AddComponent<ParticleSystem>();

        // ✅ IGUAL QUE EL ORIGINAL
        ps.transform.localScale = Vector3.one * 0.002f;

        var main = ps.main;
        main.startLifetime = new ParticleSystem.MinMaxCurve(1.2f, 2f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.005f, 0.02f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.0003f, 0.001f);
        main.maxParticles = 15;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        main.scalingMode = ParticleSystemScalingMode.Local; // ✅ igual al original

        // ✅ emission en variable primero
        var emission = ps.emission;
        emission.rateOverTime = 3;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(0.02f, 0.01f, 0.02f);

        // 🎨 COLORES DARK: cobre oxidado en lugar de oro brillante
        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(new Color(0.75f, 0.38f, 0.05f), 0f),   // cobre
                new GradientColorKey(new Color(0.48f, 0.18f, 0.0f),  0.5f), // bronce oscuro
                new GradientColorKey(new Color(0.22f, 0.07f, 0.0f),  1f)    // marrón quemado
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0f,    0f),
                new GradientAlphaKey(0.6f,  0.2f),
                new GradientAlphaKey(0.5f,  0.8f),
                new GradientAlphaKey(0f,    1f)
            }
        );
        colorOverLifetime.color = gradient;

        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(
            0.4f, AnimationCurve.EaseInOut(0, 0, 1, 0.4f));

        var noise = ps.noise;
        noise.enabled = true;
        noise.strength = 0.002f;
        noise.frequency = 0.2f;

        var rend = ps.GetComponent<ParticleSystemRenderer>();
        rend.renderMode = ParticleSystemRenderMode.Billboard;
        rend.material = FireParticle.BuildTransparentMaterial(new Color(0.65f, 0.28f, 0.04f, 1f));

        ps.Play();
    }
}