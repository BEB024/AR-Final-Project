using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadMarkerless()
    {
        SceneManager.LoadScene("AR_Markerless");
    }

    public void LoadMarkerBased()
    {
        SceneManager.LoadScene("AR_Marker");
    }

    public void QuitApp()
    {
        Application.Quit();
    }
}