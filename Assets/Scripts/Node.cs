using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node
{
    public bool walkable;           
    public Vector3 worldPosition;   
    public int gridX;              
    public int gridY;               

    public int penalty;

    public Vector2Int trafficDirection = Vector2Int.zero;

    public int gCost;
    public int hCost;
    public Node parent;

    public int fCost
    {
        get { return gCost + hCost; }
    }


    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, int _penalty = 0)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        penalty = _penalty;
    }
}
