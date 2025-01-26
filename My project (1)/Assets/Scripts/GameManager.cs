using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    private void OnEnable()
    {
        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}");

        if (scene.name == "StartScreen")
        {
            // Reassign the buttons dynamically
            Button playButton = GameObject.Find("PlayButton")?.GetComponent<Button>();
            Button instructionsButton = GameObject.Find("InstructionsButton")?.GetComponent<Button>();
            Button creditsButton = GameObject.Find("CreditsButton")?.GetComponent<Button>();

            if (playButton != null)
                playButton.onClick.AddListener(Play);

            if (instructionsButton != null)
                instructionsButton.onClick.AddListener(ShowInstructions);

            if (creditsButton != null)
                creditsButton.onClick.AddListener(ShowCredits);

            Debug.Log("Buttons reassigned successfully.");
        }
    }

    public void Play()
    {
        ChangeScene("SampleScene");
    }

    public void ShowInstructions()
    {
        Debug.Log("changing");
        ChangeScene("Instructions");
        Debug.Log("changed");
    }
    
    public void ShowCredits()
    {
        ChangeScene("Credits");
    }


    public void GoBack()
    {
        ChangeScene("StartScreen");
    }

    // Method to change the scene
    private void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Method to reload the current scene
    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


}
