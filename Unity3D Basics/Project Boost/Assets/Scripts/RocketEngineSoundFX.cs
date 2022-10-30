using UnityEngine;

public sealed class RocketEngineSoundFX : MonoBehaviour {
    
    // caching component
    private AudioSource _audioSource;

    // Start is called before the first frame update
    private void Start() {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.loop = true;
        _audioSource.Play();
    }

    // Update is called once per frame
    private void Update() {
        _audioSource.mute = !RocketMovement.IsThrusting;
    }

}