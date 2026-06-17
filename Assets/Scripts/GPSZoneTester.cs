using System;
using System.Collections;
using UnityEngine;
using TMPro;
using System.ComponentModel;
using UnityEngine.Android;

public class GPSZoneTester : MonoBehaviour
{
    private static GPSZoneTester instance;
    public static GPSZoneTester Instance
    {
        get { return instance; }
    }
    [Header("Ziel-Koordinaten")]
    [Description("Trage hier die Koordinaten ein, die du in der Fake-GPS-App ansteuerst.")]

    public double targetLatitude = 49.78828;
    public double targetLongitude = 9.92385;
    public float triggerDistance = 10f;

    [Header("Zuweisungen aus der Szene")]
    public GameObject objectToSpawn;
    public TextMeshProUGUI coordinatesText;

    public TextMeshProUGUI locationText;
    public TextMeshProUGUI routeText;
    public TextMeshProUGUI statusText;

    public GameObject startButton;

    private bool hasSpawned = false;
    private bool isGPSRunning = false;

    public double savedLatitude;
    public double savedLongitude;

    public double CurrentLatitude
    {
        get { return Input.location.lastData.latitude; }
    }

    public double CurrentLongitude
    {
        get { return Input.location.lastData.longitude; }
    }

    private void Awake()
    {
        Debug.Log("GPSZoneTester Awake auf: " + gameObject.name);

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (coordinatesText != null) coordinatesText.text = "Initialisiere GPS...";
        StartCoroutine(StartGPSService());
    }

    void Update()
    {
        if (!isGPSRunning) return;

        double currentLat = Input.location.lastData.latitude;
        double currentLon = Input.location.lastData.longitude;

        savedLatitude = currentLat;
        savedLongitude = currentLon;

        Debug.Log("Aktuelles GPS: " + currentLat + " / " + currentLon);

        double distance = CalculateDistance(currentLat, currentLon, targetLatitude, targetLongitude);

        UpdateUI(currentLat, currentLon, distance);

        if (distance <= triggerDistance && !hasSpawned)
        {
            SpawnCubeAtPlayer();
        }
    }

    private IEnumerator StartGPSService()
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            if (coordinatesText != null)
                coordinatesText.text = "Fordere GPS-Berechtigung an...";

            Permission.RequestUserPermission(Permission.FineLocation);
            yield return new WaitForSeconds(2f);
        }
#endif

        if (!Input.location.isEnabledByUser)
        {
            if (coordinatesText != null)
                coordinatesText.text = "Fehler: GPS am Handy ist AUS!";

            yield break;
        }

        Input.location.Start(1f, 1f);

        int maxWait = 20;

        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait < 1 || Input.location.status == LocationServiceStatus.Failed)
        {
            if (coordinatesText != null)
                coordinatesText.text = "Fehler: GPS-Start fehlgeschlagen.";

            yield break;
        }

        isGPSRunning = true;
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        double R = 6371000;

        double dLat = (Math.PI / 180) * (lat2 - lat1);
        double dLon = (Math.PI / 180) * (lon2 - lon1);

        double a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos((Math.PI / 180) * lat1) *
            Math.Cos((Math.PI / 180) * lat2) *
            Math.Sin(dLon / 2) *
            Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c;
    }

    private void UpdateUI(double lat, double lon, double dist)
    {
        if (locationText != null)
        {
            locationText.text = "Standort gefunden!";
        }

        if (routeText != null)
        {
            routeText.text = "Festungs-Chroniken";
        }

        if (dist <= 500)
        {
            if (statusText != null)
            {
                statusText.text =
                    $"Entfernung zum Startpunkt: {dist:F0} m\n\nDu befindest dich in Reichweite.";
            }

            if (startButton != null)
            {
                startButton.SetActive(true);
            }
        }
        else
        {
            if (statusText != null)
            {
                statusText.text =
                    $"Entfernung zum Startpunkt: {dist:F0} m\n\nDu bist noch zu weit entfernt.\nBewege dich zum Startpunkt:\n{targetLatitude:F6}, {targetLongitude:F6}";
            }

            if (startButton != null)
            {
                startButton.SetActive(false);
            }
        }

        if (coordinatesText != null)
        {
            coordinatesText.text = $"GPS: {lat:F5}, {lon:F5}";
        }
    }

    private void SpawnCubeAtPlayer()
    {
        hasSpawned = true;

        if (objectToSpawn != null)
        {
            Vector3 spawnPosition = Camera.main.transform.position + Camera.main.transform.forward * 3f;
            Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
        }
    }
}
