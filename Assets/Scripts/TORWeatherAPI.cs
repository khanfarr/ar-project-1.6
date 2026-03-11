using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class TOWeatherAPI : MonoBehaviour
{
    [SerializeField] private TMP_Text textTarget;
    [SerializeField, Min(15f)] private float refreshSeconds = 60f;

    private Coroutine refreshRoutine;
    private string lastSuccessfulDisplay = string.Empty;

    [Serializable]
    private class OpenMeteoResponse
    {
        public OpenMeteoCurrent current_weather;
    }

    [Serializable]
    private class OpenMeteoCurrent
    {
        public float temperature;
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

        using UnityWebRequest request = UnityWebRequest.Get("https://api.open-meteo.com/v1/forecast?latitude=43.6532&longitude=-79.3832&current_weather=true&temperature_unit=celsius&windspeed_unit=kmh");
        request.timeout = 10;

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            ApplyFailureState();
            yield break;
        }

        OpenMeteoResponse response = JsonUtility.FromJson<OpenMeteoResponse>(request.downloadHandler.text);
        if (response == null || response.current_weather == null)
        {
            ApplyFailureState();
            yield break;
        }

        string display = $"{response.current_weather.temperature:0.#}°C";
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
}
