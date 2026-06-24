using UnityEngine;
using TMPro;

public class GameOverlayManager : MonoBehaviour
{
    private TaskMarker aktuellerMarker;
    public TMP_Text debugText;
    public GameObject taskOverlay;
    public TMP_Text taskTitle;

    public TMP_Text distanceText;
    public TMP_Text statusText;

    public GameObject scanButton;

    public void MarkerGeklickt(TaskMarker marker)
    {
        aktuellerMarker = marker;

        debugText.text = "Marker: " + marker.zielName;

        taskTitle.text = marker.zielName;

        taskOverlay.SetActive(true);

        statusText.text = "Bewege dich näher zum Ziel";

        distanceText.text = "Entfernung: -";

        scanButton.SetActive(false);
    }

    public void OverlaySchliessen()
    {
        taskOverlay.SetActive(false);

        debugText.text = "Overlay geschlossen";
    }
}