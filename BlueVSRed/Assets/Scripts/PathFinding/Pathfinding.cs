using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;

/// <summary>
/// gCost = distance from starting node
/// hCost = distance to destination
/// fCost = gCost + hCost
/// </summary>
public class Pathfinding : MonoBehaviour {

    private PathRequestManager requestManager;
    public Grid grid;

    void Awake()
    {
        grid = GetComponent<Grid>();
        requestManager = GetComponent<PathRequestManager>();
        //grid = GetComponent<Grid>();
    }

    public void StartFindPath(Vector3 start, Vector3 end)
    {
        if (gameObject && gameObject.activeSelf)
            StartCoroutine(FindPath(start, end));
    }

    /// <summary>
    /// Find the shortest path
    /// </summary>
    /// <param name="start">starting location</param>
    /// <param name="destination">destination</param>
	private IEnumerator FindPath(Vector3 start, Vector3 destination)
    {
        //UnityEngine.Debug.Log("Finding path");
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;
        
        //Retrieve the Node locations from the Vector3
        Node startNode = grid.NodeFromWorldPoint(start);
        Node destNode = grid.NodeFromWorldPoint(destination);
        Node lastNeighbour = startNode;


        //Create the open and closed list to keep track of all nodes and add our startNode to the open list
        Heap<Node> open = new Heap<Node>(grid.maxSize);
        HashSet<Node> closed = new HashSet<Node>();
            
        open.Add(startNode);
            
        //TODO: If user clicks on unwalkable node find a walkable node closeby.
        //Loop through all the nodes in the open list
        if(destNode.walkable)
        while (gameObject && gameObject.activeSelf && open.Count > 0)
        {
            Node currentNode = open.RemoveFirst();

            closed.Add(currentNode);

            //If we reach the destination, break out of the loop
            if (currentNode == destNode)
            {
                sw.Stop();
                //UnityEngine.Debug.Log("Path found " + sw.ElapsedMilliseconds + " ms");
                pathSuccess = true;
                break;
            }

            //Check all the node's neighbours
            foreach (Node neighbour in grid.getNeighbours(currentNode))
            {
                //Skip iteration if the neighbour isn't walkable or if its been closed
                if (!neighbour.walkable || closed.Contains(neighbour))
                {
                    continue;
                }

                //Calculate the new movement cost to the found neighbour
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementPenalty;
                //If the new movement cost is less than the neighbours gCost or if the neighbours is not in the open list
                if (newMovementCostToNeighbour < neighbour.gCost || !open.Contains(neighbour))
                {
                    //Update the neighbours costs and set its parent
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, destNode);
                    neighbour.parent = currentNode;

                    //If open doesn't contain neighbour add it 
                    if (!open.Contains(neighbour))
                    {
                        open.Add(neighbour);
                        lastNeighbour = neighbour;
                    }
                    else
                    {
                        open.UpdateItem(neighbour);
                        lastNeighbour = neighbour;
                    }
                }
            }
        }
        yield return null;
        if(pathSuccess)
        {
            waypoints = RetracePath(startNode, destNode); 
        }
        //If path is unsuccessful walk to the closest walkable neighbour
        //else
        //{
        //    Node bestNeighbour = startNode;
        //    List<Node> destinationNeighbours = new List<Node>();
        //    List<Node> checkedNeighbours = new List<Node>();
        //    bool foundNeighbour = false;

        //    //Check all neighbours to the destination
        //    foreach (Node neighbour in grid.getNeighbours(destNode))
        //    {
        //        if(neighbour.walkable)
        //        {
        //            bestNeighbour = neighbour;
        //            foundNeighbour = true;
        //            break;
        //        }
        //        destinationNeighbours.Add(neighbour);
        //    }
        //    //If there is no valid neighbour next to the destination, start checking around it
        //    int iterator = 0;
        //    while (!foundNeighbour)
        //    {      
        //        foreach(Node neighbour in grid.getNeighbours(destinationNeighbours[iterator]))
        //        {            
        //            if (!checkedNeighbours.Contains(neighbour))
        //            {
        //                checkedNeighbours.Add(neighbour);
        //                if (neighbour.walkable)
        //                {
        //                    bestNeighbour = neighbour;
        //                    foundNeighbour = true;
        //                }
        //                destinationNeighbours.Add(neighbour);
        //            }

        //        }
        //        iterator++;
        //    }
        //    sw.Stop();
        //    //UnityEngine.Debug.Log("Path found " + sw.ElapsedMilliseconds + " ms");
        //    destinationNeighbours.Clear();
        //    waypoints = RetracePath(startNode, bestNeighbour);
        //    pathSuccess = true;
        //}

        requestManager.FinishedProcessingPath(waypoints, pathSuccess);

    }

    //Retrace our path by backtracking the parents.
    private Vector3[] RetracePath(Node start, Node end)
    {
        List<Node> path = new List<Node>();
        Node currentNode = end;
        
        while (currentNode != start && currentNode.parent != null)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        //Reverse the path because we went backwards
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;
        
    }

    /// <summary>
    /// Make a list of nodes into a vector3[] of waypoints
    /// </summary>
    /// <param name="path">List of nodes of path to be followed</param>
    /// <returns>Vector3[] of waypoints</returns>
    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for(int i = 1; i<path.Count; i++)
        {
            Vector2 direction = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if(direction != directionOld)
            {
                waypoints.Add(path[i-1].worldPosition);
                directionOld = direction;
            }
        }
        return waypoints.ToArray();
    }

    /// <summary>
    /// Check the distance between 2 nodes
    /// </summary>
    public int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if(distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        return 14 * distX + 10 * (distY - distX);
    }
}