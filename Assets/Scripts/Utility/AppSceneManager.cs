using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils;

public class AppSceneManager : MonoBehaviour
{
    public static AppSceneManager Instance { get; private set; }

    [Header("Scene Names")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string arGameSceneName = "AR_Game";

    [Header("Optional Loading UI")]
    [SerializeField] private GameObject loadingPanel;

    private bool isLoading;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (loadingPanel != null)
            loadingPanel.SetActive(false);
    }

    public void LoadMainMenu()
    {
        if (isLoading)
            return;

        StartCoroutine(LoadSceneRoutine(mainMenuSceneName));
    }

    public void LoadGame()
    {
        if (isLoading)
            return;

        StartCoroutine(LoadSceneRoutine(arGameSceneName));
    }

    public void RestartGame()
    {
        if (isLoading)
            return;

        StartCoroutine(LoadSceneRoutine(arGameSceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        isLoading = true;

        if (loadingPanel != null)
            loadingPanel.SetActive(true);

        CleanupCurrentSceneBeforeUnload();

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        operation.allowSceneActivation = true;

        while (!operation.isDone)
            yield return null;

        yield return null;

        Scene loadedScene = SceneManager.GetSceneByName(sceneName);

        if (loadedScene.IsValid())
            SceneManager.SetActiveScene(loadedScene);

        yield return null;

        if (sceneName == arGameSceneName)
            ResetARSceneAfterLoad();

        if (loadingPanel != null)
            loadingPanel.SetActive(false);

        isLoading = false;
    }

    private void CleanupCurrentSceneBeforeUnload()
    {
        BallSpawnManager ballSpawnManager = FindFirstObjectByType<BallSpawnManager>();

        if (ballSpawnManager != null)
            ballSpawnManager.ClearExistingBallImmediate();

        HoopManager hoopManager = FindFirstObjectByType<HoopManager>();

        if (hoopManager != null)
            hoopManager.ClearHoop();

        ARImageTracker imageTracker = FindFirstObjectByType<ARImageTracker>();

        if (imageTracker != null)
            imageTracker.ClearSpawnedContent();
    }

    private void ResetARSceneAfterLoad()
    {
        ARSession arSession = FindFirstObjectByType<ARSession>();

        if (arSession != null)
        {
            arSession.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            arSession.transform.localScale = Vector3.one;
            arSession.Reset();
        }

        XROrigin xrOrigin = FindFirstObjectByType<XROrigin>();

        if (xrOrigin != null)
        {
            xrOrigin.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            xrOrigin.transform.localScale = Vector3.one;
        }

        Camera mainCamera = Camera.main;

        if (mainCamera != null)
        {
            mainCamera.transform.localScale = Vector3.one;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}