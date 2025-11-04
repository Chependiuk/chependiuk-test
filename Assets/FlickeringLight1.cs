using UnityEngine;
using System.Collections;
using System;

public class FlickeringLight1 : MonoBehaviour
{
    public static event Action<Vector3> OnLightFlickered;

    [Tooltip("Посилання на компонент світла (Light).")]
    public Light targetLight;

    [Header("Налаштування мерехтіння")]
    public float minFlickerInterval = 0.05f;
    public float maxFlickerInterval = 0.15f;
    public float minFlickerDuration = 0.02f;
    public float maxFlickerDuration = 0.08f;
    [Range(0f, 1f)]
    public float blackoutChance = 0.3f;
    public float minDimIntensity = 0.1f;
    public float maxDimIntensity = 0.5f;

    private float originalIntensity;
    private Coroutine flickerCoroutine;

    void Start()
    {
        if (targetLight == null) targetLight = GetComponent<Light>();
        if (targetLight == null)
        {
            Debug.LogError("На об'єкті '" + gameObject.name + "' не знайдено компонент Light.", this);
            enabled = false;
            return;
        }
        originalIntensity = targetLight.intensity;
    }

    void OnEnable()
    {
        if (flickerCoroutine == null)
        {
            flickerCoroutine = StartCoroutine(FlickerRoutine());
        }
    }

    void OnDisable()
    {
        if (flickerCoroutine != null)
        {
            StopCoroutine(flickerCoroutine);
            flickerCoroutine = null;
        }
        if (targetLight != null)
        {
            targetLight.intensity = originalIntensity;
            targetLight.enabled = true;
        }
    }

    private IEnumerator FlickerRoutine()
    {
        while (true)
        {
            // --- ВИПРАВЛЕНО ---
            // Прямо вказуємо, що потрібен Random з бібліотеки UnityEngine
            yield return new WaitForSeconds(UnityEngine.Random.Range(minFlickerInterval, maxFlickerInterval));

            // --- ВИПРАВЛЕНО ---
            float flickerIntensity = UnityEngine.Random.value < blackoutChance ? 0f : UnityEngine.Random.Range(minDimIntensity, maxDimIntensity);

            if (targetLight != null)
            {
                targetLight.intensity = flickerIntensity;
                OnLightFlickered?.Invoke(transform.position);
            }

            yield return new WaitForSeconds(UnityEngine.Random.Range(minFlickerDuration, maxFlickerDuration));

            if (targetLight != null)
            {
                targetLight.intensity = originalIntensity;
            }
        }
    }
}