using UnityEngine;

public class FallingCubeController : MonoBehaviour
{
    public float timeForDroppingInSeconds = 2;
    private MeshRenderer _meshRenderer;
    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }
    
    private void Update()
    {
        DropCube(Time.time >= timeForDroppingInSeconds);
    } 
    
    private void DropCube(bool drop)
    {
        _meshRenderer.enabled = drop;
        _rigidbody.useGravity = drop;
    }
}
