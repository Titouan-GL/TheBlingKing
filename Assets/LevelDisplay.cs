using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDisplay : MonoBehaviour
{
    public LevelSelectButton levelButton;
    public float buttonsize = 100;
    public int width = 10;
    public int height = 2;
    public int totalnumber = 20;
    // Start is called before the first frame update
    void Start()
    {
        for(int nb = 0; nb < totalnumber; nb ++){
            LevelSelectButton lb = Instantiate(levelButton, transform);
            lb.transform.localPosition = new Vector3((nb%width) * buttonsize - ((width-1)/2.0f)*buttonsize, (nb/width) * -buttonsize, 0);
            lb.level = nb+1;
        }
    }

}
