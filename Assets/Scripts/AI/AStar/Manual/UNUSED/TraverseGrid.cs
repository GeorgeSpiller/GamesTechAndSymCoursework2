using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraverseGrid : MonoBehaviour
{
    /*
    Unused code. Demostrates

    This script allows the attached object to traverse a grid of vectors using the A* algorithm.
    Requirments:
            - Must have the 'HandelGrid' component attached to the same object
            - Must have set the start and end positions
    
    Step 1, evaluating the grid: 
        Access the HandelGrid component and call its update to generate the most recent update of the grid. BY REF
        Compare this grid to the current grid tht is being searched. If they are different, then set the start
        position to the current transform position, and re-run A* search over the newly updated grid. If they 
        are the same, then continue to traverse this grid.
    Step 2, traversing the grid:
        By now we have an up to date grid and a path. In update, we evaluate the grid as explained in step 1, 
        then we move to the next node as deturmined in the A* search with a given speed. 
    */

/* psudocode taken from:  https://mat.uab.cat/~alseda/MasterOpt/AStar-Algorithm.pdf

    Put node_start in the OPEN list with f (node_start) = h(node_start) (initialization)
    while the OPEN list is not empty {
        Take from the open list the node node_current with the lowest
        f (node_current) = g(node_current) + h(node_current)
        if node_current is node_goal we have found the solution; break
        Generate each state node_successor that come after node_current
        for each node_successor of node_current {
            Set successor_current_cost = g(node_current) + w(node_current, node_successor)
            if node_successor is in the OPEN list {
                if g(node_successor) ≤ successor_current_cost continue 
            } else if node_successor is in the CLOSED list {
                if g(node_successor) ≤ successor_current_cost continue 
                Move node_successor from the CLOSED list to the OPEN list
            } else {
                Add node_successor to the OPEN list
                Set h(node_successor) to be the heuristic distance to node_goal
            }
            Set g(node_successor) = successor_current_cost
            Set the parent of node_successor to node_current
        }
        Add node_current to the CLOSED list
    }
    if(node_current != node_goal) exit with error (the OPEN list is empty)
*/

    public Transform startTransform;
    public Transform endTransform;

    private node node_start;
    private node node_end;
    private node currentNode;
    private List<node> currentGrid;
    private List<node> previousGrid;
    private List<node> currentBestPath;

    public static bool CompareLists<T>(List<T> aListA, List<T> aListB)
    {
        /*
        Code taken from user Bunny83:
        https://answers.unity.com/questions/1307074/how-do-i-compare-two-lists-for-equality-not-caring.html
        */
        if (aListA == null || aListB == null || aListA.Count != aListB.Count)
            return false;
        if (aListA.Count == 0)
            return true;
        Dictionary<T, int> lookUp = new Dictionary<T, int>();
        // create index for the first list
        for(int i = 0; i < aListA.Count; i++)
        {
            int count = 0;
            if (!lookUp.TryGetValue(aListA[i], out count))
            {
                lookUp.Add(aListA[i], 1);
                continue;
            }
            lookUp[aListA[i]] = count + 1;
        }
        for (int i = 0; i < aListB.Count; i++)
        {
            int count = 0;
            if (!lookUp.TryGetValue(aListB[i], out count))
            {
                // early exit as the current value in B doesn't exist in the lookUp (and not in ListA)
                return false;
            }
            count--;
            if (count <= 0)
                lookUp.Remove(aListB[i]);
            else
                lookUp[aListB[i]] = count;
        }
        // if there are remaining elements in the lookUp, that means ListA contains elements that do not exist in ListB
        return lookUp.Count == 0;
    }

    private List<node> calculateAStar(node startNode, node targetNode, List<node> grid) 
    {
        List<node> bestPath = new List<node>();
        List<node> open_list = new List<node>();
        List<node> closed_list = new List<node>();
        node currentNode;
        open_list.Add(startNode);
        print(startNode);

        while (open_list.Count > 0) 
        { 
            print(open_list[0]);
            currentNode = open_list[0];
            
            // calc costs
            foreach (node n in currentNode.neighbours) 
            {
                n.Gcost = Vector3.Distance(node_start.worldPosition, n.worldPosition);
                n.Hcost = Vector3.Distance(node_end.worldPosition, n.worldPosition);
                n.Fcost = n.Gcost + n.Hcost;
            }

            for (int i = 1; i < open_list.Count; i++)
            {
                if (open_list[i].Fcost < currentNode.Fcost || open_list[i].Fcost == currentNode.Fcost && open_list[i].Hcost < currentNode.Hcost)
                {
                    currentNode = open_list[i];
                }
            }
            open_list.Remove(currentNode);
            closed_list.Add(currentNode);

            if (currentNode == targetNode)
            {
                return bestPath;
            }
            
            foreach (node neighbour in currentNode.neighbours) 
            {
                if (closed_list.Contains(neighbour)) continue;
    
                float newMovementCostToNeighbour = currentNode.Gcost + Vector3.Distance(currentNode.worldPosition, neighbour.worldPosition);
                if(newMovementCostToNeighbour < neighbour.Gcost || !open_list.Contains(neighbour))
                {
                    neighbour.Gcost = newMovementCostToNeighbour;
                    neighbour.Hcost = Vector3.Distance(neighbour.worldPosition, targetNode.worldPosition);
                    bestPath.Add(currentNode);
    
                    if (!open_list.Contains(neighbour))
                        open_list.Add(neighbour);
                }
            }
        }
        return null;
    }


    void Start()
    {
        // get the start and finish positions of the AI
        node_start = GetComponent<HandelGrid>().getNerestNode(startTransform.position);
        node_end = GetComponent<HandelGrid>().getNerestNode(endTransform.position);
        currentGrid = GetComponent<HandelGrid>().getGrid();
        previousGrid = currentGrid;
        currentNode = node_start;
    }

    void Update()
    {
        currentBestPath = calculateAStar(node_start, node_end, currentGrid);
        print("Path Found:");
        foreach (node n in currentBestPath) 
        {
            print(n.worldPosition);
        }
        print("End Path Found.");

        // if (CompareLists(currentGrid, previousGrid)) 
        // {
        //     // continues along best path
        // } else {
        //     // recalculate best path using A*
        // }
    }
}
