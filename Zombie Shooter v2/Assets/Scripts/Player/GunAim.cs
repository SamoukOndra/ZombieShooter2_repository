using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class GunAim : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera aimCam;
    //private Animator animator;

    bool isAiming;

    private void Start()
    {
        aimCam.enabled = false;
        //animator = GetComponentInChildren<Animator>();
    }

    public void OnAltFire(InputValue value)
    {
        isAiming = value.isPressed;
        aimCam.enabled = isAiming;
        if (isAiming)
        {
            
        }
    }
}
