using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Button = UnityEngine.UI.Button;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    [SerializeField]
    public PlayerCtrl player;

    [SerializeField]
    GameObject losePanel;

    [SerializeField]
    GameObject winPanel;

    [SerializeField]
    BSPGenerator BSPGen;


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
            if(losePanel != null)
            {
                if(losePanel.activeSelf == true)
                {
                    losePanel.SetActive(false);
                }
            }

            if(winPanel != null)
            {
                if(winPanel.activeSelf == true)
                {
                    winPanel.SetActive(false);
                }
            }
            ReloadScene();
        }
        // Reload the current scene when 'R' is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(losePanel != null)
            {
                if(losePanel.activeSelf == true)
                {
                    losePanel.SetActive(false);
                }
            }

            if(winPanel != null)
            {
                if(winPanel.activeSelf == true)
                {
                    winPanel.SetActive(false);
                }
            }
            GoBack();
        }
        if(player != null)
        {
            Debug.Log(player.currentHealth);
            if(player.currentHealth <= 0)
            {
                losePanel.SetActive(true);

            }
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
            Button statsButton = GameObject.Find("StatsButton")?.GetComponent<Button>();
            Button creditsButton = GameObject.Find("CreditsButton")?.GetComponent<Button>();

            if (playButton != null)
                playButton.onClick.AddListener(Play);

            if (instructionsButton != null)
                instructionsButton.onClick.AddListener(ShowInstructions);
            
            if (statsButton != null)
                statsButton.onClick.AddListener(ShowStats);

            if (creditsButton != null)
                creditsButton.onClick.AddListener(ShowCredits);

            Debug.Log("Buttons reassigned successfully.");
        }
        if(scene.name == "SampleScene")
        {
            player = GameObject.Find("Square")?.GetComponent<PlayerCtrl>();

            if (losePanel == null)
            {
                losePanel = FindInactiveObjectByName("LoseScreen");
                if (losePanel != null)
                {
                    Debug.Log("LoseScreen found!");
                    losePanel.SetActive(false);
                }
            }
            if (winPanel == null)
            {
                winPanel = FindInactiveObjectByName("WinScreen");
                if (winPanel != null)
                {
                    Debug.Log("WinScreen found!");
                    winPanel.SetActive(false);
                }
            }

            BSPGen = GameObject.Find("BPSGenerator")?.GetComponent<BSPGenerator>();
            BSPGen.GenerateMaze();
        }
        
    }

    private GameObject FindInactiveObjectByName(string name)
    {
        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            if (go.hideFlags == HideFlags.None && go.name == name)
            {
                return go;
            }
        }
        return null;
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
    
    public void ShowStats()
    {
        ChangeScene("Stats");
        Debug.Log("on to stats");
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
