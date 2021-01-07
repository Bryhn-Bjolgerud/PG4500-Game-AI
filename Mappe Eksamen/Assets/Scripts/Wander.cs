using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Wander : MonoBehaviour
{
    // The distance from the position of the gameobject to it's guide circle.
    private float circleDistance = 2.0f;
    // The radius of the guide circle.
    private float circleRadius = 3.0f;
    //The direction we will move.
    private float wanderAngle = 90f;
    //The amount we want our direction to change for each frame.
    private float angleChange = 0.3f;
    //To check if a collision has happened.
    public bool collision;

    public Vector3 getWanderDirection() {
        // Depending on the distance between the circle we use to get our 'wander' and the wanderee's position, we will get a stronger pull in the given direction. 
        // The greater the vector between the wanderee and the circle the more we will be moved each frame resulting in faster (stronger) movement. 
        Vector3 circleCenter = Vector3.forward * circleDistance;

        // Basing our displacement force on our objects z-axis (birds eye pov 2d pov would be it's y axis). Basically the direction the game object is facing. 
        Vector3 displacement = new Vector3(0f, 0f, 1f);
        displacement = displacement * circleRadius;

        // To make this steering behaviour actually wander seemlessly around, we need to change the angle of our direction with each frame. 
        float length = displacement.magnitude;
        displacement.x = Mathf.Cos(wanderAngle) * length;
        displacement.z = Mathf.Sin(wanderAngle) * length;
        wanderAngle += (Random.value * angleChange) - (angleChange * 0.5f);

        // Adding the distance from the guide circle and the gameobject together with the displacement force, to get the wanderforce. The vector that we will actually move along.
        Vector3 wanderForce = (displacement + circleCenter);

        //This bool gets changed each time a collision happens. Everytime an agent collides it needs to travel in opposite direction.
        if (collision)
        {
            wanderForce = -wanderForce;
        }
        return wanderForce.normalized;
    }
}
