using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadOverviewScene()
    {
        SceneManager.LoadScene("OverviewScene");
    }

    public void LoadScanScene()
    {
        SceneManager.LoadScene("ScanScene");
    }

    public void LoadStartScene()
    {
        SceneManager.LoadScene("start");
    }
}