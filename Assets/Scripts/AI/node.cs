using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; //This allows the IComparable Interface

// This is the class will be storing node data.
// In order to use a collection's Sort() method, this class needs to
// implement the IComparable interface.
[Serializable]
public class node  : IComparable<node>
{
    public Vector3 worldPosition;
    public bool visited;
    public bool expanded;
    public float Gcost;
    public float Hcost;
    public float Fcost;

    [NonSerialized] // avoids object composition cycle depth being reached
    public List<node> neighbours = null;

    public node(Vector3 pos) 
    {
        worldPosition = pos;
        Gcost = 0;
        Hcost = 0;
        Fcost = 0;
        visited = false;
        expanded = false;
    }

    public void addNeighbour(node n) 
    {
        if (neighbours == null) 
        {
            neighbours = new List<node>();
        }
        neighbours.Add(n);
    }

    public void removeNeighbour(node n) 
    {
        if (neighbours != null) 
        {
            if (neighbours.Contains(n)) 
            {
                neighbours.Remove(n);
            }
        }
    }

    public int CompareTo(node other)
    {
        if(other == null | other.worldPosition != worldPosition)
        {
            return 1;
        } else {
            return 0;
        }
    }

}
