using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuePlayerSetup : MonoBehaviour
{
    private void movePlayerToButtonID(string buttonID)
    {
        // for all buttons
        GameObject[] GameControllerObjects = GameObject.FindGameObjectsWithTag("GameController");
        foreach (GameObject button in GameControllerObjects) {
            // update all button colors based on if they were listed as completed in the save 
            button.GetComponent<SetLevelButtonColor>().updateColor();
            // if button is the new player position
            if (button.GetComponent<LevelButtonDirections>().getButtonID() == buttonID) 
            {
                // cancel all player momentum, move player to saved button
                GetComponent<Rigidbody>().velocity.Set(0f, 0f, 0f);
                Vector3 newPlayerPosition = button.GetComponent<Transform>().position;
                newPlayerPosition.y += 1f;
                transform.position = newPlayerPosition;
                GetComponent<MenuePlayerMovement>().currentLevel = button;
                GetComponent<MenuePlayerMovement>().nextLevel = GetComponent<MenuePlayerMovement>().currentLevel;
            }
        }
    }
    void Awake()
    {
        GetComponent<MenuePlayerMovement>().nextLevel = GetComponent<MenuePlayerMovement>().currentLevel;
        GetComponent<MenuePlayerMovement>().transitionTime = false;
        // load the saved game data from txt file
        GameData savedGameData = ManageGameState.readGameData();
        // place player at that position
        movePlayerToButtonID(savedGameData.playerCurrentLevel);
        
    }

    void OnApplicationQuit()
    {
        Debug.Log("Application ending, deleting all saved data.");
        ManageGameState.writeGameGata(ManageGameState.getStartState());
    }
}
