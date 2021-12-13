using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelButtonDirections : MonoBehaviour
{

    public GameObject Dir_up;
    public GameObject Dir_down;
    public GameObject Dir_left;
    public GameObject Dir_right;
    public string sceneName = "LevelNull";
    public string levelButtonType = "incomplete";
    // used for 'unique' ID, as if two buttonObjects have the exact same position, 
    // it is likley an error.
    public string ID;

    private void Start() {
        ID = transform.position.ToString();
    }


    public string getButtonID() {
        return transform.position.ToString();
    }
}
