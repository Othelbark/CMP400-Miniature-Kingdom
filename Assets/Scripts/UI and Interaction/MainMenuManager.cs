using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    public GameObject menu;
    public GameObject credits;
    public GameObject howToPlay;

    public string gameSceneName;

    // Start is called before the first frame update
    void Start()
    {
        menu.SetActive(true);
        credits.SetActive(false);
        howToPlay.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Menu()
    {
        menu.SetActive(true);
        credits.SetActive(false);
        howToPlay.SetActive(false);
    }

    public void Credits()
    {
        menu.SetActive(false);
        credits.SetActive(true);
        howToPlay.SetActive(false);
    }

    public void HowToPlay()
    {
        menu.SetActive(false);
        credits.SetActive(false);
        howToPlay.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
    }
}
