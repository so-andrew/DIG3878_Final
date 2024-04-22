using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

    }

    public void SceneChange(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void SceneChange(string name)
    {
        Debug.Log("Loading scene " + name);
        SceneManager.LoadScene(name);
    }

    public void SceneChangeAdditive(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex, LoadSceneMode.Additive);
    }

    public void SceneChangeAdditive(string name)
    {
        SceneManager.LoadScene(name, LoadSceneMode.Additive);
    }
}
