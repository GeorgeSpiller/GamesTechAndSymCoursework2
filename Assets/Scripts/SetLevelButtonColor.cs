using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetLevelButtonColor : MonoBehaviour
{
    public void updateColor() {
        // print("Updating type (" + GetComponent<LevelButtonDirections>().levelButtonType + ") and color of button " + name);
        updateLevelType();
        string lvlType = GetComponent<LevelButtonDirections>().levelButtonType;
        Renderer buttonRenderer = GetComponent<Renderer>();
        if (lvlType == "dummy") 
        {
            // set mat to blue
            buttonRenderer.material.SetColor("_Color", Color.blue);
        } else if (lvlType == "complete") 
        {
            print("Updated button " + name + "to mat: complete");
            // set mat to green
            buttonRenderer.material.SetColor("_Color", Color.green);
        } else
        {
            // set to red
            buttonRenderer.material.SetColor("_Color", Color.red);
        }
    }

    private void updateLevelType() 
    {
        // update the current lvl in the save file & save the state
        GameData savedGameData = ManageGameState.readGameData();
        List<string> completedLevels = savedGameData.completedLevels;
        string currentLevelID = GetComponent<LevelButtonDirections>().getButtonID();
        // if ID in the completed list, set ur tag to complete
        if (completedLevels.Contains(currentLevelID))
        {
            GetComponent<LevelButtonDirections>().levelButtonType = "complete";
        }
    }

    void Start()
    {
        updateColor();
    }
}
