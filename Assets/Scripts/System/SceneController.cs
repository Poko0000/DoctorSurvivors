using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance {get; private set;}

    void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScenece(SceneData sceneData)
    {
        StartCoroutine(LoadSceneCoroutine(sceneData));
    }

    private IEnumerator LoadSceneCoroutine(SceneData sceneData)
    {
        string sceneName = sceneData.sceneName;
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            Debug.Log($"Loading: {operation.progress}");
            yield return null;
        }
    }
}
