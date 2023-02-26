using UnityEngine;
using UnityEngine.Serialization;

public class HittableObjController: MonoBehaviour
{
    private CoolDownTimer _coolDownTimer;
    [HideInInspector] public bool isThisHittableObjInCoolDown;
    [SerializeField] private int cooldownDurationInSeconds = 3;
    private static Material _regularMaterial;
    private static Material _hitMaterial;

    private void Awake()
    {
        _hitMaterial = Resources.Load<Material>("Materials/HitWall");
        _regularMaterial = Resources.Load<Material>("Materials/Wall");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isThisHittableObjInCoolDown && collision.gameObject.tag.Equals("Player"))
        {
            SwitchToHitMaterial();
            _coolDownTimer.StartCoolDownTimer(cooldownDurationInSeconds);
            isThisHittableObjInCoolDown = true;
        }
    }

    private void Update()
    {
        if (_coolDownTimer.HasTimerBeenInit && _coolDownTimer.HasCoolDownFinished)
        {
            SwitchToRegularMaterial();
            _coolDownTimer.ResetCoolDownTimer();
            isThisHittableObjInCoolDown = false;
        }
    }
    
    private void SwitchToRegularMaterial() => GetComponent<Renderer>().material = _regularMaterial;
    private void SwitchToHitMaterial() => GetComponent<MeshRenderer>().material = _hitMaterial;
}

