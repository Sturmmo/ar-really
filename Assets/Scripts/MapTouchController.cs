using TMPro;
using UnityEngine;

public class MapTouchController : MonoBehaviour
{
    public TMP_Text debugText;

    void Update()
    {
        debugText.text = "Touches: " + Input.touchCount;
    }
}