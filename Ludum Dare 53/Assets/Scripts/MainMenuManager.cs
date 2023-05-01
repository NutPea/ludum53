using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public static bool gameStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        gameStarted = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayGame() {
        gameStarted = true;
    }

    public void QuitGame() {
        Application.Quit();
    }
}
