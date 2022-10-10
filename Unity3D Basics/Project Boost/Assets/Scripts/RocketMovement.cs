using UnityEngine;

public class RocketMovement : MonoBehaviour
{

    private Rigidbody _rigidbody;
    
    public float mainThrust = 1000;
    public float rotationThrust = 100;
    public float airDrag = 0.25f; // same as air resistance, makes the rocket speed down
    public Vector3 gravity = new Vector3(0, -9.81f, 0); // default gravity
    
    //processed by the ProcessInputs()
    public bool IsThrusting { get; private set; }
    public bool IsRotatingLeft { get; private set; }
    public bool IsRotatingRight { get; private set; }

    private Vector3 _initialPos; 
    
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.drag = airDrag;
        _initialPos = transform.position;
        Physics.gravity = gravity;
    } 
    
    private void Update()
    {
        ProcessInputs();
        MoveRocket();
        RotateRocket();
        PrintDebugInfo();
        if (Input.GetKey(KeyCode.P))
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            transform.rotation = Quaternion.Euler(0,0,0);
            transform.position = _initialPos;
        }
    }
    
    private void MoveRocket()
    {
        // adds force based on the obj pos not based on the world pos
        Vector3 force = Vector3.up * mainThrust * Time.deltaTime;
        if (IsThrusting) _rigidbody.AddRelativeForce(force);
    }
    
    private void RotateRocket()
    {
        
        /*  OLD SOLUTION
        angular velocity gets some inertia when the player rotates too fast, and it makes the rocket rotate by itself, 
        I want this effect only when the player collides against stuff, so, i'm only setting to zero when the player
        is controlling, this is important in order to make the rocket spin around when it collides.
        if (IsRotatingLeft || IsRotatingRight) _rigidbody.angularVelocity = Vector3.zero; */
        
        // this one also works, simply doesn't allow the physics engine to rotate the rocket while the player is rotating
        _rigidbody.freezeRotation = IsRotatingLeft || IsRotatingRight; // it receives a boolean as value

        // z == 1 == left direction | z == -1 == right direction
        if (IsRotatingLeft) transform.Rotate(0, 0, 1 * rotationThrust * Time.deltaTime);
        if (IsRotatingRight) transform.Rotate(0,0,-1 * rotationThrust * Time.deltaTime);
    }
    
    private void ProcessInputs()
    {
        ProcessThrustInputs();
        ProcessRotationInputs();
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
    }
    
    private void PrintDebugInfo()
    {
        Debug.Log($"thrust: {IsThrusting}");
        Debug.Log($"RL: {IsRotatingLeft} | RR: {IsRotatingRight}");
    }
    
}
