using System.Text;
using UnityEngine;
using UnityEngine.Serialization;

public sealed class PlayerController : MonoBehaviour
{
    
    [SerializeField] public int speed = 10;
    public bool enableHitInWallCooldown; // allow the hits in walls in cooldown to be add to TotHits
    internal static int TotHits { get; set; }
    private Rigidbody _cachedRigidbody;
    public string hittableObjTag = "HittableObj";

    //----------------------------------------------------------------
    //  UNITY API METHODS
    //----------------------------------------------------------------
    
    private void Start() 
    {
        _cachedRigidbody = GetComponent<Rigidbody>();
        PrintInstructions();
        TotHits = 0;
    }

    private void FixedUpdate() => MovePlayer();
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals(hittableObjTag))
        {
            bool isThisWallOnCoolDown = collision.gameObject.GetComponent<HittableObjController>().isThisHittableObjInCoolDown;
            if (!isThisWallOnCoolDown || enableHitInWallCooldown)
                TotHits++;
        }
    }
    
    //----------------------------------------------------------------
    //  MY METHODS
    //----------------------------------------------------------------

    private static void PrintInstructions() 
    { 
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("Welcome to the game");
        stringBuilder.AppendLine("Move your player with WASD or arrow keys");
        stringBuilder.AppendLine("Don't hit the walls");
        Debug.Log(stringBuilder);
    }

    private void MovePlayer() 
    {
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        _cachedRigidbody.velocity = direction * speed;
    }
    
}
