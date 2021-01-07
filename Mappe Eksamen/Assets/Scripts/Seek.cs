using UnityEngine;

public class Seek : MonoBehaviour
{
    /// <summary>
    /// Felt it was appropriate to make a seperate script for seeking, since it is an independent steering behaviour. This is not really needed because of how simple seek is, but i have made one for all the other behaviours.
    /// </summary>

    /// <summary>
    /// Based on the agents position and the targets position, calculates a force vector that will move the agent towards it's target.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="currentPos"></param>
    /// <returns>A force vector that moves the agent towards a target.</returns>
    public Vector3 getSeekDirection(Vector3 target, Vector3 currentPos)
    {
        Vector3 seekForce = (target - currentPos);
        return seekForce.normalized;
    }
}
