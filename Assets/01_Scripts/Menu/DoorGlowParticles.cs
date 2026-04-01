// Script: DoorGlowParticles.cs

using UnityEngine;

public class DoorGlowParticles : MonoBehaviour
{
    private ParticleSystem doorGlowPS;
    private Light doorLight;

    [SerializeField] private Transform doorPosition; // Asigna la posición de la puerta del castillo

    void Start()
    {
        if (doorPosition == null)
        {
            GameObject doorObj = new GameObject("DoorPosition");
            doorObj.transform.SetParent(this.transform);
            doorObj.transform.localPosition = new Vector3(0, 0.5f, 1.5f); // Frente del castillo
            doorPosition = doorObj.transform;
        }

        CreateDoorGlow();
    }

    void CreateDoorGlow()
    {
        GameObject glowObj = new GameObject("ParticleSystem_DoorGlow");
        glowObj.transform.SetParent(doorPosition);
        glowObj.transform.localPosition = Vector3.zero;

        doorGlowPS = glowObj.AddComponent<ParticleSystem>();

        var main = doorGlowPS.main;
        main.startLifetime = 1f;
        main.startSpeed = 0.5f;
        main.startSize = new ParticleSystem.MinMaxCurve(0.3f, 0.6f);
        main.maxParticles = 50;
        main.loop = true;

        // Emission - muy bajo inicialmente
        var emission = doorGlowPS.emission;
        emission.rateOverTime = 5;

        // Shape - esfera en la puerta
        var shape = doorGlowPS.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.5f;

        // Color - resplandor mágico intenso
        var colorOverLifetime = doorGlowPS.colorOverLifetime;
        colorOverLifetime.enabled = true;

        Gradient glowGradient = new Gradient();
        glowGradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(new Color(0f, 1f, 1f), 0.0f),      // Cian
                new GradientColorKey(new Color(0.5f, 0.5f, 1f), 0.5f),  // Azul claro
                new GradientColorKey(new Color(1f, 1f, 1f), 1.0f)       // Blanco
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1f, 0.0f),
                new GradientAlphaKey(0.8f, 0.5f),
                new GradientAlphaKey(0f, 1.0f)
            }
        );

        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(glowGradient);

        // Renderer
        var renderer = doorGlowPS.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.material = CreateDoorGlowMaterial();

        // Luz de la puerta
        CreateDoorLight();

        doorGlowPS.Stop(); // Iniciar detenido, se activará al presionar JUGAR
    }

    void CreateDoorLight()
    {
        GameObject lightObj = new GameObject("DoorLight");
        lightObj.transform.SetParent(doorPosition);
        lightObj.transform.localPosition = Vector3.zero;

        doorLight = lightObj.AddComponent<Light>();
        doorLight.type = LightType.Point;
        doorLight.color = new Color(0f, 1f, 1f);
        doorLight.intensity = 0f; // Inicia apagada
        doorLight.range = 5f;
        doorLight.enabled = false;
    }

    Material CreateDoorGlowMaterial()
    {
        Material mat = new Material(Shader.Find("Particles/Standard Unlit"));
        mat.SetColor("_Color", Color.white);
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", new Color(0f, 1f, 1f) * 10f);
        return mat;
    }

    public void ActivateDoorGlow()
    {
        doorGlowPS.Play();

        // Aumentar emisión dramáticamente
        var emission = doorGlowPS.emission;
        emission.rateOverTime = 100;

        // Activar luz
        doorLight.enabled = true;
        StartCoroutine(IncreaseDoorLight());
    }

    System.Collections.IEnumerator IncreaseDoorLight()
    {
        float duration = 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            doorLight.intensity = Mathf.Lerp(0f, 8f, elapsed / duration);
            yield return null;
        }
    }
}