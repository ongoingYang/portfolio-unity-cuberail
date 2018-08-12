using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleScript : MonoBehaviour {


    ParticleSystem[] allParticles;

    public float lifeTime = 0.7f;
    private void Awake()
    {
        allParticles = GetComponentsInChildren<ParticleSystem>();
    }

    private void OnEnable()
    {
        foreach (ParticleSystem particleSys in allParticles)
        {
            particleSys.Play();
        }
    }
    private void OnDisable()
    {
        foreach (ParticleSystem particleSys in allParticles)
        {
            particleSys.Stop();
        }
 
    }
    public void TimeOut()
    {
        if(isActiveAndEnabled) gameObject.SetActive(false);
    }
}
