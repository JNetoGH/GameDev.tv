using UnityEngine;

public sealed class RocketMovement : MonoBehaviour
{

    private Rigidbody _rigidbody;
    
    public float rocketEngineThrust = 1000;
    public float rotationThrust = 100;
    public float airDrag = 0.25f; // same as air resistance, makes the rocket speed down
    public Vector3 gravity = new Vector3(0, -9.81f, 0); // default gravity
    private Vector3 _initialPos;
    private readonly Vector3 _moveDirection = Vector3.up;
    public Vector3 MoveDirection => _moveDirection;
    
    //processed by the ProcessInputs() and trigger system on editor
    public bool IsThrusting { get; set; }
    public bool IsRotatingLeft { get; set; }
    public bool IsRotatingRight { get; set; }
    public bool IsGonnaResetPos { get; set; } = false;
    
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.drag = airDrag;
        _initialPos = transform.position;
        Physics.gravity = gravity;
    } 
    
    private void Update()
    {
        ProcessKeyboardInputs();
        MoveRocket();
        RotateRocket();
        //PrintDebugInfo();
        if (IsGonnaResetPos)
        {
            SetRocketBackToInitialPosition();
            IsGonnaResetPos = false;
        }
    }
    
    private void MoveRocket()
    {
        if (IsThrusting)
        {
            // adds force based on the obj pos not based on the world pos
            Vector3 force = _moveDirection * rocketEngineThrust * Time.deltaTime;
            _rigidbody.AddRelativeForce(force);
        }
    }
    
    private void RotateRocket()
    {
        /*  OLD SOLUTION
        angular velocity gets some inertia when the player rotates too fast, and it makes the rocket rotate by itself, 
        I want this effect only when the player collides against stuff, so, i'm only setting to zero when the player
        is controlling, this is important in order to make the rocket spin around when it collides.
        if (IsRotatingLeft || IsRotatingRight) _rigidbody.angularVelocity = Vector3.zero; */
        
        /*this one works better, simply doesn't allow the physics engine to rotate the rocket while the player is rotating,
        but allows to rotate in z when the player isn't controlling, so, when the player crashes against any obstacle,
        it will spin in the Z Axis, if the player is not controlling the rotation*/
        if (IsRotatingLeft || IsRotatingRight)
            _rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        else
            _rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        
        // z == 1 == left direction | z == -1 == right direction, the rocket can't rotate to left and right at the same time
        if (IsRotatingLeft) transform.Rotate(0, 0, 1 * rotationThrust * Time.deltaTime);
        else if (IsRotatingRight) transform.Rotate(0,0,-1 * rotationThrust * Time.deltaTime);
    }

    public void SetRocketBackToInitialPosition()
    {
        // kills the inertia first, otherwise it would keep moving
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        
        // sets back to initial pos
        transform.rotation = Quaternion.Euler(0,0,0);
        transform.position = _initialPos;
    }
    
    private void ProcessKeyboardInputs()
    {
        ProcessThrustInputs();
        ProcessRotationInputs();
        ProcessResetInput();
        
        void ProcessThrustInputs()
        {
            if (Input.GetKey(KeyCode.Space)) IsThrusting = true;
            if (Input.GetKeyUp(KeyCode.Space)) IsThrusting = false;
        }
        void ProcessRotationInputs()
        {
            // can't rotate to left and right at the same time
            if (Input.GetKey(KeyCode.A)) IsRotatingLeft = true;
            else if (Input.GetKey(KeyCode.D)) IsRotatingRight = true;

            if (Input.GetKeyUp(KeyCode.A)) IsRotatingLeft = false;
            if (Input.GetKeyUp(KeyCode.D)) IsRotatingRight = false;
        }
        void ProcessResetInput()
        {
            if (Input.GetKey(KeyCode.P)) IsGonnaResetPos = true;
        }
    }
    
    private void PrintDebugInfo()
    {
        Debug.Log($"thrust: {IsThrusting}");
        Debug.Log($"RL: {IsRotatingLeft} | RR: {IsRotatingRight}");
    }
    
}
