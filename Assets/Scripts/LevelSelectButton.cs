using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelSelectButton : MonoBehaviour
{
    public TMP_Text levelNumber;
    public int level;

    // Update is called once per frame
    void Start()
    {
        ColorBlock colorBlock = GetComponent<Button>().colors;
        if(level <= CanvasManager.levelAvailable){
            colorBlock.normalColor = new Color(0, 0, 0, 0.4f);
            colorBlock.highlightedColor = new Color(0, 0, 0, 0.6f);
            colorBlock.pressedColor = new Color(0, 0, 0, 0.8f);
        }
        else{
            colorBlock.normalColor = new Color(0, 0, 0, 0.2f);
            colorBlock.highlightedColor = new Color(0, 0, 0, 0.2f);
            colorBlock.pressedColor = new Color(0, 0, 0, 0.2f);
        }
        levelNumber.text = ""+level;
        GetComponent<Button>().colors = colorBlock;
    }
    public void LoadLevel()
    {
        if(level <= CanvasManager.levelAvailable){
            CanvasManager.gamePaused = false;
            SceneManager.LoadScene(level);
        }
    }


}
