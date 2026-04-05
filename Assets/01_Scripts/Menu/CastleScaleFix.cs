// CastleScaleFix.cs — Adjunta al GameObject CastleModel
//
// PROBLEMA: CastleModel está dentro de Content → MenuContainer
// que tiene escala heredada muy pequeña del sistema Vuforia/AR.
// Si usamos SetParent(null) rompemos el tracking de Vuforia.
//
// SOLUCIÓN: Compensar la escala heredada del padre directamente,
// sin salir de la jerarquía.

using UnityEngine;

public class CastleScaleFix : MonoBehaviour
{
    [Header("Tamaño deseado del castillo (metros en el mundo AR)")]
    [Tooltip("Empieza con 0.05 y ajusta. 0.05=5cm, 0.10=10cm, 0.15=15cm")]
    public float targetWorldScale = 0.05f;

    [Header("Offset de posición local")]
    public Vector3 localPositionOffset = Vector3.zero;

    void Start()
    {
        ApplyScale();
    }

    public void ApplyScale()
    {
        // Calcular la escala world acumulada del padre
        Vector3 parentWorldScale = transform.parent != null
            ? transform.parent.lossyScale
            : Vector3.one;

        // Compensar: si el padre tiene escala 0.001, necesitamos local = target / 0.001
        float compensatedX = parentWorldScale.x > 0.00001f ? targetWorldScale / parentWorldScale.x : targetWorldScale;
        float compensatedY = parentWorldScale.y > 0.00001f ? targetWorldScale / parentWorldScale.y : targetWorldScale;
        float compensatedZ = parentWorldScale.z > 0.00001f ? targetWorldScale / parentWorldScale.z : targetWorldScale;

        transform.localScale = new Vector3(compensatedX, compensatedY, compensatedZ);
        transform.localPosition = localPositionOffset;

        Debug.Log($"[CastleScaleFix] Escala padre world: {parentWorldScale} → Local aplicada: {transform.localScale} → World resultante: {transform.lossyScale}");
    }

    // Llama esto desde VuforiaPlaneDetection cuando detecte el plano,
    // por si el castillo necesita reposicionarse.
    public void PlaceOnPlane(Vector3 planeWorldPosition)
    {
        // Convertir posición world a local respecto al padre
        if (transform.parent != null)
            transform.localPosition = transform.parent.InverseTransformPoint(planeWorldPosition) + localPositionOffset;
        else
            transform.position = planeWorldPosition + localPositionOffset;
    }

#if UNITY_EDITOR
    // Botón en el Inspector para previsualizar el resultado en Editor
    [ContextMenu("Aplicar escala ahora (Editor)")]
    void ApplyInEditor() => ApplyScale();
#endif
}