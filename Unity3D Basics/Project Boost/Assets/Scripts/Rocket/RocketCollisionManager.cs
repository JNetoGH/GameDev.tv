using UnityEngine;

public sealed class RocketCollisionManager : MonoBehaviour {
    
    private RocketMovement _mainScript;
    private void Start() => _mainScript = GetComponent<RocketMovement>();
    
    // manages all the collisions that are not the landing pad
    private void OnCollisionEnter(Collision collision) {
        
        // Simply ignores the Landing Pads
        if (collision.gameObject.tag.Equals("LandingPad"))
            return;

        switch (collision.gameObject.tag) {
            case "Friendly":
                Debug.Log("just a friendly obj");
                break;
            case "Fuel":
                Debug.Log("got some fuel");
                break;
            default: 
                Debug.Log("Rocket blew up");
                _mainScript.ResetRocketBackToInitialPosition();
                break;
        }
    }
}
