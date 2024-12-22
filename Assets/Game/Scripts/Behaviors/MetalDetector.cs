using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetalDetector : MonoBehaviour
{
    [SerializeField] private MeshRenderer _detector1Light;
    [SerializeField] private MeshRenderer _detector2Light;
    [SerializeField] private AudioClip _playAlarmSound;
    [SerializeField] private ParticleSystem _detectedParticles;

    private bool _alarmed = false;

    private void OnTriggerEnter(Collider other)
    {
        if(_alarmed)
        {
            return;
        }
        if(other.CompareTag("Player"))
        {
            Alarm();
        }
    }

    private void Alarm()
    {
        _detector1Light.material.color = Color.red;
        _detector2Light.material.color = Color.red;
        if(_detectedParticles != null)
            _detectedParticles.Play();

        _alarmed = true;
        
        if(_playAlarmSound != null)
            AudioSource.PlayClipAtPoint(_playAlarmSound, Camera.main.transform.position);

        AlarmManager.Instance.InvokeAlarm();
    }
}
