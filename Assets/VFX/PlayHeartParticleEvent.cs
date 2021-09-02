using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayHeartParticleEvent : MonoBehaviour
{
    public ParticleSystem heartParticle;
    void OnEnableHeart()
    {
        heartParticle.Play();
        //Debug.Log("HeartOn");
    }

    void OnDisableHeart()
    {
        heartParticle.Stop();
        //Debug.Log("HeartOff");
    }
}