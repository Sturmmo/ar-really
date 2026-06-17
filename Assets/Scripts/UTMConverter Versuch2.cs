using UnityEngine;
using System; // <--- Wichtig f�r die pr�zisen Math-Klassen (Double)

public class Wgs84ToUtm32Converter : MonoBehaviour
{
    [Header("WGS84 Eingabe (Dezimalgrad)")]

    public double latitude;
    public double longitude;



    [Header("UTM-Ergebnis (Schreibgesch�tzt im Playmode)")]
    public double positionX;
    public double positionZ;

    [Header("Koordinaten-Versatz (Offset)")]
    [Tooltip("Dieser Wert wird von positionX abgezogen")]
    public double versatzX = 0.0;
    [Tooltip("Dieser Wert wird von positionZ abgezogen")]
    public double versatzZ = 0.0;

    public GameObject outOfBoundsPanel;

    void Start()
    {
        latitude = GPSZoneTester.Instance.savedLatitude;
        longitude = GPSZoneTester.Instance.savedLongitude;

        Debug.Log("GPS übernommen: " + latitude + " / " + longitude);

        // 1. Umrechnung von WGS84 in UTM Zone 32N
        ConvertWgs84ToUtm32(latitude, longitude, out double utmX, out double utmZ);

        // 2. Werte in den Variablen speichern
        positionX = utmX;
        positionZ = utmZ;

        // 3. Versatz abziehen für die finale Unity-Position
        float unityX = (float)(positionX - versatzX);
        float unityZ = (float)(positionZ - versatzZ);

        if (unityX < 0 || unityX > 1000 ||
            unityZ < 0 || unityZ > 1000)
        {
            outOfBoundsPanel.SetActive(true);
        }
        else
        {
            outOfBoundsPanel.SetActive(false);
        }

        // 4. Objekt transformieren
        transform.position = new Vector3(unityX, transform.position.y, unityZ);

        Debug.Log($"[UTM] Objekt positioniert bei X: {unityX} | Z: {unityZ} (Original UTM: E {positionX:F2}, N {positionZ:F2})");
    }



    /// <summary>
    /// Berechnet UTM Zone 32N (WGS84) Koordinaten aus Breitengrad und L�ngengrad.
    /// </summary>
    private void ConvertWgs84ToUtm32(double lat, double lon, out double utmX, out double utmZ)
    {
        // Konstanten f�r das WGS84 Ellipsoid
        const double a = 6378137.0; // Gro�e Halbachse
        const double f = 1.0 / 298.257223563; // Abplattung
        const double k0 = 0.9996; // Skalierungsfaktor f�r UTM

        // Zone 32 Zentralmeridian ist 9� Ost
        const double lonOrigin = 9.0;
        const double falseEasting = 500000.0;
        const double falseNorthing = 0.0;

        // Umrechnung in Bogenma� (Nutzt nun System.Math.PI statt Mathf)
        double latRad = lat * (Math.PI / 180.0);
        double lonRad = lon * (Math.PI / 180.0);
        double lonOriginRad = lonOrigin * (Math.PI / 180.0);

        double eSq = f * (2.0 - f);
        double ePrimeSq = eSq / (1.0 - eSq);

        double n = a / Math.Sqrt(1.0 - eSq * Math.Sin(latRad) * Math.Sin(latRad));
        double tLat = Math.Tan(latRad);
        double tSq = tLat * tLat;
        double c = ePrimeSq * Math.Cos(latRad) * Math.Cos(latRad);
        double aCoeff = (lonRad - lonOriginRad) * Math.Cos(latRad);

        // Berechnung der Meridianbogenl�nge (M)
        double m = a * ((1.0 - eSq / 4.0 - 3.0 * eSq * eSq / 64.0 - 5.0 * eSq * eSq * eSq / 256.0) * latRad
                   - (3.0 * eSq / 8.0 + 3.0 * eSq * eSq / 32.0 + 45.0 * eSq * eSq * eSq / 1024.0) * Math.Sin(2.0 * latRad)
                   + (15.0 * eSq * eSq / 256.0 + 45.0 * eSq * eSq * eSq / 1024.0) * Math.Sin(4.0 * latRad)
                   - (35.0 * eSq * eSq * eSq / 3072.0) * Math.Sin(6.0 * latRad));

        // UTM Easting (X)
        utmX = falseEasting + k0 * n * (aCoeff + (1.0 - tSq + c) * aCoeff * aCoeff * aCoeff / 6.0
               + (5.0 - 18.0 * tSq + tSq * tSq + 72.0 * c - 58.0 * ePrimeSq) * aCoeff * aCoeff * aCoeff * aCoeff * aCoeff / 120.0);

        // UTM Northing (Z)
        utmZ = falseNorthing + k0 * (m + n * tLat * (aCoeff * aCoeff / 2.0
               + (5.0 - tSq + 9.0 * c + 4.0 * c * c) * aCoeff * aCoeff * aCoeff * aCoeff / 24.0
               + (61.0 - 58.0 * tSq + tSq * tSq + 600.0 * c - 330.0 * ePrimeSq) * aCoeff * aCoeff * aCoeff * aCoeff * aCoeff * aCoeff / 720.0));
    }
}