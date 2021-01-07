using UnityEngine;
using Random = System.Random;

public class Interpose: MonoBehaviour
{
    /// <summary>
    /// This script sets two random cells on the grid. Based on the location of those two, it then calculates the location in the middle (Where the agent needs to move to interpose).
    /// </summary>

    //All the cells as a list.
    private GameObject[] ihList;
    private Random rnd;
    //The location that is in the middle of the two cells we want to interpose.
    private Vector3 _interposeLocation;

    /// <summary>
    /// Sets two random cell on the grid to be the locations the agent needs to interpose. They cannot be the same location, but they can be right next to each other.
    /// </summary>
    public void setInterposeCells()
    {
        foreach (GameObject obj in ihList)
        {
            obj.GetComponent<Renderer>().material.color = Color.green;
        }
        //Gets a random index from the list of all cells.
        int cellOneIndex = rnd.Next(ihList.Length);
        int cellTwoIndex = rnd.Next(ihList.Length);

        //If the indexes are the same, we switch the second one.
        if (cellOneIndex == cellTwoIndex)
        {
            if (cellOneIndex == 0)
            {
                cellTwoIndex++;
            }
            else
            {
                cellTwoIndex--;
            }
        }

        ihList[cellOneIndex].GetComponent<Renderer>().material.color = Color.blue;
        ihList[cellTwoIndex].GetComponent<Renderer>().material.color = Color.blue;

        //Calculating the middle point between the two locations.
        _interposeLocation.x = ((ihList[cellOneIndex].transform.localPosition.x - 1.25f) + (ihList[cellTwoIndex].transform.localPosition.x - 1.25f)) / 2;
        _interposeLocation.z = ((ihList[cellOneIndex].transform.localPosition.z - 1.25f) + (ihList[cellTwoIndex].transform.localPosition.z - 1.25f)) / 2;
        _interposeLocation.y = -16.4f;
        GameObject.FindGameObjectWithTag("Finish").transform.localPosition = _interposeLocation;
    }

    public Vector3 getInterposeLocation()
    {
        return _interposeLocation;
    }

    // Start is called before the first frame update
    void Start()
    {
        ihList = GameObject.FindGameObjectsWithTag("InterposeHideGrid");
        rnd = new Random();
    }
}
