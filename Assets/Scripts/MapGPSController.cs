using UnityEngine;
using UnityEngine.UI;

public class MapGPSController : MonoBehaviour
{
    [Header("Marker")]
    public RectTransform playerMarker;

    [Header("Kartengrenzen (UTM)")]
    public float minX = 566000f;
    public float maxX = 567000f;

    public float minY = 5515000f;
    public float maxY = 5516000f;

    [Header("Karte")]
    public RectTransform mapImage;

    void Start()
    {
        if (GPSZoneTester.Instance != null)
        {
            Debug.Log("GPSManager gefunden");

            Debug.Log("Lat: " + GPSZoneTester.Instance.CurrentLatitude);
            Debug.Log("Lon: " + GPSZoneTester.Instance.CurrentLongitude);
        }
        else
        {
            Debug.Log("GPSManager NICHT gefunden");
        }

        SetPlayerPosition(500f, 500f);
    }

    public void SetPlayerPosition(float utmX, float utmY)
    {
        float normalizedX = utmX / 1000f;
        float normalizedY = utmY / 1000f;

        float mapWidth = mapImage.rect.width;
        float mapHeight = mapImage.rect.height;

        float posX = (normalizedX - 0.5f) * mapWidth;
        float posY = (normalizedY - 0.5f) * mapHeight;

        playerMarker.anchoredPosition = new Vector2(posX, posY);
    }
}