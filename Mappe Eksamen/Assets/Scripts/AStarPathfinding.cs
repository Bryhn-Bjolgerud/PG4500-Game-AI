using System;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding : MonoBehaviour
{
    //The cells that have been visited.
    private List<PathfindingCell> _checkedCells;
    //The cells that are currently being considered.
    private List<PathfindingCell> _uncheckedCells;
    //The valid cells that are adjacent to the current cell.
    private List<PathfindingCell> _walkableNeighbourCells;
    //The whole grid as a list.
    private GameObject[] _pfList;
    //The grid as a 2d array. 
    private PathfindingCell[,] _pfGrid;

    private PathfindingCell _start;
    private PathfindingCell _goal;

    //Different colors to help visualize the pathfinding. 
    public Color gray;
    public Color blue;
    public Color green;
    public Color red;

    private bool _pathFound;

    //The agent using the path, and the script allowing it to follow that path.
    private PathFollowing _pathFollowing;
    private GameObject _lionPathFollowing;

    /// <summary>
    /// Visualizes the best path found from start to goal. A little low cohesion since it finds the path, and draws it.
    /// </summary>
    public void getPath()
    {
        //Initializing all the variables used during the pathfinding. Reinitialized to their default values each time the function is called, since the algorithm needs clean slates to work.
        _checkedCells = new List<PathfindingCell>();
        _uncheckedCells = new List<PathfindingCell>();
        _walkableNeighbourCells = new List<PathfindingCell>();
        _start = new PathfindingCell();
        _goal = new PathfindingCell();
        //The array has an extra row and column to make sure we dont get an index out of bounds error.
        _pfGrid = new PathfindingCell[12, 12];

        PathfindingCell currentNode = new PathfindingCell();
        int k = 0;

        //Setting all the necessary values before we start our algorithm.
        for (int i = 0; i < 12; i++)
        {
            for (int j = 0; j < 12; j++)
            {
                _pfGrid[i, j] = new PathfindingCell();
            }
        }
        for (int i = 1; i < 11; i++)
        {
            for (int j = 1; j < 11; j++)
            {
                _pfGrid[i, j].xPos = j;
                _pfGrid[i, j].yPos = i;
                _pfGrid[i, j].cellInGame = _pfList[k];
                //If a cell is the color blue it means it is a obstacle.
                if (_pfList[k].GetComponent<Renderer>().material.color != blue)
                {
                    _pfGrid[i, j].walkable = true;
                }
                //If a cell is the color green it means it is the start.
                if (_pfList[k].GetComponent<Renderer>().material.color == green)
                {
                    _start = _pfGrid[i, j];
                }
                //If a cell is the color red it means it is the goal.
                if (_pfList[k].GetComponent<Renderer>().material.color == red)
                {
                    _goal = _pfGrid[i, j];
                }
                k++;
            }
        }

        _uncheckedCells.Add(_start);

        while (_uncheckedCells.Count > 0)
        {
            double tmpFCost = 1500;

            //Setting the currentnode to the cell with the lowest fcost (the node with the lowest combined travel cost). 
            foreach (PathfindingCell cell in _uncheckedCells)
            {
                if (cell.fCost < tmpFCost)
                {
                    currentNode = cell;
                    tmpFCost = cell.fCost;
                }
            }

            _uncheckedCells.Remove(currentNode);
            _checkedCells.Add(currentNode);

            //When we have arrived at the goal cell, we quit and backtrack to get the path.
            if (currentNode.xPos == _goal.xPos && currentNode.yPos == _goal.yPos)
            {
                
                //When a path has been found we start the path following lion. 
                _pathFollowing.currentWaypoint = currentNode;
                _pathFollowing.enabled = true;
                _pathFollowing.resetAndPreparePathFollowingLion();

                //Coloring all the cells that were considered but not good enough.
                foreach(PathfindingCell cell in _uncheckedCells)
                {
                    cell.cellInGame.GetComponent<Renderer>().material.color = Color.yellow;
                }

                //Coloring in the cells that make up the path.
                while (currentNode.parent != null)
                {
                    currentNode.cellInGame.GetComponent<Renderer>().material.color = Color.magenta;
                    currentNode = currentNode.parent;
                }
                _start.cellInGame.GetComponent<Renderer>().material.color = green;
                _goal.cellInGame.GetComponent<Renderer>().material.color = red;
                _pathFound = true;
                break;
            }

            _walkableNeighbourCells = getViableNeighbourCells(currentNode, _pfGrid);

            //After getting the valid adjacent cells, calculating all their f,g,h costs.
            foreach (PathfindingCell cell in _walkableNeighbourCells)
            {
                double gCost = 0;
                //If we have allready visited this cell, we want to skip it and check the next neighbour.
                if (_checkedCells.Contains(cell))
                {
                    continue;
                }
                //If the cell is allready being considered we mark down it's g-cost. Then we recalculate the gcost with the currentnode as the parent. If that is lower we update it.
                if (_uncheckedCells.Contains(cell))
                {
                    gCost = cell.gCost;
                }

                cell.parent = currentNode;
                //Using heuristics to calculate the distance away from the goal.
                cell.hCost = Math.Sqrt((cell.xPos - _goal.xPos) * (cell.xPos - _goal.xPos)) + ((cell.yPos - _goal.yPos) * (cell.yPos - _goal.yPos));
                //If the parent has both different x and y values, that mean it is a diagonal move.
                if (cell.xPos != currentNode.xPos && cell.yPos != currentNode.yPos)
                {
                    cell.gCost = currentNode.gCost + 1.4;
                }
                else
                {
                    cell.gCost = currentNode.gCost + 1.0;
                }
                cell.fCost = cell.gCost + cell.hCost;

                //If the recalculated g-cost is not lower, then we skip to the next neighbour.
                if (_uncheckedCells.Contains(cell))
                {
                    if (cell.gCost >= gCost)
                    {
                        continue;
                    }

                }

                //Adding the neighbour to the list of cells we are considering.
                _uncheckedCells.Add(cell);
            }

        }
        //If all the cells were checked, but the algorithm didnt find a path it gets printed out.
        if (!_pathFound)
        {
            //Drawing a sad smiley face on the grid to indicate that no path was found.
            foreach (GameObject obj in _pfList)
            {
                obj.GetComponent<Renderer>().material.color = gray;
            }
            _pfGrid[4, 3].cellInGame.GetComponent<Renderer>().material.color = Color.cyan;
            _pfGrid[3, 3].cellInGame.GetComponent<Renderer>().material.color = Color.cyan;
            _pfGrid[4, 4].cellInGame.GetComponent<Renderer>().material.color = Color.cyan;
            _pfGrid[4, 5].cellInGame.GetComponent<Renderer>().material.color = Color.cyan;
            _pfGrid[4, 6].cellInGame.GetComponent<Renderer>().material.color = Color.cyan;
            _pfGrid[4, 7].cellInGame.GetComponent<Renderer>().material.color = Color.cyan;
            _pfGrid[4, 8].cellInGame.GetComponent<Renderer>().material.color = Color.cyan;
            _pfGrid[3, 8].cellInGame.GetComponent<Renderer>().material.color = Color.cyan;
            _pfGrid[7, 4].cellInGame.GetComponent<Renderer>().material.color = Color.cyan;
            _pfGrid[8, 4].cellInGame.GetComponent<Renderer>().material.color = Color.cyan;
            _pfGrid[7, 7].cellInGame.GetComponent<Renderer>().material.color = Color.cyan;
            _pfGrid[8, 7].cellInGame.GetComponent<Renderer>().material.color = Color.cyan;
            Debug.Log("No path was found");
        }
    }

    /// <summary>
    /// Resets the color of each cell on the grid. All the values are resest each time the getPath() function is called.
    /// </summary>
    public void clearGrid()
    {
        foreach (GameObject obj in _pfList)
        {
            obj.GetComponent<Renderer>().material.color = gray;
        }
        _pathFound = false;
    }

    /// <summary>
    /// Given a specific node and a copy of the whole grid. Finds all the valid neighbours in a 3x3 area.
    /// </summary>
    /// <param name="current"></param>
    /// <param name="pfGrid"></param>
    /// <returns>A list with all the viable neighbours from the current node.</returns>
    public List<PathfindingCell> getViableNeighbourCells(PathfindingCell current, PathfindingCell[,] pfGrid)
    {
        List<PathfindingCell> viableNeighbourCells = new List<PathfindingCell>();
        List<PathfindingCell> viableNeighbourCellsCopy = new List<PathfindingCell>();

        //Adding every neighbour to the list, regardless of if they are viable.
        viableNeighbourCells.Add(pfGrid[current.yPos, current.xPos + 1]);
        viableNeighbourCells.Add(pfGrid[current.yPos + 1, current.xPos + 1]);
        viableNeighbourCells.Add(pfGrid[current.yPos + 1, current.xPos]);
        viableNeighbourCells.Add(pfGrid[current.yPos + 1, current.xPos - 1]);
        viableNeighbourCells.Add(pfGrid[current.yPos, current.xPos - 1]);
        viableNeighbourCells.Add(pfGrid[current.yPos - 1, current.xPos - 1]);
        viableNeighbourCells.Add(pfGrid[current.yPos - 1, current.xPos]);
        viableNeighbourCells.Add(pfGrid[current.yPos - 1, current.xPos + 1]);

        //The cells that are walkable get added to the list that is returned.
        foreach (PathfindingCell cell in viableNeighbourCells)
        {
            if (cell.walkable)
            {
                viableNeighbourCellsCopy.Add(cell);
            }
        }

        return viableNeighbourCellsCopy;
    }

    void Start()
    {
        _lionPathFollowing = GameObject.FindGameObjectWithTag("LionPathFollowing");
        _pathFollowing = _lionPathFollowing.GetComponent<PathFollowing>();
        //Used FindObjectGameobjectsWithTag but that did not retrive the gameobjects in the same order when playing the game from the editor and a built version. 
        //Had to order the cells and get them through the parent instead.
        _pfList = new GameObject[100];
        for(int i = 0; i < 100; i++)
        {
            _pfList[i] = GameObject.Find("PathfindingEnclosure").transform.GetChild(i).gameObject;
        }
        
        
        gray = new Color32(106, 53, 106, 255);
        blue = new Color32(38, 64, 255, 255);
        green = new Color32(102, 157, 179, 255);
        red = new Color32(255, 79, 88, 255);

        foreach (GameObject obj in _pfList)
        {
            obj.GetComponent<Renderer>().material.color = gray;
        }

    }
}

