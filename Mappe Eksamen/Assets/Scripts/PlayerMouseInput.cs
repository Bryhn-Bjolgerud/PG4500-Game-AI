using UnityEngine;

public class PlayerMouseInput : MonoBehaviour
{
    /// <summary>
    /// This script is handling the player mouse input. It is responsible for doing the correct action corresponding to what the player is clicking on.
    /// </summary>

    //The script required to run the pathfinding.
    private AStarPathfinding _pathfinding;

    //All the different Gameobjects that we need to perform actions on.
    private GameObject[] _chickenOffsetPursuit;
    private GameObject[] _chickenEvade;
    private GameObject _chickenMom;
    private GameObject _wanderFox;
    private GameObject _lionPathFollowing;
    private GameObject _HSMCow;

    /// <summary>
    /// When the player left clicks with the mouse, checks what the player aims at and performs the corresponding action.
    /// </summary>
    void leftClick() {
        //Making a ray from the center of the screen and outwards.
        Ray directionILook = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit target;
        GameObject targetHit;

        //Removing the collider on the viewingplatforms momentarily so i can raycast through it (This is only used for the pathfinding portion).
        GameObject.FindGameObjectWithTag("ViewingPlatform").GetComponent<Collider>().enabled = false;
        if (Physics.Raycast(directionILook, out target))
        {
            //Cycling through the different colors the player can paint the pathfinding cells.
            if (target.collider.tag == "PathfindingGrid")
            {               
                targetHit = target.collider.gameObject;
                if (targetHit.GetComponent<Renderer>().material.color == _pathfinding.gray)
                {
                    targetHit.GetComponent<Renderer>().material.color = _pathfinding.blue;
                } else if (targetHit.GetComponent<Renderer>().material.color == _pathfinding.blue)
                {
                    targetHit.GetComponent<Renderer>().material.color = _pathfinding.green;
                } else if (targetHit.GetComponent<Renderer>().material.color == _pathfinding.green)
                {
                    targetHit.GetComponent<Renderer>().material.color = _pathfinding.red;
                } else if (targetHit.GetComponent<Renderer>().material.color == _pathfinding.red)
                {
                    targetHit.GetComponent<Renderer>().material.color = _pathfinding.gray;
                }               
            //Turning on the scripts when the player presses start. Turning them off and setting their velocity to zero when the player presses pause.
            } else if (target.collider.tag == "WanderOffsetPursuit")
            {
                _chickenMom.GetComponent<ChickenMomWander>().enabled = true;
                foreach (GameObject obj in _chickenOffsetPursuit) {
                    obj.GetComponent<OffsetPursuit>().enabled = true;
                }

            } else if (target.collider.tag == "StopWanderOffsetPursuit") {
                _chickenMom.GetComponent<ChickenMomWander>().enabled = false;
                _chickenMom.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
                foreach (GameObject obj in _chickenOffsetPursuit)
                {
                    obj.GetComponent<OffsetPursuit>().enabled = false;
                    obj.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
                }
            } else if (target.collider.tag == "WanderEvade") {
                _wanderFox.GetComponent<FoxWander>().enabled = true;
                foreach(GameObject obj in _chickenEvade)
                {
                    obj.GetComponent<Evade>().enabled = true;
                }

            } else if (target.collider.tag == "StopWanderEvade") {
                _wanderFox.GetComponent<FoxWander>().enabled = false;
                _wanderFox.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
                foreach (GameObject obj in _chickenEvade)
                {
                    obj.GetComponent<Evade>().enabled = false;
                    obj.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
                }
            //A player needs to have set atleast one start and one goal node for it to make a path.
            } else if (target.collider.tag == "PathFinding")
            {
                _pathfinding.getPath();
            } else if (target.collider.tag == "ClearGrid")
            {
                //Resetting both the pathfinding script and the path following lion.
                _pathfinding.clearGrid();
                _lionPathFollowing.GetComponent<PathFollowing>().enabled = false;
                _lionPathFollowing.transform.localPosition = new Vector3(-1.95f, -16.412f, -10.22f);
                _lionPathFollowing.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
                _lionPathFollowing.transform.rotation = new Quaternion(0f, 180f, 0f, 0f);
            //Changes the state of the HSMCow each time it is pressed.
            } else if (target.collider.tag == "StartInterposeHide")
            {
                _HSMCow.GetComponent<HierarchicalStateMachine>().stateDecider += 1;
                _HSMCow.GetComponent<HierarchicalStateMachine>().stateTransition();
            //Setting the state of the HSMCow to idle when the player wants to pause.
            } else if (target.collider.tag == "StopInterposeHide")
            {
                _HSMCow.GetComponent<HierarchicalStateMachine>().activeState = HierarchicalStateMachine.State.Idle;
            //Because the HSMCow somtimes would escape it's enclosure, this button will put it back in the middle.
            } else if(target.collider.tag == "ResetInterposeHide")
            {
                _HSMCow.transform.localPosition = new Vector3(-1.94f, -16.38f, -23.33f);
            }
        }
        //Enabling the collider again after the raycast has gone through so the player doesnt fall down into the enclosure. 
        GameObject.FindGameObjectWithTag("ViewingPlatform").GetComponent<Collider>().enabled = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        _chickenOffsetPursuit = GameObject.FindGameObjectsWithTag("ChickenOffsetPursuit");
        _chickenEvade = GameObject.FindGameObjectsWithTag("ChickenEvade");
        _chickenMom = GameObject.FindGameObjectWithTag("ChickenMom");
        _wanderFox = GameObject.FindGameObjectWithTag("Fox");
        _lionPathFollowing = GameObject.FindGameObjectWithTag("LionPathFollowing");
        _HSMCow = GameObject.FindGameObjectWithTag("HSMCow");
        _pathfinding = FindObjectOfType<AStarPathfinding>();
    }

    // Update is called once per frame
    void Update()
    {
        //If the player presses left click we need to call the function to handle that input.
        if (Input.GetMouseButtonDown(0))
        {
            leftClick();
        }
    }
}
