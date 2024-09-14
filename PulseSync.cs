using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Transform))]
public class PulseSync : MonoBehaviour
{
    [Header("Pulse Settings")]
    [Tooltip("Scale factor for the pulse. For example, 1.2 means the object scales to 120% of its original size.")]
    public float pulseScaleFactor = 1.2f;

    [Tooltip("Duration of the pulse up in seconds.")]
    public float pulseUpDuration = 0.1f;

    [Tooltip("Duration of the pulse down in seconds.")]
    public float pulseDownDuration = 0.1f;

    // Original scale of the object
    private Vector3 originalScale;

    void Start()
    {
        // Store the original scale
        originalScale = transform.localScale;

        // Ensure Conductor instance exists
        if (Conductor.instance == null)
        {
            Debug.LogError("PulseSync: Conductor instance not found in the scene.");
            enabled = false;
            return;
        }

        // Subscribe to the OnBeat event from the Conductor
        Conductor.instance.OnBeat += TriggerPulse;
    }

    void OnDestroy()
    {
        // Unsubscribe from the OnBeat event when this object is destroyed
        if (Conductor.instance != null)
        {
            Conductor.instance.OnBeat -= TriggerPulse;
        }
    }

    // Trigger the pulsing effect when a beat occurs
    private void TriggerPulse()
    {
        StartCoroutine(Pulse());
    }

    private IEnumerator Pulse()
    {
        // Pulse Up
        float elapsed = 0f;
        while (elapsed < pulseUpDuration)
        {
            float t = elapsed / pulseUpDuration;
            transform.localScale = Vector3.Lerp(originalScale, originalScale * pulseScaleFactor, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale * pulseScaleFactor;

        // Pulse Down
        elapsed = 0f;
        while (elapsed < pulseDownDuration)
        {
            float t = elapsed / pulseDownDuration;
            transform.localScale = Vector3.Lerp(originalScale * pulseScaleFactor, originalScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
    }

    void OnDisable()
    {
        // Reset scale when the script is disabled
        transform.localScale = originalScale;
    }
}
