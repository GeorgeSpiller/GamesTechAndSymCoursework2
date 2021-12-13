using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandelGrid : MonoBehaviour
{
    /*
    Currently unused code, but does demostrate my attempt to manually implement the A* search algorithm 
    in order to path find. Attaching this script onto an object and assiging the level plane to the 
    levelPlane GameObject will generate a grid of verticies that dissapear when gell is placed benieth them.
    (this is visualised using sphere primatives).

    This script performs operations to retrun a 2D array of vectors. These vectors are
    then used by other components ('TraverseGrid').
    Requirments:
        - An object for which the grid will generate over (encapsulate). This would be
            by default the level plane. 
    
    Generating the grid:
        1. Get the dimentions of the grid. This is the bounds size.
        3. Generate a new vector3 where the Y is constant, for each X and Z of the bounds

    next steps:
    - reduce the resolution of the gird?
    - Start the A* search algorithm
     */

    public GameObject levelPlane;
    public float constantYHeight = 0f;
    public float drawYHeight = 5f;
    public bool showGrid = false;


    // the 2D array of vectors that serve as our grid.
    private List<node> grid = new List<node>();
    // sphere primitive used to visualise our grid of verticies
    private List<GameObject> spherePrimitives = new List<GameObject>();
    private List<GameObject> spherePrimitivesToRemove = new List<GameObject>();
    private Bounds gridBounds;
    private int boundsSizeX;
    private int boundsSizeZ;

    public List<node> getGrid() 
    {
        return grid;
    }

    public void removeNodeNear(Vector3 removePos, Vector3 removeScale) 
    {
        // removes nodes in the grid that are near a specific point, by creating bounds around this point
        // and removing grid nodes that fall within these bounds.
        if (removeScale == Vector3.zero)
        {
            removeScale = new Vector3(2, 5, 2);
        } else {
            removeScale = new Vector3(removeScale.x, 5, removeScale.z);
        }
        List<node> nodesToRemove = new List<node>(grid);
        Bounds removeNodes = new Bounds(removePos, removeScale);
        foreach (node node in grid) 
        {
            if (removeNodes.Contains(node.worldPosition)) 
            {
                nodesToRemove.Remove(node);
            }
        }
        if (showGrid) 
        {
            foreach (GameObject sphere in spherePrimitives) 
            {
            if (removeNodes.Contains(new Vector3(sphere.transform.position.x, constantYHeight, sphere.transform.position.z))) 
                {
                    spherePrimitivesToRemove.Add(sphere);
                }
            }
        }
        foreach (node n in nodesToRemove) 
        {
            // remove any nodes that have neighbours that are gunna be destroyed
            foreach(node neighbour in n.neighbours) 
            {
                if (neighbour == n) 
                {
                    n.removeNeighbour(neighbour);
                }
            }
            grid.Remove(n);
        }
        
        if (showGrid) 
        {
            visualiseGrid();
        }
    }

    public node getNerestNode(Vector3 initPos) 
    {
        // return the node in the grid that is near a position
        Bounds removeNodes = new Bounds(initPos, new Vector3(2, 5, 2));
        foreach (node node in grid) 
        {
            if (removeNodes.Contains(node.worldPosition)) 
            {
                return node;
            }
        }
        return null;
    }

    private void generateGrid() 
    {
        // generates a grid the size of boundsSizeX x boundsSizeZ and assigns neighbour nodes
        Vector3 offset = new Vector3(levelPlane.transform.position.z - (boundsSizeX/2), 0, levelPlane.transform.position.z - (boundsSizeZ/2));
        for (int i = 0, z = 0; z <= boundsSizeZ; z++) {
			for (int x = 0; x <= boundsSizeX; x++, i++) {
				grid.Add(new node(new Vector3(x, constantYHeight, z) + offset));
			}
		}
        assignNeighbours(); 
    }

    private void assignNeighbours() 
    {
        //  assigns neighbour nodes to all nodes
        Vector3 range = Vector3.one * 1.5f;
        int index = 0;
        foreach (node currNode in grid) 
        {
            int currNodeIndex = grid.IndexOf(currNode);
            if (currNodeIndex + 1 < grid.Count - 1) 
            {
                node npos = grid[currNodeIndex + 1];
                currNode.addNeighbour(npos);
            }
            if (currNodeIndex - 1 >= 0) 
            {
                node nneg = grid[currNodeIndex - 1];
                currNode.addNeighbour(nneg);
            }
            if (currNodeIndex + boundsSizeZ < grid.Count - 1) 
            {
                node zpos = grid[currNodeIndex + boundsSizeZ];
                currNode.addNeighbour(zpos);
            }
           if (currNodeIndex - boundsSizeZ  >= 0) 
           {
                node zneg = grid[currNodeIndex - boundsSizeZ];
                currNode.addNeighbour(zneg);
           }
            index ++;
        }
    }

    private void visualiseGrid() 
    {
        // destroy all sphere primatives that have been removed by gell
        foreach (GameObject sphere in spherePrimitivesToRemove) 
        {
            spherePrimitives.Remove(sphere);
            Destroy(sphere);
        }

    }

    void Start()
    {
        // unity planes are a 10x10 mesh
        gridBounds = new Bounds(transform.position, levelPlane.transform.localScale * 10f);
        boundsSizeX = (int) gridBounds.size.x;
        boundsSizeZ = (int) gridBounds.size.z;

        generateGrid();
        if (showGrid) 
        {
            GameObject emptyParent = new GameObject("ShowGridSpheres");
            // create primitives for each vert
            foreach (node node in grid) 
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = new Vector3(node.worldPosition.x, drawYHeight, node.worldPosition.z);
                sphere.transform.parent = emptyParent.transform;
                spherePrimitives.Add(sphere);
            }
            visualiseGrid();
        }
    }


    void Update(){}
}
