using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayStarParticleEvent : MonoBehaviour
{
    public ParticleSystem starParticle;
    void OnEnableStar()
    {
        starParticle.Play();
        //Debug.Log("StarOn");
    }

    void OnDisableStar()
    {
        starParticle.Stop();
        //Debug.Log("StarOff");
    }
}
