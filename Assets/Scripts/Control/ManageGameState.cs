using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using UnityEngine;

public class ManageGameState : MonoBehaviour
{
    private static string m_Path = Application.dataPath;
    private static string pathToGameData = m_Path + "/GameData/GameSaveData.txt";

    public static void writeGameGata(GameData currentState)
    {
        checkTextFileExists();
        // remove duplicate entries
        currentState.completedLevels = currentState.completedLevels.Distinct().ToList();
        //Write some text to the file
        string saveText = JsonUtility.ToJson(currentState);
        File.WriteAllText(pathToGameData, saveText);
    }


    public static GameData readGameData()
    {
        checkTextFileExists();  
        string saveText = File.ReadAllText(pathToGameData);
        var saveObject = JsonUtility.FromJson<GameData>(saveText);
        if (saveObject == null) 
        {
            saveObject = getStartState();
        }
        return saveObject;
    }

    private static void checkTextFileExists() {
        if (!File.Exists(pathToGameData))
        {
            File.Create(pathToGameData);
        } 
    }

    public static GameData getStartState()
    {
        return new GameData(new List<string>(), "", new List<string>());
    }

    public static string printGameData(GameData obj)
    {
        return JsonUtility.ToJson(obj);
    }
}
