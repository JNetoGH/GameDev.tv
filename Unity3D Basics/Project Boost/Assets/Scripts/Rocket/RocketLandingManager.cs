using UnityEngine;
using UnityEngine.SceneManagement;

public class RocketLandingManager : MonoBehaviour {
    
    public Vector3 minRotationLandingRange = new Vector3(-1,-1,-1);
    public Vector3 maxRotationLandingRange = new Vector3(1, 1, 1);
    private RocketMovement _mainScript;

    private void Start() => _mainScript = GetComponent<RocketMovement>();

    // Basically checks the landing collision
    private void OnCollisionStay(Collision collisionInfo) {
        if (!collisionInfo.gameObject.tag.Equals("LandingPad") || !IsStandingProperly())
            return;

        Debug.Log("landed properly at the landing pad");
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = ++currentSceneIndex;

        bool isTheLastScene = nextSceneIndex + 1 > SceneManager.sceneCountInBuildSettings;
        if (isTheLastScene) _mainScript.ResetRocketBackToInitialPosition();
        else SceneManager.LoadScene(nextSceneIndex);
    }
    
    // Check if the rocket is standing over the landing pad inside the rotation range
    // It was done with a range because there is always some noise on the rotation, it's never just 0 in all axis
    private bool IsStandingProperly() {
        // converts from current rotation from Quaternion to euler
        Vector3 rotationNow = transform.rotation.eulerAngles;
        bool aboveMinRange = rotationNow.x > minRotationLandingRange.x && rotationNow.y > minRotationLandingRange.y && rotationNow.z > minRotationLandingRange.z;
        bool underMaxRange = rotationNow.x < maxRotationLandingRange.x && rotationNow.y < maxRotationLandingRange.y && rotationNow.z < maxRotationLandingRange.z;
        bool result = aboveMinRange && underMaxRange;
        return result;
    }
}
