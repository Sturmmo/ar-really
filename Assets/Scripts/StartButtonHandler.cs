using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButtonHandler : MonoBehaviour
{
    public void LoadOverviewScene()
    {
        SceneManager.LoadScene("OverviewScene");
    }
}