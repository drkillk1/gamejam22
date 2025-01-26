using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    private void Awake()
    {
        // Ensure that the GameManager persists through scenes
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Reload the current scene when 'R' is pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadScene();
        }
        // Reload the current scene when 'R' is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoBack();
        }
    }

    private void GoBack()
    {
        ChangeScene("StartScreen");
    }

    // Method to change the scene
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Method to reload the current scene
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
