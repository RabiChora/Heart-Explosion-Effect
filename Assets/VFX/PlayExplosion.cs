using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayExplosion : MonoBehaviour
{
    [SerializeField] private Animator myAnimationController;
    private bool PressedMouse = false;
    private float timer = 0.0f;
    [SerializeField] public float waitingTime = 5.0f;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PressedMouse = true;
            Debug.Log("Pressed primary button.");

            myAnimationController.SetBool("PressedSpace", true);
        }
        timer += Time.deltaTime;
        //Debug.Log("time++");
        if (timer > waitingTime)
        {
            myAnimationController.SetBool("PressedSpace", false);
            timer = 0f;
        }

    }

    private void OnTriggerEnter()
    {
        if(PressedMouse)
        {
            myAnimationController.SetBool("PressedSpace", true);

        }
    }
}
