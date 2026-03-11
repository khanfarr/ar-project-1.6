using System;
using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class NYCTimeAPI : MonoBehaviour
{
    [SerializeField] private TMP_Text textTarget;
    [SerializeField, Min(15f)] private float refreshSeconds = 60f;

    private Coroutine refreshRoutine;
    private string lastSuccessfulDisplay = string.Empty;

    [Serializable]
    private class TimeResponse
    {
        public CurrentWeather current_weather;
    }

    [Serializable]
    private class CurrentWeather
    {
        public string time;
    }

    private void Awake()
    {
        if (textTarget == null)
        {
            textTarget = GetComponent<TMP_Text>();
        }
    }

    private void OnEnable()
    {
        if (refreshRoutine == null)
        {
            refreshRoutine = StartCoroutine(RefreshLoop());
        }
    }

    private void OnDisable()
    {
        if (refreshRoutine != null)
        {
            StopCoroutine(refreshRoutine);
            refreshRoutine = null;
        }
    }

    private IEnumerator RefreshLoop()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 1.5f));

        while (enabled)
        {
            yield return StartCoroutine(RefreshOnce());
            yield return new WaitForSeconds(refreshSeconds);
        }
    }

    private IEnumerator RefreshOnce()
    {
        SetText("Loading...");

        using UnityWebRequest request = UnityWebRequest.Get("https://api.open-meteo.com/v1/forecast?latitude=40.7128&longitude=-74.0060&current_weather=true&timezone=America%2FNew_York");
        request.timeout = 10;

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning($"NYCTimeAPI request failed: {request.error}");
            ApplyFailureState();
            yield break;
        }

        TimeResponse response = JsonUtility.FromJson<TimeResponse>(request.downloadHandler.text);
        if (response == null || response.current_weather == null || string.IsNullOrWhiteSpace(response.current_weather.time))
        {
            ApplyFailureState();
            yield break;
        }

        if (!TryParseApiTime(response.current_weather.time, out DateTime parsedTime))
        {
            Debug.LogWarning($"NYCTimeAPI could not parse time: {response.current_weather.time}");
            ApplyFailureState();
            yield break;
        }

        string display = $"{parsedTime:h:mm tt}";
        lastSuccessfulDisplay = display;
        SetText(display);
    }

    private void ApplyFailureState()
    {
        if (!string.IsNullOrEmpty(lastSuccessfulDisplay))
        {
            SetText(lastSuccessfulDisplay);
            return;
        }

        SetText("N/A");
    }

    private void SetText(string value)
    {
        if (textTarget != null)
        {
            textTarget.text = value;
        }
    }

    private static bool TryParseApiTime(string value, out DateTime parsedTime)
    {
        if (DateTime.TryParseExact(
                value,
                "yyyy-MM-dd'T'HH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out parsedTime))
        {
            return true;
        }

        return DateTime.TryParse(value, out parsedTime);
    }
}
