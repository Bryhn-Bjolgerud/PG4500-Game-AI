using System.Collections.Generic;
using UnityEngine;

public class PathFollowing : MonoBehaviour
{
    /// <summary>
    /// Unlike the other steering behaviour scripts this does not return a force vector. For it to work it needs something externally to set the start waypoint for it. 
    /// Based on that start node it will move the Gameobject this script is a component of. This wasn't an explicit design choice, it just ended up this way and i didnt prioritize changing it over other issues.
    /// </summary>

    //The current waypoint we are seeking. This is public because it is set to what 
    public PathfindingCell currentWaypoint;
    //The position vector of the waypoint we are seeking.
    private Vector3 _target;
    //The seek script needing to move to each waypoint.
    private Seek _seek;
    //The rigidbody component needed to move the agent.
    private Rigidbody _rigidbody;
    //A list of all the different waypoints we will move through. Sorted so the first value is the goal.
    private List<PathfindingCell> _pathEndToStart;
    private float _speedForCurrentFrame = 3;
    //The amount of waypoints.
    private int _waypointCount;
    //The starting location the agent will move to before following a path.
    private Vector3 _startPos;
    private bool _startPosReached = false;

    private Vector3 _velocity;
    private Vector3 _rotation;
    private Quaternion _quRotation;

    /// <summary>
    /// Resetting the position and variables of the agent and setting it up so it can start another path following.
    /// </summary>
    public void resetAndPreparePathFollowingLion()
    {
        _pathEndToStart = new List<PathfindingCell>();
        _startPosReached = false;
        while (currentWaypoint.parent != null)
        {
            _pathEndToStart.Add(currentWaypoint);
            currentWaypoint = currentWaypoint.parent;
        }
        _waypointCount = _pathEndToStart.Count;
    }

    //Had to use awake instead of start. Could not find out the reason behind it (They essentially do the same so did not think too much of it).
    void Awake()
    {
        _startPos = new Vector3(-1.95f, -16.412f, -14f);
        currentWaypoint = new PathfindingCell();
        _seek = FindObjectOfType<Seek>();
        _pathEndToStart = new List<PathfindingCell>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //If the agent has not moved out of it's little area, it will continue moving towards it.
        if (!_startPosReached)
        {
            _velocity = _seek.getSeekDirection(_startPos, transform.localPosition);
            _velocity = _velocity / _rigidbody.mass;
            
            _rigidbody.velocity = _velocity * _speedForCurrentFrame;
            //Marking the start location as reached when the agent is close.
            if (Vector3.Distance(_startPos, transform.localPosition) < 0.2) {
                _startPosReached = true;
            }
        }
        //As long as there are more waypoints to travel to, it will continue moving.
        else if (_waypointCount >= 0)
        {
            //Every cell is 2.5 Unity measurements tall and wide. We dont want to move along the top right corners, so we recalculate the target location so it is the center of the cell.
            _target = currentWaypoint.cellInGame.transform.localPosition;
            _target.x = _target.x - 1.25f;
            _target.z = _target.z - 1.25f;
            if(_waypointCount > 0)
            {
                _velocity = _seek.getSeekDirection(_target, transform.localPosition);
            //We want to 'arrive' at the last waypoint. As the agent approaches the waypoint, the distance will be smaller and therfore the velocity will be less. The 5 will decide the how much the agent will slow down.
            } else if (_waypointCount == 0)
            {
                _velocity = _seek.getSeekDirection(_target, transform.localPosition) * (Vector3.Distance(transform.localPosition, currentWaypoint.cellInGame.transform.localPosition) / 5);
            }
            
            _rigidbody.velocity = _velocity * _speedForCurrentFrame;
            //If the agent is close to it's current taret and as long as there are more waypoints, we change the current one.
            if (Vector3.Distance(transform.localPosition, _target) < 0.2)
            {
                //Since the list of waypoints are sorted from end to start, we decrease our iterator instead of increasing it.
                _waypointCount--;
                //Dont want to change the current waypoint unless there are more.
                if (_waypointCount >= 0)
                {
                    currentWaypoint = _pathEndToStart[_waypointCount];
                }
            }
        //When there are no more waypoints, we want to stop.   
        } else {
            _rigidbody.velocity = new Vector3(0f, 0f, 0f);
        }

        _rotation = new Vector3(_velocity.x, 0f, _velocity.z);
        _quRotation = Quaternion.LookRotation(_rotation.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, _quRotation, Time.deltaTime * 10);
    }
}
