using UnityEngine;

public class CollisionAvoidance : MonoBehaviour
{
    private Vector3 _leftRay;
    private Vector3 _rightRay;

    /// <summary>
    /// Checking an area ahead of the agent if there are any Gameobjects there. 
    /// </summary>
    /// <returns>True or false whether or not there are Gameobjects ahead</returns>
    public bool collisionAhead()
    {
        bool collisionAhead = false;
        RaycastHit target;
        if(Physics.Raycast(transform.position, transform.forward, out target, 1.8f))
        {
            collisionAhead = true;
        } else if(Physics.Raycast(transform.position, _rightRay, out target, 1.4f))
        {
            collisionAhead = true;
        } else if(Physics.Raycast(transform.position, _leftRay, out target, 1.4f))
        {
            collisionAhead = true;
        }
        return collisionAhead;
    }

    // Update is called once per frame
    void Update()
    {
        _leftRay = Quaternion.AngleAxis(-40, transform.up) * transform.forward;
        _rightRay = Quaternion.AngleAxis(40, transform.up) * transform.forward;
    }
}
