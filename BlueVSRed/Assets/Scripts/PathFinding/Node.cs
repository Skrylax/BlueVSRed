using UnityEngine;
using System.Collections;

/// <summary>
/// gCost = distance from starting node
/// hCost = distance to destination
/// fCost = gCost + hCost
/// </summary>
public class Node : IHeapItem<Node>
{
    public int gCost;
    public int hCost;
    public int gridX;
    public int gridY;
    public int movementPenalty;
    public bool walkable;
    public Vector3 worldPosition;
    public Node parent;


    private int nodeHeapIndex;

    public Node(bool walkable, Vector3 worldPosition, int gridX, int gridY, int movementPenalty)
    {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
        this.gridX = gridX;
        this.gridY = gridY;
        this.movementPenalty = movementPenalty;
    }

    /// <summary>
    /// Only get because fCost never gets assigned, will only be calculated with g/hCost
    /// </summary>
    public int fCost
    {
        get
        {
            return gCost + hCost;
        }

    }

    public int HeapIndex
    {
        get
        {
            return nodeHeapIndex;
        }
        set
        {
            nodeHeapIndex = value;
        }
    }

    /// <summary>
    /// Compare 2 nodes
    /// </summary>
    /// <param name="compareNode">Node to compare with</param>
    /// <returns>
    /// Less than zero: This instance precedes compareNode in the sort order
    /// Zero This instance occurs in the same position in the order as compareNode
    /// Greater than zero: This instance follows compareNode in the sort order
    /// </returns>
    public int CompareTo(Node compareNode)
    {
        int compare = fCost.CompareTo(compareNode.fCost);
        if(compare == 0)
        {
            compare = hCost.CompareTo(compareNode.hCost);
        }

        return -compare;
    }
}
