using UnityEngine;

public class CityWeatherController : MonoBehaviour
{
    public Renderer cubeShell;

    public string weather = "clear"; 
    // options: clear, rain, snow

    void Start()
    {
        if (weather == "rain")
        {
            // darker rainy blue
            cubeShell.material.color = new Color(0.35f, 0.45f, 0.9f);
        }
        else if (weather == "snow")
        {
            // snowy white
            cubeShell.material.color = Color.white;
        }
        else
        {
            // nice sky blue for clear weather
            cubeShell.material.color = new Color(0.3f, 0.7f, 1f);
        }
    }
}