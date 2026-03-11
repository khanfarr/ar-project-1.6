using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class TONumFlight : MonoBehaviour
{
    [SerializeField] private TMP_Text textTarget;
    [SerializeField, Min(15f)] private float refreshSeconds = 60f;

    private Coroutine refreshRoutine;
    private string lastSuccessfulDisplay = string.Empty;

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

        using UnityWebRequest request = UnityWebRequest.Get("https://opensky-network.org/api/states/all?lamin=43.30&lomin=-79.90&lamax=44.10&lomax=-78.90");
        request.timeout = 10;

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            ApplyFailureState();
            yield break;
        }

        int flightsNearby = CountTopLevelArrayItems(request.downloadHandler.text, "states");
        if (flightsNearby < 0)
        {
            ApplyFailureState();
            yield break;
        }

        string display = $"Flights in the sky: {flightsNearby}";
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

    private static int CountTopLevelArrayItems(string json, string arrayName)
    {
        string key = $"\"{arrayName}\"";
        int keyIndex = json.IndexOf(key, StringComparison.Ordinal);
        if (keyIndex < 0)
        {
            return -1;
        }

        int arrayStart = json.IndexOf('[', keyIndex);
        if (arrayStart < 0)
        {
            return -1;
        }

        int depth = 1;
        bool inString = false;
        bool escaped = false;
        bool hasItemContent = false;
        int count = 0;

        for (int i = arrayStart + 1; i < json.Length; i++)
        {
            char c = json[i];

            if (inString)
            {
                hasItemContent = true;

                if (escaped)
                {
                    escaped = false;
                    continue;
                }

                if (c == '\\')
                {
                    escaped = true;
                    continue;
                }

                if (c == '"')
                {
                    inString = false;
                }

                continue;
            }

            if (c == '"')
            {
                inString = true;
                hasItemContent = true;
                continue;
            }

            if (c == '[' || c == '{')
            {
                if (depth == 1)
                {
                    hasItemContent = true;
                }

                depth++;
                continue;
            }

            if (c == ']' || c == '}')
            {
                depth--;

                if (depth == 0)
                {
                    if (hasItemContent)
                    {
                        count++;
                    }

                    return count;
                }

                continue;
            }

            if (depth == 1 && c == ',')
            {
                if (hasItemContent)
                {
                    count++;
                    hasItemContent = false;
                }

                continue;
            }

            if (depth == 1 && !char.IsWhiteSpace(c))
            {
                hasItemContent = true;
            }
        }

        return -1;
    }
}
