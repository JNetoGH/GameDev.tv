using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class RocketController : MonoBehaviour {
    
    // Movement and Rotation
    [SerializeField] public float rocketEngineThrust = 1000;
    [SerializeField] public float rotationThrust = 100;
    [SerializeField] public static Vector3 MoveDirection => Vector3.up;
    [SerializeField] public float airDrag = 0.25f; // same as air resistance, makes the rocket "slower"
    [SerializeField] public Vector3 gravity = new (0, -9.81f, 0); // default gravity
    
    // Rocket's Audio Clips
    [SerializeField] private AudioClip engineAudioClip;
    [SerializeField] private AudioClip explosionAudioClip;

    // Cached Components
    private Rigidbody _rigidbody;
    private AudioSource _audioSource;

    // States (rocket's current state)
    internal static bool HasRocketExploded { get; private set; } = false;
    internal static bool HasWon { get; set; } = false;
    internal static bool IsThrusting { get; private set; }
    internal static bool IsRotatingLeft { get; set; }
    internal static bool IsRotatingRight { get; set; }

    private void Start() {
        
        // States
        HasRocketExploded = false;
        HasWon = false;
        IsThrusting = false;
        IsRotatingLeft = false;
        IsRotatingRight = false;

        // Audio
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = engineAudioClip;
        _audioSource.loop = true;
        _audioSource.mute = false;
        _audioSource.Play();
        
        // Rigidbody & Physics
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.drag = airDrag;
        Physics.gravity = gravity;
    }

    private void Update() {
        if (HasRocketExploded || HasWon) 
            return;
        _audioSource.mute = !IsThrusting;
        ProcessKeyboardInputs();
    } 

    private void FixedUpdate() {
        if (HasRocketExploded || HasWon) 
            return;
        MoveRocket();
        RotateRocket();
    }

    //! Manages all the collisions that are not on the landing pad, there is a special script for managing this case
    private void OnCollisionEnter(Collision collision) {
        
        // Simply ignores the collision if it’s the Landing Pad or whether the player has won and is loading the next level
        if (collision.gameObject.tag.Equals("LandingPad") || HasWon) 
            return;
        
        switch (collision.gameObject.tag) {
            case "Friendly":
                Debug.Log("just a friendly obj");
                break;
            case "Fuel":
                Debug.Log("got some fuel");
                break;
            case "Obstacle": 
                Debug.Log("Rocket blew up");
                CrashSequence();
                break;
        }
    }

    private void CrashSequence() {
        
        // Allow ZAxis Rotation
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX;
        
        // Plays explosion Clip if it's the first collision
        _audioSource.loop = false;
        _audioSource.mute = false;
        _audioSource.clip = explosionAudioClip;
        if (!HasRocketExploded) 
            _audioSource.Play();
        
        // Updates the state of the rocket
        HasRocketExploded = true;
        
        // Resets the level in 2 seconds (leaves a bit of time, but the player can't control anymore)
        Invoke("ResetLevel", 2f);
    }

    private void MoveRocket() {
        // Adds force relative on the object's coordinates instead of the world's coordinates (like transform.forward)
        if (!HasRocketExploded && IsThrusting)
            _rigidbody.AddRelativeForce(force: rocketEngineThrust * Time.deltaTime * MoveDirection);
    }
    
    private void RotateRocket() {
        
        /* Doesn't allow the physics engine to rotate the rocket while the player is rotating, but allows it in the Z when the player isn't controlling,
        So, when the rocket crashes against any obstacle, it will spin in the Z Axis, as long as the player is not controlling the rotation */
        if (IsRotatingLeft || IsRotatingRight)
            _rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        else 
            _rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        
        // z == 1 == left direction | z == -1 == right direction, the rocket can't rotate to left and right at the same time
        if (IsRotatingLeft) transform.Rotate(0, 0, 1 * rotationThrust * Time.deltaTime);
        else if (IsRotatingRight) transform.Rotate(0,0,-1 * rotationThrust * Time.deltaTime);
    }

    internal void ResetLevel() {
        
        // Kills the inertia for safety, makes sure it doesn't keep moving 
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        
        // Reloads the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    private void ProcessKeyboardInputs() {
        
        // Process thrust inputs
        if (Input.GetKey(KeyCode.Space)) IsThrusting = true;
        if (Input.GetKeyUp(KeyCode.Space)) IsThrusting = false;
        
        // Processes rotation inputs: can't rotate to left and right at the same time
        if (Input.GetKey(KeyCode.A)) IsRotatingLeft = true;
        else if (Input.GetKey(KeyCode.D)) IsRotatingRight = true;
        if (Input.GetKeyUp(KeyCode.A)) IsRotatingLeft = false;
        if (Input.GetKeyUp(KeyCode.D)) IsRotatingRight = false;
        
        // Processes reset inputs
        if (Input.GetKey(KeyCode.R)) ResetLevel();
    }

}

















