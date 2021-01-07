using UnityEngine;

public class FoxWander : MonoBehaviour
{
    /// <summary>
    /// Since the wander script only returns a force vector, i needed a unique script for each of the agents that only wander, to be able to use that force vector.
    /// </summary>
    /// 

    private Vector3 _velocity;
    private Wander _wander;
    private Rigidbody _rigidbody;
    private Vector3 _rotation;
    private Quaternion _quRotation;

    void Start()
    {
        _wander = FindObjectOfType<Wander>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        _velocity = transform.forward.normalized;
        _velocity += _wander.getWanderDirection();

        _rigidbody.velocity = _velocity;
        _rotation = new Vector3(_velocity.x, 0f, _velocity.z);
        _quRotation = Quaternion.LookRotation(_rotation.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, _quRotation, Time.deltaTime * 10);

        //If the agent wanders outside of the arena we put it back on the opposite side. 
        if (transform.localPosition.x <= 3.15f)
        {
            transform.localPosition = new Vector3(18.5f, transform.localPosition.y, transform.localPosition.z);
        }
        else if (transform.localPosition.x >= 18.5f)
        {
            transform.localPosition = new Vector3(3.15f, transform.localPosition.y, transform.localPosition.z);
        }

        if (transform.localPosition.z >= 5.6f)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -10f);
        }
        else if (transform.localPosition.z <= -10f)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 5.6f);
        }
    }
}
