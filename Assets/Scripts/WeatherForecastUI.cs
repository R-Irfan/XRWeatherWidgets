using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro; // TextMeshPro namespace
using Newtonsoft.Json.Linq; // Install Newtonsoft.Json from Unity Package Manager

public class WeatherForecastUI : MonoBehaviour
{
    [Header("OpenWeatherMap Settings")]
    public string apiKey = "YOUR_API_KEY"; // Replace with your API key
    public string cityName = "London"; // Replace with the desired city
    public string units = "metric"; // Options: "metric" or "imperial"

    [Header("UI Elements")]
    public TextMeshProUGUI forecastText; // TextMeshPro component to display forecast

    private const string geoUrl = "https://api.openweathermap.org/data/2.5/weather";
    private const string forecastUrl = "https://api.openweathermap.org/data/2.5/onecall";

    private void Start()
    {
        StartCoroutine(FetchCityCoordinates());
    }

    private IEnumerator FetchCityCoordinates()
    {
        string url = $"{geoUrl}?q={cityName}&appid={apiKey}";
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error fetching city data: {request.error}");
            forecastText.text = "Failed to load city data.";
        }
        else
        {
            JObject cityData = JObject.Parse(request.downloadHandler.text);

            if (cityData["cod"].ToString() == "404")
            {
                forecastText.text = "City not found.";
                yield break;
            }

            // Extract latitude and longitude
            float lat = cityData["coord"]["lat"].ToObject<float>();
            float lon = cityData["coord"]["lon"].ToObject<float>();

            StartCoroutine(FetchWeatherForecast(lat, lon));
        }
    }

    private IEnumerator FetchWeatherForecast(float lat, float lon)
    {
        string url = $"{forecastUrl}?lat={lat}&lon={lon}&exclude=current,minutely,hourly,alerts&units={units}&appid={apiKey}";
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error fetching weather data: {request.error}");
            forecastText.text = "Failed to load weather data.";
        }
        else
        {
            ParseAndDisplayForecast(request.downloadHandler.text);
        }
    }

    private void ParseAndDisplayForecast(string jsonData)
    {
        try
        {
            JObject weatherData = JObject.Parse(jsonData);
            JArray dailyForecasts = (JArray)weatherData["daily"];

            string forecastString = $"7-Day Weather Forecast for {cityName}:\n\n";

            foreach (JToken day in dailyForecasts)
            {
                // Extract date, temperature, and description
                long unixTime = day["dt"].ToObject<long>();
                string date = UnixTimeToDate(unixTime);

                float tempDay = day["temp"]["day"].ToObject<float>();
                string description = day["weather"][0]["description"].ToString();

                forecastString += $"{date}: {tempDay}°C, {description}\n";
            }

            forecastText.text = forecastString;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error parsing weather data: {ex.Message}");
            forecastText.text = "Error displaying weather data.";
        }
    }

    private string UnixTimeToDate(long unixTime)
    {
        System.DateTime dateTime = System.DateTimeOffset.FromUnixTimeSeconds(unixTime).DateTime;
        return dateTime.ToString("ddd, MMM dd"); // e.g., "Mon, Jan 01"
    }
}
