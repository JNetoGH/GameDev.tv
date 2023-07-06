using System;
using UnityEngine;
using UnityEngine.SceneManagement;


public sealed class RocketController : MonoBehaviour {
    
    public enum Difficulty: int {
        Easy = 0,
        Mid = 1,
        Hard = 2
    }

    [Header("Difficulty")]
    [SerializeField] public Difficulty difficultyLevel = Difficulty.Easy;
    
    [Header("Movement and Rotation")]
    [SerializeField] public float rocketEngineThrust = 1000;
    [SerializeField] public float airDrag = 0.25f; 
    [SerializeField] public Vector3 gravity = new (0, -9.81f, 0);
    public static Vector3 MoveDirection => Vector3.up;
    public float RotationThrust { get; set; } = 5;
    
    [Header("Rocket's Audio Clips")]
    [SerializeField] private AudioClip _engineAudioClip;
    [SerializeField] private AudioClip _explosionAudioClip;
    
    [Header("Particles")]
    [SerializeField] private ParticleSystem _thrustParticleSystem;
    [SerializeField] private ParticleSystem _leftSideThrustParticleSystem;
    [SerializeField] private ParticleSystem _rightSideThrustParticleSystem;
    [SerializeField] private ParticleSystem _crashParticleSystem;
    
    // Cached Components
    private Rigidbody _rigidbody;
    private AudioSource _audioSource;

    // States (rocket's current state)
    internal static bool HasRocketExploded { get; private set; }
    private static bool HasAlreadyPlayedExplosionParticles { get; set; }
    internal static bool HasWon { get; set; }
    internal static bool IsThrusting { get; private set; }
    private static bool IsRotatingLeft { get; set; }
    private static bool IsRotatingRight { get; set; }

    private void Start() {
        
        // Audio
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _engineAudioClip;
        _audioSource.loop = true;
        _audioSource.mute = false;
        _audioSource.Play();

        // Rigidbody and Physics
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.drag = airDrag;
        _rigidbody.angularVelocity = Vector3.zero;
        Physics.gravity = gravity;

        // States
        HasRocketExploded = false;
        HasAlreadyPlayedExplosionParticles = false;
        HasWon = false;
        IsThrusting = false;
        IsRotatingLeft = false;
        IsRotatingRight = false;
        
        ChangeDifficulty(difficultyLevel);
    }

    private void Update() {
        SyncParticles();
        if (HasRocketExploded || HasWon) return;
        _audioSource.mute = !IsThrusting;
        ProcessKeyboardInputs(); // disable to mobile version (just make it as comment)
    }

    private void FixedUpdate() {
        if (HasRocketExploded || HasWon) return;
        MoveRocket();
        RotateRocket();
    }

    // Manages all the collisions that are not on the landing pad, there is a special script for managing this case.
    private void OnCollisionEnter(Collision collision) {
        // Simply ignores the collision if it’s the Landing Pad or whether the player has won and the next level is being loaded
        if (collision.gameObject.tag.Equals("LandingPad") || HasWon) return;
        switch (collision.gameObject.tag) {
            case "Friendly": Debug.Log("just a friendly obj"); break;
            case "Fuel"    : Debug.Log("got some fuel"); break;
            case "Obstacle": CrashSequence(); break;
        }
    }

    private void MoveRocket() {
        if (!(IsThrusting && !HasRocketExploded && !HasWon)) return;
        // Relative to its own coordinates, not the world, like transform.up
        _rigidbody.AddRelativeForce(force: rocketEngineThrust * Time.deltaTime * MoveDirection); 
    }
    
    private void RotateRocket() {
        switch (difficultyLevel) {
            case Difficulty.Mid or Difficulty.Hard: {
                if (IsRotatingLeft)  _rigidbody.angularVelocity += new Vector3(0, 0,  1 * RotationThrust * Time.deltaTime);
                if (IsRotatingRight) _rigidbody.angularVelocity += new Vector3(0, 0, -1 * RotationThrust * Time.deltaTime);
                break;
            }
            case Difficulty.Easy: {
                // Doesn't allow the physics engine to rotate the rocket while the player is rotating, but allows it in the Z when the player isn't controlling,
                // So, when the rocket crashes against any obstacle, it will spin in the Z Axis, as long as the player is not controlling the rotation
                if (IsRotatingLeft || IsRotatingRight) _rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
                else _rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
                if (IsRotatingLeft) transform.Rotate(0, 0, 1 * RotationThrust * Time.deltaTime);
                else if (IsRotatingRight) transform.Rotate(0, 0, -1 * RotationThrust * Time.deltaTime);
                break;
            }
            default: throw new ArgumentOutOfRangeException();
        }
    }
    
    private void SyncParticles() {
        // Debug.Log($"{IsThrusting} {HasRocketExploded} {HasWon}");
        
        // Main Engine
        if (!IsThrusting || HasRocketExploded || HasWon) _thrustParticleSystem.Stop();
        else if (IsThrusting && !_thrustParticleSystem.isPlaying) _thrustParticleSystem.Play();
        
        // Left Engine Thrust (turns rocket right)
        if (!IsRotatingRight || HasRocketExploded || HasWon) _leftSideThrustParticleSystem.Stop();
        else if (IsRotatingRight && !_leftSideThrustParticleSystem.isPlaying) _leftSideThrustParticleSystem.Play();
        
        // Right Engine Thrust (turns rocket left)
        if (!IsRotatingLeft || HasRocketExploded || HasWon) _rightSideThrustParticleSystem.Stop();
        else if (IsRotatingLeft && !_rightSideThrustParticleSystem.isPlaying) _rightSideThrustParticleSystem.Play();
        
        // Crash Particles
        if (HasRocketExploded && !_crashParticleSystem.isPlaying && !HasAlreadyPlayedExplosionParticles) {
            _crashParticleSystem.Play();
            HasAlreadyPlayedExplosionParticles = true;
        }
    }
    
    private void CrashSequence() {
        //  Checks if it's the first collision
        if (HasRocketExploded) return;
        
        // Plays the explosion Clip
        _audioSource.loop = false;
        _audioSource.mute = false;
        _audioSource.clip = _explosionAudioClip;
        _audioSource.Play();

        Debug.Log("Rocket blew up");
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX; // Allows Z axis rotation
        HasRocketExploded = true; // Updates the state of the rocket
        Invoke("ResetLevel", 2f); // Resets the level in 2 seconds (leaves a bit of time, but the player can't control anymore)
    }
    
    public void ResetLevel() {
        // Kills the inertia for safety reasons, makes sure it doesn't keep moving 
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        // Reloads the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ProcessKeyboardInputs() {
        IsThrusting = Input.GetKey(KeyCode.Space);
        IsRotatingLeft = Input.GetAxisRaw("Horizontal") < 0;
        IsRotatingRight = Input.GetAxisRaw("Horizontal") > 0;
        if (Input.GetKey(KeyCode.R)) ResetLevel();
    }

    public void ChangeDifficulty(Difficulty newDifficulty) {
        difficultyLevel =  newDifficulty;
        switch (difficultyLevel) {
            case Difficulty.Easy:
                _rigidbody.angularDrag = 0.05f;
                RotationThrust = 100;
                break;
            case Difficulty.Mid:
                _rigidbody.angularDrag = 0.30f;
                RotationThrust = 4;
                break;
            case Difficulty.Hard:
                _rigidbody.angularDrag = 0.1f;
                RotationThrust = 10f;
                break;
            default: throw new ArgumentOutOfRangeException();
        }
    }
    
}
