using UnityEngine;

public class PlayerMapTracker : MonoBehaviour
{
    [Header("Marker")]
    public RectTransform playerMarker;
    public RectTransform mapImage;

    [Header("Kartenzentrum")]
    public double centerLatitude = 49.78828;
    public double centerLongitude = 9.92385;

    [Header("Kartengröße")]
    public float mapSizeMeters = 1000f;

    void Update()
    {
        if (Input.location.status != LocationServiceStatus.Running)
            return;

        double currentLat = Input.location.lastData.latitude;
        double currentLon = Input.location.lastData.longitude;

        UpdatePlayerPosition(currentLat, currentLon);
    }

    void UpdatePlayerPosition(double lat, double lon)
    {
        float metersPerLat = 111000f;
        float metersPerLon = 71000f;

        float deltaNorth =
            (float)((lat - centerLatitude) * metersPerLat);

        float deltaEast =
            (float)((lon - centerLongitude) * metersPerLon);

        float normalizedX = deltaEast / mapSizeMeters;
        float normalizedY = deltaNorth / mapSizeMeters;

        float posX = normalizedX * mapImage.rect.width;
        float posY = normalizedY * mapImage.rect.height;

        playerMarker.anchoredPosition =
            new Vector2(posX, posY);
    }
}