// Script: FireTorchParticles.cs

using UnityEngine;

public class FireTorchParticles : MonoBehaviour
{
    public Transform[] torchPositions; // Asignar manualmente o crear

    void Start()
    {
        CreateTorchPositions();

        foreach (Transform torchPos in torchPositions)
        {
            CreateFireParticle(torchPos);
        }
    }

    void CreateTorchPositions()
    {
        if (torchPositions == null || torchPositions.Length == 0)
        {
            // Crear 4 antorchas alrededor del castillo
            torchPositions = new Transform[4];

            Vector3[] positions = new Vector3[]
            {
                new Vector3(-1.5f, 1f, 1.5f),   // Frontal izquierda
                new Vector3(1.5f, 1f, 1.5f),    // Frontal derecha
                new Vector3(-1.5f, 1f, -1.5f),  // Trasera izquierda
                new Vector3(1.5f, 1f, -1.5f)    // Trasera derecha
            };

            for (int i = 0; i < 4; i++)
            {
                GameObject torchObj = new GameObject($"TorchPosition_{i}");
                torchObj.transform.SetParent(this.transform);
                torchObj.transform.localPosition = positions[i];
                torchPositions[i] = torchObj.transform;
            }
        }
    }

    void CreateFireParticle(Transform parent)
    {
        GameObject fireObj = new GameObject("Fire");
        fireObj.transform.SetParent(parent);
        fireObj.transform.localPosition = Vector3.zero;

        ParticleSystem ps = fireObj.AddComponent<ParticleSystem>();

        // Main module
        var main = ps.main;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.5f, 1.5f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.5f, 2f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.3f);
        main.startRotation = new ParticleSystem.MinMaxCurve(0, 360 * Mathf.Deg2Rad);
        main.maxParticles = 50;

        // Emission
        var emission = ps.emission;
        emission.rateOverTime = 30;

        // Shape - Cono hacia arriba
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 10f;
        shape.radius = 0.1f;

        // Color over lifetime - fuego realista
        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;

        Gradient fireGradient = new Gradient();
        fireGradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(new Color(1f, 1f, 0.8f), 0.0f),    // Blanco caliente
                new GradientColorKey(new Color(1f, 0.6f, 0f), 0.3f),    // Naranja
                new GradientColorKey(new Color(1f, 0.2f, 0f), 0.7f),    // Rojo
                new GradientColorKey(new Color(0.3f, 0.1f, 0f), 1.0f)   // Rojo oscuro
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1f, 0.0f),
                new GradientAlphaKey(0.8f, 0.5f),
                new GradientAlphaKey(0.0f, 1.0f)
            }
        );

        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(fireGradient);

        // Size over lifetime
        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(0.0f, 0.5f);
        sizeCurve.AddKey(0.5f, 1.0f);
        sizeCurve.AddKey(1.0f, 0.2f);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        // Noise - para movimiento realista del fuego
        var noise = ps.noise;
        noise.enabled = true;
        noise.strength = 0.5f;
        noise.frequency = 1f;
        noise.scrollSpeed = 1f;

        // Renderer
        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.material = CreateFireMaterial();

        // Agregar luz parpadeante
        CreateFlickeringLight(parent);
    }

    void CreateFlickeringLight(Transform parent)
    {
        GameObject lightObj = new GameObject("TorchLight");
        lightObj.transform.SetParent(parent);
        lightObj.transform.localPosition = Vector3.zero;

        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = new Color(1f, 0.6f, 0.2f);
        light.intensity = 2f;
        light.range = 3f;
        light.shadows = LightShadows.Soft;

        lightObj.AddComponent<LightFlicker>();
    }

    Material CreateFireMaterial()
    {
        Material mat = new Material(Shader.Find("Particles/Standard Unlit"));
        mat.SetColor("_Color", new Color(1f, 0.8f, 0.5f));
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", new Color(1f, 0.5f, 0f) * 3f);
        return mat;
    }
}

// Script para parpadeo de luz
public class LightFlicker : MonoBehaviour
{
    private Light torchLight;
    private float baseIntensity;

    [SerializeField] private float flickerSpeed = 15f;
    [SerializeField] private float flickerAmount = 0.5f;

    void Start()
    {
        torchLight = GetComponent<Light>();
        baseIntensity = torchLight.intensity;
    }

    void Update()
    {
        float flicker = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f);
        torchLight.intensity = baseIntensity + (flicker * flickerAmount);
    }
}