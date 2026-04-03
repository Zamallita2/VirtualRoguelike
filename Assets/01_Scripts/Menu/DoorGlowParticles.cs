using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class DoorGlowParticles : MonoBehaviour
{
    [Header("Posicion de la Puerta")]
    [SerializeField] private Transform doorPosition;

    [Header("Ajuste de Escala")]
    [SerializeField] private float scaleMultiplier = 0.01f;

    [Header("FIX: Textura de partícula")]
    [SerializeField] private Texture2D particleTexture;

    private ParticleSystem glowPS;
    private ParticleSystem sparkPS;
    private ParticleSystem smokePS;
    private Light doorLight;

    void Start()
    {
        if (particleTexture == null)
            particleTexture = FireParticle.BuildSoftCircleTexture();

        if (doorPosition == null)
            AutoCreateDoorAnchor();

        BuildGlowSystem();
        BuildSparkSystem();
        BuildSmokeSystem();
        BuildDoorLight();

        glowPS.Stop();
        sparkPS.Stop();
        smokePS.Stop();
        doorLight.enabled = false;
    }

    void AutoCreateDoorAnchor()
    {
        GameObject anchor = new GameObject("DoorAnchor_Auto");
        anchor.transform.SetParent(this.transform);
        anchor.transform.localPosition = new Vector3(0f, 50f, 120f);
        doorPosition = anchor.transform;
    }

    void BuildGlowSystem()
    {
        GameObject obj = new GameObject("PS_DoorGlow");
        obj.transform.SetParent(doorPosition);
        obj.transform.localPosition = Vector3.zero;

        glowPS = obj.AddComponent<ParticleSystem>();
        var main = glowPS.main;
        main.startLifetime = new ParticleSystem.MinMaxCurve(1.2f, 2.5f); // ✅ Más duración
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.2f * scaleMultiplier, 0.6f * scaleMultiplier); // ✅ Velocidad más baja
        main.startSize = new ParticleSystem.MinMaxCurve(20f * scaleMultiplier, 40f * scaleMultiplier);
        main.maxParticles = 80; // ✅ REDUCIDO de 120 a 80
        main.loop = true;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = 0f; // ✅ Sin gravedad para movimiento natural

        var emission = glowPS.emission;
        emission.rateOverTime = 0f;

        // ✅ CORRECCIÓN: Usar Sphere para que vayan en todas direcciones
        var shape = glowPS.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 10f * scaleMultiplier;
        shape.radiusThickness = 0.8f; // Emitir desde el borde

        // 🎨 DARK: azul hielo apagado → púrpura sombrío → gris humo
        var colorOL = glowPS.colorOverLifetime;
        colorOL.enabled = true;
        Gradient g = new Gradient();
        g.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(new Color(0.0f,  0.35f, 0.55f), 0f),
                new GradientColorKey(new Color(0.18f, 0.04f, 0.38f), 0.4f),
                new GradientColorKey(new Color(0.3f,  0.25f, 0.3f),  0.75f),
                new GradientColorKey(new Color(0.08f, 0.08f, 0.08f), 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0f,    0f),
                new GradientAlphaKey(0.6f,  0.15f), // ✅ Menos opacidad
                new GradientAlphaKey(0.35f, 0.7f),
                new GradientAlphaKey(0f,    1f)
            }
        );
        colorOL.color = new ParticleSystem.MinMaxGradient(g);

        var sizeOL = glowPS.sizeOverLifetime;
        sizeOL.enabled = true;
        AnimationCurve sc = new AnimationCurve();
        sc.AddKey(0f, 0.2f); sc.AddKey(0.4f, 1f); sc.AddKey(1f, 0.3f);
        sizeOL.size = new ParticleSystem.MinMaxCurve(1f, sc);

        // ✅ AÑADIR RUIDO para movimiento orgánico
        var noise = glowPS.noise;
        noise.enabled = true;
        noise.strength = 0.4f * scaleMultiplier;
        noise.frequency = 0.3f;
        noise.scrollSpeed = 0.2f;

        ApplyMaterial(glowPS.GetComponent<ParticleSystemRenderer>(), new Color(0f, 0.28f, 0.5f, 1f));
    }

    void BuildSparkSystem()
    {
        GameObject obj = new GameObject("PS_DoorSparks");
        obj.transform.SetParent(doorPosition);
        obj.transform.localPosition = Vector3.zero;

        sparkPS = obj.AddComponent<ParticleSystem>();
        var main = sparkPS.main;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.8f, 1.8f); // ✅ Más duración
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.3f * scaleMultiplier, 1.0f * scaleMultiplier); // ✅ Menos velocidad
        main.startSize = new ParticleSystem.MinMaxCurve(5f * scaleMultiplier, 12f * scaleMultiplier);
        main.maxParticles = 100; // ✅ REDUCIDO de 200 a 100
        main.loop = true;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = -0.05f; // ✅ Casi sin gravedad

        var emission = sparkPS.emission;
        emission.rateOverTime = 0f;

        // ✅ CORRECCIÓN: Usar Sphere para dispersión en todas direcciones
        var shape = sparkPS.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 8f * scaleMultiplier;
        shape.radiusThickness = 1f;

        // 🎨 DARK: cobre apagado → marrón quemado → negro
        var colorOL = sparkPS.colorOverLifetime;
        colorOL.enabled = true;
        Gradient sg = new Gradient();
        sg.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(new Color(0.75f, 0.38f, 0.05f), 0f),
                new GradientColorKey(new Color(0.5f,  0.15f, 0.0f),  0.3f),
                new GradientColorKey(new Color(0.22f, 0.05f, 0.0f),  0.8f),
                new GradientColorKey(new Color(0.05f, 0.02f, 0.0f),  1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0.7f, 0f), // ✅ Menos opacidad
                new GradientAlphaKey(0.5f, 0.5f),
                new GradientAlphaKey(0f,   1f)
            }
        );
        colorOL.color = new ParticleSystem.MinMaxGradient(sg);

        // ✅ MÁS RUIDO para movimiento caótico
        var noise = sparkPS.noise;
        noise.enabled = true;
        noise.strength = 0.6f * scaleMultiplier;
        noise.frequency = 1.2f;
        noise.scrollSpeed = 0.8f;

        ApplyMaterial(sparkPS.GetComponent<ParticleSystemRenderer>(), new Color(0.55f, 0.18f, 0.0f, 1f));
    }

    void BuildSmokeSystem()
    {
        GameObject obj = new GameObject("PS_MysticSmoke");
        obj.transform.SetParent(doorPosition);
        obj.transform.localPosition = Vector3.zero;

        smokePS = obj.AddComponent<ParticleSystem>();
        var main = smokePS.main;
        main.startLifetime = new ParticleSystem.MinMaxCurve(2f, 4f); // ✅ Más duración
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.05f * scaleMultiplier, 0.25f * scaleMultiplier); // ✅ Muy lento
        main.startSize = new ParticleSystem.MinMaxCurve(30f * scaleMultiplier, 60f * scaleMultiplier);
        main.maxParticles = 40; // ✅ REDUCIDO de 60 a 40
        main.loop = true;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = 0f; // ✅ SIN GRAVEDAD - no va hacia arriba

        var emission = smokePS.emission;
        emission.rateOverTime = 0f;

        // ✅ CORRECCIÓN: Usar Sphere para humo que se expande
        var shape = smokePS.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 6f * scaleMultiplier;
        shape.radiusThickness = 0.5f;

        // 🎨 DARK: humo negro pesado
        var colorOL = smokePS.colorOverLifetime;
        colorOL.enabled = true;
        Gradient smg = new Gradient();
        smg.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(new Color(0.1f,  0.0f,  0.18f), 0f),
                new GradientColorKey(new Color(0.03f, 0.03f, 0.08f), 0.7f),
                new GradientColorKey(Color.black,                     1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0f,    0f),
                new GradientAlphaKey(0.22f, 0.2f), // ✅ Menos opacidad
                new GradientAlphaKey(0.15f, 0.7f),
                new GradientAlphaKey(0f,    1f)
            }
        );
        colorOL.color = new ParticleSystem.MinMaxGradient(smg);

        var sizeOL = smokePS.sizeOverLifetime;
        sizeOL.enabled = true;
        AnimationCurve smc = new AnimationCurve();
        smc.AddKey(0f, 0.5f); smc.AddKey(1f, 1.5f);
        sizeOL.size = new ParticleSystem.MinMaxCurve(1f, smc);

        // ✅ AÑADIR RUIDO para movimiento natural del humo
        var noise = smokePS.noise;
        noise.enabled = true;
        noise.strength = 0.3f * scaleMultiplier;
        noise.frequency = 0.4f;
        noise.scrollSpeed = 0.3f;

        ApplyMaterial(smokePS.GetComponent<ParticleSystemRenderer>(),
            new Color(0.08f, 0f, 0.15f, 0.45f));
    }

    void BuildDoorLight()
    {
        GameObject lightObj = new GameObject("DoorPointLight");
        lightObj.transform.SetParent(doorPosition);
        lightObj.transform.localPosition = Vector3.zero;

        doorLight = lightObj.AddComponent<Light>();
        doorLight.type = LightType.Point;
        doorLight.color = new Color(0.1f, 0.3f, 0.65f); // azul frío
        doorLight.intensity = 0f;
        doorLight.range = 0.5f;
        doorLight.enabled = false;
    }

    public void ActivateDoorGlow()
    {
        StartCoroutine(GlowSequence());
    }

    IEnumerator GlowSequence()
    {
        // ✅ REDUCIR EMISIÓN DE PARTÍCULAS
        var smokeEm = smokePS.emission;
        smokePS.Play();
        smokeEm.rateOverTime = 5f; // ✅ REDUCIDO de 8 a 5

        yield return new WaitForSeconds(0.3f);

        var glowEm = glowPS.emission;
        glowPS.Play();
        glowEm.rateOverTime = 25f; // ✅ REDUCIDO de 40 a 25
        doorLight.enabled = true;

        yield return new WaitForSeconds(0.2f);

        var sparkEm = sparkPS.emission;
        sparkPS.Play();
        sparkEm.rateOverTime = 35f; // ✅ REDUCIDO de 60 a 35

        yield return StartCoroutine(AnimLight(0f, 3f, 1.5f, new Color(0.1f, 0.3f, 0.65f)));
        yield return StartCoroutine(PulseLight(3f, 6f, 0.5f, 3));
        yield return StartCoroutine(AnimLight(doorLight.intensity, 4.5f, 0.8f, new Color(0.5f, 0.28f, 0.08f)));

        sparkEm.rateOverTime = 50f; // ✅ REDUCIDO de 120 a 50
    }

    IEnumerator AnimLight(float from, float to, float dur, Color toCol)
    {
        float t = 0f;
        Color startCol = doorLight.color;
        float startInt = doorLight.intensity;
        while (t < 1f)
        {
            t += Time.deltaTime / dur;
            doorLight.intensity = Mathf.Lerp(startInt, to, t);
            doorLight.color = Color.Lerp(startCol, toCol, t);
            yield return null;
        }
        doorLight.intensity = to;
        doorLight.color = toCol;
    }

    IEnumerator PulseLight(float minI, float maxI, float speed, int pulses)
    {
        for (int i = 0; i < pulses * 2; i++)
        {
            float target = (i % 2 == 0) ? maxI : minI;
            float start = doorLight.intensity;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / speed;
                doorLight.intensity = Mathf.Lerp(start, target, t);
                yield return null;
            }
        }
    }

    void ApplyMaterial(ParticleSystemRenderer rend, Color tint)
    {
        rend.renderMode = ParticleSystemRenderMode.Billboard;
        rend.material = FireParticle.BuildTransparentMaterial(tint);
    }
}