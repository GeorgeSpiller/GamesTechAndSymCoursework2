using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishForAI : MonoBehaviour
{
 void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "AI") 
        {
            // save the state
            GameData savedGameData = ManageGameState.readGameData();
            List<string> completedLevels = new List<string>();

            // add current level to completed level list
            if (savedGameData.completedLevels.Count > 0) 
            {
                completedLevels = savedGameData.completedLevels;
            }
            completedLevels.Add(savedGameData.playerCurrentLevel);

            // save new gameData
            GameData newSavedGameData = new GameData(completedLevels, savedGameData.playerCurrentLevel, savedGameData.AICurrentLevels);
            ManageGameState.writeGameGata(newSavedGameData);
            SceneManager.LoadScene(sceneName:"LevelSelect");
        }
    }
}
