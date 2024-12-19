using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class WeatherDisplay : MonoBehaviour
{
    public string apiKey = "8a958fc0ff314f7ab8a1b077ae3aee70"; // Replace with your OpenWeatherMap API Key
    public string city = "Hyderabad"; // Replace with your desired city
    public TextMeshProUGUI weatherText, cityNameText, tempText, WindText, humidText; // Reference to TextMeshPro UI element

    void Start()
    {
        StartCoroutine(GetWeatherData());
    }

    IEnumerator GetWeatherData()
    {
        string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric";
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error fetching weather data: {request.error}");
            weatherText.text = "Error fetching weather data.";
        }
        else
        {
            ProcessWeatherData(request.downloadHandler.text);
        }
    }

    void ProcessWeatherData(string json)
    {
        WeatherResponse weatherResponse = JsonUtility.FromJson<WeatherResponse>(json);

        string weatherInfo = $"Weather Update for {weatherResponse.name}, {weatherResponse.sys.country}:\n" +
                             $"Temperature: {weatherResponse.main.temp}°C (Feels like {weatherResponse.main.feels_like}°C)\n" +
                             $"Condition: {weatherResponse.weather[0].description}\n" +
                             $"Humidity: {weatherResponse.main.humidity}%\n" +
                             $"Wind Speed: {weatherResponse.wind.speed} m/s";

        weatherText.text = weatherInfo;


        cityNameText.text = weatherResponse.name;
        tempText.text = Math.Round( weatherResponse.main.temp).ToString();
        WindText.text = weatherResponse.wind.speed.ToString() + "m/s";
        humidText.text = weatherResponse.main.humidity.ToString() + "%";



    }

    [Serializable]
    public class WeatherResponse
    {
        public string name;
        public Sys sys;
        public Main main;
        public Wind wind;
        public Weather[] weather;

        [Serializable]
        public class Sys
        {
            public string country;
        }

        [Serializable]
        public class Main
        {
            public float temp;
            public float feels_like;
            public int humidity;
        }

        [Serializable]
        public class Wind
        {
            public float speed;
        }

        [Serializable]
        public class Weather
        {
            public string description;
        }
    }
}
