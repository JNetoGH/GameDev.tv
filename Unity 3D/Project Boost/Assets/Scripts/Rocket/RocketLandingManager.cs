using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class RocketLandingManager : MonoBehaviour {
    
    [Header("Landing Validation")]
    [SerializeField] private Vector3 _minRotLandingRange = new (-10,-10,-10);
    [SerializeField] private Vector3 _maxRotLandingRange = new (10, 10, 10);
    
    [Header("Audio")]
    [SerializeField] private AudioClip _winningAudioClip;
    
    [Header("Particles")]
    [SerializeField] private ParticleSystem _successParticleSystem;
    
    // Scene Loading
    private int _currentSceneIndex = 0;
    private int _nextSceneIndex = 0;

    // Cached Components
    private AudioSource _audioSource;
    private Rigidbody _rigidbody;
    
    // States (rocket's landing current state)
    private static bool IsLandingValid => ! (RocketController.HasRocketExploded || RocketController.HasWon);
    private bool IsStandingInRange {   
        get {   
            // Checks if the rocket is over the Landing Pad within a tolerance range, there is always some noise on the rotation, it's never 0 in all axes.
            Vector3 rotNow = transform.rotation.eulerAngles;  // Quaternion => Euler
            bool aboveMinRange = rotNow.x > _minRotLandingRange.x && rotNow.y > _minRotLandingRange.y && rotNow.z > _minRotLandingRange.z;
            bool underMaxRange = rotNow.x < _maxRotLandingRange.x && rotNow.y < _maxRotLandingRange.y && rotNow.z < _maxRotLandingRange.z;
            bool result = aboveMinRange && underMaxRange;
            return result;
        }
    }
    
    private void Start() {
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
        _audioSource.clip = _winningAudioClip;
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
        _successParticleSystem.Play();
        
        // Updates rocket's HasWon state
        RocketController.HasWon = true;
    }
    
    private void LoadNextLevel() => SceneManager.LoadScene(_nextSceneIndex);
    
}
