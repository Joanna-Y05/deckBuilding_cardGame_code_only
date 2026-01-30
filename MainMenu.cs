using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject optionsMenu;

    public void NewGame()
    {
        //this is erase any past save data so u start with a new card collection
        //for now this just starts the game
        Debug.Log("new game started");
       CameraMovementSystem.instance.MainCamFocus();

        mainMenu.SetActive(false);
        optionsMenu.SetActive(false);
    }

    public void LoadGame()
    {
        //this will load past data with games played and card collection
        Debug.Log("game file found and loaded");
    }

    public void Options()
    {
        //this will play an animation to place the options menu panel on top of the main menu
        Debug.Log("option menu opened");
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("game is closing test");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
