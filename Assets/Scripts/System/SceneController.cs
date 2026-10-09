using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance {get; private set;}
    [SerializeField] PauseEventChannel pauseEvent;
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

    void Start()
    {
        if (pauseEvent == null)
        {
            Debug.LogError("[SceneController] 沒有指定 PauseEventChannel,切換場景時不會清除暫停狀態。");
        }
    }

    public void LoadScene(SceneData sceneData)
    {
        StartCoroutine(LoadSceneCoroutine(sceneData));
    }

    private IEnumerator LoadSceneCoroutine(SceneData sceneData)
    {
        pauseEvent.RaiseClearAll();
        string sceneName = sceneData.sceneName;
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            Debug.Log($"Loading: {operation.progress}");
            yield return null;
        }
    }
}
