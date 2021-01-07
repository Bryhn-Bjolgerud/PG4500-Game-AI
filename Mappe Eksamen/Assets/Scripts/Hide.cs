using UnityEngine;
using Random = System.Random;
public class Hide : MonoBehaviour
{
    /// <summary>
    /// This script creates an enemy and an obstacle placed random on the grid. Based on the position of those calculates a location that will hide the agent from the line of sight of the enemy.
    /// To perform the actual hiding, we combine this script with the seek script.
    /// </summary>

    //All the cells on the grid represented as both a list and 2d array.
    private GameObject[] _ihList;
    private GameObject[,] _ihGrid;
    private Random _rnd;
    //The position that is hidden from the "enemy".
    private Vector3 _hideLocation;

    //A number chosen at random, that will decide if the obstacle is horizontal or vertical.
    private int _rowOrColumn;
    //The row or column the obstacle will the made on.
    private int _rowColumnPicker;
    //Where on the row or column the obstacle will start.
    private int _wallStartPos;
    //The distance the enemy is placed from the obstacle.
    private int _distanceBetweenEnemyAndWall;

    /// <summary>
    /// Draws a three cell wide/tall obstacle and places an enemy at a random distance away from that obstacle. The obstacle will never be created in such a fasion that there is no way to hide behind it*.
    /// *It can never be on the edge of the arena
    /// </summary>
    public void setHidecell()
    {
        Vector3 enemyPosition = new Vector3(0f, 0f, 0f);
        //The center of the obstacle.
        Vector3 centerWallPosition = new Vector3(0f, 0f, 0f);
        _ihGrid = new GameObject[12, 12];
        foreach (GameObject obj in _ihList)
        {
            obj.GetComponent<Renderer>().material.color = Color.green;
        }

        int k = 0;
        for (int i = 1; i < 11; i++)
        {
            for (int j = 1; j < 11; j++)
            {
                _ihGrid[i, j] = _ihList[k];
                k++;
            }
        }

        //Setting all the random values.
        _rowOrColumn = _rnd.Next(0, 2);
        _rowColumnPicker = _rnd.Next(2, 10);
        _wallStartPos = _rnd.Next(2, 7);
        _distanceBetweenEnemyAndWall = _rnd.Next(1, 11);

        //We need to draw a horizontal obstacle.
        if (_rowOrColumn == 0)
        {
            //As long as the row of the enemy and obstacle isn't the same, we place the enemy at the random position.
            if (_distanceBetweenEnemyAndWall != _rowColumnPicker)
            {
                _ihGrid[_distanceBetweenEnemyAndWall, _wallStartPos + 1].GetComponent<Renderer>().material.color = Color.blue;
                enemyPosition = _ihGrid[_distanceBetweenEnemyAndWall, _wallStartPos + 1].transform.localPosition;
            }
            //If the row that the enemy is supposed to be placed on is the same as the row we draw the obstacle on, we will override the location and place the enemy one cell away.
            else
            {
                _ihGrid[_rowColumnPicker + 1, _wallStartPos + 1].GetComponent<Renderer>().material.color = Color.blue;
                enemyPosition = _ihGrid[_rowColumnPicker + 1, _wallStartPos + 1].transform.localPosition;
            }

            //Based on the random values decided we draw the obstacle.
            _ihGrid[_rowColumnPicker, _wallStartPos].GetComponent<Renderer>().material.color = Color.red;
            _ihGrid[_rowColumnPicker, _wallStartPos + 1].GetComponent<Renderer>().material.color = Color.red;
            _ihGrid[_rowColumnPicker, _wallStartPos + 2].GetComponent<Renderer>().material.color = Color.red;
            centerWallPosition = _ihGrid[_rowColumnPicker, _wallStartPos + 1].transform.localPosition;
        }
        //We need to draw a verical obstacle.
        else if (_rowOrColumn == 1)
        {
            //Same logic as with the row, except for a column.

            if (_distanceBetweenEnemyAndWall != _rowColumnPicker)
            {
                _ihGrid[_wallStartPos + 1, _distanceBetweenEnemyAndWall].GetComponent<Renderer>().material.color = Color.blue;
                enemyPosition = _ihGrid[_wallStartPos + 1, _distanceBetweenEnemyAndWall].transform.localPosition;
            }
            else
            {
                _ihGrid[_wallStartPos + 1, _rowColumnPicker + 1].GetComponent<Renderer>().material.color = Color.blue;
                enemyPosition = _ihGrid[_wallStartPos + 1, _rowColumnPicker + 1].transform.localPosition;
            }
            _ihGrid[_wallStartPos, _rowColumnPicker].GetComponent<Renderer>().material.color = Color.red;
            _ihGrid[_wallStartPos + 1, _rowColumnPicker].GetComponent<Renderer>().material.color = Color.red;
            _ihGrid[_wallStartPos + 2, _rowColumnPicker].GetComponent<Renderer>().material.color = Color.red;
            centerWallPosition = _ihGrid[_wallStartPos + 1, _rowColumnPicker].transform.localPosition;
        }

        //Depending on if the enemies position is above, under, left or right of the obstacle, we know what position will be out of the line of sight of the enemy.
        if (enemyPosition.x > centerWallPosition.x)
        {
            _hideLocation = new Vector3(centerWallPosition.x - 1.25f, -16.4f, centerWallPosition.z - 1.25f);
            _hideLocation.x -= 2.5f;
        }
        if (enemyPosition.x < centerWallPosition.x)
        {
            _hideLocation = new Vector3(centerWallPosition.x - 1.25f, -16.4f, centerWallPosition.z - 1.25f);
            _hideLocation.x += 2.5f;
        }
        if (enemyPosition.z > centerWallPosition.z)
        {
            _hideLocation = new Vector3(centerWallPosition.x - 1.25f, -16.4f, centerWallPosition.z - 1.25f);
            _hideLocation.z -= 2.5f;
        }
        if (enemyPosition.z < centerWallPosition.z)
        {
            _hideLocation = new Vector3(centerWallPosition.x - 1.25f, -16.4f, centerWallPosition.z - 1.25f);
            _hideLocation.z += 2.5f;
        }
        GameObject.FindGameObjectWithTag("Finish").transform.localPosition = _hideLocation;
    }

    public Vector3 getHideLocation()
    {
        return _hideLocation;
    }

    // Start is called before the first frame update
    void Start()
    {   
        //Used FindObjectGameobjectsWithTag but that did not retrive the gameobjects in the same order when playing the game from the editor and a built version. 
        //Had to order the cells and get them through the parent instead.
        _ihList = new GameObject[100];
        for (int i = 0; i < 100; i++)
        {
            _ihList[i] = GameObject.Find("InterposeHideEnclosure").transform.GetChild(i).gameObject;
        }
        _rnd = new Random();
    }
}
