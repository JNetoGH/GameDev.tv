using UnityEngine;
using UnityEngine.SceneManagement;

public class RocketLandingManager : MonoBehaviour {
    
    // Scene Loading
    private int _currentSceneIndex = 0;
    private int _nextSceneIndex = 0;
    
    // Rotation
    [SerializeField] private Vector3 minRotationLandingRange = new Vector3(-10,-10,-10);
    [SerializeField] private Vector3 maxRotationLandingRange = new Vector3(10, 10, 10);

    // Winning Audio Clip
    [SerializeField] private AudioClip winningAudioClip;

    // Cached Components
    private RocketController _mainScript;
    private AudioSource _audioSource;
    private Rigidbody _rigidbody;

    private void Start() {
        _mainScript = GetComponent<RocketController>();
        _audioSource = GetComponent<AudioSource>();
        _rigidbody = GetComponent<Rigidbody>();
    } 
    
    private void OnCollisionStay(Collision collisionInfo) {
      
        // Basically checks the landing collision
        if (!collisionInfo.gameObject.tag.Equals("LandingPad") || !IsStandingProperly() || RocketController.HasRocketExploded)
            return;
        
        Debug.Log("landed properly at the landing pad");
        
        // plays the winning sound
        _audioSource.clip = winningAudioClip;
        _audioSource.loop = false;
        _audioSource.mute = false;
        if (!RocketController.HasWon) _audioSource.Play();

        // Tries to load the next scene
        _currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        _nextSceneIndex = ++_currentSceneIndex;
        if (_nextSceneIndex + 1 > SceneManager.sceneCountInBuildSettings) _nextSceneIndex = _currentSceneIndex;
        if (!RocketController.HasWon) Invoke("LoadNextLevel", 1f);
        
        // freezes the rocket at that position for until the next level is loaded
        _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        
        // Updates rocket's state
        RocketController.HasWon = true;
    }
    
    // Check if the rocket is standing over the landing pad inside the rotation range, done with a range cuz there is always some noise on the rotation, it's never just 0 in all axis
    private bool IsStandingProperly() {
        Vector3 rotationNow = transform.rotation.eulerAngles;  // converts from current rotation from Quaternion to euler
        bool aboveMinRange = rotationNow.x > minRotationLandingRange.x && rotationNow.y > minRotationLandingRange.y && rotationNow.z > minRotationLandingRange.z;
        bool underMaxRange = rotationNow.x < maxRotationLandingRange.x && rotationNow.y < maxRotationLandingRange.y && rotationNow.z < maxRotationLandingRange.z;
        bool result = aboveMinRange && underMaxRange;
        return result;
    }
    
    private void LoadNextLevel() => SceneManager.LoadScene(_nextSceneIndex);
    
    
}
