using UnityEngine;

// Node object to keep track of path cost
public class NodeObject
{

    public bool walkable = false;

    public Vector3Int position;
    /// <summary>Distance from the starting node.</summary>
    public int gCost;
    /// <summary>Distance from the end node.</summary>
    public int hCost;
    /// <summary>G cost + H cost.</summary>
    public int fCost { get => gCost + hCost; }

    public int cornerOrientation;
    /// <summary>This is the parent node this node is linked to. Use this to back trace a path from the end to the start.</summary>
    public NodeObject parent;

    // Initializers
    public NodeObject(Vector3Int position, int gCost, int hCost, int fCost, bool _walkable)
    {
        this.position = position;
        this.gCost = gCost;
        this.hCost = hCost;
        walkable = _walkable;
    }
    public NodeObject(Vector3Int position, int gCost, int hCost, int fCost, bool _walkable, int _orientation)
    {
        this.position = position;
        this.gCost = gCost;
        this.hCost = hCost;
        walkable = _walkable;
        cornerOrientation = _orientation;
    }
    public NodeObject(Vector3Int position)
    {
        this.position = position;
        this.gCost = 0;
        this.hCost = 0;
    }

}