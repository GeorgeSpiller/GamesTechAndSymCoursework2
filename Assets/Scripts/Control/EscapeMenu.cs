using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeMenu : MonoBehaviour
{
    bool paused = false;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) 
        {
            paused = togglePause();
            if (paused) 
            {
               GetComponent<CameraController>().camMovmentIsEnabled = false;
               GetComponentInChildren<HandelGunFire>().isEnabled = false;
            } else 
            {
               GetComponent<CameraController>().camMovmentIsEnabled = true;
               GetComponentInChildren<HandelGunFire>().isEnabled = true;
            }
        }  
    }

    void OnGUI()
    {
        if(paused)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            GUILayout.Label("Game paused.");

            float button1_width = 100f;
            if (GUI.Button(new Rect(Screen.width/2, (Screen.height/4) - button1_width/2, button1_width, 30), "Restart Level"))
            {
                Scene currentLevel = SceneManager.GetActiveScene(); 
                SceneManager.LoadScene(currentLevel.name);
                paused = togglePause();

                //  SceneManager.LoadScene (sceneName:"LevelSelect");
            }

            float button2_width = 100f;
            if (GUI.Button(new Rect(Screen.width/2, ((Screen.height/4) * 2) - button2_width/2, button2_width, 30), "LevelSelect"))
            {
                SceneManager.LoadScene (sceneName:"LevelSelect");
                paused = togglePause();
            }
        }
    }

    bool togglePause()
    {
        if(Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
            return(false);
        }
        else
        {
            Time.timeScale = 0f;
            return(true);    
        }
    }
}
