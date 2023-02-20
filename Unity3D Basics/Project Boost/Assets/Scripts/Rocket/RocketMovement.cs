using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class RocketMovement : MonoBehaviour {

    private Rigidbody _rigidbody;
    
    public float rocketEngineThrust = 1000;
    public float rotationThrust = 100;
    public float airDrag = 0.25f; // same as air resistance, makes the rocket "slower"
    public Vector3 gravity = new Vector3(0, -9.81f, 0); // default gravity
    private readonly Vector3 _moveDirection = Vector3.up;
    public Vector3 MoveDirection => _moveDirection;
    
    //processed by the  ProcessKeyboardInputs()/RotateRocket() and trigger system on editor
    public static bool IsThrusting { get; private set; }
    public static bool IsRotatingLeft { get; set; }
    public static bool IsRotatingRight { get; set; }
    public static bool IsGonnaResetPos { get; set; } = false;
    
    private void Start() {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.drag = airDrag;
        Physics.gravity = gravity;
    } 
    
    private void Update() {
        ProcessKeyboardInputs();
        MoveRocket();
        RotateRocket();
        
        if (!IsGonnaResetPos) return;
        ResetRocketBackToInitialPosition();
        IsGonnaResetPos = false;
    }
    
    private void MoveRocket() {
        if (!IsThrusting) return;
        // adds force based on the object's coordinates coordinates instead of the world's, so, if direction is up,
        // than it's up relative to the object's rotation (where its "head" is pointing), instead of the world y axis
        Vector3 force = rocketEngineThrust * Time.deltaTime * _moveDirection ;
        _rigidbody.AddRelativeForce(force);
    }
    
    private void RotateRocket() {
        /*  OLD SOLUTION: angular velocity gets some inertia when the player rotates too fast, and it makes the rocket rotate by itself, 
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

    private void ResetRocketBackToInitialPosition() {
        // kills the inertia first, otherwise it would keep moving
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        // kills any user's input potential noise
        IsThrusting = false;
        IsRotatingLeft = false;
        IsRotatingRight = false;
        // resets the scene
        SceneManager.LoadScene("Sandbox");
    }
    
    private static void ProcessKeyboardInputs() {
        
        ProcessThrustInputs();
        ProcessRotationInputs();
        ProcessRotationInputs();
        ProcessResetInput();
        
        void ProcessThrustInputs() {
            if (Input.GetKey(KeyCode.Space)) IsThrusting = true;
            if (Input.GetKeyUp(KeyCode.Space)) IsThrusting = false;
        }
        void ProcessRotationInputs() {
            // can't rotate to left and right at the same time
            if (Input.GetKey(KeyCode.A)) IsRotatingLeft = true;
            else if (Input.GetKey(KeyCode.D)) IsRotatingRight = true;
            if (Input.GetKeyUp(KeyCode.A)) IsRotatingLeft = false;
            if (Input.GetKeyUp(KeyCode.D)) IsRotatingRight = false;
        }
        void ProcessResetInput() {
            if (Input.GetKey(KeyCode.R)) IsGonnaResetPos = true;
        }
    }
    
}
