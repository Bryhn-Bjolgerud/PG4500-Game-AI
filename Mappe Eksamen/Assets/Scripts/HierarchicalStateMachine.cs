using UnityEngine;

public class HierarchicalStateMachine : MonoBehaviour
{
    /// There are different ways to design a hierarchical state machine, this one has one finite state machine nested in another to achieve the hierarchy.
    

    //The different states the machine can be in.
    public enum State2
    {
        Collision = 1,
        Ordinary
    }

    public enum State
    {
        Idle = 1,
        Wander,
        Interpose,
        Hide
    }

    //The active and previous state of both of the state machines.
    public State2 _activeState2 = State2.Ordinary;
    private State2 _previousState2;
    public State activeState = State.Idle;
    private State _previousState;

    //The different scripts used to do all the state actions.
    private CollisionAvoidance _collisionAvoidance;
    private Interpose _interpose;
    private Hide _hide;
    private Wander _wander;
    private Seek _seek;

    //The Rigidbody component of the gameobject. More effecient to get it once, instead of through GetComponent<>() each time.
    private Rigidbody _rigidbody;

    //The speed at which the agents move. Multiplying it to all the different steering behaviour forces.
    private float _speedForCurrentFrame = 3;

    //Two different locations used to check if the agent has reached it's target.
    private Vector3 _interposeLocation;
    private Vector3 _hideLocation;

    //Depending on whether this is an odd or even number the state will change. 
    public int stateDecider = 0;

    //The Vector and Quaternion used to change the direction the agent is looking.
    private Vector3 _rotation;
    private Quaternion _quRotation;

    /// <summary>
    /// Sets the state of the machine to corresponding to the what condition is met.
    /// </summary>
    public void stateTransition2 ()
    {
        //If there is an obstacle close in front of the agent this if statement is true.
        if(_collisionAvoidance.collisionAhead())
        {
            _activeState2 = State2.Collision;
        } else 
        {
            _activeState2 = State2.Ordinary;
        }
    }

    /// <summary>
    /// Does all the nonrepeated tasks required for the state, each time the machine switches states.
    /// </summary>
    public void stateEnter2 ()
    {
        //We only want these actions to be done once each time we enter the state.
        if (_activeState2 != _previousState2)
        {
            if(_activeState2 == State2.Collision)
            {
                if (_wander.collision)
                {
                    _wander.collision = false;
                }
                else
                {
                    _wander.collision = true;
                }
            }

            _previousState2 = _activeState2;
        }
    }

    /// <summary>
    /// Does all the actions for the active state. Called once every update.
    /// </summary>
    public void stateAction2 ()
    {
        if(_activeState2 == State2.Ordinary)
        {
            //This is the other state machine. It can only be accessed when this machine is in "Ordinary". This way we achieve a hierarchy.
            stateEnter();
            stateAction();
        } else
        {
            _rigidbody.velocity = _wander.getWanderDirection() * _speedForCurrentFrame;
        }
    }

    /// <summary>
    /// Sets the state of the machine to corresponding to the what condition is met. 
    /// This function is called by the player.
    /// </summary>
    public void stateTransition ()
    {   
        if (stateDecider % 2 == 0)
        {
            activeState = State.Interpose;
        }
        else if (stateDecider % 2 == 1)
        {
            activeState = State.Hide;
        }
    }

    public void stateEnter ()
    {
        if(activeState != _previousState)
        {
            if (activeState == State.Idle)
            {
                _rigidbody.velocity = new Vector3(0f, 0f, 0f);
            } else if (activeState == State.Interpose)
            {
                _interpose.setInterposeCells();
                _interposeLocation = _interpose.getInterposeLocation();
            } else if (activeState == State.Hide)
            {
                _hide.setHidecell();
                _hideLocation = _hide.getHideLocation();
            }
            _previousState = activeState;
        }
    }

    public void stateAction ()
    {
        if(activeState == State.Wander)
        {
            _rigidbody.velocity = transform.forward.normalized;
            _rigidbody.velocity += _wander.getWanderDirection() * _speedForCurrentFrame;
        } else if(activeState == State.Interpose)
        {
            _rigidbody.velocity = _seek.getSeekDirection(_interposeLocation, transform.localPosition) * _speedForCurrentFrame;
            //If the agent has reached it's target, it will instantly switch to wandering.
            if (Vector3.Distance(transform.localPosition, _interposeLocation) < 0.65)
            {
                _rigidbody.velocity = new Vector3(0f, 0f, 0f);
                activeState = State.Wander;
            }
        } else if (activeState == State.Hide)
        {
            _rigidbody.velocity = _seek.getSeekDirection(_hideLocation, transform.localPosition) * _speedForCurrentFrame;
            //If the agent has reached it's target, it will instantly switch to wandering.
            if (Vector3.Distance(transform.localPosition, _hideLocation) < 0.65)
            {
                _rigidbody.velocity = new Vector3(0f, 0f, 0f);
                activeState = State.Wander;
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        //Getting all the components once when we start, so we dont have to retrive them each time we need them since they are relatively high cost tasks.
        _collisionAvoidance = GetComponent<CollisionAvoidance>();
        _interpose = GetComponent<Interpose>();
        _hide = GetComponent<Hide>();
        _wander = GetComponent<Wander>();
        _rigidbody = GetComponent<Rigidbody>();
        _seek = GetComponent<Seek>();
    }

    // Update is called once per frame
    void Update()
    {
        //Running HSM.
        stateTransition2();
        stateEnter2();
        stateAction2();

        //Changing the rotation of the agent to match it's velocity, so it looks in the same direction it moves.
        _rotation = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);
        _quRotation = Quaternion.LookRotation(_rotation.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, _quRotation, Time.deltaTime * 10);
    }
}
