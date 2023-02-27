using UnityEngine;
using UnityEngine.SceneManagement;

public class RocketLandingManager : MonoBehaviour {
    
    // Winning Audio Clip
    [SerializeField] private AudioClip winningAudioClip;
    
    // Landing Validation
    [SerializeField] private Vector3 minRotLandingRange = new (-10,-10,-10);
    [SerializeField] private Vector3 maxRotLandingRange = new (10, 10, 10);
    
    // Particles
    [SerializeField] private ParticleSystem successParticleSystem;
    
    // Scene Loading
    private int _currentSceneIndex = 0;
    private int _nextSceneIndex = 0;

    // Cached Components
    private AudioSource _audioSource;
    private Rigidbody _rigidbody;
    
    // States (rocket's landing current state)
    private static bool HasAlreadyPlayedSuccessParticles { get; set; }
    private static bool IsLandingValid => ! (RocketController.HasRocketExploded || RocketController.HasWon);
    private bool IsStandingInRange {   
        get {   
            // Checks if the rocket is over the Landing Pad within a tolerance range, there is always some noise on the rotation, it's never 0 in all axes.
            Vector3 rotNow = transform.rotation.eulerAngles;  // Quaternion => Euler
            bool aboveMinRange = rotNow.x > minRotLandingRange.x && rotNow.y > minRotLandingRange.y && rotNow.z > minRotLandingRange.z;
            bool underMaxRange = rotNow.x < maxRotLandingRange.x && rotNow.y < maxRotLandingRange.y && rotNow.z < maxRotLandingRange.z;
            bool result = aboveMinRange && underMaxRange;
            return result;
        }
    }
    
    private void Start() {
        HasAlreadyPlayedSuccessParticles = false;
        _audioSource = GetComponent<AudioSource>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionStay(Collision collisionInfo) {
        // Basically, it just validates the landing
        if (!collisionInfo.gameObject.tag.Equals("LandingPad") || !IsStandingInRange || !IsLandingValid)
            return;
        SuccessSequence();
    }

    

    private void SuccessSequence() {
        
        Debug.Log("landed properly at the landing pad");
        
        // Plays the winning sound
        _audioSource.clip = winningAudioClip;
        _audioSource.loop = false;
        _audioSource.mute = false;
        _audioSource.Play();
       
        // Tries to load the next scene
        _currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        _nextSceneIndex = ++_currentSceneIndex;
        if (_nextSceneIndex + 1 > SceneManager.sceneCountInBuildSettings) 
            --_nextSceneIndex;
        Invoke("LoadNextLevel", 1f);
        
        // Freezes the rocket at that position for until the next level is loaded
        _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
       
        // Success Particles
        successParticleSystem.Play();
        
        // Updates rocket's HasWon state
        RocketController.HasWon = true;
    }
    
    private void LoadNextLevel() => SceneManager.LoadScene(_nextSceneIndex);
    
}
