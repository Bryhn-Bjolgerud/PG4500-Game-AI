using UnityEngine;

public class PathfindingCell
{
    /// <summary>
    /// This class's function is to convert an in game cell into a data structure we can use for pathfinding.
    /// </summary>

    //The coordinates of the cell. Because of the way the in game grid was constructued, GameObject.FindGameObjectsWithTag() retrives the cells from right to left. 
    //In a "normal" 2d coordinate system, an increased x value would indicate movement to the right.
    //In our 10x10 in game grid, (10, 10) would be in the top left corner, instead of the usual top right.
    public int xPos { get; set; }
    public int yPos { get; set; }
    public double fCost { get; set; } 
    public double gCost { get; set; } 
    public double hCost { get; set; }
    public PathfindingCell parent { get; set; }
    //A reference to the actual Gameobject. Needed to visualize the pathfinding.
    public GameObject cellInGame { get; set; }
    //Whether or not the cell is an obstacle or not.
    public bool walkable { get; set; }

}
