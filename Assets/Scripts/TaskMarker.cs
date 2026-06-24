using UnityEngine;

public class TaskMarker : MonoBehaviour
{
    public string zielName;

    public GameOverlayManager overlayManager;

    public void MarkerAngeklickt()
    {
        overlayManager.MarkerGeklickt(this); 
    }
}