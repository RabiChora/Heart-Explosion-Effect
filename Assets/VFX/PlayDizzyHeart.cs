using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayDizzyHeart : MonoBehaviour
{
    public ParticleSystem heartDizzyParticle;
    void OnEnableDizzy()
    {
        heartDizzyParticle.Play();
        //Debug.Log("StarOn");
    }

    void OnDisableDizzy()
    {
        heartDizzyParticle.Stop();
        //Debug.Log("StarOff");
    }
}
