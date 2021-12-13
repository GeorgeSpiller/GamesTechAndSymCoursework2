using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInfoText : MonoBehaviour
{
    [SerializeField]
    public string levelInfoText;
    public bool infoIsActive = true;
    void Awake() 
    {
        GetComponent<CameraController>().camMovmentIsEnabled = false;
        GetComponentInChildren<HandelGunFire>().isEnabled = false;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<EscapeMenu>().enabled = false;
        Time.timeScale = 0f;
    }

    void OnGUI()
    {
        if(infoIsActive)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            GUILayout.Label("Game paused.");

            float button1_width = 100f;
            if (GUI.Button(new Rect(Screen.width/2, (Screen.height/4) - button1_width/2, button1_width, 30), "Continue"))
            {
                infoIsActive = false;
                GetComponent<CameraController>().camMovmentIsEnabled = true;
                GetComponentInChildren<HandelGunFire>().isEnabled = true;
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                player.GetComponent<EscapeMenu>().enabled = true;
                Time.timeScale = 1f;
            }
            GUI.TextArea(new Rect(0, Screen.height - (Screen.height/8), Screen.width, Screen.height), levelInfoText);
        }
    }
/*
For this level you will be facing an AI in naughts and crosses. To complete the level you must beat the AI. 
To move, shoot orange gell at one of the 9 green boxes. Once you have made your move, press 'f' to let the AI play.
*/
/*
For this level you must guid the AI (small red robot) into the finish, but using your gell gun. If the AI touches
The zone in red, the level restarts.
*/

}
