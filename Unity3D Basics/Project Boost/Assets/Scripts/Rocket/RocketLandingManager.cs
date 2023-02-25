using UnityEngine;
using UnityEngine.SceneManagement;

public class RocketLandingManager : MonoBehaviour {
    
    // Winning Audio Clip
    [SerializeField] private AudioClip winningAudioClip;
    
    // Landing Validation
    [SerializeField] private Vector3 minRotLandingRange = new (-10,-10,-10);
    [SerializeField] private Vector3 maxRotLandingRange = new (10, 10, 10);
    
    // Scene Loading
    private int _currentSceneIndex = 0;
    private int _nextSceneIndex = 0;

    // Cached Components
    private RocketController _mainScript;
    private AudioSource _audioSource;
    private Rigidbody _rigidbody;

    // States (rocket's landing current state)
    private static bool IsLandingValid => RocketController.HasRocketExploded || RocketController.HasWon;
    private bool IsStandingInRange {
        get {
            /* Checks if the rocket is standing over the landing pad in the rotation range,
            It's done with a range because there is always some noise on the rotation, it's never just 0 in all axis */
            Vector3 rotationNow = transform.rotation.eulerAngles;  // converts from current rotation from Quaternion to euler
            bool aboveMinRange = rotationNow.x > minRotLandingRange.x && rotationNow.y > minRotLandingRange.y && rotationNow.z > minRotLandingRange.z;
            bool underMaxRange = rotationNow.x < maxRotLandingRange.x && rotationNow.y < maxRotLandingRange.y && rotationNow.z < maxRotLandingRange.z;
            bool result = aboveMinRange && underMaxRange;
            return result;
        }
    }

    private void Start() {
        _mainScript = GetComponent<RocketController>();
        _audioSource = GetComponent<AudioSource>();
        _rigidbody = GetComponent<Rigidbody>();
    } 
    
    private void OnCollisionStay(Collision collisionInfo) {
      
        // Basically checks if the landing collision if valid
        if (!collisionInfo.gameObject.tag.Equals("LandingPad") || !IsStandingInRange || !IsLandingValid)
            return;
       
        Debug.Log("landed properly at the landing pad");

        // plays the winning sound
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
        
        // freezes the rocket at that position for until the next level is loaded
        _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        
        // Updates rocket's Has Won state
        RocketController.HasWon = true;
    }
    
    private void LoadNextLevel() => SceneManager.LoadScene(_nextSceneIndex);
    
}
