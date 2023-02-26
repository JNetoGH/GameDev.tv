using System;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum Difficulty: int
{
    Easy = 0,
    Mid = 1,
    Hard = 2
}

public sealed class RocketController : MonoBehaviour {
    
    // Movement and Rotation
    internal static Vector3 MoveDirection => Vector3.up;
    internal float rotationThrust = 5;
    [SerializeField] public float rocketEngineThrust = 1000;
    [SerializeField] public float airDrag = 0.25f; // same as air resistance, makes the rocket "slower"
    [SerializeField] public Vector3 gravity = new (0, -9.81f, 0); // default gravity
    
    // Rocket's Audio Clips
    [SerializeField] private AudioClip engineAudioClip;
    [SerializeField] private AudioClip explosionAudioClip;

    // Cached Components
    private Rigidbody _rigidbody;
    private AudioSource _audioSource;

    // States (rocket's current state)
    public Difficulty difficulty = Difficulty.Mid;
    internal static bool HasRocketExploded { get; private set; } = false;
    internal static bool HasWon { get; set; } = false;
    internal static bool IsThrusting { get; private set; }
    private static bool IsRotatingLeft { get; set; }
    private static bool IsRotatingRight { get; set; }

    private void Start() 
    {
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
        
        // Rigidbody and Physics
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.drag = airDrag;
        _rigidbody.angularVelocity = Vector3.zero;
        Physics.gravity = gravity;

        // Difficulty Mode
        switch (difficulty)
        {
            case Difficulty.Easy:
                _rigidbody.angularDrag = 0.05f;
                rotationThrust = 100;
                break;
            case Difficulty.Mid:
                _rigidbody.angularDrag = 0.30f;
                rotationThrust = 4;
                break;
            case Difficulty.Hard:
                _rigidbody.angularDrag = 0.1f;
                rotationThrust = 10f;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }

    private void Update() 
    {
        if (HasRocketExploded || HasWon) return;
        _audioSource.mute = !IsThrusting;
        ProcessKeyboardInputs();
    }

    private void FixedUpdate() 
    {
        if (HasRocketExploded || HasWon) return;
        MoveRocket();
        RotateRocket();
    }

    // Manages all the collisions that are not on the landing pad, there is a special script for managing this case.
    private void OnCollisionEnter(Collision collision)
    {
        // Simply ignores the collision if it’s the Landing Pad or whether the player has won and the next level is being loaded
        if (collision.gameObject.tag.Equals("LandingPad") || HasWon) return;
        switch (collision.gameObject.tag) {
            case "Friendly": Debug.Log("just a friendly obj"); break;
            case "Fuel": Debug.Log("got some fuel"); break;
            case "Obstacle": CrashSequence(); break;
        }
    }
    
    private void CrashSequence() 
    {
        //  Checks if it's the first collision
        if (HasRocketExploded) return;
        
        // Plays the explosion Clip
        _audioSource.loop = false;
        _audioSource.mute = false;
        _audioSource.clip = explosionAudioClip;
        _audioSource.Play();
        
        Debug.Log("Rocket blew up");
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX; // Allows Z axis rotation
        HasRocketExploded = true; // Updates the state of the rocket
        Invoke("ResetLevel", 2f); // Resets the level in 2 seconds (leaves a bit of time, but the player can't control anymore)
    }
    
    private void MoveRocket() 
    {   
        // Adds force relative on the object's coordinates instead of the world's coordinates (like transform.forward)
        if (!HasRocketExploded && IsThrusting) _rigidbody.AddRelativeForce(force: rocketEngineThrust * Time.deltaTime * MoveDirection);
    }
    
    private void RotateRocket()
    {
        switch (difficulty)
        {
            case Difficulty.Mid or Difficulty.Hard:
            {
                if (IsRotatingLeft)  _rigidbody.angularVelocity += new Vector3(0, 0,  1 * rotationThrust * Time.deltaTime);
                if (IsRotatingRight) _rigidbody.angularVelocity += new Vector3(0, 0, -1 * rotationThrust * Time.deltaTime);
                break;
            }
            case Difficulty.Easy:
            {
                // Doesn't allow the physics engine to rotate the rocket while the player is rotating, but allows it in the Z when the player isn't controlling,
                // So, when the rocket crashes against any obstacle, it will spin in the Z Axis, as long as the player is not controlling the rotation
                if (IsRotatingLeft || IsRotatingRight) _rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
                else _rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
                // z == 1 == left direction | z == -1 == right direction, the rocket can't rotate to left and right at the same time
                if (IsRotatingLeft) transform.Rotate(0, 0, 1 * rotationThrust * Time.deltaTime);
                else if (IsRotatingRight) transform.Rotate(0, 0, -1 * rotationThrust * Time.deltaTime);
                break;
            }
        }
    }

    internal void ResetLevel() 
    {
        // Kills the inertia for safety reasons, makes sure it doesn't keep moving 
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        // Reloads the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ProcessKeyboardInputs() 
    {
        IsThrusting = Input.GetKey(KeyCode.Space);
        IsRotatingLeft = Input.GetAxisRaw("Horizontal") < 0;
        IsRotatingRight = Input.GetAxisRaw("Horizontal") > 0;
        if (Input.GetKey(KeyCode.R)) ResetLevel();
    }
    
}
