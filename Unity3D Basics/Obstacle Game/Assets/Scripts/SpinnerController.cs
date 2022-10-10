using UnityEngine;

public class SpinnerController : MonoBehaviour
{
    public float rotationSpeed = 2;
    private float _newYAngle;
    
    void Update()
    {
        _newYAngle += rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(new Vector3(0, _newYAngle, 0));
    }
}
