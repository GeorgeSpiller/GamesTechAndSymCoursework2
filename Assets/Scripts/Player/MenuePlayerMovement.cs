using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuePlayerMovement : MonoBehaviour
{
    public float stoppingThreshold = 0.5f;
    public float playerSpeed = 10f;
    public GameObject currentLevel;
    public bool transitionTime = false;
    public GameObject nextLevel;
    
    private bool timeToDraw = false;



    GameObject getLevelInDirection(int dir) {

        switch (dir)
        {
            case 0:
                nextLevel = currentLevel.GetComponent<LevelButtonDirections>().Dir_up;
                if (nextLevel) { 
                    return nextLevel; 
                } else { return currentLevel; }
            case 1:
                nextLevel = currentLevel.GetComponent<LevelButtonDirections>().Dir_down;
                if (nextLevel) { 
                    return nextLevel; 
                } else { return currentLevel; }
            case 2:
                nextLevel = currentLevel.GetComponent<LevelButtonDirections>().Dir_right;
                if (nextLevel) { 
                    return nextLevel; 
                } else { return currentLevel; }
            case 3:
                nextLevel = currentLevel.GetComponent<LevelButtonDirections>().Dir_left;
                if (nextLevel) { 
                    return nextLevel; 
                } else { return currentLevel; }
            default:
                return currentLevel;
        }
    }

     void OnTriggerEnter(Collider other) 
     {
        if (other.tag == "GameController" && other.gameObject == nextLevel )
        {
            Rigidbody r = GetComponent <Rigidbody>();
             // stop player movement
            r.velocity = Vector3.zero;
            // set current level and next lvl
            currentLevel = nextLevel;
            nextLevel = currentLevel;
        }
     }

    bool hasStoppedMoving() 
    {
        Rigidbody r = GetComponent<Rigidbody>();
        if (r.velocity.magnitude < stoppingThreshold) 
        {
            // stop player from moving and return true
            r.velocity = Vector3.zero;
            return true;
        }
        return false;
    }

    private void saveGameData() 
    {
        // update the current lvl in the save file & save the state
        GameData savedGameData = ManageGameState.readGameData();
        string playerCurrentLevel = currentLevel.GetComponent<LevelButtonDirections>().ID.ToString();
        // save new state
        if (savedGameData != null)
        {
            GameData newSavedGameData = new GameData(savedGameData.completedLevels, playerCurrentLevel, savedGameData.AICurrentLevels);
            ManageGameState.writeGameGata(newSavedGameData);
        }
    }

    private void manageInput() 
    {
        // player presses movment key
        // set next level and dissable use input
        if (hasStoppedMoving())
        {
            if (Input.GetKey ("w")) {
                nextLevel = getLevelInDirection(0);
                transitionTime = false;
            }
            if (Input.GetKey ("s")) {
                nextLevel = getLevelInDirection(1);
                transitionTime = false;
            }
            if (Input.GetKey ("d")) {
                nextLevel = getLevelInDirection(2);
                transitionTime = false;
            }
            if (Input.GetKey ("a")) {
                nextLevel = getLevelInDirection(3);
                transitionTime = false;
            }
            if (Input.GetKey("space"))
            {
                transitionTime = true;
            }
         }
    }

    void Update()
    {
        manageInput();

        // if we need to move player to the next level 
        if (nextLevel != currentLevel && hasStoppedMoving()) 
        {   // we need to move player to the next level
            Vector3 moveLocation = new Vector3(nextLevel.transform.position.x, transform.position.y, nextLevel.transform.position.z);
            transform.localPosition = Vector3.MoveTowards (transform.localPosition, moveLocation, playerSpeed * Time.deltaTime * 1.2f);
            // always look straight ahead or else the slight changes in rotation throws player off course
            transform.LookAt(new Vector3(nextLevel.transform.position.x, transform.position.y, nextLevel.transform.position.z));
        } else if (currentLevel == nextLevel && hasStoppedMoving()) 
        {   // if the player is resting on target level, popup GUI
            string nextScene = currentLevel.GetComponent<LevelButtonDirections>().sceneName;
            string buttonType = currentLevel.GetComponent<LevelButtonDirections>().levelButtonType;
            transform.LookAt(new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z));

            if (buttonType == "incomplete" | buttonType == "complete") 
            { // player is on a complete or incomplete level, draw GUI
                timeToDraw = true;
                // if user is transitioning to a new level
                if (transitionTime)
                {
                    transitionTime = false;
                    // save game data and load next scene
                    // can only be done in editor due to some build issues (OS file location) crashing the executable
                    #if UNITY_EDITOR
                    saveGameData();
                    #endif
                    SceneManager.LoadScene (sceneName:nextScene);
                }
            } else if (buttonType == "dummy")
            {
                // player is on a dummy level
                // potential dummy level funcitonality goes here
                timeToDraw = false;
            } else 
            { // unassigned button type
                Debug.Log("Error: need to assign button type to obj " + currentLevel.gameObject);
            }
        }
    }

    void OnGUI()
    {
        if (timeToDraw) 
        {
            string nextScene = currentLevel.GetComponent<LevelButtonDirections>().sceneName;
            GUI.Box(new Rect(0, Screen.height - (Screen.height/8), Screen.width, Screen.height), "Press Space to play level: " + nextScene);
        }
    }
}
