using UnityEngine;
using TMPro;

public class DayNightCycle : MonoBehaviour
{
    // --- ДОДАНО: Singleton ---
    public static DayNightCycle Instance { get; private set; }

    [Header("Налаштування часу")]
    public float dayDurationInSeconds = 120f;

    [Header("Посилання на об'єкти")]
    public Light sun;
    public TextMeshProUGUI clockText;

    [Header("Налаштування кольорів")]
    public Gradient skyColor;
    public Gradient ambientColor;

    private float currentTimeOfDay = 0.25f;

    // --- ДОДАНО: Awake() для Singleton ---
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Update()
    {
        UpdateTime();
        UpdateSunPosition();
        UpdateLightingColors();
        UpdateClock();
    }

    // --- ДОДАНО: Метод для отримання часу у вигляді тексту ---
    public string GetCurrentTimeAsString()
    {
        float hours = 24 * currentTimeOfDay;
        float minutes = 60 * (hours - Mathf.Floor(hours));
        return string.Format("{0:00}:{1:00}", (int)hours, (int)minutes);
    }

    private void UpdateTime()
    {
        currentTimeOfDay += Time.deltaTime / dayDurationInSeconds;
        currentTimeOfDay %= 1;
    }

    private void UpdateSunPosition()
    {
        if (sun == null) return;
        sun.transform.rotation = Quaternion.Euler((currentTimeOfDay * 360f) - 90f, 170f, 0);
    }

    private void UpdateLightingColors()
    {
        if (sun != null) sun.color = skyColor.Evaluate(currentTimeOfDay);
        RenderSettings.ambientLight = ambientColor.Evaluate(currentTimeOfDay);
    }



    private void UpdateClock()
    {
        if (clockText == null) return;
        clockText.text = GetCurrentTimeAsString();
    }
}