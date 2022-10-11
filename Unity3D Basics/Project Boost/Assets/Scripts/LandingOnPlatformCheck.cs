using System;
using UnityEngine;

public sealed class LandingOnPlatformCheck : MonoBehaviour
{
    // it's rarely set to 0 in all axis rotation, so, i am adding a range
    public Vector3 minRange = new Vector3(-1,-1,-1);
    public Vector3 maxRange = new Vector3(1,1,1);
   
    private GameObject _rocketTrail;
    private RocketMovement _rocketMoveScript;
    
    private void Start() => _rocketMoveScript = GetComponent<RocketMovement>();
    
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Finish") && IsStandingProperly())
            _rocketMoveScript.SetRocketBackToInitialPosition();
    }

    private bool IsStandingProperly()
    {
        // converts from current rotation from Quaternion to euler
        Vector3 thisRotation = transform.rotation.eulerAngles;
        
        bool aboveMinRange = thisRotation.x > minRange.x && thisRotation.y > minRange.y && thisRotation.z > minRange.z;
        bool underMaxRange = thisRotation.x < maxRange.x && thisRotation.y < maxRange.y && thisRotation.z < maxRange.z;
        bool result = aboveMinRange && underMaxRange;
        
        return result;
    }
}
