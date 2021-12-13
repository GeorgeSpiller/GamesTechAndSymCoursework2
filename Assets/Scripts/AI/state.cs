using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; //This allows the IComparable Interface

// This is the class will be storing game data.
// In order to use a collection's Sort() method, this class needs to
// implement the IComparable interface.
[Serializable]
public class state  : IComparable<state>
{
    //public string state_ID;
    public string state_name;
    public bool state_isDissabled;
    //GameObject state_player;
    public Action state_updateFunction;
    public state(string name, bool isDissabled, Action stateUpdateFunction) 
    {
        state_name = name;
        state_isDissabled = isDissabled;
        state_updateFunction = stateUpdateFunction;
    }

    public void runStateUpdate() 
    {
        if (!state_isDissabled) {
            state_updateFunction();
        } else {
            Debug.Log("Cannot run state " + state_name + ", as this state has been dissabled.");
        }
    }

    public int CompareTo(state other)
    {
        if(other == null | other.state_name != state_name)
        {
            return 1;
        } else {
            return 0;
        }
    }

}
