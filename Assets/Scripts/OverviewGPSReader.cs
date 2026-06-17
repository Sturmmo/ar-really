using UnityEngine;

public class OverviewGPSReader : MonoBehaviour
{
    void Start()
    {
        if (GPSZoneTester.Instance == null)
        {
            Debug.LogError("Kein GPSManager gefunden!");
            return;
        }

        double lat = GPSZoneTester.Instance.savedLatitude;
        double lon = GPSZoneTester.Instance.savedLongitude;

        Debug.Log("Overview GPS:");
        Debug.Log("Latitude: " + lat);
        Debug.Log("Longitude: " + lon);
    }
}