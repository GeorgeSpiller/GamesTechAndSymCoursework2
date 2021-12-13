using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; //This allows the IComparable Interface

// This is the class will be storing game data.
// In order to use a collection's Sort() method, this class needs to
// implement the IComparable interface.
[Serializable]
public class GameData : IComparable<GameData>
{
    [SerializeField]
    public List<string> completedLevels = new List<string>();

    [SerializeField]
    public string playerCurrentLevel;

    [SerializeField]
    public List<string> AICurrentLevels = new List<string>();

    public GameData(List<string> _completedLevels, string _playerCurrentLevel, List<string> _AICurrentLevels)
    {
        completedLevels = _completedLevels;
        playerCurrentLevel = _playerCurrentLevel;
        AICurrentLevels = _AICurrentLevels;
    }

    public int CompareTo(GameData other)
    {
        if(other == null)
        {
            return 1;
        } else if (other.completedLevels != completedLevels | other.playerCurrentLevel != playerCurrentLevel | other.AICurrentLevels != AICurrentLevels) {
            return 1;
        } else {
            return 0;
        }
    }
}
