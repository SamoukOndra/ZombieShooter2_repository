using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPause : MonoBehaviour
{
    bool gamePaused = false;

    private void Awake()
    {
        Cursor.visible = false;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!gamePaused)
            {
                Time.timeScale = 0;
                gamePaused = true;
            }
            else
            {
                Time.timeScale = 1;
                gamePaused = false;
            }
        }
    }
}
