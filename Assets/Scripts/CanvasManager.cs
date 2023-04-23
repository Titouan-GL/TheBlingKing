using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class CanvasManager : MonoBehaviour
{
    public TMP_Text blingLeftTxt;
    public TMP_Text movesLeftTxt;
    public GridManager gm;
    public GameObject pause;
    public GameObject lose;
    public GameObject win;
    public static bool gamePaused = false;
    public static int levelAvailable = 12;

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0){
            movesLeftTxt.text = "Moves Left : " + gm.movesLeft;
            blingLeftTxt.text = "Bling Left : " + gm.blingLeft;
            if(Input.GetKeyDown(KeyCode.Escape) && lose.activeSelf == false && win.activeSelf == false){
                if(gamePaused){
                    Resume();
                }
                else{
                    Pause();
                }
            }
        }
    }

    public void Resume(){
        pause.SetActive(false);
        gamePaused = false;
    }
    public void Pause(){
        pause.SetActive(true);
        gamePaused = true;
    }

    public void Win(){
        win.SetActive(true);
        levelAvailable = Math.Max(SceneManager.GetActiveScene().buildIndex +1, levelAvailable);
    }

    public void Lose(){
        lose.SetActive(true);
    }

    public void Reload(){
        string currentSceneName = SceneManager.GetActiveScene().name;
        gamePaused = false;
        SceneManager.LoadScene(currentSceneName);
    }
    public void LoadNextScene()
    {
        gamePaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void ToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Quit(){
        Debug.Log("Game Quit");
        Application.Quit();
    }


}
