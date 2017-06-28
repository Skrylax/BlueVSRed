using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

    //variables
    //public Transform player;
    private Transform floor;
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public TerrainType[] walkableRegions;
    public float nodeRadius;
    

    public bool displayGridGizmos;
    private Node[,] grid;
    private float nodeDiameter;
    private int gridSizeX, gridSizeY;
    private LayerMask walkableMask;
    private Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int, int>();

    private void Awake()
    {
        floor = GameObject.FindGameObjectWithTag("Floor").transform;
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        foreach(TerrainType region in walkableRegions)
        {
            //Use BitWise OR because layers are saved in 32bit
            walkableMask.value |= region.terrainMask.value;
            walkableRegionsDictionary.Add((int) Mathf.Log(region.terrainMask.value, 2),region.terrainPenalty);
        }

        CreateGrid();
    }

    public int maxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    /// <summary>
    /// Create the grid based on the variables
    /// </summary>
    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        //Determine the bottom left point of our world
        Vector3 worldBottomLeft = floor.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        //Loop through all nodes and fill our grid
        for (int x = 0; x < gridSizeX; x++ )
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                //Check if the node is walkable by checking sphere around the worldpoint
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));

                int movementPenalty = 0;

                // raycast
                if(walkable)
                {
                    Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 100, walkableMask))
                    {
                        walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                    }
                }

                grid[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);
            }
        }
    }
    
    /// <summary>
    /// When method gets called, draw our grid so we can see the walkable and unwalkable terrain
    /// </summary>
    void OnDrawGizmos()
    {
        if(floor != null)
            Gizmos.DrawWireCube(floor.position, new Vector3(gridWorldSize.x, 0, gridWorldSize.y));
        //if the grid exists we can draw
        if (grid != null && displayGridGizmos)
        {
            //Check where the player currently is
            foreach (Node n in grid)
            {
                //if n is walkable make color white, otherwise make color red
                Gizmos.color = (n.walkable) ? Color.white : Color.red;
                Gizmos.DrawCube(new Vector3(n.worldPosition.x, n.worldPosition.y, n.worldPosition.z), Vector3.one * (nodeDiameter - 0.1f));
            }
        }
    }

    /// <summary>
    /// Retrieve a node from a certain world position
    /// </summary>
    /// <param name="worldPosition">position of which we want the Node (Vector3f)</param>
    /// <returns>Node which belongs to worldPosition Vector </returns>
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX =(worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x ;
        float percentY =(worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;

        //Clamp the value between 0 and 1 (incase the worldPosition is outside the size)
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeX - 1) * percentY);

        return grid[x, y];
    }

    /// <summary>
    /// Retrieve neighbours of a node
    /// </summary>
    /// <param name="node">Node which's neighbours we want</param>
    /// <returns>List of neighbours</returns>
    public List<Node> getNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for(int x = -1; x<=1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                //if both x and y are 0 we are on our node and its not a neighbour
                if(x == 0 && y == 0)
                {
                    continue;
                }
                else
                {
                    //Create 2 new variables for the neighbours location
                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;

                    //Make sure the neighbours is on our grid before adding it
                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    {
                        neighbours.Add(grid[checkX, checkY]);
                    }
                    
                }
            }
        }
        return neighbours;
    }

    [System.Serializable]
    public class TerrainType
    {

        public LayerMask terrainMask;
        public int terrainPenalty;
    }
}
