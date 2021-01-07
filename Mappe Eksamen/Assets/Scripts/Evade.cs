using UnityEngine;

public class Evade : MonoBehaviour
{
    /// <summary>
    /// The difference between 'flee' and 'evade' is not that big, this script could very easily be turned into one that flees a gameobject.
    /// If an agent were to flee a gameobject it would move away from the current position of that gameobject. When evading it calculates the future position of the gameobject, and avoids that.
    /// This means that when evading you could techinally collide with the enemy, which in a real life scenario wouldn't be optimal.
    /// </summary>

    private GameObject _GameObjectWeReactTo;
    private float _speedForCurrentFrame = 2;
    private Rigidbody _rigidbody;
    private Vector3 _baseVelocity;
    private Vector3 _rotation;
    private Quaternion _quRotation;

    /// <summary>
    /// If the agent is close to the gameobject it wants to evade, it will calculate a force vector that will move the agent out of the way, otherwise that vector is zero.
    /// </summary>
    /// <returns>A force vector that will move the agent away from whatever it is evading.</returns>
    public Vector3 getEvadeForce()
    {
        Vector3 targetFuturePosition = new Vector3(0f, 0f, 0f);
        if(Vector3.Distance(_GameObjectWeReactTo.transform.localPosition + _GameObjectWeReactTo.GetComponent<Rigidbody>().velocity * 2, transform.localPosition) < 2)
        {
            targetFuturePosition = transform.localPosition - _GameObjectWeReactTo.transform.localPosition + _GameObjectWeReactTo.GetComponent<Rigidbody>().velocity * 2;
        }

        return targetFuturePosition.normalized;
    }

    void Start()
    {
        _GameObjectWeReactTo = GameObject.FindGameObjectWithTag("Fox");
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        _baseVelocity = _rigidbody.transform.forward.normalized;
        _baseVelocity = _baseVelocity + getEvadeForce();

        _rotation = new Vector3(_baseVelocity.x, 0f, _baseVelocity.z);
        _quRotation = Quaternion.LookRotation(_rotation.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, _quRotation, Time.deltaTime * 10);

        _rigidbody.velocity = _baseVelocity * _speedForCurrentFrame;

        //Checking if the chickens leave the arena, if they do we set them back to the other side.
        if (transform.localPosition.x <= 3.25f)
        {
            transform.localPosition = new Vector3(18.4f, transform.localPosition.y, transform.localPosition.z);
        }
        else if (transform.localPosition.x >= 18.4f)
        {
            transform.localPosition = new Vector3(3.25f, transform.localPosition.y, transform.localPosition.z);
        }
            
        if (transform.localPosition.z >= 5.5f)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -9.7f);
        }
        else if (transform.localPosition.z <= -9.7f)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 5.5f);
        }
    }
}
