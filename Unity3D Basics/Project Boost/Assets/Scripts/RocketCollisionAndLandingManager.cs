using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class RocketCollisionAndLandingManager : MonoBehaviour {
   
    public Vector3 minRotationRangeForLanding = new Vector3(-1,-1,-1);
    public Vector3 maxRotationRangeForLanding = new Vector3(1, 1, 1);
    
    private GameObject _rocket;
    private RocketMovement _rocketMovementScript;
    
    private void Start() {
        _rocket = GameObject.Find("Rocket");
        _rocketMovementScript = _rocket.GetComponent<RocketMovement>();
    }

    // manages all the other collisions that are not the landing pad
    private void OnCollisionEnter(Collision collision) {
        switch (collision.gameObject.tag) {
            
            case "Friendly":
                Debug.Log("just a friendly obj");
                break;
            
            case "Fuel":
                Debug.Log("got some fuel");
                break;
            
            default: // if its not the landingPad, its something that kills the rocket
                if (!collision.gameObject.tag.Equals("Landing Pad")) {
                    Debug.Log("Rocket blew up");
                    RocketMovement.IsGonnaResetPos = true;
                }
                break;
        }
    }

    // basically checks the landing collision
    private void OnCollisionStay(Collision collisionInfo) {
        switch (collisionInfo.gameObject.tag) {
            case "LandingPad" when IsStandingProperly():
                RocketMovement.IsGonnaResetPos = true;
                break;
        }
    }
    
    // Check if the rocket is standing over the landing pad inside the rotation range
    // It was done with a range because there is always some noise on the rotation, it's never just 0 in all axis
    private bool IsStandingProperly() {
        // converts from current rotation from Quaternion to euler
        Vector3 rotationNow = transform.rotation.eulerAngles;
        bool aboveMinRange = rotationNow.x > minRotationRangeForLanding.x && rotationNow.y > minRotationRangeForLanding.y && rotationNow.z > minRotationRangeForLanding.z;
        bool underMaxRange = rotationNow.x < maxRotationRangeForLanding.x && rotationNow.y < maxRotationRangeForLanding.y && rotationNow.z < maxRotationRangeForLanding.z;
        bool result = aboveMinRange && underMaxRange;
        return result;
    }
}
