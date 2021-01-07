using UnityEngine;

public class OffsetPursuit : MonoBehaviour
{
    //The Gameobject we are evading.
    private GameObject _GameObjectWeReactTo;
    private float _speedForCurrentFrame = 2;
    //The velocity of the agent.
    private Vector3 _velocity;
    //To avoid having to GetComponent() each time we want to acess the Rigidbody we store it once.
    private Rigidbody _rigidbody;
    //The seek script used to pursue the target location.
    private Seek _seek;

    //A list of the other Gameobjects also offsett pursuing, excluding this.Gameobject.
    private GameObject[] _listOfChickens;


    private Vector3 _rotation;
    private Quaternion _quRotation;

    /// <summary>
    /// Calculates the location the agent will seek. This location will allways be behind the gameobject we are pursuing.
    /// </summary>
    /// <returns>The position vector of the calculated location.</returns>
    public Vector3 getPositionToSeek()
    {
        //Taking the target's position and adding to that it's negated velocity. This will ensure us that this position is allways in the opposite direction of what the target is currently traveling.
        Vector3 targetFuturePosition = _GameObjectWeReactTo.transform.localPosition + -_GameObjectWeReactTo.GetComponent<Rigidbody>().velocity;
        return targetFuturePosition;
    }

    /// <summary>
    /// Calculates a unique seperation force for the different agents. Since they are all seeking the same position, we need a force that keeps them apart so they dont stack up.
    /// </summary>
    /// <returns>A vector that will seperate this.agent from the rest. If there aren't any neighbours return a zero vector.</returns>
    public Vector3 getSeperationForce()
    {
        Vector3 seperationForce = new Vector3();
        int neighbourCount = 0;

        //Going through every agent that isn't ourselves and adding their positions together. That gives us the center of mass of our "formation". 
        //We did not use our position when calculating that center, but we know we are apart of the formation. If we then negate the position vector we got, we get a vector pointing to a spot not occupied by another chicken.
        //Since every chicken is doing this for itself, we can align them with regards to each others positions. 
        foreach (GameObject chicken in _listOfChickens)
        {
            if (gameObject != chicken)
            {
                if (Vector3.Distance(transform.localPosition, chicken.transform.localPosition) < 3)
                {
                    seperationForce.x += chicken.transform.localPosition.x - gameObject.transform.localPosition.x;
                    seperationForce.z += chicken.transform.localPosition.z - gameObject.transform.localPosition.z;
                    neighbourCount++;
                }
            }
        }

        //If we found neighbouring agents we need to scale the seperation force based on the amount of neighbours.        
        seperationForce.x = seperationForce.x / neighbourCount;
        seperationForce.z = seperationForce.z / neighbourCount;
        //Negating the force.
        seperationForce = -seperationForce;

        //Normalizing the force, and then scaling it alittle to make it a tad stronger.
        seperationForce = seperationForce.normalized;
        seperationForce.x = seperationForce.x * 1.2f;
        seperationForce.z = seperationForce.z * 1.2f;
        return seperationForce;
    }

    /// <summary>
    /// If the agent isn't close to the target we calculate the regular seek force, but if the agent is close we need to calculate a new arrival force. 
    /// The purpose of the arrival force is to slow the agent down when it is close to reaching it's target, so it doesnt abruptly stop.
    /// </summary>
    /// <returns>A velocity vector that will move the target towards it's location and slow down when it comes close.</returns>
    public Vector3 getSeekArrivalForce()
    {
        Vector3 seekArrivalForce = new Vector3(0f, 0f, 0f);

        if (Vector3.Distance(transform.localPosition, getPositionToSeek()) < 3)
        {
            //If the agent is close we scale the seek force with the distance between the two. When the distance is lower, the force will be lower, resulting in the agent slowing down.
            seekArrivalForce = (_seek.getSeekDirection(getPositionToSeek(), transform.localPosition)) * Vector3.Distance(transform.localPosition, getPositionToSeek() / 5);
        }
        else
        {
            seekArrivalForce = _seek.getSeekDirection(getPositionToSeek(), transform.localPosition);
        }
        return seekArrivalForce.normalized;
    }

    // Start is called before the first frame update
    void Start()
    {
        _GameObjectWeReactTo = GameObject.FindGameObjectWithTag("ChickenMom");
        _listOfChickens = GameObject.FindGameObjectsWithTag("ChickenOffsetPursuit");
        _rigidbody = GetComponent<Rigidbody>();
        _seek = FindObjectOfType<Seek>();
    }

    // Update is called once per frame
    void Update()
    {
        _velocity = _rigidbody.transform.forward.normalized;
        _velocity = _velocity + getSeekArrivalForce();
        _velocity = _velocity + getSeperationForce();

        _rotation = new Vector3(_velocity.x, 0f, _velocity.z);
        _quRotation = Quaternion.LookRotation(_rotation.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, _quRotation, Time.deltaTime * 10);

        _rigidbody.velocity = _velocity * _speedForCurrentFrame;

        //Checking to see if the chickens leave the arena, if they do we set them back to the other side of the arena.
        if (transform.localPosition.x <= -3.3f)
        {
            transform.localPosition = new Vector3(11.8f, transform.localPosition.y, transform.localPosition.z);
        }
        else if (transform.localPosition.x >= 11.8f)
        {
            transform.localPosition = new Vector3(-3.3f, transform.localPosition.y, transform.localPosition.z);
        }

        if (transform.localPosition.z >= 8.5f)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -6.72f);
        }
        else if (transform.localPosition.z <= -6.72f)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 8.5f);
        }
    }
}
