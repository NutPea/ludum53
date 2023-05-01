using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{
    public GameObject playButton;
    public GameObject optionsButton;
    public GameObject quitButton;
    public GameObject backButton;
    public GameObject optionsMenu;

    public LeanTweenType inTyp;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartGame()
    {
        HideMenu();
    }

    public void ShowOptions()
    {
        optionsMenu.SetActive(true);
    }

    public void HideOptions()
    {
        optionsMenu.SetActive(false);
    }

    public void ShowMenu()
    {
        LeanTween.moveLocalX(playButton, -100, 1f).setEase(inTyp);
        LeanTween.moveLocalX(optionsButton, -100, 1.5f).setEase(inTyp);
        LeanTween.moveLocalX(quitButton, -100, 2f).setEase(inTyp);
    }

    public void HideMenu()
    {
        LeanTween.moveLocalX(playButton, 100, 1f).setEase(inTyp);
        LeanTween.moveLocalX(optionsButton, 100, 1.5f).setEase(inTyp);
        LeanTween.moveLocalX(quitButton, 100, 2f).setEase(inTyp);
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}