using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour
{
    public TMP_Text blingLeftTxt;
    public TMP_Text movesLeftTxt;
    public GridManager gm;
    public GameObject pause;
    public GameObject lose;
    public GameObject win;
    public static bool gamePaused = false;

    // Update is called once per frame
    void Update()
    {
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit(){
        Debug.Log("Game Quit");
        Application.Quit();
    }

}
